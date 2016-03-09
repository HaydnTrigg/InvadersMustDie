using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
// ----------------------------------------------------------------------------
// "THE BEER-WARE LICENSE" (Revision 42):
// <Haydn Trigg@Vagyr> wrote this file. As long as you retain this notice you
// can do whatever you want with this stuff. If we meet some day, and you think
// this stuff is worth it, you can buy me a beer in return Haydn-Richard Trigg
// ----------------------------------------------------------------------------

//Visual Basic Isotope Framework port by Haydn Trigg
//Written by: Haydn Trigg
//Created : 2/5/2013 [dd/mm/yyyy]
//Modified: 16/5/2013 [dd/mm/yyyy]
//Version : 0.3.30.1 (Relsease, Revision, Version, Build)

#region "Imports"
//System Imports
using System.Threading;

//OpenTK Imports
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
#endregion

//The "Game Engine" namespace.
namespace Isotope
{
#region "Main"
	//The Main Module for the program
    static class mainModule
	{
		//The main function for the program. This is the first thing that is run.
		[STAThread()]
		public static void Main()
		{
			//Creates a new game on the STAThread
			dynamic game = new Game();
		}
	}
	#endregion

        public partial class Game
	{
#region "Variables & Properties"
		//The games viewable Viewport
		private Viewport withEventsField_gViewport;
		public Viewport gViewport {
			get { return withEventsField_gViewport; }
			set {
				if (withEventsField_gViewport != null) {
                    withEventsField_gViewport.RenderFrame -= withEventsField_gViewport_RenderFrame;
                    withEventsField_gViewport.Closing -= withEventsField_gViewport_Closing;
                    withEventsField_gViewport.Closed -= withEventsField_gViewport_Closed;
                    withEventsField_gViewport.Disposed -= withEventsField_gViewport_Disposed;
				}
				withEventsField_gViewport = value;
				if (withEventsField_gViewport != null) {
                    withEventsField_gViewport.RenderFrame += withEventsField_gViewport_RenderFrame;
                    withEventsField_gViewport.Closing += withEventsField_gViewport_Closing;
                    withEventsField_gViewport.Closed += withEventsField_gViewport_Closed;
                    withEventsField_gViewport.Disposed += withEventsField_gViewport_Disposed;
				}
			}

		}

        void withEventsField_gViewport_Disposed(object sender, EventArgs e)
        {
            Exits();
        }

        void withEventsField_gViewport_Closed(object sender, EventArgs e)
        {
            Exits();
        }

        void withEventsField_gViewport_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Exits();
        }

        void withEventsField_gViewport_RenderFrame(object sender, FrameEventArgs e)
        {
            Draw();
        }

		//The games Update Threads
		Thread gUpdateThread;

		Thread gEffectThread;
		//Specifies if the game will use the EffectThread

		bool bUsingEffectThread;
		//Initializes a generic Random number generation used throughout the game

		Random gRandom = new Random();
		//Keyboard States used throughout the game. Default value set to the current Keyboard state upon program launch.
		OpenTK.Input.KeyboardState gPreviousKeyboardState = OpenTK.Input.Keyboard.GetState();

		OpenTK.Input.KeyboardState gCurrentKeyboardState = OpenTK.Input.Keyboard.GetState();
		//Mouse States used throughout the game. Default value set to the current Keyboard state upon program launch.
		OpenTK.Input.MouseState gPreviousMouseState = OpenTK.Input.Mouse.GetState();

		OpenTK.Input.MouseState gCurrentMouseState = OpenTK.Input.Mouse.GetState();
		//Integer that contains an amount of CPU threads that are recognised by default to the Windows operating system

		int _CoreCount = System.Environment.ProcessorCount;
		#endregion
#region "Initializer"

            public Game()
		{
			//Create the Viewport
			gViewport = new Viewport(640, 480, "Game");

			//Load the settings for the game.
			gViewport.VSync = OpenTK.VSyncMode.Adaptive;

			//Load the game content ready for use
			gLoadContent();

			//WARNING: MUST BE STARTED BEFORE EFFECT THREAD DUE TO DEPENDENCY
			//Create the new threding for the game's update loop
			gUpdateThread = new Thread(() => Update());

			//Create the new threding for the game's update loop
			gEffectThread = new Thread(() => UpdateEffects());

			//Start the games update and effects
			gUpdateThread.Start();

			//Use the STAThread to run the Viewport
			gViewport.Run();
		}

		#endregion
#region "Functions"

		private struct IterationTime
		{
			public const float fTimeScale = 100.0f / 100.0f;
			public float fUpdateAccumulator;
				//How often an update can occur
			public float fUpdateInterval;
				//The time that will be used to update
			public float fUpdateDelta;
			public long lUpdateLastTicks;

