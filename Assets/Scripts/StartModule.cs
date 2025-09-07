using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StartModule : FlexModule
{
    private LevelWaves level;
    private List<List<GameObject>> heroSpawns;
    private float counter = 0;
    public int waveNum = -1;
    private int spawnNum = 0;
    public bool allHeroesSpawned = false;
    public GameObject pathfinderFab;
    private Pathfinder pathfinder;
    public int archway;
    public bool connected;
    public override bool persistent => true;

    protected override void Start()
    {
        base.Start();
        preventPlacement = true;
        connected = false;
        level = GameManager.instance.GetComponent<LevelReader>().Read(this);
        heroSpawns = GameManager.instance.GetComponent<LevelReader>().GetWaves(level);
        GameManager.instance.GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();
    
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
            counter += Time.deltaTime;

            //(counter % ((11 - level.waves[waveNum].spawnRate) * 50) == 0
            if (counter > (11 - level.waves[waveNum].spawnRate) * 0.5f && waveNum < heroSpawns.Count)
            {
                if (spawnNum < heroSpawns[waveNum].Count)
                {
                    SpawnHero(heroSpawns[waveNum][spawnNum]);

                    spawnNum++;
                }
                counter = 0;
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
        GameManager.instance.encounteredHeroes.Add(hero.entityName);
    }

    private void SpawnPathfinder()
    {
        Pathfinder p = Instantiate(pathfinderFab, transform.position, Quaternion.identity).GetComponent<Pathfinder>();
        p.origin = this;
        pathfinder = p;
    }

    public void ValidateLocalPath()
    {
        connected = false;
        if (!pathfinder)
        {
            SpawnPathfinder();
        }
    }

    public void OnPathConnect()
    {
        connected = true;
        GameManager.instance.ConfirmPathValidation();
    }

    public void OnPathError()
    {
        connected = false;
        GameManager.instance.ConfirmPathValidation();
    }

}
