using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public MapGrid grid;
    public int gold;
    public int tilesPlaced;
    public int maxTiles = 30;

    public bool isAlive;
    public bool isValidPath;
    public bool tilePlacement;
    public int totalHeroes;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance = this;
    }


    private void Update()
    {
        Monster dragon = grid.endModule.dragonRef;
        isAlive = dragon != null ? (dragon.hitPoints > 0) : false;
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
        return gold >= GetTilePrice() && !GetGrid().startModule.playRound;
    }

    public int GetTilePrice()
    {
        if (tilesPlaced > maxTiles) return 20;
        return 0;
    }

    public MapGrid GetGrid()
    {
        return grid;
    }

    public LevelData GetLevelData() 
    { 
        return GetComponent<LevelReader>().levelData;
    }

    public Creature Spawn(GameObject creatureToSpawn, Vector3 spawnPosition, int facingAngle = 180)
    {
        GameObject c = Instantiate(creatureToSpawn, spawnPosition, Quaternion.identity);
        c.transform.eulerAngles = new Vector3(0, facingAngle, 0);
        Creature creature = c.GetComponent<Creature>();
        
        return creature;
    }
}
