using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GraphicRaycaster;


namespace Xvisio.Input.UI
{

   // [RequireComponent(typeof(HandInputController))]
    public class XvRaycaster : BaseRaycaster
    {
        internal XvInputControllerBase controllerBase
        {
            get;
            private set;
        }
        /// <summary>
        /// 记录当前帧射线选中的UI对象
        /// </summary>
        private readonly List<RaycastResult> sortedRaycastResults = new List<RaycastResult>();//当前射线选择到的对象

        /// <summary>
        /// 射线管理器
        /// </summary>
        private Type RaycasterManager;

        private RaycastResult raycastResult3D;

        /// <summary>
        /// 每个自定义输入的事件相机
        /// </summary>
        private Camera fallbackCam;
        public override Camera eventCamera
        {
            get
            {
                if (fallbackCam == null)
                {
                    var go = new GameObject(name + " FallbackCamera");
                    go.SetActive(false);

                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;

                    fallbackCam = go.AddComponent<Camera>();
                    fallbackCam.clearFlags = CameraClearFlags.Nothing;
                    fallbackCam.cullingMask = 0;
                    fallbackCam.orthographic = true;
                    fallbackCam.orthographicSize = 1;
                    fallbackCam.useOcclusionCulling = false;
#if !(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                    fallbackCam.stereoTargetEye = StereoTargetEyeMask.None;
#endif
                    fallbackCam.nearClipPlane = nearDistance;
                    fallbackCam.farClipPlane = farDistance;
                }

                return fallbackCam;
            }
        }



        //射线能交互的最近距离
        [SerializeField]
        private float nearDistance = 0f;
        public float NearDistance
        {
            get { return nearDistance; }
            set
            {
                nearDistance = Mathf.Max(0f, value);
                if (eventCamera != null)
                {
                    eventCamera.nearClipPlane = nearDistance;
                }
            }
        }


        /// <summary>
        /// 射线的最远距离
        /// </summary>
        [SerializeField]
        private float farDistance = 20f;
        public float FarDistance
        {
            get { return farDistance; }
            set
            {
                farDistance = Mathf.Max(0f, nearDistance, value);
                if (eventCamera != null)
                {
                    eventCamera.farClipPlane = farDistance;
                }
            }
        }
        private  List<BaseRaycaster> baseRaycasters;

        protected override void Awake()
        {
            controllerBase = GetComponent<XvInputControllerBase>();
        }
        protected override void OnEnable()
        {
          
        }

        protected override void Start()
        {
            //通过反射找到UGUI射线管理器
            RaycasterManager = EventSystem.current.GetType().Assembly.GetType("UnityEngine.EventSystems.RaycasterManager");
            UpdateAllBaseRaycasters();
        }

        protected override void OnDisable()
        {
            

        }
        /// <summary>
        /// 更新所有的BaseRaycaster输入,如果是动态构建的Canvas都需要调用一下
        /// </summary>
        public void UpdateAllBaseRaycasters()
        {
            baseRaycasters = (List<BaseRaycaster>)RaycasterManager.InvokeMember("GetRaycasters", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
        }



        /// <summary>
        /// 获取射线第一个触碰到的UI交互组件
        /// </summary>
        /// <returns></returns>
        public RaycastResult FirstRaycastResult()
        {
            Raycast();
            for (int i = 0; i < sortedRaycastResults.Count; ++i)
            {
                if (!sortedRaycastResults[i].isValid) { continue; }
                return sortedRaycastResults[i];
            }
            return default(RaycastResult);

        }
        /// <summary>
        /// 设置照相机的位置，循环处理射线分别和UI元素交互的结果
        /// </summary>
        private void Raycast()
        {
            if (RaycasterManager != null)
            {
                sortedRaycastResults.Clear();
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out RaycastHit raycastHit3D, FarDistance))
                {
                    if (raycastHit3D.transform.GetComponent<Collider>() != null)
                    {
                        controllerBase.CustomEventData.hover3DRaycastHit = raycastHit3D;

                        raycastResult3D = new RaycastResult
                        {
                            gameObject = raycastHit3D.transform.gameObject,
                            module = this,
                            distance = raycastHit3D.distance,
                            worldPosition = raycastHit3D.point,

                            screenPosition = CustomEventData.ScreenCenterPoint,

                        };
                        sortedRaycastResults.Add(raycastResult3D);
                    }
                    else {
                        controllerBase.CustomEventData.hover3DRaycastHit = default(RaycastHit);
                    }
                   
                }
                else {
                    controllerBase.CustomEventData.hover3DRaycastHit = default(RaycastHit);
                }


                UpdateAllBaseRaycasters();
                foreach (var item in baseRaycasters)
                {

                    Canvas canvas = item.GetComponent<Canvas>();

                    if (canvas.renderMode != RenderMode.WorldSpace)
                    {
                        continue;

                    }
                    //  Debug.LogError(12);
                  
                    if (!RectTransformUtility.RectangleContainsScreenPoint(canvas.GetComponent<RectTransform>(), CustomEventData.ScreenCenterPoint, eventCamera))
                    {
                        //Debug.LogError("不在视野之内不处理：" + canvas.name);
                        continue;
                    }
                  
                    Raycast(canvas, true, ray, FarDistance, sortedRaycastResults);
                }
            }
            else
            {
                Debug.LogError("RaycasterManager == null这是不应该的");
            }


        }

