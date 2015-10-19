using System;

namespace Polys.Video
{
    /** Represents a colour, allowing useful colour operations. */
    struct Colour
    {
        //Components
        public byte r, g, b, a;

        /** Initialises the colour from the parameters */
        public Colour(byte r_ = 0, byte g_ = 0, byte b_ = 0, byte a_ = 255)
        {
            r = r_;
            g = g_;
            b = b_;
            a = a_;
        }

        /** Initialises the colour from an "r g b" string (e.g., 200 121 1). A is set to 255  */
        public Colour(String rgbstr)
        {
            int r_, g_, b_;
            String[] components = rgbstr.Split();
            if (components.Length != 3 || !int.TryParse(components[0], out r_) ||
                                          !int.TryParse(components[1], out g_) ||
                                          !int.TryParse(components[2], out b_))
                throw new Exception(String.Format("Could not parse colour from \"{0}\"", rgbstr));
            r = (byte)r_;
            g = (byte)g_;
            b = (byte)b_;
            a = 255;
        }

        /** Initialises the colour from a System.Drawing.Color object */
        public Colour(System.Drawing.Color colour)
        {
            r = colour.R;
            g = colour.G;
            b = colour.B;
            a = colour.A;
        }
    }
}
