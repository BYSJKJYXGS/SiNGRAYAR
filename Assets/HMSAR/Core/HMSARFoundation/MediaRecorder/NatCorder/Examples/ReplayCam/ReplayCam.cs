/* 
*   NatCorder
*   Copyright (c) 2020 Yusuf Olokoba
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using System.Collections;
    using System.IO;
    using Recorders;
    using Recorders.Clocks;
    using Recorders.Inputs;
    using System;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public class ReplayCam : SingletonMonoBehaviour<ReplayCam> {

       
        [HideInInspector]
        internal int videoWidth = 1920;
        internal int videoHeight = 1080;
        public bool recordMicrophone;

        private bool isRecording;

        private IMediaRecorder recorder;
        private CameraInput cameraInput;
        private AudioInput audioInput;
        private AudioSource microphoneSource;
        private int countTime = 0;
       

        public AudioListener audioListener;
        [HideInInspector]
        internal Camera cam;
      
        //public Text infoTxt;
        //public VideoManagerControl videoManageControl;

        private IEnumerator Start () {
            audioListener = FindObjectOfType<AudioListener>();

            // Start microphone
            microphoneSource = gameObject.AddComponent<AudioSource>();
            microphoneSource.mute =
            microphoneSource.loop = true;
            microphoneSource.bypassEffects =
            microphoneSource.bypassListenerEffects = false;
            if(recordMicrophone)
                microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
            microphoneSource.Play();
        }

        private void OnDestroy () {
            // Stop microphone
            if (recordMicrophone)
            {
                microphoneSource.Stop();
                Microphone.End(null);
            }               
        }

       
        public void StartRecording()
        {
            if (isRecording) {
                return;
            }

            isRecording = true;
           
            // Start recording
            var frameRate = 30;
            //var sampleRate = recordMicrophone ? AudioSettings.outputSampleRate : 0;
            //var channelCount = recordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var sampleRate = AudioSettings.outputSampleRate;
            var channelCount = (int)AudioSettings.speakerMode;
            var clock = new RealtimeClock();
            recorder = new MP4Recorder(videoWidth, videoHeight, frameRate, sampleRate, channelCount);
            // Create recording inputs
            cameraInput = new CameraInput(recorder, clock, cam);
            //audioInput = recordMicrophone ? new AudioInput(recorder, clock, microphoneSource, true) : null;
            audioInput = new AudioInput(recorder, clock, audioListener);
            // Unmute microphone
            microphoneSource.mute = audioInput == null;


          
            MyDebugTool.Log("StartRecording");
        
        }

        public async void StopRecording (UnityAction<string> callback) {

            if (!isRecording)
            {
                return;
            }
            isRecording = false;
           

            // Mute microphone
            microphoneSource.mute = true;
            // Stop recording
            audioInput?.Dispose();
            cameraInput?.Dispose();
            var path = await recorder.FinishWriting();
            // Playback recording
            MyDebugTool.Log($"Saved recording to: {path}");
           // var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
            //infoTxt.text = path;
            // 移动端，录频完后，播放录制的视频
            //Handheld.PlayFullScreenMovie($"{prefix}{path}");


#if PLATFORM_ANDROID && !UNITY_EDITOR
            if (Directory.Exists("/storage/emulated/0/DCIM/ScreenRecorder"))
            {
            }
            else
            {
                Directory.CreateDirectory("/storage/emulated/0/DCIM/ScreenRecorder");
            }
            string path2 = "/storage/emulated/0/DCIM/ScreenRecorder/"+ getDate() + ".mp4";
            File.Copy(path, path2);

            callback?.Invoke(path2);
#endif

#if UNITY_EDITOR
            string dirPath = Path.GetFileName(path);


            if (!Directory.Exists(Application.dataPath + "/Screenshots"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Screenshots");
            }
            string path_configData = Application.dataPath + "/Screenshots/" + dirPath;
        
            Debug.LogError(path_configData);
            File.Copy(path, path_configData);
            callback?.Invoke(path_configData);

#endif
        }

        public void RemoveRecording()
        {
           // if (File.Exists(m_resultPath))
            {

               // File.Delete(m_resultPath);
            }
        }

        private static string getDate()
        {
            string str = DateTime.Now.Year.ToString();
            if (DateTime.Now.Month.ToString().Length == 1)
            {
                str += "0" + DateTime.Now.Month;
            }
            else
            {
                str += DateTime.Now.Month;
            }
            if (DateTime.Now.Day.ToString().Length == 1)
            {
                str += "0" + DateTime.Now.Day;
            }
            else
            {
                str += DateTime.Now.Day;
            }
            if (DateTime.Now.Hour.ToString().Length == 1)
            {
                str += "0" + DateTime.Now.Hour;
            }
            else
            {
                str += DateTime.Now.Hour;
            }
            if (DateTime.Now.Minute.ToString().Length == 1)
            {
                str += "0" + DateTime.Now.Minute;
            }
            else
            {
                str += DateTime.Now.Minute;
            }
            return str;
        }
    }
}