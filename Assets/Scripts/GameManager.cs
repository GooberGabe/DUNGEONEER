using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<AbilityCard> abilities;
    private bool requestBakeNavMesh = false;

    public HashSet<string> encounteredHeroes = new HashSet<string>();

    public int round { get { return grid.startModules[0].GetRound() + 1; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance = this;
        reader = GetComponent<LevelReader>();
        abilities = new List<AbilityCard>();
        grid.Initialize();
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
            HandleNewRound();
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Application has quit."); // This will only show in the console if the editor is still open after the build has quit
    }

    private void HandleNewRound()
    {
        playRound = false;
        
        InterfaceManager.instance.Message("Round Completed!", 230);
        LevelReader reader = GetComponent<LevelReader>();
        foreach (string hero in reader.NewHeroes(round, encounteredHeroes))
        {
            StartCoroutine(CreateHeroPreviewPanel(hero));
        }
    
        gold += 50;
    }

    public IEnumerator CreateHeroPreviewPanel(string hero)
    {
        yield return new WaitForSeconds(4);
        InterfaceManager.instance.CreateHeroPreviewPanel(reader.GetHeroByName(hero));
    }

    private void LateUpdate()
    {
        if (requestBakeNavMesh) ReloadPath(true);
        requestBakeNavMesh = false;
    }

    public void RequestReload()
    {
        requestBakeNavMesh = true;
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
        if (price == 0) return true;
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

    public void AddAbility(AbilityCard ability)
    {
        List<string> abilityNames = abilities.Select(obj => obj.abilityName).ToList();
        if (!abilityNames.Contains(ability.abilityName))
        {
            InterfaceManager.instance.AddAbilityCard(ability);
        }
    }

    public void ClearMap()
    {
        GetGrid().ClearMap();
    }

    /// <summary>
    /// Spawn in Pathfinder agents for all StartModules, and await confirmations.
    /// </summary>
    public void StartPathValidation()
    {
        isValidPath = false;
        MapGrid grid = GetGrid();
        foreach (StartModule origin in grid.startModules)
        {
            origin.ValidateLocalPath();
        }
    }
    public void ConfirmPathValidation()
    {
        MapGrid grid = GetGrid();
        isValidPath = grid.startModules.All(item => item.connected);
    }

    public void ReloadPath(bool validate)
    {
        GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();
        if (validate) StartPathValidation();
    }

    public Vector2[] GetStartPositions()
    {
        return reader.level.StartPositions;
    }

    public Vector2 GetEndPosition()
    {
        return reader.level.EndPositon;
    }

    public List<RoomPreview> GetRoomModules()
    {
        return reader.level.RoomModules;
    }

}
