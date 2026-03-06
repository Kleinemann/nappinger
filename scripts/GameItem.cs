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
    }

    public class GameItemMoveable : GameItem
    {
        public Vector2I TargetPosition { get; set; }
    }
}
