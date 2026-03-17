using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nappinger.scripts
{
    public enum ItemTypeEnum { NONE, PLANT, ANIMAL, BUILDING, NPC, PLAYER };
    public enum  ItemStateEnum { NONE, IDLE, WALKING, FIGHTING, WORKING, DEAD};

    public class GameItem
    {
        internal WorldMap Map => WorldMain.Instance.Map;

        public int AtlasSourceId;
        public Vector2I AtlasCoord;
        public ItemTypeEnum ItemType { get; set; } = ItemTypeEnum.NONE;
        public ItemStateEnum ItemState { get; set; } = ItemStateEnum.NONE;
        public string ItemName { get; set; }

        int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                if (_value <= 0)
                {
                    _value = 0;
                    ItemState = ItemStateEnum.DEAD;
                }
            }
        }
        public Vector2I Position { get; set; }

        public ImageTexture Icon
        {
            get
            {
                TileSetAtlasSource tss = Map.ItemLayer.TileSet.GetSource(AtlasSourceId) as TileSetAtlasSource;
                Texture2D atlasTexture = tss.Texture;
                Rect2I region = tss.GetTileTextureRegion(AtlasCoord);

                Image atlasImage = atlasTexture.GetImage();
                Image tileImage = atlasImage.GetRegion(region);
                return ImageTexture.CreateFromImage(tileImage);
            }
        }

        public static GameItem NewGameItem(ItemTypeEnum type, Vector2I pos, float noise)
        {
            GameItem item = null;

            float noisePart = 0f;
            Vector2I atlasCoord = Vector2I.Zero;

            switch (type)
            {
                case ItemTypeEnum.PLANT:
                    item = new GameItem();
                    item.AtlasSourceId = 0;

                    noisePart = noise % 0.01f * 1000f;
                    
                    if (noisePart >= 8)
                        atlasCoord.X = 0;
                    else if (noisePart >= 5)
                        atlasCoord.X = 1;
                    else if (noisePart >= 3)
                        atlasCoord.X = 2;
                    else if (noisePart >= 1)
                        atlasCoord.X = 3;
                    else
                        atlasCoord.X = 4;

                    item.AtlasCoord = atlasCoord;

                    break;

                case ItemTypeEnum.ANIMAL:
                    item = new GameItemMoveable();
                    item.AtlasSourceId = 1;

                    noisePart = noise % 0.001f * 10000f;

                    if (noisePart >= 5)
                        atlasCoord.X = 0;
                    else
                        atlasCoord.X = 2;

                    item.AtlasCoord = atlasCoord;
                    item.ItemState = ItemStateEnum.IDLE;
                    break;

                case ItemTypeEnum.BUILDING:
                    item = new GameItem();
                    break;

                case ItemTypeEnum.NPC:
                    item = new NPCGameItem();
                    item.AtlasSourceId = 2;

                    noisePart = noise % 0.0001f * 100000f;
                    atlasCoord.X = (int)noisePart;

                    item.AtlasCoord = atlasCoord;
                    item.ItemState = ItemStateEnum.IDLE;
                    break;

                case ItemTypeEnum.PLAYER:
                    item = new GameItemPlayer();
                    item.AtlasSourceId = 2;

                    noisePart = noise % 0.0001f * 100000f;
                    if (noisePart >= 5)
                        atlasCoord.X = 10;
                    else
                        atlasCoord.X = 12;

                    item.AtlasCoord = atlasCoord;
                    item.ItemState = ItemStateEnum.IDLE;
                    break;
            }
            if(item != null)
            {
                item.ItemType = type;
                item.Position = pos;

                //Get Data From Tile;
                TileSetAtlasSource tss = WorldMain.Instance.Map.ItemLayer.TileSet.GetSource(item.AtlasSourceId) as TileSetAtlasSource;
                TileData td = tss.GetTileData(item.AtlasCoord, 0);
                item.ItemName = (string)td.GetCustomData("ItemName");
                item.Value = (int)td.GetCustomData("ItemValue");
            }
            return item;
        }


        public void Process()
        {
            //Tote Items entfernen
            if (ItemState == ItemStateEnum.DEAD)
            {
                if(Map.Marker.CurrentItem == this)
                    Map.Marker.Deselect();

                Map.GetChunk(Position).Items.Remove(Position);
                Map.ItemLayer.EraseCell(Position);
                return;
            }

            //Item wird nie etwas selbständig machen
            if (ItemState == ItemStateEnum.NONE)
                return;

            if(ItemState == ItemStateEnum.IDLE)
            {
                if (ItemType == ItemTypeEnum.ANIMAL)
                    ItemState = ItemStateEnum.WALKING;

                if(ItemType == ItemTypeEnum.NPC)
                    ((NPCGameItem)(this)).ProcessIdle();
            }

            if (ItemState == ItemStateEnum.WALKING)
            {
                ((GameItemMoveable)(this)).Walking();
            }

            if (ItemState == ItemStateEnum.WORKING)
            {
                ((GameItemMoveable)(this)).Work();
            }

            if (ItemState == ItemStateEnum.FIGHTING)
            {
                ((GameItemMoveable)(this)).ItemState = ItemStateEnum.FIGHTING;
                ((GameItemMoveable)(this)).Fight();
            }
        }
    }

    public class GameItemMoveable : GameItem
    {
        public Vector2I? TargetPosition { get; set; }
        public GameItem TargetItem { get; set; }

        public void Walking()
        {
            if(ItemType == ItemTypeEnum.ANIMAL || ItemType == ItemTypeEnum.NPC)
            {
                RandomTarget();
            }

            Vector2I? step = GetNextStep();

            //Abbruch wenn kein Step gemacht wird
            if (step == null || step == Vector2I.Zero)
                return;

            Vector2I newPos = Position + (Vector2I)step;

            //Ziel Item erreicht
            if(TargetItem != null && TargetItem.Position == newPos)
            {
                if (TargetItem.ItemType == ItemTypeEnum.ANIMAL)
                {
                    ItemState = ItemStateEnum.FIGHTING;
                    GameItemMoveable target = (GameItemMoveable)TargetItem;
                    target.ItemState = ItemStateEnum.FIGHTING;
                    target.TargetItem = this;                    
                }

                if (TargetItem.ItemType == ItemTypeEnum.PLANT)
                    ItemState = ItemStateEnum.WORKING;

                return;
            }

            //an Punkt x angekommen
            if(TargetPosition != null && TargetPosition == newPos)
            {
                ItemState = ItemStateEnum.IDLE;
            }

            Chunk oldChunk = Map.GetChunk(Position);
            Chunk newChunk = Map.GetChunk(newPos);

            if (!Map.Chunks.Values.Contains(newChunk))
                return;

            Map.ItemLayer.EraseCell(Position);
            Map.ItemLayer.SetCell(newPos, AtlasSourceId, AtlasCoord);

            oldChunk.Items.Remove(Position);
            newChunk.Items.Add(newPos, this);

            Position = newPos;

            if(Map.Marker.CurrentItem != null && Map.Marker.CurrentItem == this)
                Map.Marker.Update();
        }


        public void RandomTarget()
        {
            int x = WorldMain.Random.RandiRange(-1, 1);
            int y = WorldMain.Random.RandiRange(-1, 1);
            Vector2I newPos = new Vector2I(Position.X + x, Position.Y + y);
            if(Map.GetItem(newPos) == null)
                TargetPosition = newPos;
            else
                TargetPosition = null;
        }

        public Vector2I GetNextStep()
        {
            //setze das nächste Ziel
            Vector2I? target = null;
            if (TargetItem != null)
                target = TargetItem.Position;
            else if (TargetPosition != null)
                target = TargetPosition;

            //Nächste Schritt zum Ziel
            if (target != null)
            {
                Vector2I direction = Vector2I.Zero;
                if (Position.X < target.Value.X) direction.X = 1;
                if (Position.X > target.Value.X) direction.X = -1;
                if (Position.Y < target.Value.Y) direction.Y = 1;
                if (Position.Y > target.Value.Y) direction.Y = -1;

                GameItem item = Map.GetItem(Position + direction);
                if (item == null || item==TargetItem)
                    return direction;
            }

            return Vector2I.Zero;
        }

        internal void Work()
        {
            TargetItem.Value = TargetItem.Value - 3;
            if (TargetItem.ItemState == ItemStateEnum.DEAD)
                ItemState = ItemStateEnum.IDLE;
            else
            {
                PackedScene scene = GD.Load<PackedScene>("res://szenes/particles/explosion.tscn");
                Node2D node = scene.Instantiate<Node2D>();
                Map.AddChild(node);
                node.Position = (TargetItem.Position * Chunk.TileSize);
                node.Position += new Vector2I(Chunk.TileSize / 2, Chunk.TileSize / 2);

                Timer t = new Timer();
                Map.AddChild(t);
                t.WaitTime = 0.9f;
                t.OneShot = true;
                t.Timeout += () =>
                {
                    node.QueueFree();
                    t.QueueFree();
                };
                t.Start();
            }
            Ui.Instance.Update();
        }

        internal void Fight()
        {
            TargetItem.Value = TargetItem.Value - 1;
            if (TargetItem.ItemState == ItemStateEnum.DEAD)
                ItemState = ItemStateEnum.IDLE;
            else
            {
                PackedScene scene = GD.Load<PackedScene>("res://szenes/particles/explosion.tscn");
                Node2D node = scene.Instantiate<Node2D>();
                Map.AddChild(node);
                node.Position = (TargetItem.Position * Chunk.TileSize);
                node.Position += new Vector2I(Chunk.TileSize / 2, Chunk.TileSize / 2);

                Timer t = new Timer();
                Map.AddChild(t);
                t.WaitTime = 0.9f;
                t.OneShot = true;
                t.Timeout += () =>
                {
                    node.QueueFree();
                    t.QueueFree();
                };
                t.Start();
            }

            Ui.Instance.Update();
        }
    }


    public class GameItemPlayer : GameItemMoveable
    {
    }


    public class NPCGameItem : GameItemMoveable
    {
        int waitingCount = 0;
        int walkingCount = 0;

        public void ProcessIdle()
        {
            if (waitingCount > 0)
            {
                waitingCount--;
            }
            else if (walkingCount > 0)
            {
                walkingCount--;
                ItemState = ItemStateEnum.WALKING;
            }
            else
            {
                int r = WorldMain.Random.RandiRange(0, 2);
                if (r > 0)
                    waitingCount = WorldMain.Random.RandiRange(1, 2);
                else
                {
                    ItemState = ItemStateEnum.WALKING;
                    walkingCount = WorldMain.Random.RandiRange(1, 5);
                }
            }
        }
    }
}
