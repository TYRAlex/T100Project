using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BezierPathController : MonoBehaviour
{
    public List<GameObject> ControlPointList = new List<GameObject>();
    public List<Vector3> Points =new List<Vector3>();
    public float Radius;
    public int segmentsPerCurve;

    public GameObject Go;
    private void OnDrawGizmos()
    {
        ControlPointList.Clear();

        foreach (Transform item in transform)       
            ControlPointList.Add(item.gameObject);

        List<Vector3> pointPosList = ControlPointList.Select(point => point.transform.position).ToList();

        var points=  GetDrawingPoints(pointPosList, segmentsPerCurve);
        Points = points;
        Gizmos.color = Color.blue;
        foreach (var item in points)
        {
            Gizmos.DrawSphere(item, Radius);
        }
    }


    private void Start()
    {
        var parent = Go.transform.parent;
        for (int i = 0; i < Points.Count; i++)
        {
            GameObject point = Instantiate(Go, parent);
            point.name = i.ToString();
            point.transform.position = Points[i];
        }
    }

    public List<Vector3> GetDrawingPoints(List<Vector3> cotrolPoints,int segmentsPerCurve)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < cotrolPoints.Count-3; i+=3)
        {
            var p0 = cotrolPoints[i];
            var p1 = cotrolPoints[i + 1];
            var p2 = cotrolPoints[i + 2];
            var p3 = cotrolPoints[i + 3];

            for (int j = 0; j < segmentsPerCurve; j++)
            {
                var t = j / (float)segmentsPerCurve;
                var point=  CalculateBezierPoint(t, p0, p1, p2, p3);
                points.Add(point);
            }

           
        }

        return points;
    }


    private Vector3 CalculateBezierPoint(float t ,Vector3 p0,Vector3 p1,Vector3 p2,Vector3 p3)
    {


        var x = (1 - t);
        var x2 = Mathf.Pow(x, 2);
        var x3 = Mathf.Pow(x, 3);

        var t2 = Mathf.Pow(t, 2);
        var t3 = Mathf.Pow(t, 3);


        return (p0 * x3) + (3 * p1 * t * x2) + (3 * p2 * t2 * x) + (p3 * t3);
    }
}
