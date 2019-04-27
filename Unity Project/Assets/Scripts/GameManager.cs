using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject rescuePrefab;

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

    // Win the round, the timer went to 0 and the player still had gas in his tank.
    public void WinRound()
    {
        Started = false;
        Debug.Log("You won the round");
    }
    // Game over, the timer is not done but the player ran out of gas.
    public void GameOver()
    {
        Started = false;
        Debug.Log("Game Over");
    }
    // Start the game
    public void StartGame()
    {
        Player = Instantiate(playerPrefab, PlayerSpawnPosition, Quaternion.identity).GetComponent<Player>();

        Timer.Reset();
        Started = true;
    }
    // Spawn a road and calculate if it should contain a rescue unit.
    public void SpawnRoad()
    {
        var road = roadPrefabs[Random.Range(0, roadPrefabs.Count)];
        var roadObject = Instantiate(road, SpawnPosition, Quaternion.identity);
        roadObject.transform.SetParent(RoadParent);

        // Spawning a rescue unit 20% chance.
        if (Random.Range(0f, 100f) < 20f)
        {
            roadObject.GetComponent<Road>().SpawnRescue(rescuePrefab);
        }

        roads.Add(roadObject);
    }
    // Destroy a road when it's at the end.
    public void DestroyRoad(Road road)
    {
        roads.Remove(road.gameObject);
        Destroy(road.gameObject);
    }
}
