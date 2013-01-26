using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Collision;
using Jammy.Sprites;
using Microsoft.Xna.Framework;

namespace GGJ2013.Entities
{
	public class Player
		: Sprite
	{
		public Vector2 Destination;
		public float Speed; // px/ms

		public void Update(GameTime gameTime, Polygon nav)
		{
			//(float)gameTime.ElapsedGameTime.Milliseconds * Speed;

			

			
			//var current = nav.

			//Update(gameTime);
		}
	}
}
