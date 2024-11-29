using System.Collections.Generic;
using UnityEngine;
using XvXR.MixedReality.Toolkit.XvXR.Input;
using static XvXR.MixedReality.Toolkit.XvXR.Input.XvXRJoystick;

namespace XvXRFoundation
{
    public class XvJoystickManager : MonoBehaviour
    {
        private XvJoystickManager() { }
        public static XvJoystickManager Instance;

        [SerializeField]
        private XvHandleController leftController;
        [SerializeField]

        private XvHandleController rightController;

        private XvXRJoystickKeyState left;
        private XvXRJoystickKeyState right;

        private void Awake()
        {
            Instance = this;
            if (transform.Find("RightController"))
            {
                rightController = transform.Find("RightController").GetComponent<XvHandleController>();
            }

            if (transform.Find("LeftController"))
            {
                leftController = transform.Find("LeftController").GetComponent<XvHandleController>();
            }

            if (leftController != null)
            {

                left = new XvXRJoystickKeyState(leftController);
            }
            if (rightController != null)
            {

                right = new XvXRJoystickKeyState(rightController);
            }

        }
       



        /// <summary>
        /// 获取当前手柄原始数据
        /// </summary>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public JoystickData GetJoystickData(TrackerType trackerType)
        {

            switch (trackerType)
            {

                case TrackerType.Left:


                    return leftController == null ? null : leftController.GetJoystickData();

                case TrackerType.Right:
                    return rightController == null ? null : rightController.GetJoystickData();

                default:
                    break;
            }
            return null;

        }
        /// <summary>
        /// 手柄按键按下事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public bool GetKeyDown(JoystickButton button, TrackerType trackerType)
        {

            switch (trackerType)
            {

                case TrackerType.Left:
                    return left == null ? false : left.GetKeyDown(button);

                case TrackerType.Right:
                    return right == null ? false : right.GetKeyDown(button);

                default:
                    break;
            }
            return false;

        }
        /// <summary>
        /// 手柄按键抬起事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public bool GetKeyUp(JoystickButton button, TrackerType trackerType)
        {
            switch (trackerType)
            {

                case TrackerType.Left:
                    return left == null ? false : left.GetKeyUp(button);

                case TrackerType.Right:
                    return right == null ? false : right.GetKeyUp(button);

                default:
                    break;
            }
            return false;

        }
        /// <summary>
        /// 手柄按键长按
        /// </summary>
        /// <param name="button"></param>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public bool GetKey(JoystickButton button, TrackerType trackerType)
        {

            switch (trackerType)
            {

                case TrackerType.Left:
                    return left == null ? false : left.GetKey(button);

                case TrackerType.Right:
                    return right == null ? false : right.GetKey(button);

                default:
                    break;
            }
            return false;


        }
        /// <summary>
        /// 手柄按键双击
        /// </summary>
        /// <param name="button"></param>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public bool GetDoubleClick(JoystickButton button, TrackerType trackerType)
        {
            switch (trackerType)
            {

                case TrackerType.Left:
                    return left == null ? false : left.GetDoubleClick(button);

                case TrackerType.Right:
                    return right == null ? false : right.GetDoubleClick(button);

                default:
                    break;
            }
            return false;

        }




