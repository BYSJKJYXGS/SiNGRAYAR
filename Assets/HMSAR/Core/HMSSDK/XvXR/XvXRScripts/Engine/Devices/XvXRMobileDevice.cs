using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HMS.SystemEvents;
using System;
using HMS.Utils;
using Assets.XvXRScripts.Engine;

namespace HMS.Engine
{
    public abstract class XvXRMobileDevice:XvXRBaseDevice
    {

        protected const int jniEnvInitEventId = 0x66660;//call java init the jni evn for android

        protected const int renderEventId = 0x666666;//render draw event 

        protected const int initRenderEventId = 0x666667;//init render

        protected const int changeRenderDataEventId = 0x666668;//change render data

        protected const int copyRederEventId = 0x7666666;

        protected int lastLeftId = 0;

        protected int lastRightId = 0;


        public abstract void  ReadConfigInfo();
        /// <summary>
        /// 更新参数并计算投影矩阵设置到android java库
        /// </summary>
        public override void UpdateScreenData()
        {
            XvXRLog.InternalXvXRLog("mobile test UpdateScreenData");
            //读取device mParameter参数到XvXRConfigInfo.parmeter,device mParameter是在deviceAattach时从glass获取的
            ReadConfigInfo();
            //计算出双眼的投影矩阵,这里是计算出默认的投影矩阵,赋值给leftEyeProjection,rightEyeProjection,recommendedTextureSize
            //这里要看XvXRAndroidDevice.cs里的重载定义
            ComputeEyesFromProfile();
            //设置Info.parameter和上面计算好的投影矩阵到glass，并发送changeRenderDataEventId事件到glass
            ChangeRenderData();


        }

        public override void SetStereoScreen(RenderTexture leftRenderTexture, RenderTexture rightRenderTexture)
        {


            lastLeftId = leftRenderTexture != null ? (int)leftRenderTexture.GetNativeTexturePtr() : 0;
            lastRightId = rightRenderTexture != null ? (int)rightRenderTexture.GetNativeTexturePtr() : 0;


            SetTextureIdMobile();
        }

        protected void CheckTextureId()//has the problem 
        {
            if (XvXRManager.SDK.StereoScreen != null)
            {
                if (lastLeftId != (int)XvXRManager.SDK.StereoScreen[0].GetNativeTexturePtr() || lastRightId != (int)XvXRManager.SDK.StereoScreen[1].GetNativeTexturePtr())
                {
                    //XvXRLog.InternalXvXRLog(" CheckTextureId:lastleft:" + lastLeftId + ",lastright:" + lastRightId + ",stereoscreenleftid:" + (int)XvXRManager.SDK.StereoScreen[0].GetNativeTexturePtr() + ",steroscreenrightid:" + (int)XvXRManager.SDK.StereoScreen[1].GetNativeTexturePtr());
                    SetStereoScreen(XvXRManager.SDK.StereoScreen[0], XvXRManager.SDK.StereoScreen[1]);
                }
            }
        }
        public override void SetDistortionCorrectionEnabled(bool enabled)
        {

        }

        public override void Recenter()
        {

        }


        public void ChangeRenderData()
        {
            XvXRLog.InternalXvXRLog("ChangeRenderData");
            //设置Info.parameter到glass
            SetRenderDataMobile();
            //发送事件到glass
            GL.IssuePluginEvent(RenderEventFunc(), changeRenderDataEventId);

        }

        public override void PostRender()
        {
            GL.IssuePluginEvent(RenderEventFunc(), renderEventId);
        }

        public void InitRenderEvent()
        {
            InitRenderMobile(1);//0: backbuffer ; 1 singlebuffer
            GL.IssuePluginEvent(RenderEventFunc(), initRenderEventId);
        }


        internal abstract void InitRenderMobile(int bufferMode);

        internal abstract void SetRenderDataMobile();

        internal abstract void SetTextureIdMobile();

        internal abstract IntPtr RenderEventFunc();




    }
}
