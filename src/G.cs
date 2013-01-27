using System;
using System.Collections.Generic;
using System.Linq;
using GGJ2013.Collision;
using GGJ2013.Entities;
using GGJ2013.Graphics;
using GGJ2013.Interface;
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
		public static CollisionRenderer Debug;
		public static StateManager StateManager;
		public static GraphicsDeviceManager Graphics;
		public static BloomComponent BloomRenderer;
		public static InventoryManager InventoryManager;
		public static DialogManager DialogManager;
		public static GameTime GameTime;
		public static bool DebugCollision = false;
		public static string LastScreen;	
		public static Player Player;
		public static bool Active;

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
			Debug = new CollisionRenderer (GraphicsDevice);
			StateManager = new StateManager();
			SpriteBatch = new SpriteBatch (GraphicsDevice);
			C = Content;
			BloomRenderer = new BloomComponent(this);
			BloomRenderer.LoadContent();
			InventoryManager = new InventoryManager();

			DialogManager = new DialogManager()
			{
				MessageBounds = new Rectangle(15, 15 + (int)InventoryManager.Bounds.Bottom, G.SCREEN_WIDTH, 300),
				Font = G.C.Load<SpriteFont>("fonts/debug"),
			};

			Player = new Player();

			StateManager.Add (new TentState());
			StateManager.Add (new CampState());
			StateManager.Add (new ForestState());
			StateManager.Set ("Forest");

			Activated += (s, a) => Active = true;
			Deactivated += (s, a) => Active = false;
		}

		protected override void UnloadContent()
		{
			BloomRenderer.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;
			BloomRenderer.Settings = Keyboard.GetState().IsKeyDown(Keys.NumPad0)
					                             ? BloomSettings.PresetSettings[0]
					                             : Keyboard.GetState().IsKeyDown(Keys.NumPad1)
						                               ? BloomSettings.PresetSettings[1]
						                               : Keyboard.GetState().IsKeyDown(Keys.NumPad2)
							                                 ? BloomSettings.PresetSettings[2]
							                                 : Keyboard.GetState().IsKeyDown(Keys.NumPad3)
								                                   ? BloomSettings.PresetSettings[3]
								                                   : Keyboard.GetState().IsKeyDown(Keys.NumPad4)
									                                     ? BloomSettings.PresetSettings[4]
									                                     : Keyboard.GetState().IsKeyDown(Keys.NumPad5) ? 
																			BloomSettings.PresetSettings[5] 
																			: BloomRenderer.Settings;

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
