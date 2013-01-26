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
			Texture = G.C.Load<Texture2D> ("Player");
			Origin = new Vector2 (38, 220);
		}

		public Queue<Vector2> MoveQueue = new Queue<Vector2>();

		public void ClearMove()
		{
			MoveQueue.Clear();
			hasTarget = false;
		}

		public void Update (GameTime gameTime)
		{
			if (MoveQueue.Count == 0)
				return;

			// Grab the next target in our queue
			if (MoveQueue.Count > 0 && !hasTarget)
			{
				start = new Vector2 (Location.X, Location.Y);
				movePassed = 0;
				moveTime = Vector2.Distance (MoveQueue.Peek(), Location)/SPEED;
				hasTarget = true;
			}

			movePassed += gameTime.ElapsedGameTime.Milliseconds;
			if (MathHelper.Clamp (movePassed, 0, moveTime) >= moveTime)
			{
				Location = Vector2.Lerp (start, MoveQueue.Peek(), 1f);
				MoveQueue.Dequeue();
				hasTarget = false;
				return;
			}

			Location = Vector2.Lerp (start, MoveQueue.Peek(), movePassed/moveTime);
			base.Update (gameTime);
		}

		private const float SPEED = 1; // pixel/ms
		private bool hasTarget;
		private float moveTime;
		private float movePassed;
		private Vector2 start;
	}
}
