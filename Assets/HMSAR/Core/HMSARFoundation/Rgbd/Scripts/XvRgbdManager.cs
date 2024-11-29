
using HMS.Core;
using UnityEngine;
using static HMS.Core.API;

namespace XvXRFoundation
{

    /// <summary>
    /// 该类主要实现rgbd功能，通过该类可以启用或者关闭rgbd功能，可以根据rgb像素坐标获取到三维空间坐标
    /// </summary>
    public sealed class XvRgbdManager : MonoBehaviour
    {
        private XvRgbdManager() { }


        public  XvCameraManager cameraManager;


        private double hostTimestamp;

        private void Awake()
        {
            if (cameraManager==null) {
                cameraManager=FindObjectOfType<XvCameraManager>();
            }

            if (cameraManager==null) {
                cameraManager=gameObject.AddComponent<XvCameraManager>();
            }
        }
        private void OnEnable()
        {
            StartRgbPose();
        }

        private void OnDisable()
        {
            StopRgbPose();
        }

        /// <summary>
        /// 开启RGBD功能
        /// </summary>
        public void StartRgbPose() {
#if  UNITY_EDITOR
            return;

#endif
            while (!API.xslam_ready())
            {
                MyDebugTool.Log("xslam_ready==false");
            }
            MyDebugTool.Log("xslam_ready==true");
                API.xv_start_rgb_pixel_pose();
            cameraManager.StartCapture(XvCameraStreamType.ARCameraStream);
            XvCameraManager.onARCameraStreamFrameArrived.AddListener(onFrameArrived);

            API.xv_start_rgb_pixel_pose();
            cameraManager.StartCapture(XvCameraStreamType.ARCameraStream);
            XvCameraManager.onARCameraStreamFrameArrived.AddListener(onFrameArrived);
        }
        /// <summary>
        /// 关闭RGBD功能
        /// </summary>
        public void StopRgbPose() {
#if  UNITY_EDITOR
            return;

#endif
            API.xv_stop_rgb_pixel_pose();
            cameraManager.StopCapture(XvCameraStreamType.ARCameraStream);
            XvCameraManager.onARCameraStreamFrameArrived.RemoveListener(onFrameArrived);


        }


        /// <summary>
        /// 通过rgb像素坐标获取控件三维坐标
        /// </summary>
        /// <param name="rgbPoint">rgb 像素坐标</param>
        /// <param name="spacePoint">空间三维坐标</param>
        /// <returns></returns>
        public bool GetRgbPixel3DPose(Vector2 rgbPoint,ref Vector3 spacePoint)
        {


#if PLATFORM_ANDROID && !UNITY_EDITOR
  API.Vector2F rgbPixelPoint = default(API.Vector2F);
            rgbPixelPoint.x = rgbPoint.x;
            rgbPixelPoint.y = rgbPoint.y;

            API.Vector3F pointerPose = default(API.Vector3F);
            if (API.xv_get_rgb_pixel_pose(ref pointerPose, ref rgbPixelPoint, hostTimestamp, 30))
            {
                spacePoint.x = pointerPose.x;
                spacePoint.y = -pointerPose.y;
                spacePoint.z = pointerPose.z;
                return true;
            }
#endif

            return false;
        }
        /// <summary>
        /// 根据rgb像素坐标列表，获取三维空间中坐标列表
        /// </summary>
        /// <param name="rgbPoint"></param>
        /// <param name="spacePose"></param>
        /// <returns></returns>
        public bool GetRgbPixelPoseList( Vector2[] rgbPoint,ref pointer_3dpose[] spacePose)
        {


#if PLATFORM_ANDROID && !UNITY_EDITOR
  int size = rgbPoint.Length;
            if (API.xslam_start_get_rgb_pixel_buff3d_pose(spacePose, rgbPoint, size, hostTimestamp, 30))
            {

                for (int i = 0; i < spacePose.Length; i++)
                {
                    spacePose[i].pointerPose.y *= -1;
                }
                return true;
            }

          
#endif
            return false;

        }

        private void onFrameArrived(cameraData cameraData)
        {
            hostTimestamp = cameraData.parameter.timeStamp;
        }

    }
}
