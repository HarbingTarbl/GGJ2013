using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Items;
using Jammy.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Interface
{
	public class IventoryManager
	{
		public List<string> CurrentItems;

		public Rectagon Bounds = new Rectagon(0, 0, G.SCREEN_HEIGHT, 100);
		public bool IsShown;
		public Vector2 SlotPadding = new Vector2(10, 0);

		public string SelectItemAt(Vector2 point)
		{
			point -= Bounds.Location;
			var item = (int)(point.X/(100 + SlotPadding.X));
			if (item > CurrentItems.Count)
				return CurrentItems[item];

			return null;
		}
		
		public void Draw(SpriteBatch batch)
		{
			if (!IsShown)
				return;

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			var offset = Bounds.Location;
			foreach (var str in CurrentItems)
			{
				var item = GameItem.ItemDictionary[str];
				_itemSlot.X = (int) offset.X;
				_itemSlot.Y = (int) offset.Y;
				batch.Draw(item.Texture, _itemSlot, Color.White);
				offset += SlotPadding;
			}
			batch.End();

			offset = Bounds.Location;

			G.Debug.Begin(Matrix.Identity);
			foreach (var str in CurrentItems)
			{
				var item = GameItem.ItemDictionary[str];
				_itemFrame.Location = offset;
				G.Debug.DrawPolygon(_itemFrame, Color.Black);

				offset += SlotPadding;
			}
			
			G.Debug.Stop();

		}


		private Rectangle _itemSlot = new Rectangle(0, 0, 100, 100);
		private Rectagon _itemFrame = new Rectagon(0, 0, 100, 100);

	}
}
