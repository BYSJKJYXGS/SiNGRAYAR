using HMS.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using HMS.Engine;


namespace XvXRFoundation
{
    /// <summary>
    /// 该类主要负责空面里面平面和垂面的识别
    /// </summary>

    public sealed class XvPlaneManager : MonoBehaviour
    {
        private XvPlaneManager() { }
        public event Action<plane[]> planesChanged;

        private bool isDetecting;


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

        private float gapTime;

        /// <summary>
        /// 
        /// </summary>
        public void StartPlaneDetction() {
            if (!isDetecting) {

#if !PLATFORM_ANDROID || UNITY_EDITOR
                return;
#endif

            while (!API.xslam_ready())
            {
                MyDebugTool.Log("xslam_ready==false");
            }
            MyDebugTool.Log("xslam_ready==true");

               
                CameraManager.StartCapture(XvCameraStreamType.TofDepthCameraStream);


            API.xslam_tof_set_framerate(5);
            API.xslam_start_detect_plane_from_tof_nosurface();
            isDetecting = true;
            }

        }

        public void StopPlaneDetction() {
#if !PLATFORM_ANDROID || UNITY_EDITOR
            return;
#endif

            API.xslam_stop_detect_plane_from_tof();
            isDetecting = false;

        }

        private void Update()
        {
            if (isDetecting)
            {
                gapTime += Time.deltaTime;

                if (gapTime >= 3)
                {
                    plane[] planes = GetPlane();
                    if (planes != null)
                    {
                        planesChanged?.Invoke(planes);
                    }

                    gapTime = 0;
                }

            }
        }
       


      

        private plane[] GetPlane()
        {
           int len = 1024 * 64;
            byte[] rdata = new byte[len];
            GCHandle rh = GCHandle.Alloc(rdata, GCHandleType.Pinned);
            bool ret = API.xslam_get_plane_from_tof(rh.AddrOfPinnedObject(), ref len);
            rh.Free();
            if (ret)
                return ParsePlane(rdata, len);
            else
                return null;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="rdata">平面数据</param>
       /// <param name="len">数据总长度</param>
       /// <returns></returns>
        private plane[] ParsePlane(byte[] rdata, int len)
        {
            plane[] planes = new plane[0];
            if (len < 4)
                return null;

            int pos = 0;
           
            int nPlane = BitConverter.ToInt32(rdata, pos);
            pos += 4;
            if (nPlane > 0 && len > pos)
            {
                planes = new plane[nPlane];
                for (int i = 0; i < nPlane; i++)
                {
                    if (len <= pos)
                        break;

                    planes[i] = new plane();
                    
                    int nPoint = BitConverter.ToInt32(rdata, pos); pos += 4;
                    planes[i].points = new List<Vector3D>();
                    for (int j = 0; j < nPoint; j++)
                    {
                        Vector3D point;
                        point.X = BitConverter.ToDouble(rdata, pos); pos += 8;
                        point.Y = BitConverter.ToDouble(rdata, pos); pos += 8;
                        point.Z = BitConverter.ToDouble(rdata, pos); pos += 8;
                        planes[i].points.Add(point);
                    }
                    planes[i].normal.X = BitConverter.ToDouble(rdata, pos); pos += 8;
                    planes[i].normal.Y = BitConverter.ToDouble(rdata, pos); pos += 8;
                    planes[i].normal.Z = BitConverter.ToDouble(rdata, pos); pos += 8;
                    planes[i].d = BitConverter.ToDouble(rdata, pos); pos += 8;
                    int idLen = BitConverter.ToInt32(rdata, pos); pos += 4;
                    planes[i].id = BitConverter.ToString(rdata, pos, idLen); pos += idLen;
                }
                return planes;
            }
            return null;
        }

        private void OnDestroy()
        {
            StopPlaneDetction();
        }

        private void OnApplicationQuit()
        {
            StopPlaneDetction();
        }
    }
    public struct Vector3D
    {
        public double X;
        public double Y;
        public double Z;
    };


    public class plane
    {
        public List<Vector3D> points;//平面顶点坐标
        public Vector3D normal;//平面法线
        public double d;
        public string id;//平面ID
    };
}
