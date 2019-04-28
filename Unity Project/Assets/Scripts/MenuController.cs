using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance = null;

    [SerializeField] private Animator animator;
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private float roadOffset;
    [SerializeField] private List<GameObject> roads;
    [SerializeField] private Transform RoadParent;
    public Vector3 SpawnPosition;
    public Vector3 SpawnRotation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(OpenMenu());
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    IEnumerator OpenMenu()
    {
        yield return new WaitForSeconds(2);
        animator.SetTrigger("Open");
    }

    public void GoToCredits()
    {
        animator.SetTrigger("Credits");
    }

    public void BackMenu()
    {
        animator.SetTrigger("Menu");
    }

    public void SpawnRoad()
    {
        var road = roadPrefabs[Random.Range(0, roadPrefabs.Count)];
        var roadObject = Instantiate(road, SpawnPosition, Quaternion.Euler(SpawnRotation));
        roadObject.transform.SetParent(RoadParent);
        roadObject.transform.position = SpawnPosition;
        roadObject.transform.rotation = Quaternion.Euler(SpawnRotation);
        roads.Add(roadObject);
    }
    public void DestroyRoad(MenuRoads road)
    {
        roads.Remove(road.gameObject);
        Destroy(road.gameObject);
    }
}
