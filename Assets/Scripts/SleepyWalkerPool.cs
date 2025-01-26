using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class SleepyWalkerPool : MonoBehaviour
{
    [SerializeField] private GameObject sleepyWalkerPrefab;
    [SerializeField] private UnityEvent<SleepyWalker> onSpawn;
    [SerializeField] private UnityEvent<SleepyWalker> onRelease;
    
    private IObjectPool<SleepyWalker> _sleepyWalkersPool;

    private void Awake()
    {
        _sleepyWalkersPool = new ObjectPool<SleepyWalker>(CreateSleepWalker, OnSleepWalkerGet, OnSleepWalkerRelease,
            null, true, 20, 100);
    }

    private void OnSleepWalkerRelease(SleepyWalker obj)
    {
        onRelease.Invoke(obj);
    }

    private void OnSleepWalkerGet(SleepyWalker obj)
    {
        obj.SetPool(this);
        onSpawn.Invoke(obj);
    }

    private SleepyWalker CreateSleepWalker()
    {
        var newSleepyWalker = Instantiate(sleepyWalkerPrefab);
        newSleepyWalker.SetActive(false);
        return newSleepyWalker.GetComponent<SleepyWalker>();
    }

    public SleepyWalker GetSleepWalker()
    {
        return _sleepyWalkersPool.Get();
    }

    public void ReleaseSleepWalker(SleepyWalker sleepyWalker)
    {
        _sleepyWalkersPool.Release(sleepyWalker);
    }
}
