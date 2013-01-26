using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Items;
using GGJ2013.States;
using Jammy;
using Jammy.Collision;
using Jammy.Helpers;
using Jammy.Sprites;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013
{
	public class TentState
		: MemoryState
	{
		public TentState()
			: base ("Tent")
		{
			Background = G.C.Load<Texture2D> ("TentArea/background");
			#region NavMesh
			NavMesh = new Polygon (
				new Vector2 (-27.0f, 33.0f), new Vector2 (-41.0f, 31.0f), new Vector2 (-66.0f, 30.0f), new Vector2 (-66.0f, 24.0f),
				new Vector2 (-63.0f, 21.0f), new Vector2 (-61.0f, 17.0f), new Vector2 (-61.0f, 11.0f), new Vector2 (-62.0f, 6.0f),
				new Vector2 (-62.0f, 0.0f), new Vector2 (-61.0f, -6.0f), new Vector2 (-58.0f, -9.0f), new Vector2 (-54.0f, -11.0f),
				new Vector2 (-50.0f, -14.0f), new Vector2 (-41.0f, -16.0f), new Vector2 (-35.0f, -18.0f),
				new Vector2 (-33.0f, -22.0f),
				new Vector2 (-28.0f, -26.0f), new Vector2 (-17.0f, -28.0f), new Vector2 (-7.0f, -30.0f), new Vector2 (0.0f, -32.0f),
				new Vector2 (16.0f, -32.0f), new Vector2 (21.0f, -30.0f), new Vector2 (30.0f, -29.0f), new Vector2 (30.0f, -23.0f),
				new Vector2 (88.0f, -21.0f), new Vector2 (90.0f, -17.0f), new Vector2 (90.0f, -11.0f), new Vector2 (90.0f, -5.0f),
				new Vector2 (89.0f, 0.0f), new Vector2 (86.0f, 4.0f), new Vector2 (82.0f, 7.0f), new Vector2 (77.0f, 9.0f),
				new Vector2 (71.0f, 10.0f), new Vector2 (16.0f, 12.0f), new Vector2 (14.0f, 16.0f), new Vector2 (12.0f, 20.0f),
				new Vector2 (5.0f, 25.0f), new Vector2 (19.0f, 28.0f));
			NavMesh.Location = new Vector2 (150, 50);
			InsideMesh = NavMesh;
			#endregion

			var blanket = CreateItem ("Blanket", "TentArea/blanket", 30, 104, 503);
			var flash = CreateItem ("Flashlight", "TentArea/flashlight", 30, 587, 436);
			var bag = CreateItem ("Bag", "TentArea/dufflebag", 30, 723, 444);
			var sweater = CreateItem ("Sweater", "TentArea/sweater", 30, 380, 555);

			lantern = CreateSprite ("TentArea/lantern", 520, 0);
			light1 = CreateSprite ("TentArea/light1");
			light2 = CreateSprite ("TentArea/light2");

			light1.IsVisible = false;
			light2.IsVisible = false;

			Items.Add (blanket);
			Items.Add (flash);
			Items.Add (bag);
			Items.Add (sweater);
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

		private Sprite lantern;
		private Sprite light1;
		private Sprite light2;

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
