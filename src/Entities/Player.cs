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
			Texture = G.ContentManager.Load<Texture2D>("player");
		}


		public Vector2 Destination;
		public Vector2 Direction;
		public float Speed; // px/ms

		public void Update(GameTime gameTime, Polygon nav)
		{
			var i = 1;
			var oldDist = Vector2.DistanceSquared(Location, nav.Vertices[0]);

			for (; i < nav.Vertices.Count; i++)
			{
				var dist = Vector2.DistanceSquared(Location, nav.Vertices[i]);
				if (dist < oldDist)
				{
					i--;
					break;
				}
			}

			if (Destination.X - Location.X > 0 && i < nav.Vertices.Count - 1)
			{
				Direction = nav.Vertices[i + 1] - nav.Vertices[i];
				Direction.Normalize();
			}
			else if(nav.Vertices.Count > 0)
			{
				Direction = nav.Vertices[i - 1] - nav.Vertices[i];
				Direction.Normalize();
			}
			else
			{
				Direction = Vector2.Zero;
			}

			Location += Direction*Speed*gameTime.ElapsedGameTime.Milliseconds;

			Update(gameTime);
		}
	}
}
