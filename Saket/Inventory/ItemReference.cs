namespace Saket.Inventory;


[System.Serializable]
public struct ItemReference 
{
	public string itemID;
	public uint count;

	public bool Valid()
	{
		if (string.IsNullOrWhiteSpace(itemID))
			return false;
		return true;
	}

	public ItemReference(string itemID)
	{
		this.itemID = itemID;
		this.count = 1;
	}

	public ItemReference(string itemID, uint count)
	{
		this.itemID = itemID;
		this.count = count;
	}
}