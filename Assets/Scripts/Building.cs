using UnityEngine;

public enum BuildingType
{
    Residential,
    Commercial,
    Industrial,
    Special
}

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public int level = 1;
    public int population = 10;
    public Color buildingColor = Color.white;

    [SerializeField] private ParticleSystem _addNewTower;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
    }

    public void Initialize(BuildingType type, int lvl)
    {
        buildingType = type;
        level = lvl;
        population = level * 10;
        UpdateVisuals();

    }

    public void PlayEffect()
    {
        _addNewTower.Play();
    }

    void UpdateVisuals()
    {
        if (meshRenderer == null) return;

        switch (buildingType)
        {
            case BuildingType.Residential:
                buildingColor = new Color(167f / 255f, 108f / 255f, 88f / 255f);
                break;
            case BuildingType.Commercial:
                buildingColor = new Color(89f / 255f, 83f / 255f, 86f / 255f);
                break;
            case BuildingType.Industrial:
                buildingColor = new Color(124f / 255f, 126f / 255f, 114f / 255f);
                break;
            case BuildingType.Special:
                buildingColor = new Color(179f / 255f, 143f / 255f, 160f / 255f);
                break;
        }

        // Используем URP-шадер
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("URP Shader not found! Make sure URP package is installed.");
            return;
        }

        Material mat = new Material(shader);
        mat.color = buildingColor;
        meshRenderer.material = mat;

        float baseHeight = 1f;
        float levelHeight = 0.7f;
        float height = baseHeight + (level - 1) * levelHeight;
        transform.localScale = new Vector3(1, height, 1);
    }


    public int GetPopulation()
    {
        return population;
    }
}
