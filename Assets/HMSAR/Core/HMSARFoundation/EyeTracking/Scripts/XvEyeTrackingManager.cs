using AOT;
using HMS.Core;
using HMS.Engine;
using System;
using System.IO;
using UnityEngine;

namespace XvXRFoundation
{
    using static XvEyeTracking;

    /// <summary>
    /// 该类主要负责眼控的开关以及眼控数据的获取
    /// </summary>

    public sealed class XvEyeTrackingManager : MonoBehaviour
    {

        private XvEyeTrackingManager() { }
        private static string config_path = "/data/misc/xr/";
      

        //眼镜头部6dof矩阵
        Matrix4x4 MatrixHead = Matrix4x4.identity;
        //双眼中心相对于IMU的pose矩阵
        Matrix4x4 MatrixMiddleOfEyes = Matrix4x4.identity;
        //双眼中心转换到世界坐标系下的变换矩阵
        private Matrix4x4 middleOfEyeToHeadMatrix;

        public Matrix4x4 MiddleOfEyeToHeadMatrix
        {
            get { return middleOfEyeToHeadMatrix; }
        }

        /// <summary>
        /// 是否正在追踪眼球
        /// </summary>
        private static bool tracking;
        public bool Tracking
        {
            get
            {
                return tracking;
            }
        }

        public XV_ET_EYE_DATA_EX EyeData
        {
            get
            {
                return eyeData;
            }
        }

        /// <summary>
        /// 双眼原点
        /// </summary>
        public Vector3 GazeOrigin
        {
            get
            {

                return GetGazePoint(eyeData.recomGaze.gazeOrigin);
            }
        }

        /// <summary>
        /// 双眼注视点方向
        /// </summary>

        public Vector3 GazeDirection
        {
            get
            {
                return GetDirection(eyeData.recomGaze.gazeOrigin, eyeData.recomGaze.gazeDirection);
            }
        }



        /// <summary>
        /// 左眼注视点原点
        /// </summary>
        public Vector3 LeftGazeOrigin
        {
            get
            {
                return GetGazePoint(eyeData.leftGaze.gazeOrigin);
            }
        }

        /// <summary>
        /// 左眼注视点方向
        /// </summary>
        public Vector3 LeftGazeDirection
        {
            get
            {
                return GetDirection(eyeData.leftGaze.gazeOrigin, eyeData.leftGaze.gazeDirection); ;
            }
        }


        //右眼原点
        public Vector3 RightGazeOrigin
        {
            get
            {
                return GetGazePoint(eyeData.rightGaze.gazeOrigin);

            }
        }

        /// <summary>
        /// 右眼注视点方向
        /// </summary>
        public Vector3 RightGazeDirection
        {
            get
            {
                return GetDirection(eyeData.rightGaze.gazeOrigin, eyeData.rightGaze.gazeDirection); ;
            }
        }



        /// <summary>
        /// 开启眼动追踪
        /// </summary>
        public void StartGaze()
        {

#if UNITY_EDITOR
            return;
#endif
            if (!tracking)
            {
                //设置配置文件路径
                xslam_gaze_set_config_path(config_path);
                MyDebugTool.Log($"XVETinit config_path:{config_path}");

                bool b_start_gaze = xslam_start_gaze();
                MyDebugTool.Log("XVETinit b_start_gaze:" + b_start_gaze);

                string path = "/data/misc/xr/XVETcaliData_" + HMS.SystemEvents.AndroidConnection.getLoginUser() + ".dat";
                //string path = "/sdcard/XVETcaliData.dat";
                int apply = xslam_gaze_calibration_apply(path);
                MyDebugTool.Log($"XVETinit xslam_gaze_calibration_apply:{apply},path:{path}");



                bool b_set_exposure = xslam_set_exposure(10, 6, 10, 6);
                MyDebugTool.Log("XVETinit b_set_exposure:" + b_set_exposure);
                bool b_set_bright = xslam_set_bright(2, 8, 27);
                MyDebugTool.Log("XVETinit b_set_bright:" + b_set_bright);

                int b_set_gaze_callback = xslam_set_gaze_callback(OnStartSkeletonCallback);
                MyDebugTool.Log("XVETinit b_set_gaze_callback:" + b_set_gaze_callback);


            }
        }


