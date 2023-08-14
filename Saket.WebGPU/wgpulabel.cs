using System.Runtime.InteropServices;
using System.Text;

namespace Saket.WebGPU
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct wgpulabel
    {
        public readonly ReadOnlySpan<byte> bytes;

        public wgpulabel()
        {
            bytes = new ReadOnlySpan<byte>();
        }

        public wgpulabel(ReadOnlySpan<byte> bytes)
        {
            this.bytes = bytes;
        }
        public wgpulabel(string label)
        {
            //???
            bytes = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(label));
        }
    }
}
