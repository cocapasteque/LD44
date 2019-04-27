using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasBar : MonoBehaviour
{
    private Image _img;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.Started) return;

        _img.fillAmount = GameManager.Instance.Player.Gas;
    }
}
