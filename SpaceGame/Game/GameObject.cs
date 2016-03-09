#region "Imports"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;


//Import parts of the OpenTK Framework
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

//Import parts of the Isotope Framework
using Isotope;
using Isotope.Library;

#endregion
//This is the master class for all of the games objects. It includes the functions avaliable to all objects aswell as 
//any variables that are specificaly required by any object.
public class GameObject
{
	#region "Enumeration & Structures"

	public enum ObjectType
	{
		Player = 0,
		Enemy = 1,
		Bullet = 2,
		Other = 3
	}
	public enum ParticleAlgorithm
	{
		Spread = 0,
		Circle = 1
	}

	public enum LifeState
	{
		Alive,
		Dying,
		Dead
	}

	public struct Particle
	{

		//Array to hold all of the Particle positions

		public Vector2 position;
		//Array to hold the Particle vMovement normals.
		public Vector2 direction;
		// :: PRE CALCULATED VARIABLES ::
		public Vector2 movement;

		public float rotation;
		//Array to hold all of the Particles speed.
		public float speed;
	}

	#endregion
	#region "Variables and Properties"

	//Stores the position of the Object

	public Vector2 vPosition;
	//Stores the size of the Object

	public Vector2 vSize;
	//Stores an array of integers used to indicate which textures the object should draw

	public int[] iTextureIdentification;
	//Specifies the type of explosion this object will create/is

	public ParticleAlgorithm eParticleAlgorithm;
	//Specifies the Rotation of the object in Radians

	public float fRotation;
	//Specifies the current speed of the object

	public float fSpeed;
	//Specifies the maximum current speed of the object

	public float fSpeedMax;
	//Specifies the maximum acceleration of the object

	public float fAcceleration;
	//Specifies if the object is accelerating

	public int iAccelerating = 0;
	//Specifies if the object is boosting

	public bool bBoosting = false;
	//Specifies the boosting acceleration

	public float fAccelerationBoost;
	//Specifies the maximum speed when boosting

	public float fBoostSpeedMax;
	//Specifies the vMovement normal of the object, [Normalized]

	public Vector2 vMovement = new Vector2(0, 0);
	//Specifies the objects current lifespan

	public float fLifespan;
	//Specifies how long the objects lifespan is

	public float fLifespanMax = float.MaxValue;
	//Specifies the type of object

	public ObjectType eEntity = ObjectType.Other;
	//Specifies the current life state of the object

	public LifeState eLifeState = LifeState.Alive;
	//Specifies how much health the object has left
	public float fHealth = 100.0f;

	public float fMaxHealth = 0;
	//Specifies how long it takes to die
	public float fDieTime = 1.0f;

	public float fDieTimeAccumulator = 0.0f;
	//Create's a percentage of how long the object will live.
	public float fLifespanPercentage {
		get { return fLifespan / fLifespanMax; }
	}

	//Unused Variables for reference
	//public fDumbness As Single
	//public eBonus As *BONUSENUM*
	//public iScore As Integer

	#endregion
	#region "Initializers"

	public GameObject(Vector2 _Position, Vector2 _Size, int[] _TextureID)
	{
		vPosition = _Position;
		vSize = _Size;
		iTextureIdentification = _TextureID;
	}

	#endregion
	#region "Main Methods"

	//The Overridable Update Function which is avaliable to all objects
	public virtual void Update(float delta, Random gRandom, Vector2 _Target)
	{
	}
	//The Overridable Draw Function which is avaliable to all objects
	public virtual void Draw(float delta, Viewport gViewport)
	{
	}

	#endregion
}

public class PlayerShip : GameObject
{

	#region "Enumeration & Structures"

	#endregion
	#region "Variables and Properties"

	public Vector2 vAcceleration = new Vector2(0);
	public int iKills;
		#endregion
	public int iGunLevel;
	#region "Initializers"

