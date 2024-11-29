using System.Collections;
using UnityEngine;


namespace XvXRFoundation
{
    /// <summary>
    /// ������XvMediaRecorderManager�࣬
    /// ������Ҫʵ��ͨ���˵����б�����Ƶ��¼�ƺͽ�ͼ����
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

            ///¼��֮ǰ��Ҫ��MR��Ƶ��׽����
            XvMediaRecorderManager.StartCapture();
        }

        private void OnDisable()
        {
            XvMediaRecorderManager.StopCapture(false);
        }

        /// <summary>
        /// ��ʼ¼�Ʊ�����Ƶ
        /// </summary>
        public void StartRecording()
        {
            StopAllCoroutines();
            CancelInvoke();
            StartCoroutine(RelayRecording());
        }

        /// <summary>
        /// ֹͣ������Ƶ¼��
        /// </summary>
        public void StopRecording()
        {
            StopAllCoroutines();
            CancelInvoke();
            xvMediaRecorderManager.StopRecording((filePath) =>{
            tips.gameObject.SetActive(true);

                tips.text = "�洢·����"+filePath;

                Invoke("HideTips", 1);

            });
        }

        /// <summary>
        /// ��ͼ����
        /// </summary>
        public void SaveScreenshot()
        {
            StopAllCoroutines();
            CancelInvoke();
            StartCoroutine(RelayScreenshot());
        }



        private IEnumerator RelayRecording() {
            tips.gameObject.SetActive(true);
            tips.text = "׼��¼��";

            yield return new WaitForSeconds(1);

            for (int i = 3; i >=0; i--)
            {
                tips.text = i.ToString();
                yield return new WaitForSeconds(0.5f);
            }

            
            tips.text = "¼����";
            XvMediaRecorderManager.StartRecording();
        }
        private IEnumerator RelayScreenshot()
        {
            tips.gameObject.SetActive(true);
            tips.text = "׼����";

            yield return new WaitForSeconds(1);

            for (int i = 3; i >= 0; i--)
            {
                tips.text = i.ToString();
                yield return new WaitForSeconds(0.5f);
            }

            tips.text = "��ͼ��";

            XvMediaRecorderManager.SaveScreenshot((filePath) => {
            tips.gameObject.SetActive(true);

                tips.text = "�洢·����" + filePath;
               
                Invoke("HideTips",1);
            });
        }


        private void HideTips() {
            tips.gameObject.SetActive(false);

        }
       
    }
}
