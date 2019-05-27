using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public ShapeCreator shapeCreator;
    public Creature creature;

    public List<Vector2> polygonPoints;
    public List<int> polygonTriangles;
    float[] areas;
    float totalArea = 0f;

    [Range(0, 200)]
    public short creatureAmountToSpawn = 10;

    /// <summary>
    /// Will spawn creatures on pressing Space.
    /// </summary>
    public void Update() {
        if(Input.GetButtonDown("Jump")) {
            SpawnManager.Instance.DeleteAllChilds(transform);
            SpawnCreature(creature, creatureAmountToSpawn);
        }
    }

    public void Start() {
        (polygonPoints, polygonTriangles) = GetTriangles(shapeCreator.shapes);
        (totalArea, areas) = GetAreas(polygonTriangles, polygonPoints);
    }

    /// <summary>
    /// Will delete a previously spawned creatures before calling SpawnCreature(amount).
    /// </summary>
    /// <param name="amount"></param>
    public void RespawnCreature(int amount) {
        SpawnManager.Instance.DeleteAllChilds(transform);
        SpawnCreature(creature, amount);
    }

    /// <summary>
    /// Get all Triangles formed by the shapes.
    /// </summary>
    /// <param name="shapes"></param>
    /// <returns>A list of points forming triangles and how the the points are connected(three entries in the second list form one triangle together).</returns>
    public (List<Vector2>, List<int>) GetTriangles(List<Shape> shapes) {
        Vector2[][] holes = new Vector2[0][];
        for(int i = 0; i < shapes.Count; i++) {
            int oldPolygonPointsCount = polygonPoints.Count;
            Vector2[] currentPolygonPoints = shapes[i].points.ConvertAll<Vector2>(x => new Vector2(x.x, x.z)).ToArray();
            Triangulator triangulator = new Triangulator(new Polygon(currentPolygonPoints, holes));
            List<int> currentTriangles = triangulator.Triangulate().ToList();
            for(int j = 0; j < currentTriangles.Count; j++) {
                currentTriangles[j] += oldPolygonPointsCount;
            }
            polygonPoints.AddRange(currentPolygonPoints);
            polygonTriangles.AddRange(currentTriangles);
        }

        return (polygonPoints, polygonTriangles);
    }
    /// <summary>
    /// Calculates the areas 
    /// </summary>
    /// <param name="polygonTriangles"></param>
    /// <param name="polygonPoints"></param>
    /// <returns>Returns the total area and an array holding the area for each triangle.</returns>
    public (float, float[] areas) GetAreas(List<int> polygonTriangles, List<Vector2> polygonPoints) {
        float totalArea = 0;
        float[] areas = new float[polygonTriangles.Count / 3];

        for(int i = 0, k = 0; i < polygonTriangles.Count; i += 3, k++) {
            float a = Vector2.Distance(polygonPoints[polygonTriangles[i]], polygonPoints[polygonTriangles[i + 1]]);
            float b = Vector2.Distance(polygonPoints[polygonTriangles[i]], polygonPoints[polygonTriangles[i + 2]]);
            float c = Vector2.Distance(polygonPoints[polygonTriangles[i + 1]], polygonPoints[polygonTriangles[i + 2]]);
            float s = (a + b + c) / 2f;

            areas[k] = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
            totalArea += areas[k];
        }

        return (totalArea, areas);
    }

    public void SpawnCreature(Creature creature, int amount = 1)
    {
        for(int i = 0; i < amount; i++) {
            float pickedArea = (Random.value * totalArea);
            float currentArea = 0f;

            for(int j = 0; j < areas.Length; j++) {
                currentArea += areas[j];
                if(currentArea >= pickedArea) {
                    Vector2 ab = polygonPoints[polygonTriangles[(j * 3) + 1]] - polygonPoints[polygonTriangles[j * 3]];
                    Vector2 ac = polygonPoints[polygonTriangles[(j * 3) + 2]] - polygonPoints[polygonTriangles[j * 3]];

                    float pickedMultiplierSumTotal = Random.value;
                    float pickedLengthAB = (pickedMultiplierSumTotal * Random.value);
                    float pickedLengthAC = (pickedMultiplierSumTotal - pickedLengthAB);

                    Vector2 pickedPoint = (polygonPoints[polygonTriangles[j * 3]] + (ab * pickedLengthAB + ac * pickedLengthAC));
                    GameObject.Instantiate(creature.prefab, new Vector3(pickedPoint.x, GetHeight(pickedPoint.x, pickedPoint.y), pickedPoint.y), Quaternion.identity, transform);
                    break;
                }
            }
        }
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
}
