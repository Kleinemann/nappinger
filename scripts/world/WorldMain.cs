using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

public partial class WorldMain : Node2D
{
    [Export]
    public bool UseStartPosition = false;
    [Export]
    public Vector2 StartPosition;
    public static WorldMain Instance;
    public CameraScroll Camera;
    public WorldMap Map;
    public double Time = 0;

    //Ausgewähltes Object
    public static Node2D SelectedObject;
    public static Player SelectedPlayer => SelectedObject != null && SelectedObject is Player player ? player : null;
    public static Animal SelectedAnimal => SelectedObject != null && SelectedObject is Animal animal ? animal : null;
    public static BreakableObject SelectedBreakable => SelectedObject != null && SelectedObject is BreakableObject breakable ? breakable : null;
    public static Store SelectedStore => SelectedObject != null && SelectedObject is Store store ? store : null;

    public static RandomNumberGenerator Random = new RandomNumberGenerator();

    public static AudioStreamPlayer SoundPlayer;

    public override void _Ready()
    {
        Instance = this;

        Random.Randomize();

        Map = GetNode<WorldMap>("DualTileMap");
        Camera = GetNode<CameraScroll>("Camera2D");
        SoundPlayer = GetNode<AudioStreamPlayer>("SoundPlayer");

        Vector2 v2 = UseStartPosition ? StartPosition : TOOLS.GetGeoLocation();

        Vector2I coord = new Vector2I((int)v2.X * 10, (int)v2.Y * 10);
        Camera.Position = Map.CoordsToPosition(coord);

        Map.UpdateMap();

        InitPlayerStartup();
    }

    public void InitPlayerStartup()
    {
        if (FileAccess.FileExists($"user://Player//save.dat"))
        {
            LoadPlayer();
            return;
        }

        PackedScene scene = GD.Load<PackedScene>("res://szenes/buildings/camp.tscn");
        Store camp = scene.Instantiate<Store>();

        InventoryItem item = ResourceLoader.Load<InventoryItem>("res://resources/items/food.tres");
        camp.Inventory.Slots[0].Item = item;
        camp.Inventory.Slots[0].Amount = 30;

        camp.Inventory.Slots[1].Item = InventoryItem.CreateInventoryItem("ore");
        camp.Inventory.Slots[1].Amount = 11;

        camp.Position = Camera.Position;
        Map.AddChild(camp);

        //select first player
        SelectedObject = Player.GetNextPlayer();
    }


    public void LoadPlayer()
    {
        FileAccess file = FileAccess.Open($"user://Player//save.dat", FileAccess.ModeFlags.Read);

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        string data = file.GetAsText();

        PlayerData playerData = JsonSerializer.Deserialize<PlayerData>(data, options);
        Store first = null;

        foreach(StoreData storeData in playerData.Stores)
        {
            PackedScene scene = GD.Load<PackedScene>(storeData.SceneFilePath);
            Store store = scene.Instantiate<Store>();

            foreach(InvetoryItemData iid in storeData.Inventory)
            {
                InventoryItem item = InventoryItem.CreateInventoryItem(iid.ResourceName);
                store.Inventory.Insert(item, iid.Amount);
            }

            store.Position = (Vector2)storeData.Coords;
            Map.AddChild(store);

            if (first == null)
            {
                first = store;
                Camera.Position = store.Position;
            }
        }

        foreach (PlayerAnimalData playerDataItem in playerData.Players)
        {
            PackedScene scene = GD.Load<PackedScene>("res://szenes/character/player.tscn");
            Player player = scene.Instantiate<Player>();
            player.ObjectName = playerDataItem.ObjectName;
            player.Position = playerDataItem.Position;
            player.Icon = GD.Load<Texture2D>(playerDataItem.IconPath);
            player.Speed = playerDataItem.Speed;
            player.Healt = playerDataItem.Healt;
            player.MaxHealt = playerDataItem.MaxHealt;
            
            if(playerDataItem.MissionString != null)
                player.Mission = new Mission(GameObjectState.FARMING, playerDataItem.MissionString);

            foreach (InvetoryItemData iid in playerDataItem.Inventory)
            {
                InventoryItem item = InventoryItem.CreateInventoryItem(iid.ResourceName);
                player.Inventory.Insert(item, iid.Amount);
            }
            Map.AddChild(player);
        }

        SelectedObject = Player.GetNextPlayer();
    }


