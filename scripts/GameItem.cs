using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nappinger.scripts
{
    public enum ItemType { NONE, PLANT, ANIMAL, BUILDING, NPC, PLAYER };
    public enum  ItemState { NONE, WALKING, FIGHTING, WORKING};

    public class GameItem
    {
        WorldMap Map => WorldMain.Instance.Map;

        int AtlasSourceId;
        Vector2I AtlasCoord;
        public ItemType Type { get; set; } = ItemType.NONE;
        public ItemState State { get; set; } = ItemState.NONE;
        public string Name { get; set; }
        public int Value { get; set; }
        public Vector2I Position { get; set; }

        public Vector2I ChunkCoord {  get; set; }


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

        public static GameItem NewGameItem(Vector2I pos)
        {
            //Position = pos;
            int atlasSourceId = WorldMain.Instance.Map.ItemLayer.GetCellSourceId(pos);
            Vector2I atlasCoord = WorldMain.Instance.Map.ItemLayer.GetCellAtlasCoords(pos);
            TileData data = WorldMain.Instance.Map.ItemLayer.GetCellTileData(pos);

            string name = (string)data.GetCustomData("ItemName");
            ItemType type = (ItemType)((int)data.GetCustomData("ItemType"));
            int value = (int)data.GetCustomData("ItemValue");

            GameItem item;

            if(type == ItemType.PLAYER)
            {
                item = new GameItemMoveable();
                (item as GameItemMoveable).TargetPosition = pos;
            }
            else item = new GameItem();

            item.Position = pos;
            item.AtlasSourceId = atlasSourceId;
            item.AtlasCoord = atlasCoord;
            item.Name = name;
            item.Type = type;
            item.Value = value;
            item.ChunkCoord = WorldMain.Instance.Map.GetChunkCoords(pos);

            return item;
        }


        public void Process()
        {
            if (State == ItemState.NONE)
                return;

            Vector2I newCoords = GetNextCoords();
            if(newCoords != Vector2I.MinValue)
            {
                moveItem(newCoords);
            }

            DoAction();
        }


        public virtual Vector2I GetNextCoords()
        {
            if(Type == ItemType.ANIMAL)
            {
                int x = WorldMain.Random.RandiRange(-1, 1);
                int y = WorldMain.Random.RandiRange(-1, 1);
                return new Vector2I(Position.X + x, Position.Y + y);
            }
            return Vector2I.MinValue;
        }

        public void moveItem(Vector2I newPos)
        {
            Chunk oldChunk = Map.GetChunk(Position);
            Chunk newChunk = Map.GetChunk(newPos);

            if (newChunk.Items.ContainsKey(newPos))
                return; // Can't move to occupied cell

            Map.ItemLayer.EraseCell(Position);
            Map.ItemLayer.SetCell(newPos, AtlasSourceId, AtlasCoord);

            oldChunk.Items.Remove(Position);
            newChunk.Items.Add(newPos, this);

            Position = newPos;
        }

        public virtual void DoAction()
        {

        }
    }

    public class GameItemMoveable : GameItem
    {
        public Vector2I? TargetPosition { get; set; }
        public GameItem TargetItem { get; set; }


        public override Vector2I GetNextCoords()
        {
            if(Type == ItemType.PLAYER)
            {
                Vector2I? target = TargetItem != null ? TargetItem.Position : TargetPosition;

                if(target != null)
                {
                    Vector2I direction = Vector2I.Zero;
                    if(Position.X < target.Value.X) direction.X = 1;
                    if(Position.X > target.Value.X) direction.X = -1;
                    if(Position.Y < target.Value.Y) direction.Y = 1;
                    if(Position.Y > target.Value.Y) direction.Y = -1;

                    Vector2I newPos = Position + direction;
                    if(newPos == target)
                    {
                        // Reached target
                        TargetPosition = null;
                        TargetItem = null;
                        return Vector2I.MinValue;
                    }
                    return Position + direction;
                }
            }
            return Vector2I.MinValue;
        }

        public override void DoAction()
        {

        }
    }
}
