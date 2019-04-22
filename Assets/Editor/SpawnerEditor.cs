using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    Spawner spawner;

    public void OnEnable() {
        spawner = target as Spawner;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Spawn")) {
            spawner.Spawn(spawner.prefabToSpawn, spawner.shapeCreator.shapes[0]);
        }
    }
}
