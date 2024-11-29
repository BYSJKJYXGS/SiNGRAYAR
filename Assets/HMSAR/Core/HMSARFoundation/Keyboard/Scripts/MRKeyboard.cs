using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.InputField;

namespace com.devil.Keyboard
{

    public enum KeyboardIputType
    {
        None,
        Letter,
        HanZi,
        Number,
    }

    public sealed class MRKeyboard : MonoBehaviour
    {
       
        private static MRKeyboard keyboard;


        public static MRKeyboard Instance
        {
            get
            {

                if (keyboard == null)
                {
                    keyboard = FindObjectOfType<MRKeyboard>();
                }

                if (keyboard == null)
                {
                    keyboard = Instantiate(Resources.Load<GameObject>("Keyboard")).AddComponent<MRKeyboard>();
                }
                return keyboard;

            }
        }
       

        private MRKeyboard() { }
        internal UnityAction OKEvent;//OK键监听

        internal KeyboardIputType keyboardIputType= KeyboardIputType.None;
        //private GameObject LetterPanel;
        private GameObject NumberPanel;
        private GameObject HanZiPanel;
        private InputContentPanel InputContentPanel;
        private bool isInitialized;

        private Text currentLanguage;
        private Text otherLanguage;
        internal ContentType contentType;
        public CustomInputField customInputField;
        private Image follow;


