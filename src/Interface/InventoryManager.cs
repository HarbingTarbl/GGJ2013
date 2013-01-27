using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2013.Items;
using Jammy.Collision;
using Jammy.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Interface
{
	public class InventoryManager
	{
		public InventoryManager()
		{
			CurrentItems = new List<string>();
			_upArrow = G.C.Load<Texture2D>("UI/arrow_up");
			_downArrow = G.C.Load<Texture2D>("UI/arrow_down");
			_comparts = G.C.Load<Texture2D>("UI/compartments");
		}

		public List<string> CurrentItems;

		public Rectagon Bounds = new Rectagon(10, 5, G.SCREEN_WIDTH, 100);
		public bool IsShown;
		public Vector2 SlotPadding = new Vector2(20, 0);

		public string SelectedItem1;
		public string SelectedItem2;

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
			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
			batch.Draw(IsShown ? _upArrow : _downArrow, IsShown ? _arrowSlot + new Vector2(0, 100) : _arrowSlot, Color.White);
		
			if (IsShown)
			{

				var offset = Bounds.Location;
				//batch.Draw(_comparts, offset, Color.White);
				
				_itemSlot.X = (int) offset.X;
				_itemSlot.Y = (int) offset.Y;
				foreach (var str in CurrentItems)
				{
					var item = GameItem.ItemDictionary[str];

					batch.Draw(item.InventoryIcon, _itemSlot, Color.White);
					_itemSlot.X += (int) SlotPadding.X + _itemSlot.Width;
				}


				offset = Bounds.Location;

				if (CurrentItems.Count > 0)
				{
					G.Debug.Begin(Matrix.Identity);
					_itemFrame.Location = offset;

					foreach (var str in CurrentItems)
					{
						G.Debug.DrawPolygon(_itemFrame, str == SelectedItem1 || str == SelectedItem2 ? Color.Red : Color.Gray);
						_itemFrame.Location.X += SlotPadding.X + _itemFrame.Width;

					}

					G.Debug.Stop();
				}
			}
			batch.End();

		}


		private Texture2D _upArrow;
		private Texture2D _downArrow;
		private Texture2D _comparts;

		private Vector2 _arrowSlot = new Vector2(10, 5);

		private Rectangle _itemSlot = new Rectangle(0, 0, 100, 100);
		private Rectagon _itemFrame = new Rectagon(0, 0, 100, 100);

	}
}
