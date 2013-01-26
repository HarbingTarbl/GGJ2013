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
			Player = new Player()
			{
				Location = new Vector2(G.SCREEN_WIDTH/2f, G.SCREEN_HEIGHT/2f)
			};
			ItemsToLeave = new List<string>();
			ItemsToRemember = new List<string>();
			Camera = new CameraSingle (G.SCREEN_WIDTH, G.SCREEN_HEIGHT);
			
			NextState = nextState;
		}

		public List<ReminderItem> Items;
		public List<ActivePolygon> Hotspots;

		public List<string> ItemsToLeave;
		public List<string> ItemsToRemember;

		public Polygon NavMesh;
		public Polygon InsideMesh
		{
			get { return _insideMesh; }
			set
			{
				_insideMesh = value;
				OutsideMeshes = Polygon.Clip(new Rectagon(0, 0, G.SCREEN_WIDTH, G.SCREEN_HEIGHT), value);
			}

		}
		public List<Polygon> OutsideMeshes;

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

		public override void Draw (SpriteBatch batch)
		{
			batch.Begin (
				SpriteSortMode.Deferred,
			    BlendState.NonPremultiplied,
			    SamplerState.PointClamp,
			    DepthStencilState.Default,
			    RasterizerState.CullCounterClockwise,
			    null,
				Camera.Transformation);

			batch.Draw (Texture, Vector2.Zero, Color.White);
			Items.ForEach (i => i.Draw (batch));
			Player.Draw (batch);
			batch.End();

			if (G.DebugCollision)
				DrawDebug();
		}

		public override void Update(GameTime gameTime)
		{
			Items.ForEach (i => i.Update (gameTime));

			//FIX: FIX ME
			Player.Update(gameTime, NavMesh);
			Camera.CenterOnPoint(Player.Location);
		}

		public void NextLevel()
		{
			G.StateManager.Pop();
			G.StateManager.Push (NextState);
		}

		public override bool HandleInput(GameTime gameTime)
		{
			var cMouse = Mouse.GetState();
			var target = new Vector2(cMouse.X, cMouse.Y);


			if (cMouse.LeftButton.WasButtonPressed(_oldMouse.LeftButton))
			{

				if (CollisionChecker.PointToPoly(Camera.ScreenToWorld(target), InsideMesh))
				{
					Player.Destination = target;
					Player.Direction = Player.Destination - Player.Location;
					Player.Direction.Normalize();
					var line = new Polygon(Player.Location, Player.Destination);
					foreach (var poly in OutsideMeshes)
					{
						if (CollisionChecker.PolyToPoly(line, poly)) //Use nav
						{
							var closest = 0;
							var dist = Vector2.Distance(NavMesh.Vertices[0], Player.Location);
							for (var i = 1; i < NavMesh.Vertices.Count; i++)
							{
								var testDist = Vector2.DistanceSquared(NavMesh.Vertices[i], Player.Location);
								if(testDist < dist)
								{
									closest = i;
									dist = testDist;
								}
							}


							Player.Direction = NavMesh.Vertices[(closest + 1)%NavMesh.Vertices.Count] - NavMesh.Vertices[closest];
							Player.Direction.Normalize();

							break;
						}
					}


				}


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
			Player.Location += new Vector2((keystate.IsKeyDown(Keys.D) ? 1 : 0) - (keystate.IsKeyDown(Keys.A) ? 1 : 0),
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

		protected void OnItemFound (ReminderItem item)
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
		private Polygon _insideMesh;

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
			G.CollisionRenderer.DrawPolygon (NavMesh, Color.Black);
			G.CollisionRenderer.DrawPolygon(InsideMesh, Color.Purple);

			foreach (var poly in OutsideMeshes)
			{
				G.CollisionRenderer.DrawPolygon(poly, Color.Yellow);
			}
			G.CollisionRenderer.Stop ();
		}
	}
}
