using System;

namespace Polys.Game
{
    class Entity
    {
        private OpenGL.Vector2 mPosition;

        private bool mShown;

        public bool shown
        {
            set { mShown = value; }
            get { return mShown;  }
        }
    }
}
