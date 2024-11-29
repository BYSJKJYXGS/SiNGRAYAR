using System;
using System.Runtime.InteropServices;
namespace XvXRFoundation
{
    public class XvPlane
    {
        [DllImport("XvXRRenderPlugin")]
        public static extern bool startDetectPlaneFromStereo();
        [DllImport("XvXRRenderPlugin")]
        public static extern bool getPlaneFromStereo(IntPtr data, ref int len);
        [DllImport("XvXRRenderPlugin")]
        public static extern bool stopDetectPlaneFromStereo();
    }
}
