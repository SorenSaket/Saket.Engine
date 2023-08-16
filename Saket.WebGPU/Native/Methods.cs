using System.Runtime.InteropServices;

namespace Saket.WebGPU.Native
{   
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Compiler", "CS8981")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "supress to match native api more closely")]
    public static unsafe partial class wgpu

    {
        private const string LibraryName = "wgpu_native.dll";


        [LibraryImport(LibraryName, EntryPoint = "wgpuCreateInstance")]
        public static partial IntPtr CreateInstance(in WGPUInstanceDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuGetProcAddress")]
        public static partial WGPUProc GetProcAddress(IntPtr device, char* procName);


        #region Adapter

        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterEnumerateFeatures")]
        public static partial size_t AdapterEnumerateFeatures(IntPtr adapter, WGPUFeatureName* features);

        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterGetLimits")]
        public static partial bool AdapterGetLimits(IntPtr adapter, ref WGPUSupportedLimits limits);
       
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The intention behind this method is to allow developers to request specific details about the user's GPU so that they can preemptively apply workarounds for GPU-specific bugs, or provide different codepaths to better suit different GPU architectures. Providing such information does present a security risk — it could be used for fingerprinting — therefore the information shared is to be kept at a minimum, and different browser vendors are likely to share different information types and granularities.
        ///     </remarks>
        /// <param name="adapter"></param>
        /// <param name="properties"></param>
        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterGetProperties")]
        public static partial void AdapterGetProperties(IntPtr adapter, ref WGPUAdapterProperties properties);


        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterHasFeature")]
        public static partial bool AdapterHasFeature(IntPtr adapter, WGPUFeatureName feature);
        
        /// <summary>
        /// The requestDevice() method of the GPUAdapter interface returns a Promise that fulfills with a GPUDevice object, which is the primary interface for communicating with the GPU.
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="descriptor"></param>
        /// <param name="callback"></param>
        /// <param name="userdata"></param>
        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterRequestDevice")]
        public static partial void AdapterRequestDevice(IntPtr adapter, in WGPUDeviceDescriptor descriptor, WGPURequestDeviceCallback callback, void* userdata);

        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterReference")]
        public static partial bool AdapterReference(IntPtr adapter);

        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuAdapterRelease")]
        public static partial bool AdapterRelease(IntPtr adapter);

        #endregion

        #region Bindgroup

        [LibraryImport(LibraryName, EntryPoint = "wgpuBindGroupSetLabel")]
        public static partial void BindGroupSetLabel(IntPtr bindGroup, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBindGroupReference")]
        public static partial void BindGroupReference(IntPtr bindGroup);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBindGroupRelease")]
        public static partial void BindGroupRelease(IntPtr bindGroup);

        #endregion

        #region BindGroupLayout

        [LibraryImport(LibraryName, EntryPoint = "wgpuBindGroupLayoutSetLabel")]
        public static partial void BindGroupLayoutSetLabel(IntPtr bindGroupLayout, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBindGroupLayoutReference")]
        public static partial void BindGroupLayoutReference(IntPtr bindGroupLayout);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBindGroupLayoutRelease")]
        public static partial void BindGroupLayoutRelease(IntPtr bindGroupLayout);

        #endregion

        #region Buffer

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferDestroy")]
        public static partial void BufferDestroy(IntPtr buffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferGetConstMappedRange")]
        public static partial void* BufferGetConstMappedRange(IntPtr buffer, size_t offset, size_t size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferGetMapState")]
        public static partial WGPUBufferMapState BufferGetMapState(IntPtr buffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferGetMappedRange")]
        public static partial void* BufferGetMappedRange(IntPtr buffer, size_t offset, size_t size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferGetSize")]
        public static partial ulong BufferGetSize(IntPtr buffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferGetUsage")]
        public static partial WGPUBufferUsage BufferGetUsage(IntPtr buffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferMapAsync")]
        public static partial void BufferMapAsync(IntPtr buffer, WGPUMapMode mode, size_t offset, size_t size, WGPUBufferMapCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferSetLabel")]
        public static partial void BufferSetLabel(IntPtr buffer, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferUnmap")]
        public static partial void BufferUnmap(IntPtr buffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferReference")]
        public static partial void BufferReference(IntPtr buffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuBufferRelease")]
        public static partial void BufferRelease(IntPtr buffer);

        #endregion

        #region CommandBuffer

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandBufferSetLabel")]
        public static partial void CommandBufferSetLabel(IntPtr commandBuffer, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandBufferReference")]
        public static partial void CommandBufferReference(IntPtr commandBuffer);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandBufferRelease")]
        public static partial void CommandBufferRelease(IntPtr commandBuffer);

        #endregion

        #region CommandEncoder

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderBeginComputePass")]
        public static partial IntPtr CommandEncoderBeginComputePass(IntPtr commandEncoder, in WGPUComputePassDescriptor descriptor);
        /// <summary>
        /// Starts encoding a render pass, returning a GPURenderPassEncoder that can be used to control rendering.
        /// </summary>
        /// <param name="commandEncoder"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderBeginRenderPass")]
        public static partial IntPtr CommandEncoderBeginRenderPass(IntPtr commandEncoder, in WGPURenderPassDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderClearBuffer")]
        public static partial void CommandEncoderClearBuffer(IntPtr commandEncoder, IntPtr buffer, ulong offset, ulong size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderCopyBufferToBuffer")]
        public static partial void CommandEncoderCopyBufferToBuffer(IntPtr commandEncoder, IntPtr source, ulong sourceOffset, IntPtr destination, ulong destinationOffset, ulong size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderCopyBufferToTexture")]
        public static partial void CommandEncoderCopyBufferToTexture(IntPtr commandEncoder, WGPUImageCopyBuffer* source, WGPUImageCopyTexture* destination, WGPUExtent3D* copySize);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderCopyTextureToBuffer")]
        public static partial void CommandEncoderCopyTextureToBuffer(IntPtr commandEncoder, WGPUImageCopyTexture* source, WGPUImageCopyBuffer* destination, WGPUExtent3D* copySize);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderCopyTextureToTexture")]
        public static partial void CommandEncoderCopyTextureToTexture(IntPtr commandEncoder, WGPUImageCopyTexture* source, WGPUImageCopyTexture* destination, WGPUExtent3D* copySize);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderFinish")]
        public static partial IntPtr CommandEncoderFinish(IntPtr commandEncoder, in WGPUCommandBufferDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderInsertDebugMarker")]
        public static partial void CommandEncoderInsertDebugMarker(IntPtr commandEncoder, char* markerLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderPopDebugGroup")]
        public static partial void CommandEncoderPopDebugGroup(IntPtr commandEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderPushDebugGroup")]
        public static partial void CommandEncoderPushDebugGroup(IntPtr commandEncoder, char* groupLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderResolveQuerySet")]
        public static partial void CommandEncoderResolveQuerySet(IntPtr commandEncoder, IntPtr querySet, uint firstQuery, uint queryCount, IntPtr destination, ulong destinationOffset);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderSetLabel")]
        public static partial void CommandEncoderSetLabel(IntPtr commandEncoder, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderWriteTimestamp")]
        public static partial void CommandEncoderWriteTimestamp(IntPtr commandEncoder, IntPtr querySet, uint queryIndex);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderReference")]
        public static partial void CommandEncoderReference(IntPtr commandEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuCommandEncoderRelease")]
        public static partial void CommandEncoderRelease(IntPtr commandEncoder);

        #endregion

        #region ComputePassEncoder

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderBeginPipelineStatisticsQuery")]
        public static partial void ComputePassEncoderBeginPipelineStatisticsQuery(IntPtr computePassEncoder, IntPtr querySet, uint queryIndex);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderDispatchWorkgroups")]
        public static partial void ComputePassEncoderDispatchWorkgroups(IntPtr computePassEncoder, uint workgroupCountX, uint workgroupCountY, uint workgroupCountZ);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderDispatchWorkgroupsIndirect")]
        public static partial void ComputePassEncoderDispatchWorkgroupsIndirect(IntPtr computePassEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderEnd")]
        public static partial void ComputePassEncoderEnd(IntPtr computePassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderEndPipelineStatisticsQuery")]
        public static partial void ComputePassEncoderEndPipelineStatisticsQuery(IntPtr computePassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderInsertDebugMarker")]
        public static partial void ComputePassEncoderInsertDebugMarker(IntPtr computePassEncoder, char* markerLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderPopDebugGroup")]
        public static partial void ComputePassEncoderPopDebugGroup(IntPtr computePassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderPushDebugGroup")]
        public static partial void ComputePassEncoderPushDebugGroup(IntPtr computePassEncoder, char* groupLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderSetBindGroup")]
        public static partial void ComputePassEncoderSetBindGroup(IntPtr computePassEncoder, uint groupIndex, IntPtr group, size_t dynamicOffsetCount, uint* dynamicOffsets);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderSetLabel")]
        public static partial void ComputePassEncoderSetLabel(IntPtr computePassEncoder, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderSetPipeline")]
        public static partial void ComputePassEncoderSetPipeline(IntPtr computePassEncoder, IntPtr pipeline);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderReference")]
        public static partial void ComputePassEncoderReference(IntPtr computePassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePassEncoderRelease")]
        public static partial void ComputePassEncoderRelease(IntPtr computePassEncoder);

        #endregion

        #region ComputePipeline

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePipelineGetBindGroupLayout")]
        public static partial IntPtr ComputePipelineGetBindGroupLayout(IntPtr computePipeline, uint groupIndex);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePipelineSetLabel")]
        public static partial void ComputePipelineSetLabel(IntPtr computePipeline, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePipelineReference")]
        public static partial void ComputePipelineReference(IntPtr computePipeline);

        [LibraryImport(LibraryName, EntryPoint = "wgpuComputePipelineRelease")]
        public static partial void ComputePipelineRelease(IntPtr computePipeline);

        #endregion

        #region Device

        /// <summary>
        /// Creates a GPUBindGroup based on a GPUBindGroupLayout that defines a set of resources to be bound together in a group and how those resources are used in shader stages.
        /// </summary>
        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateBindGroup")]
        public static partial IntPtr DeviceCreateBindGroup(IntPtr device, in WGPUBindGroupDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateBindGroupLayout")]
        public static partial IntPtr DeviceCreateBindGroupLayout(IntPtr device, in WGPUBindGroupLayoutDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateBuffer")]
        public static partial IntPtr DeviceCreateBuffer(IntPtr device, in WGPUBufferDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateCommandEncoder")]
        public static partial IntPtr DeviceCreateCommandEncoder(IntPtr device, in WGPUCommandEncoderDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateComputePipeline")]
        public static partial IntPtr DeviceCreateComputePipeline(IntPtr device, in WGPUComputePipelineDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateComputePipelineAsync")]
        public static partial void DeviceCreateComputePipelineAsync(IntPtr device, in WGPUComputePipelineDescriptor descriptor, WGPUCreateComputePipelineAsyncCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreatePipelineLayout")]
        public static partial IntPtr DeviceCreatePipelineLayout(IntPtr device, in WGPUPipelineLayoutDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateQuerySet")]
        public static partial IntPtr DeviceCreateQuerySet(IntPtr device, in WGPUQuerySetDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateRenderBundleEncoder")]
        public static partial IntPtr DeviceCreateRenderBundleEncoder(IntPtr device, in WGPURenderBundleEncoderDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateRenderPipeline")]
        public static partial IntPtr DeviceCreateRenderPipeline(IntPtr device, in WGPURenderPipelineDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateRenderPipelineAsync")]
        public static partial void DeviceCreateRenderPipelineAsync(IntPtr device, in WGPURenderPipelineDescriptor descriptor, WGPUCreateRenderPipelineAsyncCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateSampler")]
        public static partial IntPtr DeviceCreateSampler(IntPtr device, in WGPUSamplerDescriptor  descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateShaderModule")]
        public static partial IntPtr DeviceCreateShaderModule(IntPtr device, in WGPUShaderModuleDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateSwapChain")]
        public static partial IntPtr DeviceCreateSwapChain(IntPtr device, IntPtr surface, in WGPUSwapChainDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceCreateTexture")]
        public static partial IntPtr DeviceCreateTexture(IntPtr device, in WGPUTextureDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceDestroy")]
        public static partial void DeviceDestroy(IntPtr device);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceEnumerateFeatures")]
        public static partial size_t DeviceEnumerateFeatures(IntPtr device, WGPUFeatureName* features);
        
        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceGetLimits")]
        public static partial bool DeviceGetLimits(IntPtr device, ref WGPUSupportedLimits limits);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceGetQueue")]
        public static partial IntPtr DeviceGetQueue(IntPtr device);
        
        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceHasFeature")]
        public static partial bool DeviceHasFeature(IntPtr device, WGPUFeatureName feature);
        
        [return: MarshalAs(UnmanagedType.I1)]
        [LibraryImport(LibraryName, EntryPoint = "wgpuDevicePopErrorScope")]
        public static partial bool DevicePopErrorScope(IntPtr device, WGPUErrorCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDevicePushErrorScope")]
        public static partial void DevicePushErrorScope(IntPtr device, WGPUErrorFilter filter);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceSetDeviceLostCallback")]
        public static partial void DeviceSetDeviceLostCallback(IntPtr device, WGPUDeviceLostCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceSetLabel")]
        public static partial void DeviceSetLabel(IntPtr device, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceSetUncapturedErrorCallback")]
        public static partial void DeviceSetUncapturedErrorCallback(IntPtr device, WGPUErrorCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceReference")]
        public static partial IntPtr DeviceReference(IntPtr device);

        [LibraryImport(LibraryName, EntryPoint = "wgpuDeviceRelease")]
        public static partial IntPtr DeviceRelease(IntPtr device);

        #endregion

        #region Instance

        [LibraryImport(LibraryName, EntryPoint = "wgpuInstanceCreateSurface")]
        public static partial IntPtr InstanceCreateSurface(IntPtr instance, in WGPUSurfaceDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuInstanceProcessEvents")]
        public static partial void InstanceProcessEvents(IntPtr instance);

        /// <summary>
        /// Returns a Promise that fulfills with a GPUAdapter object instance. From this you can request a GPUDevice, which is the primary interface for using WebGPU functionality.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="options"></param>
        /// <param name="callback"></param>
        /// <param name="userdata"></param>
        [LibraryImport(LibraryName, EntryPoint = "wgpuInstanceRequestAdapter")]
        public static partial void InstanceRequestAdapter(IntPtr instance, in WGPURequestAdapterOptions options, WGPURequestAdapterCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuInstanceReference")]
        public static partial void InstanceReference(IntPtr instance);

        [LibraryImport(LibraryName, EntryPoint = "wgpuInstanceRelease")]
        public static partial void InstanceRelease(IntPtr instance);

        #endregion

        #region PipelineLayout

        [LibraryImport(LibraryName, EntryPoint = "wgpuPipelineLayoutSetLabel")]
        public static partial void PipelineLayoutSetLabel(IntPtr pipelineLayout);

        [LibraryImport(LibraryName, EntryPoint = "wgpuPipelineLayoutReference")]
        public static partial void PipelineLayoutReference(IntPtr pipelineLayout);

        [LibraryImport(LibraryName, EntryPoint = "wgpuPipelineLayoutRelease")]
        public static partial void PipelineLayoutRelease(IntPtr pipelineLayout);

        #endregion

        #region QuerySet

        [LibraryImport(LibraryName, EntryPoint = "wgpuQuerySetDestroy")]
        public static partial void QuerySetDestroy(IntPtr querySet);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQuerySetGetCount")]
        public static partial uint QuerySetGetCount(IntPtr querySet);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQuerySetGetType")]
        public static partial WGPUQueryType QuerySetGetType(IntPtr querySet);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQuerySetSetLabel")]
        public static partial void QuerySetSetLabel(IntPtr querySet, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQuerySetReference")]
        public static partial uint QuerySetReference(IntPtr querySet);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQuerySetRelease")]
        public static partial uint QuerySetRelease(IntPtr querySet);

        #endregion

        #region Queue

        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueOnSubmittedWorkDone")]
        public static partial void QueueOnSubmittedWorkDone(IntPtr queue, WGPUQueueWorkDoneCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueSetLabel")]
        public static partial void QueueSetLabel(IntPtr queue, char* label);
        
        /// <summary>
        /// Schedules the execution of the command buffers by the GPU on this queue.  Submitted command buffers cannot be used again.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="commandCount"></param>
        /// <param name="commands"></param>
        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueSubmit")]
        public static partial void QueueSubmit(IntPtr queue, size_t commandCount, IntPtr commands);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueWriteBuffer")]
        public static partial void QueueWriteBuffer(IntPtr queue, IntPtr buffer, ulong bufferOffset, void* data, size_t size);

        /// <summary>
        /// Schedule a write of some data into a texture.
        /// </summary>
        /// <remarks>
        /// This method is intended to have low performance costs. As such, the write is not immediately submitted, and instead enqueued internally to happen at the start of the next submit() call. However, data will be immediately copied into staging memory; so the caller may discard it any time after this call completes. This method fails if size overruns the size of texture, or if data is too short. 
        /// </remarks>
        /// <param name="queue"></param>
        /// <param name="destination">specifies the texture to write into, and the location within the texture (coordinate offset, mip level) that will be overwritten.</param>
        /// <param name="data">the texels to be written, which must be in the same format as the texture.</param>
        /// <param name="dataSize"></param>
        /// <param name="dataLayout">describes the memory layout of data, which does not necessarily have to have tightly packed rows.</param>
        /// <param name="writeSize">is the size, in texels, of the region to be written.</param>
        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueWriteTexture")]
        public static partial void QueueWriteTexture(IntPtr queue, in WGPUImageCopyTexture destination, void* data, size_t dataSize, in WGPUTextureDataLayout dataLayout, in WGPUExtent3D writeSize);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueReference")]
        public static partial void QueueReference(IntPtr queue);

        [LibraryImport(LibraryName, EntryPoint = "wgpuQueueRelease")]
        public static partial void QueueRelease(IntPtr queue);

        #endregion

        #region RenderBundle

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleSetLabel")]
        public static partial void RenderBundleSetLabel(IntPtr renderBundle, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleReference")]
        public static partial void RenderBundleReference(IntPtr renderBundle);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleRelease")]
        public static partial void RenderBundleRelease(IntPtr renderBundle);

        #endregion

        #region RenderBundleEncoder

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderDraw")]
        public static partial void RenderBundleEncoderDraw(IntPtr renderBundleEncoder, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderDrawIndexed")]
        public static partial void RenderBundleEncoderDrawIndexed(IntPtr renderBundleEncoder, uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderDrawIndexedIndirect")]
        public static partial void RenderBundleEncoderDrawIndexedIndirect(IntPtr renderBundleEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderDrawIndirect")]
        public static partial void RenderBundleEncoderDrawIndirect(IntPtr renderBundleEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderFinish")]
        public static partial IntPtr RenderBundleEncoderFinish(IntPtr renderBundleEncoder, WGPURenderBundleDescriptor* descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderInsertDebugMarker")]
        public static partial void RenderBundleEncoderInsertDebugMarker(IntPtr renderBundleEncoder, char* markerLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderPopDebugGroup")]
        public static partial void RenderBundleEncoderPopDebugGroup(IntPtr renderBundleEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderPushDebugGroup")]
        public static partial void RenderBundleEncoderPushDebugGroup(IntPtr renderBundleEncoder, char* groupLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderSetBindGroup")]
        public static partial void RenderBundleEncoderSetBindGroup(IntPtr renderBundleEncoder, uint groupIndex, IntPtr group, size_t dynamicOffsetCount, uint* dynamicOffsets);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderSetIndexBuffer")]
        public static partial void RenderBundleEncoderSetIndexBuffer(IntPtr renderBundleEncoder, IntPtr buffer, WGPUIndexFormat format, ulong offset, ulong size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderSetLabel")]
        public static partial void RenderBundleEncoderSetLabel(IntPtr renderBundleEncoder, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderSetPipeline")]
        public static partial void RenderBundleEncoderSetPipeline(IntPtr renderBundleEncoder, IntPtr pipeline);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderSetVertexBuffer")]
        public static partial void RenderBundleEncoderSetVertexBuffer(IntPtr renderBundleEncoder, uint slot, IntPtr buffer, ulong offset, ulong size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderReference")]
        public static partial void RenderBundleEncoderReference(IntPtr renderBundleEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderBundleEncoderRelease")]
        public static partial void RenderBundleEncoderRelease(IntPtr renderBundleEncoder);

        #endregion

        #region RenderPassEncoder

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderBeginOcclusionQuery")]
        public static partial void RenderPassEncoderBeginOcclusionQuery(IntPtr renderPassEncoder, uint queryIndex);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderBeginPipelineStatisticsQuery")]
        public static partial void RenderPassEncoderBeginPipelineStatisticsQuery(IntPtr renderPassEncoder, IntPtr querySet, uint queryIndex);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderDraw")]
        public static partial void RenderPassEncoderDraw(IntPtr renderPassEncoder, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderDrawIndexed")]
        public static partial void RenderPassEncoderDrawIndexed(IntPtr renderPassEncoder, uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderDrawIndexedIndirect")]
        public static partial void RenderPassEncoderDrawIndexedIndirect(IntPtr renderPassEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderDrawIndirect")]
        public static partial void RenderPassEncoderDrawIndirect(IntPtr renderPassEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderEnd")]
        public static partial void RenderPassEncoderEnd(IntPtr renderPassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderEndOcclusionQuery")]
        public static partial void RenderPassEncoderEndOcclusionQuery(IntPtr renderPassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderEndPipelineStatisticsQuery")]
        public static partial void RenderPassEncoderEndPipelineStatisticsQuery(IntPtr renderPassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderExecuteBundles")]
        public static partial void RenderPassEncoderExecuteBundles(IntPtr renderPassEncoder, size_t bundleCount, IntPtr* bundles);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderInsertDebugMarker")]
        public static partial void RenderPassEncoderInsertDebugMarker(IntPtr renderPassEncoder, char* markerLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderPopDebugGroup")]
        public static partial void RenderPassEncoderPopDebugGroup(IntPtr renderPassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderPushDebugGroup")]
        public static partial void RenderPassEncoderPushDebugGroup(IntPtr renderPassEncoder, char* groupLabel);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetBindGroup")]
        public static partial void RenderPassEncoderSetBindGroup(IntPtr renderPassEncoder, uint groupIndex, IntPtr group, size_t dynamicOffsetCount, uint* dynamicOffsets);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetBlendConstant")]
        public static partial void RenderPassEncoderSetBlendConstant(IntPtr renderPassEncoder, WGPUColor* color);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetIndexBuffer")]
        public static partial void RenderPassEncoderSetIndexBuffer(IntPtr renderPassEncoder, IntPtr buffer, WGPUIndexFormat format, ulong offset, ulong size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetLabel")]
        public static partial void RenderPassEncoderSetLabel(IntPtr renderPassEncoder, char* label);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetPipeline")]
        public static partial void RenderPassEncoderSetPipeline(IntPtr renderPassEncoder, IntPtr pipeline);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetScissorRect")]
        public static partial void RenderPassEncoderSetScissorRect(IntPtr renderPassEncoder, uint x, uint y, uint width, uint height);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetStencilReference")]
        public static partial void RenderPassEncoderSetStencilReference(IntPtr renderPassEncoder, uint reference);


        /// <summary>
        /// The setVertexBuffer() method of the GPURenderPassEncoder interface sets or unsets the current GPUBuffer for the given slot that will provide vertex data for subsequent drawing commands.
        /// </summary>
        /// <param name="renderPassEncoder"></param>
        /// <param name="slot">A number referencing the vertex buffer slot to set the vertex buffer for.</param>
        /// <param name="buffer">A GPUBuffer representing the buffer containing the vertex data to use for subsequent drawing commands, or null, in which case any previously-set buffer in the given slot is unset.</param>
        /// <param name="offset">A number representing the offset, in bytes, into buffer where the vertex data begins. If omitted, offset defaults to 0.</param>
        /// <param name="size">A number representing the size, in bytes, of the vertex data contained in buffer. If omitted, size defaults to the buffer's GPUBuffer.size - offset.</param>
        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetVertexBuffer")]
        public static partial void RenderPassEncoderSetVertexBuffer(IntPtr renderPassEncoder, uint slot, IntPtr buffer, ulong offset, ulong size);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderSetViewport")]
        public static partial void RenderPassEncoderSetViewport(IntPtr renderPassEncoder, float x, float y, float width, float height, float minDepth, float maxDepth);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderReference")]
        public static partial void RenderPassEncoderReference(IntPtr renderPassEncoder);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPassEncoderRelease")]
        public static partial void RenderPassEncoderRelease(IntPtr renderPassEncoder);
        
        #endregion

        #region Render Pipeline

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPipelineGetBindGroupLayout")]
        public static partial IntPtr RenderPipelineGetBindGroupLayout(IntPtr renderPipeline, uint groupIndex);

        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPipelineSetLabel")]
        public static partial void RenderPipelineSetLabel(IntPtr renderPipeline, char* label);
        
        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPipelineReference")]
        public static partial void RenderPipelineReference(IntPtr renderPipeline);
        
        [LibraryImport(LibraryName, EntryPoint = "wgpuRenderPipelineRelease")]
        public static partial void RenderPipelineRelease(IntPtr renderPipeline);

        #endregion

        #region Sampler

        [LibraryImport(LibraryName, EntryPoint = "wgpuSamplerSetLabel")]
        public static partial void SamplerSetLabel(IntPtr sampler, char* label);
        [LibraryImport(LibraryName, EntryPoint = "wgpuSamplerReference")]
        public static partial void SamplerReference(IntPtr sampler);
        [LibraryImport(LibraryName, EntryPoint = "wgpuSamplerRelease")]
        public static partial void SamplerRelease(IntPtr sampler);

        #endregion

        #region Shader Module

        [LibraryImport(LibraryName, EntryPoint = "wgpuShaderModuleGetCompilationInfo")]
        public static partial void ShaderModuleGetCompilationInfo(IntPtr shaderModule, WGPUCompilationInfoCallback callback, void* userdata);

        [LibraryImport(LibraryName, EntryPoint = "wgpuShaderModuleSetLabel")]
        public static partial void ShaderModuleSetLabel(IntPtr shaderModule, char* label);
       
        [LibraryImport(LibraryName, EntryPoint = "wgpuShaderModuleReference")]
        public static partial void ShaderModuleReference(IntPtr shaderModule);

        [LibraryImport(LibraryName, EntryPoint = "wgpuShaderModuleRelease")]
        public static partial void ShaderModuleRelease(IntPtr shaderModule);

        #endregion

        #region Surface

        [LibraryImport(LibraryName, EntryPoint = "wgpuSurfaceGetPreferredFormat")]
        public static partial WGPUTextureFormat SurfaceGetPreferredFormat(IntPtr surface, IntPtr adapter);

        [LibraryImport(LibraryName, EntryPoint = "wgpuSurfaceReference")]
        public static partial void SurfaceReference(IntPtr surface);

        [LibraryImport(LibraryName, EntryPoint = "wgpuSurfaceRelease")]
        public static partial void SurfaceRelease(IntPtr surface);

        #endregion

        #region Swapchain

        [LibraryImport(LibraryName, EntryPoint = "wgpuSwapChainGetCurrentTextureView")]
        public static partial IntPtr SwapChainGetCurrentTextureView(IntPtr swapChain);

        [LibraryImport(LibraryName, EntryPoint = "wgpuSwapChainPresent")]
        public static partial void SwapChainPresent(IntPtr swapChain);

        [LibraryImport(LibraryName, EntryPoint = "wgpuSwapChainReference")]
        public static partial void SwapChainReference(IntPtr swapChain);

        [LibraryImport(LibraryName, EntryPoint = "wgpuSwapChainRelease")]
        public static partial void SwapChainRelease(IntPtr swapChain);
       
        #endregion

        #region Texture

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureCreateView")]
        public static partial IntPtr TextureCreateView(IntPtr texture, in WGPUTextureViewDescriptor descriptor);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureDestroy")]
        public static partial void TextureDestroy(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetDepthOrArrayLayers")]
        public static partial uint TextureGetDepthOrArrayLayers(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetDimension")]
        public static partial WGPUTextureDimension TextureGetDimension(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetFormat")]
        public static partial WGPUTextureFormat TextureGetFormat(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetHeight")]
        public static partial uint TextureGetHeight(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetMipLevelCount")]
        public static partial uint TextureGetMipLevelCount(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetSampleCount")]
        public static partial uint TextureGetSampleCount(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetUsage")]
        public static partial WGPUTextureUsage TextureGetUsage(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureGetWidth")]
        public static partial uint TextureGetWidth(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureSetLabel")]
        public static partial void TextureSetLabel(IntPtr texture, char* label);
        
        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureReference")]
        public static partial void TextureReference(IntPtr texture);

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureRelease")]
        public static partial void TextureRelease(IntPtr texture);

        #endregion

        #region TextureView

        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureViewSetLabel")]
		public static partial void TextureViewSetLabel(IntPtr textureView, char* label);
        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureViewReference")]
        public static partial void TextureViewReference(IntPtr textureView);
        [LibraryImport(LibraryName, EntryPoint = "wgpuTextureViewRelease")]
        public static partial void TextureViewRelease(IntPtr textureView);

        #endregion
    }
}