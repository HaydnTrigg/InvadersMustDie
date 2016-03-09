using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

#region "Imports"

//Import OpenTK
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

//Import the Isotope Framework
using Isotope;
using Isotope.Library;

//Import System
using System.Drawing;
using System.Drawing.Imaging;

#endregion
namespace Isotope
{
	//Create a partial class to extent on the Isotope.Game class
	public partial class Game
	{
		#region "Enumeration & Structures"

		//An enumeration of the current game state
		public enum GameState
		{
			Loading = 0,
			Menu = 1,
			Game = 2
		}

		//An enumeration of the current game effect level
		public enum EffectLevel
		{
			None = 0,
			Low = 1,
			Medium = 2,
			High = 3,
			Ultra = 4
		}

		//An enumeration of a type of particle
		public enum ParticleType
		{
			EnemyExplosion,
			BulletExplosion,
			PlayerExplosion,
			RandomExplosion
		}

		//A structure to hold Texture Identitifaction Information
		public struct TextureID
		{
			public int Width;
			public int Height;
			public int ID;
			public Vector2 Size {
				get { return new Vector2(Width, Height); }
			}
		}

		#endregion
		#region "Variables & Properties"

		public Color4[] cColors = new Color4[] {
			new Color4(1f, 0f, 0f, 1f),
			new Color4(0f, 1f, 0f, 1f),
			new Color4(0f, 0.5803922f, 1f, 1f),
			new Color4(1f, 0.3529412f, 0.2078431f, 1f),
			new Color4(255, 0f, 0.8627451f, 1f),
			new Color4(0.6980392f, 0f, 1f, 1f),
			new Color4(1f, 0.8470588f, 0f, 1f)

		};
		//The current GameState

		GameState gGameState = GameState.Menu;
		//The current Level of Effects

		EffectLevel gEffectLevel = EffectLevel.Ultra;
		//The games QuadTree

		QuadTree<int> gQuadTree;
		//All of the Entity objects currently in the game

		List<GameObject> gGameEntitys = new List<GameObject>();
		//All of the Effect objects currently in the game

		List<GameObject> gGameEffects = new List<GameObject>();
		//Stores information of the loaded textures

		List<TextureID> gTextures = new List<TextureID>();
		//The time between spawning a new effect

		float fEffectSpawn;
		//The players current spawn time
		float fSpawnTime;
		//The players maximum spawn time

		float fSpawnTimeMax;
		//The controll state

        GameControll.ControllGameState gControllState;
		//Controlls how fast the player can shoot. 8[1/3] times per second
		float fRefire;

		float fRefireTime = 0.12f;
		//Controlls how fast the player can shoot. 8[1/3] times per second
		float fEnemySpawn;

		float fEnemySpawnTime = 7f;
		//Controlls how long the intro goes for
		float fIntroFadeIn = 0.5f;
		float fIntroFadeOut = 0.5f;

		float fIntroTotalTime = 5f;
		//Menu Buttons
		MenuButton btnStartGame;

		MenuButton btnExitGame;
		#endregion
		#region "Main Functions"

