using System;
using System.Runtime.InteropServices;

namespace Saket.Engine.WebGPU
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
		public char* vendorName;
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
		public uint binding;
		public IntPtr buffer;
		public ulong offset;
		public ulong size;
		public IntPtr sampler;
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
		public WGPUChainedStruct* nextInChain;
		public WGPUBufferBindingType type;
		[MarshalAs(UnmanagedType.I1)]
		public bool hasDynamicOffset;
		public ulong minBindingSize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBufferDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public WGPUBufferUsage usage;
		public ulong size;
		[MarshalAs(UnmanagedType.I1)]
		public bool mappedAtCreation;
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
		[MarshalAs(UnmanagedType.I1)]
		public bool alphaToCoverageEnabled;
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
		public uint bindGroupLayoutCount;
		public IntPtr* bindGroupLayouts;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUPrimitiveDepthClipControl
	{
		public WGPUChainedStruct chain;
		[MarshalAs(UnmanagedType.I1)]
		public bool unclippedDepth;
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
		public uint pipelineStatisticsCount;
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
		public uint colorFormatsCount;
		public WGPUTextureFormat* colorFormats;
		public WGPUTextureFormat depthStencilFormat;
		public uint sampleCount;
		[MarshalAs(UnmanagedType.I1)]
		public bool depthReadOnly;
		[MarshalAs(UnmanagedType.I1)]
		public bool stencilReadOnly;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassDepthStencilAttachment
	{
		public IntPtr view;
		public WGPULoadOp depthLoadOp;
		public WGPUStoreOp depthStoreOp;
		public float depthClearValue;
		[MarshalAs(UnmanagedType.I1)]
		public bool depthReadOnly;
		public WGPULoadOp stencilLoadOp;
		public WGPUStoreOp stencilStoreOp;
		public uint stencilClearValue;
		[MarshalAs(UnmanagedType.I1)]
		public bool stencilReadOnly;
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
		[MarshalAs(UnmanagedType.I1)]
		public bool forceFallbackAdapter;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSamplerBindingLayout
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUSamplerBindingType type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUSamplerDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public WGPUAddressMode addressModeU;
		public WGPUAddressMode addressModeV;
		public WGPUAddressMode addressModeW;
		public WGPUFilterMode magFilter;
		public WGPUFilterMode minFilter;
		public WGPUMipmapFilterMode mipmapFilter;
		public float lodMinClamp;
		public float lodMaxClamp;
		public WGPUCompareFunction compare;
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
		public WGPUTextureSampleType sampleType;
		public WGPUTextureViewDimension viewDimension;
		[MarshalAs(UnmanagedType.I1)]
		public bool multisampled;
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
		public WGPUTextureFormat format;
		public WGPUTextureViewDimension dimension;
		public uint baseMipLevel;
		public uint mipLevelCount;
		public uint baseArrayLayer;
		public uint arrayLayerCount;
		public WGPUTextureAspect aspect;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUVertexAttribute
	{
		public WGPUVertexFormat format;
		public ulong offset;
		public uint shaderLocation;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBindGroupDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public IntPtr layout;
		public uint entryCount;
		public WGPUBindGroupEntry* entries;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBindGroupLayoutEntry
	{
		public WGPUChainedStruct* nextInChain;
		public uint binding;
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
		public uint messageCount;
		public WGPUCompilationMessage* messages;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUComputePassDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public uint timestampWriteCount;
		public WGPUComputePassTimestampWrite* timestampWrites;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUDepthStencilState
	{
		public WGPUChainedStruct* nextInChain;
		public WGPUTextureFormat format;
		[MarshalAs(UnmanagedType.I1)]
		public bool depthWriteEnabled;
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
		public uint constantCount;
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
		public uint hintCount;
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
		public uint viewFormatCount;
		public WGPUTextureFormat* viewFormats;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUVertexBufferLayout
	{
		public ulong arrayStride;
		public WGPUVertexStepMode stepMode;
		public uint attributeCount;
		public WGPUVertexAttribute* attributes;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUBindGroupLayoutDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public uint entryCount;
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
		public uint requiredFeaturesCount;
		public WGPUFeatureName* requiredFeatures;
		public WGPURequiredLimits* requiredLimits;
		public WGPUQueueDescriptor defaultQueue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPURenderPassDescriptor
	{
		public WGPUChainedStruct* nextInChain;
		public char* label;
		public uint colorAttachmentCount;
		public WGPURenderPassColorAttachment* colorAttachments;
		public WGPURenderPassDepthStencilAttachment* depthStencilAttachment;
		public IntPtr occlusionQuerySet;
		public uint timestampWriteCount;
		public WGPURenderPassTimestampWrite* timestampWrites;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUVertexState
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public uint constantCount;
		public WGPUConstantEntry* constants;
		public uint bufferCount;
		public WGPUVertexBufferLayout* buffers;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct WGPUFragmentState
	{
		public WGPUChainedStruct* nextInChain;
		public IntPtr module;
		public char* entryPoint;
		public uint constantCount;
		public WGPUConstantEntry* constants;
		public uint targetCount;
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

