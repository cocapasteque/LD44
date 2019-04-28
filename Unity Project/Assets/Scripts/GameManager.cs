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
    [SerializeField] private GameObject GameOverFuelText;
    [SerializeField] private GameObject GameOverKarmaText;
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
    [SerializeField] private GasBar FuelBar;


    public Player Player;

    public bool Started = false;
    public Vector3 LimitPosition;
    public Vector3 SpawnPosition;
    public Transform PlayerSpawn;
    public List<Transform> RescueSpots;
    public RectTransform RescueStatusParent;
    public int BaseKarmaPerCar;
    [HideInInspector]
    public int KarmaPerCar;

    public float BaseSpawnProbability;
    public float SpawnProbabilityIncreasePerLevel;
    public float BaseSpeed;
    public float SpeedIncreasePerLevel;
    public float Speed;
    public float BaseTimeLimit;
    public float TimeIncreasePerLevel;
    public float TimeLimit;
    public int BaseKarmaForNextLevel;
    public int KarmaIncreasePerLevel;
   
    public Text KarmaText;
    public int Karma;
    public int[] StatValues = new int[5];

    public int CurrentLevel;
    public Text LevelText;

    private float _spawnChanceFailSave;

    public enum LossCondition
    {
        fuel,
        karma
    }

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
                GameOver(LossCondition.fuel);              
            }
            else if (Player.Gas > 0 && Timer.Timer == 0)
            {
                if (Karma >= BaseKarmaForNextLevel + CurrentLevel * KarmaIncreasePerLevel)
                {
                    WinRound();
                }
                else
                {
                    GameOver(LossCondition.karma);
                }
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
        Player.EndOfLevel(true);
        Debug.Log("You won the round");
        EndRoundPanel.SetActive(true);
    }
    // Game over, the timer is not done but the player ran out of gas.
    public void GameOver(LossCondition condition)
    {
        Player.EndOfLevel(false);
        Started = false;
        Debug.Log("Game Over");
        GameOverPanel.SetActive(true);

        switch(condition)
        {
            case LossCondition.fuel:
                GameOverFuelText.SetActive(true);
                GameOverKarmaText.SetActive(false);
                break;
            case LossCondition.karma:
                GameOverFuelText.SetActive(false);
                GameOverKarmaText.SetActive(true);
                break;
        }
    }
    // Open the upgrade menu
    public void GoToShop()
    {
        GameCamera.SetActive(false);
        GameCanvas.SetActive(false);
        ShopCamera.SetActive(true);
        ShopCanvas.SetActive(true);
    }


    public void NextLevel()
    {
        GameCamera.SetActive(true);
        GameCanvas.SetActive(true);
        ShopCamera.SetActive(false);
        ShopCanvas.SetActive(false);
        CurrentLevel++;
        LevelText.text = (CurrentLevel + 1).ToString();
        ChangeKarma(0);
        FuelBar.UpdateBarLength();
        Speed += SpeedIncreasePerLevel;
        TimeLimit += TimeIncreasePerLevel;
        KarmaPerCar = BaseKarmaPerCar + StatValues[2];
        _spawnChanceFailSave = 0f;

        Player.Init();
        Timer.Reset();
        Started = true;
    }

    // Restart the game
    public void RestartGame()
    {
        CurrentLevel = 0;
        LevelText.text = (CurrentLevel + 1).ToString();
        Karma = 0;
        ChangeKarma(0);
        ResetStatValues();       
        FuelBar.UpdateBarLength();
        Speed = BaseSpeed;
        TimeLimit = BaseTimeLimit;
        KarmaPerCar = BaseKarmaPerCar + StatValues[2];
        _spawnChanceFailSave = 0f;

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
        KarmaPerCar = BaseKarmaPerCar + StatValues[2];
        Karma = 0;
        CurrentLevel = 0;
        ChangeKarma(0);
        Started = true;
    }
    public void ResetStatValues()
    {
        for (int i = 0; i < StatValues.Length; i++)
        {
            StatValues[i] = 0;
        }
    }

    // Spawn a road and calculate if it should contain a rescue unit.
    public void SpawnRoad()
    {
        var road = roadPrefabs[Random.Range(0, roadPrefabs.Count)];
        var roadObject = Instantiate(road, SpawnPosition, Quaternion.identity);
        roadObject.transform.SetParent(RoadParent);

        //Don't spawn new cars in the last 5 seconds
        if (Timer.Timer > 5 && Random.Range(0f, 100f) < BaseSpawnProbability + SpawnProbabilityIncreasePerLevel * CurrentLevel + _spawnChanceFailSave)
        {
            _spawnChanceFailSave = 0f;
            roadObject.GetComponent<Road>().SpawnRescue(rescuePrefab);
        }
        else
        {
            _spawnChanceFailSave *= 2f;
            _spawnChanceFailSave += 1f;
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
        KarmaText.text = Karma.ToString() + " / " + (BaseKarmaForNextLevel + CurrentLevel * KarmaIncreasePerLevel);
    }
}
