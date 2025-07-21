using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))] // Requries that the object have a MeshFilter, which is necessary for the later functions.
public class MeshGeneratorScript : MonoBehaviour
{
    // Variables
    Mesh mesh;
    Vector3[] vertices; // Array of vertices to describe the mesh
    int[] triangles; // Array of trianbgles, whose points need to be given in the CLOCKWISE direction to face forward
    Color[] colors; // UV mapping array for textures

    float minTerrainHeight;
    float maxTerrainHeight;


    [SerializeField] private int xSize = 20;
    [SerializeField] private int zSize = 20;
    [SerializeField] private bool gizmos = true;
    [SerializeField] Gradient gradient;
    [SerializeField] private float floor = 0.3f;
    [SerializeField] private float ceiling = 1.5f;
    [SerializeField] private float scale = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh(); // Create the mesh
        GetComponent<MeshFilter>().mesh = mesh; // Set the mesh filter to be out mesh object (instead of, say, a cube)
        StartCoroutine(CreateShape()); // Function to cretae shape, implementation below
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMesh();
    }

    IEnumerator CreateShape () // IEnumerator is a "Co-Routine
    {

        vertices = new Vector3[(xSize + 1) * (zSize + 1)]; // Creating new Vector3 Array and defining size. Draw an array with vertices and count them to see why there is a +1

        // Loop over all verticies in the 
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Creates the grid with tall of the vertices
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * Random.Range(floor, ceiling) * scale; // Perlin Noise for random height
                // float y = Random.Range(floor, ceiling);
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }
                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }

                i++; 
            }
        }

        triangles = new int[6 * xSize * zSize];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++) 
        {
            for (int x = 0; x < xSize; x++)
            {

                // Code to make 1 square (2 triangles)
                triangles[0 + tris] = 0 + vert;
                triangles[1 + tris] = xSize + 1 + vert;
                triangles[2 + tris] = 1 + vert;
                triangles[3 + tris] = 1 + vert;
                triangles[4 + tris] = xSize + 1 + vert;
                triangles[5 + tris] = xSize + 2 + vert;
                vert++;
                tris += 6;

                yield return new WaitForSeconds(0.005f);
            }
            vert++;
        }



        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                float height = Mathf.InverseLerp((minTerrainHeight/floor) * scale, (maxTerrainHeight/ceiling) * scale, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

        Debug.Log("Color of vertex 0: " + colors[0]);





        /* CreateAssetMenuAttribute THE MESH MANUALLY
        vertices = new Vector3[]
        {
            new Vector3 (0, 0, 0), // Create a vertex
            new Vector3 (0, 0, 1),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 0, 1)
        };

        triangles = new int[]
        {
            0, 1, 2, // Create first triangle
            1, 3, 2 // Going Clockwise
        };

        */
    }

    void UpdateMesh()
    {
        mesh.Clear(); // Clear the mesh of any old data before adding new data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals(); // Have Unity calculate the mesh normals for us (rather than create them by hand)

    }

    private void OnDrawGizmos()
    {
        if (vertices == null || !gizmos)
        {
            return; // Don't run if no vertices
        }
        
        for (int i=0; i< vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

}