		private void gLoadContent()
		{

			gTextures.Add(LoadTexture("Content/Textures/Entity/Spinner/Spinner_PartB.png"));
			//0
			gTextures.Add(LoadTexture("Content/Textures/Entity/Arrow/ship.png"));
			//1
			gTextures.Add(LoadTexture("Content/Textures/Misc/overlay.png"));
			//2
			gTextures.Add(LoadTexture("Content/Textures/Misc/background.png"));
			//3
			gTextures.Add(LoadTexture("Content/Textures/Misc/cursor.png"));
			//4
			gTextures.Add(LoadTexture("Content/Textures/Entity/Spinner/Spinner_PartA.png"));
			//5
			gTextures.Add(LoadTexture("Content/Textures/Misc/logo.png"));
			//6
			gTextures.Add(LoadTexture("Content/Textures/Engine/overdraw1x1.png"));
			//7
			gTextures.Add(LoadTexture("Content/Textures/Entity/Explosion/Explosion_PartA.png"));
			//8
			gTextures.Add(LoadTexture("Content/Textures/Entity/Revolver/Revolver_PartC.png"));
			//9
			gTextures.Add(LoadTexture("Content/Textures/Entity/Revolver/Revolver_PartB.png"));
			//10
			gTextures.Add(LoadTexture("Content/Textures/Entity/Revolver/Revolver_PartA.png"));
			//11
			gTextures.Add(LoadTexture("Content/Textures/Misc/Loading.png"));
			//12
			gTextures.Add(LoadTexture("Content/Textures/Entity/Bullet/Bullet_PartA.png"));
			//13
			gTextures.Add(LoadTexture("Content/Textures/Misc/menustrip_play.png"));
			//14
			gTextures.Add(LoadTexture("Content/Textures/Misc/menustrip_exit.png"));
			//15
			gTextures.Add(LoadTexture("Content/Textures/Entity/Arrow/gun.png"));
			//16
			gTextures.Add(LoadTexture("Content/Textures/Entity/Pulser/Pulser_PartA.png"));
			//17
			gTextures.Add(LoadTexture("Content/Textures/Entity/Pulser/Pulser_PartB.png"));
			//18
			gTextures.Add(LoadTexture("Content/Textures/Entity/Reticle.png"));
			//19
			//gViewport.WindowBorder = WindowBorder.Fixed


			gViewport.CursorVisible = true;

			gViewport.Title = "Invaders Must Die!";
			gViewport.WindowState = WindowState.Maximized;

			//Enable OpenGL Alpha Blending
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			gGameState = GameState.Menu;

			//Create the controll game state for the Controll Objects
			gControllState = new GameControll.ControllGameState();
			gControllState.gPreviousKeyboardState = gPreviousKeyboardState;
			gControllState.gPreviousMouseState = gPreviousMouseState;

			btnStartGame = new MenuButton(new Vector2(630.0f, 615.0f), gTextures[14].Size, new int[] { gTextures[14].ID });
			btnExitGame = new MenuButton(new Vector2(630.0f, 655.0f), gTextures[14].Size, new int[] { gTextures[15].ID });
		}

