using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureSpawnZones
{
    public CreatureSpawnZones(Creature creature, Shape firstShape) {
        this.creature = creature;
        this.spawnZones = new List<Shape>();
        this.spawnZones.Add(firstShape);
    }

    public Creature creature;
    public List<Shape> spawnZones;
}
