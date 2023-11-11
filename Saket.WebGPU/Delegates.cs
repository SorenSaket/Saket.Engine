using System;

namespace Saket.WebGPU
{
	public unsafe delegate void BufferMapCallback(
		 BufferMapAsyncStatus status,
		 void* userdata);

	public unsafe delegate void CompilationInfoCallback(
		 CompilationInfoRequestStatus status,
		 CompilationInfo* compilationInfo,
		 void* userdata);

	public unsafe delegate void CreateComputePipelineAsyncCallback(
		 CreatePipelineAsyncStatus status,
		 IntPtr pipeline,
		 char* message,
		 void* userdata);

	public unsafe delegate void CreateRenderPipelineAsyncCallback(
		 CreatePipelineAsyncStatus status,
		 IntPtr pipeline,
		 char* message,
		 void* userdata);

	public unsafe delegate void DeviceLostCallback(
		 DeviceLostReason reason,
		 char* message,
		 void* userdata);

	public unsafe delegate void ErrorCallback(
		 ErrorType type,
		 char* message,
		 void* userdata);

	public unsafe delegate void Proc();

	public unsafe delegate void QueueWorkDoneCallback(
		 QueueWorkDoneStatus status,
		 void* userdata);

	public unsafe delegate void RequestAdapterCallback(
		 RequestAdapterStatus status,
		 IntPtr adapter,
		 char* message,
		 void* userdata);

	public unsafe delegate void RequestDeviceCallback(
		 RequestDeviceStatus status,
		 IntPtr device,
		 char* message,
		 void* userdata);
}
