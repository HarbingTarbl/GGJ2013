﻿using System;
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
			Dialog = new DialogManager()
			{
				MessageBounds = new Rectangle(15, 15, 300, 300),
				Font = G.C.Load<SpriteFont>("fonts/debug"),
			};
			NextLevel = next;
			LastLevel = prev;
			Camera = new CameraSingle (G.SCREEN_WIDTH, G.SCREEN_HEIGHT);
		}

		public Player Player;
		public CameraSingle Camera; 

		public Texture2D Background;
		public List<GameItem> Items;
		public List<Hotspot> Hotspots;
		public List<PolyNode> Nav;
		public List<Sprite> Lights; 
		public DialogManager Dialog;
		

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

		protected virtual void OnLevelStart(string LastScreen) { }
		protected virtual void OnLevelComplete() { }

		public override void OnFocus()
		{
			OnLevelStart (G.LastScreen);
			Camera.Bounds = new Rectangle(0, 0, Background.Width, Background.Height);
			Camera.UseBounds = true;
		}

		public override void Draw (SpriteBatch batch)
		{
			G.BloomRenderer.BeginDraw();

			BeginDraw (batch, BlendState.NonPremultiplied);
			batch.Draw (Background, Vector2.Zero, Color.White);
			Items.ForEach (i => i.Draw (batch));
			Player.Draw (batch);
			batch.End();

			BeginDraw(batch, BlendState.AlphaBlend);
			foreach (var light in Lights)
			{
				light.Draw(batch);
			}
			batch.End();

			G.BloomRenderer.Draw (G.GameTime);

			if (G.DebugCollision)
				DrawDebug();

			G.InventoryManager.Draw(batch);
			Dialog.Draw(batch);

		}

		public override void Update(GameTime gameTime)
		{
			Items.ForEach (i => i.Update (gameTime));

			Player.Update(gameTime);
			Camera.CenterOnPoint(Player.Location.X + Player.Texture.Width/2f, Background.Width/2f);

			Dialog.Update(gameTime);
		}

		public override bool HandleInput (GameTime gameTime)
		{
			var mouse = Mouse.GetState();
			var target = new Vector2(mouse.X, mouse.Y);

			if (CollisionChecker.PointToPoly(target, G.InventoryManager.Bounds))
				G.InventoryManager.IsShown = true;
			else G.InventoryManager.IsShown = false;


			if (mouse.LeftButton.WasButtonPressed(_oldMouse.LeftButton))
			{
				if (G.InventoryManager.IsShown)
				{
					LastLevel = CurrentItem;
					CurrentItem = G.InventoryManager.SelectItemAt(target);
				}

				if (G.Active)
				{
					var t = Camera.ScreenToWorld (target);
					Trace.WriteLine (String.Format ("({0}, {1})", t.X, t.Y));
				}

				var myPoly = Nav.Where (node => CollisionChecker.PointToPoly (
					Player.Location, node.Poly)).FirstOrDefault();

				var targetPoly = Nav.Where (node => CollisionChecker.PointToPoly (
					Camera.ScreenToWorld (target), node.Poly)).FirstOrDefault();
				
				if (targetPoly != null)
				{
					if (targetPoly == myPoly)
					{
						Player.ClearMove();
						Player.MoveQueue.Enqueue (Camera.ScreenToWorld (target));
					} else {
						// Clicked in a non direct polygon
						throw new Exception();
					}
				}
				else {
					Trace.WriteLine ("Did not click in a valid polygon");
				}
				
				for (var i = 0; i < Items.Count; i++)
				{
					var item = Items[i];
					if(item.IsFound || !item.IsActive)
						continue;

					if ( //TODO this is buggy. Needs actual player collision
						CollisionChecker.PointToPoly(target, item.CollisionData))
					{
						OnItemFound(item);
						if (item.CanPickup)
						{
							G.InventoryManager.CurrentItems.Add(item.Name);
							Items.RemoveAt(i);
						}
						break;
						
					}
				}

				foreach (var spot in Hotspots)
				{
					bool hotSpotClicked = 
						 CollisionChecker.PointToPoly (target, spot);

					if (hotSpotClicked)
						spot.OnActivate (this);
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
			_oldMouse = mouse;
			return base.HandleInput(gameTime);
		}

		private MouseState _oldMouse;
		private KeyboardState _oldKey;
		private Polygon _insideMesh;

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
			G.Debug.Begin (Camera.Transformation);
			foreach (var item in Items) {
				G.Debug.Draw (item, Color.Lime);
			}
			foreach (var hotspot in Hotspots) {
				G.Debug.DrawPolygon (hotspot, Color.Red);
			}
			foreach (var polyNode in Nav)
			{
				G.Debug.DrawPolygon (polyNode.Poly, Color.Yellow);
			}
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

		protected  GameItem CreateItem (string name, string description, string texturePath,
			int radius, int x, int y)
		{
			var item = new GameItem (name, G.C.Load<Texture2D> (texturePath))
			{
				CollisionData = new Circlegon (radius),
				Location = new Vector2 (x, y)
			};
			return item;
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
