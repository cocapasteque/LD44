using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private float roadOffset;
    [SerializeField] private List<GameObject> roads;
    [SerializeField] private Transform RoadParent;
    [SerializeField] private GameTimer Timer;

    public Player Player;

    public bool Started = false;
    public Vector3 LimitPosition;
    public Vector3 SpawnPosition;
    public Vector3 PlayerSpawnPosition;

    public float Speed;
    public float TimeLimit;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
    private void Update()
    {
        if (Started)
        {
            if (Player.Gas == 0 && Timer.Timer > 0)
            {
                GameOver();
            }
            else if (Player.Gas > 0 && Timer.Timer == 0)
            {
                WinRound();
            }
        }
    }

    public void WinRound()
    {
        Started = false;
        Debug.Log("You won the round");
    }
    public void GameOver()
    {
        Started = false;
        Debug.Log("Game Over");
    }

    public void StartGame()
    {
        Player = Instantiate(playerPrefab, PlayerSpawnPosition, Quaternion.identity).GetComponent<Player>();

        Timer.Reset();
        Started = true;
    }
    public void SpawnRoad()
    {
        var road = roadPrefabs[Random.Range(0, roadPrefabs.Count)];
        var roadObject = Instantiate(road, SpawnPosition, Quaternion.identity);
        roadObject.transform.SetParent(RoadParent);

        roads.Add(roadObject);
    }
    public void DestroyRoad(Road road)
    {
        roads.Remove(road.gameObject);
        Destroy(road.gameObject);
    }
}
