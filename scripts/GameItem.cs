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
                    item = new GameItemMoveable();
                    break;
                case ItemTypeEnum.PLAYER:
                    item = new GameItemPlayer();
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
            }

            if (ItemState == ItemStateEnum.WALKING)
            {
                ((GameItemMoveable)(this)).Walking();
            }



        }


        //public void moveItem(Vector2I newPos)
        //{
        //    Chunk oldChunk = Map.GetChunk(Position);
        //    Chunk newChunk = Map.GetChunk(newPos);

        //    if (newChunk.Items.ContainsKey(newPos))
        //        return; // Can't move to occupied cell

        //    Map.ItemLayer.EraseCell(Position);
        //    Map.ItemLayer.SetCell(newPos, AtlasSourceId, AtlasCoord);

        //    oldChunk.Items.Remove(Position);
        //    newChunk.Items.Add(newPos, this);

        //    Position = newPos;
        //}

        //public virtual void DoAction()
        //{

        //}
    }

    public class GameItemMoveable : GameItem
    {
        public Vector2I? TargetPosition { get; set; }
        public GameItem TargetItem { get; set; }

        public void Walking()
        {
            if(ItemType == ItemTypeEnum.ANIMAL)
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
                if(TargetItem.ItemType == ItemTypeEnum.ANIMAL)
                    ItemState = ItemStateEnum.FIGHTING;

                if (TargetItem.ItemType == ItemTypeEnum.PLANT)
                    ItemState |= ItemStateEnum.WORKING;

                return;
            }

            //an Punkt x angekommen
            if(TargetPosition != null && TargetPosition == newPos)
            {
                ItemState = ItemStateEnum.IDLE;
            }

            Chunk oldChunk = Map.GetChunk(Position);
            Chunk newChunk = Map.GetChunk(newPos);

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

                if (Map.GetItem(Position + direction) == null)
                    return direction;
            }

            return Vector2I.Zero;
        }
        //public override Vector2I GetNextCoords()
        //{
        //    if(ItemType == ItemTypeEnum.PLAYER)
        //    {
        //        Vector2I? target = TargetItem != null ? TargetItem.Position : TargetPosition;

        //        if(target != null)
        //        {
        //            Vector2I direction = Vector2I.Zero;
        //            if(Position.X < target.Value.X) direction.X = 1;
        //            if(Position.X > target.Value.X) direction.X = -1;
        //            if(Position.Y < target.Value.Y) direction.Y = 1;
        //            if(Position.Y > target.Value.Y) direction.Y = -1;

        //            Vector2I newPos = Position + direction;
        //            if(newPos == target)
        //            {
        //                // Reached target
        //                TargetPosition = null;
        //                TargetItem = null;
        //                return Vector2I.MinValue;
        //            }
        //            return Position + direction;
        //        }
        //    }
        //    return Vector2I.MinValue;
        //}

        //public override void DoAction()
        //{

        //}
    }

    public class GameItemPlayer : GameItemMoveable
    {
    }
}
