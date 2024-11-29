using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XvXRFoundation;
namespace XvXRFoundation
{
    /// <summary>
    /// 该类主要负责平面数据的绘制
    /// </summary>
    public sealed class XvPlaneMeshVisualizer : MonoBehaviour
    {

        private XvPlaneMeshVisualizer() { }

        private List<GameObject> planeList = new List<GameObject>();

        [SerializeField]
        private XvPlaneManager xvPlaneManager;
        [SerializeField]
        private Material planeMat;

        [SerializeField]
        private bool enableCollider=true;

        [SerializeField]
        private bool enableRender = true;


        private void Awake()
        {
            if (xvPlaneManager == null)
            {
                xvPlaneManager = FindAnyObjectByType<XvPlaneManager>();
            }

            if (xvPlaneManager == null)
            {
                xvPlaneManager = new GameObject("XvPlaneManager").AddComponent<XvPlaneManager>();

            }
        }

        private void OnEnable()
        {
            if (xvPlaneManager != null)
            {

                xvPlaneManager.planesChanged += PlanesChanged;
                xvPlaneManager.StartPlaneDetction();

            }
        }

        private void OnDisable()
        {
            if (xvPlaneManager != null)
            {
                xvPlaneManager.planesChanged -= PlanesChanged;
                xvPlaneManager.StopPlaneDetction();
            }
        }

        private void PlanesChanged(plane[] planes)
        {
            for (int i = 0; i < planeList.Count; i++)
            {
                Destroy(planeList[i]);
            }
            planeList = new List<GameObject>();

            //Debug.LogError("planes === " + planes.Length);

            for (int i = 0; i < planes.Length; i++)
            {
                Vector3[] v = new Vector3[planes[i].points.Count];

                for (int f = 0; f < planes[i].points.Count; f++)
                {
                    v[f] = new Vector3((float)planes[i].points[f].X, -(float)planes[i].points[f].Y, (float)planes[i].points[f].Z);
                }

                DoCreatPloygonMesh(v);
            }
        }

        /// <summary>
        /// 生成自定义多边形方法
        /// </summary>
        /// <param name="s_Vertives">自定义的顶点数组</param>
        private void DoCreatPloygonMesh(Vector3[] s_Vertives)
        {
            //新建一个空物体进行进行绘制自定义多边形
            GameObject tPolygon = new GameObject("tPolygon");

            //绘制所必须的两个组件
            tPolygon.AddComponent<MeshFilter>();
            tPolygon.AddComponent<MeshRenderer>();

            //新申请一个Mesh网格
            Mesh tMesh = new Mesh();

            //存储所有的顶点
            Vector3[] tVertices = s_Vertives;

            //存储画所有三角形的点排序
            List<int> tTriangles = new List<int>();

            //根据所有顶点填充点排序
            for (int i = 0; i < tVertices.Length - 1; i++)
            {
                tTriangles.Add(i);
                tTriangles.Add(i + 1);
                tTriangles.Add(tVertices.Length - i - 1);
            }

            //赋值多边形顶点
            tMesh.vertices = tVertices;

            //赋值三角形点排序
            tMesh.triangles = tTriangles.ToArray();

            //重新设置UV，法线
            tMesh.RecalculateBounds();
            tMesh.RecalculateNormals();

            //将绘制好的Mesh赋值
            tPolygon.GetComponent<MeshFilter>().mesh = tMesh;
            tPolygon.GetComponent<Renderer>().material = planeMat;
            Collider collider= tPolygon.AddComponent<MeshCollider>();
            collider.enabled = enableCollider;

            tPolygon.GetComponent<MeshRenderer>().enabled = enableRender;
            planeList.Add(tPolygon);
        }


        public void SetCollider(bool enable)
        {
            enableCollider = enable;
            foreach (var item in planeList)
            {
                item.GetComponent<Collider>().enabled = enableCollider;
            }
        }

        public void SetVisualizer(bool enable)
        {
            this.enableRender = enable;
            foreach (var item in planeList)
            {
                item.GetComponent<MeshRenderer>().enabled = enableRender;
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < planeList.Count; i++)
            {
                Destroy(planeList[i]);
            }
        }
    }
}