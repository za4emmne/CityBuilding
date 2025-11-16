using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public GridManager gridManager;
    public GameObject buildingPrefab;
    public UIManager uiManager;

    [Header("Game Settings")]
    public int startingJokers = 3;
    public List<BuildingType> availableTypes = new List<BuildingType>();

    [Header("Effects")]
    private ParticleSystem _updateTower;

    private Queue<Building> nextBuildings = new Queue<Building>();
    private int currentScore = 0;
    private int totalPopulation = 0;
    private int jokersRemaining;
    private bool gameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        jokersRemaining = startingJokers;
        InitializeAvailableTypes();
        GenerateNextBuildings(3);
        UpdateUI();
    }

    void InitializeAvailableTypes()
    {
        if (availableTypes.Count == 0)
        {
            availableTypes.Add(BuildingType.Residential);
            availableTypes.Add(BuildingType.Commercial);
            availableTypes.Add(BuildingType.Industrial);
        }
    }

    void GenerateNextBuildings(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Building building = CreateRandomBuilding();
            nextBuildings.Enqueue(building);
            building.gameObject.SetActive(false);
        }
    }

    Building CreateRandomBuilding()
    {
        GameObject buildingObj = Instantiate(buildingPrefab);
        Building building = buildingObj.GetComponent<Building>();

        if (building == null)
        {
            building = buildingObj.AddComponent<Building>();
        }

        BuildingType randomType = availableTypes[Random.Range(0, availableTypes.Count)];
        building.Initialize(randomType, 1);

        return building;
    }

    public void OnCellClicked(Cell cell)
    {
        if (gameOver || nextBuildings.Count == 0) return;

        if (cell.IsEmpty())
        {
            Building nextBuilding = nextBuildings.Dequeue();
            nextBuilding.gameObject.SetActive(true);

            if (gridManager.PlaceBuilding(cell.gridX, cell.gridY, nextBuilding))
            {
                UpdatePopulation();
                GenerateNextBuildings(1);
                UpdateUI();
                CheckGameOver();
            }
        }
    }

    public void CreateUpgradedBuilding(Cell cell, BuildingType type, int level)
    {
        GameObject buildingObj = Instantiate(buildingPrefab);
        Building building = buildingObj.GetComponent<Building>();

        if (building == null)
        {
            building = buildingObj.AddComponent<Building>();
        }

        building.Initialize(type, level);
        cell.SetBuilding(building);

        UpdatePopulation();
        building.PlayEffect();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    void UpdatePopulation()
    {
        totalPopulation = 0;
        Building[] allBuildings = FindObjectsByType<Building>(FindObjectsSortMode.None);

        foreach (Building building in allBuildings)
        {
            totalPopulation += building.GetPopulation();
        }
        UpdateUI();
    }


    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdatePopulation(totalPopulation);
            uiManager.UpdateJokers(jokersRemaining);
            uiManager.UpdateNextBuildings(GetNextBuildingsInfo());
        }
    }

    List<string> GetNextBuildingsInfo()
    {
        List<string> info = new List<string>();

        foreach (Building building in nextBuildings)
        {
            info.Add($"{building.buildingType} Lv.{building.level}");
        }
        return info;
    }

    void CheckGameOver()
    {
        if (!gridManager.HasEmptyCells())
        {
            gameOver = true;
            if (uiManager != null)
            {
                uiManager.ShowGameOver(currentScore, totalPopulation);
            }
        }
    }

    public void UseJoker()
    {
        if (jokersRemaining > 0)
        {
            jokersRemaining--;
            // Implement joker functionality here
            UpdateUI();
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
