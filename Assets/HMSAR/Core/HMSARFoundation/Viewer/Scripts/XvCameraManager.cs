
using HMS.Core;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;


namespace XvXRFoundation
{
    /// <summary>
    /// 可以通过该类获取相机的图像数据以及相机的开关功能
    /// tof相机
    /// AR眼镜相机
    /// 计算单元相机
    /// 左右鱼眼相机
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class XvCameraManager :MonoBehaviour
    {

        private XvCameraManager() { }
        /// <summary>
        /// 设置分辨率
        /// </summary>
        [SerializeField]
        private RgbResolution rgbResolution;
        /// <summary>
        /// 设置帧率
        /// </summary>
        [SerializeField]
        private int requestedFPS = 30;

        /// <summary>
        /// 宽高尺寸
        /// </summary>
        private int requestedWidth = 1920;
        private int requestedHeight = 1080;


        /// <summary>
        /// 相机数据回调
        /// </summary>
        public static UnityEvent<cameraData> onARCameraStreamFrameArrived=new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onLeftStereoStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onRightStereoStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onTofDepthCameraStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onTofIRCameraStreamFrameArrived = new UnityEvent<cameraData>();

        public static UnityEvent<cameraData> onWebCameraStreamFrameArrived = new UnityEvent<cameraData>();
        

        private void SetCameraParameter(CameraSetting cameraSetting) {
             requestedWidth = cameraSetting.width;
             requestedHeight = cameraSetting.height;
             requestedFPS = cameraSetting.requestedFPS;
        }
        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="cameraType"></param>
        public void StartCapture(XvCameraStreamType cameraType)
        {
        
#if PLATFORM_ANDROID && !UNITY_EDITOR

            while (!API.xslam_ready())
            {
                MyDebugTool.Log("xslam_ready==false");
            }
#endif

            switch (rgbResolution)
            {
                case RgbResolution.RGB_1920x1080:
                    requestedWidth = 1920;
                    requestedHeight = 1080;
                    break;
                case RgbResolution.RGB_1280x720:
                    requestedWidth = 1280;
                    requestedHeight = 720;
                    break;
                case RgbResolution.RGB_640x480:
                    requestedWidth = 640;
                    requestedHeight = 1048080;
                    break;
                case RgbResolution.RGB_320x240:
                    requestedWidth = 320;
                    requestedHeight = 240;
                    break;
                case RgbResolution.RGB_2560x1920:
                    requestedWidth = 2560;
                    requestedHeight = 1920;
                    break;
                default:
                    break;
            }


            switch (cameraType)
            {
                case XvCameraStreamType.WebCameraStream:
                   
                    XvWebCameraManager.GetXvWebCameraManager().StartCapture(requestedWidth, requestedHeight, requestedFPS);
                    break;
                case XvCameraStreamType.ARCameraStream:

                   
                    XvARCameraManager.GetXvARCameraManager().StartCapture(requestedWidth, requestedHeight, requestedFPS);
                    break;
                case XvCameraStreamType.TofDepthCameraStream:
                   
                    XvTofManager.GetXvTofManager().StartCapture(requestedWidth, requestedHeight, requestedFPS, 0);
                    break;
                //case XvCameraStreamType.TofIRCameraStream:

                //    XvTofManager.GetXvTofManager().StartCapture(requestedWidth, requestedHeight, requestedFPS, 1);
                //    break;
                case XvCameraStreamType.LeftStereoCameraStream:
                   
                    XvStereoCameraManager.GetXvStereoCameraManager(true).StartCapture(requestedWidth, requestedHeight, requestedFPS,true);
                    break;
                case XvCameraStreamType.RightStereoCameraStream:

                    XvStereoCameraManager.GetXvStereoCameraManager(false).StartCapture(requestedWidth, requestedHeight, requestedFPS, false);

                    break;
                default:
                    break;
            }
           
        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <param name="cameraType"></param>
        public void StopCapture(XvCameraStreamType cameraType)
        {
            switch (cameraType)
            {
                case XvCameraStreamType.WebCameraStream:
                    XvWebCameraManager.GetXvWebCameraManager().StopCapture();

                    break;
                case XvCameraStreamType.ARCameraStream:
                    XvARCameraManager.GetXvARCameraManager().StopCapture();
                    break;
                case XvCameraStreamType.TofDepthCameraStream:
                    XvTofManager.GetXvTofManager().StopCapture(0);
                    break;

                //case XvCameraStreamType.TofIRCameraStream:
                //    XvTofManager.GetXvTofManager().StopCapture(1);
                //    break;

                case XvCameraStreamType.LeftStereoCameraStream:
                    XvStereoCameraManager.GetXvStereoCameraManager(true).StopCapture();

                    break;
                case XvCameraStreamType.RightStereoCameraStream:
                    XvStereoCameraManager.GetXvStereoCameraManager(false).StopCapture();
                    break;
                default:
                    break;
            }
        }

        public bool IsOn(XvCameraStreamType cameraType)
        {
            switch (cameraType)
            {
                case XvCameraStreamType.WebCameraStream:
                   return XvWebCameraManager.GetXvWebCameraManager().IsOn;

                case XvCameraStreamType.ARCameraStream:
                    return XvARCameraManager.GetXvARCameraManager().IsOn;
             
                case XvCameraStreamType.TofDepthCameraStream:

                    return XvTofManager.GetXvTofManager().IsOn(0);

                //case XvCameraStreamType.TofIRCameraStream:

                //    return XvTofManager.GetXvTofManager().IsOn(1);

                case XvCameraStreamType.LeftStereoCameraStream:
                    return XvStereoCameraManager.GetXvStereoCameraManager(true).IsOn;

            
                case XvCameraStreamType.RightStereoCameraStream:
                    return XvStereoCameraManager.GetXvStereoCameraManager(false).IsOn;
               
                default:
                    break;
            }

            return false;
        }


        #region Tof相机
        private int width;
        private int height;
        private bool isGetTofData;
        private Vector3[] vecGroup;


        private bool startPointCloud;

        [DllImport("xslam-unity-wrapper")]
        public static extern bool xslam_tof_set_exposure(int aecMode, int exposureGain, float exposureTimeMs);

        /// <summary>
        /// 启用tof点云获取
        /// </summary>
        public void StartTofPointCloud()
        {
           
            startPointCloud = true;
            while (!API.xslam_ready())
            {
                MyDebugTool.Log("slam 未启动");
            }

            if (!IsOn(XvCameraStreamType.TofDepthCameraStream)) {
                StopCapture(XvCameraStreamType.TofDepthCameraStream);
            }
            XvTofManager.GetXvTofManager().SetTofStreamMode(4);
            XvTofManager.GetXvTofManager().StartTofStream();
        }

        /// <summary>
        /// 获取点云数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool GetPointCloudData(out Vector3[] data)
        {
            if (!startPointCloud)
            {
                data = null;
                return false;
            }

            while (!API.xslam_ready())
            {
                MyDebugTool.Log("slam 未启动");
            }
           
            if (!isGetTofData)
            {
                width = API.xslam_get_tof_width();
                height = API.xslam_get_tof_height();

                //Debug.Log($"XVTof width:{API.xslam_get_tof_width()},height:{API.xslam_get_tof_height()}");
                if (width > 0)
                {
                    vecGroup = new Vector3[width * height];

                    isGetTofData = true;
                }
            }

            if (isGetTofData && vecGroup.Length > 0)
            {
                bool b = API.xslam_get_cloud_data_ex(vecGroup);
                data = vecGroup;

                return b;
            }
            data = null;

            return false;
        }

        /// <summary>
        /// 设置tof参数
        /// </summary>
        /// <param name="libmode"></param>
        /// <param name="resulution">分辨率</param>
        /// <param name="fps">fps</param>
        /// <param name="exposureTimeMs">曝光时长</param>
        public void SetTofExposure(int libmode, int resulution, int fps, float exposureTimeMs)
        {
            XvTofManager.GetXvTofManager().StopTofStream();

            bool v1 = API.xslam_start_sony_tof_stream(libmode, resulution, fps);
            bool v2 = xslam_tof_set_exposure(1, 0, exposureTimeMs);
        }

        public void StopTofPointCloud()
        {
            startPointCloud = false;
            XvTofManager.GetXvTofManager().StopTofStream();

        }

        #endregion

        private void Update()
        {

            if (XvStereoCameraManager.GetXvStereoCameraManager(true).IsOn) {
                XvStereoCameraManager.GetXvStereoCameraManager(true).Update();
            }

            if (XvStereoCameraManager.GetXvStereoCameraManager(false).IsOn)
            {
                XvStereoCameraManager.GetXvStereoCameraManager(false).Update();
            }
            XvTofManager.GetXvTofManager().Update();

            if (XvARCameraManager.GetXvARCameraManager().IsOn)
            {
                XvARCameraManager.GetXvARCameraManager().Update();
            }

            if (XvWebCameraManager.GetXvWebCameraManager().IsOn)
            {
                XvWebCameraManager.GetXvWebCameraManager().Update();
            }
        }

        private void OnDestroy()
        {
           
            StopCapture(XvCameraStreamType.WebCameraStream);
            StopCapture(XvCameraStreamType.ARCameraStream);
            StopCapture(XvCameraStreamType.TofDepthCameraStream);
            //StopCapture(XvCameraStreamType.TofIRCameraStream);
            StopCapture(XvCameraStreamType.LeftStereoCameraStream);
            StopCapture(XvCameraStreamType.RightStereoCameraStream);

        }

        private void OnApplicationQuit()
        {
            StopCapture(XvCameraStreamType.WebCameraStream);
            StopCapture(XvCameraStreamType.ARCameraStream);
            StopCapture(XvCameraStreamType.TofDepthCameraStream);
           // StopCapture(XvCameraStreamType.TofIRCameraStream);
            StopCapture(XvCameraStreamType.LeftStereoCameraStream);
            StopCapture(XvCameraStreamType.RightStereoCameraStream);
        }

    }

    public enum XvCameraStreamType
    {

        WebCameraStream,//计算单元后置摄像头
        ARCameraStream,//MR眼镜rgb相机
        TofDepthCameraStream,//Tof 深度相机
        //TofIRCameraStream,//Tof IR相机

        LeftStereoCameraStream,//左鱼眼相机
        RightStereoCameraStream,//右鱼眼相机
    }

    public class cameraData
    {
        public int texWidth;
        public int texHeight;
        public Texture tex;
      

        //相机姿态


        public CameraParameter parameter;

    }

    public struct CameraParameter
    {
        //AR相机位姿
        public Vector3 position;
        public Quaternion rotation;

        //时间戳
        public double timeStamp;

        //相机内参
        public float focal;
        public float fx;
        public float fy;
        public float cx;
        public float cy;


        //纹理宽高
        public float width;
        public float height;
    }

    // RGB_1920x1080 = 0, ///< RGB 1080p
    // RGB_1280x720  = 1, ///< RGB 720p
    // RGB_640x480   = 2, ///< RGB 480p
    // RGB_320x240   = 3, ///< RGB QVGA
    // RGB_2560x1920 = 4, ///< RGB 5m
    public enum RgbResolution
    {
        RGB_1920x1080 = 0,
        RGB_1280x720 = 1,
        RGB_640x480 = 2,
        RGB_320x240 = 3,
        RGB_2560x1920 = 4
    }

    public struct CameraSetting
    {

        public int width;
        public int height;
        public int requestedFPS;
    }
}