using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GGJ2013.Collision;
using GGJ2013.Interface;
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
		public MemoryState(string name, string next, string prev)
			: base(name)
		{
			Player = G.Player;

			Items = new List<GameItem>();
			Hotspots = new List<Hotspot>();
			ItemsToLeave = new List<string>();
			ItemsToRemember = new List<string>();
			Lights = new List<Sprite>();

			NextLevel = next;
			LastLevel = prev;
			Camera = new CameraSingle (G.SCREEN_WIDTH, G.SCREEN_HEIGHT);

			InventoryOpen = new Hotspot( //Replace with Sprite
				"Open Inventory",
				new Rectagon(10, 5, 18, 55),
				(t,i) =>
				{
					G.InventoryManager.IsShown = !G.InventoryManager.IsShown;
					InventoryOpen.Rotation = G.InventoryManager.IsShown ? -MathHelper.PiOver2 : MathHelper.PiOver2;

					InventoryOpen.Location.Y += G.InventoryManager.IsShown
						                            ? G.InventoryManager.Bounds.Bottom + 25
						                            : -G.InventoryManager.Bounds.Bottom - 25;

					InventoryOpen.Name = G.InventoryManager.IsShown ? "Close Inventory" : "Open Inventory";

				}) {EnforceDistance = false};

			Hotspots.Add(InventoryOpen);
		}

		public Player Player;
		public CameraSingle Camera; 

		public Texture2D Background;
		public List<GameItem> Items;
		public List<Hotspot> Hotspots;
		public List<PolyNode> Nav;
		public List<Sprite> Lights;

		public Hotspot InventoryOpen;

		// I want to get rid of this whole chunk so bad
		public List<string> ItemsToLeave;
		public List<string> ItemsToRemember;
		public bool IsLevelComplete;
		public bool CanLeaveLevel;
		public MemoryItem Reward;

		public string NextLevel;
		public string LastLevel;

		public string CurrentItem;
		public string LastItem;

		protected virtual void OnLevelStart (string LastScreen)
		{
			if (LastScreen != null)
			{
				G.InventoryManager.IsShown = false;
			}
		}
		protected virtual void OnLevelComplete() { }

		public override void OnFocus()
		{

			OnLevelStart (LastLevel);
			Camera.Location = new Vector2(0, 0);
			Camera.Bounds = new Rectangle(0, 0, Background.Width, Background.Height);
			Camera.UseBounds = true;
		}

		public override void Draw (SpriteBatch batch)
		{
			//G.BloomRenderer.BeginDraw();
			var m = Mouse.GetState();
			BeginDraw (batch, BlendState.NonPremultiplied);
			batch.Draw (Background, Vector2.Zero, Color.White);
			Items.ForEach (i => i.Draw (batch));
			Player.Draw (batch);
			if (HeldItem != null && !WasReleased)
				batch.Draw(HeldItem.InventoryIcon, new Rectangle(m.X - 50, m.Y - 50, 100, 100), Color.White);
			batch.End();

			BeginDraw(batch, BlendState.AlphaBlend);
			foreach (var light in Lights) {
				light.Draw(batch);
			}
			batch.End();

			//G.BloomRenderer.Draw (G.GameTime);

			if (G.DebugCollision)
				DrawDebug();

			ShowItemHint();
			G.InventoryManager.Draw(batch);
			G.DialogManager.Draw(batch);
		}

		private void ShowItemHint()
		{
			var mouse = Mouse.GetState();
			var target = Camera.ScreenToWorld(new Vector2 (mouse.X, mouse.Y));

			var item =
				Items.FirstOrDefault(i => CollisionChecker.PointToPoly(target, i.CollisionData) && (i.IsActive || Keyboard.GetState().IsKeyDown(Keys.Space)));

			if (item != null)
			{
				G.DialogManager.PostMessage (item.Name, Vector2.Transform (
					item.CollisionData.Location + new Vector2(item.CollisionData.Left, item.CollisionData.Top) + new Vector2 (item.CollisionData.Width / 2f, -10),
					Camera.Transformation), TimeSpan.Zero, TimeSpan.Zero, Color.Gray);

				if (Keyboard.GetState().IsKeyDown(Keys.Space))
				{
					item.Location = target - new Vector2(item.CollisionData.Width/2f, item.CollisionData.Height/2f);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.P))
					Trace.WriteLine(string.Format("new Vector2({0},{1})", item.Location.X, item.Location.Y));

				

				return;
			}

			var hotspot =
				Hotspots.FirstOrDefault(i => CollisionChecker.PointToPoly(target, i));

			if (hotspot != null)
			{
				G.DialogManager.PostMessage (hotspot.Name, Vector2.Transform (
					hotspot.Location + new Vector2(hotspot.Left, hotspot.Top) + new Vector2 (hotspot.Width / 2f, -10),
					Camera.Transformation), TimeSpan.Zero, TimeSpan.Zero, Color.Gray);

			}
		}

		public override void Update(GameTime gameTime)
		{
			Items.ForEach (i => i.Update (gameTime));

			Player.Update(gameTime);
			Camera.CenterOnPoint(Player.Location.X, Background.Height /2f);
			InventoryOpen.Location = Camera.Location + new Vector2(10, 5) + ((G.InventoryManager.IsShown)
				                                                                 ? new Vector2(0,
				                                                                               G.InventoryManager.Bounds.Bottom + 25)
				                                                                 : Vector2.Zero);

			G.DialogManager.Update(gameTime);
		}


		public override bool HandleInput(GameTime gameTime)
		{
			if (!G.Active)
				return false;

			var mouse = Mouse.GetState ();
			var target = Camera.ScreenToWorld(new Vector2(mouse.X, mouse.Y));
			var screen = new Vector2(mouse.X, mouse.Y);

			if (mouse.LeftButton == ButtonState.Pressed && WasReleased)
			{
				if(CollisionChecker.PointToPoly(screen, G.InventoryManager.Bounds)
			    && G.InventoryManager.IsShown)
					{
						WasReleased = false;

						var item = G.InventoryManager.SelectItemAt(screen);
						if (item != null)
							HeldItem = GameItem.ItemDictionary[item];

					}

			}
				


			if (mouse.LeftButton.WasButtonRelease(_oldMouse.LeftButton))
			{
				WasReleased = true;
				Player.Target = null;

				//TODO: take out later
				var t = Camera.ScreenToWorld(target);
				Trace.WriteLine(String.Format("new Vector2({0}, {1}),", t.X, t.Y));

				var myPoly = Nav.Where(node => CollisionChecker.PointToPoly(
					Player.Location, node.Poly)).FirstOrDefault();

				var targetPoly = Nav.Where(node => CollisionChecker.PointToPoly(
					Camera.ScreenToWorld(target), node.Poly)).FirstOrDefault();

				for (var i = 0; i < Items.Count; i++)
				{
					var item = Items[i];
					if (item.IsFound || !item.IsActive)
						continue;

					if (CollisionChecker.PointToPoly(target, item.CollisionData))
					{
						if (!CollisionChecker.PolyToPoly(Player.CollisionData, item.CollisionData))
						{
							Player.Target = item;
							Player.TargerIsItem = true;
						}
						else
						{
							OnItemFound(item);

						}
						break;
					}
				}


				if (G.InventoryManager.IsShown
				    && CollisionChecker.PointToPoly(target, G.InventoryManager.Bounds))
				{

					var item  = G.InventoryManager.SelectItemAt(screen);
					if (item != null)
					{
						if (HeldItem == null
						    || HeldItem.Name == item)
						{
							G.DialogManager.PostMessage(GameItem.ItemDictionary[item].Description, TimeSpan.Zero,
							                            new TimeSpan(0, 0, 5),
							                            Color.White);
						}
						else
						{
							HeldItem.AttemptCraft(GameItem.ItemDictionary[item]);
						}
					}

				}



				foreach (var spot in Hotspots)
				{
					bool hotSpotClicked = spot.Enabled &&
					                      CollisionChecker.PointToPoly(target, spot);

					if (hotSpotClicked)
					{
						if (!spot.EnforceDistance)
						{
							spot.OnActivate(this, HeldItem);
						}
						else if (!CollisionChecker.PolyToPoly(spot, Player.CollisionData))
						{
							Player.Target = spot;
							Player.TargerIsItem = false;
							if(spot.WalkLocation != Vector2.Zero)
							targetPoly = Nav.Where(node => CollisionChecker.PointToPoly(
								spot.WalkLocation, node.Poly)).FirstOrDefault();
						}
						else
						{
							spot.OnActivate(this, HeldItem);

						}
					}

				}




				if (targetPoly != null && Player.AnimationManager.CurrentAnimation.Name != "Pick Up")
				{
					if (targetPoly == myPoly)
					{
						Player.ClearMove();

						if (Player.Target != null && Player.TargerIsItem == false && ((Hotspot)Player.Target).WalkLocation != Vector2.Zero)
						{
							Player.MoveQueue.Enqueue(((Hotspot)Player.Target).WalkLocation);
						}
						else
							Player.MoveQueue.Enqueue(Camera.ScreenToWorld(target));
					}
					else
					{
						List<Vector2> points;
						if (Player.Target != null && Player.TargerIsItem == false && ((Hotspot)Player.Target).WalkLocation != Vector2.Zero)
						{
							points = PathFinder.CalculatePath(
							Player.Location, ((Hotspot)Player.Target).WalkLocation, Nav);
						}
						else
						 points = PathFinder.CalculatePath(
							Player.Location, Camera.ScreenToWorld(target), Nav);

						Player.ClearMove();
						points.ForEach(v => Player.MoveQueue.Enqueue(v));
					}
				}
				else
				{
					//Trace.WriteLine ("Did not click in a valid polygon");
				}
			}

			var keystate = Keyboard.GetState ();
			Player.Location += new Vector2 ((keystate.IsKeyDown (Keys.D) ? 10 : 0) - (keystate.IsKeyDown (Keys.A) ? 10 : 0),
										   (keystate.IsKeyDown (Keys.S) ? 10 : 0 - (keystate.IsKeyDown (Keys.W) ? 10 : 0)));

			if (keystate.IsKeyDown (Keys.F1)
				&& _oldKey.IsKeyUp (Keys.F1))
			{
				G.DebugCollision = !G.DebugCollision;
			}

			_oldKey = keystate;
			_oldMouse = mouse;
			return base.HandleInput (gameTime);
		}


		public GameItem HeldItem;
		public bool WasReleased = false;

		protected MouseState _oldMouse;
		private KeyboardState _oldKey;
		private Polygon _insideMesh;

		public void OnItemFound (GameItem item)
		{
			if (item.CanPickup)
			{
				G.InventoryManager.CurrentItems.Add(item.Name);
				Items.Remove(item);
				Player.AnimationManager.SetAnimation("Pick Up");
			}


			if (ItemsToLeave.Contains(item.Name)) {
				ItemsToLeave.Remove(item.Name);
			}
			if (ItemsToRemember.Contains(item.Name)) {
				ItemsToRemember.Remove(item.Name);
			}

			CanLeaveLevel = (ItemsToLeave.Count == 0);
			IsLevelComplete = (ItemsToRemember.Count == 0);

			item.Clicked (this);

			if (IsLevelComplete) {
				IsLevelComplete = true;
				OnLevelComplete();
			}
		}

		private void DrawDebug()
		{
			G.Debug.Begin (Camera.Transformation);
			foreach (var item in Items) {
				G.Debug.Draw (item, Color.Blue);
			}
			foreach (var hotspot in Hotspots) {
				G.Debug.DrawPolygon (hotspot, Color.Red);
			}
			foreach (var polyNode in Nav) {
				G.Debug.DrawPolygon (polyNode.Poly, Color.Yellow);
			}
			G.Debug.DrawPolygon(G.InventoryManager.Bounds, Color.Tomato);
			G.Debug.DrawPolygon(Player.CollisionData, Color.Lime);

			G.Debug.Stop();
		}

		protected Sprite CreateSprite (string texturePath, int x = 0, int y = 0)
		{
			return new Sprite
			{
				Texture = G.C.Load<Texture2D> (texturePath),
				Location = new Vector2 (x, y)
			};
		}

		protected GameItem CreateItem(string name, string desc,
		                              string texturePath, string iconPath, int x, int y, params Vector2[] verts)
		{
			var item = new GameItem(name, G.C.Load<Texture2D>(texturePath))
			{
				CollisionData = new Polygon(verts),
				Location = new Vector2(x, y),
				Description = desc,
				InventoryIcon = G.C.Load<Texture2D>(iconPath)
			};
			return item;
		}

		protected GameItem CreateItem (string name, string desc,
			string texturePath, int x, int y, params Vector2[] verts)
		{
			return CreateItem(name, desc, texturePath, texturePath, x, y, verts);
		}

		protected void BeginDraw (SpriteBatch batch, BlendState state)
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
