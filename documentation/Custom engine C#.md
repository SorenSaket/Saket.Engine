Application Engine/Framework.

Provides
Non platform specific Windowing API
A 3D Accelerated API access
A GUI framework for displaying GUI




Featuring ECS system


Priorities:

Must Haves:
Windows Export
UI Framework
networking



Nice to haves
Mac/Linux Export
Single file Executable

Long Term Goals:
 - No External Dependencies:
	- Custom GLFW and OpenGL/Vulkan Bindings
 - or Use Portable graphics library like Veldrid, WGPU, MACH-DAWN etc.




### Entity Component System system
[[ecs]]



## Asset Management
Something similar to [Unitys Addressable Asset System](https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/index.html)

At startup the asset manager gets hold of all possible resources without loading them into memory.
- Resources can be multiple files or a single file
- A resource Loader is responsible for  loading specific types.
- File Watching and hot reloading
- Multiple Resource Locations
	- Embedded
	- Filesystem
	- URL

All assets are automatically disposed when not it use?


Handles are provided to the user featuing lazy loading of assets when referenced.

internally the resourcemanager hold weakreferences to the objects causing them to be automatically collected by garbage disposal if no other references are held.

If a user holds a handle to a resource than is unloaded load it.

|Term | Explanation|
|-----|-----|
| Loader | Function responsible to turn a binary stream into a managed object |
| Database | Abstract interface that return a dictionary of strings and binary data. <ul><li>Embedded Database</li><li>Http Database</li><li>File Database</li></ul>|
| ResourceManager | Single static point of entry for user. |


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


## Multiplayer Networking

Cost free Transport Abstraction Layer

Enet/Ruffels/LitenetLib Networking


Client-side prediction: Actions happen immediately on the client.

Server reconciliation: Predictions get acknowledged or corrected by the server

Dead reckoning: 

Interpolation: 

Lag Compensation: Server rolls back and applies input
- When you shoot, client sends this event to the server with full information: the exact timestamp of your shot, and the exact aim of the weapon.
- Here’s the crucial step. Since the server gets all the input with timestamps, it can authoritatively reconstruct the world at any instant in the past. In particular, it can reconstruct the world exactly as it looked like to any client at any point in time.
- This means the server can know exactly what was on your weapon’s sights the instant you shot. It was the past position of your enemy’s head, but the server knows it was the position of their head in your present.
- The server processes the shot at that point in time, and updates the clients.

### Summary

- Server gets inputs from all the clients, with timestamps
- Server processes inputs and updates world status
- Server sends regular world snapshots to all clients
- Client sends input and simulates their effects locally
- Client get world updates and
	- Syncs predicted state to authoritative state
	- Interpolates known past states for other entities

From a player’s point of view, this has two important consequences:
- Player sees himself in the present
- Player sees other entities in the past


### asd






### Rollback library
![alt text](./test.drawio.png)


#### Clientside

Client Time dialation based on packet loss

Clientside:
buffer of own input

buffer of old inputs for all players
Misprediction checking
roll back. 


#### Serverside 

1 frame input buffer



