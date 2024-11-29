using UnityEngine;
using UnityEngine.UI;
using System;
using HMS.Engine;
using HMS.Core;

public class RGBRecord : MonoBehaviour
{
    private Texture2D tex = null;
    //private Color32[] pixel32;
    //private GCHandle pixelHandle;
    //private IntPtr pixelPtr;
    private byte[] pixelBytes = null;
    public double rgbTimestamp = 0;
    private int lastWidth = 0;
    private int lastHeight = 0;

    //private int handtype = -1;
    public GameObject backgroundGameObjects;
    public GameObject cameraObject;

    public GameObject headObject;
 

    private bool readRgbCalibrationFlag = false;

    private double[] _R;
    private double[] _T;
    //左眼的欧拉角
    private double[] _EulerAngles;

    private double[] _poseData = new double[7];

    private XvDeviceManager xvDvManage;

    //public int delayTime = 1;
    //private DateTime lastDateTime;

    void Start()
    {
        // use uvc rgb
        //API.xslam_set_rgb_source( 0 );

        // set to 720p
        //API.xslam_set_rgb_resolution( 1 );
        //lastDateTime = DateTime.Now;

        GameObject xv = GameObject.Find("XvXRManager");
        if (xv != null)
        {
            xvDvManage = xv.GetComponent<XvDeviceManager>();
        }
    }

    private bool ifOpenRgb = false;

    public void btnClick(GameObject btn)
    {
        switch (btn.name)
        {
            case "RgbBtn":
                if (ifOpenRgb == false)
                {
                    headObject.SetActive(true);
                    backgroundGameObjects.SetActive(true);
                    ifOpenRgb = true;
                    //xvDvManage.ChangeRgbStatus(true);
                    btn.gameObject.transform.Find("Text").GetComponent<Text>().text = "Show Picture";
                }
                else
                {
                    headObject.SetActive(false);
                    backgroundGameObjects.SetActive(false);
                    ifOpenRgb = false;
                    //xvDvManage.ChangeRgbStatus(false);
                    btn.gameObject.transform.Find("Text").GetComponent<Text>().text = "Close Picture";
                }
                break;
            case "RtspBtn":
                headObject.SetActive(true);
                cameraObject.GetComponent<WifiDisplayPluginWrapper>().OnPcDisplayClick();
                break;
        }
    }

    public void openOrCloseRgb()
    {
        if (ifOpenRgb == false)
        {
            headObject.SetActive(true);
            backgroundGameObjects.SetActive(true);
            ifOpenRgb = true;
           // xvDvManage.ChangeRgbStatus(true);
        }
        else
        {
            headObject.SetActive(false);
            backgroundGameObjects.SetActive(false);
            ifOpenRgb = false;
           // xvDvManage.ChangeRgbStatus(false);
        }
    }

