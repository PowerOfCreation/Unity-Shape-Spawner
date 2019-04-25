using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;

public class ShapeCreator : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Creature creature;

    [HideInInspector]
    public List<Shape> shapes = new List<Shape>();

    [HideInInspector]
    public bool showShapesList;

    public float handleRadius = .5f;

    public void Awake() {
        for(int i = 0; i < shapes.Count; i++) {
            SpawnManager.Instance.RegisterSpawnZone(creature, shapes[i]);
        }
    }
}