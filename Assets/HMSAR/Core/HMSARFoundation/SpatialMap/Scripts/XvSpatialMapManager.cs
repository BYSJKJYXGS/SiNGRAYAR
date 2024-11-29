using AOT;
using HMS.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace XvXRFoundation
{
    /// <summary>
    /// ������Ҫʵ��Cslam���ܣ����ԶԿռ��ͼ����ɨ�����ɵ�ͼ�ļ�������ͨ����ͼ�ļ�ʵ�ֶ���Эͬ���ܵ�
    /// </summary>
    public sealed class XvSpatialMapManager : MonoBehaviour
    {
        private XvSpatialMapManager() { }


        private bool startSlam;

        /// <summary>
        /// ��һ��������map ����״̬
        /// �ڶ��������ǵ�ͼ����
        /// </summary>
        /// 
        public static UnityEvent<int,int> onMapSaveCompleteEvent=new UnityEvent<int, int>();
        /// <summary>
        /// ��һ�������ǵ�ͼ����
        /// </summary>
        public static UnityEvent<int > onMapLoadCompleteEvent=new UnityEvent<int>();

        public static UnityEvent< float> onMapMatchingEvent = new UnityEvent< float>();


        /// <summary>
        /// ��ʼ��ͼɨ��
        /// </summary>
        public void StartSlamMap()
        {

#if PLATFORM_ANDROID && !UNITY_EDITOR

            while (!API.xslam_ready())
            {
                MyDebugTool.Log("xslam_ready==false");
            }
            MyDebugTool.Log("xslam_ready==true");

#endif

            if (startSlam) {
                MyDebugTool.Log("Start SLAM multiple times");
                StopSlamMap();
            }

            if (!startSlam)
            {
              
#if UNITY_ANDROID && !UNITY_EDITOR
        API.xslam_start_map();
#endif
            }

            startSlam = true;
        }

        /// <summary>
        /// ����ɨ��ĵ�ͼ
        /// </summary>
        /// <returns></returns>
        public string SaveSlamMap()
        {
            if (startSlam)
            {
                string cslamName = GetNowStamp() + "_map.bin";

                string mapPath = Application.persistentDataPath + "/" + cslamName;
                API.xslam_save_map_and_switch_to_cslam(mapPath, OnSaveSucessCallback, OnSaveLocalized);

                return mapPath;
            }
            else {
                MyDebugTool.Log("SLAM   Not start");
            }

            return null;
        }

        /// <summary>
        /// �������е�ͼ
        /// </summary>
        /// <param name="mapPath"></param>
        public void LoadSlamMap(string mapPath)
        {
            if (!startSlam) {
                StartSlamMap();
            }

            if (startSlam)
            {
                API.xslam_load_map_and_switch_to_cslam(mapPath, OnCslamSwitched, OnLoadLocalized);
            }
        }

        /// <summary>
        /// �رյ�ͼɨɨ�蹦��
        /// </summary>
        public void StopSlamMap()
        {
            if (startSlam)
            {
                API.xslam_stop_map();
            
            }
            startSlam = false;
        }

        private void OnDestroy()
        {
            StopSlamMap();
        }
        private void OnApplicationQuit()
        {
            StopSlamMap();

        }
        /// <summary>
        /// ���鲻ҪƵ�����ã�һ��ʱ��������
        /// </summary>
        /// <returns>������ռ�����</returns>
        public List<Vector3> GetFeaturePoint()
        {
            if (startSlam)
            {
                if (!API.xslam_ready())
                {
                    return null;
                }
                int count = 0;
                IntPtr pt = API.xslam_get_slam_map(ref count);
                API.SlamMap[] objdata = new API.SlamMap[count];

                List<Vector3> pointList = new List<Vector3>();
                for (int i = 0; i < count; i++)
                {
                    IntPtr ptr = pt + i * Marshal.SizeOf(typeof(API.SlamMap));

                    objdata[i] = (API.SlamMap)Marshal.PtrToStructure(ptr, typeof(API.SlamMap));

                    Vector3 xyz = new Vector3(objdata[i].vertices[0], -objdata[i].vertices[1], objdata[i].vertices[2]);
                    pointList.Add(xyz);
                    Marshal.DestroyStructure(ptr, typeof(Vector3));
                }
                return pointList;

            }

            return null;

        }

        
        //private static float savePercent;
        //private static int save_map_quality;


        private static float similarity;
        private static int load_map_quality;

        //private static int status_of_saved_mapq;


      


        /// <summary>
        /// �����ͼ�Ļص�ʵ��
        /// </summary>
        /// <param name="status_of_saved_map"></param>
        /// <param name="map_quality"></param>
        [MonoPInvokeCallback(typeof(API.detectCslamSaved_callback))]
        static void OnSaveSucessCallback(int status_of_saved_map, int map_quality)
        {
          
            onMapSaveCompleteEvent?.Invoke(status_of_saved_map, map_quality);
        }

        /// <summary>
        /// �����ͼƥ��ȵĻص�ʵ��
        /// </summary>
        /// <param name="percentc"></param>
        [MonoPInvokeCallback(typeof(API.detectLocalized_callback))]
        static void OnSaveLocalized(float percentc)
        {
            //savePercent = percentc;
        }

        /// <summary>
        /// ���ص�ͼ�Ļص�����ʵ��
        /// </summary>
        /// <param name="map_quality"></param>
        [MonoPInvokeCallback(typeof(API.detectSwitched_callback))]
        static void OnCslamSwitched(int map_quality)
        {

            load_map_quality = map_quality;
            onMapLoadCompleteEvent?.Invoke(load_map_quality);
        }

        /// <summary>
        /// ���ص�ͼ��ƥ��ȵĻص�ʵ��
        /// </summary>
        /// <param name="percentc">0~1</param>
        [MonoPInvokeCallback(typeof(API.detectLocalized_callback))]
        static void OnLoadLocalized(float percentc)
        {
            similarity = percentc;
            onMapMatchingEvent?.Invoke( similarity);
        }


        /// <summary>
        /// ��DateTimeת����ʱ���
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long ConvertDateTimeTotTmeStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;  //��10000����Ϊ13λ   
            return t;
        }

        /// <summary>
        /// ��ȡ��ǰ��ʱ���
        /// </summary>
        /// <returns></returns>
        public long GetNowStamp()
        {
            return ConvertDateTimeTotTmeStamp(DateTime.Now);
        }
    }
}
