using System;
using OpenGL;

namespace Polys.Video
{
    class HighLevelRenderer : IDisposable
    {
        //Various shader programs used during rendering
        public static ShaderProgram shaderDrawSprite;
        public static ShaderProgram shaderIndexedBitmapSprite;

        static int targetIWidth, targetIHeight;
        static float targetWidth, targetHeight;

        public static int width { get { return targetIWidth; } }
        public static int height { get { return targetIHeight; } }

        public void Dispose()
        {
            if (shaderDrawSprite != null)
                shaderDrawSprite.Dispose();
            if (shaderIndexedBitmapSprite != null)
                shaderIndexedBitmapSprite.Dispose();
        }

        public static void setTargetSize(int width, int height)
        {
            targetIWidth = width;
            targetIHeight = height;
            targetWidth = width;
            targetHeight = height;
        }

        /** Draws a tile layer with a given camera. */
        public static void draw(TileLayer layer, Camera camera)
        {
            if (layer.visible == false)
                return;

            //Prepare
            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.shader = shaderIndexedBitmapSprite;

            //In normalised GL coordinates:
            Vector2 screenVec = new Vector2(targetWidth, targetHeight);

            foreach (var tiles in layer.tileDict)
            {
                //Bind tileset textures
                tiles.Key.bind();

                //Retrieve dimensions
                int tileWidth = tiles.Key.tileWidth;
                int tileHeight = tiles.Key.tileHeight;
                
                //For each tile
                foreach (Sprite tile in tiles.Value)
                {
                    if (!tile.visible)
                        return;

                    //Get screen coordinates of the tile in pixels
                    int screenPosX = tile.posX * tileWidth;
                    int screenPosY = tile.posY * tileHeight;
                    camera.worldToScreen(ref screenPosX, ref screenPosY);

                    if (!Util.Maths.isRectVisible(screenPosX, screenPosY, tileWidth, tileHeight, (int)targetWidth, (int)targetHeight))
                        continue;

                    //Set matrix uniforms
                    //shaderIndexedBitmapSprite["orthoMatrix"].SetValue(Matrix4.Identity);

                    shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                        Util.Maths.matrixPixelProjection(screenPosX, screenPosY, tileWidth, tileHeight, (int)targetWidth, (int)targetHeight));

                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(tile.uvMatrix(tiles.Key.width, tiles.Key.height));

                    //Draw
                    LowLevelRenderer.draw();
                }
            }
        }

        public void draw(Sprite tile, Tileset tileset, Camera camera)
        {
            throw new NotImplementedException();
        }

        public static void draw(DrawableSprite sprite, Camera camera)
        {
            if (sprite.visible == false)
                return;

            //Prepare
            shaderDrawSprite.Use();
            LowLevelRenderer.geometry = LowLevelRenderer.quad;

            //In normalised GL coordinates:
            Vector2 screenVec = new Vector2(targetWidth, targetHeight);

            //Bind tileset texture
            sprite.tileset.bind();

            //Retrieve dimensions
            int tileWidth = sprite.tileset.tileWidth;
            int tileHeight = sprite.tileset.tileHeight;

            LowLevelRenderer.shader = shaderIndexedBitmapSprite;
            

            camera.worldToScreen(sprite);

            if (!Util.Maths.isRectVisible(sprite.posX, sprite.posY, tileWidth, tileHeight, (int)targetWidth, (int)targetHeight))
                return;

            //Set matrix uniforms
            //shaderIndexedBitmapSprite["orthoMatrix"].SetValue(Matrix4.Identity);

            shaderIndexedBitmapSprite["orthoMatrix"].SetValue(
                Util.Maths.matrixPixelProjection(sprite.posX, sprite.posY, tileWidth, tileHeight, (int)targetWidth, (int)targetHeight));

            shaderIndexedBitmapSprite["uvMatrix"].SetValue(
                sprite.uvMatrix(sprite.tileset.width, sprite.tileset.height));

            //Draw
            LowLevelRenderer.draw();
        }
    }
}
