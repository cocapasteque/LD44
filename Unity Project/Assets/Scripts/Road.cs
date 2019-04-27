using UnityEngine;
using UnityEngine.Events;

public class Road : MonoBehaviour
{
    public bool IsInit = false;
    private GameObject roadObject;

    private void Update()
    {
        Move(GameManager.Instance.Speed);
    }

    public void Move(float speed)
    {
        transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "deathZone")
        {
            GameManager.Instance.DestroyRoad(this);
        }
        else if (other.tag == "roadSpawn")
        {
            GameManager.Instance.SpawnRoad();
        }
    }
}

