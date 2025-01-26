using UnityEngine;
using UnityEngine.Events;

public class SleepyWalkerAnimationEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent onWakeUpFinished;
    
    public void OnWakeUpFinished()
    {
        onWakeUpFinished.Invoke();
    }
}
