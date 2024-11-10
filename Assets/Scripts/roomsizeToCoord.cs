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