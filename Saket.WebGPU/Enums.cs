using System;

namespace Saket.WebGPU
{
    public enum AdapterType : uint
    {
        DiscreteGPU = 0,
        IntegratedGPU = 1,
        CPU = 2,
        Unknown = 3
    }

    public enum AddressMode : uint
    {
        Repeat = 0,
        MirrorRepeat = 1,
        ClampToEdge = 2
    }

    public enum BackendType : uint
    {
        Null = 0,
        WebGPU = 1,
        D3D11 = 2,
        D3D12 = 3,
        Metal = 4,
        Vulkan = 5,
        OpenGL = 6,
        OpenGLES = 7
    }

    public enum BlendFactor : uint
    {
        Zero = 0,
        One = 1,
        Src = 2,
        OneMinusSrc = 3,
        SrcAlpha = 4,
        OneMinusSrcAlpha = 5,
        Dst = 6,
        OneMinusDst = 7,
        DstAlpha = 8,
        OneMinusDstAlpha = 9,
        SrcAlphaSaturated = 10,
        Constant = 11,
        OneMinusConstant = 12
    }

    public enum BlendOperation : uint
    {
        Add = 0,
        Subtract = 1,
        ReverseSubtract = 2,
        Min = 3,
        Max = 4
    }

    public enum BufferBindingType : uint
    {
        Undefined = 0,
        Uniform = 1,
        Storage = 2,
        ReadOnlyStorage = 3
    }

    public enum BufferMapAsyncStatus : uint
    {
        Success = 0,
        Error = 1,
        Unknown = 2,
        DeviceLost = 3,
        DestroyedBeforeCallback = 4,
        UnmappedBeforeCallback = 5
    }

    public enum BufferMapState : uint
    {
        Unmapped = 0,
        Pending = 1,
        Mapped = 2
    }

    public enum CompareFunction : uint
    {
        Undefined = 0,
        Never = 1,
        Less = 2,
        LessEqual = 3,
        Greater = 4,
        GreaterEqual = 5,
        Equal = 6,
        NotEqual = 7,
        Always = 8
    }

    public enum CompilationInfoRequestStatus : uint
    {
        Success = 0,
        Error = 1,
        DeviceLost = 2,
        Unknown = 3
    }

    public enum CompilationMessageType : uint
    {
        Error = 0,
        Warning = 1,
        Info = 2
    }

    public enum ComputePassTimestampLocation : uint
    {
        Beginning = 0,
        End = 1
    }

    public enum CreatePipelineAsyncStatus : uint
    {
        Success = 0,
        ValidationError = 1,
        InternalError = 2,
        DeviceLost = 3,
        DeviceDestroyed = 4,
        Unknown = 5
    }

    public enum CullMode : uint
    {
        None = 0,
        Front = 1,
        Back = 2
    }

    public enum DeviceLostReason : uint
    {
        Undefined = 0,
        Destroyed = 1
    }

    public enum ErrorFilter : uint
    {
        Validation = 0,
        OutOfMemory = 1,
        Internal = 2
    }

    public enum ErrorType : uint
    {
        NoError = 0,
        Validation = 1,
        OutOfMemory = 2,
        Internal = 3,
        Unknown = 4,
        DeviceLost = 5
    }

    public enum FeatureName : uint
    {
        Undefined = 0,
        DepthClipControl = 1,
        Depth32FloatStencil8 = 2,
        TimestampQuery = 3,
        PipelineStatisticsQuery = 4,
        TextureCompressionBC = 5,
        TextureCompressionETC2 = 6,
        TextureCompressionASTC = 7,
        IndirectFirstInstance = 8,
        ShaderF16 = 9,
        RG11B10UfloatRenderable = 10,
        BGRA8UnormStorage = 11
    }

    public enum FilterMode : uint
    {
        Nearest = 0,
        Linear = 1
    }