    void ReadRgbCalibration()
    {
        API.rgb_calibration rgb_Calibration = default(API.rgb_calibration);
        if (API.readRGBCalibration(ref rgb_Calibration))
        {
            float near = 0.3f;
            float far = 1000f;


            if (cameraObject != null)
            {
                near = cameraObject.GetComponent<Camera>().nearClipPlane;
                far = cameraObject.GetComponent<Camera>().farClipPlane;
                Camera camera = cameraObject.GetComponent<Camera>();
                //intrinsic720
                Matrix4x4 proj = HMS.Engine.XvXRBaseDevice.PerspectiveOffCenter((float)rgb_Calibration.intrinsic720.K[0], (float)rgb_Calibration.intrinsic720.K[1],
                 (float)rgb_Calibration.intrinsic720.K[2], (float)rgb_Calibration.intrinsic720.K[3], (float)rgb_Calibration.intrinsic720.K[9], (float)rgb_Calibration.intrinsic720.K[10], near, far);
                               
                //camera.fieldOfView = 2 * Mathf.Atan(1 / proj[1, 1]) * Mathf.Rad2Deg;
                //camera.projectionMatrix = proj;

                _T = new double[3] { rgb_Calibration.extrinsic.translation[0], -rgb_Calibration.extrinsic.translation[1], rgb_Calibration.extrinsic.translation[2] };
                
                //左眼标定的旋转矩阵→欧拉角
                _R = new double[9] { rgb_Calibration.extrinsic.rotation[0], -rgb_Calibration.extrinsic.rotation[1], rgb_Calibration.extrinsic.rotation[2], -rgb_Calibration.extrinsic.rotation[3], rgb_Calibration.extrinsic.rotation[4],
                            -rgb_Calibration.extrinsic.rotation[5],rgb_Calibration.extrinsic.rotation[6],-rgb_Calibration.extrinsic.rotation[7],rgb_Calibration.extrinsic.rotation[8]};
                HMS.Engine.XvXREye.RotationMatrixToEulerAngles(ref _EulerAngles, _R);
                

                cameraObject.transform.localPosition = new Vector3((float)_T[0], (float)_T[1], (float)_T[2]);
                cameraObject.transform.localEulerAngles = new Vector3((float)_EulerAngles[0], (float)_EulerAngles[1], (float)_EulerAngles[2]);

                //设置Physical camera
                float focal = 3.519f;//RGB相机焦距（单位为毫米）
                float fx = (float)rgb_Calibration.intrinsic720.K[0];
                float fy = (float)rgb_Calibration.intrinsic720.K[1];
                float cx = (float)rgb_Calibration.intrinsic720.K[2];
                float cy = (float)rgb_Calibration.intrinsic720.K[3];

                float width = (float)rgb_Calibration.intrinsic720.K[9];
                float height = (float)rgb_Calibration.intrinsic720.K[10];
                camera.usePhysicalProperties = true;
                camera.focalLength = focal;
                camera.sensorSize = new Vector2(focal * width / fx,
                                              focal * height / fy);
                //Debug.Log($"camera.sensorSize:{Math.Round(camera.sensorSize.x,6)},{Math.Round(camera.sensorSize.y, 6)}");
                camera.lensShift = new Vector2(-(cx - width * 0.5f) / width,
                                             (cy - height * 0.5f) / height);
                //Debug.Log($"camera.lensShift:{Math.Round(camera.lensShift.x,6)},{Math.Round(camera.lensShift.y, 6)}");
                camera.gateFit = Camera.GateFitMode.Vertical;

                // int screenWidth  = UnityEngine.Screen.width;
                // int screenHeight = UnityEngine.Screen.height;
                // float cameraWidth = (float)rgb_Calibration.intrinsic1080.K[9];
                // float cameraHeight = (float)rgb_Calibration.intrinsic1080.K[10];

                // float x = (screenWidth - cameraWidth)/(2*screenWidth);
                // float y = (screenHeight - cameraHeight)/(2*screenHeight);
                // float rectWidth = cameraWidth/screenWidth ;
                // float rectHeight = cameraHeight/screenHeight ;

                // Rect rect = new Rect(x,y,rectWidth,rectHeight);
                // Vector2 center = new Vector2(0.5f,0.5f);
                // rect.center = center;
                // camera.rect = rect;
                // XvXRLog.LogInfo("RGBRecord screenWidth: "+screenWidth+",screenHeight:"+screenHeight+",cameraWidth"+cameraWidth+",cameraHeight"+cameraHeight+",rect:"+rect);

            }
            Debug.Log("RGBRecord ReadRgbCalibration:" + rgb_Calibration);

            Debug.Log("RGBRecord ReadRgbCalibration camera Fx,Fy: " +
                rgb_Calibration.intrinsic720.K[0] + "," + rgb_Calibration.intrinsic720.K[1]);

            readRgbCalibrationFlag = true;



        }
        else
        {
            Debug.Log("RGBRecord readStereoFisheyesCalibration faild");
        }
    }

    bool needProccessRgb = true;

