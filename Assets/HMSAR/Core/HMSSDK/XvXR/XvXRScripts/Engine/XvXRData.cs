using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets.XvXRScripts.Engine
{

    [StructLayout(LayoutKind.Sequential)]
    public struct XvXROpticalParameter_t
    {
       public float fov_left; //度
       public float fov_right;
       public float fov_top;
       public float fov_bottom;
     
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
       public float[] red_coff;     //畸变系数
      
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
       public float[] green_coff;
  
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
       public float[] blue_coff;
      
       public float red_effective_focal_length;  //有效焦距 
       public float green_effective_focal_length;
       public float blue_effective_focal_length;
       public float screen_width_physics;        //屏幕物理宽
       public float screen_height_physics;       //屏幕物理高
       public float screen_width_pixels;         //像素单位
       public float screen_height_pixels;        //像素单位
        
       public float left_eye_center_x;           //以屏左上角为原点，左眼中心x
       public float left_eye_center_y;           //以屏左上角为原点，左眼中心y
       public float right_eye_center_x;          //以屏左上角为原点，右眼中心x
       public float right_eye_center_y;          //以屏左上角为原点，右眼中心y
       public float separation; // Center to center. 
       public float screenDistance; // Distance from lens center to the phone screen.
       public float bottomOffset; // Offset of lens center from top or bottom
        
       public int renderType; //0,cardboard render
    };

    [StructLayout(LayoutKind.Sequential)]
    struct XvXRHmdState_t
    {
        Int64 timestamp; //ms

        /* State of driver pose, in meters*/
        /* Position of the driver tracking reference in driver world space
        * +[0] (x) is right
        * +[1] (y) is up
        * -[2] (z) is forward
        */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        float[] position; //x,y,z
        /* Orientation of the tracker, represented as a quaternion */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        float[] rotation; //w,x,y,z

        /* Acceleration meter/second^2*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        float[] accelerometer; //ax,ay,az

        /* Angular velocity of the pose in axis-angle
        * representation. The direction is the angle of
        * rotation and the magnitude is the angle around
        * that axis in radians/second. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        float []gyroscope; //gx,gy,gz
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        float []geomagnetism; //uT

        int tracking_result;
    };
}
