using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using AOT;
using XvXRFoundation;
using System.Reflection;
using HMS.Core;

namespace XvXR.MixedReality.Toolkit.XvXR.Input
{

    public class bleInfo
    {
        public string id;
        public string mac;
        public int status;
        public string serialNumber;
    }

    public partial class XvXRJoystick : SingletonBehaviour<XvXRJoystick>
    {

        const string TAG = "XvXRJoystick";

        public enum DataSource
        {
            BLE_TYPE_AC = 1,
            BLE_TYPE_B = 2,
            ANDROID_SENSOR = 3,
            BLE_TYPE_I500 = 4,
        }

        private AndroidJavaObject mAndroidBle;
        private JoystickData mJoystickData = new JoystickData();
        private JoystickData mSyncData = new JoystickData();
        [SerializeField]
        private DataSource mHandleMode = DataSource.BLE_TYPE_I500;
        [SerializeField]

        private TrackerType trackerType;
        public TrackerType TrackerType
        {
            get
            {
                return trackerType;
            }
        }




        private bool mConnect = false;
        private List<string> bleList = new List<string>();

        private static List<string> connectBlList = new List<string>();





        bool mXvBleInit = false;
        private BlePoseListener mBlePoseListener;

        private List<bleInfo> bleInfos = new List<bleInfo>();
        public List<bleInfo> GetBleInfo()
        {
            bleInfos.Clear();
            for (int i = 0; i < bleList.Count; i++)
            {
                if (bleList[i].Trim().Contains("xv"))
                {
                    bleInfo bleInfo = new bleInfo();
                    bleInfo.id = bleList[i].Split('#')[0];
                    bleInfo.mac = bleList[i].Split('#')[1];
                    bleInfo.serialNumber = bleList[i];
                    for (int j = 0; j < connectBlList.Count; j++)
                    {
                        if (connectBlList[j] == bleList[i].Split('#')[1])
                        {
                            bleInfo.status = 1;
                        }
                    }
                    bleInfos.Add(bleInfo);


                }
            }
            return bleInfos;
        }

        public class JoystickData
        {
            public Vector3 position;
            public Vector3 rotation;
            public Quaternion quaternion;
            public int confidence;
            public bool keyTrigger;

            public int keyMenu;
            public bool keyRocker;
            public bool keySlide;
            public bool keyA;
            public bool keyB;
            public int keyRockerX;
            public int keyRockerY;

            public void Copy(JoystickData data)
            {
                position = data.position;
                rotation = data.rotation;
                quaternion = data.quaternion;
                confidence = data.confidence;
                keyTrigger = data.keyTrigger;

                keyMenu = data.keyMenu;
                keyRocker = data.keyRocker;
                keySlide = data.keySlide;
                keyA = data.keyA;
                keyB = data.keyB;
                keyRockerX = data.keyRockerX;
                keyRockerY = data.keyRockerY;
            }
        }

        public static XvXRJoystick GetInstance()
        {
            return SingletonBehaviour<XvXRJoystick>.Instance;
        }

        public bool IsReady()
        {
            return mConnect;
        }

        public void SetReady(bool connect)
        {


            mConnect = connect;

        }



        AndroidJavaObject GetUnityActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return unityActivity;
        }

        class BlePoseListener : AndroidJavaProxy
        {
            private JoystickData mData;
            private Boolean mConnect;


            public BlePoseListener() : base("org.xv.xvsdk.ext.ble.IPoseListener") { }


            public void SetJoystickData(JoystickData data)
            {
                mData = data;
            }

            void onScan(string bleInfo)
            {
                Utility.Log(TAG, "onScan bleInfo:" + bleInfo);
                if (!GetInstance().bleList.Contains(bleInfo))
                {
                    if (bleInfo.Contains("xv"))
                        GetInstance().bleList.Add(bleInfo);
                }


            }
        }




        public JoystickData GetJoystickData()
        {
            return mJoystickData;
        }


        void Start()
        {

#if !UNITY_EDITOR
            StartBle();
#endif
        }

