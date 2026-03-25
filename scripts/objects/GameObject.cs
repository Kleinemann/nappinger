using Godot;
using System;
using System.Linq;

namespace nappinger.scripts
{
    public enum ObjectTypeEnum { NONE, PLANT, ANIMAL, BUILDING, NPC, PLAYER };
    public enum  ObjectStateEnum { NONE, IDLE, WALKING, FIGHTING, WORKING, DEAD};

    public class GameObject
    {
        internal WorldMap Map => WorldMain.Instance.Map;

        public int AtlasSourceId;
        public Vector2I AtlasCoord;
        public ObjectTypeEnum ObjectType { get; set; } = ObjectTypeEnum.NONE;
        public ObjectStateEnum ObjectState { get; set; } = ObjectStateEnum.NONE;
        public string ObjectName { get; set; }

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
                    ObjectState = ObjectStateEnum.DEAD;
                }
            }
        }
        public Vector2I Position { get; set; }

        public ImageTexture Icon
        {
            get
            {
                TileSetAtlasSource tss = Map.ObjectLayer.TileSet.GetSource(AtlasSourceId) as TileSetAtlasSource;
                Texture2D atlasTexture = tss.Texture;
                Rect2I region = tss.GetTileTextureRegion(AtlasCoord);

                Image atlasImage = atlasTexture.GetImage();
                Image tileImage = atlasImage.GetRegion(region);
                return ImageTexture.CreateFromImage(tileImage);
            }
        }

        public static GameObject NewGameItem(ObjectTypeEnum type, Vector2I pos, float noise)
        {
            GameObject go = null;

            float noisePart = 0f;
            float noisePart2 = 0f;
            Vector2I atlasCoord = Vector2I.Zero;

            switch (type)
            {
                case ObjectTypeEnum.PLANT:
                    go = new GameObject();
                    go.AtlasSourceId = 0;

                    noisePart = noise % 0.01f * 1000f;
                    noisePart2 = noise % 0.001f * 10000f;

                    if (noisePart >= 8)
                        atlasCoord.X = 0;
                    else if (noisePart >= 6)
                        atlasCoord.X = 1;
                    else if (noisePart >= 4)
                        atlasCoord.X = 2;
                    else if (noisePart >= 2)
                        atlasCoord.X = 3;
                    else
                        atlasCoord.X = 4;

                    if (noisePart2 > 5)
                        atlasCoord.Y = 0;
                    else if (noisePart2 >= 2)
                        atlasCoord.Y = 1;
                    else
                        atlasCoord.Y = 2;

                    go.AtlasCoord = atlasCoord;

                    break;

                case ObjectTypeEnum.ANIMAL:
                    go = new GameItemMoveable();
                    go.AtlasSourceId = 1;

                    noisePart = noise % 0.001f * 10000f;

                    if (noisePart >= 5)
                        atlasCoord.X = 0;
                    else
                        atlasCoord.X = 2;

                    go.AtlasCoord = atlasCoord;
                    go.ObjectState = ObjectStateEnum.IDLE;
                    break;

                case ObjectTypeEnum.BUILDING:
                    go = new GameObject();
                    break;

                case ObjectTypeEnum.NPC:
                    go = new NPCGameItem();
                    go.AtlasSourceId = 2;

                    noisePart = noise % 0.0001f * 100000f;
                    atlasCoord.X = (int)noisePart;

                    go.AtlasCoord = atlasCoord;
                    go.ObjectState = ObjectStateEnum.IDLE;
                    break;

                case ObjectTypeEnum.PLAYER:
                    go = new GameItemPlayer();
                    go.AtlasSourceId = 2;

                    noisePart = noise % 0.0001f * 100000f;
                    if (noisePart >= 5)
                        atlasCoord.X = 10;
                    else
                        atlasCoord.X = 12;

                    go.AtlasCoord = atlasCoord;
                    go.ObjectState = ObjectStateEnum.IDLE;
                    break;
            }
            if(go != null)
            {
                go.ObjectType = type;
                go.Position = pos;

                //Get Data From Tile;
                TileSetAtlasSource tss = WorldMain.Instance.Map.ObjectLayer.TileSet.GetSource(go.AtlasSourceId) as TileSetAtlasSource;
                TileData td = tss.GetTileData(go.AtlasCoord, 0);
                go.ObjectName = (string)td.GetCustomData("ItemName");
                go.Value = (int)td.GetCustomData("ItemValue");
            }
            return go;
        }


        public void Process()
        {
            //Tote Items entfernen
            if (ObjectState == ObjectStateEnum.DEAD)
            {
                if(Map.Marker.CurrentObject == this)
                    Map.Marker.Deselect();

                Map.GetChunk(Position).Objects.Remove(Position);
                Map.ObjectLayer.EraseCell(Position);
                return;
            }

            //Item wird nie etwas selbständig machen
            if (ObjectState == ObjectStateEnum.NONE)
                return;

            if(ObjectState == ObjectStateEnum.IDLE)
            {
                if (ObjectType == ObjectTypeEnum.ANIMAL)
                    ObjectState = ObjectStateEnum.WALKING;

                if(ObjectType == ObjectTypeEnum.NPC)
                    ((NPCGameItem)(this)).ProcessIdle();
            }

            if (ObjectState == ObjectStateEnum.WALKING)
            {
                ((GameItemMoveable)(this)).Walking();
            }

            if (ObjectState == ObjectStateEnum.WORKING)
            {
                ((GameItemMoveable)(this)).Work();
            }

            if (ObjectState == ObjectStateEnum.FIGHTING)
            {
                ((GameItemMoveable)(this)).ObjectState = ObjectStateEnum.FIGHTING;
                ((GameItemMoveable)(this)).Fight();
            }
        }
    }

    public class GameItemMoveable : GameObject
    {
        public Vector2I? TargetPosition { get; set; }
        public GameObject TargetItem { get; set; }

        public void Walking()
        {
            if(ObjectType == ObjectTypeEnum.ANIMAL || ObjectType == ObjectTypeEnum.NPC)
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
                if (TargetItem.ObjectType == ObjectTypeEnum.ANIMAL)
                {
                    ObjectState = ObjectStateEnum.FIGHTING;
                    GameItemMoveable target = (GameItemMoveable)TargetItem;
                    target.ObjectState = ObjectStateEnum.FIGHTING;
                    target.TargetItem = this;                    
                }

                if (TargetItem.ObjectType == ObjectTypeEnum.PLANT)
                    ObjectState = ObjectStateEnum.WORKING;

                return;
            }

            //an Punkt x angekommen
            if(TargetPosition != null && TargetPosition == newPos)
            {
                ObjectState = ObjectStateEnum.IDLE;
            }

            Chunk oldChunk = Map.GetChunk(Position);
            Chunk newChunk = Map.GetChunk(newPos);

            if (!Map.Chunks.Values.Contains(newChunk))
                return;

            Map.ObjectLayer.EraseCell(Position);
            Map.ObjectLayer.SetCell(newPos, AtlasSourceId, AtlasCoord);

            oldChunk.Objects.Remove(Position);
            newChunk.Objects.Add(newPos, this);

            Position = newPos;

            if(this is GameItemPlayer && newChunk.Drops.ContainsKey(Position))
            {
                ((GameItemPlayer)this).Collect(newChunk.Drops[Position]);
            }

            if(Map.Marker.CurrentObject != null && Map.Marker.CurrentObject == this)
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

                GameObject item = Map.GetItem(Position + direction);
                if (item == null || item==TargetItem)
                    return direction;
            }

            return Vector2I.Zero;
        }

        internal void Work()
        {
            TargetItem.Value = TargetItem.Value - 3;
            AddItem(1, 3);
            if (TargetItem.ObjectState == ObjectStateEnum.DEAD)
            {
                TargetItem.Process();
                SetIdle();
            }
            else
                ShowExplosion(TargetItem.Position);
            Ui.Instance.Update();
        }

        internal void Fight()
        {
            TargetItem.Value = TargetItem.Value - 1;
            AddItem(2);
            if (TargetItem.ObjectState == ObjectStateEnum.DEAD)
            {
                TargetItem.Process();
                SetIdle();
            }
            else
                ShowExplosion(TargetItem.Position);

            Ui.Instance.Update();
        }

        public void AddItem(int id, int value = 1)
        {
            Ui.Instance.ActionBar.AddItem(id, value);
        }

        void ShowExplosion(Vector2I coord)
        {
            PackedScene scene = GD.Load<PackedScene>("res://szenes/particles/explosion.tscn");
            Node2D node = scene.Instantiate<Node2D>();
            Map.AddChild(node);
            node.Position = (coord * Chunk.TileSize);
            node.Position += new Vector2I(Chunk.TileSize / 2, Chunk.TileSize / 2);

            Timer t = new Timer();
            Map.AddChild(t);
            t.WaitTime = 1f;
            t.OneShot = true;
            t.Timeout += () =>
            {
                node.QueueFree();
                t.QueueFree();
            };
            t.Start();
        }


        public void SetIdle()
        {
            ObjectState = ObjectStateEnum.IDLE;
            TargetPosition = null;
            TargetItem = null;
        }
    }


    public class GameItemPlayer : GameItemMoveable
    {
        public void Collect(DropItem dItem)
        {
            AddItem(dItem.Item.ID, dItem.Item.Count);
            dItem.QueueFree();
        }
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
                ObjectState = ObjectStateEnum.WALKING;
            }
            else
            {
                int r = WorldMain.Random.RandiRange(0, 2);
                if (r > 0)
                    waitingCount = WorldMain.Random.RandiRange(1, 2);
                else
                {
                    ObjectState = ObjectStateEnum.WALKING;
                    walkingCount = WorldMain.Random.RandiRange(1, 5);
                }
            }
        }
    }
}
