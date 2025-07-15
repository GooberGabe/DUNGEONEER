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
    public int spawnRate;
    public List<string> heroes;
}

public class LevelReader : MonoBehaviour
{
    public Level level;
    private Dictionary<string, GameObject> jsonKey = new Dictionary<string, GameObject>();

    public LevelWaves Read(StartModule start)
    {
        List<TextAsset> data = level.WaveData;
        TextAsset json = data[GameManager.instance.GetGrid().startModules.IndexOf(start)];
        string jsonString = json.ToString();
        LevelWaves wavesTxt = JsonUtility.FromJson<LevelWaves>(jsonString);

        return wavesTxt;
    }

    private void Start()
    {
        GameObject[] heroes = Resources.LoadAll<GameObject>("Heroes");
        foreach (GameObject hero in heroes)
        {
            jsonKey.Add(hero.name, hero);
        }
    }

    public List<List<GameObject>> GetWaves(LevelWaves wavesTxt)
    {
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















