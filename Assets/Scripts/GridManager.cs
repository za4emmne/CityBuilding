using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float cellSize = 2f;
    public GameObject cellPrefab;

    [Header("Visual Settings")]
    public Material emptyMaterial;
    public Material highlightMaterial;

    private Cell[,] grid;
    private Vector3 gridOrigin;

    void Start()
    {
        gridOrigin = transform.position;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new Cell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = gridOrigin + new Vector3(x * cellSize, 0, y * cellSize);
                GameObject cellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";

                Cell cell = cellObj.GetComponent<Cell>();
                if (cell == null)
                {
                    cell = cellObj.AddComponent<Cell>();
                }

                cell.Initialize(x, y, this);
                grid[x, y] = cell;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return grid[x, y];
        }
        return null;
    }

    public bool PlaceBuilding(int x, int y, Building building)
    {
        Cell cell = GetCell(x, y);
        if (cell != null && cell.IsEmpty())
        {
            cell.SetBuilding(building);
            CheckForMerges(x, y);
            return true;
        }
        return false;
    }

    void CheckForMerges(int x, int y)
    {
        Cell currentCell = GetCell(x, y);
        if (currentCell == null || currentCell.CurrentBuilding == null) return;

        Building currentBuilding = currentCell.CurrentBuilding;
        List<Cell> matchingCells = new List<Cell>();

        // Используем алгоритм flood fill для поиска всех соседних одинаковых зданий
        HashSet<Cell> visited = new HashSet<Cell>();
        FindConnectedBuildings(x, y, currentBuilding.buildingType, currentBuilding.level, matchingCells, visited);

        if (matchingCells.Count >= 3)
        {
            MergeBuildings(matchingCells);
        }
    }

    void FindConnectedBuildings(int x, int y, BuildingType type, int level, List<Cell> matches, HashSet<Cell> visited)
    {
        Cell cell = GetCell(x, y);

        // Проверка границ и посещённости
        if (cell == null || visited.Contains(cell)) return;

        // Проверка наличия здания нужного типа и уровня
        if (cell.CurrentBuilding == null) return;
        if (cell.CurrentBuilding.buildingType != type || cell.CurrentBuilding.level != level) return;

        // Добавляем в список
        visited.Add(cell);
        matches.Add(cell);

        // Рекурсивно проверяем соседей (вверх, вниз, влево, вправо)
        FindConnectedBuildings(x - 1, y, type, level, matches, visited);
        FindConnectedBuildings(x + 1, y, type, level, matches, visited);
        FindConnectedBuildings(x, y - 1, type, level, matches, visited);
        FindConnectedBuildings(x, y + 1, type, level, matches, visited);
    }

    void MergeBuildings(List<Cell> cells)
    {
        if (cells.Count < 3) return;

        Building firstBuilding = cells[0].CurrentBuilding;
        int newLevel = firstBuilding.level + 1;
        BuildingType buildingType = firstBuilding.buildingType;

        // Удаляем все здания
        foreach (Cell cell in cells)
        {
            if (cell.CurrentBuilding != null)
            {
                Destroy(cell.CurrentBuilding.gameObject);
                cell.ClearBuilding();
            }
        }

        // Создаём улучшенное здание в первой ячейке
        CreateUpgradedBuilding(cells[0], buildingType, newLevel);
        GameManager.Instance.AddScore(newLevel * 100);

        Debug.Log($"Merged {cells.Count} buildings of type {buildingType} level {newLevel - 1} into level {newLevel}");
    }

    public void CreateUpgradedBuilding(Cell cell, BuildingType type, int level)
    {
        GameObject buildingObj = Instantiate(GameManager.Instance.buildingPrefab);
        Building building = buildingObj.GetComponent<Building>();

        if (building == null)
        {
            building = buildingObj.AddComponent<Building>();
        }

        building.Initialize(type, level);
        cell.SetBuilding(building);
    }

    public List<Cell> GetEmptyCells()
    {
        List<Cell> emptyCells = new List<Cell>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].IsEmpty())
                {
                    emptyCells.Add(grid[x, y]);
                }
            }
        }
        return emptyCells;
    }

    public bool HasEmptyCells()
    {
        return GetEmptyCells().Count > 0;
    }
}
