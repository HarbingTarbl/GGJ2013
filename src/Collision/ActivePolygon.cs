using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Collision;

namespace GGJ2013.Collision
{
	public class ActivePolygon
		: Polygon
	{
		public ActivePolygon(Polygon verts, Action<BaseMemoryState> action)
		{
			Vertices.AddRange(verts.Vertices);
			Location = verts.Location;
			Activated += action;
		}

		public event Action<BaseMemoryState> Activated;

		public void OnActivate(BaseMemoryState state)
		{
			var handler = Activated;
			if (handler != null)
				Activated(state);
		}

	}
}
