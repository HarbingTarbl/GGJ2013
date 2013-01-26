using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
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
	public class HitlerGameState
		: BaseGameState
	{
		public HitlerGameState(string name, string nextState)
			: base(name)
		{
			Items = new List<ReminderItem>();
			Hotspots = new List<ActivePolygon>();
			ItemsToLeave = new List<string>();
			ItemsToRemember = new List<string>();
			Camera = new CameraSingle(LiterallyHitler.Instance.GraphicsDevice.Viewport.Width,
			                          LiterallyHitler.Instance.GraphicsDevice.Viewport.Height);
			NextState = nextState;
		}

		public override void OnFocus()
		{
			LiterallyHitler.Camera = Camera;
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

		public Texture2D Texture;
		public Size Size;

		public Sprite Player;
		public CameraSingle Camera; 

		public MemoryItem Reward;

		public bool IsLevelComplete;
		public bool CanLeaveLevel;

		public string NextState;

		public event Action<HitlerGameState, ReminderItem> ItemFound;
		public event Action<HitlerGameState> LevelComplete;

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, Vector2.Zero, Color.White);

			foreach (var item in Items)
			{
				item.Draw(batch);
			}
		}

		public override void Update(GameTime gameTime)
		{
			Player.Update(gameTime);
			Camera.CenterOnPoint(Player.Location);
		}

		public void NextLevel()
		{
			LiterallyHitler.StateManager.Pop();
			LiterallyHitler.StateManager.Push(NextState);
		}

		public override bool HandleInput(GameTime gameTime)
		{
			var cMouse = Mouse.GetState();

			if (cMouse.LeftButton.WasButtonPressed(_oldMouse.LeftButton))
			{
				foreach (var item in Items)
				{
					if (!CollisionChecker.PointToPoly(new Vector2(cMouse.X, cMouse.Y), (Polygon) item.CollisionData)) continue;
					if (item.IsFound) continue;

					OnItemFound(item);
					break;
				}

				foreach (var spot in Hotspots)
				{
					if (!CollisionChecker.PointToPoly(new Vector2(cMouse.X, cMouse.Y), spot)) continue;
					spot.OnActivate();
					break;
				}
			}

			var keystate = Keyboard.GetState();
			Camera.Location += new Vector2((keystate.IsKeyDown(Keys.D) ? 1 : 0) - (keystate.IsKeyDown(Keys.A) ? 1 : 0),
			                               (keystate.IsKeyDown(Keys.S) ? 1 : 0 - (keystate.IsKeyDown(Keys.W) ? 1 : 0)));


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
		private int _foundItems;
	}
}
