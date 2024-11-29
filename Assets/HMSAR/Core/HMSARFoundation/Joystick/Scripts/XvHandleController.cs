using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XvXR.MixedReality.Toolkit.XvXR.Input;
using static XvXR.MixedReality.Toolkit.XvXR.Input.XvXRJoystick;

namespace XvXRFoundation
{
    public class XvHandleController : MonoBehaviour
    {
        public XvXRJoystick XvXRJoystick
        {
            private set;
            get;
        }

        //手柄在Unity中的坐标原点
        private Vector3 originPosition = Vector3.zero;
        private Quaternion originRotation = Quaternion.identity;


        //手柄当前在Unity中的坐标
        private Vector3 currentPosition = Vector3.zero;
        private Quaternion currentRotation = Quaternion.identity;


        
        private Matrix4x4 realOrigin;

        private Matrix4x4 virOrigin;
        [SerializeField]
        private JoystickButton joystickButton= JoystickButton.Button_A;

        [SerializeField]
        private string serialNumber;

        public string SerialNumber { 
            get { return serialNumber; } 
        
        }


        private void Awake()
        {
            XvXRJoystick = GetComponent<XvXRJoystick>();
            cameraTran = Camera.main.transform;
            ResetCenter();
        }

        private void Update()
        {
            if (XvXRJoystick.TrackerType == TrackerType.Left)
            {
                if (XvJoystickManager.Instance.GetKeyDown(joystickButton, TrackerType.Left))
                {
                    ResetCenter();
                }
            }
            else
            {
                if (XvJoystickManager.Instance.GetKeyDown(joystickButton, TrackerType.Right))
                {
                    ResetCenter();
                    MyDebugTool.Log("GetKeyDown:B");
                }
            }
        }


        private void LateUpdate()
        {


            UpdatePose();

        }

        private void UpdatePose()
        {
            JoystickData joystickData = GetJoystickData();

            //真实手柄的坐标原点Quaternion
            Quaternion realOringinQua = GetQuaternionByMatrix(realOrigin);

            //真实手柄Quaternion
            Quaternion realCurrentQua = Quaternion.Euler(joystickData.rotation);


            //Unity坐标原点的Quaternion
            Quaternion virOringinQua = GetQuaternionByMatrix(virOrigin);


            currentRotation = virOringinQua * (Quaternion.Inverse(realOringinQua) * realCurrentQua);


            currentPosition = virOrigin.GetColumn(3) + virOrigin * (realOrigin.inverse.MultiplyPoint(joystickData.position));
        }


        private Quaternion GetQuaternionByMatrix(Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }
        private Vector3 GetScale(Matrix4x4 matrix)
        {
            return new Vector3(
                matrix.GetColumn(0).magnitude,
                matrix.GetColumn(1).magnitude,
                matrix.GetColumn(2).magnitude);
        }
        private Vector3 GetPosition(Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        private Transform cameraTran;

       

        public Vector3 GetPosition()
        {
            return currentPosition;

        }
        public Quaternion GetRotation()
        {
            return currentRotation;
        }

        public JoystickData GetJoystickData()
        {
            return XvXRJoystick.GetJoystickData();
        }

        public void ResetCenter()
        {
            Vector3 dir = Vector3.ProjectOnPlane(cameraTran.forward, Vector3.up).normalized;


            Vector3 pos = cameraTran.position + dir * 0.5f - Vector3.up * 0.2f;
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);


            SetOrigin(pos, rot);
            JoystickData joystickData = GetJoystickData();


            //真实手柄坐标原点矩阵
            realOrigin = Matrix4x4.TRS(joystickData.position, Quaternion.Euler(joystickData.rotation), Vector3.one);


            //虚拟手柄坐标原点矩阵
            virOrigin = Matrix4x4.TRS(originPosition, originRotation, Vector3.one);
        }



        private void SetOrigin(Vector3 position, Quaternion rotation)
        {
            originPosition = position;
            originRotation = rotation;

        }

    }
}