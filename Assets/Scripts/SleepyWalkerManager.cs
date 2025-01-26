using UnityEngine;

public class SleepyWalkerManager : MonoBehaviour
{
    private const float SpawnOffsetFromScreenEdge = 1;
    
    [Header("References")]
    [SerializeField] private SleepyWalkerPool pool;
    [SerializeField] private Camera mainCamera;

    [Header("Configs")] 
    [SerializeField] private float timeToStartSpawning = 10;
    [SerializeField] private float spawnInterval = 20;
    [SerializeField] private int maxSpawnedWalkersAllowed = 20;

    private float _numSpawnedWalkers;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnSleepyWalker), timeToStartSpawning, spawnInterval);
    }
    
    public SleepyWalker SpawnSleepyWalker()
    {
        if (_numSpawnedWalkers >= maxSpawnedWalkersAllowed) return null;
        
        Vector2 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector2 topRight = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, mainCamera.nearClipPlane));
        
        var sleepyWalker = pool.GetSleepWalker();
        sleepyWalker.transform.position =
            new Vector3(Random.Range(bottomLeft.x + SpawnOffsetFromScreenEdge, topRight.x - SpawnOffsetFromScreenEdge),
                Random.Range(bottomLeft.y + SpawnOffsetFromScreenEdge, topRight.y - SpawnOffsetFromScreenEdge));
        sleepyWalker.ActivateWalker();
        sleepyWalker.PlaySpawnAnimation();
        return sleepyWalker;
    }

    public void OnWalkerSpawn()
    {
        _numSpawnedWalkers++;
    }

    public void OnWalkerRelease()
    {
        _numSpawnedWalkers--;
    }
}
