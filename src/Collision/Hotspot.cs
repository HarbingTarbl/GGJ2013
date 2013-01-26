using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Collision;

namespace GGJ2013.Collision
{
	public class Hotspot
		: Polygon
	{
		public Hotspot(Polygon verts, Action<MemoryState> action)
		{
			Vertices.AddRange(verts.Vertices);
			Location = verts.Location;
			Activated += action;
		}

		public event Action<MemoryState> Activated;

		public void OnActivate(MemoryState state)
		{
			var handler = Activated;
			if (handler != null)
				Activated(state);
		}

	}
}
