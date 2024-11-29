
namespace XvXRFoundation
{
    #region ”„—€œ‡ª˙
    public sealed class XvStereoCameraManager
    {
        private static XvStereoCameraManager xvLeftStereoCameraManager;

        private static XvStereoCameraManager xvRightStereoCameraManager;

        public static XvStereoCameraManager GetXvStereoCameraManager(bool left)
        {
            if (left)
            {
                if (xvLeftStereoCameraManager == null)
                {
                    xvLeftStereoCameraManager = new XvStereoCameraManager();
                }
                return xvLeftStereoCameraManager;
            }
            else {
                if (xvRightStereoCameraManager == null)
                {
                    xvRightStereoCameraManager = new XvStereoCameraManager();
                }
                return xvRightStereoCameraManager;
            }
           
        }
        private XvStereoCameraManager() { }


        private XvCameraBase frameBase;
        private bool isOn;
        public bool IsOn { get { return isOn; } }

        private bool isLeft;
        public void StartCapture(int requestedWidth, int requestedHeight, int requestedFPS, bool isLeft)
        {
#if UNITY_EDITOR
            return;
#endif
            if (IsOn)
            {
                return;
            }
            this.isLeft = isLeft;

            StopCapture();
            frameBase = new XvStereoCamera(requestedWidth, requestedHeight, requestedFPS, FrameArrived, isLeft);
            isOn = true;
            frameBase.StartCapture();

        }

        public void StopCapture()
        {
#if UNITY_EDITOR
            return;
#endif
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
            if (isLeft)
            {

                XvCameraManager.onLeftStereoStreamFrameArrived?.Invoke(cameraData);
            }
            else
            {
                XvCameraManager.onRightStereoStreamFrameArrived?.Invoke(cameraData);

            }


        }

        public void Update()
        {
            if (!IsOn)
            {
                return;
            }
#if UNITY_EDITOR
            return;
#endif
            frameBase?.Update();

        }

    }

    #endregion

}