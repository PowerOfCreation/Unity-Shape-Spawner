using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public List<CreatureSpawnZones> creatureSpawnZones = new List<CreatureSpawnZones>();
    public Dictionary<Creature, List<Entity>> allSpawnedEntities = new Dictionary<Creature, List<Entity>>();
    
    public void RegisterSpawnZone(Creature creature, Shape shape) {
        for(int i = 0; i < creatureSpawnZones.Count; i++) {
            if(creatureSpawnZones[i].creature == creature) {
                creatureSpawnZones[i].spawnZones.Add(shape);
                break;
            }
        }

        creatureSpawnZones.Add(new CreatureSpawnZones(creature, shape));
    }

    public Vector3 GetBestSpawnPosition(Creature creature) {
        List<Entity> entities;

        if(allSpawnedEntities.ContainsKey(creature)) {
            entities = allSpawnedEntities[creature];
        }
        else {
            entities = new List<Entity>();
        }
        
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
                    for(int e = 0; e < entities.Count; e++) {
                        spawnMetric += Mathf.Pow(Vector3.Distance(possibleSpawnPoints[j], entities[e].transform.position), 2);
                    }

                    if(spawnMetric > bestSpawnMetric) {
                        bestSpawnMetric = spawnMetric;
                        allSpawnedEntities.Clear();
                    }
                    else if(spawnMetric == bestSpawnMetric) {
                        bestSpawnPositions.Add(possibleSpawnPoints[j]);
                    }
                }

                break;
            }
        }

        return bestSpawnPositions.Shuffle()[0];
    }
}
