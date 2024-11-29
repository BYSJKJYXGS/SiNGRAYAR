using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using HMS.Utils;

public class UvcPluginWrapper : MonoBehaviour
{

    private static int renderInit = 0x22;
    private static int renderDraw = 0x24;



    protected const string dllName = "UvcPlugin";
 

    [DllImport(dllName)]
    private static extern IntPtr GetRenderEventFunc();



    private AndroidJavaObject interfaceObject;

    private Texture2D playPlaneTexture = null;

    public GameObject playGameObject = null;

    public int videoWidth = 1920;
    public int videoHeight = 1080;

 

    private AndroidJavaObject InterfaceObject
    {
        get
        {
            if (interfaceObject == null)
            {
                AndroidJavaClass activityClass = AndroidHelper.GetClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activityObject = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (activityObject != null)
                {
                    AndroidJavaClass interfaceClass = AndroidHelper.GetClass("com.xv.uvcplugin.UnityInterface");

                    interfaceObject = interfaceClass.CallStatic<AndroidJavaObject>("getInstance", new object[] { activityObject });
                }
            }
            return interfaceObject;
        }
    }

    IEnumerator Start()
    {
      
        if (Application.platform == RuntimePlatform.Android)
        {
           
            AndroidHelper.CallObjectMethod(InterfaceObject, "nativeInit", new object[] { });
            
            CreateTextureToPlayStream();
            GL.IssuePluginEvent(GetRenderEventFunc(), renderInit);
            yield return StartCoroutine("CallPluginAtEndOfFrames");
        }
        else
        {
            CreateTextureToPlayStream();
        }
    }

   

    private void CreateTextureToPlayStream()
    {
        if (playPlaneTexture == null)
        {
            playPlaneTexture = new Texture2D(videoWidth, videoHeight);
            playPlaneTexture.Apply();
            
        }
        if (playGameObject != null)
        {
            playGameObject.GetComponent<Renderer>().material.mainTexture = playPlaneTexture;
        }
        SetCameraTexture();

       }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();
            GL.IssuePluginEvent(GetRenderEventFunc(), renderDraw);
         

        }
    }

    private void Update()
    {
       
       
    }


    private void SetCameraTexture()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidHelper.CallObjectMethod(InterfaceObject, "setPlayStreamTexture", new object[] { (int)playPlaneTexture.GetNativeTexturePtr(), videoWidth, videoHeight });
        }
    }




}
