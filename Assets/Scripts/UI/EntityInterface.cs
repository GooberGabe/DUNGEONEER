using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInterface : MonoBehaviour
{
    public Entity target;
    public GameObject spotLight;
    public GameObject[] rangeDisplays;

    private bool _showRange;
    private bool lightOff;

    private void Start()
    {
        target = transform.parent.GetComponent<Entity>();
        lightOff = true;
        BuildRangeDisplays();
    }

    private void Update()
    {
        bool preview = transform.parent.GetComponent<PlacementPreview>() != null;
        ShowRange(preview || InterfaceManager.instance.selectedEntity == target);
        if (lightOff || preview)
        {
            ShowSpotlight(false);
        }
        lightOff = true;
    }

    private void BuildRangeDisplays()
    {
        if (target == null) return;

        Collider[] targetColliders = target.GetRangeColliders();
        if (targetColliders == null || targetColliders.Length == 0) return;

        // Initialize the rangeDisplays array
        rangeDisplays = new GameObject[targetColliders.Length];

        for (int i = 0; i < targetColliders.Length; i++)
        {
            Collider col = targetColliders[i];
            if (col == null) continue;

            // Create the display GameObject
            GameObject rangeDisplay = new GameObject($"RangeDisplay_{i}");
            rangeDisplay.transform.SetParent(transform);

            // Copy transform properties from the collider
            rangeDisplay.transform.localPosition = GetColliderCenter(col);
            rangeDisplay.transform.localPosition += Vector3.down * 0.01f;
            rangeDisplay.transform.localRotation = col.transform.localRotation;

            // Add MeshRenderer and MeshFilter components
            MeshRenderer meshRenderer = rangeDisplay.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = rangeDisplay.AddComponent<MeshFilter>();

            // Create appropriate mesh and scale based on collider type
            if (col is SphereCollider sphereCol)
            {
                meshFilter.mesh = CreateCylinderMesh();
                float diameter = sphereCol.radius * 2f;
                rangeDisplay.transform.localScale = new Vector3(diameter, 0.1f, diameter);
            }
            else if (col is BoxCollider boxCol)
            {
                meshFilter.mesh = CreateCubeMesh();
                rangeDisplay.transform.localScale = new Vector3(boxCol.size.x, 0.1f, boxCol.size.z);
            }
            else if (col is CapsuleCollider capsuleCol)
            {
                meshFilter.mesh = CreateCylinderMesh();
                float diameter = capsuleCol.radius * 2f;
                rangeDisplay.transform.localScale = new Vector3(diameter, 0.1f, diameter); // Thin cylinder
            }
            else
            {
                // For unsupported collider types, create a simple cube
                meshFilter.mesh = CreateCubeMesh();
                rangeDisplay.transform.localScale = Vector3.one;
            }

            // Set up a basic material (you may want to customize this)
            Material rangeMaterial = new Material(Shader.Find("Standard"));
            rangeMaterial.color = new Color(1f, 1f, 1f, 0.3f); // Semi-transparent white
            rangeMaterial.SetFloat("_Mode", 2); // Set to Fade mode for transparency
            rangeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            rangeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            rangeMaterial.SetInt("_ZWrite", 0);
            rangeMaterial.DisableKeyword("_ALPHATEST_ON");
            rangeMaterial.EnableKeyword("_ALPHABLEND_ON");
            rangeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            rangeMaterial.renderQueue = 3000;

            meshRenderer.material = rangeMaterial;
            meshRenderer.enabled = false; // Start hidden

            rangeDisplays[i] = rangeDisplay;
        }
    }

    // Helper method to get collider center
    private Vector3 GetColliderCenter(Collider col)
    {
        if (col is SphereCollider sphereCol)
            return sphereCol.center;
        else if (col is BoxCollider boxCol)
            return boxCol.center;
        else if (col is CapsuleCollider capsuleCol)
            return capsuleCol.center;
        else
            return Vector3.zero; // Default for unsupported collider types
    }

    // Helper method to create a sphere mesh
    private Mesh CreateSphereMesh()
    {
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sphereMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempSphere);
        return sphereMesh;
    }

    // Helper method to create a cube mesh
    private Mesh CreateCubeMesh()
    {
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempCube);
        return cubeMesh;
    }

    // Helper method to create a cylinder mesh
    private Mesh CreateCylinderMesh()
    {
        GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Mesh cylinderMesh = tempCylinder.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempCylinder);
        return cylinderMesh;
    }

    // Helper method to create a capsule mesh
    private Mesh CreateCapsuleMesh()
    {
        GameObject tempCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Mesh capsuleMesh = tempCapsule.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempCapsule);
        return capsuleMesh;
    }

    private void ShowRange(bool show)
    {
        if (_showRange != show)
        {
            _showRange = show;
            for (int i = 0; i < rangeDisplays.Length; i++)
            {
                rangeDisplays[i].GetComponent<Renderer>().enabled = show;
            }
        }
    }

    private void ShowSpotlight(bool show)
    {
        spotLight.GetComponent<Light>().enabled = show;
        lightOff = false;
    }

    public void OnHover()
    {
        ShowSpotlight(true);
    }

    public void OnClick()
    {
        InterfaceManager.instance.selectedEntity = target;
        if (target != null)
        {
            InterfaceManager.instance.DisplayUpgradeTree(target);
        }
    }

}
