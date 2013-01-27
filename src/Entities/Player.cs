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
		: AnimatedSprite
	{
		public Player()
			: base(G.C.Load<Texture2D>("character"),
			       new[]
			       {
				       new Animation("Idle",
							new []
							{
								new Rectangle(0, 0, 250, 500)
							}),
					   
					   new Animation("Walk",
							new[]
							{
								new Rectangle(0, 0, 250, 500),
								new Rectangle(250, 0, 250, 500),
								new Rectangle(500, 0, 250, 500),
								new Rectangle(750, 0, 250, 500),
								new Rectangle(1000, 0, 250, 500),
								new Rectangle(1250, 0, 250, 500),
								new Rectangle(1500, 0, 250, 500),
								new Rectangle(1750, 0, 250, 500),
							})
			       })
		{
			Origin = new Vector2(250, 500);

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
			{
				AnimationManager.SetAnimation("Idle");
				return;				
			}
			
			if (AnimationManager.CurrentAnimation.Name != "Walk")
			{
				AnimationManager.SetAnimation("Walk");
			}

			// Grab the next target in our queue
			if (MoveQueue.Count > 0 && !hasTarget)
			{
				start = new Vector2 (Location.X, Location.Y);
				movePassed = 0;
				moveTime = Vector2.Distance (MoveQueue.Peek(), Location)/SPEED;
				hasTarget = true;
			}

			movePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
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

		private const float SPEED = 120; // pixel/sec
		private bool hasTarget;
		private float moveTime;
		private float movePassed;
		private Vector2 start;
	}
}
