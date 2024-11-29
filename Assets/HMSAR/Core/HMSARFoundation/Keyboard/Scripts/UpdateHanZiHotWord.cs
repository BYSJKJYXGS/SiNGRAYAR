using com.devil.Keyboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class UpdateHanZiHotWord : MonoBehaviour
{
    //public TextAsset textAsset;
    public InputField inputField;
    public Button update;


    private void Awake()
    {
        update.onClick.AddListener(()=> {
           string str= inputField.text;

            if (!string.IsNullOrEmpty(str)) {
                foreach (var item in str)
                {
                    PinYin.Instance.UpdateHotWord(item);
                }
                PinYin.Instance.SaveHotWord();
            }

           


        });
    }


   
}
