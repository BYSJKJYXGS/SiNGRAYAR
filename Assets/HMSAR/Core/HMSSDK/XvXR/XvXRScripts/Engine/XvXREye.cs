using UnityEngine;
using System.Collections;
using HMS.Utils;
using System;
using System.Collections.Generic;
using HMS.Core;

namespace HMS.Engine
{
    [RequireComponent(typeof(Camera))]
    public class XvXREye : MonoBehaviour
    {

        public XvXRManager.Eye eye;
        public LayerMask toggleCullingMask = 0;

        public static int EDI = 2;
        public static double EyeDistance = 0;

        //左眼旋转矩阵（数组）与位移矩阵（数组）
        // eye rotation matrix (array) and transform matrix (array)
        private double[] _R;
        private double[] _T;
        //左眼的欧拉角
        private double[] _EulerAngles;

        private XvXRStereoController mController;
        private List<Transform> mTransformsList =   new List<Transform>();

        public XvXRStereoController Controller
        {

            get
            {
                if (transform.parent == null) { return null; }
                if ((XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR == XvXRSdkConfig.XvXR_PLATFORM && !Application.isPlaying) || mController == null)
                {
                    mController= transform.parent.GetComponentInParent<XvXRStereoController>();
                }
                return mController;
            }

        }

        public XvXRHeadTracking Head
        {
            get
            {
                return GetComponentInParent<XvXRHeadTracking>();
            }
        }
        private Camera monoCamera;

        new public Camera camera { get; private set; }

        void Awake()
        {
            camera = GetComponent<Camera>();
            camera.backgroundColor = new Color(0F, 0F, 0F, 1F);
            camera.clearFlags = CameraClearFlags.Skybox;
            UnityEngine.Object[] mRenderObject;
            UnityEngine.Object[] mCanvasRenderObject;
            mRenderObject = UnityEngine.Object.FindObjectsOfType(typeof(MeshRenderer));
            float IndividualDist = 0.14f;
            mCanvasRenderObject = UnityEngine.Object.FindObjectsOfType(typeof(CanvasRenderer));
            float canIndDist = 0.5f;




            for (int i = 0; i < mRenderObject.Length; i++)
            {
                GameObject tGO = GameObject.Find(mRenderObject[i].name);
                Vector3 tV3 = tGO.transform.position;

                bool IsOverlay = false;
                for (int j = 0; j < mTransformsList.Count; j++)
                {
                    if ((mTransformsList[j].position - tV3).magnitude < IndividualDist)
                    {
                        IsOverlay = true;
                        break;
                    }
                }
                if (IsOverlay == false)
                {
                    mTransformsList.Add(tGO.transform);
                }
            }

            if(mTransformsList.Count ==  0)
            {
                for (int i = 0; i < mCanvasRenderObject.Length; i++)
                {
                    GameObject tGO = GameObject.Find(mCanvasRenderObject[i].name);
                    Vector3 tV3 = tGO.transform.position;

                    bool IsOverlay = false;
                    for (int j = 0; j < mTransformsList.Count; j++)
                    {
                        if ((mTransformsList[j].position - tV3).magnitude < canIndDist)
                        {
                            IsOverlay = true;
                            break;
                        }
                    }
                    if (IsOverlay == false)
                    {
                        mTransformsList.Add(tGO.transform);
                    }
                }
            }

        }


        void Start()
        {
#if UNITY_EDITOR_WIN
            this.gameObject.SetActive(false);
            return;
#endif
            var ctlr = Controller;
            if (ctlr == null)
            {
                XvXRLog.InternalXvXRLog("vreye must be child of a stereocontroller.");
                enabled = false;
            }
            //XvXRLog.LogError("this game object name is :" + this.gameObject.name);

 
            monoCamera = Controller.GetComponent<Camera>();
          
            UpdateStereoValues();

        }


        //void OnPreCull()
        //{
        //    SetupStereo();
        //}