    public enum FrontFace : uint
    {
        CCW = 0,
        CW = 1
    }

    public enum IndexFormat : uint
    {
        Undefined = 0,
        Uint16 = 1,
        Uint32 = 2
    }

    public enum LoadOp : uint
    {
        Undefined = 0,
        Clear = 1,
        Load = 2
    }

    public enum MipmapFilterMode : uint
    {
        Nearest = 0,
        Linear = 1
    }

    public enum PipelineStatisticName : uint
    {
        VertexShaderInvocations = 0,
        ClipperInvocations = 1,
        ClipperPrimitivesOut = 2,
        FragmentShaderInvocations = 3,
        ComputeShaderInvocations = 4
    }
    /// <summary>
    /// An enumerated value that can be used to provide a hint to the user agent indicating what class of adapter should be chosen from the system's available adapters.
    /// </summary>
    public enum PowerPreference : uint
    {
        /// <summary>
        /// undefined (or not specified), which provides no hint.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// "low-power", which provides a hint to prioritize power savings over performance. If your app runs OK with this setting, it is recommended to use it, as it can significantly improve battery life on portable devices. This is usually the default if no options are provided.
        /// </summary>
        LowPower = 1,
        /// <summary>
        /// "high-performance", which provides a hint to prioritize performance over power consumption. You are encouraged to only specify this value if absolutely necessary, since it may significantly decrease battery life on portable devices. It may also result in increased GPUDevice loss — the system will sometimes elect to switch to a lower-power adapter to save power.
        /// </summary>
        HighPerformance = 2
    }

    public enum PresentMode : uint
    {
        Immediate = 0,
        Mailbox = 1,
        Fifo = 2
    }

    public enum PrimitiveTopology : uint
    {
        PointList = 0,
        LineList = 1,
        LineStrip = 2,
        TriangleList = 3,
        TriangleStrip = 4
    }

    public enum QueryType : uint
    {
        Occlusion = 0,
        PipelineStatistics = 1,
        Timestamp = 2
    }

    public enum QueueWorkDoneStatus : uint
    {
        Success = 0,
        Error = 1,
        Unknown = 2,
        DeviceLost = 3
    }

    public enum RenderPassTimestampLocation : uint
    {
        Beginning = 0,
        End = 1
    }

    public enum RequestAdapterStatus : uint
    {
        Success = 0,
        Unavailable = 1,
        Error = 2,
        Unknown = 3
    }

    public enum RequestDeviceStatus : uint
    {
        Success = 0,
        Error = 1,
        Unknown = 2
    }

    public enum SType : uint
    {
        Invalid = 0,
        SurfaceDescriptorFromMetalLayer = 1,
        SurfaceDescriptorFromWindowsHWND = 2,
        SurfaceDescriptorFromXlibWindow = 3,
        SurfaceDescriptorFromCanvasHTMLSelector = 4,
        ShaderModuleSPIRVDescriptor = 5,
        ShaderModuleWGSLDescriptor = 6,
        PrimitiveDepthClipControl = 7,
        SurfaceDescriptorFromWaylandSurface = 8,
        SurfaceDescriptorFromAndroidNativeWindow = 9,
        SurfaceDescriptorFromXcbWindow = 10,
        RenderPassDescriptorMaxDrawCount = 15
    }

    public enum SamplerBindingType : uint
    {
        Undefined = 0,
        Filtering = 1,
        NonFiltering = 2,
        Comparison = 3
    }

    public enum StencilOperation : uint
    {
        Keep = 0,
        Zero = 1,
        Replace = 2,
        Invert = 3,
        IncrementClamp = 4,
        DecrementClamp = 5,
        IncrementWrap = 6,
        DecrementWrap = 7
    }

    public enum StorageTextureAccess : uint
    {
        Undefined = 0,
        WriteOnly = 1
    }

    public enum StoreOp : uint
    {
        Undefined = 0,
        Store = 1,
        Discard = 2
    }

