using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Items
{
	public class GameItem
		: Sprite
	{
		public GameItem (string name, Texture2D texture)
		{
			Name = name;
			Texture = texture;
			ItemDictionary.Add(name, this);
		}

		public static List<Tuple<string, string, GameItem>>  CraftingList = new List<Tuple<string, string, GameItem>> ();
		public static Dictionary<string, GameItem> ItemDictionary = new Dictionary<string, GameItem>(); 

		public static void AddCraftingRecipie(string item1, string item2, GameItem result)
		{
			CraftingList.Add(new Tuple<string, string, GameItem>(item1, item2, result));
		}

		public GameItem AttemptCraft(GameItem other)
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

		public bool CanPickup;

		public bool IsActive;

		public Action<MemoryState> OnClick;

		public string Name;

		public void Clicked(MemoryState ugh)
		{
			IsFound = true;

			var handler = OnClick;
			if (handler != null)
				handler(ugh);

		}
	}
}
