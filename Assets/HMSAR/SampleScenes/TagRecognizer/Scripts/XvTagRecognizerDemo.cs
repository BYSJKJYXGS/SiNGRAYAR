using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XvXRFoundation
{
    public class XvTagRecognizerDemo : MonoBehaviour
    {
        public void OnFoundEvent(XvTagRecognizerBehavior xvTagRecognizerBehavior)
        {
            Transform TagCenter = xvTagRecognizerBehavior.transform.Find("TagCenter");
            char[] chars= xvTagRecognizerBehavior.TagDetection.qrcode;
            TagCenter.Find("msg").GetComponent<TextMesh>().text =new string(chars);
        }
        public void OnLostEvent(XvTagRecognizerBehavior xvTagRecognizerBehavior)
        {

        }
    }
}
