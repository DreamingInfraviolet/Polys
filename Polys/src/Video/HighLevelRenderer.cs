using System;
using OpenGL;

namespace Polys.Video
{
    class HighLevelRenderer : IDisposable
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

            foreach (var tiles in layer.tileDict)
            {
                //Bind tileset textures

                tiles.Key.bind();

                //For each tile
                foreach (Sprite tile in tiles.Value)
                {
                    if (!tile.visible)
                        continue;

                    //Get screen coordinates of the tile in pixels
                    int screenPosX = tile.posX * gridTileWidth;
                    int screenPosY = tile.posY * gridTileHeight;

                    if (camera != null)
                        camera.worldToScreen(ref screenPosX, ref screenPosY);

                    if (!Util.Maths.isRectVisible(screenPosX, screenPosY, tile.width, tile.height, targetWidth, targetHeight))
                        continue;

                    //Set matrix uniforms
                    shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                        Util.Maths.matrixPixelProjection(screenPosX, screenPosY, tile.width, tile.height, targetWidth, targetHeight));

                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(tile.uvMatrix(tiles.Key.width, tiles.Key.height));

                    //Draw
                    LowLevelRenderer.draw();
                }
            }
        }

        public static void draw(Sprite sprite, Tileset tileset, Camera camera = null)
        {
            if (!sprite.visible)
                return;

            //Prepare
            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.shader = shaderIndexedBitmapSprite;

            //Bind tileset texture
            tileset.bind();

            int spriteX = sprite.posX;
            int spriteY = sprite.posY;

            if (camera != null)
                camera.worldToScreen(ref spriteX, ref spriteY);

            if (!Util.Maths.isRectVisible(spriteX, spriteY, sprite.width, sprite.height, targetWidth, targetHeight))
                return;

            shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                Util.Maths.matrixPixelProjection(spriteX, spriteY, sprite.width, sprite.height, targetWidth, targetHeight));

            shaderIndexedBitmapSprite["uvMatrix"].SetValue(
                sprite.uvMatrix(tileset.width, tileset.height));

            //Draw
            LowLevelRenderer.draw();
        }

        public static void draw(DrawableSprite sprite, Camera camera = null)
        {
            draw(sprite, sprite.tileset, camera);
        }
    }
}