        /// <summary>
        /// 停止眼动追踪
        /// </summary>
        public void StopGaze()
        {
#if UNITY_EDITOR
            return;
#endif

            if (Tracking)
            {

                bool b_set_bright = xslam_set_bright(2, 8, 0);
                MyDebugTool.Log("XVETmanager b_set_bright:" + b_set_bright);
                bool unset = UnsetGazeCallback();
                MyDebugTool.Log($"XVETmanager xslam_unset_gaze_callback:{unset}");
                bool b = xslam_stop_gaze();
                MyDebugTool.Log($"XVETmanager xslam_stop_gaze:{b}");
                tracking = false;
            }
        }

        public static XV_ET_EYE_DATA_EX eyeData = new XV_ET_EYE_DATA_EX();

        [MonoPInvokeCallback(typeof(fn_gaze_callback))]
        private static void OnStartSkeletonCallback(XV_ET_EYE_DATA_EX gazedata)
        {
            tracking = true;
            MyDebugTool.Log($"OnStartSkeletonCallback");
            eyeData = gazedata;

            MyDebugTool.Log($"XVETmanager ipd:{gazedata.ipd}");
            MyDebugTool.Log($"XVETmanager eyeData leftEyeMove:{eyeData.leftEyeMove}");
            MyDebugTool.Log($"XVETmanager eyeData rightEyeMove:{eyeData.rightEyeMove}");
        }


        private void Update()
        {

            if (!Tracking)
            {
                return;
            }

            UpdateMatrix();

        }

        private void UpdateMatrix()
        {
            MatrixHead.SetTRS(Camera.main.transform.position, Camera.main.transform.rotation, Vector3.one);

            API.stereo_pdm_calibration fed = XvXRManager.SDK.GetDevice().GetFed();

            Vector3 LeftdisplayPos = new Vector3((float)fed.calibrations[0].extrinsic.translation[0], -(float)fed.calibrations[0].extrinsic.translation[1], (float)fed.calibrations[0].extrinsic.translation[2]);
            Vector3 RightdisplayPos = new Vector3((float)fed.calibrations[1].extrinsic.translation[0], -(float)fed.calibrations[1].extrinsic.translation[1], (float)fed.calibrations[1].extrinsic.translation[2]);

            Vector3 normal = (RightdisplayPos - LeftdisplayPos).normalized;
            float distance = Vector3.Distance(LeftdisplayPos, RightdisplayPos);
            Vector3 middleOfEyes_pos = normal * (distance * 0.5f) + LeftdisplayPos;


            DebugLog($"middleOfEyes_pos:" + middleOfEyes_pos);

            Quaternion middleQua = RotationMatrixToQuaternion(fed.calibrations[0].extrinsic.rotation);
            MatrixMiddleOfEyes.SetTRS(middleOfEyes_pos, new Quaternion(-middleQua.x, middleQua.y, -middleQua.z, middleQua.w), Vector3.one);

            middleOfEyeToHeadMatrix = MatrixHead * MatrixMiddleOfEyes;
        }

        private Vector3 GetGazePoint(XV_ETPoint3D oGazeOrigin)
        {
            Vector3 gazeOrigin = new Vector3(oGazeOrigin.x, -oGazeOrigin.y, oGazeOrigin.z) / 1000;



            Matrix4x4 Matrix_gazeOrigin = Matrix4x4.identity;
            Matrix4x4 Matrix_XVgazeOrigin = Matrix4x4.identity;
            Matrix_gazeOrigin.SetTRS(gazeOrigin, Quaternion.identity, Vector3.one);
            Matrix_XVgazeOrigin = middleOfEyeToHeadMatrix * Matrix_gazeOrigin;

            ///获取位置
            gazeOrigin = Matrix_XVgazeOrigin.GetColumn(3);

            return gazeOrigin;

        }
        private Vector3 GetDirection(XV_ETPoint3D oGazeOrigin, XV_ETPoint3D oGazeDirection)
        {
            Vector3 gazeOrigin = new Vector3(oGazeOrigin.x, -oGazeOrigin.y, oGazeOrigin.z) / 1000;
            Vector3 gazeDirection = gazeOrigin + new Vector3(oGazeDirection.x, -oGazeDirection.y, oGazeDirection.z) * 15;
            Matrix4x4 Matrix_target = Matrix4x4.identity;
            Matrix4x4 Matrix_XVtarget = Matrix4x4.identity;

            Matrix_target.SetTRS(gazeDirection, Quaternion.identity, Vector3.one);

            Matrix_XVtarget = middleOfEyeToHeadMatrix * Matrix_target;
            //获取新的方向向量
            gazeDirection = (Vector3)Matrix_XVtarget.GetColumn(3) - GetGazePoint(oGazeOrigin);

            return gazeDirection;

        }

