

namespace Xvisio.Input.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    public class CustomEventData : PointerEventData
    {
        #region  避免不必要的麻烦最好不要直接访问

        public bool pressPrecessed;//标记按下抬起

        public float screenPositionDeltadelta;//移动位置增量

        private XvInputControllerBase inputControllerBase;



        #endregion

        public CustomEventData(EventSystem eventSystem, XvInputControllerBase inputControllerBase) : base(eventSystem)
        {
            this.inputControllerBase = inputControllerBase;
        }



        public RaycastHit hover3DRaycastHit;//选中的3D物体
        public static Vector3 ScreenCenterPoint { get { return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f); } }

        internal bool GetKeyPress()
        {
            return inputControllerBase.GetKeyPress();

        }
    }

  
    

    public abstract class XvInputControllerBase : MonoBehaviour
    {
      
        internal XvRaycaster customRaycaster
        {
            get;
            private set;
        }
        private CustomEventData customEventData;
        public CustomEventData CustomEventData
        {
            get
            {

                if (customEventData == null)
                {
                    customEventData = new CustomEventData(EventSystem.current, this);
                }
                return customEventData;
            }
        }



        //是否是3D空间射线输入
        internal bool Is3DInput
        {
            get
            {
                return customRaycaster != null;
            }
        }

        /// <summary>
        /// 获取当前用户自定的屏幕上的点
        /// </summary>
        internal Vector3 screenPosition
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取两帧之前的移动位置
        /// </summary>
        internal Vector3 PositionDeltadelta
        {
            get
            {
                return positionDeltadelta;
            }
            private set
            {
                positionDeltadelta = value;
            }
        }

        private Vector3 lastPosition;

        private Vector3 positionDeltadelta;


        /// <summary>
        /// 获取屏幕输入的坐标
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetInputPosition()
        {

            if (Is3DInput)
            {
                return transform.forward;
            }
            else
            {
                return Input.mousePosition;
            }

        }

        public virtual Vector2 GetScrollDelta()
        {
            return Vector3.zero;
        }

        public virtual bool GetKeyPress()
        {
           
            return Input.GetMouseButton(0);
        }
        protected virtual void Awake()
        {
            Initialized();
        }

        protected virtual void Update()
        {
            UpdateInputPosition();
        }

        /// <summary>
        /// 初始化自定义输入模块
        /// </summary>
        private void Initialized()
        {
            screenPosition = new Vector2(Screen.width / 2, Screen.height / 2);

            customRaycaster = GetComponent<XvRaycaster>();
            if (EventSystem.current.gameObject.GetComponent<XvXRInputModule>() == null)
            {
                Debug.LogError("执行次数");
                Destroy(EventSystem.current.gameObject.GetComponent<StandaloneInputModule>());
                EventSystem.current.gameObject.AddComponent<XvXRInputModule>();
            }
        }

        /// <summary>
        /// 更新输入坐标点
        /// </summary>
        private void UpdateInputPosition()
        {
            if (Is3DInput)
            {
                positionDeltadelta = (GetInputPosition() - lastPosition) * 1000;
                lastPosition = GetInputPosition();
            }
            else
            {
                screenPosition = GetInputPosition();
                positionDeltadelta = screenPosition - lastPosition;
                lastPosition = screenPosition;
            }
        }
    }
}
