using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
[System.Serializable]
public class IconBakeSettings
{
    [Header("Camera Settings")]
    public Camera iconCamera;
    public int iconResolution = 256;
    public bool useOrthographic = true;
    public float orthographicSize = 2f;
    public float fieldOfView = 60f;

    [Header("Positioning")]
    public Vector3 cameraOffset = new Vector3(0, 0, -5);
    public Vector3 objectRotation = Vector3.zero;
    public bool autoFrameObject = true;
    public float framingPadding = 0.2f;

    [Header("Lighting")]
    public Light[] customLights;
    public bool useSceneLighting = true;

    [Header("Background")]
    public Color backgroundColor = Color.clear;
    public bool useTransparentBackground = true;

    [Header("Output")]
    public string outputFolder = "Assets/GeneratedIcons/";
    public string filePrefix = "";
    public string fileSuffix = "_icon";
}

public class CustomPrefabIconBaker : EditorWindow
{
    private IconBakeSettings settings = new IconBakeSettings();
    private List<GameObject> prefabsToBake = new List<GameObject>();
    private Vector2 scrollPosition;
    private SerializedObject serializedSettings;

    [MenuItem("Tools/Custom Prefab Icon Baker")]
    static void ShowWindow()
    {
        GetWindow<CustomPrefabIconBaker>("Custom Icon Baker");
    }

    void OnEnable()
    {
        serializedSettings = new SerializedObject(this);
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Custom Prefab Icon Baker", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Settings section
        DrawSettings();

        GUILayout.Space(15);

        // Prefab list section
        DrawPrefabList();

        GUILayout.Space(15);

        // Action buttons
        DrawActionButtons();

        EditorGUILayout.EndScrollView();
    }

    void DrawSettings()
    {
        GUILayout.Label("Baking Settings", EditorStyles.boldLabel);

        settings.iconCamera = (Camera)EditorGUILayout.ObjectField("Icon Camera", settings.iconCamera, typeof(Camera), true);
        settings.iconResolution = EditorGUILayout.IntSlider("Resolution", settings.iconResolution, 64, 1024);

        settings.useOrthographic = EditorGUILayout.Toggle("Use Orthographic", settings.useOrthographic);
        if (settings.useOrthographic)
        {
            settings.orthographicSize = EditorGUILayout.FloatField("Orthographic Size", settings.orthographicSize);
        }
        else
        {
            settings.fieldOfView = EditorGUILayout.Slider("Field of View", settings.fieldOfView, 10f, 120f);
        }

        settings.cameraOffset = EditorGUILayout.Vector3Field("Camera Offset", settings.cameraOffset);
        settings.objectRotation = EditorGUILayout.Vector3Field("Object Rotation", settings.objectRotation);
        settings.autoFrameObject = EditorGUILayout.Toggle("Auto Frame Object", settings.autoFrameObject);

        if (settings.autoFrameObject)
        {
            settings.framingPadding = EditorGUILayout.Slider("Framing Padding", settings.framingPadding, 0f, 1f);
        }

        settings.useSceneLighting = EditorGUILayout.Toggle("Use Scene Lighting", settings.useSceneLighting);
        settings.backgroundColor = EditorGUILayout.ColorField("Background Color", settings.backgroundColor);
        settings.useTransparentBackground = EditorGUILayout.Toggle("Transparent Background", settings.useTransparentBackground);

        settings.outputFolder = EditorGUILayout.TextField("Output Folder", settings.outputFolder);
        settings.filePrefix = EditorGUILayout.TextField("File Prefix", settings.filePrefix);
        settings.fileSuffix = EditorGUILayout.TextField("File Suffix", settings.fileSuffix);
    }

