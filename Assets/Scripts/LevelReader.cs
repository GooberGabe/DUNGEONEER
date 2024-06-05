using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

[Serializable]
public class LevelWaves
{
    public List<Wave> waves;
}

[Serializable]
public class Wave
{
    public List<string> heroes;
}

public class LevelReader : MonoBehaviour
{
    public LevelData levelData;
    private Dictionary<string, GameObject> jsonKey = new Dictionary<string, GameObject>();

    private void Start()
    {
        for (int i = 0; i < levelData.heroes.Length; i++)
        {
            jsonKey.Add(levelData.heroes[i].GetComponent<Hero>().entityName, levelData.heroes[i]);
        }
    }

    public List<List<GameObject>> Read(TextAsset file)
    {
        string jsonString = levelData.wavesJSON.ToString();
        LevelWaves wavesTxt = JsonUtility.FromJson<LevelWaves>(jsonString);

        List<List<GameObject>> waves = new List<List<GameObject>>();
        for (int i = 0; i < wavesTxt.waves.Count; i++)
        {
            waves.Add(new List<GameObject>());
            for (int j = 0; j < wavesTxt.waves[i].heroes.Count; j++)
            {
                waves[i].Add(jsonKey[wavesTxt.waves[i].heroes[j]]);
            }
        }

        return waves;
    }
}







