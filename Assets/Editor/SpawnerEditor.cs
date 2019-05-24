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
            SpawnManager.Instance.DeleteAllChilds(spawner.transform);
            spawner.SpawnAll(spawner.prefabToSpawn);
        }
        else if(GUILayout.Button("Clear")) {
            SpawnManager.Instance.DeleteAllChilds(spawner.transform);
        }
    }
}
