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
	public class G 
		: Game
	{

		public static G Instance;
		public static SpriteBatch SpriteBatch;
		public static CollisionRenderer CollisionRenderer;
		public static StateManager StateManager;
		public static CameraSingle Camera;
		public static GraphicsDeviceManager Graphics;
		public static bool DebugCollision = false;

		public G()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			Instance = this;
			IsMouseVisible = true;
			CollisionRenderer = new CollisionRenderer (GraphicsDevice);
			StateManager = new StateManager ();
			SpriteBatch = new SpriteBatch (GraphicsDevice);

			var debug1 = new BaseMemoryState("Debug1", "Debug2", "None");
			debug1.Texture = Content.Load<Texture2D>("Debug1");

			var item = new ReminderItem("Circle", Content.Load<Texture2D>("item1"));
			item.CollisionData = new Circlegon(30);
			item.Location = new Vector2(150, 200);
			debug1.Items.Add(item);



			var debug2 = new BaseMemoryState("Debug2", "Debug3", "Debug1");
			debug2.Texture = Content.Load<Texture2D>("Debug2");

			var debug3 = new BaseMemoryState("Debug3", "Debug1", "Debug2");
			debug3.Texture = Content.Load<Texture2D>("Debug3");
			debug1.Hotspots.Add(new ActivePolygon(new Rectagon(30, 30, 50, 50), t => StateManager.Push(debug2)));

			debug2.Hotspots.Add(new ActivePolygon(new Rectagon(60, 90, 100, 200), t => StateManager.Push(debug3)));

			debug3.Hotspots.Add(new ActivePolygon(new Rectagon(10, 100, 50, 300), t => StateManager.Push(debug1)));

			StateManager.Push(debug3);
			StateManager.Push(debug2);
			StateManager.Push(debug1);

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
			SpriteBatch.Begin(SpriteSortMode.Deferred,
				BlendState.NonPremultiplied,
				SamplerState.PointClamp,
				DepthStencilState.None,
			    RasterizerState.CullNone,
				null,
				Camera.Transformation);

			StateManager.Draw(SpriteBatch);
			SpriteBatch.End();

			if (DebugCollision)
			{
				var state = (BaseMemoryState) StateManager.CurrentState;
				if (state == null)
					return;

				CollisionRenderer.Begin(Camera.Transformation);
				foreach(var item in state.Items)
				{
					CollisionRenderer.Draw(item, Color.Lime);
				}
				foreach (var hotspot in state.Hotspots)
				{
					CollisionRenderer.DrawPolygon(hotspot, Color.Red);
				}
				CollisionRenderer.Stop();
			}
			base.Draw(gameTime);
		}
	}
}
