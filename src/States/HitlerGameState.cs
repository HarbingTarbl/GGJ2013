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
			ParallaxLayer = new StaticParallax();
			Camera = new CameraSingle(LiterallyHitler.Instance.GraphicsDevice.Viewport.Width,
			                          LiterallyHitler.Instance.GraphicsDevice.Viewport.Height);
			NextState = nextState;
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


		public Sprite Player;
		public CameraSingle Camera; 

		public MemoryItem Reward;

		public bool IsLevelComplete;
		public string NextState;

		public event Action<bool, ReminderItem> ItemFound;
		public event Action<HitlerGameState> LevelComplete;

		public StaticParallax ParallaxLayer;

		public override void Draw(SpriteBatch batch)
		{
			ParallaxLayer.Draw(batch);

			foreach (var item in Items)
			{
				item.Draw(batch);
			}
		}

		public override void Update(GameTime gameTime)
		{


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
				}
			}

			_oldMouse = cMouse;
			return base.HandleInput(gameTime);
		}

		protected void OnItemFound(ReminderItem item)
		{
			_foundItems++;

			item.Clicked(_foundItems == Items.Count);

			var handler = ItemFound;
			if (handler != null)
				handler(_foundItems == Items.Count, item);

			if (_foundItems == Items.Count)
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
