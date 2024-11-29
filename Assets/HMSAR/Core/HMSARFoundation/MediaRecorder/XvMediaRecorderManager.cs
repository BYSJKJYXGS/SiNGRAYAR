using NatSuite.Examples;
using UnityEngine;
using UnityEngine.Events;

namespace XvXRFoundation
{
    /// <summary>
    /// 该类主要负责本地视频的录制和截图
    /// </summary>
    [RequireComponent(typeof(ReplayCam))]
    [RequireComponent(typeof(JPG))]

    public sealed class XvMediaRecorderManager : MonoBehaviour
    {
        private XvMediaRecorderManager() { }
        private ReplayCam replayCam;
        private JPG jpg;

        [SerializeField]
        private XvMRVideoCaptureManager xvMRVideoCaptureManager;
        public XvMRVideoCaptureManager XvMRVideoCaptureManager {

            get {

                if (xvMRVideoCaptureManager == null)
                {
                    xvMRVideoCaptureManager = FindObjectOfType<XvMRVideoCaptureManager>();
                }

                if (xvMRVideoCaptureManager == null)
                {
                    GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));
                    xvMRVideoCaptureManager = newObj.GetComponent<XvMRVideoCaptureManager>();
                    newObj.name = "XvMRVideoCaptureManager";
                }

                return xvMRVideoCaptureManager;
            }
        }
     
        [SerializeField]
        private int width=1920;
        public int Width { 
        get { return width; }
        }
        [SerializeField]
        private int height=1080;
        public int Height { 
        get { return height; }
        }

        private void Awake()
        {
            replayCam=GetComponent<ReplayCam>();
            jpg=GetComponent<JPG>();
            replayCam.videoWidth = width;
            replayCam.videoHeight = height;
            jpg.imageWidth=width; 
            jpg.imageHeight=height;
            jpg.cam = XvMRVideoCaptureManager.BgCamera;
            replayCam.cam = XvMRVideoCaptureManager.BgCamera;

        }


        /// <summary>
        /// 开启捕捉虚实结合视频流
        /// </summary>
        public void StartCapture() {
            XvMRVideoCaptureManager.StartCapture();
        }
        /// <summary>
        /// 停止捕捉虚实结合视频流
        /// 注意：相机是公用的，关闭的时候需要评估其他模块是否需要用到相机，如果其他模块使用相机，最好closeCamera==false
        /// </summary>
        /// <param name="closeCamera">true:关闭相机   false:不关闭相机</param>
        public void StopCapture(bool closeCamera=false)
        {
            XvMRVideoCaptureManager.StopCapture(closeCamera);
        }

        /// <summary>
        /// 开始录像，再开始录制前，需要确保StartCapture方法已经调用，混合现实视频流已经开启。
        /// </summary>

        public void StartRecording()
        {
            replayCam.StartRecording();
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        /// <param name="callback"></param>
        public void StopRecording(UnityAction<string> callback)
        {
            replayCam.StopRecording(callback);
        }

        /// <summary>
        /// 保存截图，截图前需要确保StartCapture方法已经调用，混合现实视频流已经开启。
        /// </summary>
        /// <param name="callback"></param>
        public void SaveScreenshot(UnityAction<string> callback)
        {
            jpg.SaveScreenshot(callback);
        }
    }
}
