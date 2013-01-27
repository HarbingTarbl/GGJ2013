using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Collision;
using Microsoft.Xna.Framework;

namespace GGJ2013.Collision
{
	public class Hotspot
		: Polygon
	{
		public string Name;

		public Hotspot(string name, Polygon verts, Action<MemoryState> action)
		{
			Name = name;
			Vertices.AddRange(verts.Vertices);
			Location = verts.Location;
			Activated += action;
		}

		public event Action<MemoryState> Activated;

		public bool Enabled = true;
		public bool EnforceDistance = true;

		public Vector2 WalkLocation; 

		public void OnActivate(MemoryState state)
		{
			var handler = Activated;
			if (handler != null)
				Activated(state);
		}

	}
}
