using System;
using OpenGL;
using System.Collections.Generic;

namespace Polys.Video
{
    public class HighLevelRenderer : IDisposable
    {
        //Various shader programs used during rendering
        public static ShaderProgram shaderDrawSprite;
        public static ShaderProgram shaderIndexedBitmapSprite;

        static int targetWidth, targetHeight;

        public static int width { get { return targetWidth; } }
        public static int height { get { return targetHeight; } }

        public void Dispose()
        {
            if (shaderIndexedBitmapSprite != null)
                shaderIndexedBitmapSprite.Dispose();
        }

        public static void setTargetSize(int width, int height)
        {
            targetWidth = width;
            targetHeight = height;
        }

        public static void draw(Game.Scene scene, Camera camera = null)
        {
            foreach (var layer in scene.layers)
                if (layer.visible)
                    draw(layer, scene.gridTileWidth, scene.gridTileHeight, camera);
        }

        /** Draws a tile layer with a given camera. */
        public static void draw(TileLayer layer, int gridTileWidth, int gridTileHeight, Camera camera = null)
        {
            //Util.Util.insertionSort<Sprite, Transformable>(layer.tiles);

            int tileMinX, tileMinY, tileMaxX, tileMaxY;
            if (camera != null)
            {
                tileMinX = (camera.mCornerX - layer.maxTileWidth) / layer.genericTileWidth;
                tileMinY = (camera.mCornerY - layer.maxTileHeight) / layer.genericTileHeight;
                tileMaxX = tileMinX + (targetWidth+ layer.maxTileWidth) / layer.genericTileWidth+1;
                tileMaxY = tileMinY + (targetHeight+ layer.maxTileHeight) / layer.genericTileHeight+1;
            }
            else
            { 
                tileMinX = 0;
                tileMinY = 0;
                tileMaxX = (targetWidth) / layer.genericTileWidth+1;
                tileMaxY = (targetHeight) / layer.genericTileHeight+1;
            }

            for (int yid = Math.Max(tileMinX, 0); yid < Math.Min(layer.tileCountY, tileMaxY); ++yid)
                for (int xid = Math.Max(tileMinX, 0); xid < Math.Min(layer.tileCountX, tileMaxX); ++xid)
                {
                    Sprite tile = layer.tileCache[layer.tiles[xid, yid]];

                    int x = xid * layer.genericTileWidth;
                    int y = yid * layer.genericTileWidth;

                    //Bind tileset textures
                    tile.tileset.bind();

                    if (!tile.visible)
                        continue;

                    //Get screen coordinates of the tile in pixels
                    Util.Rect rect = tile.rect;
                    rect.x = x;
                    rect.y = y;

                    if (camera != null)
                        camera.worldToScreen(ref rect);

                    if (!Util.Maths.isRectVisible(rect, targetWidth, targetHeight))
                        continue;

                    //Set matrix uniforms
                    shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                        Util.Maths.matrixPixelProjection(rect, targetWidth, targetHeight));

                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(tile.uvMatrix(tile.tileset.width, tile.tileset.height));

                    //Draw
                    LowLevelRenderer.draw();

                }
        }

        public static void draw(Sprite sprite, Camera camera = null)
        {
            if (!sprite.visible)
                return;

            //Bind tileset texture
            sprite.tileset.bind();

            Util.Rect rect = sprite.rect;

            if (camera != null)
                camera.worldToScreen(ref rect);

            if (!Util.Maths.isRectVisible(rect, targetWidth, targetHeight))
                return;

            shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                Util.Maths.matrixPixelProjection(rect, targetWidth, targetHeight));

            shaderIndexedBitmapSprite["uvMatrix"].SetValue(
                sprite.uvMatrix(sprite.tileset.width, sprite.tileset.height));

            //Draw
            LowLevelRenderer.draw();
        }
    }
}

