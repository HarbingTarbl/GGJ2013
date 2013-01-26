using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Items
{
	public class ReminderItem
		: Sprite
	{
		public ReminderItem(string name, Texture2D texture)
		{
			Name = name;
			Texture = texture;
			ItemDictionary.Add(name, this);
		}

		public static List<Tuple<string, string, ReminderItem>>  CraftingList = new List<Tuple<string, string, ReminderItem>> ();
		public static Dictionary<string, ReminderItem> ItemDictionary = new Dictionary<string, ReminderItem>(); 

		public static void AddCraftingRecipie(string item1, string item2, ReminderItem result)
		{
			CraftingList.Add(new Tuple<string, string, ReminderItem>(item1, item2, result));
		}

		public ReminderItem AttemptCraft(ReminderItem other)
		{
			foreach (var tup in CraftingList)
			{
				if ((tup.Item1 == Name && tup.Item2 == other.Name)
				    || (tup.Item1 == other.Name && tup.Item2 == Name))
				{
					return tup.Item3;
				}
			}

			return null;
		}

		public bool IsFound;

		public Action<BaseMemoryState> OnClick;

		public string Name;

		public void Clicked(BaseMemoryState ugh)
		{
			IsFound = true;

			var handler = OnClick;
			if (handler != null)
				handler(ugh);

		}
	}
}
