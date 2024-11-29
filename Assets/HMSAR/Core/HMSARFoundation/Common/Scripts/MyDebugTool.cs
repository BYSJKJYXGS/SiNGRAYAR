using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public sealed class MyDebugTool 
{
    private static string TAG = "wuxh:";
    private static StringBuilder stringBuilder = new StringBuilder("wuxh");
    private static bool logEnable=true;
    public static void Log(object msg)
    {
        if (!logEnable) {
            return;
        }
        stringBuilder.Clear();
        stringBuilder.Append(TAG);
        stringBuilder.Append(msg);
        Debug.Log(stringBuilder.ToString());
    }

    public static void LogError(object msg)
    {
        if (!logEnable)
        {
            return;
        }

        stringBuilder.Clear();

        stringBuilder.Append(TAG);
        stringBuilder.Append(msg);
        Debug.LogError(stringBuilder.ToString());

    }
}
