using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.StateManager;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013
{
	public partial class G
	{
		public void LoadAreaTent()
		{
			var tent = new BaseMemoryState ("Tent", "", "");
			tent.Texture = C.Load<Texture2D> ("Area_Tent");

			StateManager.Add (tent);
		}
	}
}
