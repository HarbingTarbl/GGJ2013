using System;
using System.Collections.Generic;
using System.Linq;
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
		GraphicsDeviceManager graphics;

		public static G Instance;
		public static SpriteBatch SpriteBatch;
		public static CollisionRenderer CollisionRenderer;
		public static StateManager StateManager;
		public static CameraSingle Camera;
		public static GraphicsDeviceManager Graphics;
		public static bool DebugCollision = false;

		public G()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			Instance = this;
			Graphics = graphics;
			CollisionRenderer = new CollisionRenderer (GraphicsDevice);
			StateManager = new StateManager ();
			SpriteBatch = new SpriteBatch (GraphicsDevice);

			var state = new BaseMemoryState("InterTest", "InterTest");

			state.Texture = (Content.Load<Texture2D>("tent_interior_concept_sketch"));
			StateManager.Push(state);
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
				BlendState.Additive,
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
