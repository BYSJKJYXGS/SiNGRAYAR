
using UnityEngine;
namespace XvXRFoundation
{
    public class XvWebCamera : XvCameraBase
    {
        private WebCamTexture webCamTexture;
        private Material material;
        private RenderTexture renderTexture;
        public XvWebCamera(int width, int height, int fps, FrameArrived frameArrived) : base(width, height, fps, frameArrived)
        {

            Shader shader = Shader.Find("MyShader/RgbImage");

            if (shader != null)
            {
                material = new Material(shader);

            }
            else
            {
                Debug.LogError("没有找到材质球MyShader/RgbImage");
            }
        }
        public override void StartCapture()
        {
            WebCamDevice[] webCamDevices = WebCamTexture.devices;

            if (webCamDevices == null || webCamDevices.Length < 0)
            {
                isOpen = false;
                MyDebugTool.Log("No camera detected");

                return;
            }
            else
            {
                MyDebugTool.Log("Detected number of cameras：" + webCamDevices.Length);
            }
            webCamTexture = new WebCamTexture(webCamDevices[0].name, width, height);
            webCamTexture.Play();
            cameraData.tex = webCamTexture;
            cameraData.texWidth = width;
            cameraData.texHeight = height;
            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

            MyDebugTool.Log("Turn on the camera" + webCamTexture.isPlaying);
            isOpen = true;

        }

        public override void Update()
        {
            if (webCamTexture != null)
            {
                cameraData.tex = webCamTexture;
                cameraData.texHeight = width;
                cameraData.texHeight = height;

                Graphics.Blit(webCamTexture, renderTexture, material);

                cameraData.tex = renderTexture;
               
                frameArrived.Invoke(cameraData);
            }
            else
            {
                MyDebugTool.Log(123);

            }

        }

        public override void StopCapture()
        {

            if (isOpen && webCamTexture != null)
            {
                webCamTexture.Stop();
                webCamTexture = null;
                renderTexture = null;



                MyDebugTool.Log("Turn off the camera");
            }
            isOpen = false;


        }


    }
}
