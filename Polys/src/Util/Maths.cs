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

        public static bool isRectVisible(int rectX, int rectY, int rectW, int rectH, int screenW, int screenH)
        {
            if (rectX + rectW < 0 || rectY + rectH < 0 ||
                rectX > screenW || rectY > screenH)
                return false;
            else
                return true;
        }

        //The position is the bottom left corner.
        public static Matrix4 matrixPixelProjection(int posX, int posY, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            //We want the position to be the bottom left corner, so compensate.
            //Temporary solution.
            //posX -= tileWidth;
            //posY -= tileHeight;

            //Find projection and UV matrices.
            float px = (posX + 0.5f) / screenWidth * 2.0f - 1.0f;
            float py = (screenHeight - posY - 0.5f - tileHeight) / screenHeight * 2.0f - 1.0f;
            float sx = (float)tileWidth /screenWidth;
            float sy = (float)tileHeight/screenHeight;

            return new Matrix4(new float[] { sx, 0, 0, 0,
                                                           0, sy, 0, 0,
                                                           0, 0, 0, 0,
                                                           sx+px, sy+py, 0, 1});
        }
    }
}
