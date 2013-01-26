using System;
using System.Collections.Generic;
using System.Linq;
using GGJ2013.Collision;
using GGJ2013.Entities;
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
		public static bool DebugCollision = true;
		public static string LastScreen;
		
		public static Player Player;

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

			Player = new Player();

			StateManager.Add (new TentState());
			StateManager.Set ("Tent");
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
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
