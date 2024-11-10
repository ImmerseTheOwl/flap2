using UnityEngine;

public class roomsizeToCoord : MonoBehaviour
{
    // Make this static so other scripts can access the bounds
    public static Bounds roomBounds;
    public Bounds editorRoomBounds;

    void Awake()
    {
        roomBounds = editorRoomBounds;
    }

    // Optional: Draw the bounds in the Scene view for visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(editorRoomBounds.center, editorRoomBounds.size);
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class roomsizeToCoord : MonoBehaviour
// {
//     // Use a public field to set the room's bounds
//     public Bounds roomBounds;

//     // Reference to the red cube prefab or use a primitive cube
//     public GameObject cubePrefab;

//     // Start is called before the first frame update
//     void Start()
//     {
//         CreateRedCube(GetRandomDestination());
//     }

//     /// <summary>
//     /// Generates a random destination within the user-defined room bounds.
//     /// </summary>
//     /// <returns>A random Vector3 position within the bounds.</returns>
//     private Vector3 GetRandomDestination()
//     {
//         float randomX = Random.Range(roomBounds.min.x, roomBounds.max.x);
//         float randomY = Random.Range(roomBounds.min.y, roomBounds.max.y);
//         float randomZ = Random.Range(roomBounds.min.z, roomBounds.max.z);

//         return new Vector3(randomX, randomY, randomZ);
//     }

//     /// <summary>
//     /// Creates a red cube at the specified position.
//     /// </summary>
//     /// <param name="position">The position to place the red cube.</param>
//     private void CreateRedCube(Vector3 position)
//     {
//         GameObject redCube;

//         if (cubePrefab != null)
//         {
//             // Use the provided prefab
//             redCube = Instantiate(cubePrefab, position, Quaternion.identity);
//         }
//         else
//         {
//             // Create a primitive cube if no prefab is provided
//             redCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//             redCube.transform.position = position;
//         }

//         // Set the cube's color to red
//         Renderer cubeRenderer = redCube.GetComponent<Renderer>();
//         if (cubeRenderer != null)
//         {
//             cubeRenderer.material.color = Color.red;
//         }
//     }

//     // Optional: Draw the bounds in the Scene view for visualization
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.cyan;
//         Gizmos.DrawWireCube(roomBounds.center, roomBounds.size);
//     }
// }