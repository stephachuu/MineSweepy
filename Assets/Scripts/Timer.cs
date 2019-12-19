using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText = null;
    
    private float _timeCounter = 0f;
    private bool _timerActive = false;

    private void Start()
    {
        timerText.text = String.Empty;
    }

    private void Update()
    {
        if (_timerActive)
        {
            _timeCounter += Time.deltaTime;
            timerText.text = _timeCounter.ToString("F2");
        }
    }

    public void StartTimer()
    {
        _timeCounter = 0f;
        _timerActive = true;
    }

    public void StopTimer()
    {
        _timerActive = false;
    }
}