        /// <summary>
        /// 更新相关内外参数并赋值给左右两个camera(unity camera)，从glass里获取显示标定参数
        /// Camera.onPreCull和start()触发调用,用来在Camera剔除场景之前执行自定义代码。
        /// </summary>
        public void UpdateStereoValues()//原vr
        {
#if UNITY_EDITOR_WIN
           // return;
#endif
            if (XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR || XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_IOS)
            {
                camera = GetComponent<Camera>();
                monoCamera = Controller.GetComponent<Camera>();
            }

            if (closeVrMode)
            {
                camera.CopyFrom(monoCamera);
                camera.targetTexture = null;

            }
            else
            {

              if(XvXRManager.SDK.GetDevice().isConnected && XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_ANDROID )
                {

               
                API.stereo_pdm_calibration fed_ = default(API.stereo_pdm_calibration);

                //     XvXRLog.InternalXvXRLog("camera is :" + (camera == null) + ",monocamera is:" + (monoCamera == null));
                if (XvXRManager.SDK.GetDevice().isReadFed == false)
                {
                        //从glass获取显示标定参数fed,fed会在ComputeEyesFromProfile()计算投影矩阵时使用
                        if (XvXRAndroidDevice.readStereoDisplayCalibration(ref fed_))
                        {
                            XvXRLog.LogInfo("readStereoDisplayCalibration:" + fed_);
                            XvXRManager.SDK.GetDevice().SetFed(fed_);
                            XvXRManager.SDK.GetDevice().isReadFed = true;
                            // 更新参数并计算投影矩阵设置到android java库
                            XvXRManager.SDK.GetDevice().UpdateScreenData();
                        }
                        else
                        {
                            XvXRLog.LogInfo("readStereoDisplayCalibration faild");
                        }
                }
                }
				  else if(XvXRManager.SDK.GetDevice().isConnected && XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR)
                {

                    API.stereo_pdm_calibration fed_ = default(API.stereo_pdm_calibration);

                    if (XvXRManager.SDK.GetDevice().isReadFed == false)
                    {

                        if (XvXRUnityEditorDevice.ReadStereoDisplayCalibration(ref fed_))
                        {
                            XvXRLog.LogInfo("ReadStereoDisplayCalibration:" + fed_);
                            XvXRManager.SDK.GetDevice().SetFed(fed_);
                            XvXRManager.SDK.GetDevice().isReadFed = true;
                            XvXRManager.SDK.GetDevice().UpdateScreenData();
                        }
                        else
                        {
                            XvXRLog.LogInfo("ReadStereoDisplayCalibration faild");
                        }
                    }
                }

                Matrix4x4 proj = XvXRManager.SDK.Projection(eye);

               
                camera.CopyFrom(monoCamera);


                if (XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR == XvXRSdkConfig.XvXR_PLATFORM)
                {
                    camera.fieldOfView = 2 * Mathf.Atan(1 / proj[1, 1]) * Mathf.Rad2Deg;
                }


                //camera.cullingMask ^= toggleCullingMask.value;
                camera.depth = monoCamera.depth;

                camera.projectionMatrix = proj;

             

                API.stereo_pdm_calibration fed= XvXRManager.SDK.GetDevice().GetFed();

                //眼镜标定数据的使用（眼镜左右眼是正常的顺序）
                if (eye == XvXRManager.Eye.Left)
                {

                    if (XvXRManager.SDK.GetDevice().isReadFed)
                    {
                        _T = new double[3] { fed.calibrations[0].extrinsic.translation[0], -fed.calibrations[0].extrinsic.translation[1], fed.calibrations[0].extrinsic.translation[2] };
                        //左眼标定的旋转矩阵→欧拉角
                        _R = new double[9] { fed.calibrations[0].extrinsic.rotation[0], -fed.calibrations[0].extrinsic.rotation[1], fed.calibrations[0].extrinsic.rotation[2], -fed.calibrations[0].extrinsic.rotation[3], fed.calibrations[0].extrinsic.rotation[4],
                                     -fed.calibrations[0].extrinsic.rotation[5],fed.calibrations[0].extrinsic.rotation[6],-fed.calibrations[0].extrinsic.rotation[7],fed.calibrations[0].extrinsic.rotation[8]};
                        RotationMatrixToEulerAngles(ref _EulerAngles, _R);

                        //眼镜的外参设置（左眼相机）
                        transform.localPosition = new Vector3((float)_T[0], (float)_T[1], (float)_T[2]);

                        transform.localEulerAngles = new Vector3((float)_EulerAngles[0], (float)_EulerAngles[1], (float)_EulerAngles[2]);

                    }
                    else
                    {

                    }

                    if (XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR == XvXRSdkConfig.XvXR_PLATFORM)
                    {
                        camera.fieldOfView = 2 * Mathf.Atan(1 / proj[1, 1]) * Mathf.Rad2Deg;
                        monoCamera.fieldOfView = 2 * Mathf.Atan(1 / proj[1, 1]) * Mathf.Rad2Deg;
                        monoCamera.projectionMatrix = proj;

                    }



                }
                else if (eye == XvXRManager.Eye.Right)
                {

                    if (XvXRManager.SDK.GetDevice().isReadFed)
                    {
                        //右眼相机
                        _T = new double[3] { fed.calibrations[1].extrinsic.translation[0], -fed.calibrations[1].extrinsic.translation[1], fed.calibrations[1].extrinsic.translation[2] };
                        _R = new double[9] { fed.calibrations[1].extrinsic.rotation[0], -fed.calibrations[1].extrinsic.rotation[1], fed.calibrations[1].extrinsic.rotation[2], -fed.calibrations[1].extrinsic.rotation[3], fed.calibrations[1].extrinsic.rotation[4],
                                     -fed.calibrations[1].extrinsic.rotation[5],fed.calibrations[1].extrinsic.rotation[6],-fed.calibrations[1].extrinsic.rotation[7],fed.calibrations[1].extrinsic.rotation[8]};
                       
                        RotationMatrixToEulerAngles(ref _EulerAngles, _R);


                        //眼镜的外参设置（右眼相机）
                        transform.localPosition = new Vector3((float)_T[0], (float)_T[1], (float)_T[2]);

                        transform.localEulerAngles = new Vector3((float)_EulerAngles[0], (float)_EulerAngles[1], (float)_EulerAngles[2]);

                    }
                    else
                    {

                    }

                  
                }


                transform.localScale = Vector3.one;


                if (XvXRSdkConfig.sdkUseMode == XvXRSdkConfig.SDK_MODE.XvXR_UNITY_CLIENT_MODE && XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR)
                {

                    //Rect rect = camera.rect;
                    //Vector2 center = rect.center;
                    //center.x = Mathf.Lerp(center.x, 0.5f, 0);
                    //center.y = Mathf.Lerp(center.y, 0.5f, 0);
                    //rect.center = center;

                    //float width = Mathf.SmoothStep(-0.5f, 0.5f, (rect.width + 1) / 2);
                    //rect.x += (rect.width - width) / 2;
                    //rect.width = width;
                    //rect.x *= (0.5f - rect.width) / (1 - rect.width);

                    //if (eye == BvrManager.Eye.Right)
                    //{
                    //    rect.x += 0.5f; // Move to right half of the screen.
                    //}

                    camera.targetTexture = monoCamera.targetTexture;
                    //camera.rect = rect;
                    camera.targetTexture = (eye == XvXRManager.Eye.Left ? XvXRManager.SDK.StereoScreen[0] : XvXRManager.SDK.StereoScreen[1]);

                }
                else
                {

                    if (XvXRManager.DistortionCorrectionMethod.None == XvXRManager.SDK.DistortionCorrection || XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_EDITOR)
                    {


                        Rect rect = camera.rect;
                        Vector2 center = rect.center;
                        center.x = Mathf.Lerp(center.x, 0.5f, 0);
                        center.y = Mathf.Lerp(center.y, 0.5f, 0);
                        rect.center = center;

                        float width = Mathf.SmoothStep(-0.5f, 0.5f, (rect.width + 1) / 2);
                        rect.x += (rect.width - width) / 2;
                        rect.width = width;
                        rect.x *= (0.5f - rect.width) / (1 - rect.width);

                        if (eye == XvXRManager.Eye.Right)
                        {
                            rect.x += 0.5f; // Move to right half of the screen.
                        }

                        camera.targetTexture = monoCamera.targetTexture;
                        camera.rect = rect;
                    }
                    else
                    {

                        camera.targetTexture = (eye == XvXRManager.Eye.Left ? XvXRManager.SDK.StereoScreen[0] : XvXRManager.SDK.StereoScreen[1]);

                    }

                }


            }

        }
        bool closeVrMode = false;
        internal void CloseVrMode()
        {
            closeVrMode = true;
            XvXRManager.SDK.UpdateState();
            UpdateStereoValues();
        }