    public enum TextureAspect : uint
    {
        All = 0,
        StencilOnly = 1,
        DepthOnly = 2
    }

    public enum TextureComponentType : uint
    {
        Float = 0,
        Sint = 1,
        Uint = 2,
        DepthComparison = 3
    }

    public enum TextureDimension : uint
    {
        _1D = 0,
        _2D = 1,
        _3D = 2
    }

    public enum TextureFormat : uint
    {
        Undefined = 0,
        R8Unorm = 1,
        R8Snorm = 2,
        R8Uint = 3,
        R8Sint = 4,
        R16Uint = 5,
        R16Sint = 6,
        R16Float = 7,
        RG8Unorm = 8,
        RG8Snorm = 9,
        RG8Uint = 10,
        RG8Sint = 11,
        R32Float = 12,
        R32Uint = 13,
        R32Sint = 14,
        RG16Uint = 15,
        RG16Sint = 16,
        RG16Float = 17,
        RGBA8Unorm = 18,
        RGBA8UnormSrgb = 19,
        RGBA8Snorm = 20,
        RGBA8Uint = 21,
        RGBA8Sint = 22,
        BGRA8Unorm = 23,
        BGRA8UnormSrgb = 24,
        RGB10A2Unorm = 25,
        RG11B10Ufloat = 26,
        RGB9E5Ufloat = 27,
        RG32Float = 28,
        RG32Uint = 29,
        RG32Sint = 30,
        RGBA16Uint = 31,
        RGBA16Sint = 32,
        RGBA16Float = 33,
        RGBA32Float = 34,
        RGBA32Uint = 35,
        RGBA32Sint = 36,
        Stencil8 = 37,
        Depth16Unorm = 38,
        Depth24Plus = 39,
        Depth24PlusStencil8 = 40,
        Depth32Float = 41,
        Depth32FloatStencil8 = 42,
        BC1RGBAUnorm = 43,
        BC1RGBAUnormSrgb = 44,
        BC2RGBAUnorm = 45,
        BC2RGBAUnormSrgb = 46,
        BC3RGBAUnorm = 47,
        BC3RGBAUnormSrgb = 48,
        BC4RUnorm = 49,
        BC4RSnorm = 50,
        BC5RGUnorm = 51,
        BC5RGSnorm = 52,
        BC6HRGBUfloat = 53,
        BC6HRGBFloat = 54,
        BC7RGBAUnorm = 55,
        BC7RGBAUnormSrgb = 56,
        ETC2RGB8Unorm = 57,
        ETC2RGB8UnormSrgb = 58,
        ETC2RGB8A1Unorm = 59,
        ETC2RGB8A1UnormSrgb = 60,
        ETC2RGBA8Unorm = 61,
        ETC2RGBA8UnormSrgb = 62,
        EACR11Unorm = 63,
        EACR11Snorm = 64,
        EACRG11Unorm = 65,
        EACRG11Snorm = 66,
        ASTC4x4Unorm = 67,
        ASTC4x4UnormSrgb = 68,
        ASTC5x4Unorm = 69,
        ASTC5x4UnormSrgb = 70,
        ASTC5x5Unorm = 71,
        ASTC5x5UnormSrgb = 72,
        ASTC6x5Unorm = 73,
        ASTC6x5UnormSrgb = 74,
        ASTC6x6Unorm = 75,
        ASTC6x6UnormSrgb = 76,
        ASTC8x5Unorm = 77,
        ASTC8x5UnormSrgb = 78,
        ASTC8x6Unorm = 79,
        ASTC8x6UnormSrgb = 80,
        ASTC8x8Unorm = 81,
        ASTC8x8UnormSrgb = 82,
        ASTC10x5Unorm = 83,
        ASTC10x5UnormSrgb = 84,
        ASTC10x6Unorm = 85,
        ASTC10x6UnormSrgb = 86,
        ASTC10x8Unorm = 87,
        ASTC10x8UnormSrgb = 88,
        ASTC10x10Unorm = 89,
        ASTC10x10UnormSrgb = 90,
        ASTC12x10Unorm = 91,
        ASTC12x10UnormSrgb = 92,
        ASTC12x12Unorm = 93,
        ASTC12x12UnormSrgb = 94
    }

