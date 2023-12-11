
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Saket.Inventory;

	[System.Serializable]
	public class Inventory
	{
		public List<ItemInstance> Items => items;
		public uint InventorySize
		{
			get => inventorySize;
			set
			{
				inventorySize = value;
				//Array.Resize(ref items, (int)value);
			}
		}
		public int AvaliableSpace
		{
			get
			{
				return ((int)inventorySize - (int)items.Count);
			}
		}

		


		public Inventory(uint inventorySize)
		{
			this.inventorySize = inventorySize;
			this.items = new List<ItemInstance>();
		}
		

		private uint inventorySize;

		private List<ItemInstance> items;

		public void AddItem(ItemReference[] itemReferences)
		{
			for (int i = 0; i < itemReferences.Length; i++)
			{
				AddItem(itemReferences[i]);
			}
		}
		public void AddItem(ItemReference itemReference)
		{
			Item item = null;// StaticDatabase.LoadItem(itemReference.itemID);

			// Stack Items if possible
			if (item.Stackable)
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (items[i].itemID == itemReference.itemID)
					{
						uint totalCount = items[i].count + itemReference.count;
						items[i] = new ItemInstance(items[i]) { count = totalCount };


						return;
					}
				}
			}
			// try stack failed 
			// add as new item

			int order = 0;
			items.ForEach((x) => { if (!x.equipped) order++; });

			if (items.Count < inventorySize)
			{
				items.Add(new ItemInstance(itemReference.itemID, itemReference.count, order, false));

				return;
			}

			Console.WriteLine("Inventory full, item discarded"); // WIP
		}

		public bool HasItem(ItemReference[] _itemsReferences)
		{
			for (int i = 0; i < _itemsReferences.Length; i++)
			{
				HasItem(_itemsReferences[i]);
			}
			return true;
		}
		public bool HasItem(ItemReference _itemReference)
		{
			// Find the item
			int index = items.FindIndex((x) => x.itemID == _itemReference.itemID);
			// If item doesn't exist
			if (index == -1)
				return false;
			// If doesn't have enough
			if (items[index].count < _itemReference.count)
				return false;

			return true;
		}

		public void RemoveItem(ItemReference[] _itemsReferences)
		{
			for (int i = 0; i < _itemsReferences.Length; i++)
			{
				RemoveItem(_itemsReferences[i]);
			}
		}
		public void RemoveItem(ItemReference _itemReference)
		{
			// Find the item
			int index = items.FindIndex((x) => x.itemID == _itemReference.itemID);
			// If item doesn't exist
			if (index == -1)
				return;
			// Remove 
			// If is totally removed

			uint newCount = items[index].count - _itemReference.count;

			if (newCount <= 0)
				items.RemoveAt(index); // Remove totally
			else
				// Update count
				items[index] = new ItemInstance(items[index]) { count = (newCount) };
		}

		
	}