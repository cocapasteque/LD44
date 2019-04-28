using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoads : MonoBehaviour
{
    private void Update()
    {
        Move(5);
    }

    // Move the road to get the sidescrolling effect.
    public void Move(float speed)
    {
        transform.position = new Vector3(0, 0, transform.position.z + speed * Time.deltaTime);
    }

    // Handling creation and destruction of the roads.
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "deathZone")
        {
            MenuController.Instance.DestroyRoad(this);
        }
        else if (other.tag == "roadSpawn")
        {
            MenuController.Instance.SpawnRoad();
        }
    }
}