    public void SavePlayer()
    {
        Dictionary<Type, List<Node2D>> nodes = GetNodesInChunk(new Type[] { typeof(Player), typeof(Store) });

        PlayerData playerData = new PlayerData();

        foreach (Type t in nodes.Keys)
        {
            //Stores
            if(t == typeof(Store))
            {
                foreach (Store store in nodes[t])
                {
                    StoreData data = new StoreData()
                    {
                        Coords = (Vector2I)store.Position,
                        SceneFilePath = store.SceneFilePath,
                    };

                    foreach (InventorySlot slot in store.Inventory.Slots)
                    {
                        if (slot.Item != null && slot.Amount > 0)
                        {
                            InvetoryItemData itemData = new InvetoryItemData()
                            {
                                ResourceName = slot.Item.GroupName,
                                Amount = slot.Amount
                            };
                            data.Inventory.Add(itemData);
                        }
                    }
                    playerData.Stores.Add(data);
                }
            }            
            //PlayerCharakters
            else if(t == typeof(Player))
            {
                foreach (Player player in nodes[t])
                {
                    PlayerAnimalData p = new PlayerAnimalData()
                    {
                        ObjectName = player.ObjectName,
                        Position = player.Position,
                        IconPath = player.Icon.ResourcePath,
                        Speed = player.Speed,
                        Healt = player.Healt,
                        MaxHealt = player.MaxHealt,
                        State = GameObjectState.IDLE,
                        MissionString = player.Mission.State == GameObjectState.FARMING ? (string)player.Mission.Target : null
                    };

                    foreach (InventorySlot slot in player.Inventory.Slots)
                    {
                        if (slot.Item != null && slot.Amount > 0)
                        {
                            InvetoryItemData itemData = new InvetoryItemData()
                            {
                                ResourceName = slot.Item.GroupName,
                                Amount = slot.Amount
                            };
                            p.Inventory.Add (itemData);
                        }
                    }
                    playerData.Players.Add(p);
                }
            }
            // Wrong type
            else
            {
                GD.PrintErr("Unknown Type: " + t);
            }
        }

        FileAccess file = FileAccess.Open($"user://Player//save.dat", FileAccess.ModeFlags.Write);

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        string json = JsonSerializer.Serialize(playerData, options);
        file.StoreLine(json);

        GD.Print("Saving Player Data");
    }


    public Dictionary<Type, List<Node2D>> GetNodesInChunk(Type[] types)
    {
        Dictionary<Type, List<Node2D>> nodes = new Dictionary<Type, List<Node2D>>();

        foreach (Node2D node in Map.GetChildren())
        {
            if (!types.Contains(node.GetType()))
                continue;

            Type t = node.GetType();
            if (!nodes.ContainsKey(t))
                nodes.Add(t, new List<Node2D>());

            nodes[t].Add(node);
        }

        return nodes;
    }


    public override void _Process(double delta)
    {
        Hud.Instance.UpdateHud();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //Select next player
        if (@event.IsActionPressed("next_player"))
        {
            Player p = Player.GetNextPlayer();
            if (p != null)
            {
                SelectedObject = p;
                Camera.CameraTarget = p;
            }
        }

        //Quit
        if (@event.IsActionPressed("ui_cancel"))
        {
            SavePlayer();
            GetTree().Quit();
        }

        //Focus / defocus Player
        if (@event.IsActionPressed("camera_focus"))
        {
            Camera.SwitchFocus();
        }

        //Show Position
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            Vector2 posMouse = WorldMain.Instance.Camera.GetGlobalMousePosition();
            Vector2I posWorld = Map.WorldLayer.LocalToMap(posMouse);
            Vector2I posBuilding = Map.BuildingFloor.LocalToMap(posMouse);
            GD.Print("Mouse Position: " + posMouse);
            GD.Print("World Position: " + posWorld);
            GD.Print("Build Position: " + posBuilding);
        }

            //Deselect 
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("WORLD CLICK");
            WorldMain.SelectedObject = null;

            BuildMenu.Instance?.CreateBuildItem();
        }

        //targeting
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        { 
            Player player = SelectedPlayer;
            if (player != null) {
                player.SetTarget(Map.GetGlobalMousePosition());
                player.State = GameObjectState.IDLE;
                player.Mission = null;
            }
        }

        if (@event.IsActionPressed("add_value"))
        {
            if(SelectedAnimal != null)
                SelectedAnimal.Healt++;
            else if(SelectedBreakable != null)
                SelectedBreakable.Healt++;
        }

        if (@event.IsActionPressed("remove_value"))
        {
            if(SelectedAnimal != null)
                SelectedAnimal.Healt--;
            else if(SelectedBreakable != null)
                SelectedBreakable.Healt--;
        }

        if(@event.IsActionPressed("ControlCenter"))
        {
            var nodes = GetTree().GetNodesInGroup("Storable");
            //TODO: dound the nearest
            if (nodes.Count > 0)
                WorldMain.SelectedObject = (Node2D)nodes[0];

            Hud.Instance.SwitchPlayerControlCenter();
        }

        if(@event.IsActionPressed("build_menu"))
        {
            Hud.Instance.SwitchBuildMenu();
        }

        @event.Dispose();        
    }

    public static void PlaySound(AudioStream sound)
    {
        SoundPlayer.Stream = sound;
        SoundPlayer.Play();
    }
}

