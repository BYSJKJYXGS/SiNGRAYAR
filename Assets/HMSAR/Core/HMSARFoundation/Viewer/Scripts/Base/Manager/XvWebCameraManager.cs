
namespace XvXRFoundation
{
    #region WebCamera  3588∫–◊”∫Û÷√…„œÒÕ∑

    public sealed class XvWebCameraManager
    {


        private static XvWebCameraManager xvWebCameraManager;

        public static XvWebCameraManager GetXvWebCameraManager()
        {
            if (xvWebCameraManager == null)
            {
                xvWebCameraManager = new XvWebCameraManager();
            }
            return xvWebCameraManager;
        }

        private XvWebCameraManager() { }

        private XvCameraBase frameBase;
        private bool isOn;
        public bool IsOn { get { return isOn; } }
        public void StartCapture(int requestedWidth, int requestedHeight, int requestedFPS)
        {
            if (IsOn)
            {
                return;
            }

            StopCapture();
            frameBase = new XvWebCamera(requestedWidth, requestedHeight, requestedFPS, FrameArrived);
            isOn = true;

            frameBase.StartCapture();
        }

        public void StopCapture()
        {
            if (frameBase != null && frameBase.IsOpen)
            {
                frameBase.StopCapture();
            }

            frameBase = null;
            // GC.Collect();

            isOn = false;
        }

       



        private void FrameArrived(cameraData cameraData)
        {
            XvCameraManager.onWebCameraStreamFrameArrived?.Invoke(cameraData);
        }

        public void Update()
        {
            if (!IsOn)
            {
                return;
            }
            frameBase?.Update();
        }
    }
    #endregion

}