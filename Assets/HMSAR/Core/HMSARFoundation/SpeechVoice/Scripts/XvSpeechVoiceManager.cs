using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.Events;
using HMS.Core;
using HMS.Utils;

namespace XvXRFoundation
{
    public class result {
        public  string word;//命令词
        public int id;// id
        public int sc;//置信度
    }
   /// <summary>
   /// 提供语音识别的配置方法以及识别接口
   /// </summary>
    public sealed class XvSpeechVoiceManager : MonoBehaviour
    {
        private AndroidJavaObject interfaceObject;

        private AndroidJavaObject InterfaceObject
        {
            get
            {
                if (interfaceObject == null)
                {
                    AndroidJavaClass activityClass = XvAndroidHelper.GetClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activityObject = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    if (activityObject != null)
                    {
                        interfaceObject = XvAndroidHelper.Create("com.xv.aitalk.UnityInterface", new object[] { activityObject });
                    }
                }
                return interfaceObject;
            }
        }

        private XvSpeechVoiceManager() { }

        private const string ENGINE_TYPE = "local";

        private string LOCAL_BNF = "#BNF+IAT 1.0 UTF-8;\n"
        + "!grammar word;\n"
        + "!slot <words>;\n"
        + "!start <words>;\n";
        private const string LOCAL_GRAMMAR = "word";

        private const string LOCAL_THRESHOLD = "60";
        private UnityAction<RecognizedStatus,string> OnRecognizedStatus;//所有的语音状态

        private result result1 = new result();

        /// <summary>
        /// 识别到的语音词命令 id  word
        /// </summary>
        [SerializeField]
        private List<AitalkWord> aitalkWords = new List<AitalkWord>();

        /// <summary>
        /// 命令词识别结果回调
        /// </summary>

        public UnityEvent<result> OnCommandRecognized;
        /// <summary>
        /// 唤醒词唤醒回调
        /// </summary>
        public UnityEvent OnSpeechRecognizeWake;

        /// <summary>
        /// 命令词静默超时回调
        /// </summary>
        public UnityEvent OnSpeechRecognizeEnd;


       

        [Tooltip("置信度阈值")]
        [Range(0,100)]
        [SerializeField]
        private int sc=20;

       


        void Start()
        {
            this.name = "Aitalk";

            StringBuilder stringBuilder = new StringBuilder("<words>:");

            for (int i = 0; i < aitalkWords.Count; i++)
            {
                if (i == aitalkWords.Count - 1)
                {
                    
                    stringBuilder.Append(string.Format("{0}!id({1});\n", aitalkWords[i].word, aitalkWords[i].id));

                }
                else
                {
                    stringBuilder.Append(string.Format("{0}!id({1})|", aitalkWords[i].word, aitalkWords[i].id));

                }
            }

            LOCAL_BNF += stringBuilder.ToString();

            MyDebugTool.Log(LOCAL_BNF);
#if UNITY_EDITOR
            return;
#endif

            //Invoke("StartAVW", 3);
        }


        /// <summary>
        /// 开启命令词监听，非唤醒词模式下调用
        /// </summary>
        public void StartASR(int local_VAD_BOS = 5000, int LOCAL_VAD_EOS = 500)
        {
            MyDebugTool.Log("StartASR");

            try
            { 
            
            UpdateText("startASR");
            AndroidHelper.CallObjectMethod(InterfaceObject, "init", new object[] { });
            UpdateText("init");

            AndroidHelper.CallObjectMethod(InterfaceObject, "buildGrammar", new object[] { ENGINE_TYPE, LOCAL_BNF });
            UpdateText("buildGrammar");

            AndroidHelper.CallObjectMethod(InterfaceObject, "setParam", new object[] { ENGINE_TYPE, LOCAL_GRAMMAR, LOCAL_THRESHOLD, local_VAD_BOS.ToString(),LOCAL_VAD_EOS .ToString() });
            UpdateText("setParam");

             AndroidHelper.CallObjectMethod(InterfaceObject, "setUseKeepAlive", new object[] { true });//

                AndroidHelper.CallObjectMethod(InterfaceObject, "startASR", new object[] { });
            UpdateText("startASREnd");
            }catch (Exception e)
            {

                MyDebugTool.Log("aitak_log:Exception  " + e.Message);
            }

        }


        /// <summary>
        /// 关闭命令词监听
        /// </summary>
        public void StopASR() { 
            MyDebugTool.Log("StopASR");

            AndroidHelper.CallObjectMethod(InterfaceObject, "stopASR", new object[] { });

        }

        /// <summary>
        /// 开启唤醒词+命令词识别
        /// </summary>
        public void StartAVW(int local_VAD_BOS=5000,int LOCAL_VAD_EOS=500)
        {
            MyDebugTool.Log("StartAVW");

            AndroidHelper.CallObjectMethod(InterfaceObject, "init", new object[] { });
            //  AndroidHelper.CallObjectMethod(InterfaceObject, "buildWakeUpGrammar", new object[] { ENGINE_TYPE, LOCAL_WAKE_BNF });
            AndroidHelper.CallObjectMethod(InterfaceObject, "buildGrammar", new object[] { ENGINE_TYPE, LOCAL_BNF });
            AndroidHelper.CallObjectMethod(InterfaceObject, "setParam", new object[] { ENGINE_TYPE, LOCAL_GRAMMAR, LOCAL_THRESHOLD, local_VAD_BOS.ToString() , LOCAL_VAD_EOS .ToString()});
            AndroidHelper.CallObjectMethod(InterfaceObject, "setUseKeepAlive", new object[] { false });//
            AndroidHelper.CallObjectMethod(InterfaceObject, "startAvw", new object[] { false });//
        }

