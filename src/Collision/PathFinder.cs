using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Jammy.Collision;
using Microsoft.Xna.Framework;

namespace GGJ2013.Collision
{
	public static class PathFinder
	{
		public static List<Vector2> CalculatePath (Vector2 start, Vector2 end,
			List<PolyNode> n)
		{
			var startPoly = n.FirstOrDefault (pn => CollisionChecker.PointToPoly (start, pn.Poly));
			var endPoly = n.FirstOrDefault (pn => CollisionChecker.PointToPoly (end, pn.Poly));

			PolyNode currentPoly = startPoly;
			List<Vector2> route = new List<Vector2>(new [] {
				start});

			int MAX_STEPS = 10;
			int steps = 0;
			while (true)
			{
				currentPoly = StepPath (route, currentPoly, endPoly, ref end);
				if (currentPoly == null)
					break;

				steps++;
				if (steps >= MAX_STEPS)
					throw new Exception();
			}

			route.Add (end);
			return route;
		}

		private static PolyNode StepPath (List<Vector2> route, PolyNode current,
			PolyNode end, ref Vector2 endv)
		{
			if (current == end) { 
				return null;
			}

			PolyLink shortestLink = null;
			float shortestDistance = int.MaxValue;

			foreach (PolyLink link in current.Links)
			{
				var d = Vector2.Distance (link.Target.GetVertex(), endv);
				if (shortestLink == null || d < shortestDistance) {
					shortestDistance = d;
					shortestLink = link.Target;
				}
			}

			if (shortestLink != null)
			{
				route.Add (shortestLink.GetVertex());
				return shortestLink.Parent;
			}

			return null;
		}
	}
}
