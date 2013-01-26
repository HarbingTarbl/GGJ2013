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
			Texture = G.C.Load<Texture2D>("player");
		}

		public Vector2 Destination;
		public Vector2 Direction;
		public float Speed = 1; // px/ms

		public void Update(GameTime gameTime, Polygon nav)
		{
			if (Vector2.Distance(Location, Destination) < 25)
				Direction = Vector2.Zero;
			Location += Direction*Speed*gameTime.ElapsedGameTime.Milliseconds;

			Update(gameTime);
		}
	}
}
