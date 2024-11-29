using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMS.Engine;

public class XvParticlesCloudPoint : MonoBehaviour
{
    public Transform centerCube;
    public ParticleSystem ps;
    ParticleSystem.Particle[] allParticles;

    List<Vector3> FilterVec = new List<Vector3>();

    Matrix4x4 P_tofpoint_world = Matrix4x4.identity;

    Matrix4x4 P_tofpoint_tofcam = Matrix4x4.identity;//原始tof点云
    Matrix4x4 P_tofcam_glassImu = Matrix4x4.identity;//tof外参
    Matrix4x4 P_glassImu_world = Matrix4x4.identity;//眼镜6dof

    public Material[] Mat_alpha;
    public List<GameObject> planeList = new List<GameObject>();
    bool isCreateMesh;
    float totaltime;
    void Start()
    {
        
    }

    void Update()
    {
        /*totaltime += Time.deltaTime;

       if (totaltime>=1)
       {
           isCreateMesh = true;
           totaltime = 0;
       }*/
    }

    public void StartDraw(Vector3[] vs)
    {
        FilterVec.Clear();

        /*for (int i = 0; i < Mathf.FloorToInt(vs.Length); i++)
        {
            //int random = UnityEngine.Random.Range(i*10, i*10 + 99);

            #region tof点云坐标转换
            ////tof点云坐标转换（tof点云相对于tof相机 --> tof点云相对于世界中心）
            //P_tofpoint_tofcam.SetTRS(new Vector3(vs[i].x, -vs[i].y, vs[i].z), new Quaternion(0, 0, 0, 1), Vector3.one);
            //P_tofcam_glassImu.SetTRS(TofCloudManager.tofpos, TofCloudManager.tofQua, Vector3.one);
            //P_glassImu_world.SetTRS(XvXRManager.SDK.HeadPose.Position, XvXRManager.SDK.HeadPose.Orientation, Vector3.one);

            //P_tofpoint_world = P_glassImu_world * P_tofcam_glassImu * P_tofpoint_tofcam;

            //FilterVec.Add(P_tofpoint_world.GetColumn(3));
            #endregion

            //直接使用接口传过来的点云数据
            //FilterVec.Add(vs[i]);
        }*/

        //过滤
        for (int i = 0; i < vs.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, 30);
            if (randomIndex > 28)
            {
                FilterVec.Add(vs[i]);
            }
        }


        //for (int i = 0; i < vs.Length; i++)
        //{
        //    if (i % 100 == 0)
        //    {

        //        //tof点云坐标转换（tof点云相对于tof相机 --> tof点云相对于世界中心）
        //        P_tofpoint_tofcam.SetTRS(new Vector3(vs[i].x, -vs[i].y, vs[i].z), new Quaternion(0, 0, 0, 1), Vector3.one);
        //        P_tofcam_glassImu.SetTRS(TofCloudManager.tofpos, TofCloudManager.tofQua, Vector3.one);
        //        P_glassImu_world.SetTRS(XvXRManager.SDK.HeadPose.Position, XvXRManager.SDK.HeadPose.Orientation, Vector3.one);

        //        P_tofpoint_world = P_glassImu_world * P_tofcam_glassImu * P_tofpoint_tofcam;

        //        FilterVec.Add(P_tofpoint_world.GetColumn(3));
        //    }
        //}
        ////Debug.Log($"XVTof FilterVec.Length:{FilterVec.Count}");

        //设置centerCube位置
        //centerCube.position = FilterVec[Mathf.FloorToInt(FilterVec.Count / 2)];
        ////Debug.Log($"XVTof centerCube pos:{centerCube.position}");

        var main = ps.main;

        var pointCount = FilterVec.Count;
        allParticles = new ParticleSystem.Particle[pointCount];
        main.maxParticles = pointCount;
        //ps.maxParticles = pointCount;
        ////Debug.Log("绘制点count " + main.maxParticles);
        ps.Emit(pointCount);
        ps.GetParticles(allParticles);
        for (int i = 0; i < pointCount; i++)
        {
            allParticles[i].position = (Vector3)FilterVec[i];    // 设置每个点的位置
            allParticles[i].startColor = Color.blue;    // 设置每个点的rgb
            allParticles[i].startSize = 0.025f;
        }


        ps.SetParticles(allParticles, pointCount);      // 将点云载入粒子系统
        /*
        //绘制mesh
        if (isCreateMesh)
        {
            CreateMesh(FilterVec);
            isCreateMesh = false;
        }
        
        */

        //Debug.Log($"XVTof finish");
    }

    void CreateMesh(List<Vector3> FilterVec)
    {
        for (int i = 0; i < planeList.Count; i++)
        {
            Destroy(planeList[i]);
        }
        planeList = new List<GameObject>();

        Vector3[] Verts = new Vector3[FilterVec.Count];
        for (int i = 0; i < FilterVec.Count; i++)
        {
            Verts[i] = FilterVec[i];
        }
        DoCreatPloygonMesh(Verts);
    }

    /// <summary>
    /// 生成自定义多边形方法
    /// </summary>
    /// <param name="s_Vertives">自定义的顶点数组</param>
    public void DoCreatPloygonMesh(Vector3[] s_Vertives)
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
        tPolygon.GetComponent<Renderer>().materials = Mat_alpha;
        tPolygon.AddComponent<MeshCollider>();

        planeList.Add(tPolygon);
    }
}
