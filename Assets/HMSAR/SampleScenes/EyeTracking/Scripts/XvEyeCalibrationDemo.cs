using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using HMS.Engine;
namespace XvXRFoundation
{
    public class XvEyeCalibrationDemo : MonoBehaviour
    {
        public XvEyeTrackingManager xvEyeTrackingManager;

        static string TAG = "CalibrationManager";

        bool isAdjustGlass = false;
        [Header("瞳孔左右眼图标")]
        public GameObject leftEye;
        public GameObject rightEye;

        public GameObject corner;

        Image leftEyeCircle;
        Image rightEyeCircle;
        Image corner4;

        public static bool leftReady;
        public static bool rightReady;

        public GameObject adjustPupil;


        float leftPupilTime;
        float rightPupilTime;
        bool pupilAdj = true;
        public GameObject caliTipText;
        public GameObject finishTipText;

        public static GameObject caliCube;
        public static Vector3[] caliPoints;
        public static int caliIndex;
        static bool everyCali = false;
        public AnimationClip rotLoop;

        static XvEyeCalibrationDemo calibrationManager;

        private void Awake()
        {
            if (xvEyeTrackingManager == null)
            {
                xvEyeTrackingManager = FindObjectOfType<XvEyeTrackingManager>();

                if (xvEyeTrackingManager == null)
                {
                    xvEyeTrackingManager = new GameObject("XvEyeTrackingManager").AddComponent<XvEyeTrackingManager>();
                }
            }
        }
        private void OnEnable()
        {
            xvEyeTrackingManager.StartGaze();
           
        }
        private void OnDisable()
        {
            xvEyeTrackingManager.StopGaze();

        }

        // Start is called before the first frame update
        void Start()
        {
            cameraTran = Camera.main.transform;
            //瞳距检测准备中
            leftEyeCircle = leftEye.GetComponent<Image>();
            rightEyeCircle = rightEye.GetComponent<Image>();
            corner4 = corner.GetComponent<Image>();

            caliCube = GameObject.Find("CaliCube");
            caliCube.SetActive(false);

            caliPoints = new Vector3[5];

            for (int i = 0; i < 5; i++)
            {
                //校准点位坐标基于ETCS坐标系描述
                caliPoints[i].x = GameObject.Find("5" + i.ToString()).transform.localPosition.x * 1000;
                caliPoints[i].y = -GameObject.Find("5" + i.ToString()).transform.localPosition.y * 1000;
                caliPoints[i].z = GameObject.Find("5" + i.ToString()).transform.localPosition.z * 1000;
            }

            calibrationManager = GetComponent<XvEyeCalibrationDemo>();


            Invoke("AdjustGlass", 10);
        }

        void AdjustGlass()
        {
            isAdjustGlass = true;
        }


        private Transform cameraTran;
        // Update is called once per frame
        void Update()
        {
            transform.position = cameraTran.position;
            transform.rotation = cameraTran.rotation;
            //校准第一步，调整设备观察是否正确检测到左右眼瞳孔
            if (adjustPupil.activeSelf && xvEyeTrackingManager.Tracking)
            {

                MyDebugTool.Log(string.Format($"{TAG} pupilCenter L:{xvEyeTrackingManager.EyeData.leftPupil.pupilCenter.x},{xvEyeTrackingManager.EyeData.leftPupil.pupilCenter.y}"));
                MyDebugTool.Log(string.Format($"{TAG} pupilCenter R:{xvEyeTrackingManager.EyeData.rightPupil.pupilCenter.x},{xvEyeTrackingManager.EyeData.rightPupil.pupilCenter.y}"));

                //pupilCenter.x和y的值的范围是（-1~1）
                leftEye.transform.localPosition = new Vector3(-xvEyeTrackingManager.EyeData.leftPupil.pupilCenter.x * 50f, -xvEyeTrackingManager.EyeData.leftPupil.pupilCenter.y * 25f, 0f);
                rightEye.transform.localPosition = new Vector3(-xvEyeTrackingManager.EyeData.rightPupil.pupilCenter.x * 50f, -xvEyeTrackingManager.EyeData.rightPupil.pupilCenter.y * 25f, 0f);

                if (xvEyeTrackingManager.EyeData.leftPupil.pupilCenter.x == -1 || xvEyeTrackingManager.EyeData.leftPupil.pupilCenter.y == -1)
                {
                    leftReady = false;
                    leftEyeCircle.enabled = false;
                }
                else
                {
                    leftReady = true;
                    leftEyeCircle.enabled = true;
                }

                if (xvEyeTrackingManager.EyeData.rightPupil.pupilCenter.x == -1 || xvEyeTrackingManager.EyeData.rightPupil.pupilCenter.y == -1)
                {
                    rightReady = false;
                    rightEyeCircle.enabled = false;
                }
                else
                {
                    rightReady = true;
                    rightEyeCircle.enabled = true;
                }

                if (leftReady && rightReady)
                {
                    corner4.color = Color.green;
                }
                else
                {
                    corner4.color = Color.red;
                }
            }



            //校准第二步，校准5个点位
            if (leftReady)
            {
                leftPupilTime += Time.deltaTime;
            }
            else
            {
                leftPupilTime = 0f;
            }

            if (rightReady)
            {
                rightPupilTime += Time.deltaTime;
            }
            else
            {
                rightPupilTime = 0f;
            }

            if (leftPupilTime > 2f && rightPupilTime > 2f && pupilAdj && isAdjustGlass)
            {
                pupilAdj = false;
                PrepareCali();
            }

            //逐步校准5个点位
            if (everyCali)
            {
                everyCali = false;
                caliIndex++;
                StartCoroutine(StartCaliPoint());
            }
            MyDebugTool.Log($"{TAG} caliIndex:{caliIndex}");
        }

