using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject rescuePrefab;
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject EndRoundPanel;

    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private float roadOffset;
    [SerializeField] private List<GameObject> roads;
    [SerializeField] private Transform RoadParent;
    [SerializeField] private GameTimer Timer;
    [SerializeField] private CameraShake CameraShake;
    [SerializeField] private Transform ExplosionTransform;

    [SerializeField] private GameObject GameCamera;
    [SerializeField] private GameObject GameCanvas;
    [SerializeField] private GameObject ShopCamera;
    [SerializeField] private GameObject ShopCanvas;

    public Player Player;

    public bool Started = false;
    public Vector3 LimitPosition;
    public Vector3 SpawnPosition;
    public Transform PlayerSpawn;
    public List<Transform> RescueSpots;
    public RectTransform RescueStatusParent;
    public int KarmaPerCar => 3 + StatValues[2];

    public float Speed;
    public float TimeLimit;

    public Text KarmaText;
    public int Karma;
    public int[] StatValues = new int[5];


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

    public void Explosion()
    {
        ShakeCamera(0.5f);
        var expl = Instantiate(explosionPrefab, ExplosionTransform);
        Destroy(expl, 5f);
    }

    public void ShakeCamera(float duration)
    {
        CameraShake.shakeDuration = duration;
    }

    // Win the round, the timer went to 0 and the player still had gas in his tank.
    public void WinRound()
    {
        Started = false;
        Debug.Log("You won the round");
        EndRoundPanel.SetActive(true);
    }
    // Game over, the timer is not done but the player ran out of gas.
    public void GameOver()
    {
        Started = false;
        Debug.Log("Game Over");
        GameOverPanel.SetActive(true);
    }
    // Open the upgrade menu
    public void GoToShop()
    {
        GameCamera.SetActive(false);
        GameCanvas.SetActive(false);
        ShopCamera.SetActive(true);
        ShopCanvas.SetActive(true);
    }
    // Restart the game
    public void RestartGame()
    {
        Player.Init();
        Timer.Reset();
        Started = true;
    }
    // Start the game
    public void StartGame()
    {
        Player = Instantiate(playerPrefab, PlayerSpawn).GetComponent<Player>();
        Player.RescueSpots = RescueSpots;
        Player.Init();

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

    //Increase/Decrease Karma during game
    public void ChangeKarma(int value)
    {
        Karma += value;
        KarmaText.text = Karma.ToString();
    }
}
