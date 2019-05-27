using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public Creature dummyCreature = null; // Just for testing purposes

    /// <summary>
    /// A list of all spawnzones for all creatures in the scene. There can be multiplace instances of CreatureSpawnzones for the same creature.
    /// </summary>
    /// <typeparam name="CreatureSpawnzones"></typeparam>
    /// <returns></returns>
    public List<CreatureSpawnzones> creatureSpawnzones = new List<CreatureSpawnzones>();
    public Dictionary<Creature, List<Entity>> allSpawnedEntities = new Dictionary<Creature, List<Entity>>();
    
    public float minDistanceBetweenEntities = 10f;

    /// <summary>
    /// Will spawn creatures on pressing Space. Testing purposes.
    /// </summary>
    public void Update() {
        if(Input.GetButtonDown("Jump")) {
            SpawnManager.Instance.DeleteAllChilds(transform);
            Spawn(dummyCreature, 10);
        }
    }

    public void Spawn(Creature creature, int amount = 1)
    {
        float creatureTotalArea = 0f;

        for(int i = 0; i < creatureSpawnzones.Count; i++) {
            if(creatureSpawnzones[i].creature == creature) {
                creatureTotalArea += creatureSpawnzones[i].totalArea; // No break as there can be multiple creatureSpawnzones for the same creature
            }
        }

        for(int c = 0; c < amount; c++) {
            float pickedArea = (Random.value * creatureTotalArea);
            float currentArea = 0f;

            for(int i = 0; i < creatureSpawnzones.Count; i++) {
                for(int j = 0; j < creatureSpawnzones[i].areas.Length; j++) {
                    currentArea += creatureSpawnzones[i].areas[j];
                    if(currentArea >= pickedArea) {
                        Vector2 ab = creatureSpawnzones[i].polygonPoints[creatureSpawnzones[i].polygonTriangles[(j * 3) + 1]] - creatureSpawnzones[i].polygonPoints[creatureSpawnzones[i].polygonTriangles[j * 3]];
                        Vector2 ac = creatureSpawnzones[i].polygonPoints[creatureSpawnzones[i].polygonTriangles[(j * 3) + 2]] - creatureSpawnzones[i].polygonPoints[creatureSpawnzones[i].polygonTriangles[j * 3]];

                        float pickedMultiplierSumTotal = Random.value;
                        float pickedLengthAB = (pickedMultiplierSumTotal * Random.value);
                        float pickedLengthAC = (pickedMultiplierSumTotal - pickedLengthAB);

                        Vector2 pickedPoint = (creatureSpawnzones[i].polygonPoints[creatureSpawnzones[i].polygonTriangles[j * 3]] + (ab * pickedLengthAB + ac * pickedLengthAC));
                        Entity entity = GameObject.Instantiate(creature.prefab, new Vector3(pickedPoint.x, CreatureSpawnzones.GetHeight(pickedPoint.x, pickedPoint.y), pickedPoint.y), Quaternion.identity, transform).GetComponent<Entity>();
                        entity.creature = creature;

                        if(allSpawnedEntities.ContainsKey(creature)) {
                            allSpawnedEntities[creature].Add(entity);
                        }
                        else {
                            allSpawnedEntities.Add(creature, new List<Entity>() {entity});
                        }

                        break;
                    }
                }
            }
        }
    }

    public float CalculateSpawnMetric(Vector3 point, List<Entity> entities) {
        float spawnMetric = 0f;

        for(int e = 0; e < entities.Count; e++) {
            float distanceEntityPoint = Vector3.Distance(point, entities[e].transform.position);
            spawnMetric -= Mathf.Pow(distanceEntityPoint / 50f, -8f);
            spawnMetric += (distanceEntityPoint * (Random.Range(80, 120) / 100f));
        }

        return spawnMetric;
    }

    public void DeleteAllChilds(Transform transform)
    {
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        
        for(int i = 1; i < childs.Length; i++)
        {
            Entity entity = childs[i].GetComponent<Entity>();
            allSpawnedEntities[entity.creature].Remove(entity);
            DestroyImmediate(childs[i].gameObject);
        }
    }
}
