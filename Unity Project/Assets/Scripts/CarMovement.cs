using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Transform CarObject;
    public float RollSpeed = 5f;
    public float PitchSpeed = 5f;
    private void Update()
    {
        var roll = Mathf.Sin(Time.time * RollSpeed);
        var pitch = Mathf.Sin(Time.time * PitchSpeed);
        var y = CarObject.transform.rotation.eulerAngles.y;
        CarObject.rotation = Quaternion.Euler(pitch, y, roll);
    }
}

