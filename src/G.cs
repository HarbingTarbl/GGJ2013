using System;
using System.Collections.Generic;
using System.Linq;
using GGJ2013.Collision;
<<<<<<< HEAD
using GGJ2013.Graphics;
=======
using GGJ2013.Entities;
>>>>>>> 6e5fbb69ad5be00bbe83d97d4cc441429a472fd4
using GGJ2013.Items;
using GGJ2013.States;
using Jammy;
using Jammy.Collision;
using Jammy.Parallax;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GGJ2013
{
	public class G 
		: Game
	{
		public G()
		{
			Graphics = new GraphicsDeviceManager (this);
		}

		public static ContentManager C;
		public static SpriteBatch SpriteBatch;
		public static CollisionRenderer CollisionRenderer;
		public static StateManager StateManager;
		public static GraphicsDeviceManager Graphics;
<<<<<<< HEAD
		public static BloomComponent BloomRenderer;
		public static GameTime GameTime;
		public static bool DebugCollision = false;
=======
		public static bool DebugCollision = true;
		public static string LastScreen;
		
		public static Player Player;
>>>>>>> 6e5fbb69ad5be00bbe83d97d4cc441429a472fd4

		public static readonly int SCREEN_WIDTH = 1280;
		public static readonly int SCREEN_HEIGHT = 720;

		protected override void LoadContent()
		{
			Content.RootDirectory = "Content";

			Graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			Graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			Graphics.IsFullScreen = false;
			Graphics.ApplyChanges();


			IsMouseVisible = true;
			CollisionRenderer = new CollisionRenderer (GraphicsDevice);
			StateManager = new StateManager();
			SpriteBatch = new SpriteBatch (GraphicsDevice);
			C = Content;
			BloomRenderer = new BloomComponent(this);
			BloomRenderer.LoadContent();


			Player = new Player();

			StateManager.Add (new TentState());
			StateManager.Set ("Tent");
		}

		protected override void UnloadContent()
		{
			BloomRenderer.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;

			BloomRenderer.Settings =
				BloomSettings.PresetSettings[
				                             Keyboard.GetState().IsKeyDown(Keys.NumPad0)
					                             ? 0
					                             : Keyboard.GetState().IsKeyDown(Keys.NumPad1)
						                               ? 1
						                               : Keyboard.GetState().IsKeyDown(Keys.NumPad2)
							                                 ? 2
							                                 : Keyboard.GetState().IsKeyDown(Keys.NumPad3)
								                                   ? 3
								                                   : Keyboard.GetState().IsKeyDown(Keys.NumPad4)
									                                     ? 4
									                                     : Keyboard.GetState().IsKeyDown(Keys.NumPad5) ? 5 : 0
					];

			if (Keyboard.GetState().IsKeyDown(Keys.F2))
			{
				BloomRenderer.ShowBuffer = BloomComponent.IntermediateBuffer.PreBloom;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.F3))
			{
				BloomRenderer.ShowBuffer = BloomComponent.IntermediateBuffer.FinalResult;
			}



			StateManager.Update(gameTime);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.Black);
			StateManager.Draw (SpriteBatch);

			base.Draw(gameTime);
		}
	}
}
