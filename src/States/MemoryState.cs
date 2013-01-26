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
	public class MemoryState
		: BaseGameState
	{
		public MemoryState(string name)
			: base(name)
		{
			Player = G.Player;

			Items = new List<GameItem>();
			
			Hotspots = new List<ActivePolygon>();
			ItemsToLeave = new List<string>();
			ItemsToRemember = new List<string>();
			Camera = new CameraSingle (G.SCREEN_WIDTH, G.SCREEN_HEIGHT);
		}

		public Player Player;
		public CameraSingle Camera; 

		public Texture2D Background;
		public List<GameItem> Items;
		public List<ActivePolygon> Hotspots;
		public Polygon NavMesh;

		// I want to get rid of this whole chunk so bad
		public List<string> ItemsToLeave;
		public List<string> ItemsToRemember;
		public bool IsLevelComplete;
		public bool CanLeaveLevel;
		public MemoryItem Reward;

		protected virtual void OnLevelStart(string LastScreen) { }
		protected virtual void OnLevelComplete() { }

		public override void OnFocus()
		{
			OnLevelStart (G.LastScreen);
		}

		public override void Draw (SpriteBatch batch)
		{
			BeginDraw (batch, BlendState.NonPremultiplied);
			batch.Draw (Background, Vector2.Zero, Color.White);
			Items.ForEach (i => i.Draw (batch));
			Player.Draw (batch);
			batch.End();

			if (G.DebugCollision)
				DrawDebug();
		}

		public override void Update(GameTime gameTime)
		{
			Items.ForEach (i => i.Update (gameTime));

			Player.Update(gameTime, NavMesh);
			Camera.CenterOnPoint (Player.Location);
		}

		public override bool HandleInput (GameTime gameTime)
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
					bool hotSpotClicked = Vector2.DistanceSquared (Player.Location, spot.AbsoluteCenter) < 128
						&& CollisionChecker.PointToPoly (Camera.ScreenToWorld (target), spot);

					if (hotSpotClicked)
						spot.OnActivate (this);
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

		private MouseState _oldMouse;
		private KeyboardState _oldKey;

		private void OnItemFound (GameItem item)
		{
			if (ItemsToLeave.Contains(item.Name)) {
				ItemsToLeave.Remove(item.Name);
			}
			if (ItemsToRemember.Contains(item.Name)) {
				ItemsToRemember.Remove(item.Name);
			}

			CanLeaveLevel = (ItemsToLeave.Count == 0);
			IsLevelComplete = (ItemsToRemember.Count == 0);

			item.Clicked(this);

			if (IsLevelComplete) {
				IsLevelComplete = true;
				OnLevelComplete();
			}
		}

		private void DrawDebug()
		{
			G.CollisionRenderer.Begin (Camera.Transformation);
			foreach (var item in Items)
			{
				G.CollisionRenderer.Draw (item, Color.Lime);
			}
			foreach (var hotspot in Hotspots)
			{
				G.CollisionRenderer.DrawPolygon (hotspot, Color.Red);
			}
			G.CollisionRenderer.DrawPolygon (NavMesh, Color.Yellow);
			G.CollisionRenderer.Stop();
		}

		protected Sprite CreateSprite(string texturePath, int x = 0, int y = 0)
		{
			return new Sprite
			{
				Texture = G.C.Load<Texture2D> (texturePath),
				Location = new Vector2 (x, y)
			};
		}

		protected  GameItem CreateItem(string name, string texturePath,
			int radius, int x, int y)
		{
			var item = new GameItem (name, G.C.Load<Texture2D> (texturePath))
			{
				CollisionData = new Circlegon (radius),
				Location = new Vector2 (x, y)
			};
			return item;
		}

		protected void BeginDraw(SpriteBatch batch, BlendState state)
		{
			batch.Begin (
				SpriteSortMode.Deferred,
				state,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullCounterClockwise,
				null,
				Camera.Transformation);
		}
	}
}
