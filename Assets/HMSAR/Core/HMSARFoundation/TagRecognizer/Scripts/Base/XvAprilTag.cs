using HMS.Core;
using UnityEngine;
namespace XvXRFoundation
{
    /// <summary>
    /// 识别结果
    /// </summary>
    public class TagDetection
    {
        public int id;//ID

        public Vector3 translation;//位置

        public Vector3 rotation;

        public Vector4 quaternion;//旋转

        public float confidence;//识别率，范围0-1
       
        public char[] qrcode; //二维码信息
    }

    public class XvAprilTag
    {
        /// <summary>
        /// 开启Apritag的鱼眼检测模式
        /// </summary>
        /// <param name="tagFamily">
        /// AprilTag检测tagFamily传参36h11
        /// QRCode检测tagFamily传参qr-code</param>
        /// <param name="size">识别码的物理尺寸，单位米</param>
        /// <returns></returns>
        public static TagDetection[] StartFishEyeDetector(string tagFamily, double size)
        {
            if (!API.xslam_ready())
            {
                Debug.LogError("xslam没有准备好");
                return null;
            }
            Debug.LogError("wayland =2= fisheye");

            API.TagData tags = default(API.TagData);
            //int len = API.xslam_start_detect_tags(tagFamily, size, ref tags, 64);
            int len = API.xslam_detect_tags(tagFamily, size, ref tags, 64);
            if (len <= 0)
            {
                return null;
            }
            Debug.LogError("wayland =3= fisheye");

            TagDetection[] result = new TagDetection[len];
            MyDebugTool.Log("AprilTag##StartDetector tags size:" + len);
            for (int i = 0; i < len; i++)
            {
                API.DetectData tag = tags.detect[i];
                // Debug.Log("AprilTag##StartDetector tag position:(" + tag.position.x + "," + tag.position.y + "," + tag.position.z + ")");
                // Debug.Log("AprilTag##StartDetector tag orientation:(" + tag.orientation.x + "," + tag.orientation.y + "," + tag.orientation.z + ")");
                // Debug.Log("AprilTag##StartDetector tag quaternion:(" + tag.quaternion.x + "," + tag.quaternion.y + "," + tag.quaternion.z + "," + tag.quaternion.w + ")");

                TagDetection detection = new TagDetection();
                detection.id = tag.tagID;
                detection.translation = new Vector3(tag.position.x, tag.position.y, tag.position.z);
                detection.rotation = new Vector3(tag.orientation.x, tag.orientation.y, tag.orientation.z);
                detection.quaternion = new Vector4(tag.quaternion.x, tag.quaternion.y, tag.quaternion.z, tag.quaternion.w);
                detection.confidence = tag.confidence;
                result[i] = detection;

                MyDebugTool.Log("AprilTag##StartDetector detection translation:(" + detection.translation.x + "," + detection.translation.y + "," + detection.translation.z + ")");
                MyDebugTool.Log("AprilTag##StartDetector detection rotation:(" + detection.rotation.x + "," + detection.rotation.y + "," + detection.rotation.z + ")");
                MyDebugTool.Log("AprilTag##StartDetector detection quaternion:(" + detection.quaternion.x + "," + detection.quaternion.y + "," + detection.quaternion.z + "," + detection.quaternion.w + ")");
            }

            return result;
        }


        /// <summary>
        /// 关闭Apritag的鱼眼检测模式
        /// </summary>
        public static void StopFishEyeDetector()
        {
            if (!API.xslam_ready())
            {
                Debug.LogError("xslam没有准备好");
                return;
            }

            API.xslam_stop_detect_tags();
        }


        /// <summary>
        /// 开启Apritag的RGB相机检测模式
        /// </summary>
        /// <param name="tagFamily">
        /// AprilTag检测tagFamily传参36h11
        /// QRCode检测tagFamily传参qr-code</param>
        /// <param name="size">识别码的物理尺寸，单位米</param>
        /// <returns></returns>
        public static TagDetection[] StartRgbDetector(string tagFamily, double size)
        {
            if (!API.xslam_ready())
            {
                Debug.LogError("xslam没有准备好");
                return null;
            }

            API.TagData tags = default(API.TagData);
            int len = API.xslam_start_rgb_detect_tags(tagFamily, size, ref tags, 64);
            if (len <= 0)
            {
                return null;
            }

            TagDetection[] result = new TagDetection[len];
            MyDebugTool.Log("AprilTag##StartDetector tags size:" + len);
            for (int i = 0; i < len; i++)
            {
                API.DetectData tag = tags.detect[i];

                // Debug.Log("AprilTag##StartRgbDetector tag position:(" + tag.position.x + "," + tag.position.y + "," + tag.position.z + ")");
                // Debug.Log("AprilTag##StartRgbDetector tag orientation:(" + tag.orientation.x + "," + tag.orientation.y + "," + tag.orientation.z + ")");
                // Debug.Log("AprilTag##StartRgbDetector tag quaternion:(" + tag.quaternion.x + "," + tag.quaternion.y + "," + tag.quaternion.z + "," + tag.quaternion.w + ")");

                TagDetection detection = new TagDetection();

                if (detection.qrcode == null)
                {
                    detection.qrcode = new char[512];
                }

                detection.id = tag.tagID;
                detection.translation = new Vector3(tag.position.x, tag.position.y, tag.position.z);
                detection.rotation = new Vector3(tag.orientation.x, tag.orientation.y, tag.orientation.z);
                detection.quaternion = new Vector4(tag.quaternion.x, tag.quaternion.y, tag.quaternion.z, tag.quaternion.w);
                detection.confidence = tag.confidence;
                detection.qrcode = tag.qrcode;
                result[i] = detection;

                MyDebugTool.Log("AprilTag##StartRgbDetector detection translation:(" + detection.translation.x + "," + detection.translation.y + "," + detection.translation.z + ")");
                MyDebugTool.Log("AprilTag##StartRgbDetector detection rotation:(" + detection.rotation.x + "," + detection.rotation.y + "," + detection.rotation.z + ")");
                MyDebugTool.Log("AprilTag##StartRgbDetector detection quaternion:(" + detection.quaternion.x + "," + detection.quaternion.y + "," + detection.quaternion.z + "," + detection.quaternion.w + ")");


                
                try
                {
                    //for (int j = 0; j < detection.qrcode.Length; j++)
                    //{
                    //    Debug.Log($"AprilTag##StartRgbDetector detection qrcode[{j}]:{detection.qrcode[j]}");
                    //}
                    MyDebugTool.Log($"AprilTag##StartRgbDetector detection qrcode string:{new string(detection.qrcode)}");
                }
                catch (System.Exception e)
                {
                    MyDebugTool.Log($"AprilTag##StartRgbDetector Exception:{e}");
                    MyDebugTool.Log($"AprilTag##StartRgbDetector detection qrcode==null?:{detection.qrcode == null}");
                }

                 
            }

            return result;
        }

        /// <summary>
        /// 关闭Apritag的RGB相机检测模式
        /// </summary>
        public static void StopRgbDetector()
        {
            if (!API.xslam_ready())
            {
                Debug.LogError("xslam没有准备好");
                return;
            }

            API.xslam_stop_rgb_detect_tags();
        }
    }
}