	public PlayerShip(Vector2 _Position, Vector2 _Size, int[] _TextureID) : base(_Position, _Size, _TextureID)
	{

		//Specifies that this object is a "Player"
		eEntity = ObjectType.Player;

		//Defines the Acceleration of the Object
		fAcceleration = 125f;

		//Defines the extra boost acceleration of the Object
		fAccelerationBoost = 50f;

		//Defines the maximum normal speed
		fSpeedMax = 350f;

		//Defines the extra boost speed of the Object
		fBoostSpeedMax = 150f;
		iGunLevel = 1;
	}

	#endregion
	#region "Main Methods"
	public override void Update(float delta, Random gRandom, Vector2 _Target)
	{
        switch (eLifeState)
        {
            case LifeState.Alive:
                //If bosting then accelerate with boost speed else use normal speed and check if going to fast.
                if (bBoosting)
                {
                    fSpeed = GameMath.ClampFloat(fSpeed + (fAcceleration + fAccelerationBoost) * delta, 0, fSpeedMax + fBoostSpeedMax);
                }
                else
                {
                    if (fSpeed > fSpeedMax)
                    {
                        fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration * delta, 0, fSpeedMax + fBoostSpeedMax);
                    }
                    else
                    {
                        //Determine if accelerating, idle or in reverse.
                        switch (iAccelerating)
                        {
                            case 1:
                                fSpeed = GameMath.ClampFloat(fSpeed + (fAcceleration + fAccelerationBoost) * delta, 0, fSpeedMax);
                                break;
                            case 0:
                                fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration / 5 * delta, 0, fSpeedMax + fBoostSpeedMax);
                                break;
                            case -1:
                                fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration * delta, 0, fSpeedMax + fBoostSpeedMax);
                                break;
                        }
                    }
                }

                //Weapon Level
                if (iKills > 20)
                {
                    iGunLevel = 2;
                    if (iKills > 40)
                    {
                        iGunLevel = 3;
                        if (iKills > 75)
                        {
                            iGunLevel = 4;
                            if (iKills > 100)
                            {
                                iGunLevel = 5;
                            }
                        }
                    }
                }

                Vector2 v = _Target - vPosition;
                if ((v.X == 0 & v.Y == 0))
                {
                    v = new Vector2(0, 1);
                }
                v.Normalize();
                //Calculate the rotation to draw with
                fRotation = v.Rotation() + (float)(Math.PI / 2.0);

                //Smooth the vMovement of the players object
                //vMovement = GameMath.Lerp(vMovement, v, delta * 8)

                vMovement = GameMath.Lerp(vMovement, vAcceleration, delta * 8);

                //Calculate the vMovement
                vPosition = GameMath.ClampVector(vPosition + vMovement * (delta * fSpeed), vSize / 2, new Vector2(1000f) - vSize / 2);
                break;
            default:
                fDieTimeAccumulator += delta;
                if (fDieTimeAccumulator >= fDieTime) eLifeState = LifeState.Dead;
                break;
        }
	}

	public override void Draw(float delta, Viewport gViewport)
	{
        if(eLifeState == LifeState.Alive)
        {
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            //Use additive blending with transparency
            DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize, 0);
            DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[1], vPosition, vSize, fRotation);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //Use linear transparency blending [default]
        }
	}
	#endregion
}

public class Enemy : GameObject
{
	#region "Enumeration & Structures"
	public struct ExplosionEffect
	{
		public Color4 cColor;
		public float fTime;
		public float fDeathRandomise;
		public int iDeathNumber;
		public float fDeathTime;

		public float fSpeed;
		public ExplosionEffect(Color4 color, float time, float speed, float deathrandomise, int deathnumber, float deathtime)
		{
			cColor = color;
			fTime = time;
			fDeathRandomise = deathrandomise;
			iDeathNumber = deathnumber;
			fDeathTime = deathtime;
			fSpeed = speed;
		}
	}

	#endregion
	#region "Variables and Properties"

	public ExplosionEffect eeExplosionEffect = new ExplosionEffect(new Color4(1f, 1f, 1f, 1f), 0.5f, 50, 0, 1, 1);

	public bool bCreateExplosion = false;

	public float fHealthPercent = 1.0f;

    public Random _Random;
	#endregion
	#region "Initializers"

