namespace Saket.Inventory;

public class Item 
{
	public string ID { get; set; }
    public string DisplayName { get; set; }
	public string Description { get; set; }
	public bool Stackable { get; set; }
	public uint MaxStackSize { get; set; }
	public bool Craftable
	{
		get
		{
			return (Recipe.items != null);
		}
	}
	public ItemRecipe Recipe { get; set; }
}