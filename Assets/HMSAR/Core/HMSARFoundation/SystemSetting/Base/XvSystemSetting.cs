using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace XvXRFoundation
{
    /// <summary>
    //key = 2 ,state = 0 眼镜摘掉状态
    //key = 2 ,state = 1 眼镜戴上状态
    //key = 6 ,state = 0 光感
    //key = 14 1 13 3 ,state = 254 压下 255 抬起 
    //key = 17 18 ,state = 101 旋转+ 99 旋转-
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XvEvent
    {
        public double hostTimestamp;
        public long edgeTimestampUs;
        public int type;
        public int state;
    };
    public class XvSystemSetting 
    {
        /// <summary>
        /// 设置眼镜亮度
        /// </summary>
        /// <param name="level">0~9个等级</param>
        [DllImport("xslam-unity-wrapper")]
        public static extern void xslam_display_set_brightnesslevel(int level); //level为亮度等级

 
    }
}