    void DrawPrefabList()
    {
        GUILayout.Label("Prefabs to Bake", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Selected"))
        {
            AddSelectedPrefabs();
        }
        if (GUILayout.Button("Clear List"))
        {
            prefabsToBake.Clear();
        }
        EditorGUILayout.EndHorizontal();

        // Display prefab list
        for (int i = prefabsToBake.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            prefabsToBake[i] = (GameObject)EditorGUILayout.ObjectField(prefabsToBake[i], typeof(GameObject), false);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                prefabsToBake.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (prefabsToBake.Count == 0)
        {
            EditorGUILayout.HelpBox("No prefabs added. Select prefabs in the project and click 'Add Selected'.", MessageType.Info);
        }
    }

    void DrawActionButtons()
    {
        EditorGUI.BeginDisabledGroup(settings.iconCamera == null || prefabsToBake.Count == 0);

        if (GUILayout.Button("Bake All Icons", GUILayout.Height(30)))
        {
            BakeAllIcons();
        }

        EditorGUI.EndDisabledGroup();

        if (settings.iconCamera == null)
        {
            EditorGUILayout.HelpBox("Please assign an Icon Camera to proceed.", MessageType.Warning);
        }
    }

    void AddSelectedPrefabs()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".prefab"))
            {
                if (!prefabsToBake.Contains(obj))
                {
                    prefabsToBake.Add(obj);
                }
            }
        }
    }

    void BakeAllIcons()
    {
        if (!Directory.Exists(settings.outputFolder))
        {
            Directory.CreateDirectory(settings.outputFolder);
        }

        // Store original camera settings
        Vector3 originalCameraPosition = settings.iconCamera.transform.position;
        Quaternion originalCameraRotation = settings.iconCamera.transform.rotation;
        bool originalOrthographic = settings.iconCamera.orthographic;
        float originalOrthographicSize = settings.iconCamera.orthographicSize;
        float originalFieldOfView = settings.iconCamera.fieldOfView;
        Color originalBackgroundColor = settings.iconCamera.backgroundColor;
        CameraClearFlags originalClearFlags = settings.iconCamera.clearFlags;

        // Setup camera
        settings.iconCamera.orthographic = settings.useOrthographic;
        settings.iconCamera.orthographicSize = settings.orthographicSize;
        settings.iconCamera.fieldOfView = settings.fieldOfView;
        settings.iconCamera.backgroundColor = settings.backgroundColor;
        settings.iconCamera.clearFlags = settings.useTransparentBackground ?
            CameraClearFlags.SolidColor : CameraClearFlags.Skybox;

        // Create render texture
        RenderTexture renderTexture = new RenderTexture(settings.iconResolution, settings.iconResolution, 24, RenderTextureFormat.ARGB32);
        settings.iconCamera.targetTexture = renderTexture;

        try
        {
            for (int i = 0; i < prefabsToBake.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Baking Icons",
                    $"Processing {prefabsToBake[i].name} ({i + 1}/{prefabsToBake.Count})",
                    (float)i / prefabsToBake.Count);

                BakeSingleIcon(prefabsToBake[i], renderTexture);
            }
        }
        finally
        {
            // Restore original camera settings
            settings.iconCamera.transform.position = originalCameraPosition;
            settings.iconCamera.transform.rotation = originalCameraRotation;
            settings.iconCamera.orthographic = originalOrthographic;
            settings.iconCamera.orthographicSize = originalOrthographicSize;
            settings.iconCamera.fieldOfView = originalFieldOfView;
            settings.iconCamera.backgroundColor = originalBackgroundColor;
            settings.iconCamera.clearFlags = originalClearFlags;
            settings.iconCamera.targetTexture = null;

            // Cleanup
            DestroyImmediate(renderTexture);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Successfully baked {prefabsToBake.Count} icons to {settings.outputFolder}");
    }

    void BakeSingleIcon(GameObject prefab, RenderTexture renderTexture)
    {
        // Instantiate prefab
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        try
        {
            // Apply object rotation
            instance.transform.rotation = Quaternion.Euler(settings.objectRotation);

            // Position camera
            if (settings.autoFrameObject)
            {
                FrameObject(instance);
            }
            else
            {
                settings.iconCamera.transform.position = instance.transform.position + settings.cameraOffset;
                settings.iconCamera.transform.LookAt(instance.transform.position);
            }

            // Render
            settings.iconCamera.Render();

            // Save texture
            SaveRenderTexture(renderTexture, GetIconFileName(prefab));
        }
        finally
        {
            // Clean up instance
            DestroyImmediate(instance);
        }
    }

    void FrameObject(GameObject obj)
    {
        Bounds bounds = GetObjectBounds(obj);

        if (bounds.size == Vector3.zero)
        {
            // Fallback if no renderers found
            settings.iconCamera.transform.position = obj.transform.position + settings.cameraOffset;
            settings.iconCamera.transform.LookAt(obj.transform.position);
            return;
        }

        Vector3 center = bounds.center;
        float maxExtent = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        if (settings.useOrthographic)
        {
            settings.iconCamera.orthographicSize = (maxExtent * 0.5f) * (1f + settings.framingPadding);
            settings.iconCamera.transform.position = center + settings.cameraOffset.normalized * 10f;
        }
        else
        {
            float distance = (maxExtent * (1f + settings.framingPadding)) / (2f * Mathf.Tan(settings.fieldOfView * 0.5f * Mathf.Deg2Rad));
            settings.iconCamera.transform.position = center + settings.cameraOffset.normalized * distance;
        }

        settings.iconCamera.transform.LookAt(center);
    }

    Bounds GetObjectBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    void SaveRenderTexture(RenderTexture renderTexture, string fileName)
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        byte[] pngData = texture.EncodeToPNG();
        string fullPath = Path.Combine(settings.outputFolder, fileName);
        File.WriteAllBytes(fullPath, pngData);

        DestroyImmediate(texture);
    }

    string GetIconFileName(GameObject prefab)
    {
        return $"{settings.filePrefix}{prefab.name}{settings.fileSuffix}.png";
    }
}
#endif