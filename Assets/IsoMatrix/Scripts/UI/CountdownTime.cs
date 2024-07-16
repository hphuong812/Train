using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownTime : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI countdownText;
    [SerializeField]
    private float countdownTime = 5f;
    public UnityEvent CountdownTimeCompleted;

    private void Start()
    {
        StartCountDown();
    }

    public void StartCountDown()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        float currentTime = countdownTime;

        while (currentTime > 0)
        {
            countdownText.text = currentTime.ToString("F0");
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        CountdownTimeCompleted?.Invoke();
    }
}
