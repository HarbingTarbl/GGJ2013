using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Collision;

namespace GGJ2013.Collision
{
	public class ActivePolygon
		: Polygon
	{
		public ActivePolygon(Polygon verts)
		{
			Vertices.AddRange(verts.Vertices);
		}

		public event Action Activated;

		public void OnActivate()
		{
			var handler = Activated;
			if (handler != null)
				Activated();
		}

	}
}
