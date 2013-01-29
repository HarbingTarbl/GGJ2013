using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Collision;
using Memory.Items;
using Memory.States;
using Microsoft.Xna.Framework;

namespace Memory.Collision
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
