using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasBar : MonoBehaviour
{
    [SerializeField] RectTransform BarContainer;
    private Image _img;
    private Player _player;   
    private float _baseLength;

    // Start is called before the first frame update
    void Start()
    {
        _baseLength = BarContainer.rect.width;
        _img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.Started) return;

        _img.fillAmount = GameManager.Instance.Player.Gas / GameManager.Instance.Player.MaxGas;
    }

    public void UpdateBarLength()
    {
        BarContainer.sizeDelta = new Vector2(_baseLength * GameManager.Instance.Player.MaxGas, BarContainer.sizeDelta.y);
    }
}
