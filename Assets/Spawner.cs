﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public ShapeCreator shapeCreator;
    public GameObject prefabToSpawn;

    public void Spawn(GameObject gameObject, Shape shape)
    {
        float minX = shape.points[0].x, maxX = shape.points[0].x;
        float minY = shape.points[0].z, maxY = shape.points[0].z;

        for(int i = 0; i < shape.points.Count; i++)
        {
            Vector3 point = shape.points[i];

            if(point.x < minX)
            {
                minX = point.x;
            }
            else if(point.x > maxX)
            {
                maxX = point.x;
            }
            if(point.z < minY)
            {
                minY = point.z;
            }
            else if(point.z > maxY)
            {
                maxY = point.z;
            }

        }

        for(int x = Mathf.CeilToInt(minX); x < maxX; x++)
        {
            for(int y = Mathf.CeilToInt(minY); y < maxY; y++)
            {
                Vector3 currentPosition = new Vector3(x, 0.5f, y);
                if(IsInPolygon(shape.points, currentPosition))
                {
                    GameObject.Instantiate(prefabToSpawn, currentPosition, Quaternion.identity, transform);
                }
            }
        }
    }

    public static bool IsInPolygon(List<Vector3> polygon, Vector3 testPoint)
    {
        bool result = false;
        int j = polygon.Count - 1;
        for (int i = 0; i < polygon.Count; i++)
        {
            if (polygon[i].z < testPoint.z && polygon[j].z >= testPoint.z || polygon[j].z < testPoint.z && polygon[i].z >= testPoint.z)
            {
                if (polygon[i].x + (testPoint.z - polygon[i].z) / (polygon[j].z - polygon[i].z) * (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }

        return result;
    }
}
