using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public List<CreatureSpawnZones> creatureSpawnZones = new List<CreatureSpawnZones>();
    public Dictionary<Creature, List<Entity>> allSpawnedEntities = new Dictionary<Creature, List<Entity>>();
    

    public void Spawn(Creature creature)
    {
        Vector3? bestSpawnPosition = SpawnManager.Instance.GetBestSpawnPosition(creature);
        if(bestSpawnPosition == null) { return; }
        Entity entity = GameObject.Instantiate(creature.prefab, (Vector3) bestSpawnPosition, Quaternion.identity, transform).GetComponent<Entity>();
        entity.creature = creature;

        if(allSpawnedEntities.ContainsKey(creature)) {
            allSpawnedEntities[creature].Add(entity);
        }
        else {
            allSpawnedEntities.Add(creature, new List<Entity>() {entity});
        }
    }

    public void RegisterSpawnZone(Creature creature, Shape shape) {
        for(int i = 0; i < creatureSpawnZones.Count; i++) {
            if(creatureSpawnZones[i].creature == creature) {
                creatureSpawnZones[i].spawnZones.Add(shape);
                break;
            }
        }

        creatureSpawnZones.Add(new CreatureSpawnZones(creature, shape));
    }

    public Vector3? GetBestSpawnPosition(Creature creature) {
        List<Entity> entities = (allSpawnedEntities.ContainsKey(creature))?allSpawnedEntities[creature]:new List<Entity>();
        List<Vector3> bestSpawnPositions = new List<Vector3>();
        float bestSpawnMetric = 0f;
        float spawnMetric = 0f;

        for(int i = 0; i < creatureSpawnZones.Count; i++) {
            if(creatureSpawnZones[i].creature == creature) {
                List<Vector3> possibleSpawnPoints = new List<Vector3>();

                for(int j = 0; j < creatureSpawnZones[i].spawnZones.Count; j++) {
                    possibleSpawnPoints.AddRange(Spawner.GetPossibleSpawnPoints(creatureSpawnZones[i].spawnZones[j]));
                }

                for(int j = 0; j < possibleSpawnPoints.Count; j++) {
                    spawnMetric = 0;

                    for(int e = 0; e < entities.Count; e++) {
                        float distanceEntityPoint = Vector3.Distance(possibleSpawnPoints[j], entities[e].transform.position);
                        spawnMetric += Mathf.Pow(distanceEntityPoint * (Random.Range(90, 111) / 100f), 2f);
                        spawnMetric -= Mathf.Pow(distanceEntityPoint / 10f, -24f);
                    }

                    if(spawnMetric > bestSpawnMetric) {
                        bestSpawnMetric = spawnMetric;
                        bestSpawnPositions.Clear();
                        bestSpawnPositions.Add(possibleSpawnPoints[j]);
                    }
                    else if(spawnMetric == bestSpawnMetric) {
                        bestSpawnPositions.Add(possibleSpawnPoints[j]);
                    }
                }

                break;
            }
        }
        
        if(bestSpawnPositions.Count > 0) {
            return bestSpawnPositions.Shuffle()[0];
        }

        return null;
    }

    public void DeleteAllChilds(Transform transform) {
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        
        for(int i = 1; i < childs.Length; i++) {
            Entity entity = childs[i].GetComponent<Entity>();
            allSpawnedEntities[entity.creature].Remove(entity);
            DestroyImmediate(childs[i].gameObject);
        }
    }
}
