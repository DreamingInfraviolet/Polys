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
            //Prepare
            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.shader = shaderIndexedBitmapSprite;

            //Sort the elements
            Sprite[] sprites = new Sprite[layer.tiles.Count];
            {
                int index = 0;
                foreach (Sprite s in layer.tiles)
                    sprites[index++] = s;
             }

            Array.Sort<Sprite>(sprites);


            foreach (Sprite s in sprites)
            {
                //Bind tileset textures
                s.tileset.bind();

                    if (!s.visible)
                        continue;

                    //Get screen coordinates of the tile in pixels
                    Util.Rect rect = s.rect;

                    if (camera != null)
                        camera.worldToScreen(ref rect);

                    if (!Util.Maths.isRectVisible(rect, targetWidth, targetHeight))
                        continue;

                    //Set matrix uniforms
                    shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                        Util.Maths.matrixPixelProjection(rect, targetWidth, targetHeight));

                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(s.uvMatrix(s.tileset.width, s.tileset.height));

                    //Draw
                    LowLevelRenderer.draw();
               
            }
        }

        public static void draw(Sprite sprite,  Camera camera = null)
        {
            if (!sprite.visible)
                return;

            //Prepare
            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.shader = shaderIndexedBitmapSprite;

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

