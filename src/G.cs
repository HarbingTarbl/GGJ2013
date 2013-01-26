using System;
using System.Collections.Generic;
using System.Linq;
using GGJ2013.Collision;
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
	public partial class G 
		: Game
	{
		public static ContentManager C;
		public static SpriteBatch SpriteBatch;
		public static CollisionRenderer CollisionRenderer;
		public static StateManager StateManager;
		public static GraphicsDeviceManager Graphics;
		public static bool DebugCollision = false;

		public static readonly int SCREEN_WIDTH = 1280;
		public static readonly int SCREEN_HEIGHT = 720;

		protected override void Initialize()
		{
			LoadContent();
		}

		protected override void LoadContent()
		{
			Content.RootDirectory = "Content";

			Graphics = new GraphicsDeviceManager (this);
			Graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			Graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			Graphics.IsFullScreen = false;
			Graphics.ApplyChanges();

			IsMouseVisible = true;
			CollisionRenderer = new CollisionRenderer (GraphicsDevice);
			StateManager = new StateManager();
			SpriteBatch = new SpriteBatch (GraphicsDevice);
			C = Content;

			LoadRealContent();
			//LoadTestContent();
		}

		private void LoadRealContent()
		{
			LoadAreaTent();
			StateManager.Set ("Tent");
		}

		private void LoadTestContent()
		{
			var debug1 = new BaseMemoryState ("Debug1", "Debug2", "None");
			debug1.Texture = Content.Load<Texture2D> ("Debug1");
			debug1.NavMesh = new Polygon (
				new Vector2 (0, 0),
				new Vector2 (0, 5),
				new Vector2 (1, 7),
				new Vector2 (2, 9),
				new Vector2 (5, 14));

			var item = new ReminderItem ("Circle", Content.Load<Texture2D> ("item1"));
			item.CollisionData = new Circlegon (30);
			item.Location = new Vector2 (250, 200);
			debug1.Items.Add (item);
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
			GraphicsDevice.Clear(Color.CornflowerBlue);
			StateManager.Draw(SpriteBatch);

			base.Draw(gameTime);
		}
	}
}
