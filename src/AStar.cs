using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Collision;
using Microsoft.Xna.Framework;

namespace GGJ2013
{
	public class Pathfinder
	{
		public List<Vector2> FindPath (PolyLink Start, PolyLink End)
		{
			List<PolyLink> searched;

			PolyLink currentNode = Start;
			throw new NotImplementedException();

		}

		private static int GetCost(PolyLink a, PolyLink b)
		{
			return (int) Vector2.Distance (a.GetVertex(), b.GetVertex());
		}
	}
}
