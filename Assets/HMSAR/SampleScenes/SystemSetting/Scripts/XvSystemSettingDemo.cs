using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static XvXRFoundation.XvSystemSetting;

namespace XvXRFoundation
{
    public class XvSystemSettingDemo : MonoBehaviour
    {
        public XvSystemSettingManager settingManager;
        public Text brightnessValue;
        public Text ipdValue;

       


        private float ipd;

        private void Awake()
        {
            if (settingManager==null) {
                settingManager=FindObjectOfType<XvSystemSettingManager>();

                if (settingManager==null) {
                    settingManager = new GameObject("XvSystemSettingManager").AddComponent<XvSystemSettingManager>();
                }
            }
            brightnessValue = transform.Find("UI/Canvas/Brightness/brightnessValue").GetComponent<Text>();
            ipdValue = transform.Find("UI/Canvas/Ipd/IpdValue").GetComponent<Text>();

        
        }

        private void Start()
        {
            Invoke("Initialized", 2);
        }

        
        private void Initialized()
        {

            ipd=  (float)Math.Round(settingManager.GetIPD(), 1);
            SetIpd(ipd);

            brightnessValue.text = string.Format("{0}", settingManager.GetBrightnessLevel());
            ipdValue.text = string.Format("{0}mm", ipd);

          
        }

        public void BrightnessUp() {
           int level= settingManager.GetBrightnessLevel();
            level++;
            SetBrightness(level);
        }
        public void BrightnessDown()
        {
            int level = settingManager.GetBrightnessLevel();
            level--;
            SetBrightness(level);
        }
        public void SetBrightness(float value)
        {
            int level = (int)value;
            level = Mathf.Clamp(level, 0, 9);
           
            brightnessValue.text = string.Format("{0}", level);

           
            settingManager.SetBrightnessLevel(level);
        }
        public void IpdUp()
        {
            ipd += 1;
           
            SetIpd(ipd);
        }
        public void IpdDown()
        {
            ipd -= 1;
            SetIpd(ipd);
        }
        public void SetIpd(float value)
        {
            ipd = value;
          
            ipd = Mathf.Clamp(ipd, 55, 75);
            settingManager.SetIPD(ipd);
            ipdValue.text = string.Format("{0}mm", ipd);
          
            // ipdSlider.value = value;

        }


       

    }
}
