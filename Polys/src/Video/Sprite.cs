using System;
using OpenGL;

namespace Polys.Video
{
    public class Sprite : Transformable
    {
        /**  whether it is visible. */
        public bool  visible;

        int uvOffsetX, uvOffsetY;

        /** Constructs a tile. The position is subtracted from the tile count to account for up=down issue.*/
        public Sprite(TiledSharp.TmxLayerTile tile, int genericTileWidth, int genericTileHeight,
            Tileset tileset, int tileCountY)
            : base(new Util.Rect(tile.X* genericTileWidth,
                (tileCountY-tile.Y-1)* genericTileHeight, 
                tileset.tileWidth,
                tileset.tileHeight))
        {
            visible = tile.Gid != 0;
            setUV(tile.Gid < 1 ? 0 : ((tile.Gid - tileset.firstGid) % tileset.tileCountX)* tileset.tileWidth,
                tile.Gid < 1 ? 0 : ((tile.Gid - tileset.firstGid) / tileset.tileCountX)* tileset.tileHeight);
        }

        /** Constructs the tile */
        public Sprite(Util.Rect rect, bool visible = true,
            int uvX = 0, int uvY = 0)
            : base(rect)
        {
            //Copy in properties
            this.visible = visible;
            setUV(uvX, uvY);
        }


        void setUV(int uvX, int uvY)
        {
            uvOffsetX = uvX;
            uvOffsetY = uvY;
        }

        public void setTilesetIndex(int x, int y)
        {
            uvOffsetX = x* rect.w;
            uvOffsetY = y* rect.h;
        }

        public Matrix4 uvMatrix(float tilesetWidth, float tilesetHeight)
        {
            return new Matrix4(new float[] { rect.w/tilesetWidth, 0, 0, 0,
                               0, rect.h/tilesetHeight, 0, 0,
                               0, 0, 0, 0,
                               uvOffsetX/tilesetWidth + (0.5f/tilesetWidth), uvOffsetY/tilesetHeight + (0.5f/tilesetHeight), 0, 1 });
        }
        
        public bool overlaps(Sprite s)
        {
            //if (RectA.Left < RectB.Right && RectA.Right > RectB.Left &&
            //RectA.Bottom < RectB.Top && RectA.Top > RectB.Bottom)

            return rect.x < (s.rect.x+s.rect.w) && (rect.x+ rect.w) > s.rect.x &&
                rect.y < (s.rect.y + s.rect.h) && (rect.y + rect.h) > s.rect.y;
        }
    }
}
