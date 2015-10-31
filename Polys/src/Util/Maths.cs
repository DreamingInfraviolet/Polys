using System;
using OpenGL;

namespace Polys.Util
{
    class Maths
    {
        /** Calculates a matrix to fit a unit rectangle into the screen */
        public static Matrix4 matrixFitRectIntoScreen(int sourceWidth, int sourceHeight, int targetWidth, int targetHeight)
        {

            float aspecti = (float)sourceWidth / sourceHeight;
            float aspectr = (float)targetWidth / targetHeight;

            float scaleX = 1, scaleY = 1;

            if (aspectr < aspecti)
                scaleY = aspectr / aspecti;
            else
                scaleX = aspecti / aspectr;

            return new Matrix4(new float[] {   scaleX,0,0,0,
                                            0, scaleY,0,0,
                                            0,0,1,0,
                                            0,0,0,1});
        }

        public static bool isRectVisible(int rectX, int rectY, int rectW, int rectH, int screenW, int screenH)
        {
            if (rectX + rectW < 0 || rectY + rectH < 0 ||
                rectX >= (int)screenW || rectY >= (int)screenH)
                return false;
            else
                return true;
        }

        public static Matrix4 matrixPixelProjection(int posX, int posY, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            //We want the position to be the bottom left corner, so compensate.
            //Temporary solution.
            posX -= tileWidth;
            posY -= tileHeight;

            //Find projection and UV matrices.
            Vector2 screenVec = new Vector2(screenWidth, screenHeight);
            Vector2 p = (new Vector2(posX, posY) + 0.5f) / screenVec * 2.0f - 1.0f;
            Vector2 s = new Vector2(tileWidth, tileHeight) / screenVec;

            return new Matrix4(new float[] { s.x, 0, 0, 0,
                                                           0, s.y, 0, 0,
                                                           0, 0, 0, 0,
                                                           s.x+p.x, s.y+p.y, 0, 1});
        }
    }
}
