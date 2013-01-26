using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
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

		public Action<BaseMemoryState> OnClick;

		public string Name;

		public void Clicked(BaseMemoryState ugh)
		{
			var handler = OnClick;
			if (handler != null)
				handler(ugh);

			IsFound = true;
		}
		
	}
}
