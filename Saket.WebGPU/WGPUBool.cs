using System.Runtime.InteropServices;

namespace Saket.WebGPU
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WGPUBool
    {
        private int value;
        public WGPUBool(int value)
        {
            this.value = value;
        }
        public WGPUBool(bool value)
        {
            this.value = value ? (byte)1 : (byte)0;
        }
    
        public bool Value { readonly get => value != 0; set { this.value = value ? (int)1 : (int)0; } }

        public static implicit operator WGPUBool(bool value) {  return new WGPUBool(value); }
        public static explicit operator bool(WGPUBool value) {  return value.Value; }
    }
}