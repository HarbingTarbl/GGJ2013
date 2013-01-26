﻿using System;
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
			tent.Texture = C.Load<Texture2D> ("TentArea/background");
			//tent.InsideMesh = new Circlegon(tent.Texture.Width/2f, tent.Texture.Height/2f, 300f);
			//tent.NavMesh = new Circlegon(tent.Texture.Width/2f, tent.Texture.Height/2f, 100f);

			var half = new Vector2(tent.Texture.Width/2f, tent.Texture.Height/2f);

			tent.InsideMesh = new Polygon(
				half + new Vector2(100, 100),
				half + new Vector2(100, -100),
				half + new Vector2(-100, -100),
				half + new Vector2(-100, 100),
				half + new Vector2(-200, 100),
				half + new Vector2(-200, -200),
				half + new Vector2(200, -200),
				half + new Vector2(200, 200));

			tent.NavMesh = new Polygon(
				half + new Vector2(150, 150),
				half + new Vector2(150, -150),
				half + new Vector2(-150, -150),
				half + new Vector2(-150, 150));

			var lantern = new ReminderItem ("Lantern", Content.Load<Texture2D>("TentArea/lantern"));
			lantern.CollisionData = new Circlegon(30);
			lantern.Location = new Vector2(520, 0);

			var blanket = new ReminderItem ("Blanket", Content.Load<Texture2D> ("TentArea/blanket"));
			blanket.CollisionData = new Circlegon (30);
			blanket.Location = new Vector2 (104, 503);

			var light = new ReminderItem ("Flashlight", Content.Load<Texture2D> ("TentArea/flashlight"));
			light.CollisionData = new Circlegon (30);
			light.Location = new Vector2 (587, 436);

			var bag = new ReminderItem ("Bag", Content.Load<Texture2D> ("TentArea/dufflebag"));
			bag.CollisionData = new Circlegon (30);
			bag.Location = new Vector2 (723, 444);

			var sweater = new ReminderItem ("Sweater", Content.Load<Texture2D> ("TentArea/sweater"));
			sweater.CollisionData = new Circlegon (30);
			sweater.Location = new Vector2 (380, 555);

			tent.Items.Add (lantern);
			tent.Items.Add (light);
			tent.Items.Add (bag);
			tent.Items.Add (sweater);
			tent.Items.Add (blanket);

			StateManager.Add (tent);
		}
	}
}
