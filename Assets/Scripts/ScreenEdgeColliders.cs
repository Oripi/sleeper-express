using UnityEngine;
using UnityEngine.Serialization;

public class ScreenEdgeColliders : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private EdgeCollider2D edge;
    
    private Vector2[] _edgePoints;

    void Awake()
    {
        if (Camera.main == null) Debug.LogError("Camera.main not found, failed to create edge colliders");
        else mainCamera = Camera.main;

        if (!mainCamera.orthographic) Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders");
        
        _edgePoints = new Vector2[5];

        AddCollider();
    }

    //Use this if you're okay with using the global fields and code in Awake() (more efficient)
    //You can just ignore/delete StandaloneAddCollider() if thats the case
    void AddCollider()
    {
        //Vector2's for the corners of the screen
        Vector2 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector2 topRight = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, mainCamera.nearClipPlane));
        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);

        //Update Vector2 array for edge collider
        _edgePoints[0] = bottomLeft;
        _edgePoints[1] = topLeft;
        _edgePoints[2] = topRight;
        _edgePoints[3] = bottomRight;
        _edgePoints[4] = bottomLeft;

        edge.points = _edgePoints;
    }

    //Use this if you want a single function to handle everything (less efficient)
    //You can just ignore/delete the rest of this class if thats the case
    void StandaloneAddCollider()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Camera.main not found, failed to create edge colliders");
            return;
        }

        var cam = Camera.main;
        if (!cam.orthographic)
        {
            Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders");
            return;
        }

        Vector2 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);

        // add or use existing EdgeCollider2D
        var edge = GetComponent<EdgeCollider2D>() == null
            ? gameObject.AddComponent<EdgeCollider2D>()
            : GetComponent<EdgeCollider2D>();

        var edgePoints = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };
        edge.points = edgePoints;
    }
}