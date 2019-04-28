using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueUnit : MonoBehaviour
{
    [SerializeField] private GameObject statusBar;

    private RescueStatus _statusBar;
    private BoxCollider _bc;
    public AudioSource Audio;
    public AudioClip OutOfGas;

    public GameObject ExhaustEffect;
    public Transform RescueSpot;
    public float Gas = 1f;
    public float GasDecreaseSpeed => 0.01f - GameManager.Instance.StatValues[4] / 10;
    public bool Rescued = false;

    public List<GameObject> CarModels;

    private bool abandoned = false;
    private bool _toDestroy = false;

    private void Start()
    {
        _bc = GetComponent<BoxCollider>();
        GameObject go = GameObject.Instantiate(CarModels[Random.Range(0, CarModels.Count)]);
        go.transform.parent = this.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (_toDestroy || !Rescued || !GameManager.Instance.Started) return;

        HandleGas();
    }

    public void Rescue(Transform rescueSpot)
    {
        Audio.Play();
        RescueSpot = rescueSpot;
        _bc.enabled = false;
        transform.SetParent(rescueSpot);
        transform.position = rescueSpot.position;
        transform.rotation = rescueSpot.rotation;
        GetComponent<CarMovement>().enabled = true;

        Gas = Random.Range(0.05f, 0.15f);

        // UI Stuff
        _statusBar = Instantiate(statusBar, GameManager.Instance.RescueStatusParent).GetComponent<RescueStatus>();
        _statusBar.GasBarButton.onClick.AddListener(GasBarClicked);
        _statusBar.AbandonButton.onClick.AddListener(AbandonUnit);

        ExhaustEffect.SetActive(true);

        Rescued = true;
    }

    // The gas bar has been clicked, give gas to rescue unit
    private void GasBarClicked()
    {
        GameManager.Instance.Player.GiveGas(this, 0.1f);
    }
    // The abandon button has been clicked, sending the unit in the fire of hell.
    private void AbandonUnit()
    {
        Audio.Stop();
        Audio.PlayOneShot(OutOfGas);

        abandoned = true;
        StartCoroutine(AbandonUnitCoroutine());
    }

    private IEnumerator AbandonUnitCoroutine()
    {
        // Removing the status bar
        Destroy(_statusBar.gameObject);
        GameManager.Instance.Player.AbandonUnit(this);

        ExhaustEffect.SetActive(false);

        var t = 0f;
        var from = transform.position;
        var to = new Vector3(transform.position.x - 15, transform.position.y, transform.position.z);
        while (t < 1)
        {
            // lerping position into the fire.
            transform.position = Vector3.Lerp(from, to, t);
            t += Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.ChangeKarma(-6 + GameManager.Instance.StatValues[3]);
        GameManager.Instance.Explosion();
        Destroy(this.gameObject);
    }

    public void DestroyUnit()
    {
        Destroy(_statusBar.gameObject);
        Destroy(this.gameObject);
    }

    private void HandleGas()
    {
        Gas = Mathf.Clamp01(Gas - (GasDecreaseSpeed * Time.deltaTime));
        _statusBar.SetGas(Gas);

        if (Gas == 0 && !abandoned)
        {
            AbandonUnit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "rescueDeathZone")
        {
            GameManager.Instance.ChangeKarma(-6 + GameManager.Instance.StatValues[3]);
            GameManager.Instance.Explosion();
            Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
