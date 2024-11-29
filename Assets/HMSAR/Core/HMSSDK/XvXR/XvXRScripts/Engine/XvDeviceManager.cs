using HMS.Utils;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using AOT;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.Threading;
using HMS.SystemEvents;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using HMS.Core;

namespace HMS.Engine
{

    class XvDeviceManager : MonoBehaviour
    {
        public static XvDeviceManager Manager
        {

            get
            {
                if (manager == null)
                {
                    manager = UnityEngine.Object.FindObjectOfType<XvDeviceManager>();
                }
                if (manager == null)
                {
                    var go = new GameObject("XvDeviceManager");
                    manager = go.AddComponent<XvDeviceManager>();
                    go.transform.localPosition = Vector3.zero;
                }
                return manager;
            }
        }
        //mesh注册回调接口返回的id
        public static int meshSurfaceId;
        private static XvDeviceManager manager = null;
        
        private static Text infoTxt;
       
        void Start()
        {
            if (GameObject.Find("Canvas/Text") != null)
            {
                infoTxt = GameObject.Find("Canvas/Text").GetComponent<Text>();
            }
            Application.targetFrameRate = 60;
#if UNITY_EDITOR
            return;
#endif

            Invoke("getConfig", 0.5f);
            Invoke("hideMask", 0.8f);
           // mask = GameObject.Find("ThrowScene/Canvas/mask");
        }

        private void getConfig()
        {
            int ipdVal = API.xslam_get_glass_ipd2();
            if (ipdVal != 65000)
            {
                XvXRAndroidDevice.updateCalibra((float)(ipdVal / 10000.0));
                XvXRManager.SDK.GetDevice().setFedDis((float)(ipdVal / 10000.0));
                XvXREye.EDI = 0;
            }
            
            API.xslam_set_device_status_callback(OnDeviceStatusCallback);
        }

        private void hideMask()
        {
            //if (mask != null)
            //{
            //    mask.gameObject.SetActive(false);
            //}
        }

