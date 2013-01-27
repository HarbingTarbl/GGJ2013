﻿using System;
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
			: base("Camp", "Forest", "Tent")
		{
			Background = G.C.Load<Texture2D>("CampArea/background");
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

			Backpack = CreateItem("Backpack", "A torn backpack", "CampArea/backpack", 734, 410,
			                      new Rectagon(0, 0, 111, 59).Vertices.ToArray());

			Batteries = CreateItem("Batteries", "Your tongue hurts - they are supprisingly strong", "CampArea/batteries", "UI/Icons/batteries", 761, 480,
								new Rectagon(0, 0, 78, 32).Vertices.ToArray());

			EmptyWineBottle = CreateItem("Empty Wine Bottle", "It's empty", "CampArea/wine1", 137, 574,
				new Rectagon(0, 0, 24, 81).Vertices.ToArray());

			BrokenWineBottle = CreateItem("Broken Wine Bottle", "The neck of the bottle has been broken, likely due to a fall",
										  "CampArea/win2", 422, 615, new Rectagon(0, 0, 53, 35).Vertices.ToArray());

			Papers = CreateItem("Shrededd paper", "[TODO]", "CampArea/papers", "UI/Icons/papers", 822, 457, new Rectagon(0, 0, 53, 35).Vertices.ToArray());
			
			Boulder = CreateItem ("Boulder", "A heavy rock", "CampArea/boulder", "UI/Icons/papers", 480, 313);

			Machete = CreateItem ("Machete", "A knife used for cutting things down", "CampArea/machete", 560, 420, new Rectagon (0, 0, 53, 35).Vertices.ToArray());
				

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

			Boulder.IsActive = false;
			Boulder.CanPickup = false;
			Boulder.MouseHover = false;

			Machete.IsActive = false;
			Machete.CanPickup = false;
			Machete.MouseHover = false;

			Backpack.IsActive = false;
			Backpack.CanPickup = false;

			Batteries.IsActive = false;
			Batteries.CanPickup = true;

			EmptyWineBottle.IsActive = false;
			EmptyWineBottle.CanPickup = true;

			BrokenWineBottle.IsActive = false;
			BrokenWineBottle.CanPickup = true;
			BrokenWineBottle.IsVisible= false;

			Papers.IsActive = false;
			Papers.CanPickup = true;

			FirepitLight = CreateSprite("CampArea/light map 1", 0, 0);
			FirepitLight.IsVisible = false;
			TentLight = CreateSprite("CampArea/light map 1", 0, 0);
#region stuff
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
				    if (!leaveOverride)
				    {
					    if (i != null && i.Description == "A flashlight, it's batteries are fully charged") {
						    CanLeaveLevel = true;
						    leaveOverride = true;
					    }
					    else
						    CanLeaveLevel = false; // HACK
				    }

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
					    G.DialogManager.PostQueuedMessage("It's too dark, maybe if I had a flashlight or something.");
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
					if (i != null && i.Name == "Matches")
					{
						TentLight.IsVisible = false;
						FirepitAnimation.IsVisible = true;
						fireLit = true;
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
				new Polygon (new Vector2 (57, 497),
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
						Boulder.IsVisible = false;
						Machete.IsActive = true;
						Machete.MouseHover = true;
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
						G.DialogManager.PostMessage ("I need something long to pry this yI need matches to light this...", TimeSpan.Zero, new TimeSpan (0, 0, 3));
					}
				});
#endregion

			GameItem.AddCraftingRecipie("Flashlight", "Batteries", () =>
			{
				GameItem.ItemDictionary["Flashlight"].Description = "A flashlight, it's batteries are fully charged";
				GameItem.ItemDictionary["Flashlight"].InventoryIcon = G.C.Load<Texture2D>("UI/Icons/flashlight_on");
				G.InventoryManager.CurrentItems.Remove("Batteries");
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
				CreateSprite ("CampArea/foreground"),
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
			fireLit = true;
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
		public Sprite Foreground;
		private bool leaveOverride;

		public AnimatedSprite FirepitAnimation;
		private bool fireLit = false;

		public override void Draw(SpriteBatch batch)
		{
			base.Draw (batch);

			BeginDraw (batch, BlendState.NonPremultiplied);
			FirepitAnimation.Draw (batch);
			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update (gameTime);
			FirepitAnimation.Update (gameTime);
		}

		protected override void OnLevelStart (string LastScreen)
		{
			switch (LastScreen)
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
