using System;
using System.Runtime.InteropServices;

namespace Saket.Engine.WebGPU
{
    //supress to match native api more closely
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public static unsafe partial class wgpu
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        private const string dllName = "wgpu_native.dll";


        [DllImport(dllName, EntryPoint = "wgpuCreateInstance")]
        public static extern IntPtr CreateInstance(ref WGPUInstanceDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuGetProcAddress")]
        public static extern WGPUProc GetProcAddress(IntPtr device, char* procName);


        #region Adapter

        [DllImport(dllName, EntryPoint = "wgpuAdapterEnumerateFeatures")]
        public static extern ulong AdapterEnumerateFeatures(IntPtr adapter, WGPUFeatureName* features);

        [DllImport(dllName, EntryPoint = "wgpuAdapterGetLimits")]
        public static extern bool AdapterGetLimits(IntPtr adapter, WGPUSupportedLimits* limits);

        [DllImport(dllName, EntryPoint = "wgpuAdapterGetProperties")]
        public static extern void AdapterGetProperties(IntPtr adapter, WGPUAdapterProperties* properties);

        [DllImport(dllName, EntryPoint = "wgpuAdapterHasFeature")]
        public static extern bool AdapterHasFeature(IntPtr adapter, WGPUFeatureName feature);

        [DllImport(dllName, EntryPoint = "wgpuAdapterRequestDevice")]
        public static extern void AdapterRequestDevice(IntPtr adapter, WGPUDeviceDescriptor* descriptor, WGPURequestDeviceCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuAdapterReference")]
        public static extern bool AdapterReference(IntPtr adapter);
        
        [DllImport(dllName, EntryPoint = "wgpuAdapterRelease")]
        public static extern bool AdapterRelease(IntPtr adapter);

        #endregion

        #region Bindgroup

        [DllImport(dllName, EntryPoint = "wgpuBindGroupSetLabel")]
        public static extern void BindGroupSetLabel(IntPtr bindGroup, char* label);

        [DllImport(dllName, EntryPoint = "wgpuBindGroupReference")]
        public static extern void BindGroupReference(IntPtr bindGroup);

        [DllImport(dllName, EntryPoint = "wgpuBindGroupRelease")]
        public static extern void BindGroupRelease(IntPtr bindGroup);

        #endregion

        #region BindGroupLayout

        [DllImport(dllName, EntryPoint = "wgpuBindGroupLayoutSetLabel")]
        public static extern void BindGroupLayoutSetLabel(IntPtr bindGroupLayout, char* label);

        [DllImport(dllName, EntryPoint = "wgpuBindGroupLayoutReference")]
        public static extern void BindGroupLayoutReference(IntPtr bindGroupLayout);

        [DllImport(dllName, EntryPoint = "wgpuBindGroupLayoutRelease")]
        public static extern void BindGroupLayoutRelease(IntPtr bindGroupLayout);

        #endregion

        #region Buffer

        [DllImport(dllName, EntryPoint = "wgpuBufferDestroy")]
        public static extern void BufferDestroy(IntPtr buffer);

        [DllImport(dllName, EntryPoint = "wgpuBufferGetConstMappedRange")]
        public static extern void* BufferGetConstMappedRange(IntPtr buffer, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuBufferGetMapState")]
        public static extern WGPUBufferMapState BufferGetMapState(IntPtr buffer);

        [DllImport(dllName, EntryPoint = "wgpuBufferGetMappedRange")]
        public static extern void* BufferGetMappedRange(IntPtr buffer, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuBufferGetSize")]
        public static extern ulong BufferGetSize(IntPtr buffer);

        [DllImport(dllName, EntryPoint = "wgpuBufferGetUsage")]
        public static extern WGPUBufferUsage BufferGetUsage(IntPtr buffer);

        [DllImport(dllName, EntryPoint = "wgpuBufferMapAsync")]
        public static extern void BufferMapAsync(IntPtr buffer, WGPUMapMode mode, ulong offset, ulong size, WGPUBufferMapCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuBufferSetLabel")]
        public static extern void BufferSetLabel(IntPtr buffer, char* label);

        [DllImport(dllName, EntryPoint = "wgpuBufferUnmap")]
        public static extern void BufferUnmap(IntPtr buffer);

        [DllImport(dllName, EntryPoint = "wgpuBufferReference")]
        public static extern void BufferReference(IntPtr buffer);

        [DllImport(dllName, EntryPoint = "wgpuBufferRelease")]
        public static extern void BufferRelease(IntPtr buffer);

        #endregion

        #region CommandBuffer

        [DllImport(dllName, EntryPoint = "wgpuCommandBufferSetLabel")]
        public static extern void CommandBufferSetLabel(IntPtr commandBuffer, char* label);

        [DllImport(dllName, EntryPoint = "wgpuCommandBufferReference")]
        public static extern void CommandBufferReference(IntPtr commandBuffer);

        [DllImport(dllName, EntryPoint = "wgpuCommandBufferRelease")]
        public static extern void CommandBufferRelease(IntPtr commandBuffer);

        #endregion

        #region CommandEncoder

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderBeginComputePass")]
        public static extern IntPtr CommandEncoderBeginComputePass(IntPtr commandEncoder, WGPUComputePassDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderBeginRenderPass")]
        public static extern IntPtr CommandEncoderBeginRenderPass(IntPtr commandEncoder, ref WGPURenderPassDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderClearBuffer")]
        public static extern void CommandEncoderClearBuffer(IntPtr commandEncoder, IntPtr buffer, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderCopyBufferToBuffer")]
        public static extern void CommandEncoderCopyBufferToBuffer(IntPtr commandEncoder, IntPtr source, ulong sourceOffset, IntPtr destination, ulong destinationOffset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderCopyBufferToTexture")]
        public static extern void CommandEncoderCopyBufferToTexture(IntPtr commandEncoder, WGPUImageCopyBuffer* source, WGPUImageCopyTexture* destination, WGPUExtent3D* copySize);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderCopyTextureToBuffer")]
        public static extern void CommandEncoderCopyTextureToBuffer(IntPtr commandEncoder, WGPUImageCopyTexture* source, WGPUImageCopyBuffer* destination, WGPUExtent3D* copySize);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderCopyTextureToTexture")]
        public static extern void CommandEncoderCopyTextureToTexture(IntPtr commandEncoder, WGPUImageCopyTexture* source, WGPUImageCopyTexture* destination, WGPUExtent3D* copySize);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderFinish")]
        public static extern IntPtr CommandEncoderFinish(IntPtr commandEncoder, ref WGPUCommandBufferDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderInsertDebugMarker")]
        public static extern void CommandEncoderInsertDebugMarker(IntPtr commandEncoder, char* markerLabel);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderPopDebugGroup")]
        public static extern void CommandEncoderPopDebugGroup(IntPtr commandEncoder);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderPushDebugGroup")]
        public static extern void CommandEncoderPushDebugGroup(IntPtr commandEncoder, char* groupLabel);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderResolveQuerySet")]
        public static extern void CommandEncoderResolveQuerySet(IntPtr commandEncoder, IntPtr querySet, uint firstQuery, uint queryCount, IntPtr destination, ulong destinationOffset);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderSetLabel")]
        public static extern void CommandEncoderSetLabel(IntPtr commandEncoder, char* label);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderWriteTimestamp")]
        public static extern void CommandEncoderWriteTimestamp(IntPtr commandEncoder, IntPtr querySet, uint queryIndex);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderReference")]
        public static extern void CommandEncoderReference(IntPtr commandEncoder);

        [DllImport(dllName, EntryPoint = "wgpuCommandEncoderRelease")]
        public static extern void CommandEncoderRelease(IntPtr commandEncoder);

        #endregion

        #region ComputePassEncoder

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderBeginPipelineStatisticsQuery")]
        public static extern void ComputePassEncoderBeginPipelineStatisticsQuery(IntPtr computePassEncoder, IntPtr querySet, uint queryIndex);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderDispatchWorkgroups")]
        public static extern void ComputePassEncoderDispatchWorkgroups(IntPtr computePassEncoder, uint workgroupCountX, uint workgroupCountY, uint workgroupCountZ);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderDispatchWorkgroupsIndirect")]
        public static extern void ComputePassEncoderDispatchWorkgroupsIndirect(IntPtr computePassEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderEnd")]
        public static extern void ComputePassEncoderEnd(IntPtr computePassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderEndPipelineStatisticsQuery")]
        public static extern void ComputePassEncoderEndPipelineStatisticsQuery(IntPtr computePassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderInsertDebugMarker")]
        public static extern void ComputePassEncoderInsertDebugMarker(IntPtr computePassEncoder, char* markerLabel);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderPopDebugGroup")]
        public static extern void ComputePassEncoderPopDebugGroup(IntPtr computePassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderPushDebugGroup")]
        public static extern void ComputePassEncoderPushDebugGroup(IntPtr computePassEncoder, char* groupLabel);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderSetBindGroup")]
        public static extern void ComputePassEncoderSetBindGroup(IntPtr computePassEncoder, uint groupIndex, IntPtr group, uint dynamicOffsetCount, uint* dynamicOffsets);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderSetLabel")]
        public static extern void ComputePassEncoderSetLabel(IntPtr computePassEncoder, char* label);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderSetPipeline")]
        public static extern void ComputePassEncoderSetPipeline(IntPtr computePassEncoder, IntPtr pipeline);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderReference")]
        public static extern void ComputePassEncoderReference(IntPtr computePassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuComputePassEncoderRelease")]
        public static extern void ComputePassEncoderRelease(IntPtr computePassEncoder);

        #endregion

        #region ComputePipeline

        [DllImport(dllName, EntryPoint = "wgpuComputePipelineGetBindGroupLayout")]
        public static extern IntPtr ComputePipelineGetBindGroupLayout(IntPtr computePipeline, uint groupIndex);

        [DllImport(dllName, EntryPoint = "wgpuComputePipelineSetLabel")]
        public static extern void ComputePipelineSetLabel(IntPtr computePipeline, char* label);

        [DllImport(dllName, EntryPoint = "wgpuComputePipelineReference")]
        public static extern void ComputePipelineReference(IntPtr computePipeline);

        [DllImport(dllName, EntryPoint = "wgpuComputePipelineRelease")]
        public static extern void ComputePipelineRelease(IntPtr computePipeline);

        #endregion

        #region Device

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateBindGroup")]
        public static extern IntPtr DeviceCreateBindGroup(IntPtr device, WGPUBindGroupDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateBindGroupLayout")]
        public static extern IntPtr DeviceCreateBindGroupLayout(IntPtr device, WGPUBindGroupLayoutDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateBuffer")]
        public static extern IntPtr DeviceCreateBuffer(IntPtr device, ref WGPUBufferDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateCommandEncoder")]
        public static extern IntPtr DeviceCreateCommandEncoder(IntPtr device, ref WGPUCommandEncoderDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateComputePipeline")]
        public static extern IntPtr DeviceCreateComputePipeline(IntPtr device, WGPUComputePipelineDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateComputePipelineAsync")]
        public static extern void DeviceCreateComputePipelineAsync(IntPtr device, WGPUComputePipelineDescriptor* descriptor, WGPUCreateComputePipelineAsyncCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreatePipelineLayout")]
        public static extern IntPtr DeviceCreatePipelineLayout(IntPtr device, ref WGPUPipelineLayoutDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateQuerySet")]
        public static extern IntPtr DeviceCreateQuerySet(IntPtr device, WGPUQuerySetDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateRenderBundleEncoder")]
        public static extern IntPtr DeviceCreateRenderBundleEncoder(IntPtr device, WGPURenderBundleEncoderDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateRenderPipeline")]
        public static extern IntPtr DeviceCreateRenderPipeline(IntPtr device, ref WGPURenderPipelineDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateRenderPipelineAsync")]
        public static extern void DeviceCreateRenderPipelineAsync(IntPtr device, ref WGPURenderPipelineDescriptor descriptor, WGPUCreateRenderPipelineAsyncCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateSampler")]
        public static extern IntPtr DeviceCreateSampler(IntPtr device, WGPUSamplerDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateShaderModule")]
        public static extern IntPtr DeviceCreateShaderModule(IntPtr device, ref WGPUShaderModuleDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateSwapChain")]
        public static extern IntPtr DeviceCreateSwapChain(IntPtr device, IntPtr surface, ref WGPUSwapChainDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceCreateTexture")]
        public static extern IntPtr DeviceCreateTexture(IntPtr device, WGPUTextureDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuDeviceDestroy")]
        public static extern void DeviceDestroy(IntPtr device);

        [DllImport(dllName, EntryPoint = "wgpuDeviceEnumerateFeatures")]
        public static extern ulong DeviceEnumerateFeatures(IntPtr device, WGPUFeatureName* features);

        [DllImport(dllName, EntryPoint = "wgpuDeviceGetLimits")]
        public static extern bool DeviceGetLimits(IntPtr device, WGPUSupportedLimits* limits);

        [DllImport(dllName, EntryPoint = "wgpuDeviceGetQueue")]
        public static extern IntPtr DeviceGetQueue(IntPtr device);

        [DllImport(dllName, EntryPoint = "wgpuDeviceHasFeature")]
        public static extern bool DeviceHasFeature(IntPtr device, WGPUFeatureName feature);

        [DllImport(dllName, EntryPoint = "wgpuDevicePopErrorScope")]
        public static extern bool DevicePopErrorScope(IntPtr device, WGPUErrorCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuDevicePushErrorScope")]
        public static extern void DevicePushErrorScope(IntPtr device, WGPUErrorFilter filter);

        [DllImport(dllName, EntryPoint = "wgpuDeviceSetDeviceLostCallback")]
        public static extern void DeviceSetDeviceLostCallback(IntPtr device, WGPUDeviceLostCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuDeviceSetLabel")]
        public static extern void DeviceSetLabel(IntPtr device, char* label);

        [DllImport(dllName, EntryPoint = "wgpuDeviceSetUncapturedErrorCallback")]
        public static extern void DeviceSetUncapturedErrorCallback(IntPtr device, WGPUErrorCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuDeviceReference")]
        public static extern IntPtr DeviceReference(IntPtr device);

        [DllImport(dllName, EntryPoint = "wgpuDeviceRelease")]
        public static extern IntPtr DeviceRelease(IntPtr device);

        #endregion

        #region Instance

        [DllImport(dllName, EntryPoint = "wgpuInstanceCreateSurface")]
        public static extern IntPtr InstanceCreateSurface(IntPtr instance, ref WGPUSurfaceDescriptor descriptor);

        [DllImport(dllName, EntryPoint = "wgpuInstanceProcessEvents")]
        public static extern void InstanceProcessEvents(IntPtr instance);

        [DllImport(dllName, EntryPoint = "wgpuInstanceRequestAdapter")]
        public static extern void InstanceRequestAdapter(IntPtr instance, WGPURequestAdapterOptions* options, WGPURequestAdapterCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuInstanceReference")]
        public static extern void InstanceReference(IntPtr instance);

        [DllImport(dllName, EntryPoint = "wgpuInstanceRelease")]
        public static extern void InstanceRelease(IntPtr instance);

        #endregion

        #region PipelineLayout

        [DllImport(dllName, EntryPoint = "wgpuPipelineLayoutSetLabel")]
        public static extern void PipelineLayoutSetLabel(IntPtr pipelineLayout);

        [DllImport(dllName, EntryPoint = "wgpuPipelineLayoutReference")]
        public static extern void PipelineLayoutReference(IntPtr pipelineLayout);

        [DllImport(dllName, EntryPoint = "wgpuPipelineLayoutRelease")]
        public static extern void PipelineLayoutRelease(IntPtr pipelineLayout);

        #endregion

        #region QuerySet

        [DllImport(dllName, EntryPoint = "wgpuQuerySetDestroy")]
        public static extern void QuerySetDestroy(IntPtr querySet);

        [DllImport(dllName, EntryPoint = "wgpuQuerySetGetCount")]
        public static extern uint QuerySetGetCount(IntPtr querySet);

        [DllImport(dllName, EntryPoint = "wgpuQuerySetGetType")]
        public static extern WGPUQueryType QuerySetGetType(IntPtr querySet);

        [DllImport(dllName, EntryPoint = "wgpuQuerySetSetLabel")]
        public static extern void QuerySetSetLabel(IntPtr querySet, char* label);

        [DllImport(dllName, EntryPoint = "wgpuQuerySetReference")]
        public static extern uint QuerySetReference(IntPtr querySet);

        [DllImport(dllName, EntryPoint = "wgpuQuerySetRelease")]
        public static extern uint QuerySetRelease(IntPtr querySet);

        #endregion

        #region Queue

        [DllImport(dllName, EntryPoint = "wgpuQueueOnSubmittedWorkDone")]
        public static extern void QueueOnSubmittedWorkDone(IntPtr queue, WGPUQueueWorkDoneCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuQueueSetLabel")]
        public static extern void QueueSetLabel(IntPtr queue, char* label);

        [DllImport(dllName, EntryPoint = "wgpuQueueSubmit")]
        public static extern void QueueSubmit(IntPtr queue, uint commandCount, ref IntPtr commands);

        [DllImport(dllName, EntryPoint = "wgpuQueueWriteBuffer")]
        public static extern void QueueWriteBuffer(IntPtr queue, IntPtr buffer, ulong bufferOffset, void* data, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuQueueWriteTexture")]
        public static extern void QueueWriteTexture(IntPtr queue, WGPUImageCopyTexture* destination, void* data, ulong dataSize, WGPUTextureDataLayout* dataLayout, WGPUExtent3D* writeSize);

        [DllImport(dllName, EntryPoint = "wgpuQueueReference")]
        public static extern void QueueReference(IntPtr queue);

        [DllImport(dllName, EntryPoint = "wgpuQueueRelease")]
        public static extern void QueueRelease(IntPtr queue);

        #endregion

        #region RenderBundle

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleSetLabel")]
        public static extern void RenderBundleSetLabel(IntPtr renderBundle, char* label);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleReference")]
        public static extern void RenderBundleReference(IntPtr renderBundle);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleRelease")]
        public static extern void RenderBundleRelease(IntPtr renderBundle);

        #endregion

        #region RenderBundleEncoder

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderDraw")]
        public static extern void RenderBundleEncoderDraw(IntPtr renderBundleEncoder, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderDrawIndexed")]
        public static extern void RenderBundleEncoderDrawIndexed(IntPtr renderBundleEncoder, uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderDrawIndexedIndirect")]
        public static extern void RenderBundleEncoderDrawIndexedIndirect(IntPtr renderBundleEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderDrawIndirect")]
        public static extern void RenderBundleEncoderDrawIndirect(IntPtr renderBundleEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderFinish")]
        public static extern IntPtr RenderBundleEncoderFinish(IntPtr renderBundleEncoder, WGPURenderBundleDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderInsertDebugMarker")]
        public static extern void RenderBundleEncoderInsertDebugMarker(IntPtr renderBundleEncoder, char* markerLabel);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderPopDebugGroup")]
        public static extern void RenderBundleEncoderPopDebugGroup(IntPtr renderBundleEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderPushDebugGroup")]
        public static extern void RenderBundleEncoderPushDebugGroup(IntPtr renderBundleEncoder, char* groupLabel);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderSetBindGroup")]
        public static extern void RenderBundleEncoderSetBindGroup(IntPtr renderBundleEncoder, uint groupIndex, IntPtr group, uint dynamicOffsetCount, uint* dynamicOffsets);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderSetIndexBuffer")]
        public static extern void RenderBundleEncoderSetIndexBuffer(IntPtr renderBundleEncoder, IntPtr buffer, WGPUIndexFormat format, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderSetLabel")]
        public static extern void RenderBundleEncoderSetLabel(IntPtr renderBundleEncoder, char* label);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderSetPipeline")]
        public static extern void RenderBundleEncoderSetPipeline(IntPtr renderBundleEncoder, IntPtr pipeline);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderSetVertexBuffer")]
        public static extern void RenderBundleEncoderSetVertexBuffer(IntPtr renderBundleEncoder, uint slot, IntPtr buffer, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderReference")]
        public static extern void RenderBundleEncoderReference(IntPtr renderBundleEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderBundleEncoderRelease")]
        public static extern void RenderBundleEncoderRelease(IntPtr renderBundleEncoder);

        #endregion

        #region RenderPassEncoder

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderBeginOcclusionQuery")]
        public static extern void RenderPassEncoderBeginOcclusionQuery(IntPtr renderPassEncoder, uint queryIndex);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderBeginPipelineStatisticsQuery")]
        public static extern void RenderPassEncoderBeginPipelineStatisticsQuery(IntPtr renderPassEncoder, IntPtr querySet, uint queryIndex);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderDraw")]
        public static extern void RenderPassEncoderDraw(IntPtr renderPassEncoder, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderDrawIndexed")]
        public static extern void RenderPassEncoderDrawIndexed(IntPtr renderPassEncoder, uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderDrawIndexedIndirect")]
        public static extern void RenderPassEncoderDrawIndexedIndirect(IntPtr renderPassEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderDrawIndirect")]
        public static extern void RenderPassEncoderDrawIndirect(IntPtr renderPassEncoder, IntPtr indirectBuffer, ulong indirectOffset);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderEnd")]
        public static extern void RenderPassEncoderEnd(IntPtr renderPassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderEndOcclusionQuery")]
        public static extern void RenderPassEncoderEndOcclusionQuery(IntPtr renderPassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderEndPipelineStatisticsQuery")]
        public static extern void RenderPassEncoderEndPipelineStatisticsQuery(IntPtr renderPassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderExecuteBundles")]
        public static extern void RenderPassEncoderExecuteBundles(IntPtr renderPassEncoder, uint bundleCount, IntPtr* bundles);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderInsertDebugMarker")]
        public static extern void RenderPassEncoderInsertDebugMarker(IntPtr renderPassEncoder, char* markerLabel);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderPopDebugGroup")]
        public static extern void RenderPassEncoderPopDebugGroup(IntPtr renderPassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderPushDebugGroup")]
        public static extern void RenderPassEncoderPushDebugGroup(IntPtr renderPassEncoder, char* groupLabel);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetBindGroup")]
        public static extern void RenderPassEncoderSetBindGroup(IntPtr renderPassEncoder, uint groupIndex, IntPtr group, uint dynamicOffsetCount, uint* dynamicOffsets);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetBlendConstant")]
        public static extern void RenderPassEncoderSetBlendConstant(IntPtr renderPassEncoder, WGPUColor* color);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetIndexBuffer")]
        public static extern void RenderPassEncoderSetIndexBuffer(IntPtr renderPassEncoder, IntPtr buffer, WGPUIndexFormat format, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetLabel")]
        public static extern void RenderPassEncoderSetLabel(IntPtr renderPassEncoder, char* label);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetPipeline")]
        public static extern void RenderPassEncoderSetPipeline(IntPtr renderPassEncoder, IntPtr pipeline);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetScissorRect")]
        public static extern void RenderPassEncoderSetScissorRect(IntPtr renderPassEncoder, uint x, uint y, uint width, uint height);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetStencilReference")]
        public static extern void RenderPassEncoderSetStencilReference(IntPtr renderPassEncoder, uint reference);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetVertexBuffer")]
        public static extern void RenderPassEncoderSetVertexBuffer(IntPtr renderPassEncoder, uint slot, IntPtr buffer, ulong offset, ulong size);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderSetViewport")]
        public static extern void RenderPassEncoderSetViewport(IntPtr renderPassEncoder, float x, float y, float width, float height, float minDepth, float maxDepth);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderReference")]
        public static extern void RenderPassEncoderReference(IntPtr renderPassEncoder);

        [DllImport(dllName, EntryPoint = "wgpuRenderPassEncoderRelease")]
        public static extern void RenderPassEncoderRelease(IntPtr renderPassEncoder);
        
        #endregion

        #region Render Pipeline

        [DllImport(dllName, EntryPoint = "wgpuRenderPipelineGetBindGroupLayout")]
        public static extern IntPtr RenderPipelineGetBindGroupLayout(IntPtr renderPipeline, uint groupIndex);

        [DllImport(dllName, EntryPoint = "wgpuRenderPipelineSetLabel")]
        public static extern void RenderPipelineSetLabel(IntPtr renderPipeline, char* label);
        
        [DllImport(dllName, EntryPoint = "wgpuRenderPipelineReference")]
        public static extern void RenderPipelineReference(IntPtr renderPipeline);
        
        [DllImport(dllName, EntryPoint = "wgpuRenderPipelineRelease")]
        public static extern void RenderPipelineRelease(IntPtr renderPipeline);

        #endregion

        #region Sampler

        [DllImport(dllName, EntryPoint = "wgpuSamplerSetLabel")]
        public static extern void SamplerSetLabel(IntPtr sampler, char* label);
        [DllImport(dllName, EntryPoint = "wgpuSamplerReference")]
        public static extern void SamplerReference(IntPtr sampler);
        [DllImport(dllName, EntryPoint = "wgpuSamplerRelease")]
        public static extern void SamplerRelease(IntPtr sampler);

        #endregion

        #region Shader Module

        [DllImport(dllName, EntryPoint = "wgpuShaderModuleGetCompilationInfo")]
        public static extern void ShaderModuleGetCompilationInfo(IntPtr shaderModule, WGPUCompilationInfoCallback callback, void* userdata);

        [DllImport(dllName, EntryPoint = "wgpuShaderModuleSetLabel")]
        public static extern void ShaderModuleSetLabel(IntPtr shaderModule, char* label);
       
        [DllImport(dllName, EntryPoint = "wgpuShaderModuleReference")]
        public static extern void ShaderModuleReference(IntPtr shaderModule);

        [DllImport(dllName, EntryPoint = "wgpuShaderModuleRelease")]
        public static extern void ShaderModuleRelease(IntPtr shaderModule);

        #endregion

        #region Surface

        [DllImport(dllName, EntryPoint = "wgpuSurfaceGetPreferredFormat")]
        public static extern WGPUTextureFormat SurfaceGetPreferredFormat(IntPtr surface, IntPtr adapter);

        [DllImport(dllName, EntryPoint = "wgpuSurfaceReference")]
        public static extern void SurfaceReference(IntPtr surface);

        [DllImport(dllName, EntryPoint = "wgpuSurfaceRelease")]
        public static extern void SurfaceRelease(IntPtr surface);

        #endregion

        #region Swapchain

        [DllImport(dllName, EntryPoint = "wgpuSwapChainGetCurrentTextureView")]
        public static extern IntPtr SwapChainGetCurrentTextureView(IntPtr swapChain);

        [DllImport(dllName, EntryPoint = "wgpuSwapChainPresent")]
        public static extern void SwapChainPresent(IntPtr swapChain);

        [DllImport(dllName, EntryPoint = "wgpuSwapChainReference")]
        public static extern void SwapChainReference(IntPtr swapChain);

        [DllImport(dllName, EntryPoint = "wgpuSwapChainRelease")]
        public static extern void SwapChainRelease(IntPtr swapChain);
       
        #endregion

        #region Texture

        [DllImport(dllName, EntryPoint = "wgpuTextureCreateView")]
        public static extern IntPtr TextureCreateView(IntPtr texture, WGPUTextureViewDescriptor* descriptor);

        [DllImport(dllName, EntryPoint = "wgpuTextureDestroy")]
        public static extern void TextureDestroy(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetDepthOrArrayLayers")]
        public static extern uint TextureGetDepthOrArrayLayers(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetDimension")]
        public static extern WGPUTextureDimension TextureGetDimension(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetFormat")]
        public static extern WGPUTextureFormat TextureGetFormat(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetHeight")]
        public static extern uint TextureGetHeight(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetMipLevelCount")]
        public static extern uint TextureGetMipLevelCount(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetSampleCount")]
        public static extern uint TextureGetSampleCount(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetUsage")]
        public static extern WGPUTextureUsage TextureGetUsage(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureGetWidth")]
        public static extern uint TextureGetWidth(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureSetLabel")]
        public static extern void TextureSetLabel(IntPtr texture, char* label);
        
        [DllImport(dllName, EntryPoint = "wgpuTextureReference")]
        public static extern void TextureReference(IntPtr texture);

        [DllImport(dllName, EntryPoint = "wgpuTextureRelease")]
        public static extern void TextureRelease(IntPtr texture);

        #endregion

        #region TextureView

        [DllImport(dllName, EntryPoint = "wgpuTextureViewSetLabel")]
		public static extern void TextureViewSetLabel(IntPtr textureView, char* label);
        [DllImport(dllName, EntryPoint = "wgpuTextureViewReference")]
        public static extern void TextureViewReference(IntPtr textureView);
        [DllImport(dllName, EntryPoint = "wgpuTextureViewRelease")]
        public static extern void TextureViewRelease(IntPtr textureView);

        #endregion
    }
}