    public enum TextureSampleType : uint
    {
        Undefined = 0,
        Float = 1,
        UnfilterableFloat = 2,
        Depth = 3,
        Sint = 4,
        Uint = 5
    }

    public enum TextureViewDimension : uint
    {
        Undefined = 0,
		_1D = 1,
        _2D = 2,
        _2DArray = 3,
        _Cube = 4,
        _CubeArray = 5,
        _3D = 6
    }

    public enum VertexFormat : uint
    {
        Undefined = 0,
        Uint8x2 = 1,
        Uint8x4 = 2,
        Sint8x2 = 3,
        Sint8x4 = 4,
        Unorm8x2 = 5,
        Unorm8x4 = 6,
        Snorm8x2 = 7,
        Snorm8x4 = 8,
        Uint16x2 = 9,
        Uint16x4 = 10,
        Sint16x2 = 11,
        Sint16x4 = 12,
        Unorm16x2 = 13,
        Unorm16x4 = 14,
        Snorm16x2 = 15,
        Snorm16x4 = 16,
        Float16x2 = 17,
        Float16x4 = 18,
        Float32 = 19,
        Float32x2 = 20,
        Float32x3 = 21,
        Float32x4 = 22,
        Uint32 = 23,
        Uint32x2 = 24,
        Uint32x3 = 25,
        Uint32x4 = 26,
        Sint32 = 27,
        Sint32x2 = 28,
        Sint32x3 = 29,
        Sint32x4 = 30
    }

    /// <summary>
    /// The step mode configures how an address for vertex buffer data is computed, based on the current vertex or instance index
    /// </summary>
    public enum VertexStepMode : uint
    {
        /// <summary>
        /// The address is advanced by arrayStride for each vertex, and reset between instances.
        /// </summary>
        Vertex = 0,
        /// <summary>
        /// The address is advanced by arrayStride for each instance.
        /// </summary>
        Instance = 1,
        VertexBufferNotUsed = 2
    }
    
    /// <summary>
    /// The GPUBufferUsage flags determine how a GPUBuffer may be used after its creatio
    /// </summary>
    [Flags]
    public enum BufferUsage : uint
    {
        None = 0,
        /// <summary>
        /// The buffer can be mapped for reading. (Example: calling mapAsync() with GPUMapMode.READ) May only be combined with COPY_DST.
        /// </summary>
        MapRead = 1,
        /// <summary>
        /// The buffer can be mapped for writing. (Example: calling mapAsync() with GPUMapMode.WRITE). May only be combined with COPY_SRC.
        /// </summary>
        MapWrite = 2,
        /// <summary>
        /// The buffer can be used as the source of a copy operation. (Examples: as the source argument of a copyBufferToBuffer() or copyBufferToTexture() call.)
        /// </summary>
        CopySrc = 4,
        /// <summary>
        /// The buffer can be used as the destination of a copy or write operation. (Examples: as the destination argument of a copyBufferToBuffer() or copyTextureToBuffer() call, or as the target of a writeBuffer() call.)
        /// </summary>
        CopyDst = 8,
        /// <summary>
        /// The buffer can be used as an index buffer. (Example: passed to setIndexBuffer().)
        /// </summary>
        Index = 16,
        /// <summary>
        /// The buffer can be used as a vertex buffer. (Example: passed to setVertexBuffer().)
        /// </summary>
        Vertex = 32,
        /// <summary>
        /// The buffer can be used as a uniform buffer. (Example: as a bind group entry for a GPUBufferBindingLayout with a buffer.type of "uniform".)
        /// </summary>
        Uniform = 64,
        /// <summary>
        /// The buffer can be used as a storage buffer. (Example: as a bind group entry for a GPUBufferBindingLayout with a buffer.type of "storage" or "read-only-storage".)
        /// </summary>
        Storage = 128,
        /// <summary>
        /// The buffer can be used as to store indirect command arguments. (Examples: as the indirectBuffer argument of a drawIndirect() or dispatchWorkgroupsIndirect() call.)
        /// </summary>
        Indirect = 256,
        /// <summary>
        /// The buffer can be used to capture query results. (Example: as the destination argument of a resolveQuerySet() call.)
        /// </summary>
        QueryResolve = 512,
    }

