using HMS.Core;
using UnityEngine;
using HMS.Engine;
using static XvXRFoundation.XvSystemSetting;

namespace XvXRFoundation
{
    /// <summary>
    /// 该类提供系统相关的参数设置、获取方法
    /// </summary>
    public sealed class XvSystemSettingManager : MonoBehaviour
    {
        private XvSystemSettingManager() { }

        private int level = 6;

        /// <summary>
        /// 获取当前亮度
        /// </summary>
        /// <returns></returns>
        public int GetBrightnessLevel() {
            return level;
        }
        /// <summary>
        /// 设置眼镜亮度
        /// </summary>
        /// <param name="level">1~9</param>
        public void SetBrightnessLevel(int level)
        {
            this.level = level;

            XvSystemSetting.xslam_display_set_brightnesslevel(level);
#if !UNITY_EDITOR
         
            XvSystemSetting.xslam_display_set_brightnesslevel(level);
#endif
        }

        /// <summary>
        /// 设置当前瞳距
        /// </summary>
        /// <param name="ipd">55mm~75mm</param>
        public void SetIPD(float ipd)
        {
#if !UNITY_EDITOR
           float nowIpd = GetIPD();
            XvXRAndroidDevice.updateCalibra((2 * ipd - nowIpd) / 10);
            XvXRManager.SDK.GetDevice().setFedDis((2 * ipd - nowIpd) / 10);
            XvXREye.EDI = 0;
#endif

        }
        /// <summary>
        /// 获取当前IPD
        /// </summary>
        /// <returns> 返回值单位为mm（毫米）</returns>
        public float GetIPD()
        {
#if UNITY_EDITOR
            return 0;
#endif
            API.stereo_pdm_calibration fed = XvXRManager.SDK.GetDevice().GetFed();
            float nowIpd = (float)(fed.calibrations[1].extrinsic.translation[0] - fed.calibrations[0].extrinsic.translation[0]);
            nowIpd *= 1000;
            return nowIpd;
        }

    }
}
