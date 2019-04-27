using System;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float Timer;

    private Text _text;
    private TimeSpan _ts;

    private void Start()
    {
        _text = GetComponent<Text>();
        _text.text = "";
    }

    private void Update()
    {
        if (!GameManager.Instance.Started) return;
        Timer -= Time.deltaTime;
        _ts = TimeSpan.FromSeconds(Timer);

        _text.text = $"{_ts.Minutes : 00} : {_ts.Seconds : 00}";
    }

    public void Reset()
    {
        Timer = GameManager.Instance.TimeLimit;
    }
}

