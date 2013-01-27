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
				new Vector2 (715, 558),
				new Vector2(492, 555));


			var p5 = new Polygon (
				new Vector2 (768, 488),
				new Vector2 (2032, 502),
				new Vector2 (2026, 702),
				new Vector2 (866, 653),
				new Vector2 (715, 557));

			var p1n = new PolyNode(p1);
			var p2n = new PolyNode(p2);
			var p3n = new PolyNode(p3);
			var p4n = new PolyNode(p4);
			var p5n = new PolyNode (p5);
			
			PolyLink.AttachLinks(277, 350, ref p1n, ref p2n);
			PolyLink.AttachLinks(268, 414, ref p2n, ref p3n);
			PolyLink.AttachLinks(505, 521, ref p3n, ref p4n);
			PolyLink.AttachLinks (740, 526, ref p4n, ref p5n);

			Nav = new List<PolyNode> {
				p1n,
				p2n,
				p3n,
				p4n,
				p5n
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

			//Shoe = CreateItem("Shoe");
			Foreground = CreateSprite ("ForestArea/foreground");

			Shoe = CreateItem ("Sarah's Shoe", "", "ForestArea/shoe", "UI/Icons/papers", 326, 467,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			Branch = CreateItem ("Bloody Broken Branch", "", "ForestArea/branch", "UI/Icons/papers", 784, 410,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			ThornBush = CreateItem ("Bush of Thorns", "", "ForestArea/thornbush", "UI/Icons/papers", 830, 283,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			CoverBrush1 = CreateItem ("CB1", "", "ForestArea/coverbush", "UI/Icons/papers", 1230, 489,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());
			CoverBrush2 = CreateItem ("CB2", "", "ForestArea/coverbush", "UI/Icons/papers", 1463, 486,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());
			CoverBrush3 = CreateItem ("CB13", "", "ForestArea/coverbush", "UI/Icons/papers", 1666, 493,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			Items.Add (Shoe);
			Items.Add (Branch);
			Items.Add (ThornBush);
			Items.Add (CoverBrush1);
			Items.Add (CoverBrush2);
			Items.Add (CoverBrush3);

			Lights.Add (Foreground);
		}

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);

			return;
			var lightRect = new Rectangle (0, 0, Flashlight.Texture.Width, Flashlight.Texture.Height);
			GeomHelpers.CenterRectangle (ref lightRect, Flashlight.Location);

			var r1 = new Rectangle(0, 
				0,
				G.SCREEN_WIDTH,
				lightRect.Top);

			var r2 = new Rectangle(0,
				0,
			    lightRect.Left,
			    G.SCREEN_HEIGHT);

			var r3 = new Rectangle(
				lightRect.Right,
				0,
				G.SCREEN_WIDTH - lightRect.Right,
				G.SCREEN_HEIGHT);

			var r4 = new Rectangle (
				0,
				lightRect.Bottom,
				G.SCREEN_WIDTH,
				G.SCREEN_HEIGHT + lightRect.Bottom);

			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			batch.Draw (pixel, r1, Color.Black);
			batch.Draw (pixel, r2, Color.Black);
			batch.Draw (pixel, r3, Color.Black);
			batch.Draw (pixel, r4, Color.Black);
			batch.Draw (Flashlight.Texture, lightRect, Color.White);
			batch.End();
		}

		public override bool HandleInput(GameTime gameTime)
		{
			Flashlight.Location = new Vector2(_oldMouse.X, _oldMouse.Y);
			return base.HandleInput(gameTime);

		}

		public Sprite Foreground;

		public Sprite Flashlight;
		public Sprite Lightmask;

		public GameItem CoverBrush1;
		public GameItem CoverBrush2;
		public GameItem CoverBrush3;
		public GameItem ThornBush;
		public GameItem Branch;
		public GameItem Shoe;

		public Hotspot CampEntrance;
		public Hotspot DeadyBody;

		public Hotspot Bush;
		public Hotspot TreeBranch;

		protected override void OnLevelStart(string LastScreen)
		{
			Player.Location = new Vector2 (101, 249);
		}

		private RenderTarget2D light;
		private Texture2D pixel;
	}
}
