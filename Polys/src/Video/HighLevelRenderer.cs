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
            int tileMinX, tileMinY, tileMaxX, tileMaxY;
            if (camera != null)
            {
                tileMinX = (camera.mCornerX - layer.maxTileWidth) / layer.genericTileWidth;
                tileMinY = (camera.mCornerY - layer.maxTileHeight) / layer.genericTileHeight;
                tileMaxX = tileMinX + (targetWidth+ layer.maxTileWidth) / layer.genericTileWidth+2;
                tileMaxY = tileMinY + (targetHeight+ layer.maxTileHeight) / layer.genericTileHeight+2;
            }
            else
            { 
                tileMinX = 0;
                tileMinY = 0;
                tileMaxX = (targetWidth) / layer.genericTileWidth+1;
                tileMaxY = (targetHeight) / layer.genericTileHeight+1;
            }

            int limX = Math.Min(layer.tileCountX, tileMaxX);
            int limY = Math.Max(tileMinY, 0);

            Util.Util.insertionSort<Sprite, Transformable>(layer.objects);
            int objectIndex = 0;
            for (int yid = Math.Min(layer.tileCountY, tileMaxY)-1; yid >= limY; --yid)
            {
                int y = yid * layer.genericTileHeight;

                //Draw objects until they start going below the current layer
                for (; objectIndex < layer.objects.Count; ++objectIndex)
                {
                    if (layer.objects[objectIndex].rect.y < y)
                        break;
                    draw(layer.objects[objectIndex], camera);
                }


                for (int xid = Math.Max(tileMinX, 0); xid < limX; ++xid)
                {
                    int tileId = layer.tiles[xid, yid];
                    if (tileId < 0)
                        continue;

                    Sprite tile = layer.tileCache[tileId];

                    int x = xid * layer.genericTileWidth;

                    //Bind tileset textures
                    tile.tileset.bind();

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

            //Draw all remaining objects
            for (; objectIndex < layer.objects.Count; ++objectIndex)
                draw(layer.objects[objectIndex], camera);
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