        /// <summary>
        /// 停止唤醒词+命令词识别
        /// </summary>
        public void StopAVW() {
            MyDebugTool.Log("StopAVW");
            AndroidHelper.CallObjectMethod(InterfaceObject, "stopAvw", new object[] {  });//

        }

        /// <summary>
        /// 唤醒词回调
        /// </summary>
        /// <param name="result"></param>
        public void onAvwResult(string result)
        {
            MyDebugTool.Log("aitak_log:unity:LogInfo:onAvwResult:" + result);
            // awvResult data = SimpleJson.SimpleJson.DeserializeObject<awvResult>(result, new JsonSerializerStrategy());
          
            AndroidHelper.CallObjectMethod(InterfaceObject, "startASR", new object[] { });

            OnSpeechRecognizeWake?.Invoke();
        }

        /// <summary>
        /// 自动监听回调命令词停止回调
        /// </summary>
        /// <param name="msg"></param>
        public void onSpeechRecognizeEnd(string msg)
        {
            MyDebugTool.Log("aitak_log:unity:LogInfo:onSpeechRecognizeEnd:" + msg);
            OnSpeechRecognizeEnd?.Invoke();
        }

        
       

        private void UpdateText(string text)
        {
           MyDebugTool.Log(text);
            //statusText.text = text;
        }

        /// <summary>
        /// 语音初始化完成回到
        /// </summary>
        /// <param name="result"></param>
        public void onInit(string result)
        {
            OnRecognizedStatus?.Invoke(RecognizedStatus.Init, result);

            MyDebugTool.Log("aitak_log:unity:LogInfo:onInit:" + result);
            UpdateText("init" + result);
        }

        /// <summary>
        /// 语法构建完成回调
        /// </summary>
        /// <param name="result"></param>
        public void onBuildFinish(string result)
        {

            MyDebugTool.Log("aitak_log:unity:LogInfo:onBuildFinish:" + result);
            string[] strArray = result.Split('|');
            if (strArray.Length > 1)
            {
                UpdateText("onBuildFinish:" + strArray[1]);
                OnRecognizedStatus?.Invoke(RecognizedStatus.BuildSuccess, result);


            }
            else
            {
                UpdateText("onBuildFinish:false");
                OnRecognizedStatus?.Invoke(RecognizedStatus.BuildFail, result);

            }

        }

        /// <summary>
        /// 开始监听语音输入回调
        /// </summary>
        /// <param name="nullstr"></param>
        public void onBeginOfSpeech(string nullstr)
        {
            MyDebugTool.Log("aitak_log:unity:LogInfo:onBeginOfSpeech.....");
            UpdateText("说话中....");

            OnRecognizedStatus?.Invoke(RecognizedStatus.BeginOfSpeech,nullstr);
        }

        /// <summary>
        /// 语音输入结束回调
        /// </summary>
        /// <param name="nullstr"></param>
        public void onEndOfSpeech(string nullstr)
        {
            MyDebugTool.Log("aitak_log:unity:LogInfo:onEndOfSpeech.....");
            UpdateText("说话结束");
            OnRecognizedStatus?.Invoke(RecognizedStatus.EndOfSpeech,nullstr);

        }

        /// <summary>
        /// 错误回调
        /// </summary>
        /// <param name="error"></param>
        public void onError(string error)
        {
            OnRecognizedStatus?.Invoke(RecognizedStatus.Error, error);

            Debug.LogError("aitak_log:unity:LogError:" + error);
            UpdateText(error);
        }


        /// <summary>
        /// 命令词识别回调
        /// </summary>
        /// <param name="result"></param>
        public void onResult(string result)
        {
            OnRecognizedStatus?.Invoke(RecognizedStatus.Result, result);

            MyDebugTool.Log("aitak_log:unity:LogInfo:onResult:" + result);
            string[] strArray = result.Split('|');
            if (strArray.Length > 1)
            {

                if ("true" == strArray[1])
                {
                    try
                    {
                        XvAitalkModels.result data = SimpleJson.SimpleJson.DeserializeObject<XvAitalkModels.result>(result, new JsonSerializerStrategy());
                        if (data != null)
                        {
                            if (data.sc > sc)
                            {
                                for (int i = 0; i < aitalkWords.Count; i++)
                                {
                                    if (aitalkWords[i].id == data.ws[0].cw[0].id)
                                    {
                                        aitalkWords[i].action?.Invoke();

                                        result1.id = data.ws[0].cw[0].id;
                                        result1.word = aitalkWords[i].word;
                                        result1.sc = data.sc;

                                        OnCommandRecognized?.Invoke(result1);
                                        break;
                                    }
                                }
                            }
                            //UpdateText("识别结果:true，可信度:" + data.sc + ",内容:" + data.ws[0].cw[0].w + ",id:" + data.ws[0].cw[0].id);
                        }
                    }
                    catch (Exception e)
                    {
                        UpdateText("onResult:false");
                    }
                }
                else
                {
                    UpdateText("onResult:false");
                }
            }
            else
            {
                UpdateText("onResult:false");
            }


        }
        private class JsonSerializerStrategy : SimpleJson.PocoJsonSerializerStrategy
        {
            // convert string to int
            public override object DeserializeObject(object value, Type type)
            {
                if (type == typeof(Int32) && value.GetType() == typeof(string))
                {
                    return Int32.Parse(value.ToString());
                }
                return base.DeserializeObject(value, type);
            }
        }


    }

    [Serializable]
    public class AitalkWord
    {
        public int id;
        public string word;
        public UnityEvent action;
    }


    public enum RecognizedStatus
    {
        None,
        Init,//初始化完成
        BuildSuccess,//构建成功
        BuildFail,//构建失败
        BeginOfSpeech,//开始说话
        EndOfSpeech,//结束输入
        Error,//
        Result,//

    }
}