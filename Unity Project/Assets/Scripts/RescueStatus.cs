using UnityEngine;
using UnityEngine.UI;

public class RescueStatus : MonoBehaviour
{
    [SerializeField] private Image _bar;
    public Button GasBarButton;
    public Button AbandonButton;

    public void SetGas(float value)
    {
        if(_bar != null) _bar.fillAmount = value;
    }
}