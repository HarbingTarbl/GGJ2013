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
	public class InventoryManager
	{
		public InventoryManager()
		{
			CurrentItems = new List<string>();
		}

		public List<string> CurrentItems;

		public Rectagon Bounds = new Rectagon(10, 5, G.SCREEN_WIDTH, 100);
		public bool IsShown;
		public Vector2 SlotPadding = new Vector2(10, 0);

		public string SelectItemAt(Vector2 point)
		{
			point -= Bounds.Location;
			var item = (int)(point.X/(100 + SlotPadding.X));
			if (item < CurrentItems.Count)
				return CurrentItems[item];

			return null;
		}
		
		public void Draw(SpriteBatch batch)
		{
			if (!IsShown || CurrentItems.Count == 0)
				return;

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			var offset = Bounds.Location;

			_itemSlot.X = (int)offset.X;
			_itemSlot.Y = (int)offset.Y;
			foreach (var str in CurrentItems)
			{
				var item = GameItem.ItemDictionary[str];

				batch.Draw(item.Texture, _itemSlot, Color.White);
				_itemSlot.X += (int)SlotPadding.X + _itemSlot.Width;
			}
			batch.End();

			offset = Bounds.Location;

			G.Debug.Begin(Matrix.Identity);
			_itemFrame.Location = offset;

			foreach (var str in CurrentItems)
			{
				G.Debug.DrawPolygon(_itemFrame, Color.Gray);

				_itemFrame.Location.X += SlotPadding.X + _itemFrame.Width;
			}
			
			G.Debug.Stop();

		}


		private Rectangle _itemSlot = new Rectangle(0, 0, 100, 100);
		private Rectagon _itemFrame = new Rectagon(0, 0, 100, 100);

	}
}
