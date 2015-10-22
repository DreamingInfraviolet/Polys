namespace Polys.Video
{
    /** Represents a camera in the game world. Does not allow for zoom or rotation, so it's purely positional. */
    class Camera
    {
        /** The world pixel coordinate to be the top right corner of the screen */
        int mCornerX, mCornerY;
        
        /** Applies the inverse of the camera transformation to a point. */
        public void worldToScreen(ref int x, ref int y)
        {
            x -= mCornerX;
            y -= mCornerY;
        }

        /** Moves the camera by a specified amount. */
        public void move(int dx, int dy)
        {
            mCornerX += dx;
            mCornerY += dy;
        }

        public void centreOn(int x, int y)
        {
            mCornerX = x - HighLevelRenderer.width / 2;
            mCornerY = y - HighLevelRenderer.height / 2;
        }
    }
}
