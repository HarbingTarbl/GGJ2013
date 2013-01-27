using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Items;
using GGJ2013.States;
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
								new Rectangle(0, 0, 250, 500),
								new Rectangle(250, 0, 250, 500),
								new Rectangle(500, 0, 250, 500),
								new Rectangle(750, 0, 250, 500)
							}, Looping:false),
					   
					   new Animation("Walk",
							new[]
							{
								new Rectangle(0, 500, 250, 500),
								new Rectangle(250, 500, 250, 500),
								new Rectangle(500, 500, 250, 500),
								new Rectangle(750, 500, 250, 500),
								new Rectangle(1000, 500, 250, 500),
								new Rectangle(1250, 500, 250, 500),
								new Rectangle(1500, 500, 250, 500),
								new Rectangle(1750, 500, 250, 500),
							}),
						new Animation("Pick Up",
							new []
							{
								new Rectangle(0, 1000, 250, 500),
								new Rectangle(250, 1000, 250, 500),
								new Rectangle(500, 1000, 250, 500),
								new Rectangle(500, 1000, 250, 500),
								new Rectangle(250, 1000, 250, 500),
								new Rectangle(0, 1000, 250, 500),
							}, Looping:false) { NextAnim = "Idle" }
			       })
		{
			_IHateRectangles.Width = 125;
			_IHateRectangles.Height = 250;



			Origin = new Vector2(125, 500);
			AnimationManager.SetAnimation("Idle");
			CollisionData = new Rectagon(0, 0, 100, 250);

		}

		public Queue<Vector2> MoveQueue = new Queue<Vector2>();

		public void ClearMove()
		{
			MoveQueue.Clear();
			hasTarget = false;
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, _IHateRectangles, AnimationManager.Bounding, Color.White, Rotation, Origin,
				MoveQueue.Count > 0 ?  MoveQueue.Peek().X - _IHateRectangles.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
		}

		public override void Update (GameTime gameTime)
		{
			AnimationManager.Update(gameTime);
			_IHateRectangles.X = (int)Location.X;
			_IHateRectangles.Y = (int)Location.Y;
			CollisionData.Location = Location - Origin - new Vector2(-75, -250);

			if (MoveQueue.Count == 0)
			{
				if (AnimationManager.CurrentAnimation.Name == "Walk")
				{
					AnimationManager.SetAnimation("Idle");
					if (Target != null)
					{
						var state = (MemoryState) G.StateManager.CurrentState;
						if (TargerIsItem)
						{
							state.OnItemFound((GameItem) Target);
							Trace.WriteLine(((GameItem) Target).Location);
						}
						else
							((Hotspot) Target).OnActivate(state);
					}
				}

				return;				
			}


			if (AnimationManager.CurrentAnimation.Name == "Idle")
			{
				AnimationManager.SetAnimation("Walk");
			}

			if (AnimationManager.CurrentAnimation.Name == "Walk")
			{

				// Grab the next target in our queue
				if (MoveQueue.Count > 0
				    && !hasTarget)
				{
					start = new Vector2(Location.X, Location.Y);
					movePassed = 0;
					moveTime = Vector2.Distance(MoveQueue.Peek(), Location)/SPEED;
					hasTarget = true;
				}

				movePassed += (float) gameTime.ElapsedGameTime.TotalSeconds;
				if (MathHelper.Clamp(movePassed, 0, moveTime) >= moveTime)
				{
					Location = Vector2.Lerp(start, MoveQueue.Peek(), 1f);
					MoveQueue.Dequeue();
					hasTarget = false;
					return;
				}

				Location = Vector2.Lerp(start, MoveQueue.Peek(), movePassed/moveTime);
			}
		}

		public object Target;
		public bool TargerIsItem;

		private const float SPEED = 120; // pixel/sec
		private bool hasTarget;
		private bool frozen = false;
		private float moveTime;
		private float movePassed;
		private Vector2 start;
	}
}