        public void SetupStereo()
        {
            closeVrMode = false;
            XvXRManager.SDK.UpdateState();
            UpdateStereoValues();
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
           

            if (null != destTexture)
            {
                Graphics.Blit(sourceTexture, destTexture);
            }
            
        }
#endif

        private void Update()
        {
            /*Camera myCamera = GetComponent<Camera>();
            
            if (eye == XvXRManager.Eye.Left)
            {
                myCamera.cullingMask = 1 << 8;
            }
            else
            {
                myCamera.cullingMask = 1 << 9;
            }
            */


            //if (eye == XvXRManager.Eye.Left) { return; }

            //XvXRManager.setTexture();

            //camera.targetTexture = (eye == XvXRManager.Eye.Left ? XvXRManager.SDK.StereoScreen[0] : XvXRManager.SDK.StereoScreen[1]);
            bool isLeft = eye == XvXRManager.Eye.Left ? true : false;
     
            camera.targetTexture = XvXRManager.GetTexture(isLeft);


           // Debug.LogError("shudan Object count:" + mTransformsList.Count);
            if (isLeft)
            {
                int tCount = 0;
                float sumDis = 0.0f;

                for (int i = 0;i< mTransformsList.Count; i++) 
                {
                    Vector3 dir = (mTransformsList[i].position - this.transform.position).normalized;
                    float dot = Vector3.Dot(this.transform.forward, dir);
                    //float dotX = Vector2.Dot(new Vector2(this.transform.forward.x, this.transform.forward.z), new Vector2(dir.x, dir.z));
                    //float dotY = Vector2.Dot(new Vector2(this.transform.forward.y, this.transform.forward.z), new Vector2(dir.y, dir.z));

                    //if (dotX >= XvXRManager.SDK.GetDevice().CosX - 0.01 && dotY >= XvXRManager.SDK.GetDevice().CosY - 0.01)
                    //{

                    //    tCount++;
                    //    sumDis += (this.transform.position - mTransformsList[i].position).magnitude;
                    //}
                    if (dot > 0.1f)
                    {
                        Vector2 viewPos = this.GetComponent<Camera>().WorldToViewportPoint(mTransformsList[i].position);
                        if (viewPos.x >= 0.0f && viewPos.x <= 1.0f && viewPos.y >= 0.0f && viewPos.y <= 1.0f)
                        {
                            tCount++;
                            sumDis += (this.transform.position - mTransformsList[i].position).magnitude;
                        }
                    }

                }
                XvXRManager.SetUpdateTexture(sumDis, tCount);
            }
            if (EDI < 2)
            {
                //Debug.LogError("set localPosition:" + transform.localPosition[0] + transform.localPosition[1] + transform.localPosition[2]);
                API.stereo_pdm_calibration fed = XvXRManager.SDK.GetDevice().GetFed();
                //double oldDis = (fed.calibrations[1].extrinsic.translation[0] - fed.calibrations[0].extrinsic.translation[0] - EyeDistance) / 2.0;
                //double leftT = fed.calibrations[0].extrinsic.translation[0] - oldDis;
                //double ringhtT = fed.calibrations[1].extrinsic.translation[0] + oldDis;

                if (eye == XvXRManager.Eye.Left)
                {

                    //眼镜的外参设置（左眼相机）
                    transform.localPosition = new Vector3((float)fed.calibrations[0].extrinsic.translation[0], (float)-fed.calibrations[0].extrinsic.translation[1],(float) fed.calibrations[0].extrinsic.translation[2]);

                    EDI++;
                    //Debug.LogError("set localPosition left:" + transform.localPosition[0] + transform.localPosition[1] + transform.localPosition[2]);
                }
                else if (eye == XvXRManager.Eye.Right)
                {
                    transform.localPosition = new Vector3((float)fed.calibrations[1].extrinsic.translation[0],(float) -fed.calibrations[1].extrinsic.translation[1], (float)fed.calibrations[1].extrinsic.translation[2]);

                    EDI++;
                    //Debug.LogError("set localPosition Right:" + transform.localPosition[0] + transform.localPosition[1] + transform.localPosition[2]);
                }
            }
        }


        //旋转矩阵转换成欧拉角
        internal static void RotationMatrixToEulerAngles(ref double[] eulerAngle, double[] rm)
        {

            double sy = Math.Sqrt(rm[0] * rm[0] + rm[3] * rm[3]);

            bool singular = sy < 1e-6; // If

            double x, y, z;
            if (!singular)
            {
                x = Math.Atan2(rm[7], rm[8]);
                y = Math.Atan2(-rm[6], sy);
                z = Math.Atan2(rm[3], rm[0]);
            }
            else
            {
                x = Math.Atan2(-rm[5], rm[4]);
                y = Math.Atan2(-rm[6], sy);
                z = 0;
            }
            x = x * 180.0f / Math.PI;
            y = y * 180.0f / Math.PI;
            z = z * 180.0f / Math.PI;
            eulerAngle = new double[3] { x, y, z };

        }


    }

}


