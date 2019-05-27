using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public List<CreatureSpawnZones> creatureSpawnZones = new List<CreatureSpawnZones>();
    public Dictionary<Creature, List<Entity>> allSpawnedEntities = new Dictionary<Creature, List<Entity>>();
    
    public float minDistanceBetweenEntities = 10f;

    /*/public void Spawn(Creature creature)
    {
        Vector3 bestSpawnPosition = SpawnManager.Instance.GetBestSpawnPosition(creature);
        Entity entity = GameObject.Instantiate(creature.prefab, new Vector3(bestSpawnPosition.x, Spawner.GetHeight((int) bestSpawnPosition.x, (int) bestSpawnPosition.z), bestSpawnPosition.z), Quaternion.identity, transform).GetComponent<Entity>();
        entity.creature = creature;

        if(allSpawnedEntities.ContainsKey(creature)) {
            allSpawnedEntities[creature].Add(entity);
        }
        else {
            allSpawnedEntities.Add(creature, new List<Entity>() {entity});
        }
    }*/

    public void RegisterSpawnZone(Creature creature, Shape shape) {
        for(int i = 0; i < creatureSpawnZones.Count; i++) {
            if(creatureSpawnZones[i].creature == creature) {
                creatureSpawnZones[i].spawnZones.Add(shape);
                break;
            }
        }

        creatureSpawnZones.Add(new CreatureSpawnZones(creature, shape));
    }

    /*/public Vector3 GetBestSpawnPosition(Creature creature) {
        List<Entity> entities = (allSpawnedEntities.ContainsKey(creature))?allSpawnedEntities[creature]:new List<Entity>();
        List<Vector3> bestSpawnPositions = new List<Vector3>();
        float bestSpawnMetric = float.MinValue;
        float spawnMetric = 0f;

        for(int i = 0; i < creatureSpawnZones.Count; i++) {
            if(creatureSpawnZones[i].creature == creature) {
                for(int j = 0; j < creatureSpawnZones[i].spawnZones.Count; j++) {
                    List<Vector3> possibleSpawnPoints = Spawner.GetPossibleSpawnPoints(creatureSpawnZones[i].spawnZones[j]);

                    for(int k = 0; k < possibleSpawnPoints.Count; k++) {
                        spawnMetric = CalculateSpawnMetric(possibleSpawnPoints[k], entities);
                        if(spawnMetric > bestSpawnMetric) {
                            bestSpawnMetric = spawnMetric;
                            bestSpawnPositions.Clear();
                            bestSpawnPositions.Add(possibleSpawnPoints[k]);
                        }
                        else if(spawnMetric == bestSpawnMetric) {
                            bestSpawnPositions.Add(possibleSpawnPoints[k]);
                        }
                        else {
                            k += 20;
                        }
                    }
                }

                break;
            }
        }

        return bestSpawnPositions[Random.Range(0, bestSpawnPositions.Count)];
    }*/

    public float CalculateSpawnMetric(Vector3 point, List<Entity> entities) {
        float spawnMetric = 0f;

        for(int e = 0; e < entities.Count; e++) {
            float distanceEntityPoint = Vector3.Distance(point, entities[e].transform.position);
            spawnMetric -= Mathf.Pow(distanceEntityPoint / 50f, -8f);
            spawnMetric += (distanceEntityPoint * (Random.Range(80, 120) / 100f));
        }

        return spawnMetric;
    }

    public void DeleteAllChilds(Transform transform) {
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        
        for(int i = 1; i < childs.Length; i++) {
       //     Entity entity = childs[i].GetComponent<Entity>();
      //      allSpawnedEntities[entity.creature].Remove(entity);
            DestroyImmediate(childs[i].gameObject);
        }
    }
}
