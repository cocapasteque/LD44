using UnityEngine;
using UnityEngine.Events;

public class Road : MonoBehaviour
{
    public bool IsInit = false;
    private GameObject roadObject;

    private void Update()
    {
        if (transform.position.x < GameManager.Instance.LimitPosition.x)
        {
            transform.position = GameManager.Instance.SpawnPosition;
        }
        Move(GameManager.Instance.Speed);
    }

    public void Init(GameObject prefab)
    {
        IsInit = true;
        Debug.Log("Init road " + name);

        roadObject = Instantiate(prefab);
        roadObject.transform.SetParent(transform);
    }

    public void Move(float speed)
    {
        transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, 0, 0);
    }
}

