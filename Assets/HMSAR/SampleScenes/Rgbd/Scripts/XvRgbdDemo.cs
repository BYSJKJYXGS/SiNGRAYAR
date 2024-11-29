
using UnityEngine;
using XvXRFoundation;


public class XvRgbdDemo : MonoBehaviour
{
    [SerializeField]
    private XvRgbdManager rgbdManager;
    private void Awake()
    {
        if (rgbdManager==null) {
            rgbdManager = FindObjectOfType<XvRgbdManager>();
        }

        if (rgbdManager == null) {
            rgbdManager=gameObject.AddComponent<XvRgbdManager>();
        }
    }
    private void OnEnable()
    {
        rgbdManager.StartRgbPose();
    }

    private void OnDisable()
    {
        rgbdManager.StopRgbPose();

    }

    

    private void Update()
    {
        if (Time.frameCount%5==0) {
            Vector2 rgbPixelPoint=new Vector2(960,540);
            Vector3 pointerPose = new Vector3();

            if (rgbdManager.GetRgbPixel3DPose(rgbPixelPoint, ref pointerPose))
            {
                transform.position = pointerPose;
            }
        }
    }

}
