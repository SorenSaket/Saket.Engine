﻿

```csharp

// Imagining a platforming api
using Saket.Engine.WebGPU;
using Saket.Engine.Platform;

var eventloop = (e) =>{

}

// platform is static and all the functions change depending on compile target
// 
var window = Platform.CreateWindow(eventloop);


WGPUInstanceDescriptor desc = new WGPUInstanceDescriptor() { };
nint instance = wgpu.CreateInstance(ref desc);

if (instance == 0)
    throw new Exception("Could not initialize WebGPU");

window.CreateWGPUSurface(instance);


...
Initialize wgpu


```


















https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke

https://learn.microsoft.com/en-us/cpp/dotnet/performance-considerations-for-interop-cpp?view=msvc-170




https://www.chriswirz.com/software/cs-pinvoke-alternative-plugin-architecture




https://github.com/microsoft/win32metadata

https://github.com/dotnet/pinvoke

https://github.com/microsoft/CsWin32




https://learn.microsoft.com/en-us/windows/win32/apiindex/windows-api-list

https://learn.microsoft.com/en-us/dotnet/framework/interop/default-marshalling-for-strings