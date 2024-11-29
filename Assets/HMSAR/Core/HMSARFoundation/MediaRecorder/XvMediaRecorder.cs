using System.Collections;
using UnityEngine;


namespace XvXRFoundation
{
    /// <summary>
    /// 依赖于XvMediaRecorderManager类，
    /// 该类主要实现通过菜单进行本地视频的录制和截图功能
    /// </summary>
    public class XvMediaRecorder : MonoBehaviour
    {
        [SerializeField]
        private XvMediaRecorderManager xvMediaRecorderManager;

        public XvMediaRecorderManager XvMediaRecorderManager { 
        get {
                if (xvMediaRecorderManager == null)
                {
                    xvMediaRecorderManager = FindFirstObjectByType<XvMediaRecorderManager>();
                    if (xvMediaRecorderManager == null)
                    {

                        GameObject newObj = new GameObject("XvMediaRecorderManager");
                        xvMediaRecorderManager = newObj.AddComponent<XvMediaRecorderManager>();

                    }
                }
                return xvMediaRecorderManager;
            }
        }
        public TextMesh tips;

        private void OnEnable()
        {

            ///录制之前需要打开MR视频捕捉功能
            XvMediaRecorderManager.StartCapture();
        }

        private void OnDisable()
        {
            XvMediaRecorderManager.StopCapture(false);
        }

        /// <summary>
        /// 开始录制本地视频
        /// </summary>
        public void StartRecording()
        {
            StopAllCoroutines();
            CancelInvoke();
            StartCoroutine(RelayRecording());
        }

        /// <summary>
        /// 停止本地视频录制
        /// </summary>
        public void StopRecording()
        {
            StopAllCoroutines();
            CancelInvoke();
            xvMediaRecorderManager.StopRecording((filePath) =>{
            tips.gameObject.SetActive(true);

                tips.text = "存储路径："+filePath;

                Invoke("HideTips", 1);

            });
        }

        /// <summary>
        /// 截图保存
        /// </summary>
        public void SaveScreenshot()
        {
            StopAllCoroutines();
            CancelInvoke();
            StartCoroutine(RelayScreenshot());
        }



        private IEnumerator RelayRecording() {
            tips.gameObject.SetActive(true);
            tips.text = "准备录制";

            yield return new WaitForSeconds(1);

            for (int i = 3; i >=0; i--)
            {
                tips.text = i.ToString();
                yield return new WaitForSeconds(0.5f);
            }

            
            tips.text = "录制中";
            XvMediaRecorderManager.StartRecording();
        }
        private IEnumerator RelayScreenshot()
        {
            tips.gameObject.SetActive(true);
            tips.text = "准备中";

            yield return new WaitForSeconds(1);

            for (int i = 3; i >= 0; i--)
            {
                tips.text = i.ToString();
                yield return new WaitForSeconds(0.5f);
            }

            tips.text = "截图中";

            XvMediaRecorderManager.SaveScreenshot((filePath) => {
            tips.gameObject.SetActive(true);

                tips.text = "存储路径：" + filePath;
               
                Invoke("HideTips",1);
            });
        }


        private void HideTips() {
            tips.gameObject.SetActive(false);

        }
       
    }
}
