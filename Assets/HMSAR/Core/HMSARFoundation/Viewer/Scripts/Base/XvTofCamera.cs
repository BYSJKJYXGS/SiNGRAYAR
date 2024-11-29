using UnityEngine;
using System.Runtime.InteropServices;
using System;
using HMS.Core;

namespace XvXRFoundation
{
    public class XvTofCamera : XvCameraBase
    {
        public XvTofCamera(int width, int height, int fps, FrameArrived frameArrived) : base(width, height, fps, frameArrived)
        {

        }

        private Texture2D tex = null;


        private Color32[] pixel32;
        private GCHandle pixelHandle;
        private IntPtr pixelPtr;

        public override void StartCapture()
        {
            if (!isOpen) {

                isOpen = true;
            }
        }

        public override void StopCapture()
        {
            if (isOpen) {
                pixelHandle.Free();
                tex = null;
                
            }
            isOpen = false;

        }
        public override void Update()
        {
            if (isOpen&&API.xslam_ready() )
            {
                int width = API.xslam_get_tof_width();
                int height = API.xslam_get_tof_height();

                if (width > 0 && height > 0)
                {

                    if (!tex)
                    {
                        MyDebugTool.Log("Create TOF texture " + width + "x" + height);
                        TextureFormat format = TextureFormat.RGBA32;
                        tex = new Texture2D(width, height, format, false);


                        pixel32 = tex.GetPixels32();
                        pixelHandle = GCHandle.Alloc(pixel32, GCHandleType.Pinned);
                        pixelPtr = pixelHandle.AddrOfPinnedObject();


                    }


                    if (API.xslam_get_tof_image(pixelPtr, tex.width, tex.height))
                    {
                        //Update the Texture2D with array updated in C++
                        tex.SetPixels32(pixel32);
                        tex.Apply();
                        cameraData.tex = tex;
                        cameraData.texWidth = tex.width;

                        cameraData.texHeight = tex.height;
                        

                        frameArrived?.Invoke(cameraData);
                    }
                    else
                    {
                        MyDebugTool.Log("Invalid TOF texture");
                    }



                }
            }
        }
      
       
    }
}
