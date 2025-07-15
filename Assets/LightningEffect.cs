using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    [Header("Lightning Settings")]
    public Transform startObject;
    public Transform endObject;

    [Header("Randomization")]
    [Range(4, 12)]
    public int minVertices = 5;
    [Range(4, 12)]
    public int maxVertices = 8;

    [Range(0.1f, 2f)]
    public float maxDeviation = 0.5f; // Maximum distance vertices can deviate from the straight line

    [Range(0.01f, 0.5f)]
    public float updateInterval = 0.05f; // How often to regenerate (in seconds)

    private LineRenderer lineRenderer;
    private float lastUpdateTime;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lastUpdateTime = Time.time;
    }

    private void Update()
    {
        // Only update if objects are in different positions
        if (startObject.position == endObject.position)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        // Regenerate vertices at specified interval
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            GenerateLightningPath();
            lastUpdateTime = Time.time;
        }
    }

    private void GenerateLightningPath()
    {
        Vector3 start = startObject.position;
        Vector3 end = endObject.position;

        // Calculate distance to determine number of intermediate vertices
        float distance = Vector3.Distance(start, end);
        int intermediateVertices = Random.Range(minVertices, maxVertices + 1);

        // Adjust vertex count based on distance (optional - you can remove this if you want fixed count)
        intermediateVertices = Mathf.Max(3, Mathf.RoundToInt(intermediateVertices * (distance / 10f)));

        // Total vertices = start + intermediates + end
        int totalVertices = intermediateVertices + 2;
        lineRenderer.positionCount = totalVertices;

        // Set start position
        lineRenderer.SetPosition(0, start);

        // Generate intermediate vertices
        Vector3 direction = end - start;
        Vector3 perpendicular1 = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 perpendicular2 = Vector3.Cross(direction, perpendicular1).normalized;

        for (int i = 1; i < totalVertices - 1; i++)
        {
            // Calculate progress along the line (0 to 1)
            float t = (float)i / (totalVertices - 1);

            // Base position along the straight line
            Vector3 basePosition = Vector3.Lerp(start, end, t);

            // Add random deviation
            float randomX = Random.Range(-maxDeviation, maxDeviation);
            float randomY = Random.Range(-maxDeviation, maxDeviation);

            Vector3 deviation = perpendicular1 * randomX + perpendicular2 * randomY;
            Vector3 finalPosition = basePosition + deviation;

            lineRenderer.SetPosition(i, finalPosition);
        }

        // Set end position
        lineRenderer.SetPosition(totalVertices - 1, end);
    }
}