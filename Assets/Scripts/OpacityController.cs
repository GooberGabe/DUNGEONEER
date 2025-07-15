using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// High-performance opacity controller for per-frame updates
/// </summary>
public class OpacityController : MonoBehaviour
{
    [System.Serializable]
    public class MaterialData
    {
        public Material material;
        public Color originalColor;
        public bool hasBaseColor;
        public bool hasAlpha;
        public int colorPropertyID;
        public int baseColorPropertyID;
        public int alphaPropertyID;

        public MaterialData(Material mat)
        {
            material = mat;

            // Cache property IDs for better performance
            colorPropertyID = Shader.PropertyToID("_Color");
            baseColorPropertyID = Shader.PropertyToID("_BaseColor");
            alphaPropertyID = Shader.PropertyToID("_Alpha");

            // Check which properties exist
            hasBaseColor = mat.HasProperty(baseColorPropertyID);
            hasAlpha = mat.HasProperty(alphaPropertyID);

            // Store original color
            if (hasBaseColor)
                originalColor = mat.GetColor(baseColorPropertyID);
            else if (mat.HasProperty(colorPropertyID))
                originalColor = mat.GetColor(colorPropertyID);
            else
                originalColor = Color.white;
        }
    }

    private List<MaterialData> cachedMaterials = new List<MaterialData>();
    private float currentOpacity = 1f;
    private bool isInitialized = false;

    [SerializeField] private bool includeChildren = true;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize the opacity controller - call this once
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;

        cachedMaterials.Clear();

        // Get all renderers
        Renderer[] renderers = includeChildren ?
            GetComponentsInChildren<Renderer>() :
            GetComponents<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                // Create material instance only once
                Material instanceMaterial = new Material(material);
                renderer.material = instanceMaterial;

                // Cache material data
                cachedMaterials.Add(new MaterialData(instanceMaterial));
            }
        }

        isInitialized = true;
    }

    /// <summary>
    /// Set opacity - optimized for per-frame calls
    /// </summary>
    /// <param name="opacity">Opacity value (0.0 = transparent, 1.0 = opaque)</param>
    public void SetOpacity(float opacity)
    {
        if (!isInitialized) Initialize();

        opacity = Mathf.Clamp01(opacity);

        // Skip if opacity hasn't changed
        if (Mathf.Approximately(currentOpacity, opacity)) return;

        currentOpacity = opacity;

        // Update all cached materials
        foreach (MaterialData matData in cachedMaterials)
        {
            UpdateMaterialOpacity(matData, opacity);
        }
    }

    /// <summary>
    /// Optimized material opacity update
    /// </summary>
    private void UpdateMaterialOpacity(MaterialData matData, float opacity)
    {
        Color newColor = matData.originalColor;
        newColor.a = opacity;

        // Use cached property IDs for better performance
        if (matData.hasBaseColor)
        {
            matData.material.SetColor(matData.baseColorPropertyID, newColor);
        }
        else
        {
            matData.material.SetColor(matData.colorPropertyID, newColor);
        }

        if (matData.hasAlpha)
        {
            matData.material.SetFloat(matData.alphaPropertyID, opacity);
        }
    }

    /// <summary>
    /// Get current opacity
    /// </summary>
    public float GetOpacity()
    {
        return currentOpacity;
    }

    /// <summary>
    /// Reset to original opacity
    /// </summary>
    public void ResetOpacity()
    {
        SetOpacity(1f);
    }

    void OnDestroy()
    {
        // Clean up material instances
        foreach (MaterialData matData in cachedMaterials)
        {
            if (matData.material != null)
            {
                DestroyImmediate(matData.material);
            }
        }
        cachedMaterials.Clear();
    }
}

/// <summary>
/// Static utility for one-time opacity changes (less performant but more convenient)
/// </summary>
public static class OpacityUtils
{
    private static readonly int ColorPropertyID = Shader.PropertyToID("_Color");
    private static readonly int BaseColorPropertyID = Shader.PropertyToID("_BaseColor");
    private static readonly int AlphaPropertyID = Shader.PropertyToID("_Alpha");

    /// <summary>
    /// Quick opacity change for one-time use (not optimized for per-frame)
    /// </summary>
    public static void SetOpacityImmediate(GameObject gameObject, float opacity, bool includeChildren = true)
    {
        opacity = Mathf.Clamp01(opacity);

        Renderer[] renderers = includeChildren ?
            gameObject.GetComponentsInChildren<Renderer>() :
            gameObject.GetComponents<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                SetMaterialOpacityFast(material, opacity);
            }
        }
    }

    private static void SetMaterialOpacityFast(Material material, float opacity)
    {
        // Use cached property IDs
        if (material.HasProperty(BaseColorPropertyID))
        {
            Color color = material.GetColor(BaseColorPropertyID);
            color.a = opacity;
            material.SetColor(BaseColorPropertyID, color);
        }
        else if (material.HasProperty(ColorPropertyID))
        {
            Color color = material.GetColor(ColorPropertyID);
            color.a = opacity;
            material.SetColor(ColorPropertyID, color);
        }

        if (material.HasProperty(AlphaPropertyID))
        {
            material.SetFloat(AlphaPropertyID, opacity);
        }
    }
}

/// <summary>
/// Extension methods for convenient usage
/// </summary>
public static class GameObjectOpacityExtensions
{
    /// <summary>
    /// Get or add OpacityController component
    /// </summary>
    public static OpacityController GetOpacityController(this GameObject gameObject)
    {
        OpacityController controller = gameObject.GetComponent<OpacityController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<OpacityController>();
        }
        return controller;
    }

    /// <summary>
    /// Set opacity using the performant controller
    /// </summary>
    public static void SetOpacityPerformant(this GameObject gameObject, float opacity)
    {
        gameObject.GetOpacityController().SetOpacity(opacity);
    }
}