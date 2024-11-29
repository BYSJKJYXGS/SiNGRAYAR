
namespace XvXRFoundation
{

    public class XvCameraBase
{

    protected cameraData cameraData = new cameraData();
    protected bool isOpen;
    protected int width;
    protected int height;
    protected int fps;
    public delegate void FrameArrived(cameraData cameraData);
    public FrameArrived frameArrived;


    public XvCameraBase(int width, int height, int fps, FrameArrived frameArrived) {
        this.width = width;
        this.height = height;
        this.fps = fps;
        this.frameArrived = frameArrived;
    }
    public virtual void StartCapture() { 
        isOpen = true;
        }

    public virtual void StopCapture() {
        isOpen = false;
    }

    public virtual void Update() { 
    
    }

    public bool IsOpen
    {
        get {
            return isOpen;
        }
    
    }
}

}