	public Enemy(Vector2 _Position, Vector2 _Size, Random _Random, int[] _TextureID) : base(_Position, _Size, _TextureID)
	{
        this._Random = _Random;

		//Add extra code below
		eEntity = ObjectType.Enemy;

		if ((eLifeState == LifeState.Dead)) {
			bCreateExplosion = true;
		}
	}

	#endregion
	#region "Main Methods"

	//Override Update statement.
	public override void Update(float delta, Random gRandom, Vector2 _Target)
	{
		fHealthPercent = fHealth / fMaxHealth;
	}

	public override void Draw(float delta, Viewport gViewport)
	{
	}

	#endregion
}

public class Revolver : Enemy
{

	#region "Enumeration & Structures"

	#endregion
	#region "Variables and Properties"
	bool[] bCanHaveExplosion = {
		true,
		true
		#endregion
	};
	#region "Initializers"

	public Revolver(Vector2 _Position, Vector2 _Size, Random _Random, int[] _TextureID) : base(_Position, _Size, _Random, _TextureID)
	{
        fRotation = (float)(_Random.NextDouble() * 100.0);

		//Set the Entity's acceleration to 150
		fAcceleration = 150f;
		//Set the maximum speed of the Entity to 150
		fSpeedMax = 90.0f + (float)(_Random.NextDouble() * 120.0);

		fDieTime = 0.55f;

		fMaxHealth = 133.0f + (float)(_Random.NextDouble() * 20.0);
		fHealth = fMaxHealth;

		//Add extra code below
		eEntity = ObjectType.Enemy;

		eeExplosionEffect = new ExplosionEffect(new Color4(0.1f, 1.0f, 0.1f, 1.0f), 0.5f, 75.0f, 100.0f, 5, 0.4f);
	}

	#endregion
	#region "Main Methods"

	//Override Update statement.
	public override void Update(float delta, Random gRandom, Vector2 _Target)
	{
		base.Update(delta, gRandom, _Target);

		switch (eLifeState) {
			case LifeState.Alive:
				// :: ALIVE ::
				//Accelerate the object and clamp its speed to the maximum.
				fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * delta, 0, fSpeedMax);

				//Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
				vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), delta);

				//Calculate and clamp the position.
				vPosition = GameMath.ClampVector(vPosition + vMovement * delta * fSpeed, vSize / 2, new Vector2(1000f) - vSize / 2);

				if ((fHealth <= 0)) {
					eLifeState = GameObject.LifeState.Dying;
				}

				//Spin the entity around and change it a little so it dosen't mimic every other entity.
                fRotation += delta * fHealthPercent;

				break;
			case LifeState.Dying:
				// :: DYING/DEAD ::
				vSize += delta * 150;
				fDieTimeAccumulator += delta;
				if ((fDieTimeAccumulator >= fDieTime)) {
					eLifeState = LifeState.Dead;
				}
				break;
		}

		if ((fHealthPercent < 0.65)) {
			if ((bCanHaveExplosion[0])) {
				bCanHaveExplosion[0] = false;
				bCreateExplosion = true;
			}

			if ((fHealthPercent < 0.35)) {
				if ((bCanHaveExplosion[1])) {
					bCanHaveExplosion[1] = false;
					bCreateExplosion = true;
				}
			}
		}
	}

	public override void Draw(float delta, Viewport gViewport)
	{
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
		//Use additive blending with transparency
		//atleast 80%
		if ((fHealthPercent > 0.65)) {
            DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize * fHealthPercent, -fRotation * 2f);
		}
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[1], vPosition, vSize * (float)Math.Max(fHealthPercent, 0.75), fRotation * 1.5f);
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[2], vPosition, vSize, 0);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		//Use linear transparency blending [default]
	}

	#endregion
}

public class Spinner : Enemy
{

	#region "Enumeration & Structures"

	#endregion
	#region "Variables and Properties"
		#endregion
	bool[] bCanHaveExplosion = { true };
	#region "Initializers"

