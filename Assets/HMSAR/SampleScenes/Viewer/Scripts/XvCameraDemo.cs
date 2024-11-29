using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace XvXRFoundation
{
   /// <summary>
   /// 该类演示相机的开关控制，以及视频流的获取
   /// </summary>
    public class XvCameraDemo : MonoBehaviour
    {
        [SerializeField]
        private XvCameraManager cameManager;

        [SerializeField]
        private XvMRVideoCaptureManager captureManager;

        public RawImage arCameraImage;
        public RawImage webCameraImage;
        public RawImage leftStereoCameraImage;
        public RawImage rightStereoCameraImage;
        public RawImage tofCameraImage;

        public RawImage mrVideoImage;

       // public RawImage tofIRCameraImage;




        private void Awake()
        {
            if (cameManager==null) {
                cameManager = FindObjectOfType<XvCameraManager>();

                if (cameManager == null)
                {
                    cameManager = new GameObject("XvCameraManager").AddComponent<XvCameraManager>();
                }
            }

            if (captureManager == null)
            {
                captureManager = FindObjectOfType<XvMRVideoCaptureManager>();

                if (captureManager == null)
                {
                    GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));

                    newObj.name = "XvMRVideoCaptureManager";
                    captureManager = newObj.GetComponent<XvMRVideoCaptureManager>();
                }
            }

        }

     

       
        public void StartARCamera() {
            XvCameraManager.onARCameraStreamFrameArrived.AddListener(onARCameraFrameArrived);
            cameManager.StartCapture(XvCameraStreamType.ARCameraStream);
        }
        public void StopARCamera()
        {
            XvCameraManager.onARCameraStreamFrameArrived.RemoveListener(onARCameraFrameArrived);

            cameManager.StopCapture(XvCameraStreamType.ARCameraStream);

            arCameraImage.texture = null;
        }

        public void StartTofCamera() {
            XvCameraManager.onTofDepthCameraStreamFrameArrived.AddListener(onTofCameraFrameArrived);
            cameManager.StartCapture(XvCameraStreamType.TofDepthCameraStream);

        }
        public void StopTofCamera()
        {
            XvCameraManager.onTofDepthCameraStreamFrameArrived.RemoveListener(onTofCameraFrameArrived);
            cameManager.StopCapture(XvCameraStreamType.TofDepthCameraStream);
            tofCameraImage.texture = null;
        }

        //public void StartTofIRCamera()
        //{
        //    XvCameraManager.onTofIRCameraStreamFrameArrived.AddListener(onTofIRCameraFrameArrived);
        //    cameManager.StartCapture(XvCameraStreamType.TofIRCameraStream);

        //}
        //public void StopTofIRCamera()
        //{
        //    XvCameraManager.onTofIRCameraStreamFrameArrived.RemoveListener(onTofIRCameraFrameArrived);
        //    cameManager.StopCapture(XvCameraStreamType.TofIRCameraStream);
        //    tofIRCameraImage.texture = null;
        //}



        public void StartWebCamera()
        {
            XvCameraManager.onWebCameraStreamFrameArrived.AddListener(onWebCameraFrameArrived);

            cameManager.StartCapture(XvCameraStreamType.WebCameraStream);

        }

        public void StopWebCamera() {
            XvCameraManager.onWebCameraStreamFrameArrived.RemoveListener(onWebCameraFrameArrived);

            cameManager.StopCapture(XvCameraStreamType.WebCameraStream);
            webCameraImage.texture = null;

        }

        public void StartLeftStereoCamera() {
            XvCameraManager.onLeftStereoStreamFrameArrived.AddListener(onLeftStereoCameraFrameArrived);

            cameManager.StartCapture(XvCameraStreamType.LeftStereoCameraStream);

        }

        public void StopLeftStereoCamera()
        {
            XvCameraManager.onLeftStereoStreamFrameArrived.RemoveListener(onLeftStereoCameraFrameArrived);

            cameManager.StopCapture(XvCameraStreamType.LeftStereoCameraStream);
            leftStereoCameraImage.texture = null;

        }

        public void StartRightStereoCamera()
        {
            XvCameraManager.onRightStereoStreamFrameArrived.AddListener(onRightStereoCameraFrameArrived);

            cameManager.StartCapture(XvCameraStreamType.RightStereoCameraStream);

        }

        public void StopRightStereoCamera()
        {
            XvCameraManager.onRightStereoStreamFrameArrived.RemoveListener(onRightStereoCameraFrameArrived);

            cameManager.StopCapture(XvCameraStreamType.RightStereoCameraStream);
            rightStereoCameraImage.texture = null;

        }


        public void StartMRCaptureCamera()
        {
            mrVideoImage.texture = captureManager.CameraRenderTexture;
            captureManager.StartCapture();
        }

        public void StopMRCaptureCamera()
        {
            mrVideoImage.texture = null;
            captureManager.StopCapture();
        }
        private void onARCameraFrameArrived(cameraData cameraData)
        {

            if (arCameraImage != null)
            {
                arCameraImage.texture = cameraData.tex;
            }
        }

        private void onWebCameraFrameArrived(cameraData cameraData)
        {

            if (webCameraImage != null)
            {
                webCameraImage.texture = cameraData.tex;
            }
        }
        private void onLeftStereoCameraFrameArrived(cameraData cameraData)
        {

            if (leftStereoCameraImage != null)
            {
                leftStereoCameraImage.texture = cameraData.tex;
            }
        }
        private void onRightStereoCameraFrameArrived(cameraData cameraData)
        {

            if (rightStereoCameraImage != null)
            {
                rightStereoCameraImage.texture = cameraData.tex;
            }
        }
        private void onTofCameraFrameArrived(cameraData cameraData)
        {

            if (tofCameraImage != null)
            {
                tofCameraImage.texture = cameraData.tex;
            }
        }

        //private void onTofIRCameraFrameArrived(cameraData cameraData)
        //{

        //    if (tofIRCameraImage != null)
        //    {
        //        tofIRCameraImage.texture = cameraData.tex;
        //    }
        //}
    }
}