        private void Awake()
        {
            keyboard = this;
            Initialized();

        }
        private void Start()
        {
            NumberPanel.gameObject.SetActive(false);
            InputContentPanel.gameObject.SetActive(false);
            HanZiPanel.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        private void Initialized()
        {
           
            isInitialized = true;
            follow = transform.Find("Follow/Icon").GetComponent<Image>();
             //LetterPanel = transform.Find("LetterPanel").gameObject;
             NumberPanel = transform.Find("NumberPanel").gameObject;
            HanZiPanel = transform.Find("HanZiPanel").gameObject;
            InputContentPanel = transform.Find("InputContentPanel").GetComponent<InputContentPanel>();

            InputContentPanel.Initialized();

            foreach (Button item in GetComponentsInChildren<Button>(true))
            {

                if (item.name == "CN" || item.name == "EN")
                {
                    currentLanguage = item.transform.Find("CurrentText").GetComponent<Text>();
                    otherLanguage = item.transform.Find("OtherText").GetComponent<Text>();
                }
                Button bt = item;
                bt.onClick.AddListener(delegate
                {
                    OnButtonClick(bt);
                });
                
            }
        
        }

       


        public void Show()
        {
            gameObject.SetActive(true);

            if (!isInitialized)
            {
                Initialized();
            }
            SwitchInputType(KeyboardIputType.Letter);
        }


        public void Hide()
        {
            InputContentPanel.OKEvent();
            gameObject.SetActive(false);
          
            SwitchInputType(KeyboardIputType.None);

        }
        public void SetInputField(ICustomInputField inputField, Pose pose, ContentType contentType)
        {
            //transform.parent.parent.position = pose.position;
            //transform.parent.parent.rotation = pose.rotation;


           Vector3 dir= Vector3.ProjectOnPlane(Camera.main.transform.forward,Vector3.up).normalized;
            transform.parent.parent.position = Camera.main.transform.position + dir * 0.6f - Vector3.up * 0.15f; ;
            transform.parent.parent.rotation = Quaternion.LookRotation(dir,Vector3.up)*Quaternion.Euler(Vector3.right*30);
            if (!isInitialized)
            {
                Initialized();
            }
            InputContentPanel.SetInputField(inputField);

            this.contentType = contentType;

        }

        public void Clear()
        {
            InputContentPanel.Clear();

        }
        private void SwitchInputType(KeyboardIputType inputType)
        {
            if (keyboardIputType != inputType)
            {
                InputContentPanel.InputKeyboardChange(keyboardIputType);
                keyboardIputType = inputType;
                //LetterPanel.gameObject.SetActive(false);
                NumberPanel.gameObject.SetActive(false);
                HanZiPanel.gameObject.SetActive(false);
                InputContentPanel.gameObject.SetActive(true);


                switch (keyboardIputType)
                {
                    case KeyboardIputType.None:
                        InputContentPanel.gameObject.SetActive(false);
                        break;
                    case KeyboardIputType.Letter:
                        // LetterPanel.gameObject.SetActive(true);
                        HanZiPanel.gameObject.SetActive(true);
                        currentLanguage.text = "英文";
                        otherLanguage.text = "中文";
                        break;
                    case KeyboardIputType.HanZi:
                        currentLanguage.text = "中文";
                        otherLanguage.text = "英文";
                        HanZiPanel.gameObject.SetActive(true);
                        break;
                    case KeyboardIputType.Number:
                        NumberPanel.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }


            }

        }



        private void OnButtonClick(Button bt)
        {
            MyDebugTool.Log("OnButtonClick：" + bt.name);
           

            switch (bt.name)
            {
                case "BackSpace":
                    InputContentPanel.BackSpaceEvent();
                    break;
                case "Shift":
                    LetterSwitch();
                    break;
                case "SPACE":
                    InputContentPanel.InputEvent(keyboardIputType, " ");
                    break;
                case "OK":
              
                    Hide();
                    break;
                case "123":
                    SwitchInputType(KeyboardIputType.Number);

                    break;
                case "ABC":
                    bt.name = "CN";
                    currentLanguage.text = "英文";
                    otherLanguage.text = "中文";
                    SwitchInputType(KeyboardIputType.Letter);

                    break;
                case "HanZi":
                    bt.name = "CN";
                    currentLanguage.text = "英文";
                    otherLanguage.text = "中文";
                    SwitchInputType(KeyboardIputType.Letter);

                    break;


                case "CN":
                    bt.name = "EN";
                    currentLanguage.text = "英文";
                    otherLanguage.text = "中文";

                    SwitchInputType(KeyboardIputType.Letter);
                    break;
                case "EN":
                    bt.name = "CN";
                    currentLanguage.text = "中文";
                    otherLanguage.text = "英文";
                    SwitchInputType(KeyboardIputType.HanZi);


                    break;
                default:
                    Text text = bt.GetComponentInChildren<Text>();
                    if (text != null)
                    {


                        //if (
                        //    (inputType == ContentType.IntegerNumber && IsNumber(text.text[0]))
                        // || inputType == == ContentType.Standard && IsLetter(text.text[0])
                        // || inputType == 3

                        // ||(inputType==4&&(IsNumber(text.text[0]) || IsLetter(text.text[0]))))
                        //{


                        //}

                        switch (contentType)
                        {
                            
                               
                           
                            case ContentType.IntegerNumber:
                                if (IsNumber(text.text[0])) { 
                                    InputContentPanel.InputEvent(keyboardIputType, text.text);
                                }
                                break;
                            case ContentType.Standard:
                                
                            case ContentType.Autocorrected:
                             
                            case ContentType.DecimalNumber:
                               
                            case ContentType.Alphanumeric:
                               
                            case ContentType.Name:
                               
                            case ContentType.EmailAddress:
                               
                            case ContentType.Password:
                               
                            case ContentType.Pin:
                               
                            case ContentType.Custom:
                              
                            default:
                                InputContentPanel.InputEvent(keyboardIputType, text.text);
                                break;
                        }

                    }
                    else
                    {
                        MyDebugTool.Log(bt.name + "No child nodes found");
                    }
                    break;
            }
        }



        /// <summary>
        /// 大小写切换
        /// </summary>
        private void LetterSwitch()
        {
            Text[] texts = GetComponentsInChildren<Text>();
            foreach (var item in texts)
            {
                if (item.text.Length != 1)
                {
                    continue;
                }
                char newLetter = item.text.Replace(" ", "")[0];
                int ascll = (int)(item.text[0]);
                if (ascll >= 65 && ascll <= 90)
                {
                    //小写，转换成大写
                    newLetter = (char)(97 + (ascll - 65));
                }
                else
                {
                    if (ascll >= 97 && ascll <= 122)
                    {
                        //小写，转换成大写
                        newLetter = (char)(65 + (ascll - 97));
                    }

                }
                item.text = newLetter.ToString();
            }
        }

        private bool IsNumber(char tex)
        {



            int ascll = (int)tex;

            return ascll >= 48 && ascll <= 57;

        }

        private bool IsLetter(char tex)
        {
            int ascll = (int)tex;
            if (ascll >= 65 && ascll <= 90)
            {
                return true;
            }
            else
            {
                if (ascll >= 97 && ascll <= 122)
                {
                    return true;

                }

            }

            return false;
        }

    }

}