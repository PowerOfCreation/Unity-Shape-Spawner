using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public ShapeCreator shapeCreator;
    public GameObject prefabToSpawn;
    public Creature creature;

    public List<Vector2> polygonPoints;
    public List<int> polygonTriangles;
    float[] areas;
    float totalArea = 0f;
    [Range(0, 200)]
    public short creatureAmountToSpawn = 10;

    public void Update() {
        if(Input.GetButtonDown("Jump")) {
          //  StartCoroutine(SpawnCreature(creatureAmountToSpawn));
          SpawnCreature(creatureAmountToSpawn);
        }
    }

    public void Start() {
        FillTrianglesField(shapeCreator.shapes);
        FillAreaField();
    }

    public void SpawnCreature(int amount) {
        SpawnManager.Instance.DeleteAllChilds(transform);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        for(int i = 0; i < amount; i++) {
           SpawnAll(creature.prefab);
        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log(elapsedMs);
    }

    public void FillTrianglesField(List<Shape> shapes) {
        Vector2[][] holes = new Vector2[0][];
        for(int i = 0; i < shapes.Count; i++) {
            int oldPolygonPointsCount = polygonPoints.Count;
            Vector2[] currentPolygonPoints = shapes[i].points.ConvertAll<Vector2>(x => new Vector2(x.x, x.z)).ToArray();
            Triangulator triangulator = new Triangulator(new Polygon(currentPolygonPoints, holes));
            List<int> currentTriangles = triangulator.Triangulate().ToList();
            for(int j = 0; j < currentTriangles.Count; j++) {
                currentTriangles[j] += oldPolygonPointsCount;
            }
            polygonTriangles.AddRange(currentTriangles);
            polygonPoints.AddRange(currentPolygonPoints);
        }

        areas = new float[polygonTriangles.Count / 3];
    }

    public void FillAreaField() {
        for(int i = 0, k = 0; i < polygonTriangles.Count; i += 3, k++) {
            float a = Vector2.Distance(polygonPoints[polygonTriangles[i]], polygonPoints[polygonTriangles[i + 1]]);
            float b = Vector2.Distance(polygonPoints[polygonTriangles[i]], polygonPoints[polygonTriangles[i + 2]]);
            float c = Vector2.Distance(polygonPoints[polygonTriangles[i + 1]], polygonPoints[polygonTriangles[i + 2]]);
            float s = (a + b + c) / 2f;

            areas[k] = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
            totalArea += areas[k];
        }
    }

    public void SpawnAll(GameObject gameObject)
    {
        float pickedArea = (Random.value * totalArea);
        float currentArea = 0f;

        for(int i = 0; i < areas.Length; i++) {
            currentArea += areas[i];
            if(currentArea >= pickedArea) {
                Vector2 ab = polygonPoints[polygonTriangles[(i * 3) + 1]] - polygonPoints[polygonTriangles[i * 3]];
                Vector2 ac = polygonPoints[polygonTriangles[(i * 3) + 2]] - polygonPoints[polygonTriangles[i * 3]];

                float pickedMultiplierSumTotal = Random.value;
                float pickedLengthAB = (pickedMultiplierSumTotal * Random.value);
                float pickedLengthAC = (pickedMultiplierSumTotal - pickedLengthAB);

                Vector2 pickedPoint = (polygonPoints[polygonTriangles[i * 3]] + (ab * pickedLengthAB + ac * pickedLengthAC));
                GameObject.Instantiate(gameObject, new Vector3(pickedPoint.x, GetHeight(pickedPoint.x, pickedPoint.y), pickedPoint.y), Quaternion.identity, transform);
                break;
            }
        }
    }

    public static List<Vector3> GetPossibleSpawnPoints(Shape shape) {
        float minX = shape.points[0].x, maxX = shape.points[0].x;
        float minY = shape.points[0].z, maxY = shape.points[0].z;

        for(int i = 1; i < shape.points.Count; i++)
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

        List<Vector3> result = new List<Vector3>((((int) maxX - Mathf.CeilToInt(minX)) / 2) * (((int) maxY - Mathf.CeilToInt(minY)) / 2));

        for(int x = Mathf.CeilToInt(minX); x < maxX; x += 2)
        {
            for(int y = Mathf.CeilToInt(minY); y < maxY; y += 2)
            {
                Vector3 currentPosition = new Vector3(x, GetHeightRough(x, y), y);

                if(IsInPolygon(shape.points, currentPosition))
                {
                    result.Add(currentPosition);
                }
                else {
                    y += 10;
                }
            }
        }

        return result;
    }

    public static float GetHeightRough(int x, int y) {
        return Terrain.activeTerrain.terrainData.GetHeight(x, y);
    }

    public static float GetHeight(float x, float y) {
        if(Physics.Raycast(new Vector3(x, 0f, y), Vector3.down, out RaycastHit hit)) {
            return hit.point.y;
        }
        Physics.queriesHitBackfaces = true;
        if(Physics.Raycast(new Vector3(x, 0f, y), Vector3.up, out hit)) {
            Physics.queriesHitBackfaces = false;
            return hit.point.y;
        }
        Physics.queriesHitBackfaces = false;

        return .5f;
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
