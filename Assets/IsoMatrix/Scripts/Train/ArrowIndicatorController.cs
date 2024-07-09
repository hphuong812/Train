using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArrowIndicatorController : MonoBehaviour
{
    public UnityEvent TimeDelayed;
    private bool started = false;
    
    public void PlayAnimation(float time)
    {
        if (started)
        {
            return;
        }
        StartCoroutine(DelayTime(time));
        started = true;
    }
    
    public IEnumerator DelayTime(float time)
    {
        yield return new WaitForSeconds(time);
        TimeDelayed?.Invoke();
    }
}