			public bool bCanIterate;
			public IterationTime(float interval)
			{
                bCanIterate = false;
				fUpdateAccumulator = 0.0f;
				fUpdateInterval = interval;
				fUpdateDelta = interval * fTimeScale;
				lUpdateLastTicks = DateTime.Now.Ticks;
			}
			public void Update()
			{
				long lNowTicks = DateTime.Now.Ticks;
				TimeSpan tsNow = TimeSpan.FromTicks(lNowTicks - lUpdateLastTicks);
				lUpdateLastTicks = lNowTicks;

				fUpdateAccumulator += (float)tsNow.TotalSeconds;

				if ((fUpdateAccumulator > fUpdateInterval)) {
					bCanIterate = true;
				} else {
					bCanIterate = false;
				}
			}
			public void Iterate()
			{
				fUpdateAccumulator -= fUpdateInterval;
				if ((fUpdateAccumulator > fUpdateInterval)) {
					bCanIterate = true;
				} else {
					bCanIterate = false;
				}
			}
		}

		private IterationTime itEffect = new IterationTime(1.0f / 30.0f);
		private IterationTime itUpdate = new IterationTime(1.0f / 120.0f);
		private IterationTime itDraw = new IterationTime(1.0f / 12.0f);

		private float fTotalTime = new float();

		//The game function that handels the effectThread
		public void UpdateEffects()
		{
			while (gEffectThread.IsAlive & gUpdateThread.IsAlive) {
				itEffect.Update();
				while (itEffect.bCanIterate) {
					itEffect.Iterate();
					gUpdateEffects(itEffect.fUpdateDelta);
					//Update the effects
				}
				Thread.Sleep(5);
			}
			Exits();
		}

		public void Update()
		{
			//If the host computer has more then 2 virtual cores then enable the second thread.
			if (_CoreCount >= 2) {
				bUsingEffectThread = true;
				gEffectThread.Start();
			}
			while (gUpdateThread.IsAlive) {
				if (!bUsingEffectThread) {
					itEffect.Update();
					while (itEffect.bCanIterate) {
						itEffect.Iterate();
						gUpdateEffects(itEffect.fUpdateDelta);
						//Update the effects
					}
				}
				itUpdate.Update();
				while (itUpdate.bCanIterate) {
					itUpdate.Iterate();
					fTotalTime += itUpdate.fUpdateDelta;



					//Update the Keyboard+Mouse State if the client is currently viewing the game
					if (gViewport.Focused) {
						gCurrentKeyboardState = OpenTK.Input.Keyboard.GetState();
						gCurrentMouseState = OpenTK.Input.Mouse.GetState();
					} else {
						gCurrentKeyboardState = new OpenTK.Input.KeyboardState();
						gCurrentMouseState = new OpenTK.Input.MouseState();
					}

					//Update the Keyboard+Mouse State
					gPreviousMouseState = gCurrentMouseState;
					gPreviousKeyboardState = gCurrentKeyboardState;

					gUpdate(itUpdate.fUpdateDelta);
					//Update the effects
				}

				Thread.Sleep(5);
			}
			Exits();
		}

        public void Draw()
		{
			if ((gViewport.IsExiting)) {
				Exits();
			}

			//Clear the OpenGL Device with Black ready to draw a frame.
			GL.ClearColor(Color4.Black);

			//Clears the color and depth mask.
			GL.Clear(ClearBufferMask.ColorBufferBit);
			//Creates a default Projection Matrix (Matrix.Identity)
			GL.MatrixMode(MatrixMode.Projection);
			//Loads the Matrix Identity (Matrix.Identity)
			GL.LoadIdentity();
			//Setup Orthographic Rendering see: http://en.wikipedia.org/wiki/Orthographic_projection
			GL.Ortho(0, gViewport.Width / gViewport.ViewportScale, gViewport.Height / gViewport.ViewportScale, 0, -1, 1);

			//Creates the Viewport at 0,0 with its Width and Height
			GL.Viewport(0, 0, gViewport.Width, gViewport.Height);
			//Enables rendering textures with the current pass.
			GL.Enable(EnableCap.Texture2D);

			//<<<ALL SPECIFIC DRAWING CODE BELOW HERE>>>

			//Begin the gDraw() Logic Function
			gDraw(itDraw.fUpdateDelta);

			//<<ALL SPECIFIC DRAWING CODE ABOVE HERE>>>
			//Disables Rendering Textures with the current pass.
			GL.Disable(EnableCap.Texture2D);
			//End Drawing things with OpenGL
			GL.End();
			//Flush the Device
			GL.Flush();
			//Swap the buffers around, 0-1, 1-0 "Double Buffering" ready for the next frame.
			gViewport.SwapBuffers();
		}

		private void Exits()
		{
			System.Environment.Exit(0);
		}

		#endregion
	}
}