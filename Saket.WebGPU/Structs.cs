using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Saket.WebGPU
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ChainedStruct
    {
        public ChainedStruct* next;
        public SType sType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ChainedStructOut
    {
        public ChainedStructOut* next;
        public SType sType;
    }

    [StructLayout(LayoutKind.Sequential)]
	public unsafe struct AdapterProperties
	{
		public ChainedStructOut* nextInChain;
		public uint vendorID;
		public char*  vendorName;
		public char* architecture;
		public uint deviceID;
        public char* name;
		public char* driverDescription;
		public AdapterType adapterType;
		public BackendType backendType;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BindGroupEntry
	{
		public ChainedStruct* nextInChain;
        /// <summary>
        /// A number representing a unique identifier for this resource binding, which matches the binding value of a corresponding GPUBindGroupLayout entry. In addition, it matches the n index value of the corresponding @binding(n) attribute in the shader (GPUShaderModule) used in the related pipeline.
        /// </summary>
		public uint binding;

        /// <summary>
        /// The GPUBuffer to bind
        /// </summary>
		public IntPtr buffer;
        /// <summary>
        /// The offset, in bytes, from the beginning of buffer to the beginning of the range exposed to the shader by the buffer binding.
        /// </summary>
		public ulong offset;
        /// <summary>
        /// The size, in bytes, of the buffer binding. If not provided, specifies the range starting at offset and ending at the end of buffer.
        /// </summary>
		public ulong size;

        // Sampler Resource
		public IntPtr sampler;

        // Texture Resource
		public IntPtr textureView;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BlendComponent
	{
		public BlendOperation operation;
		public BlendFactor srcFactor;
		public BlendFactor dstFactor;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BufferBindingLayout
	{
		public ChainedStruct* nextInChain = null;
        /// <summary>
        /// Indicates the type required for buffers bound to this bindings.
        /// </summary>
		public BufferBindingType type = BufferBindingType.Uniform;
        /// <summary>
        /// Indicates whether this binding requires a dynamic offset.
        /// </summary>
		public WGPUBool hasDynamicOffset = false;
        /// <summary>
        /// Indicates the minimum size of a buffer binding used with this bind point.
        /// Bindings are always validated against this size in createBindGroup().
        /// If this is not 0, pipeline creation additionally validates that this value ≥ the minimum buffer binding size of the variable.
        /// If this is 0, it is ignored by pipeline creation, and instead draw/dispatch commands validate that each binding in the GPUBindGroup satisfies the minimum buffer binding size of the variable.
        /// </summary>
		public ulong minBindingSize = 0;

        public BufferBindingLayout()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BufferDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public BufferUsage usage;
        /// <summary>
        /// The size of the buffer in bytes.
        /// </summary>
		public ulong size;
        /// <summary>
        /// If true creates the buffer in an already mapped state, allowing getMappedRange() to be called immediately. It is valid to set mappedAtCreation to true even if usage does not contain MAP_READ or MAP_WRITE. This can be used to set the buffer�s initial data.  Guarantees that even if the buffer creation eventually fails, it will still appear as if the mapped range can be written/read to until it is unmapped.
        /// </summary>
        public WGPUBool mappedAtCreation;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Color
	{
		public double r;
		public double g;
		public double b;
		public double a;

        public Color(double r, double g, double b, double a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CommandBufferDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CommandEncoderDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CompilationMessage
	{
		public ChainedStruct* nextInChain;
		public char* message;
		public CompilationMessageType type;
		public ulong lineNum;
		public ulong linePos;
		public ulong offset;
		public ulong length;
		public ulong utf16LinePos;
		public ulong utf16Offset;
		public ulong utf16Length;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ComputePassTimestampWrite
	{
		public IntPtr querySet;
		public uint queryIndex;
		public ComputePassTimestampLocation location;
	}
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ConstantEntry
	{
		public ChainedStruct* nextInChain;
		public char* key;
		public double value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Extent3D
	{
		public uint width;
		public uint height;
		public uint depthOrArrayLayers;

        public Extent3D(uint width, uint height, uint depthOrArrayLayers)
        {
            this.width = width;
            this.height = height;
            this.depthOrArrayLayers = depthOrArrayLayers;
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct InstanceDescriptor
	{
		public ChainedStruct* nextInChain;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Limits
	{
		public uint maxTextureDimension1D;
		public uint maxTextureDimension2D;
		public uint maxTextureDimension3D;
		public uint maxTextureArrayLayers;
		public uint maxBindGroups;
		public uint maxBindingsPerBindGroup;
		public uint maxDynamicUniformBuffersPerPipelineLayout;
		public uint maxDynamicStorageBuffersPerPipelineLayout;
		public uint maxSampledTexturesPerShaderStage;
		public uint maxSamplersPerShaderStage;
		public uint maxStorageBuffersPerShaderStage;
		public uint maxStorageTexturesPerShaderStage;
		public uint maxUniformBuffersPerShaderStage;
		public ulong maxUniformBufferBindingSize;
		public ulong maxStorageBufferBindingSize;
		public uint minUniformBufferOffsetAlignment;
		public uint minStorageBufferOffsetAlignment;
		public uint maxVertexBuffers;
		public ulong maxBufferSize;
		public uint maxVertexAttributes;
		public uint maxVertexBufferArrayStride;
		public uint maxInterStageShaderComponents;
		public uint maxInterStageShaderVariables;
		public uint maxColorAttachments;
		public uint maxColorAttachmentBytesPerSample;
		public uint maxComputeWorkgroupStorageSize;
		public uint maxComputeInvocationsPerWorkgroup;
		public uint maxComputeWorkgroupSizeX;
		public uint maxComputeWorkgroupSizeY;
		public uint maxComputeWorkgroupSizeZ;
		public uint maxComputeWorkgroupsPerDimension;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MultisampleState
	{
		public ChainedStruct* nextInChain;
		public uint count;
		public uint mask;
		public WGPUBool alphaToCoverageEnabled;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Origin3D
	{
		public uint x;
		public uint y;
		public uint z;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PipelineLayoutDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t bindGroupLayoutCount;
		public IntPtr* bindGroupLayouts;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PrimitiveDepthClipControl
	{
		public ChainedStruct chain;
		public WGPUBool unclippedDepth;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PrimitiveState
	{
		public ChainedStruct* nextInChain;
		public PrimitiveTopology topology;
		public IndexFormat stripIndexFormat;
		public FrontFace frontFace;
		public CullMode cullMode;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct QuerySetDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public QueryType type;
		public uint count;
		public PipelineStatisticName* pipelineStatistics;
		public size_t pipelineStatisticsCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct QueueDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderBundleDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderBundleEncoderDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t colorFormatsCount;
		public TextureFormat* colorFormats;
		public TextureFormat depthStencilFormat;
		public uint sampleCount;
		public WGPUBool depthReadOnly;
		public WGPUBool stencilReadOnly;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderPassDepthStencilAttachment
	{
		public IntPtr view;
		public LoadOp depthLoadOp;
		public StoreOp depthStoreOp;
		public float depthClearValue;
		public WGPUBool depthReadOnly;
		public LoadOp stencilLoadOp;
		public StoreOp stencilStoreOp;
		public uint stencilClearValue;
		public WGPUBool stencilReadOnly;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderPassDescriptorMaxDrawCount
	{
		public ChainedStruct chain;
		public ulong maxDrawCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderPassTimestampWrite
	{
		public IntPtr querySet;
		public uint queryIndex;
		public RenderPassTimestampLocation location;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RequestAdapterOptions
	{
		public ChainedStruct* nextInChain;
		public IntPtr compatibleSurface;
		public PowerPreference powerPreference;
		public WGPUBool forceFallbackAdapter;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SamplerBindingLayout
	{
		public ChainedStruct* nextInChain;
        /// <summary>
        /// Indicates the required type of a sampler bound to this bindings.
        /// </summary>
		public SamplerBindingType type = SamplerBindingType.Filtering;

        public SamplerBindingLayout()
        {

        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SamplerDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;

        /// <summary>
        /// Specifies the address modes for the texture width, height, and depth coordinates, respectively.
        /// </summary>
		public AddressMode addressModeU = AddressMode.ClampToEdge;
        /// <summary>
        /// Specifies the address modes for the texture width, height, and depth coordinates, respectively.
        /// </summary>
        public AddressMode addressModeV = AddressMode.ClampToEdge;
        /// <summary>
        /// Specifies the address modes for the texture width, height, and depth coordinates, respectively.
        /// </summary>
        public AddressMode addressModeW = AddressMode.ClampToEdge;
        /// <summary>
        /// Specifies the sampling behavior when the sample footprint is smaller than or equal to one texel.
        /// </summary>
		public FilterMode magFilter = FilterMode.Nearest;
        /// <summary>
        /// Specifies the sampling behavior when the sample footprint is larger than one texel.
        /// </summary>
		public FilterMode minFilter = FilterMode.Nearest;
        /// <summary>
        /// Specifies behavior for sampling between mipmap levels.
        /// </summary>
		public MipmapFilterMode mipmapFilter = MipmapFilterMode.Nearest;
        /// <summary>
        /// Specifies the minimum levels of detail, respectively, used internally when sampling a texture.
        /// </summary>
        public float lodMinClamp = 0f;
        /// <summary>
        /// Specifies themaximum levels of detail, respectively, used internally when sampling a texture.
        /// </summary>
		public float lodMaxClamp = 32f;
        /// <summary>
        /// When provided the sampler will be a comparison sampler with the specified GPUCompareFunction.
        /// </summary>
        /// <remarks>
        /// Comparison samplers may use filtering, but the sampling results will be implementation-dependent and
        /// may differ from the normal filtering rules. 
        /// </remarks>
		public CompareFunction compare;
        /// <summary>
        /// Specifies the maximum anisotropy value clamp used by the sampler.
        /// </summary>
        /// <remarks>
        /// Most implementations support maxAnisotropy values in range between 1 and 16, inclusive. The used value of maxAnisotropy will be clamped to the maximum value that the platform supports.
        /// </remarks>
        public ushort maxAnisotropy = 1;

        public SamplerDescriptor()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ShaderModuleCompilationHint
	{
		public ChainedStruct* nextInChain;
		public char* entryPoint;
		public IntPtr layout;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ShaderModuleSPIRVDescriptor
	{
		public ChainedStruct chain;
		public uint codeSize;
		public uint* code;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ShaderModuleWGSLDescriptor
	{
		public ChainedStruct chain;
		public char* code;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct StencilFaceState
	{
		public CompareFunction compare;
		public StencilOperation failOp;
		public StencilOperation depthFailOp;
		public StencilOperation passOp;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct StorageTextureBindingLayout
	{
		public ChainedStruct* nextInChain;
		public StorageTextureAccess access;
		public TextureFormat format;
		public TextureViewDimension viewDimension;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromAndroidNativeWindow
	{
		public ChainedStruct chain;
		public void* window;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromCanvasHTMLSelector
	{
		public ChainedStruct chain;
		public char* selector;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromMetalLayer
	{
		public ChainedStruct chain;
		public void* layer;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromWaylandSurface
	{
		public ChainedStruct chain;
		public void* display;
		public void* surface;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromWindowsHWND
	{
		public ChainedStruct chain;
		public nint hinstance;
		public nint hwnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromXcbWindow
	{
		public ChainedStruct chain;
		public void* connection;
		public uint window;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SurfaceDescriptorFromXlibWindow
	{
		public ChainedStruct chain;
		public void* display;
		public uint window;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SwapChainDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public TextureUsage usage;
		public TextureFormat format;
		public uint width;
		public uint height;
		public PresentMode presentMode;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct TextureBindingLayout
	{
		public ChainedStruct* nextInChain;
        /// <summary>
        /// Indicates the type required for texture views bound to this binding.
        /// </summary>
		public TextureSampleType sampleType = TextureSampleType.Float;
        /// <summary>
        /// Indicates the required dimension for texture views bound to this binding.
        /// </summary>
		public TextureViewDimension viewDimension = TextureViewDimension._2D;
        /// <summary>
        /// Indicates whether or not texture views bound to this binding must be multisampled.
        /// </summary>
        public WGPUBool multisampled = false;

        public TextureBindingLayout()
        {
        }
    }
    /// <summary>
    /// A GPUImageDataLayout is a layout of images within some linear memory. It’s used when copying data between a texture and a GPUBuffer, or when scheduling a write into a texture from the GPUQueue.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
	public unsafe struct TextureDataLayout
    {
        public ChainedStruct* nextInChain;
        /// <summary>
        /// The offset, in bytes, from the beginning of the image data source (such as a GPUImageCopyBuffer.buffer) to the start of the image data within that source.
        /// </summary>
        public ulong offset;
        /// <summary>
        /// The stride, in bytes, between the beginning of each block row and the subsequent block row. Required if there are multiple block rows(i.e.the copy height or depth is more than one block).
        /// </summary>
		public uint bytesPerRow;
        /// <summary>
        /// Number of block rows per single image of the texture. rowsPerImage × bytesPerRow is the stride, in bytes,  between the beginning of each image of data and the subsequent image. Required if there are multiple images (i.e.the copy depth is more than one).
        /// </summary>
		public uint rowsPerImage;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct TextureViewDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
        /// <summary>
        /// The format of the texture view. Must be either the format of the texture or one of the viewFormats specified during its creation.
        /// </summary>
		public TextureFormat format;
        /// <summary>
        /// The dimension to view the texture as.
        /// </summary>
		public TextureViewDimension dimension;
        /// <summary>
        /// The first (most detailed) mipmap level accessible to the texture view.
        /// </summary>
		public uint baseMipLevel;
        /// <summary>
        /// How many mipmap levels, starting with baseMipLevel, are accessible to the texture view.
        /// </summary>
		public uint mipLevelCount;
        /// <summary>
        /// The index of the first array layer accessible to the texture view.
        /// </summary>
		public uint baseArrayLayer;
        /// <summary>
        /// How many array layers, starting with baseArrayLayer, are accessible to the texture view.
        /// </summary>
		public uint arrayLayerCount;
        /// <summary>
        /// Which aspect(s) of the texture are accessible to the texture view.
        /// </summary>
		public TextureAspect aspect;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct VertexAttribute
	{
        /// <summary>
        /// The <see cref="VertexFormat"/> of the attribute.
        /// </summary>
		public VertexFormat format;
        /// <summary>
        /// The offset, in bytes, from the beginning of the element to the data for the attribute.
        /// </summary>
		public ulong offset;
        /// <summary>
        /// The numeric location associated with this attribute, which will correspond with a "@location" attribute declared in the vertex.module.
        /// </summary>
		public uint shaderLocation;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BindGroupDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public IntPtr layout;
		public size_t entryCount;
		public BindGroupEntry* entries;
	}
    
    /// <summary>
    /// Describes a single shader resource binding to be included in a GPUBindGroupLayout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
	public unsafe struct BindGroupLayoutEntry
	{
		public ChainedStruct* nextInChain;
        /// <summary>
        /// A unique identifier for a resource binding within the GPUBindGroupLayout, corresponding to a GPUBindGroupEntry.binding and a @binding attribute in the GPUShaderModule.
        /// </summary>
        public uint binding;
        /// <summary>
        /// A bitset of the members of GPUShaderStage. Each set bit indicates that a GPUBindGroupLayoutEntry's resource will be accessible from the associated shader stage.
        /// </summary>
		public ShaderStage visibility;
		public BufferBindingLayout buffer;
		public SamplerBindingLayout sampler;
		public TextureBindingLayout texture;
		public StorageTextureBindingLayout storageTexture;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BlendState
	{
		public BlendComponent color;
		public BlendComponent alpha;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CompilationInfo
	{
		public ChainedStruct* nextInChain;
		public size_t messageCount;
		public CompilationMessage* messages;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ComputePassDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t timestampWriteCount;
		public ComputePassTimestampWrite* timestampWrites;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DepthStencilState
	{
		public ChainedStruct* nextInChain;
		public TextureFormat format;
		public WGPUBool depthWriteEnabled;
		public CompareFunction depthCompare;
		public StencilFaceState stencilFront;
		public StencilFaceState stencilBack;
		public uint stencilReadMask;
		public uint stencilWriteMask;
		public int depthBias;
		public float depthBiasSlopeScale;
		public float depthBiasClamp;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ImageCopyBuffer
	{
		public ChainedStruct* nextInChain;
		public TextureDataLayout layout;
        /// <summary>
        /// A buffer which either contains image data to be copied or will store the image data being copied, depending on the method it is being passed to.
        /// </summary>
		public IntPtr buffer;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ImageCopyTexture
	{
		public ChainedStruct* nextInChain;
        /// <summary>
        /// Texture to copy to/from.
        /// </summary>
		public IntPtr texture;
        /// <summary>
        /// Mip-map level of the texture to copy to/from.
        /// </summary>
        public uint mipLevel = 0;
        /// <summary>
        /// Defines the origin of the copy - the minimum corner of the texture sub-region to copy to/from. Together with `copySize`, defines the full copy sub-region.
        /// </summary>
        public Origin3D origin;
        /// <summary>
        /// Defines which aspects of the texture to copy to/from
        /// </summary>
		public TextureAspect aspect = TextureAspect.All;

        public ImageCopyTexture()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ProgrammableStageDescriptor
	{
		public ChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public size_t constantCount;
		public ConstantEntry* constants;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderPassColorAttachment
	{
        /// <summary>
        /// A GPUTextureView describing the texture subresource that will be output to for this color attachment
        /// </summary>
		public IntPtr view;
        /// <summary>
        /// A GPUTextureView describing the texture subresource that will receive the resolved output for this color attachment if view is multisampled.
        /// </summary>
		public IntPtr resolveTarget;
        /// <summary>
        /// Indicates the load operation to perform on view prior to executing the render pass.
        /// </summary>
        /// <remarks>It is recommended to prefer clearing; see "clear" for details.</remarks>
		public LoadOp loadOp;
        /// <summary>
        /// The store operation to perform on view after executing the render pass.
        /// </summary>
		public StoreOp storeOp;
        /// <summary>
        /// Indicates the value to clear view to prior to executing the render pass. If not provided, defaults to {r: 0, g: 0, b: 0, a: 0}. Ignored if loadOp is not "clear". The components of clearValue are all double values.They are converted matching the render attachment. If conversion fails, a validation error is generated.
        /// </summary>
		public Color clearValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RequiredLimits
	{
		public ChainedStruct* nextInChain;
		public Limits limits;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ShaderModuleDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t hintCount;
		public ShaderModuleCompilationHint* hints;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SupportedLimits
	{
		public ChainedStructOut* nextInChain;
		public Limits limits;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct TextureDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
        /// <summary>
        /// The allowed usages for the texture.
        /// </summary>
		public TextureUsage usage;
        /// <summary>
        /// Whether the texture is one-dimensional, an array of two-dimensional layers, or three-dimensional.
        /// </summary>
		public TextureDimension dimension = TextureDimension._2D;
        /// <summary>
        /// The width, height, and depth or layer count of the texture.
        /// </summary>
		public Extent3D size;
        /// <summary>
        /// The format of the texture.
        /// </summary>
		public TextureFormat format;
        /// <summary>
        /// The number of mip levels the texture will contain.
        /// </summary>
		public uint mipLevelCount = 1;
        /// <summary>
        /// The sample count of the texture. A sampleCount > 1 indicates a multisampled texture.
        /// </summary>
		public uint sampleCount = 1;

		public size_t viewFormatCount;
        /// <summary>
        /// Specifies what view format values will be allowed when calling createView() on this texture (in addition to the texture’s actual format).
        /// </summary>
        /// <remarks>
        ///  Adding a format to this list may have a significant performance impact, so it is best to avoid adding formats unnecessarily.
        ///  The actual performance impact is highly dependent on the target system; developers must test various systems to find out the impact on their particular application.For example, on some systems any texture with a format or viewFormats entry including "rgba8unorm-srgb" will perform less optimally than a "rgba8unorm" texture
        ///  which does not. Similar caveats exist for other formats and pairs of formats on other systems.
        /// </remarks>
        public TextureFormat* viewFormats;

        public TextureDescriptor()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct VertexBufferLayout
	{
        /// <summary>
        /// The stride, in bytes, between elements of this array.
        /// </summary>
		public ulong arrayStride;
        /// <summary>
        /// Whether each element of this array represents per-vertex data or per-instance data
        /// </summary>
        public VertexStepMode stepMode;

		public size_t attributeCount;
        /// <summary>
        /// An array defining the layout of the vertex attributes within each element.
        /// </summary>
		public VertexAttribute* attributes;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BindGroupLayoutDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t entryCount;
		public BindGroupLayoutEntry* entries;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ColorTargetState
	{
		public ChainedStruct* nextInChain;
		public TextureFormat format;
		public BlendState* blend;
		public ColorWriteMask writeMask;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ComputePipelineDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public IntPtr layout;
		public ProgrammableStageDescriptor compute;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DeviceDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t requiredFeaturesCount;
		public FeatureName* requiredFeatures;
		public RequiredLimits* requiredLimits;
		public QueueDescriptor defaultQueue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderPassDescriptor
	{
		public ChainedStruct* nextInChain;
		public char* label;
		public size_t colorAttachmentCount;
		public RenderPassColorAttachment* colorAttachments;
		public RenderPassDepthStencilAttachment* depthStencilAttachment;
		public IntPtr occlusionQuerySet;
		public size_t timestampWriteCount;
		public RenderPassTimestampWrite* timestampWrites;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct VertexState
	{
		public ChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public size_t constantCount;
		public ConstantEntry* constants;
		public size_t bufferCount;
		public VertexBufferLayout* buffers;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct FragmentState
	{
		public ChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public size_t constantCount;
		public ConstantEntry* constants;
		public size_t targetCount;
		public ColorTargetState* targets;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct RenderPipelineDescriptor
	{
		public ChainedStruct* nextInChain;
        public char* label;
		public IntPtr layout;
		public VertexState vertex;
		public PrimitiveState primitive;
		public DepthStencilState* depthStencil;
		public MultisampleState multisample;
		public FragmentState* fragment;
	}
}