using System;
using UnityEngine;

public class InitialSleepyWalker : MonoBehaviour
{
    [SerializeField] private SleepyWalker sleepyWalker;
    [SerializeField] private float timeToWakeUpAfterInitialWakeUp = 5;

    private void Start()
    {
        GameManager.Instance.OnGameStart += OnInstanceOnOnGameStart;
    }

    private void OnInstanceOnOnGameStart()
    {
        sleepyWalker.ActivateWalker();
    }

    public void OnWakeUp()
    {
        sleepyWalker.TimeToWakeUp = timeToWakeUpAfterInitialWakeUp;
        sleepyWalker.NumHitsToWakeUp = 3;
    }
}
