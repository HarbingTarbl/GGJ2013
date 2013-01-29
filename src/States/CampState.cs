using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Items;
using Jammy.Collision;
using Jammy.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GGJ2013.States
{
	public class CampState
		: MemoryState
	{
		public CampState()
			: base("Camp")
		{
			#region NavMesh

			var p1 = new Polygon (
				new Vector2(151, 274),
				new Vector2(331, 257),
				new Vector2(435, 384),
				new Vector2(215, 392));

			var p2 = new Polygon (
				new Vector2(4, 416),
				new Vector2(215, 395),
				new Vector2(434, 388),
				new Vector2(871, 405),
				new Vector2(904, 714),
				new Vector2(2, 717));

			var p3 = new Polygon (
				new Vector2 (1090 - 225, 406),
				new Vector2 (1563 - 225, 414),
				new Vector2 (1573 - 225, 533),
				new Vector2 (1300 - 225, 715),
				new Vector2 (1128 - 225, 714));

			var p4 = new Polygon (
				new Vector2(936, 398),
				new Vector2(1113, 287),
				new Vector2(1219, 295),
				new Vector2(1267, 402));
			
			var p1n = new PolyNode (p1);
			var p2n = new PolyNode (p2);
			var p3n = new PolyNode (p3);
			var p4n = new PolyNode (p4);

			PolyLink.AttachLinks (318, 391, ref p1n, ref p2n);
			PolyLink.AttachLinks (887, 555, ref p2n, ref p3n);
			PolyLink.AttachLinks (1119, 403, ref p3n, ref p4n);

			Nav = new List<PolyNode> {
				p1n,
				p2n,
				p3n,
				p4n,
			};
			#endregion

			Background = G.C.Load<Texture2D>("CampArea/background");
			Foreground = G.C.Load<Texture2D>("CampArea/foreground");

			Backpack = CreateItem("Backpack", "A torn backpack", "CampArea/backpack", 734, 410,
			                      new Rectagon(0, 0, 111, 59).Vertices.ToArray());

			Batteries = CreateItem("Batteries", "Your tongue hurts - they are supprisingly strong", "CampArea/batteries", "UI/Icons/batteries", 761, 480,
								new Rectagon(0, 0, 78, 32).Vertices.ToArray());

			EmptyWineBottle = CreateItem("Empty Wine Bottle", "It's empty", "CampArea/wine1", "UI/Icons/bottle", 137, 574,
				new Rectagon(0, 0, 24, 81).Vertices.ToArray());

			BrokenWineBottle = CreateItem("Broken Wine Bottle", "The neck of the bottle has been broken, likely due to a fall",
				"CampArea/wine2", "UI/Icons/bottle", 422, 615, new Rectagon(0, 0, 53, 35).Vertices.ToArray());

			Papers = CreateItem("Shrededd paper", "[TODO]", "CampArea/papers", "UI/Icons/papers", 822, 457, new Rectagon(0, 0, 53, 35).Vertices.ToArray());
			
			Boulder = CreateItem ("Boulder", "A heavy rock", "CampArea/boulder", "UI/Icons/papers", 480, 313);

			Machete = CreateItem ("Machete", "A knife used for cutting things down", "CampArea/machete", 560, 420, new Rectagon (0, 0, 150, 60).Vertices.ToArray());
				

			var fireIdle = new Animation ("Idle",
			   new[]
				{
					new Rectangle(0, 0, 450, 300),
					new Rectangle(450, 0, 450, 300),
					new Rectangle(900, 0, 450, 300),
					new Rectangle(1350, 0, 450, 300)
				}, Looping: true);
			FirepitAnimation = new AnimatedSprite (G.C.Load<Texture2D> ("CampArea/fire_animation"), new Animation[] { fireIdle });
			FirepitAnimation._IHateRectangles = new Rectangle (0, 0, 450, 300);
			FirepitAnimation.IsVisible = false;
			FirepitAnimation.Location = new Vector2 (0, 350);
			FirepitAnimation.AnimationManager.SetAnimation ("Idle");

			var fireflies_idle = new Animation ("Idle",
			   new[]
				{
					new Rectangle(0, 0, 200, 200),
					new Rectangle(200, 0, 200, 200),
					new Rectangle(400, 0, 200, 200),
					new Rectangle(600, 0, 200, 200),
					new Rectangle(800, 0, 200, 200),
					new Rectangle(1000, 0, 200, 200),
					new Rectangle(1200, 0, 200, 200),
					new Rectangle(1400, 0, 200, 200)
				}, Looping: true);
			FliesAnimation = new AnimatedSprite (G.C.Load<Texture2D> ("CampArea/fireflies_animation"), new [] { fireflies_idle});
			FliesAnimation._IHateRectangles = new Rectangle (0, 0, 200, 200);
			FliesAnimation.IsVisible = true;
			FliesAnimation.Location = new Vector2 (900, 325);
			FliesAnimation.AnimationManager.SetAnimation ("Idle");

			Boulder.IsActive = false;
			Boulder.CanPickup = false;
			Boulder.IsMouseHover = false;

			Machete.IsActive = false;
			Machete.CanPickup = false;
			Machete.IsMouseHover = false;

			Backpack.IsActive = true;
			Backpack.CanPickup = false;

			Batteries.IsActive = false;
			Batteries.CanPickup = false;
			Batteries.IsVisible = false;
			Batteries.IsMouseHover = false;

			EmptyWineBottle.IsActive = false;
			EmptyWineBottle.CanPickup = true;

			BrokenWineBottle.IsActive = false;
			BrokenWineBottle.CanPickup = true;
			BrokenWineBottle.IsVisible= false;

			Papers.IsActive = false;
			Papers.CanPickup = false;
			Papers.IsVisible = false;
			Papers.IsMouseHover = false;

			FirepitLight = CreateSprite("CampArea/light map 1", 0, 0);
			FirepitLight.IsVisible = false;
			TentLight = CreateSprite("CampArea/light map 1", 0, 0);

			#region HotSpots
			TentEntrance = new Hotspot (
				"Tent Entrance",
				new Polygon (new Vector2 (3, 277),
				new Vector2 (333, 247),
				new Vector2 (83, 107),
				new Vector2 (2, 107)),
				(t,i) =>
				{
					G.LastScreen = "Camp";
					G.FadeOut.Finished = () =>
					{
						G.FadeOut.Reset ();
						G.StateManager.Set ("Tent");
						G.FadeIn.TriggerStart ();
					};
					G.FadeOut.TriggerStart ();
				}) { WalkLocation = new Vector2 (170, 279) };

			CampExit = new Hotspot(
				"Camp Exit",
			    new Polygon(
				new Vector2(1087, 281),
				new Vector2(1075, 110),
				new Vector2(1277, 114),
				new Vector2(1268, 301)),
			    (t,i) =>
			    {
				    if (CanLeaveLevel)
				    {
						G.FadeOut.Finished += () =>
						{
							G.FadeOut.Reset ();
							G.StateManager.Set ("Forest");
							G.FadeIn.TriggerStart ();
						};
						G.FadeOut.TriggerStart ();
				    }
				    else
				    {
					    G.DialogManager.PostQueuedMessage("It's too dark, maybe if I had a flashlight or something.", new TimeSpan (0, 0, 3));
				    }
				}) { WalkLocation = new Vector2 (1170, 298) };

			Firepit = new Hotspot(
				"Light Fire Pit",
				new	Polygon(new Vector2(57, 497),
					new Vector2(50, 465),
					new Vector2(184, 412),
					new Vector2 (307, 458),
					new Vector2 (290, 503),
					new Vector2(194, 526),
					new Vector2(97, 518)),
				(t,i) =>
				{
					if (FirepitAnimation.IsVisible)
						return;

					if (i != null && i.Name == "Matches")
					{
						G.C.Load<SoundEffect> ("sfx/Match").Play();
						TentLight.IsVisible = false;
						FirepitAnimation.IsVisible = true;
						G.DialogManager.PostMessage("You have used the matches", TimeSpan.Zero, new TimeSpan(0, 0, 3));
						G.InventoryManager.CurrentItems.Remove("Matches");
						foreach (var item in Items) {
							item.IsActive = true;
						}
					}
					else
					{
						G.DialogManager.PostMessage("I need matches to light this...", TimeSpan.Zero, new TimeSpan(0, 0, 3));
					}
				});

			BoulderSpot = new Hotspot (
				"Pry Boulder up",
				new Polygon (
					new Vector2 (485, 388),
					new Vector2 (555, 314),
					new Vector2 (626, 315),
					new Vector2 (676, 384),
					new Vector2 (673, 431),
					new Vector2 (589, 474),
					new Vector2 (484, 441)),
				(t, i) =>
				{
					if (i != null && i.Name == "Bloody Broken Branch")
					{
						BoulderSpot.IsUsable = false;
						Boulder.IsVisible = false;
						Machete.IsActive = true;
						Machete.IsMouseHover = true;
						Machete.CanPickup = true;

						G.DialogManager.PostMessage ("You pried up the boulder with the branch", TimeSpan.Zero, new TimeSpan (0, 0, 3));
						G.InventoryManager.CurrentItems.Remove ("Bloody Broken Branch");
						foreach (var item in Items)
						{
							item.IsActive = true;
						}
					}
					else
					{
						G.DialogManager.PostMessage ("I need something strong to pry this up", TimeSpan.Zero, new TimeSpan (0, 0, 3));
					}
				}) { WalkLocation = new Vector2(606, 470) };
			#endregion

			Backpack.OnClick += state =>
			{
				Batteries.IsActive = true;
				Batteries.CanPickup = true;
				Batteries.IsVisible = true;
				Batteries.IsMouseHover = true;
				Papers.IsActive = true;
				Papers.CanPickup = true;
				Papers.IsVisible = true;
				Papers.IsMouseHover = true;
			};

			GameItem.AddCraftingRecipie("Flashlight", "Batteries", () =>
			{
				GameItem.ItemDictionary["Flashlight"].Description = "A flashlight, it's batteries are fully charged";
				GameItem.ItemDictionary["Flashlight"].InventoryIcon = G.C.Load<Texture2D>("UI/Icons/flashlight_on");

				G.InventoryManager.CurrentItems.Remove("Batteries");

				// Since we're doing this manually, we have to update it manually
				ItemsToLeave.Remove ("Flashlight_lit");
				CanLeaveLevel = (ItemsToLeave.Count == 0);
			});

			ItemsToLeave.Add("Flashlight_lit");

			Items.AddRange(new[]
			{
				Batteries,
				EmptyWineBottle,
				BrokenWineBottle,
				Papers, 
				Backpack,
				Machete,
				Boulder
			});

			Lights.AddRange(new[]
			{
				FirepitLight,
				TentLight
			});

			Hotspots.AddRange(new[]
			{
				Firepit,
				TentEntrance,
				CampExit,
				BoulderSpot
			});

			return;
			//REMOVE
			TentLight.IsVisible = false;
			FirepitAnimation.IsVisible = true;
			G.DialogManager.PostMessage ("You have used the matches", TimeSpan.Zero, new TimeSpan (0, 0, 3));
			G.InventoryManager.CurrentItems.Remove ("Matches");
			foreach (var item in Items)
			{
				item.IsActive = true;
			}
		}

		public GameItem Backpack;
		public GameItem Batteries;
		public GameItem EmptyWineBottle;
		public GameItem BrokenWineBottle;
		public GameItem Papers;
		public GameItem Boulder;
		public GameItem Machete;
		public Hotspot TentEntrance;
		public Hotspot CampExit;
		public Hotspot Firepit;
		public Hotspot BoulderSpot;
		public Sprite FirepitLight;
		public Sprite TentLight;

		public AnimatedSprite FirepitAnimation;
		public AnimatedSprite FliesAnimation;

		protected override void DrawBottomLayer (SpriteBatch batch)
		{
			FliesAnimation.Draw (batch);
			FirepitAnimation.Draw (batch);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update (gameTime);
			FirepitAnimation.Update (gameTime);
			FliesAnimation.Update (gameTime);
		}

		protected override void OnLevelStart (string lastScreen)
		{
			switch (lastScreen)
			{
				case null:
				case "None":
				case "Tent":
					Player.Location = new Vector2 (188, 283);
					MediaPlayer.Play (G.C.Load<Song> ("sfx/Chirping"));
					MediaPlayer.Volume = 0.25f;
					MediaPlayer.IsRepeating = true;
					break;
				case "Forest":
					Player.Location = new Vector2 (1177, 359);
					break;
				default:
					throw new Exception ("The person has come from an invalid state");
			}
		}
	}
}
