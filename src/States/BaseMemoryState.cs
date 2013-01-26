using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Entities;
using GGJ2013.Items;
using Jammy;
using Jammy.Collision;
using Jammy.Parallax;
using Jammy.Helpers;
using Jammy.Sprites;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GGJ2013.States
{
	public class BaseMemoryState
		: BaseGameState
	{
		public BaseMemoryState(string name, string nextState, string lastState)
			: base(name)
		{
			Items = new List<ReminderItem>();
			Hotspots = new List<ActivePolygon>();
			Player = new Player();
			ItemsToLeave = new List<string>();
			ItemsToRemember = new List<string>();
			Camera = new CameraSingle(G.Instance.GraphicsDevice.Viewport.Width,
			                          G.Instance.GraphicsDevice.Viewport.Height);
			
			NextState = nextState;
		}

		public override void OnFocus()
		{
			G.Camera = Camera;
			base.OnFocus();
		}

		public override void Load()
		{
			base.Load();
		}

		public override void PostLoad()
		{
			base.PostLoad();
		}

		public List<ReminderItem> Items;
		public List<ActivePolygon> Hotspots;

		public List<string> ItemsToLeave;
		public List<string> ItemsToRemember;

		public Polygon NavMesh;

		public Texture2D Texture;
		public Size Size;

		public Player Player;
		public CameraSingle Camera; 

		public MemoryItem Reward;

		public bool IsLevelComplete;
		public bool CanLeaveLevel;

		public string LastState;
		public string NextState;

		public event Action<BaseMemoryState, ReminderItem> ItemFound;
		public event Action<BaseMemoryState> LevelComplete;

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, Vector2.Zero, Color.White);

			foreach (var item in Items)
			{
				item.Draw(batch);
			}

			Player.Draw(batch);
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var item in Items)
			{
				item.Update(gameTime);
			}



			//FIX: FIX ME
			Player.Update(gameTime, NavMesh);
			Camera.CenterOnPoint(Player.Location);
		}

		public void NextLevel()
		{
			G.StateManager.Pop();
			G.StateManager.Push(NextState);
		}

		public override bool HandleInput(GameTime gameTime)
		{
			var cMouse = Mouse.GetState();
			var target = new Vector2(cMouse.X, cMouse.Y);


			if (cMouse.LeftButton.WasButtonPressed(_oldMouse.LeftButton))
			{
				Player.Destination = target;


				foreach (var item in Items)
				{

					if (Vector2.DistanceSquared(Player.Location, item.CollisionData.AbsoluteCenter) > 128
					    && !CollisionChecker.PointToPoly(Camera.ScreenToWorld(target),
					                                     (Polygon) item.CollisionData)) continue;

					if (item.IsFound) continue;

					OnItemFound(item);
					break;
				}

				foreach (var spot in Hotspots)
				{
					if (Vector2.DistanceSquared(Player.Location, spot.AbsoluteCenter) > 128
						&& !CollisionChecker.PointToPoly(Camera.ScreenToWorld(target), spot)) continue;
					spot.OnActivate(this);
					break;
				}
			}

			var keystate = Keyboard.GetState();
			Camera.Location += new Vector2((keystate.IsKeyDown(Keys.D) ? 1 : 0) - (keystate.IsKeyDown(Keys.A) ? 1 : 0),
			                               (keystate.IsKeyDown(Keys.S) ? 1 : 0 - (keystate.IsKeyDown(Keys.W) ? 1 : 0)));

			if (keystate.IsKeyDown(Keys.F1)
			    && _oldKey.IsKeyUp(Keys.F1))
			{
				G.DebugCollision = !G.DebugCollision;
			}

			_oldKey = keystate;
			_oldMouse = cMouse;
			return base.HandleInput(gameTime);
		}

		protected void OnItemFound(ReminderItem item)
		{
			if (ItemsToLeave.Contains(item.Name))
			{
				ItemsToLeave.Remove(item.Name);
			}

			if (ItemsToRemember.Contains(item.Name))
			{
				ItemsToRemember.Remove(item.Name);
			}

			CanLeaveLevel = (ItemsToLeave.Count == 0);
			IsLevelComplete = (ItemsToRemember.Count == 0);

			item.Clicked(this);

			var handler = ItemFound;
			if (handler != null)
				handler(this, item);

			if (IsLevelComplete)
				OnLevelComplete();
		}

		protected void OnLevelComplete()
		{
			IsLevelComplete = true;
	
			var handler = LevelComplete;
			if (handler != null)
				handler(this);

		}


		private MouseState _oldMouse;
		private KeyboardState _oldKey;
	}
}
