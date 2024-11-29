using UnityEngine;
using UnityEngine.UI;

namespace XvXRFoundation
{
    /// <summary>
    /// 该类主要涵盖混合现实视频的捕捉，依赖于XvCameraManager类，
    /// 启用混合现实捕捉之前需要确保相机已经打开
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class XvMRVideoCaptureManager : MonoBehaviour
    {
        private XvMRVideoCaptureManager() { }
        [SerializeField]
        private XvCameraManager cameraManager;


        public XvCameraManager CameraManager
        {
            get
            {

                if (cameraManager == null)
                {
                    cameraManager = FindObjectOfType<XvCameraManager>();
                }

                if (cameraManager == null)
                {
                    cameraManager = new GameObject("XvCameraManager").AddComponent<XvCameraManager>();
                }
                return cameraManager;

            }
        }
        [SerializeField]

        private RawImage rgbBackground;

        [SerializeField]
        private int width = 1920;
        [SerializeField]
        private int height = 1080;


        [SerializeField]

        private Camera bgCamera;
        public Camera BgCamera
        {
            get
            {
                if (bgCamera==null) {
                    bgCamera = transform.Find("BgCamera").GetComponent<Camera>(); ;
                }

               
                return bgCamera;
            }

        }

        /// <summary>
        /// 混合现实纹理
        /// </summary>

        private RenderTexture cameraRenderTexture = null;

        public RenderTexture CameraRenderTexture
        {
            get
            {
                if (cameraRenderTexture == null)
                {
                    cameraRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.RGB565);
                }
                return cameraRenderTexture;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            BgCamera.targetTexture = CameraRenderTexture;
        }


        //private void OnEnable()
        //{
        //    StartCapture();
        //}

        //private void OnDisable()
        //{
        //    StopCapture();
        //}


        /// <summary>
        ///  开启捕捉虚实结合视频流
        /// </summary>

        public void StartCapture()
        {
            gameObject.SetActive(true);
            rgbBackground.gameObject.SetActive(true);
            BgCamera.gameObject.SetActive(true);
            CameraManager.StartCapture(XvCameraStreamType.ARCameraStream);
            XvCameraManager.onARCameraStreamFrameArrived.AddListener(onFrameArrived);
        }
        /// <summary>
        /// 停止捕捉虚实结合视频流，由于相机是共用的，关闭的时候确保需要判断是否会对其他功能造成影响
        /// </summary>
        /// <param name="closeCamera"> true:关闭相机  false：不关闭相机</param>
        public void StopCapture(bool closeCamera = false)
        {
            if (closeCamera)
            {
                CameraManager.StopCapture(XvCameraStreamType.ARCameraStream);
            }
            XvCameraManager.onARCameraStreamFrameArrived.RemoveListener(onFrameArrived);
            gameObject.SetActive(false);

        }

        /// <summary>
        /// 接收相机图像并填充到背景图像，
        /// 设置相机参数
        /// </summary>
        /// <param name="cameraData"></param>
        private void onFrameArrived(cameraData cameraData)
        {

            if (rgbBackground != null)
            {
                rgbBackground.texture = cameraData.tex;
            }

            BgCamera.usePhysicalProperties = true;
            BgCamera.focalLength = cameraData.parameter.focal;
            BgCamera.sensorSize = new Vector2(cameraData.parameter.focal * cameraData.parameter.width / cameraData.parameter.fx,
                                          cameraData.parameter.focal * cameraData.parameter.height / cameraData.parameter.fy);

            BgCamera.lensShift = new Vector2(-(cameraData.parameter.cx - cameraData.parameter.width * 0.5f) / cameraData.parameter.width,
                                         (cameraData.parameter.cy - cameraData.parameter.height * 0.5f) / cameraData.parameter.height);

            BgCamera.gateFit = Camera.GateFitMode.Vertical;


            transform.position = cameraData.parameter.position;
            transform.rotation = cameraData.parameter.rotation;
        }

    }
}
