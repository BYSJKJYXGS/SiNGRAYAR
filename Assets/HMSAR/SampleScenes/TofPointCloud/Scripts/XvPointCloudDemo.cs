using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using TMPro;
using XvXRFoundation;
using HMS.Core;

public class XvPointCloudDemo : MonoBehaviour
{
    public XvParticlesCloudPoint particlesCloudPoint;
    Vector3[] vecGroup;

    API.pdm_calibration tofcalibration;
    public static Vector3 tofpos;
    public static Quaternion tofQua;
    static double[] rotation3x3 = new double[9];

    int width;
    int height;
    bool isGetTofData;


    public TextMeshProUGUI info;
    public Slider slider;


    public TextMeshProUGUI info_0;
    public Slider slider_0;

    public TextMeshProUGUI info_1;
    public Slider slider_1;

    public TextMeshProUGUI info_2;
    public Slider slider_2;

    public int v_0 = 4;
    public int v_1 = 1;
    public int v_2 = 5;
    public float v_3 = 0.2f;


    public TextMeshProUGUI vvv;

    [SerializeField]
    private XvCameraManager cameraManager;


    public XvCameraManager XvCameraManager
    {
        get
        {

            if (cameraManager == null)
            {
                cameraManager = FindObjectOfType<XvCameraManager>();
            }

            if (cameraManager == null)
            {
                cameraManager = new GameObject("XvCameraManager").AddComponent<XvCameraManager>();
            }
            return cameraManager;

        }
    }

   
   
    private void Start()
    {

    }

    private int countTime = 0;
   


    // Update is called once per frame
    void Update()
    {
       
        countTime++;
        if (countTime == 10)
        {
            countTime = 0;

            if (XvCameraManager.GetPointCloudData(out vecGroup))
            {
                particlesCloudPoint.gameObject.SetActive(true);
                particlesCloudPoint.StartDraw(vecGroup);
            }
        }

    }
    public void  StartTofPointCloud() { 
        XvCameraManager.StartTofPointCloud();

    }

    public void StopTofPointCloud()
    {
        particlesCloudPoint.gameObject.SetActive(false);
        XvCameraManager.StopTofPointCloud();
    }


    public void changeIpd0()
    {
        v_0 = int.Parse(slider_0.value.ToString());
       
        info_0.text = v_0 + " ";
    }

    public void changeIpd1()
    {
        v_1 = int.Parse(slider_1.value.ToString());
        
        info_1.text = v_1 + " ";
    }

    public void changeIpd2()
    {
        v_2 = int.Parse(slider_2.value.ToString());
      
        info_2.text = v_2 + " ";
    }


    public void changeIpd()
    {
        v_3 = slider.value;
       
        info.text = v_3 + " ";
    }

    public void tofSet()
    {
        XvCameraManager.SetTofExposure(v_0, v_1, v_2, v_3);
       
    }

   
}
