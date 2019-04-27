using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private float roadOffset;
    [SerializeField] private int roadPoolSize;

    public Vector3 LimitPosition;
    public Vector3 SpawnPosition;
    public float Speed;

    private Pool<Road> _roads;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        _roads = new Pool<Road>(roadPoolSize, "roads");
        InitRoadPool();
    }

    private void InitRoadPool()
    {
        Road road;
        while (!(road = _roads.Next()).IsInit)
        {
            var prefab = roadPrefabs[Random.Range(0, roadPrefabs.Count)];
            road.Init(prefab);
        }

        // Init Roads
        for (int i = 0; i < roadPoolSize; i++)
        {
            road = _roads.Next();
            road.transform.position = new Vector3(i * roadOffset, 0, 0);
        }
    }
}
