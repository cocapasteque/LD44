using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    public List<Stat> Stats;   
    public List<int> UpgradeKarmaKosts;
    public Text AvailableKarma;
    public Transform CarPedestal;

    private Dictionary<int, int> decreasableStats;
    private GameObject currentCar;

    private void OnEnable()
    {
        SetCar();
        decreasableStats = new Dictionary<int, int>();
        for (int i = 0; i < Stats.Count; i++)
        {
            UpdateStat(i);
            Stats[i].DecreaseButton.interactable = false;        
        }
        UpdateKarma();
    }

    private void UpdateStat(int index)
    {
        Stats[index].FillStatus.fillAmount = GameManager.Instance.StatValues[index] * 0.2f;
        if (GameManager.Instance.StatValues[index] == 5)
        {
            Stats[index].IncreaseButton.interactable = false;
            Stats[index].Cost.text = "Fully upgraded";
        }
        else
        {
            Stats[index].Cost.text = string.Format("Cost: {0} Karma", UpgradeKarmaKosts[GameManager.Instance.StatValues[index]]);
        }

        for (int i = 0; i < Stats.Count; i++)
        {
            if (GameManager.Instance.StatValues[i] == 5 || GameManager.Instance.Karma < UpgradeKarmaKosts[GameManager.Instance.StatValues[i]])
            {
                Stats[i].IncreaseButton.interactable = false;
            }
            else
            {
                Stats[i].IncreaseButton.interactable = true;
            }
        }  
    }

    public void IncreaseStat(int index)
    {
        GameManager.Instance.Karma -= UpgradeKarmaKosts[GameManager.Instance.StatValues[index]];
        GameManager.Instance.StatValues[index]++;
        UpdateStat(index);
        UpdateKarma();
        if (decreasableStats.ContainsKey(index))
        {
            decreasableStats[index]++;
        }
        else
        {
            decreasableStats.Add(index, 1);
        }
        Stats[index].DecreaseButton.interactable = true;
    }

    public void DecreaseStat(int index)
    {
        GameManager.Instance.Karma += UpgradeKarmaKosts[GameManager.Instance.StatValues[index] - 1];
        GameManager.Instance.StatValues[index]--;
        UpdateStat(index);
        UpdateKarma();
        decreasableStats[index]--;
        if (decreasableStats[index] <= 0)
        {
            decreasableStats.Remove(index);
            Stats[index].DecreaseButton.interactable = false;
        }
    }

    private void UpdateKarma()
    {
        AvailableKarma.text = string.Format("Available Karma: {0}", GameManager.Instance.Karma);
    }

    private void SetCar()
    {
        if (currentCar != null)
        {
            Destroy(currentCar);
        }
        GameObject Car = GameManager.Instance.Player.GetComponent<CarMovement>().CarObject.gameObject;
        currentCar = GameObject.Instantiate(Car);
        currentCar.transform.parent = CarPedestal;
        currentCar.transform.localPosition = Vector3.zero;
        currentCar.transform.localRotation = Car.transform.localRotation;
        currentCar.transform.localScale = Car.transform.localScale;
    }
}