        private void OnDestroy()
        {
            StopGaze();
        }
        private void OnApplicationQuit()
        {
            StopGaze();
        }

        //旋转矩阵转换成四元数
        private Quaternion RotationMatrixToQuaternion(double[] rm)
        {
            double w, x, y, z;
            //if (1 + rm[0] + rm[4] + rm[8]>0&& (Math.Sqrt(1 + rm[0] + rm[4] + rm[8]) / 2)!=0)
            {
                w = Math.Sqrt(1 + rm[0] + rm[4] + rm[8]) / 2;
                x = (rm[7] - rm[5]) / (4 * w);
                y = (rm[2] - rm[6]) / (4 * w);
                z = (rm[3] - rm[1]) / (4 * w);
            }
            Quaternion qua = new Quaternion((float)x, (float)y, (float)z, (float)w);
            return qua;
        }

        private void DebugLog(object msg)
        {
             MyDebugTool.Log(msg);
        }



        #region 眼控校准

        /// <summary>
        /// 进入校准模式
        /// </summary>
        public void GazeCalibrationEnter()
        {
            XvEyeTracking.xslam_gaze_calibration_enter();
        }

        /// <summary>
        /// 采集校准点位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GazeCalibrationCollect(Vector3 point, int index)
        {
            return XvEyeTracking.xslam_gaze_calibration_collect(point.x, point.y, point.z, index);
        }

        /// <summary>
        /// 重置校准数据
        /// </summary>
        /// <returns></returns>
        public bool UnsetGazeCallback()
        {
            return XvEyeTracking.xslam_unset_gaze_callback();
        }
        /// <summary>
        /// 校准完成
        /// </summary>
        public int CalibrationComplete()
        {
            int setup = xslam_gaze_calibration_setup();
            MyDebugTool.Log($" xslam_gaze_calibration_setup:{setup}");

            int compute_apply = xslam_gaze_calibration_compute_apply();
            MyDebugTool.Log($" xslam_gaze_calibration_compute_apply:{compute_apply}");

            int leave = xslam_gaze_calibration_leave();
            MyDebugTool.Log($" xslam_gaze_calibration_leave:{leave}");

            string path = "/data/misc/xr/XVETcaliData_" + HMS.SystemEvents.AndroidConnection.getLoginUser() + ".dat";//眼动校准完成，校准文件保存的路径
                                                                                                                      //string path = "/sdcard/XVETcaliData.dat";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            int retrieve = xslam_gaze_calibration_retrieve(path);
            MyDebugTool.Log($" xslam_gaze_calibration_retrieve:{retrieve}");

            //设置保存的校准文件权限为可读，确保第三方应用可以读取并使用
            sys_chmod(path, _0755);
            return retrieve;

        }

        // user permissions
        const int S_IRUSR = 0x100;
        const int S_IWUSR = 0x80;
        const int S_IXUSR = 0x40;

        // group permission
        const int S_IRGRP = 0x20;
        const int S_IWGRP = 0x10;
        const int S_IXGRP = 0x8;

        // other permissions
        const int S_IROTH = 0x4;
        const int S_IWOTH = 0x2;
        const int S_IXOTH = 0x1;

        const int _0755 = S_IRUSR | S_IXUSR | S_IWUSR | S_IRGRP | S_IXGRP | S_IROTH | S_IXOTH;
        [System.Runtime.InteropServices.DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
        private static extern int sys_chmod(string path, int mode);
        #endregion
    }
}