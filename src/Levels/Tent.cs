using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Items;
using GGJ2013.States;
using Jammy.Collision;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013
{
	public partial class G
	{
		public void LoadAreaTent()
		{
			var tent = new BaseMemoryState ("Tent", "", "");
			tent.Texture = C.Load<Texture2D> ("Area_Tent");
			tent.NavMesh = new Polygon(
				new Vector2(-27.0f, 33.0f), new Vector2(-41.0f, 31.0f), new Vector2(-66.0f, 30.0f), new Vector2(-66.0f, 24.0f),
				new Vector2(-63.0f, 21.0f), new Vector2(-61.0f, 17.0f), new Vector2(-61.0f, 11.0f), new Vector2(-62.0f, 6.0f),
				new Vector2(-62.0f, 0.0f), new Vector2(-61.0f, -6.0f), new Vector2(-58.0f, -9.0f), new Vector2(-54.0f, -11.0f),
				new Vector2(-50.0f, -14.0f), new Vector2(-41.0f, -16.0f), new Vector2(-35.0f, -18.0f), new Vector2(-33.0f, -22.0f),
				new Vector2(-28.0f, -26.0f), new Vector2(-17.0f, -28.0f), new Vector2(-7.0f, -30.0f), new Vector2(0.0f, -32.0f),
				new Vector2(16.0f, -32.0f), new Vector2(21.0f, -30.0f), new Vector2(30.0f, -29.0f), new Vector2(30.0f, -23.0f),
				new Vector2(88.0f, -21.0f), new Vector2(90.0f, -17.0f), new Vector2(90.0f, -11.0f), new Vector2(90.0f, -5.0f),
				new Vector2(89.0f, 0.0f), new Vector2(86.0f, 4.0f), new Vector2(82.0f, 7.0f), new Vector2(77.0f, 9.0f),
				new Vector2(71.0f, 10.0f), new Vector2(16.0f, 12.0f), new Vector2(14.0f, 16.0f), new Vector2(12.0f, 20.0f),
				new Vector2(5.0f, 25.0f), new Vector2(19.0f, 28.0f));
			tent.NavMesh.Location = new Vector2(150, 50);


			var item = new ReminderItem(
				"Sweater",
				C.Load<Texture2D>("sweater"))
			{
				CollisionData = new Polygon(
					new Vector2(39.0f, 65.0f),new Vector2(25.0f, 63.0f),new Vector2(0.0f, 62.0f),new Vector2(3.0f, 54.0f),new Vector2(5.0f, 46.0f),new Vector2(4.0f, 37.0f),new Vector2(4.0f, 27.0f),new Vector2(12.0f, 21.0f),new Vector2(20.0f, 17.0f),new Vector2(31.0f, 14.0f),new Vector2(35.0f, 7.0f),new Vector2(49.0f, 4.0f),new Vector2(59.0f, 2.0f),new Vector2(82.0f, 0.0f),new Vector2(96.0f, 3.0f),new Vector2(154.0f, 11.0f),new Vector2(156.0f, 19.0f),new Vector2(156.0f, 29.0f),new Vector2(152.0f, 36.0f),new Vector2(145.0f, 40.0f),new Vector2(137.0f, 42.0f),new Vector2(82.0f, 44.0f),new Vector2(78.0f, 51.0f),new Vector2(71.0f, 57.0f),new Vector2(85.0f, 60.0f))
			};


			tent.Items.Add(item);

			StateManager.Add (tent);
		}
	}
}