	public Spinner(Vector2 _Position, Vector2 _Size, Random _Random, int[] _TextureID) : base(_Position, _Size, _Random, _TextureID)
	{
        fRotation = (float)(_Random.NextDouble() * 100.0);

		//Set the Entity's acceleration to 150
		fAcceleration = 150f;
		//Set the maximum speed of the Entity to 150
		fSpeedMax = 80.0f + (float)(_Random.NextDouble() * 80.0);

		fDieTime = 0.55f;

		fMaxHealth = 53.0f + (float)(_Random.NextDouble() * 10.0);
		fHealth = fMaxHealth;
		//Add extra code below
		eEntity = ObjectType.Enemy;

		eeExplosionEffect = new ExplosionEffect(new Color4(1.0f, 0.1f, 0.1f, 1.0f), 0.5f, 75.0f, 0.0f, 1, 0.8f);
	}

	#endregion
	#region "Main Methods"

	public override void Update(float delta, Random gRandom, Vector2 _Target)
	{
		base.Update(delta, gRandom, _Target);

		switch (eLifeState) {
			case LifeState.Alive:
				// :: ALIVE ::
				//Accelerate the object and clamp its speed to the maximum.
				fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * delta, 0, fSpeedMax);

				//Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
				vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), delta);

				//Calculate and clamp the position.
				vPosition = GameMath.ClampVector(vPosition + vMovement * delta * fSpeed, vSize / 2, new Vector2(1000f) - vSize / 2);

				if ((fHealth <= 0)) {
					eLifeState = GameObject.LifeState.Dying;
				}

				//Spin the entity around and change it a little so it dosen't mimic every other entity.
                fRotation += delta * (fHealthPercent * 0.75f + 0.25f) * 0.666f;

				break;
			case LifeState.Dying:
				// :: DYING/DEAD ::
				vSize += delta * 150;
				fDieTimeAccumulator += delta;
				if ((fDieTimeAccumulator >= fDieTime)) {
					eLifeState = LifeState.Dead;
				}
				break;
		}

		if ((fHealthPercent < 0.5)) {
			if ((bCanHaveExplosion[0])) {
				bCanHaveExplosion[0] = false;
				bCreateExplosion = true;
			}
		}

	}

	public override void Draw(float delta, Viewport gViewport)
	{
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
		//Use additive blending with transparency
		//Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize, fRotation)
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize * (float)Math.Max(fHealthPercent, 0.6), fRotation);
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[1], vPosition, vSize, 0);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		//Use linear transparency blending [default]
	}

	#endregion
}

public class Pulser : Enemy
{

	#region "Enumeration & Structures"

	#endregion
	#region "Variables and Properties"
	bool[] bCanHaveExplosion = { true };
		#endregion
	float fPulseTime;
	#region "Initializers"

	public Pulser(Vector2 _Position, Vector2 _Size, Random _Random, int[] _TextureID) : base(_Position, _Size, _Random, _TextureID)
	{

		//Set the Entity's acceleration to 150
		fAcceleration = 150f;
		//Set the maximum speed of the Entity to 150
		fSpeedMax = 80.0f + (float)(_Random.NextDouble() * 80.0);

		fDieTime = 0.55f;

		fMaxHealth = 23.0f + (float)(_Random.NextDouble() * 10.0);
		fHealth = fMaxHealth;
		//Add extra code below
		eEntity = ObjectType.Enemy;

		eeExplosionEffect = new ExplosionEffect(new Color4(1f, 0.55f, 0.1f, 1f), 0.5f, 80.0f, 0.0f, 1, 0.4f);

		fPulseTime += (float)_Random.NextDouble() * 100.0f;
	}

	#endregion
	#region "Main Methods"