        /// <summary>
        /// 处理射线和UI元素交互结果
        /// </summary>
        /// <param name="canvas">当前的Canvas</param>
        /// <param name="ignoreReversedGraphics">是否忽略反方向上的图形</param>
        /// <param name="ray">当前交互的射线</param>
        /// <param name="distance">能够检测的最大距离</param>
        /// <param name="raycastResults">返回一个检测结果</param>
        private void Raycast(Canvas canvas, bool ignoreReversedGraphics, Ray ray, float distance, List<RaycastResult> raycastResults)
        {
            if (canvas == null) { return; }

           

            var screenCenterPoint = CustomEventData.ScreenCenterPoint;
            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            float rayCasterDis = FarDistance;

            Debug.DrawLine(ray.origin, ray.origin+ray.direction*5,Color.green);
           
          
            for (int i = 0; i < graphics.Count; ++i)
            {
                var graphic = graphics[i];
                // -1的情况和不接受射线检测情况下是不处理
                if (graphic.depth == -1 || !graphic.raycastTarget) { continue; }
                //当前射线点不在矩形框区域内不处理
                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, screenCenterPoint, eventCamera)) { continue; }
                //反方向点击不处理
                if (ignoreReversedGraphics && Vector3.Dot(ray.direction, graphic.transform.forward) <= 0f) { continue; }

                //当前射线没有选中不处理
                if (!graphic.Raycast(screenCenterPoint, eventCamera)) { continue; }
                //超过照相机最远距离不处理
                float dist = 10;
                new Plane(graphic.transform.forward, graphic.transform.position).Raycast(ray, out dist);
                if (dist > distance || dist > rayCasterDis)
                {
                   

                    continue;
                }

                if (dist < rayCasterDis && sortedRaycastResults.Contains(raycastResult3D))
                {
                    controllerBase.CustomEventData.hover3DRaycastHit = default(RaycastHit);
                    sortedRaycastResults.Remove(raycastResult3D);
                }
                //Debug.LogError(graphic.name);
               // controllerBase.CustomEventData.hover3DRaycastHit = default(RaycastHit);

                raycastResults.Add(new RaycastResult
                {
                    gameObject = graphic.gameObject,
                    module = this,
                    distance = dist,
                    worldPosition = ray.GetPoint(dist),
                    worldNormal = -graphic.transform.forward,
                    screenPosition = screenCenterPoint,
                    index = raycastResults.Count,
                    depth = graphic.depth,
                    sortingLayer = canvas.sortingLayerID,
                    sortingOrder = canvas.sortingOrder
                });
            }
            //将结果的深度进行升序排序
            raycastResults.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
        }
        private void LateUpdate()
        {
            controllerBase.CustomEventData.hover3DRaycastHit = default(RaycastHit);
        }
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {

        }
    }
}