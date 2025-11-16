using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public Building CurrentBuilding { get; private set; }

    private GridManager gridManager;
    private Renderer cellRenderer;
    private Material originalMaterial;

    public void Initialize(int x, int y, GridManager manager)
    {
        gridX = x;
        gridY = y;
        gridManager = manager;
        cellRenderer = GetComponent<Renderer>();

        if (cellRenderer != null)
        {
            originalMaterial = cellRenderer.material;
        }
    }

    public bool IsEmpty()
    {
        return CurrentBuilding == null;
    }

    public void SetBuilding(Building building)
    {
        CurrentBuilding = building;
        building.transform.position = transform.position + Vector3.up * 0.5f;
        building.transform.SetParent(transform);
    }

    public void ClearBuilding()
    {
        CurrentBuilding = null;
    }

    void OnMouseEnter()
    {
        if (IsEmpty() && cellRenderer != null && gridManager.highlightMaterial != null)
        {
            cellRenderer.material = gridManager.highlightMaterial;
        }
    }

    void OnMouseExit()
    {
        if (cellRenderer != null && originalMaterial != null)
        {
            cellRenderer.material = originalMaterial;
        }
    }

    void OnMouseDown()
    {
        if (IsEmpty() && GameManager.Instance != null)
        {
            GameManager.Instance.OnCellClicked(this);
        }
    }
}
