using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy.Sprites;

namespace GGJ2013.Items
{
	public class ReminderItem
		: Sprite
	{
		public ReminderItem(string name)
		{
			Name = name;
		}

		public bool IsFound;

		public Action<bool> OnClick;

		public string Name;

		public void Clicked(bool ugh)
		{
			var handler = OnClick;
			if (handler != null)
				handler(ugh);

			IsFound = true;
		}
		
	}
}
