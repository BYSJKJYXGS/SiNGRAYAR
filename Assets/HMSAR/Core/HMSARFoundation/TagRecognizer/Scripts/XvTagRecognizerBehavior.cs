using UnityEngine;
using UnityEngine.Events;
namespace XvXRFoundation
{

    /// <summary>
    /// 该类提供识别成功以后回调方法
    /// </summary>

    public class XvTagRecognizerBehavior : MonoBehaviour
    {
        

        public int id=-1;
        public UnityEvent OnFoundEvent = new UnityEvent();
        public UnityEvent OnLostEvent = new UnityEvent();

        private XvTagRecognizerManager aprilTagManager;


        private TagDetection tagDetection;
        public TagDetection TagDetection { 
        get { return tagDetection; }
        }





        private void Awake()
        {
       
            aprilTagManager = FindAnyObjectByType<XvTagRecognizerManager>();

            if (id==-1) {
                int.TryParse(gameObject.name, out id);
            }
           // 
        }

        private void OnEnable()
        {
            if (aprilTagManager != null)
            {
                aprilTagManager.OnDetectedAprilTagEvent.AddListener(OnDetectedAprilTagEvent);
            }
        }

        private void OnDisable()
        {
            if (aprilTagManager != null)
            {
                aprilTagManager.OnDetectedAprilTagEvent.RemoveListener(OnDetectedAprilTagEvent);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            if (OnFoundEvent == null)
            {
                OnFoundEvent = new UnityEvent();
            }
            if (OnLostEvent == null)
            {
                OnLostEvent = new UnityEvent();
            }
            OnLostEvent?.Invoke();

        }



        private void OnDetectedAprilTagEvent(TagDetection[] tagDetections)
        {
            bool isFound = false;


            if (tagDetections == null)
            {
                isFound = false;
                MyDebugTool.Log("tagDetection == null");
            }
            else
            {
                //识别出来
                for (int i = 0; i < tagDetections.Length; i++)
                {
                    MyDebugTool.Log("detected：" + tagDetections[i].id + "   " + tagDetections[i].confidence + "  " + aprilTagManager.Confidence);

                    if (tagDetections[i].confidence >= aprilTagManager.Confidence)
                    {
                        if (tagDetections[i].id == id)
                        {
                            isFound = true;
                            Vector3 position = tagDetections[i].translation;
                            Quaternion rotation = new Quaternion(tagDetections[i].quaternion[0], tagDetections[i].quaternion[1], tagDetections[i].quaternion[2], tagDetections[i].quaternion[3]);
                            transform.position = position;
                            transform.rotation = rotation;
                            tagDetection = tagDetections[i];
                            break;
                        }
                    }
                }
            }

            if (isFound)
            {
                MyDebugTool.Log("detected");

                transform.localScale = Vector3.one * (float)aprilTagManager.Size;
                OnFoundEvent?.Invoke();
            }
            else
            {
                //Debug.Log("丢失");

                OnLostEvent?.Invoke();

            }
        }

    }
}