		private void gUpdate(float delta)
		{
			//Update the Viewport
			gViewport.Update(delta);

			//Update the ControllState
			gControllState.gPreviousKeyboardState = gPreviousKeyboardState;
			gControllState.gPreviousMouseState = gPreviousMouseState;
			gControllState.gViewport = gViewport;

			//Update all of the KeyDown and KeyPress Events
			if (GetKeyPress(Key.Number4)) {
                gEffectLevel = (EffectLevel)GameMath.ClampInteger((int)gEffectLevel - 1, 1, (int)EffectLevel.Ultra);
			}

			if (gCurrentKeyboardState.IsKeyDown(Key.L)) {
				gViewport.ViewportRealSize += new Vector2(1f, 1f);
			}
			if (gCurrentKeyboardState.IsKeyDown(Key.K)) {
				gViewport.ViewportRealSize += new Vector2(1f, 1f);
			}
			if (GetKeyPress(Key.Number5)) {
                gEffectLevel = (EffectLevel)GameMath.ClampInteger((int)gEffectLevel + 1, 0, (int)EffectLevel.Ultra);
			}
			if (gCurrentKeyboardState.IsKeyDown(Key.Q)) {
				createParticle(ParticleType.RandomExplosion);
			}
			if (GetKeyPress(Key.E)) {
				createParticle(new Vector2(500, 500), ParticleType.PlayerExplosion);
			}

			//Determine how the game should update based on its current state
			switch (gGameState) {
				case GameState.Game:
					//Set the Viewport relative resolution
					//gViewport.ViewportRealSize = New Vector2(800.0F, 800.0F)

					bool b = false;
					//Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
					if (gGameEntitys.Count > 0) {
                        if (gGameEntitys[0].eEntity == GameObject.ObjectType.Player && gGameEntitys[0].eLifeState == GameObject.LifeState.Alive)
                        {
							gViewport.Position = (gGameEntitys[0].vPosition * gViewport.ViewportScale - new Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale;

							//Increment the players spawn time counter
							fSpawnTime += delta;

							//Run all player code below!!!

							//Is the client asking the player to accelerate, decelerate to idle.
							gGameEntitys[0].iAccelerating = 0;

							PlayerShip gPlayer = (PlayerShip)gGameEntitys[0];

							gPlayer.vAcceleration.X = 0;
							gPlayer.vAcceleration.Y = 0;

							if (gCurrentKeyboardState.IsKeyDown(Key.W)) {
								gGameEntitys[0].iAccelerating = 1;
								gPlayer.vAcceleration.Y -= 1;
							}

							if (gCurrentKeyboardState.IsKeyDown(Key.S)) {
								gGameEntitys[0].iAccelerating = 1;
								gPlayer.vAcceleration.Y += 1;
							}

							if (gCurrentKeyboardState.IsKeyDown(Key.A)) {
								gGameEntitys[0].iAccelerating = 1;
								gPlayer.vAcceleration.X -= 1;
							}

							if (gCurrentKeyboardState.IsKeyDown(Key.D)) {
								gGameEntitys[0].iAccelerating = 1;
								gPlayer.vAcceleration.X += 1;

							}
							gPlayer.vAcceleration = GameMath.NormalizeVector2(gPlayer.vAcceleration);

							//If gCurrentKeyboardState.IsKeyDown(Key.W) Then

							//ElseIf gCurrentKeyboardState.IsKeyDown(Key.S) Then
							//    gGameEntitys[0].iAccelerating = -1
							//Else
							//    gGameEntitys[0].iAccelerating = 0
							//End If

							if (gCurrentKeyboardState.IsKeyDown(Key.ShiftLeft)) {
								gGameEntitys[0].bBoosting = true;
							} else {
								gGameEntitys[0].bBoosting = false;
							}

							//Increment the refire counter
							fRefire += delta;
							//Is the client asking to fire, is the player allowes to shoot yet?
							if (gCurrentMouseState.LeftButton == ButtonState.Pressed & fRefire > fRefireTime) {
								PlayerShip p = (PlayerShip)gGameEntitys[0];
								const float fAngle = 4.5f;
								for (int i = 1; i <= p.iGunLevel; i++) {
									float fStartAngle = (-fAngle / 2) * p.iGunLevel;
                                    float fCurrentAngle = fStartAngle + fAngle * i;
									gGameEntitys.Add(new Bullet(gGameEntitys[0].vPosition, gViewport.MousePosition, new Vector2(40.0f, 20.0f), p, fCurrentAngle, new int[] { gTextures[13].ID }, gRandom, new Color4(1.0f, 0.66666f, 0.2f, 0.0f), 5.0f, GameObject.ParticleAlgorithm.Circle));
								}

								fRefire = 0f;
							}

							//Increment the enemy spawn counter.
							fEnemySpawn += delta;
							//If the player is alive and its time to spawn some enemies, spawn some enemies.
							if (fEnemySpawn > fEnemySpawnTime) {
								//Add 5 Spinners
								for (int iii = 1; iii <= 5; iii++) {
									//Generate a random position on the playboard.
                                    Vector2 vPosition = new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f);
									//Keep generating a new position until the distance is 100 units away from the player.
									while (GameMath.Vector2Distance(vPosition, gGameEntitys[0].vPosition) < 250) {
										//System.Diagnostics.Debug.WriteLine("HERE 111")
                                        vPosition = new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f);
									}
									gGameEntitys.Add(new Spinner(vPosition, new Vector2(60f, 60f), gRandom, new int[] {
										gTextures[0].ID,
										gTextures[5].ID
									}));
								}
								//Add 2 Revolvers
								for (int iii = 1; iii <= 2; iii++) {
									//Generate a random position on the playboard.
                                    Vector2 vPosition = new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f);
									//Keep generating a new position until the distance is 100 units away from the player.
									while (GameMath.Vector2Distance(vPosition, gGameEntitys[0].vPosition) < 250) {
                                        vPosition = new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f);
									}
									gGameEntitys.Add(new Revolver(vPosition, new Vector2(60f, 60f), gRandom, new int[] {
										gTextures[9].ID,
										gTextures[10].ID,
										gTextures[11].ID
									}));
								}
								//Add 2 Pulsers
								for (int iii = 1; iii <= 2; iii++) {
									//Generate a random position on the playboard.
                                    Vector2 vPosition = new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f);
									//Keep generating a new position until the distance is 100 units away from the player.
									while (GameMath.Vector2Distance(vPosition, gGameEntitys[0].vPosition) < 250) {
                                        vPosition = new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f);
									}
									gGameEntitys.Add(new Pulser(vPosition, new Vector2(60f, 60f), gRandom, new int[] {
										gTextures[17].ID,
										gTextures[18].ID
									}));
								}
								fEnemySpawn = 0f;
							}

						} else {
                            if (gGameEntitys[0].eLifeState == GameObject.LifeState.Dead)
                            {
                                //If the player is dead or not in the game yet
                                b = true;



                                //Clear all of the enemys
                                gGameEntitys.Clear();

                                //Something else or dead
                                if (fSpawnTime > fSpawnTimeMax)
                                {
                                    fSpawnTime = 0f;
                                    SpawnPlayer();
                                    fEnemySpawn = 5;
                                }
                            }

							//Center the viewport's position
							gViewport.Position = GameMath.Lerp(gViewport.Position, (new Vector2(500, 500) * gViewport.ViewportScale - new Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, delta * 3);
						}

					} else {
						b = true;
					}
					if (b) {
						gGameEntitys.Clear();
						gViewport.Position = GameMath.Lerp(gViewport.Position, (new Vector2(500, 500) * gViewport.ViewportScale - new Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, delta * 2);
						//Something else or dead
						if (fSpawnTime > fSpawnTimeMax) {
							fSpawnTimeMax = fSpawnTime;
						}
						fSpawnTime -= delta;
						if (fSpawnTime < -1) {
							SpawnPlayer();
							fSpawnTime = 0f;
						}
					}

					//Create a new Quadtree to calculate the Collisions
					gQuadTree = new QuadTree<int>(new Vector2(0, 0), 0);

					//"Try" and Iterate through all of the objects and Update
					try {
						GameObject[] gGameObjectsTemp_1 = gGameEntitys.ToArray();
                        for (int i = gGameEntitys.Count - 1; i >= 0; i--)
                        {
							GameObject g = gGameObjectsTemp_1[i];
							if (g.eEntity.Equals(GameObject.ObjectType.Player)) {
								g.Update(delta, gRandom, gViewport.MousePosition);

							} else {
								g.Update(delta, gRandom, gGameEntitys[0].vPosition);
							}
							if (g.fLifespan >= g.fLifespanMax || g.eLifeState == GameObject.LifeState.Dead) {
								gGameEntitys.Remove(g);
							}
						}

                        for (int i = 0; i <= gGameEntitys.Count - 1; i++) {
                            gQuadTree.Insert(gGameEntitys[i].vPosition, i);
                        }

					#if DEBUG
					} catch (Exception ex) {
						Console.WriteLine("PROBLEM WITH QUADTREE COLLISIONS!!!");
						Console.WriteLine(ex.Message);
						#endif
						gQuadTree = null;
					}

					if ((gQuadTree != null)) {
                        //try {
                            //Iterate through all of the games objects and check their collisions
                            foreach (GameObject g in gGameEntitys.ToArray())
                            {
                                switch (g.eEntity) {
                                    case GameObject.ObjectType.Player:
                                        int[] CollisionIdsA = gQuadTree.GetWithin(g.vPosition, 100).ToArray();
                                        for (int i = 0; i <= CollisionIdsA.Length - 1; i++) {
                                            if (gGameEntitys[CollisionIdsA[i]].eEntity == GameObject.ObjectType.Enemy) {
                                                if ((gGameEntitys[CollisionIdsA[i]].eLifeState == GameObject.LifeState.Alive)) {
                                                    if (GameMath.Vector2Distance(gGameEntitys[CollisionIdsA[i]].vPosition, g.vPosition) < gGameEntitys[CollisionIdsA[i]].vSize.X - 10) {
                                                        //Destroy all objects
                                                        foreach (GameObject gg in gGameEntitys.ToArray()) {
                                                            if (!(gg.eEntity == GameObject.ObjectType.Bullet)) {
                                                                createParticle(gg.vPosition, ParticleType.EnemyExplosion);
                                                            } else {
                                                                createParticle(gg.vPosition, ParticleType.BulletExplosion);
                                                            }
                                                        }
                                                        createParticle(g.vPosition, ParticleType.PlayerExplosion);

                                                        for (int j = 0; j < gGameEntitys.Count; j++) 
                                                        {
                                                            gGameEntitys[j].eLifeState = GameObject.LifeState.Dying;
                                                        }
                                                        fSpawnTime = 0f;
                                                    }
                                                }
                                            }
                                        }

                                        break;
                                    case GameObject.ObjectType.Bullet:
                                        int[]  CollisionIdsB = gQuadTree.GetWithin(g.vPosition, 100).ToArray();
                                        for (int i = 0; i <= CollisionIdsB.Length - 1; i++) {
                                            if (gGameEntitys[CollisionIdsB[i]].eEntity == GameObject.ObjectType.Enemy) {

                                                if ((gGameEntitys[CollisionIdsB[i]].eLifeState == GameObject.LifeState.Alive)) {
                                                    float fRadius = gGameEntitys[CollisionIdsB[i]].vSize.X / 2;
                                                    if (GameMath.Vector2Distance(gGameEntitys[CollisionIdsB[i]].vPosition, g.vPosition) < Math.Sqrt(fRadius * fRadius + fRadius * fRadius)) {
                                                        //Destroy the enemy and the bullet
                                                        //createParticle(gGameEntitys[CollisionIdsB[i]].vPosition, ParticleType.Firework, GameObject.ParticleAlgorithm.Circle)
                                                        createParticle(g.vPosition, ParticleType.BulletExplosion, new Color4(1f, 0.35f, 0.125f, 0f));

                                                        g.eLifeState = GameObject.LifeState.Dead;

                                                        // Decrease enemy health
                                                        gGameEntitys[CollisionIdsB[i]].fHealth -= 30;
                                                        //gGameEntitys.RemoveAt(CollisionIdsB(i)) 'Remove entity
                                                    }
                                                }
                                            }
                                        }

                                        if (g.vPosition.X > 995f | g.vPosition.X < 5f | g.vPosition.Y > 995f | g.vPosition.Y < 5f) {
                                            createParticle(g.vPosition, ParticleType.BulletExplosion, new Color4(1f, 0.35f, 0.125f, 0f));

                                            g.eLifeState = GameObject.LifeState.Dead;

                                        } else {
                                        }
                                        break;
                                    case GameObject.ObjectType.Enemy:
                                        Enemy e = (Enemy)g;
                                        if ((e.bCreateExplosion)) {
                                            createParticle(g.vPosition, ParticleType.EnemyExplosion, e.eeExplosionEffect.cColor, e.eeExplosionEffect.fTime);
                                            e.bCreateExplosion = false;
                                        }
                                        if ((g.eLifeState == GameObject.LifeState.Dead)) {
                                            PlayerShip p = (PlayerShip)gGameEntitys[0];
                                            p.iKills += 1;
                                            //Create a bunch of smaller particles
                                            Vector2 v = new Vector2(0);
                                            for (int i = 0; i <= e.eeExplosionEffect.iDeathNumber; i++) {
                                                if ((e.eeExplosionEffect.fDeathRandomise != 0.0)) {
                                                    float a = (float)(gRandom.NextDouble() * Math.PI);
                                                    v.X = (float)Math.Cos(a);
                                                    v.Y = (float)Math.Sin(a);
                                                    v *= (float)(gRandom.NextDouble() * 0.9 + 0.1);
                                                    v *= e.eeExplosionEffect.fDeathRandomise;
                                                }
                                                createParticle(g.vPosition + v, ParticleType.EnemyExplosion, e.eeExplosionEffect.cColor, e.eeExplosionEffect.fDeathTime, e.eeExplosionEffect.fSpeed);
                                            }
                                        }

                                        break;
                                }
                        //    }
                        //#if DEBUG
                        //} catch (Exception ex) {
                        //    Console.WriteLine(ex.Message);
                        //    #endif
                        //}
					}
                        }

					break;
				case GameState.Menu:
					//Set the Viewport relative resolution
					gViewport.ViewportRealSize = new Vector2(1000f, 1000f);

					//Center the Viewport
					gViewport.Position = new Vector2(500f, 500f) * gViewport.ViewportScale;

					//Destroy any entitys on the screen to clear the game.
					gGameEntitys.Clear();

					//Spawning random fireworks
					float f = 1.0f / 10.0f * (float)(int)gEffectLevel;
					fEffectSpawn += delta;
					while (fEffectSpawn > f) {
						createParticle(ParticleType.RandomExplosion);
						fEffectSpawn -= f;
					}

					//Iterate through all the games objects
					foreach (GameObject g in gGameEntitys.ToArray()) {
						g.Update(delta, gRandom, gViewport.Position);
						if (g.fLifespan >= g.fLifespanMax) {
							gGameEntitys.Remove(g);
						}
					}


					//Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
					gViewport.Position = (new Vector2(500, 500) * gViewport.ViewportScale - new Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale;

					//Update the menu button
					btnStartGame.gControllGameState = gControllState;
					btnStartGame.Update(delta, gRandom, gViewport.MousePosition);
					btnExitGame.gControllGameState = gControllState;
					btnExitGame.Update(delta, gRandom, gViewport.MousePosition);


					if (gCurrentMouseState.LeftButton == ButtonState.Pressed) {
						if (btnStartGame.bIsHovering) {
							gGameState = GameState.Game;
							fSpawnTime = 0;
						}
						if (btnExitGame.bIsHovering) {
							Exits();
						}

					}

					break;
				case GameState.Loading:
					if (fTotalTime > fIntroTotalTime + 0.5f) {
						gGameState = GameState.Menu;
						fTotalTime = 0;
					} else {
						gViewport.Position = GameMath.Lerp(gViewport.Position, (new Vector2(500, 500) * gViewport.ViewportScale - new Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, delta * 2);
						gViewport.ViewportRealSize = new Vector2(gViewport.Width, gViewport.Height);
					}
					break;
			}
		}


		//This is a function called from the Update Thread of the Particle Update thread to iterate through all of the particles.
		//If this is run from the (updateThreadParticle) then the particle count is allowed to be between Off-Ultra else its manually set between Off-Low
		private void gUpdateEffects(float delta)
		{
			try {
				//Iterate through all the games objects [Particles]
				foreach (GameObject g in gGameEffects.ToArray()) {
					if (!g.Equals(null) | g.Equals(null)) {
						g.Update(delta, gRandom, gViewport.Position);
						if (g.fLifespan >= g.fLifespanMax) {
							gGameEffects.Remove(g);
						}
					}
				}
			#if DEBUG
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				#endif
			}
		}

		//The primary draw function
		//This draw function has been setup for 2D Rendering using 3d primatives
		private void gDraw(float delta)
		{
			//Overdraw the device with a 1x1 Pixel. Useful for reducing artifacts on the last draw call.
			DrawSprite.Draw2D(gViewport, gTextures[6].ID, new Vector2(float.MaxValue, float.MaxValue), new Vector2(1, 1));

			switch (gGameState) {
				case GameState.Game:
					//Draw the background
					Vector2 v = new Vector2(0, 0);
					if ((gGameEntitys.Count > 0)) {
						if (((gGameEntitys[0] != null))) {
							v = (gGameEntitys[0].vPosition - new Vector2(500.0f, 500.0f)) * 1.5f;
						}
					}

                    DrawSprite.Draw2dRotated(gViewport, gTextures[3].ID, new Vector2(500, 500) + v, gTextures[3].Size, 0);
                    DrawSprite.Draw2D(gViewport, gTextures[2].ID, new Vector2(0, 0), gTextures[2].Size);

					//Draw the game
					DrawParticles(delta);
					DrawEntitys(delta);

					//Draw the hud
					DrawSprite.Draw2D(gViewport, gTextures[4].ID, new Vector2(-12, -12) / gViewport.ViewportScale + gViewport.MousePosition, new Vector2(24, 24) / gViewport.ViewportScale);

					break;
				case GameState.Menu:
					if ((fTotalTime < 1)) {
						GL.Color4(new Color4(1f, 1f, 1f, fTotalTime));
					}

					//Draw the background
                    DrawSprite.Draw2D(gViewport, gTextures[3].ID, gViewport.Position / 2 - new Vector2(1000, 1000), gTextures[3].Size * 1.5f);

					//Draw the game
					DrawParticles(delta);

					if ((fTotalTime < 1)) {
						GL.Color4(new Color4(1f, 1f, 1f, fTotalTime));
					}

					//Draw logo
                    DrawSprite.Draw2D(gViewport, gTextures[6].ID, new Vector2(500f, 500f) - new Vector2(700f, 288.2353f) / 2, new Vector2(700f, 288.2353f));

					//Draw buttons
					btnStartGame.Draw(delta, gViewport);
					btnExitGame.Draw(delta, gViewport);

					//Draw the hud
                    DrawSprite.Draw2D(gViewport, gTextures[4].ID, new Vector2(-12, -12) / gViewport.ViewportScale + gViewport.MousePosition, new Vector2(24, 24) / gViewport.ViewportScale);

					break;
				case GameState.Loading:
					//Draw Intro Screen
					if (fTotalTime > fIntroTotalTime - fIntroFadeIn) {
						GL.Color4(new Color4(1f, 1f, 1f, 1f - Convert.ToSingle((fTotalTime - (fIntroTotalTime - fIntroFadeIn)) / fIntroFadeOut)));
					} else {
						GL.Color4(new Color4(1f, 1f, 1f, Convert.ToSingle(fTotalTime / fIntroFadeIn) - 1f));
					}
					//Fill the playing area with image.
                    DrawSprite.Draw2D(gViewport, gTextures[12].ID, new Vector2(0, 0), new Vector2(1000, 1000));
					GL.Color4(Color4.White);
					break;
			}

			//Overdraw the device with a 1x1 Pixel. Useful for reducing artifacts on the last draw call.
            DrawSprite.Draw2D(gViewport, gTextures[6].ID, new Vector2(float.MaxValue, float.MaxValue), new Vector2(1, 1));
		}

		#endregion
		#region "Secondary Functions"

		//Draws the particles
		public void DrawParticles(float delta)
		{
			//Iterate through all the games objects [Particles]
			try {
				foreach (GameObject g in gGameEffects.ToArray()) {
					//Select the type of object and do the appropriate update for it
					if (!g.Equals(null) | g.Equals(null)) {
						g.Draw(delta, gViewport);
					}
				}
			#if DEBUG
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				#endif
			}
			//After drawing all of the particles reset the Drawing Color to White
			GL.Color4(1f, 1f, 1f, 1f);
		}

		//Draw the entitys
		public void DrawEntitys(float delta)
		{
			//Iterate through all the games objects
            //try {
				foreach (GameObject g in gGameEntitys.ToArray()) {
					//Select the type of object and do the appropriate update for it
					g.Draw(delta, gViewport);

					if ((g.eEntity == GameObject.ObjectType.Enemy)) {
						Vector2 TPosInit = g.vPosition;
						g.Update(delta, gRandom, gGameEntitys[0].vPosition);
                        Vector2 TPosFinal = g.vPosition;
						g.vPosition = TPosInit;

                        float TVelocity = g.fSpeed;
						Vector2 I = GameMath.NormalizeVector2(TPosFinal - TPosInit) * TVelocity;

                        Vector2 BPosInit = gGameEntitys[0].vPosition;
                        float BVelocity = 750f;


						// Square Target Velocity, Subtract Bullet Velocityu
                        float a = (TVelocity * TVelocity) - (BVelocity * BVelocity);

						Vector2 D = TPosInit - BPosInit;

                        float b = 2.0f * (D.X * I.X + D.Y * I.Y);

                        float c = (D.X * D.X + D.Y * D.Y);

                        float det = b * b - 4.0f * a * c;
						float time = 0.0f;


						if (det < 0) {
						}

						if (det > 0) {
							det = (float)Math.Sqrt(det);
							time = -0.5f * (b - det) / a;
							if ((time < 0)) {
								time = -0.5f * (b + det) / a;
							}
						} else {
							time = -0.5f * b / a;
						}
                        Vector2 result = TPosInit + I * time;

                        //Use additive blending with transparency
						//GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
						
                        //DrawSprite.Draw2dRotated(gViewport, gTextures[19].ID, result, gTextures[19].Size, 0);
						//GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					}
                //}
            //#if DEBUG
            //} catch (Exception ex) {
            //    Console.WriteLine(ex.Message);
            //    #endif
			}
		}

		//Spawns the Player is the client has died or is beginning the game.
		public void SpawnPlayer()
		{
			if (!(gGameEntitys.Count == 0)) {
				gGameEntitys[0] = new PlayerShip(new Vector2(500, 500), new Vector2(60, 60), new int[] {
					gTextures[1].ID,
					gTextures[16].ID
				});
			} else {
				gGameEntitys.Add(new PlayerShip(new Vector2(500, 500), new Vector2(60, 60), new int[] {
					gTextures[1].ID,
					gTextures[16].ID
				}));
			}
		}

		public TextureID LoadTexture(string path)
		{
			TextureID tid = new TextureID();
			//Load image to a bitmap
			Bitmap Bitmap = new Bitmap(path);

			int texture = 0;
			//Generate texture
			GL.GenTextures(1, out texture);
			GL.BindTexture(TextureTarget.Texture2D, texture);

			// Store texture size
			tid.Width = Bitmap.Width;
			tid.Height = Bitmap.Height;
			tid.ID = texture;

			BitmapData data = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			Bitmap.UnlockBits(data);

			//Setup filtering
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.Linear));
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMagFilter.Linear));

			return tid;
		}

		private bool GetKeyPress(Key _Key)
		{
			if (gCurrentKeyboardState.IsKeyDown(_Key) & !gPreviousKeyboardState.IsKeyDown(_Key)) {
				return true;
			}
			return false;
		}

		private void createParticle(ParticleType _ParticleType)
		{
            createParticle(new Vector2((float)gRandom.NextDouble() * 900.0f + 50.0f, (float)gRandom.NextDouble() * 900.0f + 50.0f), _ParticleType, ZeroColorAlpha(RandomColor4()));
		}

		private void createParticle(Vector2 _Position, ParticleType _ParticleType)
		{
			createParticle(_Position, _ParticleType, ZeroColorAlpha(RandomColor4()));
		}
		//New Color4(1.0F, 0.35F, 0.125F, 0.0F)
		private void createParticle(Vector2 _Position, ParticleType _ParticleType, Color4 _Color)
		{
			createParticle(_Position, _ParticleType, _Color, 1.0f);
		}

		private void createParticle(Vector2 _Position, ParticleType _ParticleType, Color4 _Color, float _Time)
		{
			createParticle(_Position, _ParticleType, _Color, _Time, 150);
		}
		private void createParticle(Vector2 _Position, ParticleType _ParticleType, Color4 _Color, float _Time, float _Speed)
		{
			switch (_ParticleType) {
				case ParticleType.RandomExplosion:
					if (gGameEffects.Count < (float)(int)gEffectLevel * 25.0f) {
						if (bUsingEffectThread) {
							//If running from the bParticleThread
							//Create a new Explosion Particle with whatever settings are supplied
                            if(gGameEffects.Count < (int)gEffectLevel * 3) gGameEffects.Add(new Explosion(_Position, gRandom, new int[] { gTextures[8].ID }, (float)(int)gEffectLevel, ZeroColorAlpha(RandomColor4()), 2f, false, _Speed));
						} else {
							//If Not running from the bParticleThread
							//Create a new Explosion Particle with whatever settings are supplied with the exception the maximum particles can only be on Low
                            if (gGameEffects.Count < (int)gEffectLevel * 3) gGameEffects.Add(new Explosion(_Position, gRandom, new int[] { gTextures[8].ID }, GameMath.ClampInteger((int)gEffectLevel, 0, (int)EffectLevel.Low), ZeroColorAlpha(RandomColor4()), 2f, false, _Speed));
						}
					}
					break;
				case ParticleType.PlayerExplosion:
                    gGameEffects.Add(new Explosion(_Position, gRandom, new int[] { gTextures[8].ID }, (float)(int)gEffectLevel * 5, new Color4(1f, 1f, 1f, 0f), 5f, true, _Speed));
					break;
				case ParticleType.EnemyExplosion:
                    if (gGameEffects.Count < (int)gEffectLevel * 3) gGameEffects.Add(new Explosion(_Position, gRandom, new int[] { gTextures[8].ID }, 6, _Color, _Time, true, _Speed));
					break;
				case ParticleType.BulletExplosion:
                    if (gGameEffects.Count < (int)gEffectLevel * 5) gGameEffects.Add(new Explosion(_Position, gRandom, new int[] { gTextures[8].ID }, 3, _Color, 0.35f, true, _Speed));
					break;
				//gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures[8].ID}, 0.0F * gEffectLevel, New Color4(1.0F, 0.35F, 0.125F, 0.0F), 2.0F, Explosion.ParticleAlgorithm.Spread))
			}
		}

		//Returns a RandomColor from the array cColors
		private Color4 RandomColor4()
		{
			return cColors[GameMath.ClampInteger((int)(gRandom.NextDouble() * cColors.Length - 1), 0, cColors.Length - 1)];
		}
		//Returns a color with a premultiplied alpha value of 0
		private Color4 ZeroColorAlpha(Color4 _Color)
		{
			_Color.A = 0;
			return _Color;
		}

		#endregion
	}
}
