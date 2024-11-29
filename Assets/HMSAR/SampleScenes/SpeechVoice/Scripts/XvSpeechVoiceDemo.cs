using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XvXRFoundation;

public class XvSpeechVoiceDemo : MonoBehaviour
{
    public XvSpeechVoiceManager xvSpeechVoiceManager;
    private bool isRotate;
    public Transform obj;
    public AudioSource audioSource;
    public AudioClip voiceWake;
    public AudioClip voiceEnd;
    public GameObject StopCurrent;
    public Text modeText;

    [SerializeField]
    [Tooltip(" «∑Ò π”√”Ô“ÙªΩ–—")]

    private bool isAvw = true;

    private void Awake()
    {
        if (xvSpeechVoiceManager==null) {
            xvSpeechVoiceManager=GameObject.FindObjectOfType<XvSpeechVoiceManager>();
        }
        modeText.text = isAvw ? "”Ô“ÙªΩ–—ƒ£ Ω" : "√¸¡Ó¥ ƒ£ Ω";

        StopCurrent.SetActive(isAvw);
    }

    private void Update()
    {
        if (isRotate) {
            obj.rotation *= Quaternion.Euler(Vector3.up*Time.deltaTime*50);
        }

      
    }
    public void StartRotate() {
        isRotate = true;
    }
    public void StopRotate()
    {
        isRotate = false;
    }
    public void ScaleUp()
    {
        
        obj.localScale *= 1.1f;
    }
    public void ScaleDown()
    {
        obj.localScale *= 0.9f;
    }

    public void  StartSpeechRecognition() {

        if (isAvw)
        {

            xvSpeechVoiceManager.StartAVW(5000,500);
        }
        else {
            xvSpeechVoiceManager.StartASR(5000,500);
        }
    }

    public void StopSpeechRecognition()
    {
        if (isAvw)
        {
            xvSpeechVoiceManager.StopAVW();
        }
        else {
            xvSpeechVoiceManager.StopASR();
        }

    }
    public void StopCurrentSpeechRecognition() {
        xvSpeechVoiceManager.StopASR();
    }


    public void PlayVoiceWake() {
        if (audioSource!=null&& voiceWake!=null) {
          
            audioSource.PlayOneShot(voiceWake);
        }
    }


    public void RecognitionMode() {
        isAvw = !isAvw;
        modeText.text = isAvw ? "”Ô“ÙªΩ–—ƒ£ Ω" : "√¸¡Ó¥ ƒ£ Ω";
        StopSpeechRecognition();
        StartSpeechRecognition();
    }

    public void PlayVoiceEnd()
    {
        if (audioSource != null && voiceEnd!=null)
        {
            audioSource.PlayOneShot(voiceEnd);
        }
    }


    public void OnRecognizedStatus(result result) {

        MyDebugTool.Log("OnRecognizedStatus:" + result.word);
    
    }


}
