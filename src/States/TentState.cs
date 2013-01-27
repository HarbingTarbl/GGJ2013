using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Items;
using GGJ2013.States;
using Jammy;
using Jammy.Collision;
using Jammy.Helpers;
using Jammy.Sprites;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GGJ2013
{
	public class TentState
		: MemoryState
	{
		public TentState()
			: base ("Tent", "Tent", "None")
		{
			Background = G.C.Load<Texture2D> ("TentArea/background");
			#region NavMesh
			var p1 = new Polygon (
				new Vector2 (106, 469),
				new Vector2 (997, 480),
				new Vector2 (896, 594),
				new Vector2 (627, 646),
				new Vector2 (345, 644),
				new Vector2 (159, 605));

			var p2 = new Polygon (
				new Vector2 (106, 769),
				new Vector2 (997, 780),
				new Vector2 (896, 894),
				new Vector2 (627, 946),
				new Vector2 (345, 944),
				new Vector2 (159, 905));

			var p1n = new PolyNode (p1);
			var p2n = new PolyNode (p2);

			PolyLink.AttachLinks (3, 0, ref p1n, ref p2n);

			Nav = new List<PolyNode> {
				p1n,
				p2n
			};
			#endregion

			Blanket = CreateItem ("Blanket", "A warm blanket", "TentArea/blanket", 200, 500,
				new Vector2 (0, 41),
				new Vector2 (256, -6),
				new Vector2 (419, 91),
				new Vector2 (296, 153));

			Flash = CreateItem ("Flashlight", "A bright flashlight", "TentArea/flashlight", 587, 436,
				new Vector2 (568 - 587, 493 - 436),
				new Vector2 (575 - 587, 427 - 436),
				new Vector2 (630 - 587, 433 - 436),
				new Vector2 (634 - 587, 484 - 436));

			Sweater = CreateItem("Sweater", "A Sweater", "TentArea/sweater", 390, 550,
				new Vector2 (426 - 500, 563 - 550),
				new Vector2 (513 - 500, 551 - 550),
				new Vector2 (653 - 500, 559 - 550),
				new Vector2 (639 - 500, 594 - 550),
				new Vector2 (566 - 500, 615 - 550),
				new Vector2 (496 - 500, 603 - 550));

			Matches = CreateItem ("Matches", "A set of matches", "TentArea/matches", 850, 500,
				new Vector2 (830 - 850, 505 - 500),
				new Vector2 (892 - 850, 491 - 500),
				new Vector2 (905 - 850, 516 - 500),
				new Vector2 (844 - 850, 532 - 500));

			Blanket.OnClick += t =>
			{
				Blanket.IsActive = false;
				Blanket.Texture = G.C.Load<Texture2D> ("TentArea/tossedblanket");
				Sweater.IsActive = true;
			};

			Matches.IsActive = false;
			Matches.IsVisible = false;
			Blanket.IsActive = true;
			Blanket.CanPickup = false;
			Sweater.CanPickup = true;
			Flash.IsActive = true;
			Flash.CanPickup = true;

			Bag = new Hotspot (
				"Duffle Bag",
				new Polygon (
					new Vector2 (757, 443),
					new Vector2 (883, 417),
					new Vector2 (902, 489),
					new Vector2 (776, 507),
					new Vector2 (745, 483)
					), t =>
					   {
						   Matches.IsVisible = true;
						   Matches.IsActive = true;
						   Matches.CanPickup = true;
					   });

			Exit = new Hotspot(
				"Tent Exit",
				new Polygon(
					new Vector2 (620 + 90, 250),
					new Vector2 (607 + 90, 373),
					new Vector2 (575 + 90, 459),
					new Vector2 (650 + 90, 468),
					new Vector2 (675 + 90, 374),
					new Vector2 (657 + 90, 299)), t =>
					{
						if (CanLeaveLevel)
						{
							G.StateManager.Pop();
							G.StateManager.Push (NextLevel);
						}
						else
						{
							G.DialogManager.PostMessage("I should put on some clothes... ", TimeSpan.Zero, new TimeSpan(0, 0, 5));
							G.DialogManager.PostMessage("and grab my flashlight.", new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 5));
						}
					});

			ItemsToLeave.Add("Sweater");
			ItemsToLeave.Add("Flashlight");

			lantern = CreateSprite ("TentArea/lantern", 510, 89);
			light1 = CreateSprite ("TentArea/light1");
			light2 = CreateSprite ("TentArea/light2");
			glow = CreateSprite ("TentArea/lanternGlow", 370, 140);
			border = CreateSprite ("TentArea/border");

			LanternSpot = new Hotspot("Unlit Lantern",
				new Circlegon(545, 315, 64), t =>
			{
				LanternSpot.Name = "Lit Lantern";
				light1.IsVisible = false;
				light2.IsVisible = true;
				//glow.IsVisible = true;
			});

			light1.IsVisible = true;
			light2.IsVisible = false;
			glow.IsVisible = false;

			Items.Add (Flash);
			Items.Add (Sweater);
			Items.Add (Matches);
			Items.Add (Blanket);

			Lights.Add (lantern);
			Lights.Add (light1);
			Lights.Add (light2);
			Lights.Add (glow);
			Lights.Add (border);

			Hotspots.Add (Exit);
			Hotspots.Add (LanternSpot);
			Hotspots.Add (Bag);
		}

		public override bool HandleInput(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				G.DialogManager.PostQueuedMessage("Spaaaaaace", new TimeSpan(0, 0, 5));
			}

			return base.HandleInput(gameTime);
		}

		private Sprite lantern;
		private Sprite light1;
		private Sprite light2;
		private Sprite glow;
		private Sprite border;

		public GameItem Sweater;
		public GameItem Blanket;
		public GameItem Flash;
		public GameItem Matches;

		public Hotspot LanternSpot;
		public Hotspot Exit;
		public Hotspot Bag;

		protected override void OnLevelStart (string LastScreen)
		{
			if (String.IsNullOrEmpty (LastScreen))
				RunIntroCinematics();
		}

		private void RunIntroCinematics()
		{
			G.Player.Location = new Vector2 (440, 495);
			//TODO: Put player in laying down animation
			//TODO: fade in?
			//TODO: Wait for player to play stand up animation
			//TODO: play any dialog we have
		}
	}
}
