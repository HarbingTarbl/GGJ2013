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
			: base("Forest", "Camp", "Camp")
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
				new Vector2 (714, 560),
				new Vector2 (828, 380),
				new Vector2 (928, 380),
				new Vector2 (929, 542),
				new Vector2 (866, 653));

			var p6 = new Polygon (
				new Vector2 (928, 541),
				new Vector2 (2032, 502),
				new Vector2 (2026, 702),
				new Vector2 (866, 653));

			var p1n = new PolyNode(p1);
			var p2n = new PolyNode(p2);
			var p3n = new PolyNode(p3);
			var p4n = new PolyNode(p4);
			var p5n = new PolyNode(p5);
			var p6n = new PolyNode(p6);
			
			PolyLink.AttachLinks(277, 350, ref p1n, ref p2n);
			PolyLink.AttachLinks(268, 414, ref p2n, ref p3n);
			PolyLink.AttachLinks(505, 521, ref p3n, ref p4n);
			PolyLink.AttachLinks (744, 514, ref p4n, ref p5n);
			PolyLink.AttachLinks (891, 581, ref p5n, ref p6n);

			Nav = new List<PolyNode> {
				p1n,
				p2n,
				p3n,
				p4n,
				p5n,
				p6n
			};
			#endregion

			
			light = new RenderTarget2D(G.Graphics.GraphicsDevice, G.SCREEN_WIDTH, G.SCREEN_HEIGHT, false, SurfaceFormat.Color,
			                           DepthFormat.None);
			Flashlight = CreateSprite("ForestArea/flashlight_beam");
			Lightmask = new Sprite
			{
				IsVisible = true,
				Texture = light,
			};

			pixel = new Texture2D(G.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			pixel.SetData<Color>(new Color[] {Color.White});

			//Shoe = CreateItem("Shoe");
			Foreground = CreateSprite ("ForestArea/foreground");

			Shoe = CreateItem ("Sarah's Shoe", "", "ForestArea/shoe", "UI/Icons/shoe", 326, 467,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			Branch = CreateItem ("Bloody Broken Branch", "", "ForestArea/branch", "UI/Icons/branch", 784, 410,
			    new Vector2 (815 - 784, 383 - 410),
			    new Vector2 (761 - 784, 501 - 410),
			    new Vector2 (814 - 784, 511 - 410),
			    new Vector2 (916 - 784, 435 - 410));

			ThornBush = CreateItem ("Bush of Thorns", "", "ForestArea/bush", "UI/Icons/papers", 830, 283,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			CoverBrush1 = CreateItem ("CB1", "", "ForestArea/coverbush", "UI/Icons/papers", 1230, 489,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());
			CoverBrush2 = CreateItem ("CB2", "", "ForestArea/coverbush", "UI/Icons/papers", 1463, 486,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());
			CoverBrush3 = CreateItem ("CB13", "", "ForestArea/coverbush", "UI/Icons/papers", 1666, 493,
				new Rectagon (0, 0, 53, 35).Vertices.ToArray ());

			Shoe.IsActive = true;
			Shoe.CanPickup = true;

			Branch.IsActive = true;
			Branch.CanPickup = true;

			ThornBush.IsActive = false;
			ThornBush.CanPickup = false;
			ThornBush.MouseHover = false;

			CoverBrush1.IsActive = false;
			CoverBrush2.IsActive = false;
			CoverBrush3.IsActive = false;

			ForestExit = new Hotspot (
				"Tent Entrance",
				new Polygon (
				new Vector2(4, 271),
				new Vector2(3, 4),
				new Vector2(330, 114),
				new Vector2(205, 258)),
				(t, i) =>
				{
					G.LastScreen = "Forest";
					G.FadeOut.Finished = () =>
					{
						G.FadeOut.Reset ();
						G.StateManager.Set ("Camp");
						G.FadeIn.TriggerStart();
					};
					G.FadeOut.TriggerStart();
				}) { WalkLocation = new Vector2 (170, 279) };

			SarahSpot = new Hotspot (
				"Sarah's Body",
				new Polygon (
				new Vector2(1545, 551),
				new Vector2(1839, 548),
				new Vector2(1871, 659),
				new Vector2(1552, 638)),
				(t, i) =>
				{
				}) { WalkLocation = new Vector2 (170, 279) };

			ShrubSpot = new Hotspot (
				"Chop down thorny bushes",
				new Polygon (
					new Vector2(938, 596),
					new Vector2(1021, 343),
					new Vector2(2046, 483),
					new Vector2(2036, 709)),
				(t, i) =>
				{
					if (i != null && i.Name == "Machete")
					{
						CoverBrush1.IsVisible = false;
						CoverBrush2.IsVisible = false;
						CoverBrush3.IsVisible = false;
						ThornBush.IsVisible = false;
						ThornBush.IsActive = false;
						ThornBush.MouseHover = false;
						SarahSpot.Enabled = true;
						ShrubSpot.Enabled = false;

						G.DialogManager.PostMessage ("You chopped down the thorny bushes", TimeSpan.Zero, new TimeSpan (0, 0, 3));
						foreach (var item in Items)
						{
							item.IsActive = true;
						}
					}
					else
					{
						G.DialogManager.PostMessage ("I need something to chop these bushes down", TimeSpan.Zero, new TimeSpan (0, 0, 3));
					}
				});

			var exitidle = new Animation ("Idle",
			   new[]
				{
					new Rectangle(0, 0, 200, 200),
					new Rectangle(200, 0, 200, 200),
					new Rectangle(400, 0, 200, 200),
					new Rectangle(600, 0, 200, 200),
					new Rectangle(800, 0, 200, 200)
				}, Looping: true);
			ExitLight = new AnimatedSprite (G.C.Load<Texture2D> ("ForestArea/exit_animation"), new [] { exitidle});
			ExitLight._IHateRectangles = new Rectangle (0, 0, 200, 200);
			ExitLight.IsVisible = true;
			ExitLight.Location = new Vector2 (0, 0);
			ExitLight.AnimationManager.SetAnimation ("Idle");

			SarahSpot.Enabled = false;
			ShrubSpot.Enabled = true;

			Items.Add (Shoe);
			Items.Add (Branch);
			Items.Add (ThornBush);
			Items.Add (CoverBrush1);
			Items.Add (CoverBrush2);
			Items.Add (CoverBrush3);

			Hotspots.Add (ForestExit);
			Hotspots.Add (ShrubSpot);
			Hotspots.Add (SarahSpot);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			ExitLight.Update (gameTime);
		}

		protected override void DrawBottomLayer(SpriteBatch batch)
		{
			ExitLight.Draw (batch);
		}

		protected override void DrawTopLayer (SpriteBatch batch)
		{
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

			BeginDraw (batch, BlendState.NonPremultiplied, false);
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

		public AnimatedSprite ExitLight;
		public Sprite Flashlight;
		public Sprite Lightmask;

		public GameItem CoverBrush1;
		public GameItem CoverBrush2;
		public GameItem CoverBrush3;
		public GameItem ThornBush;
		public GameItem Branch;
		public GameItem Shoe;

		public Hotspot ForestExit;
		public Hotspot SarahSpot;
		public Hotspot ShrubSpot;

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