	public override void Update(float delta, Random gRandom, Vector2 _Target)
	{
		base.Update(delta, gRandom, _Target);

		switch (eLifeState) {
			case LifeState.Alive:
				// :: ALIVE ::
				//Accelerate the object and clamp its speed to the maximum.
				fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * delta, 0, fSpeedMax);

				//Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
				vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), delta);

				//Calculate and clamp the position.
				vPosition = GameMath.ClampVector(vPosition + vMovement * delta * fSpeed, vSize / 2, new Vector2(1000f) - vSize / 2);

				if ((fHealth <= 0)) {
					eLifeState = GameObject.LifeState.Dying;
				}

				//Spin the entity around and change it a little so it dosen't mimic every other entity.
                fRotation += delta * (float)((0.5f + _Random.NextDouble()) * 4.5) * fHealthPercent;

				break;
			case LifeState.Dying:
				// :: DYING/DEAD ::
				vSize += delta * 150;
				fDieTimeAccumulator += delta;
				if ((fDieTimeAccumulator >= fDieTime)) {
					eLifeState = LifeState.Dead;
				}
				break;
		}

		if ((fHealthPercent < 0.5)) {
			if ((bCanHaveExplosion[0])) {
				bCanHaveExplosion[0] = false;
				bCreateExplosion = true;
			}
		}

	}

	public override void Draw(float delta, Viewport gViewport)
	{
		fPulseTime += delta * (float)(Math.PI / (2.0f * 2.0f)) * 3.0f;

		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
		//Use additive blending with transparency
		//Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize, fRotation)
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize, 0);
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[1], vPosition, vSize * (float)Math.Max(fHealthPercent * Math.Abs(Math.Sin(fPulseTime)), 0.6), fRotation);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		//Use linear transparency blending [default]
	}

	#endregion
}

public class Bullet : GameObject
{

	#region "Enumeration & Structures"

	#endregion
	#region "Variables and Properties"

	//The color of the particles

	Color4 cColorTarget;
	//The current drawing color of the particles.

	Color4 cDrawColor;

	Vector2 vInertia;


	#endregion
	#region "Initializers"

	public Bullet(Vector2 _Position, Vector2 _MousePosition, Vector2 _Size, PlayerShip _Parent, float angle, int[] _TextureID, System.Random gRandom, Color4 _ColorTarget, float _Lifespan, ParticleAlgorithm _Algorithm) : base(_Position, _Size, _TextureID)
	{

		//Define the acceleration of the of the object
		fSpeed = 750f;
		fSpeedMax = fSpeed + 100f;
		//fSpeed = 0.0F 'Inertia test

		//Define the lifespan of the particle.
		fLifespanMax = 5f;

		//Define the color of the particle.
		cColorTarget = _ColorTarget;

		//Define the type of object as "Other"
		eEntity = ObjectType.Bullet;

		//Define an algorithm to use with the particles.
		eParticleAlgorithm = _Algorithm;

		//Assign the objects movement.
		Vector2 vTargetDirection = _MousePosition - _Position;
		double fTargetAngle = Math.Atan2((double)vTargetDirection.Y, (double)vTargetDirection.X) + (angle / 180);
		double fRandomAngle = ((gRandom.NextDouble() - 0.5) * 2.0) * (3.0 / 180.0) * Math.PI;
		//Random Angle between +- 3 degrees
		vMovement.X = (float)Math.Cos(fTargetAngle + fRandomAngle);
		vMovement.Y = (float)Math.Sin(fTargetAngle + fRandomAngle);

		vInertia = _Parent.vMovement * _Parent.fSpeed;
	}

	#endregion
	#region "Main Methods"


	public override void Update(float delta, System.Random gRandom, Vector2 _Movement)
	{
		//vPosition += ((vInertia + vMovement * fSpeed)) * delta
		vPosition += ((vMovement * fSpeed)) * delta;
		fRotation = vMovement.Rotation();

		//Calculate the current color of the particle object.
		cDrawColor.R = GameMath.Lerp(cColorTarget.R / 2 + 0.5f, cColorTarget.R, fLifespanPercentage);
		cDrawColor.G = GameMath.Lerp(cColorTarget.G / 2 + 0.5f, cColorTarget.G, fLifespanPercentage);
		cDrawColor.B = GameMath.Lerp(cColorTarget.B / 2 + 0.5f, cColorTarget.B, fLifespanPercentage);
		cDrawColor.A = GameMath.Lerp(1f, cColorTarget.A, fLifespanPercentage);

		//Add time to the particles current Lifespan.
		fLifespan += delta;
	}

