using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XvXRFoundation
{
    public class MRVideoCaptureDemo : MonoBehaviour
    {
        [SerializeField]
        private XvMRVideoCaptureManager captureManager;

        public RawImage rawImage;
        public RawImage rawImageinphone;
        void Start()
        {
            if (captureManager==null) {
                captureManager = FindObjectOfType<XvMRVideoCaptureManager>();

                if (captureManager==null) {
                    GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));

                    newObj.name = "XvMRVideoCaptureManager";
                    captureManager = newObj.GetComponent<XvMRVideoCaptureManager>();
                }
            }
        }

        public void StartMRCaptureCamera() {
           
            rawImage.texture = captureManager.CameraRenderTexture;
            rawImageinphone.texture = captureManager.CameraRenderTexture;
            captureManager.StartCapture();
        }

        public void StopMRCaptureCamera()
        {
            rawImage.texture = null;
            rawImageinphone.texture = null;
            captureManager.StopCapture();
        }
    }
}
