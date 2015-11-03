using System;
using OpenGL;

namespace Polys.Util
{
    public class Maths
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

        public static bool isRectVisible(Rect rect, int screenW, int screenH)
        {
            if (rect.right < 0 || rect.top < 0 ||
                rect.x > screenW || rect.y > screenH)
                return false;
            else
                return true;
        }

        //The position is the bottom left corner.
        public static Matrix4 matrixPixelProjection(Rect spriteRect, int screenWidth, int screenHeight)
        {
            //We want the position to be the bottom left corner, so compensate.
            //Temporary solution.
            //posX -= tileWidth;
            //posY -= tileHeight;

            //Find projection and UV matrices.
            float px = (spriteRect.x + 0.5f) / screenWidth * 2.0f - 1.0f;
            float py = (screenHeight - spriteRect.y - 0.5f - spriteRect.h) / screenHeight * 2.0f - 1.0f;
            float sx = (float)spriteRect.w / screenWidth;
            float sy = (float)spriteRect .h/ screenHeight;

            return new Matrix4(new float[] { sx, 0, 0, 0,
                                                           0, sy, 0, 0,
                                                           0, 0, 0, 0,
                                                           sx+px, sy+py, 0, 1});
        }
    }
}