    [Flags]
    public enum ColorWriteMask : uint
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
        Alpha = 8,
        All = 15
    }

    /// <summary>
    /// The GPUMapMode flags determine how a GPUBuffer is mapped when calling mapAsync()
    /// </summary>
    [Flags]
    public enum MapMode : uint
    {
        None = 0,
        /// <summary>
        /// Only valid with buffers created with the MAP_READ usage. Once the buffer is mapped, calls to getMappedRange() will return an ArrayBuffer containing the buffer’s current values. Changes to the returned ArrayBuffer will be discarded after unmap() is called.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Only valid with buffers created with the MAP_WRITE usage. Once the buffer is mapped, calls to getMappedRange() will return an ArrayBuffer containing the buffer’s current values. Changes to the returned ArrayBuffer will be stored in the GPUBuffer after unmap() is called.
        /// </summary>
        /// <remarks>
        /// Since the MAP_WRITE buffer usage may only be combined with the COPY_SRC buffer usage, mapping for writing can never return values produced by the GPU, and the returned ArrayBuffer will only ever contain the default initialized data (zeros) or data written by the webpage during a previous mapping.
        /// </remarks>
        Write = 2
    }

    /// <summary>
    /// GPUShaderStage contains the following flags, which describe which shader stages a corresponding GPUBindGroupEntry for this GPUBindGroupLayoutEntry will be visible to:
    /// </summary>
    [Flags]
    public enum ShaderStage : uint
    {
        None = 0,
        /// <summary>
        /// The bind group entry will be accessible to vertex shaders.
        /// </summary>
        Vertex = 1,
        /// <summary>
        /// The bind group entry will be accessible to fragment shaders.
        /// </summary>
        Fragment = 2,
        /// <summary>
        /// The bind group entry will be accessible to compute shaders.
        /// </summary>
        Compute = 4
    }

    /// <summary>
    /// The GPUTextureUsage flags determine how a GPUTexture may be used after its creation
    /// </summary>
    [Flags]
    public enum TextureUsage : uint
    {
        None = 0,
        /// <summary>
        /// The texture can be used as the source of a copy operation. (Examples: as the source argument of a copyTextureToTexture() or copyTextureToBuffer() call.
        /// </summary>
        CopySrc = 1,
        /// <summary>
        /// The texture can be used as the destination of a copy or write operation. (Examples: as the destination argument of a copyTextureToTexture() or copyBufferToTexture() call, or as the target of a writeTexture() call.)
        /// </summary>
        CopyDst = 2,
        /// <summary>
        /// The texture can be bound for use as a sampled texture in a shader (Example: as a bind group entry for a GPUTextureBindingLayout.)
        /// </summary>
        TextureBinding = 4,
        /// <summary>
        /// The texture can be bound for use as a storage texture in a shader (Example: as a bind group entry for a GPUStorageTextureBindingLayout.)
        /// </summary>
        StorageBinding = 8,
        /// <summary>
        /// The texture can be used as a color or depth/stencil attachment in a render pass. (Example: as a GPURenderPassColorAttachment.view or GPURenderPassDepthStencilAttachment.view.)
        /// </summary>
        RenderAttachment = 16
    }

}
