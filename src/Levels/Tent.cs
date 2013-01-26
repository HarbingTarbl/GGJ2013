using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Collision;
using Jammy.StateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013
{
	public partial class G
	{
		public void LoadAreaTent()
		{
			var tent = new BaseMemoryState ("Tent", "", "");
			tent.Texture = C.Load<Texture2D> ("Area_Tent");

			tent.NavMesh = new Polygon (
				new Vector2 (0, 0),
				new Vector2 (0, 5),
				new Vector2 (1, 7),
				new Vector2 (2, 9),
				new Vector2 (5, 14));

			StateManager.Add (tent);
		}
	}
}