	public override void Draw(float delta, Viewport gViewport)
	{
		// :: SHIP ::
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
		//Use additive blending with transparency
		//Draw the bullet with the specified color
		GL.Color4(cDrawColor);

		//GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.OneMinusDstAlpha)

		//Draw the object with rotation and a texture. Also rotate the texture 180 degrees.
        DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[0], vPosition, vSize, fRotation + (float)Math.PI);

		//Reset the drawing color back to the default
		GL.Color4(Color4.White);

		// :: ENGINE PARTICLES ::


		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		//Use linear transparency blending [default]
	}

	#endregion
}

public class Explosion : GameObject
{

	#region "Enumeration & Structures"

	#endregion
	#region "Variables and Properties"
	//Arrray to hold all of the Particles

    Particle[] vParticles;
	//The color of the particles

	Color4 cColorTarget;
	//The current drawing color of the particles.

	Color4 cDrawColor;
	//The size the particles for drawing.

	Vector2 vDrawSize = new Vector2(18f, 2f);

	bool bCollisionCheck;

	float fSpeedMul = 2.0f;
	#endregion
	#region "Initializers"

	public Explosion(Vector2 _Position, Random _Random, int[] _TextureID, float _ParticleLevel, Color4 _ColorTarget, float _Lifespan, bool _CollisionCheck, float _Speed) : base(_Position, new Vector2(0), _TextureID)
	{

		bCollisionCheck = _CollisionCheck;
		// Assign the collision checking parameter

		//Calculate the amount of particles that will be created.
		float fRavg = (1.5f * 200.0f / 100.0f) * _Lifespan;
		//Calculate the average radius of the spread
		double fC = 2.0 * Math.PI * fRavg;
		//Calculate the average circumference
		int iParticleCount = (int)GameMath.ClampInteger((int)(fC * _ParticleLevel), 0, int.MaxValue) - 1;
		iParticleCount *= 3;

		// Create an array to store all the particles
		vParticles = new Particle[iParticleCount + 1];

		for (int i = 0; i <= iParticleCount; i++) {
			vParticles[i].position = vPosition;
			//Set starting position

			double angle = _Random.NextDouble() * 2.0 * Math.PI;
			//0-360 degrees in radians
			vParticles[i].direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			//Set the direction as a vector

			vParticles[i].speed = (float)(_Random.NextDouble() + 1) * _Speed;

			vParticles[i].movement = vParticles[i].direction * vParticles[i].speed;
			vParticles[i].rotation = vParticles[i].movement.Rotation();
		}





		//'Create 50*x Positions, Speed and Movement values for each particle.
		//vPositions = New Vector2(iParticleCount) {}
		//vSpeed = New Single(iParticleCount) {}
		//vParticleMovement = New Vector2(iParticleCount) {}
		//For i As Integer = 0 To vParticleMovement.Length - 1
		//    vSpeed(i) = (_Random.NextDouble() + 1) * 200
		//    vParticleMovement(i) = New Vector2(_Random.NextDouble, _Random.NextDouble) - 0.5F
		//    vParticleMovement(i).Normalize()
		//    vPositions(i) = vPosition
		//Next

		//Define the fastest speed of the particles
		fSpeedMax = 400f;

		//Define the lifespan of the particle.
		fLifespanMax = _Lifespan;

		//Define the color of the particle.
		cColorTarget = _ColorTarget;
		cColorTarget.A = 0;

		//Define the type of object as "Other"
		eEntity = ObjectType.Other;

		//Define an algorithm to use with the particles.
		//eParticleAlgorithm = _Algorithm
	}

	#endregion
	#region "Main Methods"