        /// <summary>
        /// 获取手柄摇杆输入
        /// </summary>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public Vector2 GetRockerVector2(TrackerType trackerType)
        {

            JoystickData joystickData = null;
            switch (trackerType)
            {

                case TrackerType.Left:
                    joystickData = leftController?.GetJoystickData();
                    break;
                case TrackerType.Right:
                    joystickData = rightController?.GetJoystickData();
                    break;
                default:
                    break;
            }
            if (joystickData != null)
            {
                Vector2 RockerVec = new Vector2(joystickData.keyRockerX, -joystickData.keyRockerY) / 32767f;


                return RockerVec;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// 获取当前手柄位置
        /// </summary>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public Vector3 GetPosition(TrackerType trackerType)
        {
            switch (trackerType)
            {

                case TrackerType.Left:
                    return leftController == null ? Vector3.zero : leftController.GetPosition();

                case TrackerType.Right:
                    return rightController == null ? Vector3.zero : rightController.GetPosition();


                default:
                    break;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// 获取手柄当前旋转
        /// </summary>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public Quaternion GetRotation(TrackerType trackerType)
        {
            switch (trackerType)
            {

                case TrackerType.Left:
                    return leftController == null ? Quaternion.identity : leftController.GetRotation();

                case TrackerType.Right:
                    return rightController == null ? Quaternion.identity : rightController.GetRotation();
                default:
                    break;
            }

            return Quaternion.identity;
        }

        /// <summary>
        /// 获取当前手柄置信度
        /// </summary>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public int GetConfidence(TrackerType trackerType)
        {
            switch (trackerType)
            {

                case TrackerType.Left:
                    return leftController == null ? 0 : leftController.GetJoystickData().confidence;

                case TrackerType.Right:
                    return rightController == null ? 0 : rightController.GetJoystickData().confidence;
                default:
                    break;
            }

            return 0;
        }

        public string GetSerialNumber(TrackerType trackerType) {
            switch (trackerType)
            {
                case TrackerType.Left:
                    return leftController == null ? null : leftController.SerialNumber;

                case TrackerType.Right:
                    return rightController == null ? null : rightController.SerialNumber;
                default:
                    break;
            }
            return null;
        }


        #region 蓝牙手柄

        /// <summary>
        /// 获取蓝牙列表
        /// </summary>
        /// <param name="trackerType"></param>
        /// <returns></returns>
        public List<bleInfo> GetBleInfo(TrackerType trackerType)
        {
            switch (trackerType)
            {
                case TrackerType.Left:

                    return leftController?.XvXRJoystick.GetBleInfo();

                case TrackerType.Right:
                    
                    return rightController?.XvXRJoystick.GetBleInfo();

            }

            return null;
        }

        /// <summary>
        /// 连接蓝牙设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mac"></param>
        public void ConnectXvBle(TrackerType trackerType, string name, string mac)
        {
            switch (trackerType)
            {
                case TrackerType.Left:
                    leftController?.XvXRJoystick.ConnectXvBle(name, mac);

                    break;
                case TrackerType.Right:
                    rightController?.XvXRJoystick.ConnectXvBle(name, mac);

                    break;
              
            }
        }

        /// <summary>
        /// 断开蓝牙连接
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mac"></param>
        public void DisConnectXvBle(TrackerType trackerType, string name, string mac)
        {
            switch (trackerType)
            {
                case TrackerType.Left:
                    leftController?.XvXRJoystick.DisConnectXvBle(name, mac);

                    break;
                case TrackerType.Right:
                    rightController?.XvXRJoystick.DisConnectXvBle(name, mac);

                    break;
            
            }
        }

        public bool IsConnected(TrackerType trackerType)
        {
            switch (trackerType)
            {
                case TrackerType.Left:
                    if (leftController==null|| leftController.XvXRJoystick==null) { 
                    return false;
                    }

                   return leftController.XvXRJoystick.IsReady();

                
                case TrackerType.Right:
                    if (rightController == null || rightController.XvXRJoystick==null)
                    {
                        return false;
                    }
                    return  rightController.XvXRJoystick.IsReady();

                 
              
            }
            return false;
}

        #endregion


        private void Update()
        {

            if (left != null)
            {

                left.LateUpdate();
            }
            if (right != null)
            {

                right.LateUpdate();
            }
        }


    }

    #region
    public enum TrackerType
    {
        //None,
        Left,
        Right,
        //Tracker
    }

    public enum JoystickButton
    {
     
        Button_Trigger,//扳机键
        Button_A,
        Button_B,
        Button_Grip,//侧卧键
        Button_Thumbstick,//拇指摇杆
    }
    public enum DataSource
    {
        ANDROID_BLE = 1,
        ANDROID_2_4G = 2,
        WINDOWS_2_4G = 3
    }
    public class KeyState
    {

        public DataSource dataSource;
        private JoystickButton button;
        private bool IsPress;
        private KeyCode keyCode;
        private XvHandleController xRJoystick;
        public KeyState(JoystickButton buttonKey, XvHandleController xRJoystick)
        {
            this.button = buttonKey;
            this.xRJoystick = xRJoystick;
        }



        private bool IsTriggerPress;
        private float timer;
        private bool isKeyDown;


        private bool keyDown;
        private bool keyUp;
        private bool key;
        private bool doubleClick;



        public void Update()
        {
            keyDown = false;
            keyUp = false;

            doubleClick = false;



            switch (button)
            {
                case JoystickButton.Button_Trigger:
                    IsPress = xRJoystick.GetJoystickData().keyTrigger;
                    keyCode = KeyCode.Space;
                    break;
                case JoystickButton.Button_A:
                    IsPress = xRJoystick.GetJoystickData().keyA;
                    keyCode = KeyCode.A;

                    break;
                case JoystickButton.Button_B:
                    keyCode = KeyCode.B;

                    IsPress = xRJoystick.GetJoystickData().keyB;

                    break;
                case JoystickButton.Button_Grip:
                    keyCode = KeyCode.G;

                    IsPress = xRJoystick.GetJoystickData().keySlide;

                    break;
                case JoystickButton.Button_Thumbstick:
                    keyCode = KeyCode.H;

                    IsPress = xRJoystick.GetJoystickData().keyRocker;

                    break;
                default:
                    break;
            }
            if (IsPress || Input.GetKey(keyCode))
            {
                if (!IsTriggerPress)
                {
                    keyDown = true;
                    //MyDebugTool.Log("OnTriggerKeyDown:   " + buttonKey);
                    IsTriggerPress = true;

                    if (isKeyDown)
                    {

                        if (timer < 0.2f)
                        {
                            doubleClick = true;
                            //MyDebugTool.Log("OnTriggerDoubleClick:" + buttonKey);

                        }
                        isKeyDown = false;
                        timer = 0;
                    }
                    else
                    {

                        isKeyDown = true;
                    }
                }
                else
                {


                    key = true;
                    // MyDebugTool.Log("OnTriggerKey");

                }

            }
            else
            {
                if (isKeyDown)
                {
                    timer += Time.deltaTime;
                    if (timer > 0.3f)
                    {
                        timer = 0;
                        isKeyDown = false;
                    }
                }
                if (IsTriggerPress)
                {
                    keyUp = true;

                    //MyDebugTool.Log("OnTriggerKeyUp:" + buttonKey);

                    key = false;
                    IsTriggerPress = false;
                }
            }

        }




        public bool GetKeyDown()
        {
            return keyDown;

        }
        public bool GetKeyUp()
        {
            return keyUp;

        }

        public bool GetKey()
        {
            return key;

        }
        public bool GetDoubleClick()
        {
            return doubleClick;

        }


    }
    public class XvXRJoystickKeyState
    {
        public Dictionary<JoystickButton, KeyState> buttonKeyDic = new Dictionary<JoystickButton, KeyState>();
        public XvXRJoystickKeyState(XvHandleController xvXRJoystick)
        {

            buttonKeyDic.Add(JoystickButton.Button_Trigger, new KeyState(JoystickButton.Button_Trigger, xvXRJoystick));
            buttonKeyDic.Add(JoystickButton.Button_A, new KeyState(JoystickButton.Button_A, xvXRJoystick));
            buttonKeyDic.Add(JoystickButton.Button_B, new KeyState(JoystickButton.Button_B, xvXRJoystick));
            buttonKeyDic.Add(JoystickButton.Button_Grip, new KeyState(JoystickButton.Button_Grip, xvXRJoystick));
            buttonKeyDic.Add(JoystickButton.Button_Thumbstick, new KeyState(JoystickButton.Button_Thumbstick, xvXRJoystick));
        }



        public void LateUpdate()
        {
            foreach (var item in buttonKeyDic.Values)
            {
                item.Update();
            }
        }

        public bool GetKeyDown(JoystickButton buttonKey)
        {
            buttonKeyDic.TryGetValue(buttonKey, out KeyState keyState);
            return keyState.GetKeyDown();


        }
        public bool GetKeyUp(JoystickButton buttonKey)
        {
            buttonKeyDic.TryGetValue(buttonKey, out KeyState keyState);
            return keyState.GetKeyUp();

        }

        public bool GetKey(JoystickButton buttonKey)
        {
            buttonKeyDic.TryGetValue(buttonKey, out KeyState keyState);
            return keyState.GetKey();

        }
        public bool GetDoubleClick(JoystickButton buttonKey)
        {
            buttonKeyDic.TryGetValue(buttonKey, out KeyState keyState);
            return keyState.GetDoubleClick();

        }
    }
    #endregion

}
