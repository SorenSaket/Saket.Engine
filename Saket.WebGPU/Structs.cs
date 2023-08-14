using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;

namespace Saket.WebGPU
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WGPUChainedStruct
    {
        public WGPUChainedStruct* next;
        public WGPUSType sType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WGPUChainedStructOut
    {
        public WGPUChainedStructOut* next;
        public WGPUSType sType;
    }

    [StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUAdapterProperties
	{
		public WGPUChainedStructOut* nextInChain;
		public uint vendorID;
		public char*  vendorName;
		public char* architecture;
		public uint deviceID;
        public char* name;
		public char* driverDescription;
		public WGPUAdapterType adapterType;
		public WGPUBackendType backendType;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBindGroupEntry
	{
		public WGPUChainedStruct* nextInChain;
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
	public unsafe struct WGPUBlendComponent
	{
		public WGPUBlendOperation operation;
		public WGPUBlendFactor srcFactor;
		public WGPUBlendFactor dstFactor;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBufferBindingLayout
	{
		public WGPUChainedStruct* nextInChain = null;
        /// <summary>
        /// Indicates the type required for buffers bound to this bindings.
        /// </summary>
		public WGPUBufferBindingType type = WGPUBufferBindingType.Uniform;
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

        public WGPUBufferBindingLayout()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBufferDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public WGPUBufferUsage usage;
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
	public unsafe struct WGPUColor
	{
		public double r;
		public double g;
		public double b;
		public double a;

        public WGPUColor(double r, double g, double b, double a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUCommandBufferDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUCommandEncoderDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUCompilationMessage
	{
		public WGPUChainedStruct* nextInChain;
		public char* message;
		public WGPUCompilationMessageType type;
		public ulong lineNum;
		public ulong linePos;
		public ulong offset;
		public ulong length;
		public ulong utf16LinePos;
		public ulong utf16Offset;
		public ulong utf16Length;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUComputePassTimestampWrite
	{
		public IntPtr querySet;
		public uint queryIndex;
		public WGPUComputePassTimestampLocation location;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUConstantEntry
	{
		public WGPUChainedStruct* nextInChain;
		public char* key;
		public double value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUExtent3D
	{
		public uint width;
		public uint height;
		public uint depthOrArrayLayers;

        public WGPUExtent3D(uint width, uint height, uint depthOrArrayLayers)
        {
            this.width = width;
            this.height = height;
            this.depthOrArrayLayers = depthOrArrayLayers;
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUInstanceDescriptor
	{
		public WGPUChainedStruct* nextInChain;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPULimits
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
	public unsafe struct WGPUMultisampleState
	{
		public WGPUChainedStruct* nextInChain;
		public uint count;
		public uint mask;
		public WGPUBool alphaToCoverageEnabled;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUOrigin3D
	{
		public uint x;
		public uint y;
		public uint z;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUPipelineLayoutDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t bindGroupLayoutCount;
		public IntPtr* bindGroupLayouts;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUPrimitiveDepthClipControl
	{
		public WGPUChainedStruct chain;
		public WGPUBool unclippedDepth;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUPrimitiveState
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUPrimitiveTopology topology;
		public WGPUIndexFormat stripIndexFormat;
		public WGPUFrontFace frontFace;
		public WGPUCullMode cullMode;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUQuerySetDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public WGPUQueryType type;
		public uint count;
		public WGPUPipelineStatisticName* pipelineStatistics;
		public size_t pipelineStatisticsCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUQueueDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderBundleDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderBundleEncoderDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t colorFormatsCount;
		public WGPUTextureFormat* colorFormats;
		public WGPUTextureFormat depthStencilFormat;
		public uint sampleCount;
		public WGPUBool depthReadOnly;
		public WGPUBool stencilReadOnly;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassDepthStencilAttachment
	{
		public IntPtr view;
		public WGPULoadOp depthLoadOp;
		public WGPUStoreOp depthStoreOp;
		public float depthClearValue;
		public WGPUBool depthReadOnly;
		public WGPULoadOp stencilLoadOp;
		public WGPUStoreOp stencilStoreOp;
		public uint stencilClearValue;
		public WGPUBool stencilReadOnly;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassDescriptorMaxDrawCount
	{
		public WGPUChainedStruct chain;
		public ulong maxDrawCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassTimestampWrite
	{
		public IntPtr querySet;
		public uint queryIndex;
		public WGPURenderPassTimestampLocation location;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURequestAdapterOptions
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr compatibleSurface;
		public WGPUPowerPreference powerPreference;
		public WGPUBool forceFallbackAdapter;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSamplerBindingLayout
	{
		public WGPUChainedStruct* nextInChain;
        /// <summary>
        /// Indicates the required type of a sampler bound to this bindings.
        /// </summary>
		public WGPUSamplerBindingType type = WGPUSamplerBindingType.Filtering;

        public WGPUSamplerBindingLayout()
        {

        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSamplerDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;

        /// <summary>
        /// Specifies the address modes for the texture width, height, and depth coordinates, respectively.
        /// </summary>
		public WGPUAddressMode addressModeU;
        /// <summary>
        /// Specifies the address modes for the texture width, height, and depth coordinates, respectively.
        /// </summary>
        public WGPUAddressMode addressModeV;
        /// <summary>
        /// Specifies the address modes for the texture width, height, and depth coordinates, respectively.
        /// </summary>
        public WGPUAddressMode addressModeW;
        /// <summary>
        /// Specifies the sampling behavior when the sample footprint is smaller than or equal to one texel.
        /// </summary>
		public WGPUFilterMode magFilter;
        /// <summary>
        /// Specifies the sampling behavior when the sample footprint is larger than one texel.
        /// </summary>
		public WGPUFilterMode minFilter;
        /// <summary>
        /// Specifies behavior for sampling between mipmap levels.
        /// </summary>
		public WGPUMipmapFilterMode mipmapFilter;
        /// <summary>
        /// Specifies the minimum levels of detail, respectively, used internally when sampling a texture.
        /// </summary>
		public float lodMinClamp;
        /// <summary>
        /// Specifies themaximum levels of detail, respectively, used internally when sampling a texture.
        /// </summary>
		public float lodMaxClamp;
        /// <summary>
        /// When provided the sampler will be a comparison sampler with the specified GPUCompareFunction.
        /// </summary>
        /// <remarks>
        /// Comparison samplers may use filtering, but the sampling results will be implementation-dependent and
        /// may differ from the normal filtering rules. 
        /// </remarks>
		public WGPUCompareFunction compare;
        /// <summary>
        /// Specifies the maximum anisotropy value clamp used by the sampler.
        /// </summary>
        /// <remarks>
        /// Most implementations support maxAnisotropy values in range between 1 and 16, inclusive. The used value of maxAnisotropy will be clamped to the maximum value that the platform supports.
        /// </remarks>
        public ushort maxAnisotropy;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUShaderModuleCompilationHint
	{
		public WGPUChainedStruct* nextInChain;
		public char* entryPoint;
		public IntPtr layout;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUShaderModuleSPIRVDescriptor
	{
		public WGPUChainedStruct chain;
		public uint codeSize;
		public uint* code;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUShaderModuleWGSLDescriptor
	{
		public WGPUChainedStruct chain;
		public char* code;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUStencilFaceState
	{
		public WGPUCompareFunction compare;
		public WGPUStencilOperation failOp;
		public WGPUStencilOperation depthFailOp;
		public WGPUStencilOperation passOp;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUStorageTextureBindingLayout
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUStorageTextureAccess access;
		public WGPUTextureFormat format;
		public WGPUTextureViewDimension viewDimension;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromAndroidNativeWindow
	{
		public WGPUChainedStruct chain;
		public void* window;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromCanvasHTMLSelector
	{
		public WGPUChainedStruct chain;
		public char* selector;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromMetalLayer
	{
		public WGPUChainedStruct chain;
		public void* layer;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromWaylandSurface
	{
		public WGPUChainedStruct chain;
		public void* display;
		public void* surface;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromWindowsHWND
	{
		public WGPUChainedStruct chain;
		public nint hinstance;
		public nint hwnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromXcbWindow
	{
		public WGPUChainedStruct chain;
		public void* connection;
		public uint window;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSurfaceDescriptorFromXlibWindow
	{
		public WGPUChainedStruct chain;
		public void* display;
		public uint window;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSwapChainDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public WGPUTextureUsage usage;
		public WGPUTextureFormat format;
		public uint width;
		public uint height;
		public WGPUPresentMode presentMode;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUTextureBindingLayout
	{
		public WGPUChainedStruct* nextInChain;
        /// <summary>
        /// Indicates the type required for texture views bound to this binding.
        /// </summary>
		public WGPUTextureSampleType sampleType = WGPUTextureSampleType.Float;
        /// <summary>
        /// Indicates the required dimension for texture views bound to this binding.
        /// </summary>
		public WGPUTextureViewDimension viewDimension = WGPUTextureViewDimension._2D;
        /// <summary>
        /// Indicates whether or not texture views bound to this binding must be multisampled.
        /// </summary>
        public WGPUBool multisampled = false;

        public WGPUTextureBindingLayout()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUTextureDataLayout
	{
		public WGPUChainedStruct* nextInChain;
		public ulong offset;
		public uint bytesPerRow;
		public uint rowsPerImage;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUTextureViewDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
        /// <summary>
        /// The format of the texture view. Must be either the format of the texture or one of the viewFormats specified during its creation.
        /// </summary>
		public WGPUTextureFormat format;
        /// <summary>
        /// The dimension to view the texture as.
        /// </summary>
		public WGPUTextureViewDimension dimension;
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
		public WGPUTextureAspect aspect;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUVertexAttribute
	{
        /// <summary>
        /// The <see cref="WGPUVertexFormat"/> of the attribute.
        /// </summary>
		public WGPUVertexFormat format;
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
	public unsafe struct WGPUBindGroupDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public IntPtr layout;
		public size_t entryCount;
		public WGPUBindGroupEntry* entries;
	}
    
    /// <summary>
    /// Describes a single shader resource binding to be included in a GPUBindGroupLayout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBindGroupLayoutEntry
	{
		public WGPUChainedStruct* nextInChain;
        /// <summary>
        /// A unique identifier for a resource binding within the GPUBindGroupLayout, corresponding to a GPUBindGroupEntry.binding and a @binding attribute in the GPUShaderModule.
        /// </summary>
        public uint binding;
        /// <summary>
        /// A bitset of the members of GPUShaderStage. Each set bit indicates that a GPUBindGroupLayoutEntry's resource will be accessible from the associated shader stage.
        /// </summary>
		public WGPUShaderStage visibility;
		public WGPUBufferBindingLayout buffer;
		public WGPUSamplerBindingLayout sampler;
		public WGPUTextureBindingLayout texture;
		public WGPUStorageTextureBindingLayout storageTexture;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBlendState
	{
		public WGPUBlendComponent color;
		public WGPUBlendComponent alpha;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUCompilationInfo
	{
		public WGPUChainedStruct* nextInChain;
		public size_t messageCount;
		public WGPUCompilationMessage* messages;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUComputePassDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t timestampWriteCount;
		public WGPUComputePassTimestampWrite* timestampWrites;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUDepthStencilState
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUTextureFormat format;
		public WGPUBool depthWriteEnabled;
		public WGPUCompareFunction depthCompare;
		public WGPUStencilFaceState stencilFront;
		public WGPUStencilFaceState stencilBack;
		public uint stencilReadMask;
		public uint stencilWriteMask;
		public int depthBias;
		public float depthBiasSlopeScale;
		public float depthBiasClamp;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUImageCopyBuffer
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUTextureDataLayout layout;
		public IntPtr buffer;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUImageCopyTexture
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr texture;
		public uint mipLevel;
		public WGPUOrigin3D origin;
		public WGPUTextureAspect aspect;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUProgrammableStageDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public size_t constantCount;
		public WGPUConstantEntry* constants;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassColorAttachment
	{
		public IntPtr view;
		public IntPtr resolveTarget;
		public WGPULoadOp loadOp;
		public WGPUStoreOp storeOp;
		public WGPUColor clearValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURequiredLimits
	{
		public WGPUChainedStruct* nextInChain;
		public WGPULimits limits;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUShaderModuleDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t hintCount;
		public WGPUShaderModuleCompilationHint* hints;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSupportedLimits
	{
		public WGPUChainedStructOut* nextInChain;
		public WGPULimits limits;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUTextureDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public WGPUTextureUsage usage;
		public WGPUTextureDimension dimension;
		public WGPUExtent3D size;
		public WGPUTextureFormat format;
		public uint mipLevelCount;
		public uint sampleCount;
		public size_t viewFormatCount;
		public WGPUTextureFormat* viewFormats;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUVertexBufferLayout
	{
        /// <summary>
        /// The stride, in bytes, between elements of this array.
        /// </summary>
		public ulong arrayStride;
        /// <summary>
        /// Whether each element of this array represents per-vertex data or per-instance data
        /// </summary>
        public WGPUVertexStepMode stepMode;

		public size_t attributeCount;
        /// <summary>
        /// An array defining the layout of the vertex attributes within each element.
        /// </summary>
		public WGPUVertexAttribute* attributes;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBindGroupLayoutDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t entryCount;
		public WGPUBindGroupLayoutEntry* entries;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUColorTargetState
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUTextureFormat format;
		public WGPUBlendState* blend;
		public WGPUColorWriteMask writeMask;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUComputePipelineDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public IntPtr layout;
		public WGPUProgrammableStageDescriptor compute;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUDeviceDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t requiredFeaturesCount;
		public WGPUFeatureName* requiredFeatures;
		public WGPURequiredLimits* requiredLimits;
		public WGPUQueueDescriptor defaultQueue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public size_t colorAttachmentCount;
		public WGPURenderPassColorAttachment* colorAttachments;
		public WGPURenderPassDepthStencilAttachment* depthStencilAttachment;
		public IntPtr occlusionQuerySet;
		public size_t timestampWriteCount;
		public WGPURenderPassTimestampWrite* timestampWrites;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUVertexState
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public size_t constantCount;
		public WGPUConstantEntry* constants;
		public size_t bufferCount;
		public WGPUVertexBufferLayout* buffers;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUFragmentState
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public size_t constantCount;
		public WGPUConstantEntry* constants;
		public size_t targetCount;
		public WGPUColorTargetState* targets;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPipelineDescriptor
	{
		public WGPUChainedStruct* nextInChain;
        public char* label;
		public IntPtr layout;
		public WGPUVertexState vertex;
		public WGPUPrimitiveState primitive;
		public WGPUDepthStencilState* depthStencil;
		public WGPUMultisampleState multisample;
		public WGPUFragmentState* fragment;
	}
}