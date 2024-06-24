using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StartModule : FlexModule
{
    private LevelWaves level;
    private List<List<GameObject>> heroSpawns;
    private int counter = 0;
    public int waveNum = -1;
    private int spawnNum = 0;
    public bool allHeroesSpawned = false;
    public GameObject pathfinder;
    public int archway;

    protected override void Start()
    {
        base.Start();
        preventPlacement = true;
        level = GameManager.instance.GetComponent<LevelReader>().Read(this);
        heroSpawns = GameManager.instance.GetComponent<LevelReader>().GetWaves(level);
        GameManager.instance.GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();

        SpawnPathfinder();
    
        openings = new bool[4] { false, false, false, false };
        openings[archway] = true;
    
    }

    public int GetRound()
    {
        return waveNum;
    }

    private void Update()
    {
        // If during round
        if (GameManager.instance.playRound)
        {
            counter++;
            
            if (counter % ((11 - level.waves[waveNum].spawnRate) * 50) == 0 && waveNum < heroSpawns.Count)
            {
                if (spawnNum < heroSpawns[waveNum].Count)
                {
                    SpawnHero(heroSpawns[waveNum][spawnNum]);

                    spawnNum++;
                }
            }
            // If all heroes have been spawned and all heroes have been killed
            if (spawnNum >= heroSpawns[waveNum].Count)
            {
                allHeroesSpawned = true;
            }

        }
        
    }

    public override void Assemble()
    {
        base.Assemble();
        for (int i = 0; i < 4; i++)
        {
            if (i != archway)
            {
                gridTile.adjacents[i].validPlacement = false;
            }
            else
            {
                walls.GetChild(i).gameObject.SetActive(false);
                arches.GetChild(i).gameObject.SetActive(true);
            }
        }
        
    }

    public void NewRound()
    {
        waveNum++;
        spawnNum = 0;
        counter = 0;
        allHeroesSpawned = false;
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

}
