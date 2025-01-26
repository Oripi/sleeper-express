using UnityEngine;

public class InitialGameSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timeToReachFirstSleepyWalker = 4;
    [SerializeField] private float timeToSpawnFirstSleepyWalker = 3;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SleepyWalkerManager sleepyWalkerManager;
    [SerializeField] private SleepyWalker firstActiveSleepyWalker;
    
    private SleepyWalker _sleepyWalker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(SpawnFirstWalker), timeToSpawnFirstSleepyWalker);
    }
    
    private void SpawnFirstWalker()
    {
        _sleepyWalker = sleepyWalkerManager.SpawnSleepyWalker();
        if (!_sleepyWalker) return;

        _sleepyWalker.TimeToUpdateVelocity = 10;
        _sleepyWalker.CurrentVelocity = (firstActiveSleepyWalker.transform.position - _sleepyWalker.transform.position) / timeToReachFirstSleepyWalker;
        _sleepyWalker.ShouldGetAngry = false;
        Invoke(nameof(OnReachedFirstWalker), timeToReachFirstSleepyWalker);
    }
    
    private void OnReachedFirstWalker()
    {
        _sleepyWalker.GetAngry();
    }
}