	public override void Update(float delta, System.Random gRandom, Vector2 _Target)
	{
		//Iterate through all of the particles inside of this Object
		//For i As Integer = 0 To vParticles.Length - 1
		//    'Select Case eParticleAlgorithm
		//    '    Case ParticleAlgorithm.Spread
		//    '        'Slow the particle by a 7th of the current particles speed. X = X-X(delta/7)
		//    '        vSpeed(i) = GameMath.ClampFloat(vSpeed(i) - Math.Sqrt(vSpeed(i)) * delta / 3.5F, 0.1F, fSpeedMax)
		//    '    Case ParticleAlgorithm.Circle
		//    '        'Use SpreadRoot Algorithm
		//    '        vSpeed(i) = GameMath.ClampFloat(vSpeed(i) - Math.Sqrt(vSpeed(i)) * delta * fLifespanPercentage * 250, fSpeedMax * (1 - fLifespanPercentage), fSpeedMax)
		//    'End Select


		//    'Check if the particle collides with the boundary on the X axis.
		//    If vPositions(i).X > 995.0F Or vPositions(i).X < 5.0F Then
		//        vParticleMovement(i).X *= -1
		//    Else
		//        'Check if the particle collides with the boundary on the Y axis.
		//        If vPositions(i).Y > 995.0F Or vPositions(i).Y < 5.0F Then
		//            vParticleMovement(i).Y *= -1
		//        End If
		//    End If
		//    vPositions(i) = GameMath.ClampVectorSingle(vPositions(i) + (vParticleMovement(i) * vSpeed(i) * delta), 4.0F, 4.0F, 996.0F, 996.0F)
		//Next

		fSpeedMul -= delta * 1.75f;
		fSpeedMul = GameMath.ClampFloat(fSpeedMul, 1, float.MaxValue);

		for (int i = 0; i <= vParticles.Length - 1; i++) {
			//Check if the particle collides with the boundary on the X axis.

			if ((bCollisionCheck)) {
				if (vParticles[i].position.X >= 995f | vParticles[i].position.X <= 5f) {
					vParticles[i].movement.X *= -1;
					vParticles[i].rotation = vParticles[i].movement.Rotation();
				} else {
					//Check if the particle collides with the boundary on the Y axis.
					if (vParticles[i].position.Y >= 995f | vParticles[i].position.Y <= 5f) {
						vParticles[i].movement.Y *= -1;
						vParticles[i].rotation = vParticles[i].movement.Rotation();
					}
				}
				//vParticles[i].position = GameMath.ClampVectorSingle(vParticles[i].position + (vParticles[i].direction * vParticles[i].speed * delta), 4.0F, 4.0F, 996.0F, 996.0F)
				vParticles[i].position = GameMath.ClampVectorSingle(vParticles[i].position + (vParticles[i].movement * fSpeedMul * delta), 5f, 5f, 995f, 995f);
			} else {
				vParticles[i].position += (vParticles[i].movement * fSpeedMul * delta);
			}

		}

		//Calculate the current color of the particle object.
		cDrawColor.R = GameMath.ClampFloat(GameMath.Lerp((cColorTarget.R / 2 + 0.5f) * 2f, cColorTarget.R * 2f, fLifespanPercentage), 0, 1);
		cDrawColor.G = GameMath.ClampFloat(GameMath.Lerp((cColorTarget.G / 2 + 0.5f) * 2f, cColorTarget.G * 2f, fLifespanPercentage), 0, 1);
		cDrawColor.B = GameMath.ClampFloat(GameMath.Lerp((cColorTarget.B / 2 + 0.5f) * 2f, cColorTarget.B * 2f, fLifespanPercentage), 0, 1);
		cDrawColor.A = GameMath.Lerp(1f, cColorTarget.A, fLifespanPercentage);

		//Add time to the particles current Lifespan.
		fLifespan += delta;
	}

	public override void Draw(float delta, Viewport gViewport)
	{
		//Draw anything with the specified drawing color
		GL.Color4(cDrawColor);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
		//Use additive blending with transparency
		//Iterate through all of the particles and draw them.
		for (int i = 0; i <= vParticles.Length - 1; i++) {
            DrawSprite.Draw2dRotated(gViewport, iTextureIdentification[0], vParticles[i].position, vDrawSize, vParticles[i].rotation);
		}
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		//Use linear transparency blending [default]
	}

	#endregion
}
