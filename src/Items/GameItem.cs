using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GGJ2013.States;
using Jammy.Collision;
using Jammy.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Items
{
	public class GameItem
		: Sprite, IInteractable
	{
		public GameItem (string name, Texture2D texture)
		{
			Name = name;
			Texture = texture;
			ItemDictionary.Add(name, this);

			IsMouseHover = true;
		}

		public static List<Tuple<string, string, Action >>  CraftingList = new List<Tuple<string, string, Action>> ();
		public static Dictionary<string, GameItem> ItemDictionary = new Dictionary<string, GameItem>(); 

		public static void AddCraftingRecipie(string item1, string item2, Action result)
		{
			CraftingList.Add(new Tuple<string, string, Action>(item1, item2, result));
		}

		public GameItem AttemptCraft(GameItem other)
		{
			foreach (var tup in CraftingList)
			{
				if ((tup.Item1 == Name && tup.Item2 == other.Name)
				    || (tup.Item1 == other.Name && tup.Item2 == Name))
				{
					tup.Item3();
					break;
				}
			}

			return null;
		}

		// IInteractable
		public string Name { get; set; }
		public Polygon Region { get { return CollisionData; } }
		public bool IsUsable { get { return CanPickup && IsActive; } }

		public bool IsMouseHover { get; set; }

		public Texture2D InventoryIcon;
		public Action<MemoryState> OnClick;

		public bool IsFound;
		public bool CanPickup;
		public bool IsActive;
		public string Description;

		public void Clicked(MemoryState ugh)
		{
			Trace.WriteLine ("Found clicked " + Name);
			IsFound = true;

			var handler = OnClick;
			if (handler != null)
				handler(ugh);

		}	
	}
}
