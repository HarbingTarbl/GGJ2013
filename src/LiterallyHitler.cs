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
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class LiterallyHitler 
		: Game
	{
		GraphicsDeviceManager graphics;

		public static LiterallyHitler Instance;
		public static SpriteBatch SpriteBatch;
		public static CollisionRenderer CollisionRenderer;
		public static StateManager StateManager;
		public static CameraSingle Camera;
		public static bool DebugCollision = false;



		static LiterallyHitler()
		{
			Instance = new LiterallyHitler();
		}


		private LiterallyHitler()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			CollisionRenderer = new CollisionRenderer(GraphicsDevice);
			StateManager = new StateManager();
			SpriteBatch = new SpriteBatch(GraphicsDevice);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.

			var state = new HitlerGameState("InterTest", "InterTest");

			state.Texture = (Content.Load<Texture2D>("tent_interior_concept_sketch"));
			StateManager.Push(state);


			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();


			StateManager.Update(gameTime);
			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None,
			                  RasterizerState.CullNone, null, Camera.Transformation);
			StateManager.Draw(SpriteBatch);
			SpriteBatch.End();

			if (DebugCollision)
			{
				var state = (HitlerGameState) StateManager.CurrentState;
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
			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
