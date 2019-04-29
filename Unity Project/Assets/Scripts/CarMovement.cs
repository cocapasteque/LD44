using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Transform CarObject;
    public float RollSpeed = 5f;
    public float PitchSpeed = 5f;
    public Vector3 baseRotation;
    public List<GameObject> CarModels;

    private void Start()
    {
        baseRotation = CarObject.transform.rotation.eulerAngles;
    }

    private void Update()
    {
        var roll = Mathf.Sin(Time.time * RollSpeed);
        var pitch = Mathf.Sin(Time.time * PitchSpeed);
        var y = CarObject.transform.rotation.eulerAngles.y;
        CarObject.rotation = Quaternion.Euler(baseRotation.x + pitch, baseRotation.y, baseRotation.z + roll);
    }

    public void SetNewModel()
    {
        foreach (GameObject car in CarModels)
        {
            car.SetActive(false);
        }
        if (GameManager.Instance.CurrentLevel + 1 >= CarModels.Count)
        {
            CarModels[CarModels.Count - 1].SetActive(true);
        }
        else
        {
            CarModels[GameManager.Instance.CurrentLevel].SetActive(true);
        }
        if (GameManager.Instance.CurrentLevel < CarModels.Count)
        {
            CarObject = CarModels[GameManager.Instance.CurrentLevel].transform;
        }
        else
        {
            CarObject = CarModels[CarModels.Count - 1].transform;
        }
        
        baseRotation = CarObject.transform.rotation.eulerAngles;
    }
}