        /// <summary>
        /// 初始化蓝牙 java环境
        /// </summary>
        void StartBle()
        {
            AndroidJavaObject unityActivity = GetUnityActivity();
            AndroidJavaClass clazz = new AndroidJavaClass("org.xv.xvsdk.ext.ble.BleController");
            mAndroidBle = clazz.CallStatic<AndroidJavaObject>("getInstance", unityActivity);

            mBlePoseListener = new BlePoseListener();
            mBlePoseListener.SetJoystickData(mSyncData);
            mAndroidBle.Call<bool>("start", (int)mHandleMode, mBlePoseListener);
        }

        /// <summary>
        /// 获取蓝牙信息列表状态
        /// </summary>
        /// <param name="blName"></param>
        /// <param name="blMac"></param>
        /// <param name="state"></param>
        [MonoPInvokeCallback(typeof(API.WirelessStateCallback))]
        static void OnWirelessStateCallback(IntPtr blName, IntPtr blMac, int state)
        {
            string sName = Marshal.PtrToStringAnsi(blName);
            string sMac = Marshal.PtrToStringAnsi(blMac);
            if (state == 1)
            {
                connectBlList.Add(sMac);
            }
            else
            {
                connectBlList.Remove(sMac);
            }
        }


        /// <summary>
        /// 获取蓝牙手柄数据
        /// </summary>
        /// <param name="data"></param>
        [MonoPInvokeCallback(typeof(API.WirelessPoseCallback))]
        static void OnWirelessPoseCallback(ref API.WirelessPos data)
        {
            Quaternion q = new Quaternion((float)data.quaternion.x, (float)data.quaternion.y, (float)data.quaternion.z, (float)data.quaternion.w);
            GetInstance().mSyncData.position = new Vector3(data.position.x, -data.position.y, data.position.z);
            Vector3 rotation = new Vector3(q.eulerAngles.x * -1.0f, q.eulerAngles.y, q.eulerAngles.z * -1.0f);
            GetInstance().mSyncData.rotation = rotation;
            GetInstance().mSyncData.quaternion = q;

            GetInstance().mSyncData.keyRockerX = data.rocker_x;
            GetInstance().mSyncData.keyRockerY = data.rocker_y;

            GetInstance().mSyncData.keyA = data.keyA == 1;
            GetInstance().mSyncData.keyB = data.keyB == 1;
            GetInstance().mSyncData.keySlide = data.keySide > 50;
            GetInstance().mSyncData.keyTrigger = data.keyTrigger > 50;




            GetInstance().SetReady(true);
        }

        /// <summary>
        /// 获取蓝牙列表信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mac"></param>
        [MonoPInvokeCallback(typeof(API.wirelessScanCallback))]
        static void OnWirelessScanCallback(IntPtr name, IntPtr mac)
        {
            string sName = Marshal.PtrToStringAnsi(name);
            string sMac = Marshal.PtrToStringAnsi(mac);
            string bleInfo = sName + "#" + sMac;
            Utility.Log(TAG, "OnWirelessScanCallback :" + bleInfo);

            if (!GetInstance().bleList.Contains(bleInfo))
            {
                if (bleInfo.Contains("xv"))
                    GetInstance().bleList.Add(bleInfo);
            }
        }

        /// <summary>
        /// 连接对应蓝牙设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mac"></param>
        public void ConnectXvBle(string name, string mac)
        {
            Utility.Log(TAG, "wirelessConnect name:" + name + " mac:" + mac);
            API.xv_wireless_connect(name, mac);
        }

        /// <summary>
        /// 断开对应的蓝牙设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mac"></param>
        public void DisConnectXvBle(string name, string mac)
        {
            Utility.Log(TAG, "wirelessDisConnect name:" + name + " mac:" + mac);
            API.xv_wireless_disconnect(name, mac);
        }

        /// <summary>
        /// 注册蓝牙相关回调
        /// </summary>
        void StartXvBle()
        {
            if (mXvBleInit)
            {
                return;
            }

            API.xv_wireless_start();//开启接受信息
            API.xv_wireless_scan(OnWirelessScanCallback);//获取蓝牙列表
            API.xv_wireless_register(OnWirelessPoseCallback);//手柄状态 pose
            API.xv_wireless_register_state(OnWirelessStateCallback);//蓝牙连接状态
            Utility.Log(TAG, "StartXvBle");
            mXvBleInit = true;
        }



        // Update is called once per frame
        void Update()
        {
#if !UNITY_EDITOR
            if (API.xslam_ready())
            {
                StartXvBle();
            }

            mJoystickData.Copy(mSyncData);

        
#endif
        }
    }
}