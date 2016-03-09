
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
#region "Imports"

//Import parts of the OpenTK Framework
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

//Import parts of the Isotope Framework
using Isotope;
using Isotope.Library;

#endregion

public class GameControll
{
    public enum ControllType
    {
        Button = 0,
        Other = 100
    }

    public ControllType eControllType = ControllType.Other;
    //Controll Game State
    public struct ControllGameState
    {
        public KeyboardState gPreviousKeyboardState;
        public KeyboardState gCurrentKeyboardState;
        public MouseState gPreviousMouseState;
        public MouseState gCurrentMouseState;
        public Viewport gViewport;
    }

    public ControllGameState gControllGameState;

    public bool bIsHovering;

    public Color4 cDrawColor;

    //Stores the position of the Object
    public Vector2 vPosition;
    //Stores the size of the Object
    public Vector2 vSize;
    //Stores an array of integers used to indicate which textures the object should draw

    public int[] iTextureIdentification;
    public GameControll(Vector2 _Position, Vector2 _Size, int[] _TextureID)
    {
        vPosition = _Position;
        vSize = _Size;
        iTextureIdentification = _TextureID;
    }

    //The Overridable Update Function which is avaliable to all Controll's
    public virtual void Update(float delta, Random gRandom, Vector2 _Target)
    {
    }
    //The Overridable Draw Function which is avaliable to all Controll's
    public virtual void Draw(float delta, Viewport gViewport)
    {
    }

}


//The object for main menu buttons
public class MenuButton : GameControll
{

    public MenuButton(Vector2 _Position, Vector2 _Size, int[] _TextureID)
        : base(_Position, _Size, _TextureID)
    {
        //Add extra code below
        eControllType = ControllType.Button;
        cDrawColor = Color4.White;
    }

    //Override Update statement.
    public override void Update(float delta, Random gRandom, Vector2 _Target)
    {
        bIsHovering = false;
        if (CheckCollision(_Target, vPosition, vSize, gControllGameState.gViewport))
        {
            cDrawColor.A = GameMath.ClampFloat(cDrawColor.A + 1f * delta, 0.5f, 1f);
            bIsHovering = true;
        }
        else
        {
            cDrawColor.A = GameMath.ClampFloat(cDrawColor.A - 1f * delta, 0.5f, 1f);
        }
    }

    public override void Draw(float delta, Viewport gViewport)
    {
        //GL.Color4(cDrawColor)
        DrawSprite.Draw2D(gViewport, iTextureIdentification[0], vPosition - vSize / 2, vSize);
        //GL.Color4(Color4.White)
    }

    public bool CheckCollision(Vector2 _Point1, Vector2 _Position, Vector2 _Size, Viewport _Viewport)
    {
        System.Drawing.RectangleF objectrectangle = new System.Drawing.RectangleF(_Position.X - _Size.X / 2f, _Position.Y - _Size.Y / 2f, _Size.X, _Size.Y);
        if (objectrectangle.IntersectsWith(new System.Drawing.RectangleF(_Point1.X, _Point1.Y, 1f, 1f)))
        {
            return true;
        }
        return false;
    }
}
