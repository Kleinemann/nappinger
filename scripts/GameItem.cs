using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nappinger.scripts
{
    public enum ItemType { NONE, PLANT, ANIMAL, BUILDING, NPC, PLAYER };

    public class GameItem
    {
        WorldMap Map => WorldMain.Instance.Map;

        int AtlasSourceId;
        Vector2I AtlasCoord;
        public ItemType Type { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public Vector2I Position { get; set; }

        public Vector2I ChunkCoord {  get; set; }


        public ImageTexture Icon
        {
            get
            {
                int sourceId = Map.ItemLayer.GetCellSourceId(Position);
                Vector2I atlasCoords = Map.ItemLayer.GetCellAtlasCoords(Position);
                TileSetAtlasSource tss = Map.ItemLayer.TileSet.GetSource(sourceId) as TileSetAtlasSource;
                Texture2D atlasTexture = tss.Texture;
                Rect2I region = tss.GetTileTextureRegion(atlasCoords);

                Image atlasImage = atlasTexture.GetImage();
                Image tileImage = atlasImage.GetRegion(region);
                return ImageTexture.CreateFromImage(tileImage);
            }
        }

        public GameItem(Vector2I pos)
        {
            Position = pos;
            TileData data = Map.ItemLayer.GetCellTileData(pos);

            Name = (string)data.GetCustomData("ItemName");
            Type = (ItemType)((int)data.GetCustomData("ItemType"));
            Value = (int)data.GetCustomData("ItemValue");


        }
    }

    public class GameItemMoveable : GameItem
    {
        public Vector2I TargetPosition { get; set; }

        public GameItemMoveable(Vector2I pos) : base(pos)
        { }
    }
}
