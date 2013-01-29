using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Collision;

namespace GGJ2013.Items
{
	public interface IInteractable
	{
		string Name { get; }
		Polygon Region { get; }
		bool IsUsable { get; }
		bool IsMouseHover { get; }
	}
}
