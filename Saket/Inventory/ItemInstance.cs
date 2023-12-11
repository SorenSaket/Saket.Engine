using System.Collections;
using System.Collections.Generic;

namespace Saket.Inventory; 

[System.Serializable]
public struct ItemInstance 
{
	public string itemID;

	public uint count;
	//public AttributeTable attributeModifer;
	// player specfic. move?
	public int order;

	public bool equipped;
	public ItemInstance(ItemInstance i) 
	{
		this.itemID = i.itemID;
		this.count = i.count;
		this.order = i.order;
		this.equipped = i.equipped;
	}

	public ItemInstance(string itemID, uint count, int order, bool equipped)
	{
		this.itemID = itemID;
		this.count = count;
		this.order = order;
		this.equipped = equipped;
	}

	// name modifer
	// durability
	// enchantment 
	// etc etc
	// 
	//
}
