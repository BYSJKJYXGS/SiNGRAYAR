using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMS.Utils;
using System.Text;

using UnityEngine.UI;

using UnityEditor;

using System;
using System.Runtime.InteropServices;

namespace HMS.Core
{
    public class ReadFisheyesCalibration : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ReadStereoFisheyesCalibration()
        {
            MyDebugTool.Log("ReadStereoFisheyesCalibration start xxxx");
            HMS.Engine.XvDeviceManager.Manager.ReadStereoFisheyesCalibration();
        }

    }
}