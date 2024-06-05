using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartModule : Module
{
    private LevelData levelData;
    private List<List<GameObject>> heroSpawns;
    private int counter = 0;
    private int waveNum = -1;
    private int spawnNum = 0;
    public bool playRound = false;
    public GameObject pathfinder;

    private void Start()
    {
        preventPlacement = true;
        levelData = GameManager.instance.GetLevelData();
        heroSpawns = GameManager.instance.GetComponent<LevelReader>().Read(levelData.wavesJSON);
        GameManager.instance.GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();
        SpawnPathfinder();
    }

    public int GetRound()
    {
        return waveNum;
    }

    private void Update()
    {
        // If during round
        if (playRound)
        {
            counter++;
            
            if (counter % ((11 - levelData.spawnRate) * 70) == 0 && waveNum < heroSpawns.Count)
            {
                if (spawnNum < heroSpawns[waveNum].Count)
                {
                    SpawnHero(heroSpawns[waveNum][spawnNum]);

                    spawnNum++;
                }
            }
            // If all heroes have been spawned and all heroes have been killed
            if (spawnNum >= heroSpawns[waveNum].Count && GameManager.instance.totalHeroes == 0)
            {
                playRound = false;
                InterfaceManager.instance.Message("Round Completed!", 240);
                GameManager.instance.gold += waveNum * 30;
            }
        }
        
    }

    private void SpawnHero(GameObject heroPrefab)
    {
        Hero hero = (Hero)GameManager.instance.Spawn(heroPrefab, transform.position, facingAngle:0);

        //hero.targetDestination = GameManager.instance.GetGrid().endModule.transform;          // Un-comment when EndModule is implemented fully
        hero.mainDestination = GameManager.instance.GetGrid().endModule.dragonRef.transform;
    }

    private void SpawnPathfinder()
    {
        Pathfinder p = Instantiate(pathfinder, transform.position, Quaternion.identity).GetComponent<Pathfinder>();
    }

    public void NewRound()
    {
        // If in between rounds
        if (!playRound)
        {
            waveNum++;
            spawnNum = 0;
            counter = 0;
            playRound = true;
        }
    }

}
