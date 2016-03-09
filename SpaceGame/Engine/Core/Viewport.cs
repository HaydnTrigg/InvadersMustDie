using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
//Import the OpenTK Library's
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Isotope
{
    //Create the class Viewport.
    //This class will Inherit the GameWindow class "OpenTK.GameWindow"
    public class Viewport : GameWindow
    {

        #region "publicField"
        public Vector2 Position = new Vector2(0, 0);
        #endregion
        public Vector2 ViewportRealSize = new Vector2(1600, 1600);

        #region "PrivateField"


        #endregion

        //Returns the current scale of the Viewport
        public float ViewportScale
        {
            get
            {
                if (Width >= Height)
                {
                    float a = Width / ViewportRealSize.X;
                    if ((a > 0.5))
                    {
                        return 1.25f * Height / ViewportRealSize.Y;
                    }
                    else
                    {
                        return a;
                    }
                }
                else
                {
                    return 1.25f * Height / ViewportRealSize.Y;
                }
            }
        }

        //Returns the current mouse position.
        public Vector2 MousePosition
        {
            //Returns the correct mouse position based on Scale and the Position of the Viewport.
            get { return new Vector2(Mouse.X, Mouse.Y) / ViewportScale + Position; }
        }


        //Creates a New Viewport with some specified properties.
        public Viewport(int _Width, int _Height, string _Title)
            : base(_Width, _Height, new GraphicsMode(32, 24, 8, 2), _Title)
        {
        }

        //Updates the Viewport
        public void Update(float delta)
        {
        }
        public float ModS(float _Single)
        {
            if (_Single < 0)
            {
                return _Single * -1;
            }
            return _Single;
        }

    }
}
