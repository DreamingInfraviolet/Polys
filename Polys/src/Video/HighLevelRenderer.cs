using System;
using OpenGL;

namespace Polys.Video
{
    class HighLevelRenderer : IDisposable
    {
        //Various shader programs used during rendering
        public static ShaderProgram shaderDrawSprite;
        public static ShaderProgram shaderIndexedBitmapSprite;

        static float targetWidth, targetHeight;

        public void Dispose()
        {
            if (shaderDrawSprite != null)
                shaderDrawSprite.Dispose();
            if (shaderIndexedBitmapSprite != null)
                shaderIndexedBitmapSprite.Dispose();
        }

        public static void setTargetSize(int width, int height)
        {
            targetWidth = width;
            targetHeight = height;
        }

        /** Draws a tile layer with a given camera. */
        public static void draw(TileLayer layer, Camera camera)
        {
            if (layer.visible == false)
                return;

            //Prepare
            shaderDrawSprite.Use();
            LowLevelRenderer.geometry = LowLevelRenderer.quad;

            //In normalised GL coordinates:
            Vector2 screenVec = new Vector2(targetWidth, targetHeight);

            foreach (var tiles in layer.tileDict)
            {
                //Bind tileset textures
                tiles.Key.bind();

                //Retrieve dimensions
                int tileWidth = tiles.Key.tileWidth;
                int tileHeight = tiles.Key.tileHeight;
                
                LowLevelRenderer.geometry = LowLevelRenderer.quad;

                //For each tile
                foreach (Tile tile in tiles.Value)
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

                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(tile.uvMatrix());

                    //Draw
                    LowLevelRenderer.draw();
                }
            }
        }

        public void draw(Tile tile, Camera camera, ShaderProgram program)
        {
            throw new NotImplementedException();
        }
    }
}
