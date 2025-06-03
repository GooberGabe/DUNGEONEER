using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private LevelReader reader;
    public MapGrid grid;
    public Color[] skinTones;
    public int gold;
    public int tilesPlaced;
    public int maxTiles = 30;

    public bool isAlive;
    public bool isValidPath;
    public bool tilePlacement;
    public int totalHeroes;
    public bool playRound = false;

    public int round { get { return grid.startModules[0].GetRound() + 1; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance = this;
        reader = GetComponent<LevelReader>();
    }


    private void Update()
    {
        Monster dragon = grid.endModule.dragonRef;
        isAlive = dragon != null ? (dragon.hitPoints > 0) : false;

        // Make sure all modules have finished spawning heroes
        bool allHeroesSpawned = true;
        foreach (StartModule start in grid.startModules)
        {
            if (!start.allHeroesSpawned) allHeroesSpawned = false;
        }
        if (allHeroesSpawned && totalHeroes == 0 && playRound)
        {
            playRound = false;
            InterfaceManager.instance.Message("Round Completed!", 240);
            gold += (2 + grid.startModules[0].waveNum) * 20;
        }
    }

    public void NewRound()
    {
        // If in between rounds
        if (!playRound)
        {
            foreach (StartModule start in grid.startModules)
            {
                start.NewRound();
            }
            playRound = true;
        }
    }

    public bool TryBuy(int price)
    {
        if (gold >= price)
        {
            gold -= price;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanPlaceTiles()
    {
        return gold >= GetTilePrice() && !playRound;
    }

    public int GetTilePrice()
    {
        if (tilesPlaced >= maxTiles) return 20;
        return 0;
    }

    public MapGrid GetGrid()
    {
        return grid;
    }


    // NOTE: Changed return type from Creature to Entity
    public Entity Spawn(GameObject creatureToSpawn, Vector3 spawnPosition, int facingAngle = 180)
    {
        GameObject c = Instantiate(creatureToSpawn, spawnPosition, Quaternion.identity);
        c.transform.eulerAngles = new Vector3(0, facingAngle, 0);
        Entity creature = c.GetComponent<Entity>();
        
        return creature;
    }
}
