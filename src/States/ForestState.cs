using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Items;
using Jammy.Collision;
using Jammy.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Jammy.Helpers;

namespace GGJ2013.States
{
	public class ForestState
		: MemoryState
	{
		public ForestState()
			: base("Forest", "WinRwar", "Camp")
		{
			Background = G.C.Load<Texture2D>("ForestArea/background");

			#region Nav Mesh
			var p1 = new Polygon(
				new Vector2(111, 233),
				new Vector2(339, 304),
				new Vector2(316, 367),
				new Vector2(71, 259));

			var p2 = new Polygon(
				new Vector2(316, 367),
				new Vector2(240, 449),
				new Vector2(160, 414),
				new Vector2(242, 336));

			var p3 = new Polygon(
				new Vector2(293, 390),
				new Vector2(241, 446),
				new Vector2(492, 555),
				new Vector2(515, 490));

			var p4 = new Polygon(
				new Vector2(515, 490),
				new Vector2(770, 488),
				new Vector2(964, 562),
				new Vector2(804, 550),
				new Vector2(492, 555));

			var p1n = new PolyNode(p1);
			var p2n = new PolyNode(p2);
			var p3n = new PolyNode(p3);
			var p4n = new PolyNode(p4);
			
			PolyLink.AttachLinks(276, 347, ref p1n, ref p2n);
			//PolyLink.AttachLinks(268, 416, ref p2n, ref p3n);
			//PolyLink.AttachLinks(503, 523, ref p3n, ref p4n);

			Nav = new List<PolyNode> {
				p1n,
				p2n,
				//p3n,
				//p4n
			};
			#endregion

			light = new RenderTarget2D(G.Graphics.GraphicsDevice, G.SCREEN_WIDTH, G.SCREEN_HEIGHT, false, SurfaceFormat.Color,
			                           DepthFormat.None);
			Flashlight = CreateSprite("flashlight_beam", 0, 0);
			Lightmask = new Sprite
			{
				IsVisible = true,
				Texture = light,
			};

			pixel = new Texture2D(G.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			pixel.SetData<Color>(new Color[] {Color.White});


		}

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);

			G.Graphics.GraphicsDevice.SetRenderTarget(null);
			//G.Graphics.GraphicsDevice.Clear(Color.Transparent);

			var screen = new Rectangle(0, 0, G.SCREEN_WIDTH, G.SCREEN_HEIGHT);
			var blocks = new Rectangle[4];

				//screen.Subtract(new Rectangle((int)Flashlight.Location.X, (int)Flashlight.Location.Y, Flashlight.Texture.Width,
				//						  Flashlight.Texture.Height));

			blocks[0] = new Rectangle(0, (int) Flashlight.Location.Y, (int) Flashlight.Location.X, Flashlight.Texture.Height);
			blocks[1] = new Rectangle((int)Flashlight.Location.X + Flashlight.Texture.Width, (int) Flashlight.Location.Y,
			                          G.SCREEN_WIDTH - (int) Flashlight.Location.X + Flashlight.Texture.Width,
			                          Flashlight.Texture.Height);

			blocks[2] = new Rectangle(0, 0, G.SCREEN_WIDTH, (int) Flashlight.Location.Y);
			blocks[3] = new Rectangle(0, (int) Flashlight.Location.Y, G.SCREEN_WIDTH,
			                          G.SCREEN_HEIGHT - (int) Flashlight.Location.Y + Flashlight.Texture.Height);

			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			for (var i = 3; i < blocks.Length; i++)
			{
				batch.Draw(pixel, blocks[i], Color.Black);
			}
			batch.Draw(Flashlight.Texture, Flashlight.Location - new Vector2(Flashlight.Texture.Width/2f, Flashlight.Texture.Height/2f), Color.White);
			
			batch.End();	
			G.Graphics.GraphicsDevice.SetRenderTarget(null);

		}

		public override bool HandleInput(GameTime gameTime)
		{
			Flashlight.Location = new Vector2(_oldMouse.X, _oldMouse.Y);
			return base.HandleInput(gameTime);

		}

		public Sprite Flashlight;
		public Sprite Lightmask;

		public GameItem Shoe;

		public Hotspot CampEntrance;
		public Hotspot DeadyBody;

		public Hotspot Bush;
		public Hotspot TreeBranch;

		protected override void OnLevelStart(string LastScreen)
		{
			Player.Location = new Vector2 (74, 232);
		}

		private RenderTarget2D light;
		private Texture2D pixel;
	}
}
