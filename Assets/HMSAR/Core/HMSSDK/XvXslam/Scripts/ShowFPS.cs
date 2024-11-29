using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HMS.Core
{
    public class ShowFPS : MonoBehaviour
    {
        //更新的时间间隔
        public float UpdateInterval = 0.5F;
        //最后的时间间隔
        private float _lastInterval;
        //帧[中间变量 辅助]
        private int _frames = 0;
        //当前的帧率
        private float _fps;

        private TextMesh _text;

        int count = 0;

        //private int _temp = 0;

        void Start()
        {
            //Application.targetFrameRate=60;

            //UpdateInterval = Time.realtimeSinceStartup;

            _frames = 0;
            _text = GameObject.Find("FPS").GetComponent<TextMesh>();

            InvokeRepeating("GetAllObjects", 1, 1);
        }

        void OnGUI()
        {
            if (_text != null)
            {
                //_text.text = "FPS:" + _fps.ToString("f2") + "\nverts:" + verts + "\ntris:" + tris;
                _text.text = "FPS:" + _fps.ToString("f2");
            }
            //_text.text = "temp:" + _temp;
        }


        public static int verts;
        public static int tris;

        void GetAllObjects()
        {
            verts = 0;
            tris = 0;
            GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject obj in ob)
            {
                GetAllVertsAndTris(obj);
            }
        }

        //得到三角面和顶点数
        void GetAllVertsAndTris(GameObject obj)
        {
            Component[] filters;
            filters = obj.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter f in filters)
            {
                tris += f.sharedMesh.triangles.Length / 3;
                verts += f.sharedMesh.vertexCount;
            }
        }


        void Update()
        {
            ++_frames;

            if (Time.realtimeSinceStartup > _lastInterval + UpdateInterval)
            {
                _fps = _frames / (Time.realtimeSinceStartup - _lastInterval);

                _frames = 0;
                count++;
                _lastInterval = Time.realtimeSinceStartup;
                if (count % 10 == 0)
                {
                    MyDebugTool.Log("fps is:" + _fps);
                }

            }

            //if (_temp == 0) {
            //	Debug.Log( "gettemp" );
            //    byte[] cmd = {0x02, 0xde, 0x78};
            //    byte[] rdata = API.HidWriteAndRead(cmd, cmd.Length);
            //    if (rdata != null) {
            //        _temp = rdata[3];
            //        Debug.Log( "gettemp" + _temp );
            //    }
            //}
        }
    }
}