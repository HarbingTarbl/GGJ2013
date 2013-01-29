using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Items;
using GGJ2013.States;
using Jammy.Collision;
using Microsoft.Xna.Framework;

namespace GGJ2013.Collision
{
	public class Hotspot
		: Polygon, IInteractable
	{
		public Hotspot(string name, Polygon poly, Action<MemoryState, GameItem> action)
		{
			Name = name;
			Vertices.AddRange(poly.Vertices);
			Location = poly.Location;
			Activated += action;

			IsUsable = true;
			IsMouseHover = true;
		}

		public string Name { get; set; }
		public Polygon Region { get { return this; } }
		public bool IsUsable { get; set; }
		public bool IsMouseHover { get; set; }

		public event Action<MemoryState, GameItem> Activated;
		public bool EnforceDistance = true;
		public Vector2 WalkLocation; 

		public void OnActivate(MemoryState state, GameItem draggedItem)
		{
			var handler = Activated;
			if (handler != null)
				Activated(state, draggedItem);
		}

	}
}
