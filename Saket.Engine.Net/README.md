Currently depent on LiteNetLib. Future proposal to make transport abstraction layer. TODO


Goals:

Garbage Free Loops
	


Serialization of structs and w

Automatic Shapshot Creation on server
	- Delta compression
	- 
	
Selective sending to clients
	- All clients should not receive entire snapshot but only data that is relevant


	
Floats should have a threshhold before to become dirty


Two snapshooting implmentations:

Snapshot_A: Is a simple implementation that doesn't contain any deltacompresseion and has a large size

Snapshot_B: is more bandwith optimized at the cost of serverside complexcity in terms of memory and cpu