        void PrepareCali()
        {
            adjustPupil.SetActive(false);
            StartCoroutine(showTipText());
        }

        IEnumerator showTipText()
        {
            //瞳距检测开始
            caliTipText.SetActive(true);
            caliCube.SetActive(true);
            caliCube.transform.localPosition = new Vector3(0, 0, caliCube.transform.localPosition.z);
            caliCube.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(5f);
            caliTipText.SetActive(false);
            StartCali();
        }

        void StartCali()
        {
            caliIndex = 0;

            MyDebugTool.Log($"{TAG} xslam_gaze_calibration_enter");
            xvEyeTrackingManager.GazeCalibrationEnter();

            //开始使用蓝色cube校准
            StartCoroutine(StartFirstPoint());
        }

        public  IEnumerator StartFirstPoint()
        {
            Animation animPos = caliCube.GetComponent<Animation>();

            AnimationCurve scaleUp = AnimationCurve.Linear(0f, 1f, 0.3f, 2f);

            AnimationClip clipPos = new AnimationClip
            {
                legacy = true
            };

            clipPos.SetCurve("", typeof(Transform), "localScale.x", scaleUp);
            clipPos.SetCurve("", typeof(Transform), "localScale.y", scaleUp);
            clipPos.SetCurve("", typeof(Transform), "localScale.z", scaleUp);

            animPos.AddClip(clipPos, "ScaleUp");
            animPos.Play("ScaleUp");
            yield return new WaitForSeconds(clipPos.length);
            yield return new WaitForSeconds(0.25f);

            AnimationCurve scaleDown = AnimationCurve.Linear(0f, 2f, 0.5f, 1f);

            clipPos.SetCurve("", typeof(Transform), "localScale.x", scaleDown);
            clipPos.SetCurve("", typeof(Transform), "localScale.y", scaleDown);
            clipPos.SetCurve("", typeof(Transform), "localScale.z", scaleDown);

            animPos.AddClip(clipPos, "ScaleDown");
            animPos.Play("ScaleDown");
            yield return new WaitForSeconds(clipPos.length);


            animPos.AddClip(calibrationManager.rotLoop, "rotLoop");
            animPos.clip = calibrationManager.rotLoop;
            animPos.Play();
            //蓝色校准点移动到下一点位后等待0.7s后再开始collect，确保用户视线注视到目标点位
            yield return new WaitForSeconds(0.7f);

            //接口返回成功的次数
            int collectSuccess = 0;
            //接口调用次数
            int calltimes = 0;
            while (collectSuccess < 5 && calltimes < 100)
            {
                int c = xvEyeTrackingManager.GazeCalibrationCollect(caliPoints[caliIndex], caliIndex);
                calltimes++;
                MyDebugTool.Log($"{TAG} calltimes:{calltimes} StartFirstPoint xslam_gaze_calibration_collect[{caliIndex}] return :{c}");
                if (c == 0)
                {
                    collectSuccess++;
                }
                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(1f);
            everyCali = true;
        }

        public  IEnumerator StartCaliPoint()
        {
            MyDebugTool.Log($"{TAG} ##StartCaliPoint##");
            Animation animPos = caliCube.GetComponent<Animation>();

            AnimationCurve scaleDown = AnimationCurve.Linear(0f, 1f, 0.5f, 1.7f);

            AnimationClip clipPos = new AnimationClip
            {
                legacy = true
            };

            clipPos.SetCurve("", typeof(Transform), "localScale.x", scaleDown);
            clipPos.SetCurve("", typeof(Transform), "localScale.y", scaleDown);
            clipPos.SetCurve("", typeof(Transform), "localScale.z", scaleDown);

            animPos.AddClip(clipPos, "ScaleDown");
            animPos.Play("ScaleDown");

            yield return new WaitForSeconds(clipPos.length);

            if (caliIndex == 5)
            {
                #region 校准完最后一个点位后回到初始点位
                caliCube.transform.localEulerAngles = Vector3.zero;
                AnimationCurve scaleMoveEnd = AnimationCurve.Linear(0f, 1.7f, 1f, 1f);


                AnimationCurve curveXEnd = AnimationCurve.Linear(0, caliCube.transform.localPosition.x, 1.5f, GameObject.Find("50").transform.localPosition.x);
                AnimationCurve curveYEnd = AnimationCurve.Linear(0, caliCube.transform.localPosition.y, 1.5f, GameObject.Find("50").transform.localPosition.y);
                AnimationCurve curveZEnd = AnimationCurve.Linear(0, caliCube.transform.localPosition.z, 1.5f, GameObject.Find("50").transform.localPosition.z);



                clipPos.SetCurve("", typeof(Transform), "localScale.x", scaleMoveEnd);
                clipPos.SetCurve("", typeof(Transform), "localScale.y", scaleMoveEnd);
                clipPos.SetCurve("", typeof(Transform), "localScale.z", scaleMoveEnd);

                clipPos.SetCurve("", typeof(Transform), "localPosition.x", curveXEnd);
                clipPos.SetCurve("", typeof(Transform), "localPosition.y", curveYEnd);
                clipPos.SetCurve("", typeof(Transform), "localPosition.z", curveZEnd);

                animPos.AddClip(clipPos, "Trans");
                animPos.Play("Trans");
                #endregion

                yield return new WaitForSeconds(clipPos.length + 3f);
                FinishCali();
                yield break;
            }

            AnimationCurve scaleMove = AnimationCurve.Linear(0f, 1.7f, 1f, 1f);


            AnimationCurve curveX = AnimationCurve.Linear(0, caliCube.transform.localPosition.x, 1.5f, GameObject.Find("5" + caliIndex.ToString()).transform.localPosition.x);
            AnimationCurve curveY = AnimationCurve.Linear(0, caliCube.transform.localPosition.y, 1.5f, GameObject.Find("5" + caliIndex.ToString()).transform.localPosition.y);
            AnimationCurve curveZ = AnimationCurve.Linear(0, caliCube.transform.localPosition.z, 1.5f, GameObject.Find("5" + caliIndex.ToString()).transform.localPosition.z);



            clipPos.SetCurve("", typeof(Transform), "localScale.x", scaleMove);
            clipPos.SetCurve("", typeof(Transform), "localScale.y", scaleMove);
            clipPos.SetCurve("", typeof(Transform), "localScale.z", scaleMove);

            clipPos.SetCurve("", typeof(Transform), "localPosition.x", curveX);
            clipPos.SetCurve("", typeof(Transform), "localPosition.y", curveY);
            clipPos.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

            animPos.AddClip(clipPos, "Trans");
            animPos.Play("Trans");

            yield return new WaitForSeconds(clipPos.length + 0.5f);

            animPos.clip = calibrationManager.rotLoop;

            animPos.Play();
            //蓝色校准点移动到下一点位后等待0.7s后再开始collect，确保用户视线注视到目标点位
            yield return new WaitForSeconds(0.7f);

            //接口返回成功的次数
            int collectSuccess = 0;
            //接口调用次数
            int calltimes = 0;
            while (collectSuccess < 5 && calltimes < 100)
            {
                int c = xvEyeTrackingManager.GazeCalibrationCollect(caliPoints[caliIndex], caliIndex);
                calltimes++;
                MyDebugTool.Log($"{TAG} calltimes:{calltimes} StartCaliPoint xslam_gaze_calibration_collect[{caliIndex}] return :{c}");
                if (c == 0)
                {
                    collectSuccess++;
                }
                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(1f);
            everyCali = true;
        }

        int retrieve = -1;
        public  void FinishCali()
        {
            retrieve= xvEyeTrackingManager. CalibrationComplete();

            calibrationManager.StartCoroutine(showFinishText());

            MyDebugTool.Log($"{TAG} xslam_gaze_calibration_retrieve:{retrieve}");
        }

      
      

        public  IEnumerator showFinishText()
        {
            caliCube.SetActive(false);

            yield return new WaitForSeconds(2f);

            MyDebugTool.Log($"{TAG} xslam_gaze_calibration_retrieve:{calibrationManager.retrieve}    ipd={xvEyeTrackingManager.EyeData.ipd}");

            if (calibrationManager.retrieve == 0 && xvEyeTrackingManager.EyeData.ipd > 50 && xvEyeTrackingManager.EyeData.ipd < 80)
            {
                calibrationManager.finishTipText.GetComponent<Text>().text = $"校准成功\nipd:{xvEyeTrackingManager.EyeData.ipd}";

            }
            else
            {
                calibrationManager.finishTipText.GetComponent<Text>().text = $"校准失败，请重新校准";
            }
            calibrationManager.finishTipText.SetActive(true);


        }


        //重新校准
        public void RestartCalibration()
        {
            StopAllCoroutines();

            calibrationManager.finishTipText.SetActive(false);
            adjustPupil.SetActive(true);
            leftReady = false;
            rightReady = false;
            leftPupilTime = 0f;
            rightPupilTime = 0f;
            pupilAdj = true;

            Animation anim = caliCube.GetComponent<Animation>();
            foreach (AnimationState clip in anim)
            {
                anim.RemoveClip(clip.clip);
            }

            caliIndex = 0;

          //  bool unset = xvEyeTrackingManager.UnsetGazeCallback();
           // Debug.Log($"ResetCalibration unset {unset}");
        }


    }
}