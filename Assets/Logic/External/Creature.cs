using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Combat/Creature", order = 1)]
public class Creature : ScriptableObject
{
    public string creatureName;
    public GameObject prefab;
}