using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace XvXRFoundation
{
    public class XvTagRecognizerUIController : MonoBehaviour
    {
        public XvTagRecognizerManager tagManager;
        public Text TagGroupNameTxt;

        public Text checkTxt;
        public Text typeTxt;
        

        public InputField apriltagSizeInputTxt;
        // public TextMesh cubePos;


        public GameObject fisheyeBtn;
        public GameObject rgbBtn;

        private void Awake()
        {
            if (tagManager == null)
            {
                tagManager = FindAnyObjectByType<XvTagRecognizerManager>();
            }
            if (tagManager == null)
            {

                Debug.LogError("AprilTag:Manager==null");
                return;
            }
            if (tagManager.CameraType == CameraType.FishEye)
            {
                typeTxt.text = "”„—€ºÏ≤‚";
            }
            else
            {
                typeTxt.text = "RgbºÏ≤‚";

            }
            apriltagSizeInputTxt.text = tagManager.Size.ToString();

            checkTxt.text = tagManager.IsDetect.ToString();


            TagGroupNameTxt.text = tagManager.TagGroupName;
        }
        public void btnClick(GameObject btn)
        {

            if (tagManager == null)
            {

                Debug.LogError("AprilTag:Manager==null");
                return;
            }

            switch (btn.name)
            {
                case "36h11Btn":
                    tagManager.TagGroupName = "36h11";
                    TagGroupNameTxt.text = tagManager.TagGroupName;
                    fisheyeBtn.SetActive(true);
                    rgbBtn.SetActive(true);
                    break;
                case "qrcodeBtn":
                    tagManager.TagGroupName = "qr-code";
                    TagGroupNameTxt.text = tagManager.TagGroupName;
                    fisheyeBtn.SetActive(false);
                    rgbBtn.SetActive(true);
                    break;
                case "CheckBtn":
                    if (tagManager.IsDetect == false)
                    {
                        tagManager.IsDetect = true;
                    }
                    else
                    {
                        tagManager.IsDetect = false;
                    }
                    checkTxt.text = tagManager.IsDetect.ToString();
                    break;
                case "4cmBtn":
                    tagManager.Size = 0.04;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "6cmBtn":
                    tagManager.Size = 0.06;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "8cmBtn":
                    tagManager.Size = 0.08;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "10cmBtn":
                    tagManager.Size = 0.1;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "12cmBtn":
                    tagManager.Size = 0.12;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "14cmBtn":
                    tagManager.Size = 0.14;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "16cmBtn":
                    tagManager.Size = 0.16;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "18cmBtn":
                    tagManager.Size = 0.18;
                    apriltagSizeInputTxt.text = tagManager.Size.ToString();
                    break;
                case "SizeBtn":
                    if (apriltagSizeInputTxt != null)
                    {
                        tagManager.Size = float.Parse(apriltagSizeInputTxt.text);
                    }

                    break;
                case "FishEyeBtn":

                    tagManager.CameraType = CameraType.FishEye;
                    typeTxt.text = "”„—€ºÏ≤‚";
                    break;
                case "RgbBtn":
                    tagManager.CameraType = CameraType.Rgb;
                    typeTxt.text = "RgbºÏ≤‚";
                    break;
            }
        }



    }
}
