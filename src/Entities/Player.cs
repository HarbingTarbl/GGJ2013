using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Collision;
using Jammy.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Entities
{
	public class Player
		: Sprite
	{
		public Player()
		{
			Texture = G.C.Load<Texture2D>("Player");
		}

		public Vector2 Destination;
		public Vector2 Direction;
		public float Speed = 1; // px/ms

		public void Update(GameTime gameTime, Polygon nav)
		{
			return;
				var i = 1;
				var oldDist = Vector2.Distance(Location, nav.Vertices[0] + nav.Location);
				var index = 0;
				for (; i < nav.Vertices.Count; i++)
				{
					var dist = Vector2.Distance(Location, nav.Vertices[i] + nav.Location);
					if (dist < oldDist)
					{
						oldDist = dist;
						index = i;
					}
				}
				i = index;
				if (Destination.X - Location.X > 0
				    && i < nav.Vertices.Count - 1)
				{
					Destination = nav.Vertices[i + 1];
					Direction = nav.Vertices[i + 1] - Location;
					Direction.Normalize();
				}
				else if (i > 0
				         && i < nav.Vertices.Count)
				{
					Destination = nav.Vertices[i - 1];
					Direction = Location -  nav.Vertices[i - 1];
					Direction.Normalize();
				}
				else
				{
					Destination = Location;
					Direction = Vector2.Zero;
				}

				if(Vector2.Distance(Destination, Location) > 100)
					Location += Direction*Speed*gameTime.ElapsedGameTime.Milliseconds;
		

			Update(gameTime);
		}
	}
}
