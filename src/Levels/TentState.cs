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
