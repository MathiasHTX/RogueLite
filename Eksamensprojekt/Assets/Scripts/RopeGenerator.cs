using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public int numPoints = 10;
    public float ropeLength = 1f; // Length of each segment of the rope
    public GameObject ropeSegmentPrefab; // Prefab for the rope segment
    public Rigidbody[] ropePoints; // Array to store Rigidbody points
    public Joint[] ropeJoints; // Array to store joints connecting the Rigidbody points
    public Material ropeMaterial; // Material for the rope mesh

    private Mesh ropeMesh; // Mesh for the rope

    void Start()
    {
        GenerateRope();
    }

    void GenerateRope()
    {
        ropePoints = new Rigidbody[numPoints];
        ropeJoints = new Joint[numPoints - 1];

        // Instantiate rope segments at the positions of the empty GameObjects
        for (int i = 0; i < numPoints; i++)
        {
            Vector3 position = transform.position + Vector3.up * i * ropeLength; // Adjust position based on rope length
            GameObject segment = Instantiate(ropeSegmentPrefab, position, Quaternion.identity);
            ropePoints[i] = segment.GetComponent<Rigidbody>();
        }

        // Connect the rope segments with joints
        for (int i = 0; i < numPoints - 1; i++)
        {
            ConfigurableJoint joint = ropePoints[i].gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = ropePoints[i + 1];
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = -Vector3.up * ropeLength;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            ropeJoints[i] = joint;
        }

        // Create rope mesh
        CreateRopeMesh();
    }

    void CreateRopeMesh()
    {
        // Create new mesh
        ropeMesh = new Mesh();

        // Lists to store vertices and triangles
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();

        // Generate vertices and triangles for each segment
        for (int i = 0; i < numPoints - 1; i++)
        {
            // Get vertices of current and next rope segments
            Vector3[] segmentVertices1 = GetEdgeVertices(ropePoints[i]);
            Vector3[] segmentVertices2 = GetEdgeVertices(ropePoints[i + 1]);

            // Add vertices of the current segment
            verticesList.Add(transform.InverseTransformPoint(segmentVertices1[1])); // Top right vertex of current segment
            verticesList.Add(transform.InverseTransformPoint(segmentVertices1[2])); // Bottom right vertex of current segment
            verticesList.Add(transform.InverseTransformPoint(segmentVertices2[0])); // Bottom left vertex of next segment
            verticesList.Add(transform.InverseTransformPoint(segmentVertices2[3])); // Top left vertex of next segment

            // Calculate the index of the first vertex of the current segment
            int vertexIndex = i * 4;

            // Add triangles of the current segment
            trianglesList.Add(vertexIndex);         // Top right
            trianglesList.Add(vertexIndex + 1);     // Bottom right
            trianglesList.Add(vertexIndex + 2);     // Bottom left
            trianglesList.Add(vertexIndex);         // Top right
            trianglesList.Add(vertexIndex + 2);     // Bottom left
            trianglesList.Add(vertexIndex + 3);     // Top left
        }

        // Convert lists to arrays
        Vector3[] vertices = verticesList.ToArray();
        int[] triangles = trianglesList.ToArray();

        // Assign vertices and triangles to the mesh
        ropeMesh.vertices = vertices;
        ropeMesh.triangles = triangles;

        // Recalculate normals for proper shading
        ropeMesh.RecalculateNormals();

        // Assign mesh to MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = ropeMesh;

        // Assign material to the mesh
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = ropeMaterial;
    }

    public Vector3[] GetEdgeVertices(Rigidbody segmentRigidbody)
        {
            GameObject segment = segmentRigidbody.gameObject;
            Mesh segmentMesh = segment.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = segmentMesh.vertices;
            Vector3[] edgeVertices = new Vector3[4];

            // Get the transform of the segment
            Transform segmentTransform = segment.transform;

            // Get the local bounds of the segment mesh
            Bounds bounds = segmentMesh.bounds;

            // Calculate edge vertices in local space
            edgeVertices[0] = segmentTransform.TransformPoint(bounds.center + bounds.extents); // Top right
            edgeVertices[1] = segmentTransform.TransformPoint(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z)); // Bottom right
            edgeVertices[2] = segmentTransform.TransformPoint(bounds.center - bounds.extents); // Bottom left
            edgeVertices[3] = segmentTransform.TransformPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z)); // Top left

            return edgeVertices;
        }
}