    void Update()
    {
#if UNITY_EDITOR
        return;
#endif
        //if (!needProccessRgb)
        //{
        //    double t = GetTime(lastDateTime);

        //    if (t > delayTime)
        //    {
        //        needProccessRgb = true;
        //    }
        //}

        if (API.xslam_ready()&&needProccessRgb)
        {
            if (!readRgbCalibrationFlag)
            {
                ReadRgbCalibration();
            }
            int width = API.xslam_get_rgb_width();
            int height = API.xslam_get_rgb_height();
            if (width > 0 && height > 0)
            {
                if (lastWidth != width || lastHeight != height)
                {
                    try
                    {
                        double r = 1.0;
                        // if (width <=1280 && height <=720) {
                        // r = 1.0;
                        // }
                        int w = (int)(width * r);
                        int h = (int)(height * r);
                        Debug.Log("RGBRecord Create RGB texture " + w + "x" + h);
                        TextureFormat format = TextureFormat.RGBA32;
                        tex = new Texture2D(w, h, format, false);
                        //tex.filterMode = FilterMode.Point;
                        tex.Apply();
                        pixelBytes = new byte[w * h * 4];
                 
                        Debug.Log("RGBRecord Create RGB texture end" + w + "x" + h);

                        if (backgroundGameObjects != null)
                        {

                            Material material = new Material(Shader.Find("Unlit/Texture"));
                            material.SetTextureScale("_MainTex", new Vector2(-1, -1));
                            material.mainTexture = tex;
                            backgroundGameObjects.GetComponent<Image>().material = material;
                            Debug.Log("RGBRecord material.mainTexture  setting ....");
                        }
                        else
                        {
                            Debug.Log("RGBRecord material.mainTexture not set");
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                        return;
                    }

                    lastWidth = width;
                    lastHeight = height;
                }

                if (pixelBytes!=null&&tex!=null)
                {
                    try
                    {
                        if (API.xslam_get_rgb_image_RGBA_Byte(pixelBytes, tex.width, tex.height, ref rgbTimestamp))
                        {
                          //  Debug.Log("vr_log:timestamp RGBRecord Update xslam_get_rgb_image_RGBA rgbTimestamp: " + rgbTimestamp);
                            tex.SetPixelData(pixelBytes, 0, 0);
                            tex.Apply();

                            if (rgbTimestamp > 0)
                            {
                                if (API.xslam_get_pose_at(_poseData, rgbTimestamp))
                                {
                                    if (headObject != null)
                                    {
                                        headObject.transform.localRotation = new Quaternion(-(float)_poseData[0], (float)_poseData[1], -(float)_poseData[2], (float)_poseData[3]);
                                        headObject.transform.localPosition = new Vector3((float)_poseData[4], -(float)_poseData[5], (float)_poseData[6]);
                                    }

                                    if (XvXRSdkConfig.XvXR_PLATFORM == XvXRSdkConfig.PLATFORM.XvXR_UNITY_ANDROID && XvXRManager.SDK.IsUseUserPose) {
                                        Quaternion quaternion= headObject.transform.localRotation;
                                        ((XvXRAndroidDevice)XvXRManager.SDK.GetDevice()).SetSrcQuaternionUnity(quaternion,(long)rgbTimestamp*1000000);
                                        Debug.Log("IsUseUserPose");
                                    }
                                }
                                else
                                {
                                    Debug.Log("RGBRecord xslam_get_pose_at faild");
                                }
                            }
                        }
                        else
                        {
                            //Debug.Log("RGBRecord Invalid texture");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                        return;
                    }

                   
                }

            }

        
        }
    }
    void OnGUI()
    {

    }



    void OnApplicationQuit()
    {
        //if(pixelHandle!=null&&pixelHandle.IsAllocated){
        //    //Free handle
        //    pixelHandle.Free();
        //}
    
    }


    public static double GetTime(DateTime timeA)
    {
        //timeA 表示需要计算
        DateTime timeB = DateTime.Now;  //获取当前时间
        TimeSpan ts = timeB - timeA;    //计算时间差
        double time = ts.TotalSeconds;  //将时间差转换为秒
        return time;
    }
}
