using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Items;
using GGJ2013.States;
using Jammy;
using Jammy.Collision;
using Jammy.Helpers;
using Jammy.Sprites;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GGJ2013
{
	public class TentState
		: MemoryState
	{
		public TentState()
			: base ("Tent", "Tent", "None")
		{
			Background = G.C.Load<Texture2D> ("TentArea/background");
			#region NavMesh
			var p1 = new Polygon (
				new Vector2 (106, 469),
				new Vector2 (997, 480),
				new Vector2 (896, 594),
				new Vector2 (627, 646),
				new Vector2 (345, 644),
				new Vector2 (159, 605));

			Nav = new List<PolyNode>();
			Nav.Add (new PolyNode (p1));
			#endregion

			Blanket = CreateItem ("Blanket", "TentArea/blanket", 30, 300, 503);
			Flash = CreateItem ("Flashlight", "TentArea/flashlight", 30, 587, 436);
			Bag = CreateItem ("Bag", "TentArea/dufflebag", 30, 723, 444);
			Sweater = CreateItem("Sweater", "TentArea/sweater", 30, 500, 550);

			Blanket.OnClick += (t) =>
			{
				var that = this;
				that.BlanketClicked = true;
				that.Blanket.IsActive = false;
			};

			Blanket.IsActive = true;
			Blanket.CanPickup = false;

			Sweater.CanPickup = true;

			Flash.IsActive = true;
			Flash.CanPickup = true;

			Exit = new Hotspot(
				new Polygon(
					new Vector2(665, 275),
					new Vector2(657, 373),
					new Vector2(625, 459),
					new Vector2(696, 468),
					new Vector2(726, 374),
					new Vector2(707, 299)), t =>
					{
						if (CanLeaveLevel)
						{
							G.StateManager.Pop();
							G.StateManager.Push(NextLevel);
						}
						else
						{
							Dialog.PostMessage("I should put on some clothes... ", TimeSpan.Zero, new TimeSpan(0, 0, 5));
							Dialog.PostMessage("and grab my flashlight.", new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 5));
						}
					});

			ItemsToLeave.Add("Sweater");
			ItemsToLeave.Add("Flashlight");

			lantern = CreateSprite ("TentArea/lantern", 520, 0);
			light1 = CreateSprite ("TentArea/light1");
			light2 = CreateSprite ("TentArea/light2");

			

			light1.IsVisible = false;
			light2.IsVisible = false;

			Items.Add (Flash);
			Items.Add (Bag);
			Items.Add (Sweater);
			Items.Add (Blanket);

			Hotspots.Add(Exit);

		}

		protected override void OnLevelComplete()
		{
			Dialog.PostMessage("YAY YOU WIN", TimeSpan.Zero, TimeSpan.FromSeconds(5), Color.Red);

		}

		public override void Draw(SpriteBatch batch)
		{
 			base.Draw(batch);

			BeginDraw (batch, BlendState.AlphaBlend);
			lantern.Draw (batch);
			light1.Draw (batch);
			light2.Draw (batch);
			batch.End();

		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (BlanketClicked)
			{
				CurrentBlanketMoveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				Blanket.Location = Vector2.SmoothStep(Blanket.Location, BlanketDestination, CurrentBlanketMoveTime / BlanketMoveTime);
				if (CurrentBlanketMoveTime >= BlanketMoveTime - 3)
				{
					BlanketClicked = false;
					Sweater.IsActive = true;
				}
			}

		}

		public override bool HandleInput(GameTime gameTime)
		{
			

			if (Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				Dialog.PostQueuedMessage("Spaaaaaace", new TimeSpan(0, 0, 5));
			}

			return base.HandleInput(gameTime);
		}

		private Sprite lantern;
		private Sprite light1;
		private Sprite light2;

		public GameItem Sweater;
		public GameItem Blanket;
		public GameItem Bag;
		public GameItem Flash;

		public Hotspot Exit; 

		public bool BlanketClicked;
		public Vector2 BlanketDestination = new Vector2(103, 450);


		public float BlanketMoveTime = 6; //Move time in seconds ish
		public float CurrentBlanketMoveTime;


		protected override void OnLevelStart (string LastScreen)
		{
			if (String.IsNullOrEmpty (LastScreen))
				RunIntroCinematics();
		}

		private void RunIntroCinematics()
		{
			G.Player.Location = new Vector2 (430, 495);
			//TODO: Put player in laying down animation
			//TODO: fade in?
			//TODO: Wait for player to play stand up animation
			//TODO: play any dialog we have
		}
	}
}
