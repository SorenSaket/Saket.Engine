# Resource Management
When making an application or game theres often need to load resources for it to function. Other terms used to describe resources are content, assets, files etc. It's an umbrella term for individual pieces of data. Often used resources are Textures, Scripts, Shaders, Audio etc.

Often these resources come from different places like the local filesystem, a web based api or embedded in the executable. The resource manager is responsible for creating a single easy to use api for fetching resources.

There is no need for the user to cache or store resources individually.


## Quick start


|Term               | Explanation|
|-------------------|-----|
| Loader            | Function responsible to turn a binary stream into a managed object |
| Database          | Abstract interface that return a dictionary of strings and binary data. <ul><li>Embedded Database</li><li>Http Database</li><li>File Database</li></ul>|
| ResourceManager   | The class responsible for all resource management functionality. Single point of entry for a user. |


```csharp

	ResourceManager resources = new ResourceManager();
	// Embedded resources can be accessed this way
	resources.AddDatabase(new DatabaseEmbedded(Assembly.GetEntryAssembly()));

	// Add loader
	resources.AddLoader<Shader>((stream)=>{
		
	});

	// Load a given resource as type
	// Will remain loaded as long as a reference is stored to it
	Shader shader = resources.Load<Shader>();

	
	// A handle ensures that 
	ResourceHandle<Shader> handleShader = resources.Get<Shader>();


	// To unload explicitly

```

### Creating a custom loader
```csharp
public class LoaderSheet : ResourceLoader<Sheet>
{
	public override Sheet Load(string name, ResourceManager resourceManager)
	{
		string basepath = "sheet_" + name + ".json";
		
		if (resourceManager.TryGetStream(basepath, out Stream? stream))
		{
			StreamReader reader = new StreamReader(stream);
			var sheet = JsonConvert.DeserializeObject<Sheet>(reader.ReadToEnd());
			if (sheet != null)
			{
				return sheet;
			}        
		}

		throw new Exception("Failed to load sheet");
	}
}
```

### Unloading files explicitly



## Implementation details


Something similar to [Unitys Addressable Asset System](https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/index.html)

At startup the asset manager gets hold of all possible resources without loading them into memory.
- Resources can be multiple files or a single file
- A resource Loader is responsible for loading specific types.
- File Watching and hot reloading
- Multiple Resource Locations
    - Embedded
    - Filesystem
    - URL

All assets are automatically disposed when not it use?

Handles are provided to the user featuing lazy loading of assets when referenced.

internally the resourcemanager hold weakreferences to the objects causing them to be automatically collected by garbage disposal if no other references are held.

If a user holds a handle to a resource than is unloaded load it.



Todo:
- Creature and enforce a proper naming scheme.
- Figure out naming for the library. Assets/Resource
- Unloading / Streaming
	- We want resource to automatically be unloaded. But no create a senario where it's frequently cycled in and out of mem.
- finish all databases
- Testing and documentation
