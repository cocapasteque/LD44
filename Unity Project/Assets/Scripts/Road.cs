using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Road : MonoBehaviour
{
    [SerializeField] private List<Transform> rescuePositions;

    public bool IsInit = false;
    private GameObject roadObject;

    private void Update()
    {
        if (!GameManager.Instance.Started) return;

        Move(GameManager.Instance.Speed);
    }

    // Move the road to get the sidescrolling effect.
    public void Move(float speed)
    {
        transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, 0, 0);
    }
    // Spawn a rescue unit on one of the rescue position on this road block.
    public void SpawnRescue(GameObject prefab)
    {
        var pos = rescuePositions[Random.Range(0, rescuePositions.Count)];
        GameObject rescue = Instantiate(prefab, pos);
    }

    // Handling creation and destruction of the roads.
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

