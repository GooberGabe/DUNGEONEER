using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public TextAsset wavesJSON;
    public GameObject[] heroes;

    [InspectorRange(1,10)]
    public float spawnRate;
}