        [MonoPInvokeCallback(typeof(API.device_status_callback_ex))]
        public static void OnDeviceStatusCallback(API.deviceStatus_package devicesStatus)
        {
            Debug.LogError("devicesStatus.status === " + devicesStatus.status[0]);
            //if (mask != null)
            //{
            //    if (devicesStatus.status[0] == 1)
            //    {
            //        mask.gameObject.SetActive(false);
            //    }
            //    else
            //    {
            //        mask.gameObject.SetActive(true);
            //    }
            //}
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)|| Input.GetKeyUp(KeyCode.Home))
            {
                API.xslam_stop_rgb_stream();
                API.xslam_stop_imu();
                API.xslam_stop_tof_stream();
                API.xslam_stop_stereo_stream();
                API.xslam_stop_play();
                API.xslam_unset_mic_callback();
                //AndroidConnection.resetDevice();
                //AndroidProcessData.OpenPackage("com.xv.launcher");
                Application.Quit();
            }


#if UNITY_EDITOR
            return;
#endif
            if (API.xslam_ready())
            {
               
             

                if (needStartGesture)
                {
                    if (!isStartGesture)
                    {
                       
                        if (skeletonId >= 0)
                        {
                            API.xslam_stop_slam_skeleton_with_cb(skeletonType, skeletonId);
                            skeletonId = -1;
                        }


                       // API.xslam_set_gesture_filter(level);

                        skeletonId = API.xslam_start_skeleton_ex_with_cb(XvXRInput.OnSkeletonCallback);
                      
                        isStartGesture = true;
                    }
                }
            }
            else
            {
                if (XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR)
                {
                    API.xslam_init();

                }
            }
        }

        public void OnDestory()
        {

        }


        public void onDeviceConnectChanged(bool isConnected)
        {
            HMS.Engine.XvXRManager.SDK.GetDevice().isConnected = isConnected;
            HMS.Engine.XvXRManager.SDK.GetDevice().isReadFed = false;
        }


       
        public void init()
        {
           
            needStartGesture = false;
#if UNITY_ANDROID && !UNITY_EDITOR
            API.xslam_stop_stereo_stream();
            API.xslam_stop_rgb_stream();
            API.xslam_stop_tof_stream();
#endif
        }

        //public void SetFilterLevel(int level) {

        //    MyDebugTool.Log("SetFilterLevel"+ level);
        //    ChangeGetureStatus(false);
        //    this.level = level;
        //    ChangeGetureStatus(true);
        //    //Invoke("delayOpen",3);
            
        //}

        //private void delayOpen() {
        //    ChangeGetureStatus(true);
        //}

        //private int level;
        public bool ChangeGetureStatus(bool isOn)
        {
            if (API.xslam_ready())
            {
                try
                {
                    if (isOn)
                    {
                        isStartGesture = false;
                        needStartGesture = true;
                        return true;
                    }
                    else
                    {
                      
                        needStartGesture = false;
                        if (skeletonId >= 0)
                        {
                           bool close=  API.xslam_stop_slam_skeleton_with_cb(skeletonType, skeletonId);
                            MyDebugTool.Log("Stop gesture recognition:" + close);
                            skeletonId = -1;
                        }


                        return true;
                    }
                }
                catch (Exception e)
                {

                    MyDebugTool.Log("ChangeGetureStatusException" + e.Message);

                    return false;
                }
            }
            else
            {
                return false;
            }
        }




        // Update is called once per frame
        public void ReadStereoFisheyesCalibration()
        {
            XvXRLog.LogInfo("readStereoFisheyesCalibration: start....");
            API.stereo_fisheyes stereo_params = default(API.stereo_fisheyes);
            API.stereo_pdm_calibration stereo_pdm = default(API.stereo_pdm_calibration);
            int imu_fisheye_shift_us = 0;
            if (API.xslam_ready())
            {

                if (API.readStereoFisheyesCalibration(ref stereo_params, ref imu_fisheye_shift_us))
                {
                    XvXRLog.LogInfo("readStereoFisheyesCalibration:" + stereo_params);
                    for (int i = 0; i < 2; i++)
                    {
                        XvXRLog.LogInfo("readStereoFisheyesCalibration camera" + i + " Fx,Fy: " +
                        stereo_params.calibrations[i].intrinsic.K[0] + "," + stereo_params.calibrations[i].intrinsic.K[1] +
                        ",Cx,Cy:" + stereo_params.calibrations[i].intrinsic.K[2] + "," + stereo_params.calibrations[i].intrinsic.K[3] +
                        "rotation[0]:" + stereo_params.calibrations[i].extrinsic.rotation[0]);

                    }

                }
                else
                {
                    XvXRLog.LogInfo("readStereoFisheyesCalibration faild");
                }
            }

        }


    
        public bool needStartGesture = false;

       

       
        private bool isStartGesture = false;

     

      
        private int skeletonId = -1;

        private const int skeletonType = 1;


       

        private bool isFocus = false;

        void OnEnable()
        {
            isFocus = false;
        }

        private void OnApplicationPause()
        {
            quit();
        }

        public void quit()
        {
            if (isFocus == false)
            {
                isFocus = true;
            }
            else
            {
                isFocus = false;
            }
        }

        private string GetText(string file_path, string file_name)
        {
            string str_info = "62;4";
            StreamWriter sw;
            FileInfo file_info = new FileInfo(file_path + "//" + file_name);
            if (!file_info.Exists)
            {
                sw = file_info.CreateText();//创建一个用于写入 UTF-8 编码的文本  
                MyDebugTool.Log("File created successfully！");
                sw.Write(str_info);
                sw.Close();
                sw.Dispose();//文件流释放  
            }

            string result = string.Empty;
            try
            {
                FileInfo file = new FileInfo(file_path + "//" + file_name);
                result = file.OpenText().ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        void StartFeThread(){
            MyDebugTool.Log("Fisheye FE startThread...........");
            API.xslam_start_fisheyes_rectification_thread();
        }

    }
}
