using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenGL;

/** Used for software drawing. It has a small resolution and is rendered onto a hardware surface before being show. */

namespace Polys.Video
{
    class SoftwareRenderTarget
    {
        byte[] mData;
        uint mPixelCount, mWidth, mHeight;
        uint mTextureId;

        public uint id { get { return mTextureId; } }

        public uint width { get { return mWidth; } }

        public uint height { get { return mHeight; } }

        public SoftwareRenderTarget(uint width, uint height)
        {
            if (width * height == 0)
                throw new Exception("Invalid render target parameters");

            mPixelCount = width * height;
            mData = new byte[mPixelCount * 4];
            mWidth = width;
            mHeight = height;


        }

        public void shutdown()
        {
            deupload();
        }

        public void clear()
        {
            Util.Util.MemSet(mData, 0, 64);
        }

        public void deupload()
        {
            if (mTextureId != 0)
            {
                uint[] arr = new uint[1];
                arr[0] = mTextureId;
                Gl.DeleteTextures(1, arr);
                mTextureId = 0;
            }
        }

        //returns the GL id of the texture
        public uint upload()
        {
            deupload();
            mTextureId = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, mTextureId);

            IntPtr dataPtr = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(mData, 0);
            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)mWidth, (int)mHeight, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, dataPtr);

            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);

            return mTextureId;
        }

        public void draw(TileLayer layer, Camera camera)
        {
            if (layer.visible == false)
                return;

            foreach (Tile tile in layer.tiles)
            {
                draw(tile, camera);
            }
        }

        public void draw(Tile tile, Camera camera)
        {
            if (!tile.visible)
                return;
            
            //Get start coordinates and end
            int startDrawX = tile.worldX * tile.tileset.tileWidth;
            int startDrawY = tile.worldY * tile.tileset.tileHeight;
            camera.worldToScreen(ref startDrawX, ref startDrawY);
            
            int endDrawX = startDrawX + tile.tileset.tileWidth;
            int endDrawY = startDrawY + tile.tileset.tileHeight;

            //Check if they are on the screen
            if (endDrawX < 0 || endDrawY < 0 || startDrawX >= (int)width || startDrawY >= (int)height)
                return;

            tile.tileset.image.bind();
            tile.tileset.mPalette.bind();

            /*

            //after clamping - before clamping
            int amountClampedX = -startDrawX, amountClampedY = -startDrawY;

            if (startDrawX < 0)
                startDrawX = 0;
            if (startDrawY < 0)
                startDrawY = 0;
            endDrawX = Util.Util.Clamp(endDrawX, 0, (int)width);
            endDrawY = Util.Util.Clamp(endDrawY, 0, (int)height);

            amountClampedX += startDrawX;
            amountClampedY += startDrawY;

            int tilesetPixelStartX = tile.tilesetX * tile.tileset.tileWidth+ amountClampedX;
            int tilesetPixelStartY = tile.tilesetY * tile.tileset.tileWidth+ amountClampedY;

            for (int iDrawYScreen = startDrawY, iDrawYTile = 0; iDrawYScreen < endDrawY; ++iDrawYScreen, ++iDrawYTile)
                for(int iDrawXScreen = startDrawX, iDrawXTile = 0; iDrawXScreen < endDrawX; ++iDrawXScreen, ++iDrawXTile)
                {
                    int colourIndex = tile.tileset.image.indexAt(
                        tilesetPixelStartX + iDrawXTile, tilesetPixelStartY + iDrawYTile);
                    if (colourIndex == tile.tileset.invisibilityColourIndex)
                        continue;

                    Colour colour = tile.tileset.mPalette.colours[colourIndex];

                    int index = (iDrawXScreen + iDrawYScreen * (int)width)*4;
                    mData[index] = colour.r;
                    mData[index + 1] = colour.g;
                    mData[index + 2] = colour.b;
                    //mData[index + 3] = colour.a;
                }
                */
        }
    }
}
