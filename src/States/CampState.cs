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

			var p1 = new Polygon(
				new Vector2(397, 280),
				new Vector2(159, 275),
				new Vector2(154, 292),
				new Vector2(186, 298),
				new Vector2(204, 334),
				new Vector2(129, 352),
				new Vector2(127, 384),
				new Vector2(212, 429),
				new Vector2(196, 454),
				new Vector2(245, 473),
				new Vector2(296, 502),
				new Vector2(121, 539),
				new Vector2(118, 646),
				new Vector2(163, 672),
				new Vector2(213, 636),
				new Vector2(269, 639),
				new Vector2(441, 648),
				new Vector2(528, 675),
				new Vector2(610, 680),
				new Vector2(726, 674),
				new Vector2(819, 655),
				new Vector2(929, 664),
				new Vector2(1081, 634),
				new Vector2(1144, 600),
				new Vector2(1159, 543),
				new Vector2(1372, 477),
				new Vector2(1374, 345),
				new Vector2(1291, 352),
				new Vector2(1164, 441),
				new Vector2(1079, 478),
				new Vector2(897, 445),
				new Vector2(838, 501),
				new Vector2(639, 500),
				new Vector2(482, 382),
				new Vector2(399, 283));



			var p2 = new Polygon();
			foreach (var vector in p1.Vertices)
			{
				p2.Vertices.Add(new Vector2(0, 400)+vector);
			}

			var p1n = new PolyNode(p1);
			var p2n = new PolyNode(p2);

			PolyLink.AttachLinks(3, 0, ref p1n, ref p2n);

			Nav = new List<PolyNode> {
				p1n,
				p2n
			};
			#endregion

			Backpack = CreateItem("Backpack", "A torn backpack", "CampArea/backpack", 0, 0, new Vector2(0, 0));

			Batteries = CreateItem("Batteries", "Your tongue hurts - they are supprisingly strong", "CampArea/batteries", "UI/Icons/batteries", 0, 0,
			                       new Vector2(0, 0));

			EmptyWineBottle = CreateItem("Empty Wine Bottle", "It's empty", "CampArea/wine1", 0, 0, new Vector2(0, 0));

			BrokenWineBottle = CreateItem("Broken Wine Bottle", "The neck of the bottle has been broken, likely due to a fall",
			                              "CampArea/win2", 0, 0, new Vector2(0, 0));

			Papers = CreateItem("Shrededd paper", "[TODO]", "CampArea/papers", "UI/Icons/papers", 0, 0, new Vector2(0, 0));


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
				new Polygon(),
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
				"Fire Pit",
				new	Polygon(),
				t =>
				{
					var that = this;
					foreach (var item in that.Items)
					{
						item.IsActive = true;
						//FirepitLight.IsVisible = true;
					}
				});


			Items.AddRange(new[]
			{
				Batteries,
				EmptyWineBottle,
				BrokenWineBottle,
				Papers
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

	}
}
