using UnityEngine;

public class SmoothCurveCreator : MonoBehaviour
{
    public Transform[] nodes; // Array of game objects representing the nodes
    public int resolution = 10; // Resolution of the curve
    public int numberOfNodes = 12;
    public float trackWidth = 30f;
    public float radius = 100f;

    public LineRenderer lineRenderer; // LineRenderer component to draw the curve
    public LineRenderer innerTrackRenderer; // track borders
    public LineRenderer outerTrackRenderer;
    public Sprite defaultsprite;
    public Dragging scriptToAttach;

    private void Start()
    {
        SetLineRenderer(lineRenderer);
        SetLineRenderer(innerTrackRenderer);
        SetLineRenderer(outerTrackRenderer);

        SetNodes();
        ArrangeNodes();
        CreateSmoothCurve();
    }

    public void SetNodes()
    {
        nodes = new Transform[numberOfNodes];

        // Loop through from 1 to the number of nodes
        for (int i = 1; i <= numberOfNodes; i++)
        {
            // Find the GameObject with the corresponding name
            GameObject nodeObject = GameObject.Find(i.ToString());

            // Check if the GameObject exists
            if (nodeObject != null)
            {
                // Get the Transform component and assign it to the nodes array
                nodes[i - 1] = nodeObject.transform;
            }
            else
            {
                GameObject gameObject = new GameObject();
                gameObject.name = i.ToString();
                gameObject.AddComponent<SpriteRenderer>();
                gameObject.GetComponent<SpriteRenderer>().sprite = defaultsprite;
                gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                Color currentColor = gameObject.GetComponent<SpriteRenderer>().color;
                currentColor.a = 1/3f;
                gameObject.GetComponent<SpriteRenderer>().color = currentColor;
                gameObject.AddComponent<Dragging>();
                gameObject.AddComponent<CircleCollider2D>();
                nodes[i - 1] = gameObject.transform;
            }
        }
    }

    public void ArrangeNodes()
    {
        float angleIncrement = 360f / numberOfNodes;

        for (int i = 0; i < numberOfNodes; i++)
        {
            float angle = i * angleIncrement;
            float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float y = radius * Mathf.Sin(Mathf.Deg2Rad * angle);

            // Create a GameObject if it doesn't exist
            GameObject node = GameObject.Find((i + 1).ToString());

            // Set the position of the GameObject
            node.transform.position = new Vector3(x, y, 0f);
        }
    }

    public void AddNode()
    {
        numberOfNodes += 1;
        GameObject gameObject = new GameObject();
        gameObject.name = numberOfNodes.ToString();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sprite = defaultsprite;
        gameObject.transform.position = new Vector3(0, 0, 0);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        Color currentColor = gameObject.GetComponent<SpriteRenderer>().color;
        currentColor.a = 1/3f;
        gameObject.GetComponent<SpriteRenderer>().color = currentColor;
        gameObject.AddComponent<Dragging>();
        gameObject.AddComponent<CircleCollider2D>();
    }

    public void DeleteNode()
    {
        numberOfNodes -= 1;

        nodes = RemoveNodeIndex(nodes);
        
        Destroy(GameObject.Find((numberOfNodes + 1).ToString()));
    }

    public Transform[] RemoveNodeIndex(Transform[] currentNodes)
    {
        Transform[] newNodes = new Transform[currentNodes.Length - 1];
        for (int i = 0; i < newNodes.Length; i++)
        {
            newNodes[i] = currentNodes[i];
        }
        return newNodes;
    }

    public void SetLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    public void Update()
    {
        CreateSmoothCurve();
        SetNodes();
    }

    public void CreateSmoothCurve()
    {
        int nodeCount = nodes.Length;

        // Calculate the total number of points required for the curve
        int pointCount = resolution * nodeCount;

        // Create an array to store the positions of the curve points
        Vector3[] curvePoints = new Vector3[pointCount];

        // Calculate the curve points using cubic interpolation
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)resolution;
            int nodeIndex1 = Mathf.FloorToInt(t) % nodeCount;
            int nodeIndex2 = (nodeIndex1 + 1) % nodeCount;
            int nodeIndex3 = (nodeIndex1 + 2) % nodeCount;
            int nodeIndex4 = (nodeIndex1 + 3) % nodeCount;

            Vector3 p0 = nodes[nodeIndex1].position;
            Vector3 p1 = nodes[nodeIndex2].position;
            Vector3 p2 = nodes[nodeIndex3].position;
            Vector3 p3 = nodes[nodeIndex4].position;

            curvePoints[i] = CubicInterpolation(p0, p1, p2, p3, t - Mathf.Floor(t));
        }

        // Set the positions of the LineRenderer
        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(curvePoints);
        lineRenderer.loop = true;
        TrackBorder(lineRenderer, trackWidth);
    }

    // track border layout
    private void TrackBorder(LineRenderer lineRenderer, float trackWidth)
    {
        int pointCount = lineRenderer.positionCount;
        Vector3[] curvePoints = new Vector3[pointCount];
        lineRenderer.GetPositions(curvePoints);

        // Create two new arrays to store the points of the inner and outer track borders
        Vector3[] innerTrackPoints = new Vector3[pointCount];
        Vector3[] outerTrackPoints = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            // Calculate the tangent vector at the current point
            Vector3 tangent;
            if (i > 0 && i < pointCount - 1)
            {
                // For inner points, calculate the tangent using neighboring points
                Vector3 p1 = curvePoints[i - 1];
                Vector3 p2 = curvePoints[i + 1];
                tangent = (p2 - p1).normalized;
            }
            else
            {
                // For start and end points, use the next or previous point to calculate the tangent
                int validIndex1;
                int validIndex2;

                if (i == 0)
                {
                    validIndex1 = pointCount - 1;
                    validIndex2 = 1;
                }
                else
                {
                    validIndex1 = pointCount - 2;
                    validIndex2 = 0;
                }

                tangent = curvePoints[validIndex2] - curvePoints[validIndex1];
            }

            // Calculate the normal vector (rotate the tangent vector 90 degrees clockwise)
            Vector3 normal = new Vector3(tangent.y, -tangent.x, 0f).normalized;

            // Calculate the two points on either side of the existing point
            Vector3 innerPoint = curvePoints[i] + normal * (trackWidth / 2f);
            Vector3 outerPoint = curvePoints[i] - normal * (trackWidth / 2f);

            // Store the points in the respective arrays
            innerTrackPoints[i] = innerPoint;
            outerTrackPoints[i] = outerPoint;
        }

        // Create two new LineRenderers for the inner and outer track borders
        innerTrackRenderer = UpdateLineRenderer(innerTrackRenderer, innerTrackPoints);
        outerTrackRenderer = UpdateLineRenderer(outerTrackRenderer, outerTrackPoints);

        // Optionally, connect the first and last points to close the track
        innerTrackRenderer.loop = true;
        outerTrackRenderer.loop = true;
    }

    private LineRenderer UpdateLineRenderer(LineRenderer trackRenderer, Vector3[] points)
    {
        trackRenderer.positionCount = points.Length;
        trackRenderer.SetPositions(points);
        return trackRenderer;
    }

    // Performs cubic interpolation between the given control points
    private Vector3 CubicInterpolation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float tSquared = t * t;
        float tCubed = tSquared * t;

        Vector3 interpolatedPosition = 0.5f * (
            (-p0 + 3f * p1 - 3f * p2 + p3) * tCubed +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * tSquared +
            (-p0 + p2) * t +
            (2f * p1));

        return interpolatedPosition;
    }
}
