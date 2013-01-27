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

namespace GGJ2013.States
{
	public class CampState
		: MemoryState
	{
		public CampState()
			: base("Camp", "Woods", "Tent")
		{
			Background = G.C.Load<Texture2D>("CampArea/background");
			#region NavMesh

			var p1 = new Polygon (
				new Vector2 (166, 274),
				new Vector2 (392, 288),
				new Vector2 (521, 490),
				new Vector2 (396, 489),
				new Vector2 (230, 477),
				new Vector2 (155, 390),
				new Vector2 (152, 278));

			var p2 = new Polygon (
				new Vector2 (307, 493),
				new Vector2 (53, 553),
				new Vector2 (214, 655),
				new Vector2 (467, 660),
				new Vector2 (554, 574),
				new Vector2 (640, 500),
				new Vector2 (396, 489));

			var p3 = new Polygon (
				new Vector2 (640, 500),
				new Vector2 (554, 574),
				new Vector2 (467, 660),
				new Vector2 (1069, 659),
				new Vector2 (1070, 657),
				new Vector2 (963, 535),
				new Vector2 (880, 452));

			var p4 = new Polygon (
				new Vector2 (963, 535),
				new Vector2 (880, 452),
				new Vector2 (1122, 470),
				new Vector2 (1192, 480),
				new Vector2 (1271, 494),
				new Vector2 (1070, 657));

			var p5 = new Polygon (
				new Vector2 (1122, 470),
				new Vector2 (1148, 418),
				new Vector2 (1209, 421),
				new Vector2 (1274, 423),
				new Vector2 (1271, 494),
				new Vector2 (1192, 480));

			var p6 = new Polygon(
				new Vector2 (1148, 418),
				new Vector2 (1066, 385),
				new Vector2 (1188, 250),
				new Vector2 (1275, 254),
				new Vector2 (1274, 423),
				new Vector2 (1209, 421));
			
			var p1n = new PolyNode (p1);
			var p2n = new PolyNode (p2);
			var p3n = new PolyNode (p3);
			var p4n = new PolyNode (p4);
			var p5n = new PolyNode (p5);
			var p6n = new PolyNode (p6);

			PolyLink.AttachLinks (400, 487, ref p1n, ref p2n);
			PolyLink.AttachLinks (546, 580, ref p2n, ref p3n);
			PolyLink.AttachLinks (972, 544, ref p3n, ref p4n);
			PolyLink.AttachLinks (1191, 478, ref p4n, ref p5n);
			PolyLink.AttachLinks (1209, 420, ref p5n, ref p6n);

			Nav = new List<PolyNode> {
				p1n,
				p2n,
				p3n,
				p4n,
				p5n,
				p6n
			};
			#endregion

			Backpack = CreateItem("Backpack", "A torn backpack", "CampArea/backpack", 900, 500,
			                      new Rectagon(0, 0, 111, 59).Vertices.ToArray());

			Batteries = CreateItem("Batteries", "Your tongue hurts - they are supprisingly strong", "CampArea/batteries", "UI/Icons/batteries", 761, 549,
								new Rectagon(0, 0, 78, 32).Vertices.ToArray());

			EmptyWineBottle = CreateItem("Empty Wine Bottle", "It's empty", "CampArea/wine1", 137, 574,
				new Rectagon(0, 0, 24, 81).Vertices.ToArray());

			BrokenWineBottle = CreateItem("Broken Wine Bottle", "The neck of the bottle has been broken, likely due to a fall",
										  "CampArea/win2", 422, 615, new Rectagon(0, 0, 53, 35).Vertices.ToArray());

			Papers = CreateItem("Shrededd paper", "[TODO]", "CampArea/papers", "UI/Icons/papers", 1134, 456, new Rectagon(0, 0, 53, 35).Vertices.ToArray());


			Backpack.IsActive = false;
			Backpack.CanPickup = false;

			Batteries.IsActive = false;
			Batteries.CanPickup = true;

			EmptyWineBottle.IsActive = false;
			EmptyWineBottle.CanPickup = true;

			BrokenWineBottle.IsActive = false;
			BrokenWineBottle.CanPickup = true;

			Papers.IsActive = false;
			Papers.CanPickup = true;

			//FirepitLight = CreateSprite("CampArea/FirepitLightMap", 0, 0);
			//TentLight = CreateSprite("CampArea/TentFlapLightMap", 0, 0);

			TentEntrance = new Hotspot(
				"Tent Entrance",
				new Polygon(new Vector2(143, 266),
				            new Vector2(40, 90),
				            new Vector2(247, 252)),
				t =>
				{
					G.StateManager.Pop();
					G.StateManager.Push(LastLevel);
				});

			CampExit = new Hotspot("Camp Exit",
			                       new Polygon(),
			                       t =>
			                       {
				                       if (CanLeaveLevel)
				                       {
					                       G.StateManager.Pop();
					                       G.StateManager.Push(NextLevel);
										   
				                       }
				                       else
				                       {
					                       G.DialogManager.PostQueuedMessage("[Can't leave message]");
				                       }
			                       });

			Firepit = new Hotspot(
				"Light Fire Pit",
				new	Polygon(new Vector2(57, 497),
					new Vector2(176, 463),
					new Vector2(286, 480),
					new Vector2(190, 510)),
				t =>
				{
					if (G.InventoryManager.CurrentItems.Contains("Matches"))
					{
						G.DialogManager.PostMessage("You have used the matches", TimeSpan.Zero, new TimeSpan(0, 0, 3));
						G.InventoryManager.CurrentItems.Remove("Matches");
						var that = this;
						foreach (var item in that.Items)
						{
							item.IsActive = true;
						}
					}
					else
					{
						G.DialogManager.PostMessage("I need matches to light this...", TimeSpan.Zero, new TimeSpan(0, 0, 3));
					}
				});

			GameItem.AddCraftingRecipie("Flashlight", "Batteries", () =>
			{
				GameItem.ItemDictionary["Flashlight"].Description = "A flashlight, it's batteries are fully charged";
				G.InventoryManager.CurrentItems.Remove("Batteries");
			});

			ItemsToLeave.Add("Flashlight_lit");

			Items.AddRange(new[]
			{
				Batteries,
				EmptyWineBottle,
				BrokenWineBottle,
				Papers, 
				Backpack
			});

			/*Lights.AddRange(new[]
			{
				FirepitLight,
				TentLight
			});*/

			Hotspots.AddRange(new[]
			{
				Firepit,
				TentEntrance,
				CampExit
			});
		}



		private bool fireLit = false;



		//Items
		public GameItem Backpack;
		public GameItem Batteries;
		public GameItem EmptyWineBottle;
		public GameItem BrokenWineBottle;
		public GameItem Papers;

		//State Changers
		public Hotspot TentEntrance;
		public Hotspot CampExit;

		//Events
		public Hotspot Firepit;

		//Lights
		public Sprite FirepitLight;
		public Sprite TentLight;

		protected override void OnLevelStart (string LastScreen)
		{
			if (LastScreen == "Tent")
				Player.Location = new Vector2 (188, 283);
			base.OnLevelStart(LastScreen);
		}
	}
}
