﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int laneOffset;
    [SerializeField] private Vector3 rightLane;
    [SerializeField] private Vector3 midLane;
    [SerializeField] private Vector3 leftLane;
    [SerializeField] private PlayerMovement currentPosition;
    [SerializeField] private AudioSource engine;

    public List<Transform> RescueSpots;
    private List<Transform> _usedSpots = new List<Transform>();
    private List<RescueUnit> _rescued = new List<RescueUnit>();

    public float Gas = 1f;
    public float MaxGas => 1f + GameManager.Instance.StatValues[0] / 5f;
    public float GasDecreaseSpeed => 0.01f - (GameManager.Instance.StatValues[1] / 1000);
    public GameObject Model;

    private bool _canMove = true;
    private bool _keyUp = true;
    private bool _moveForward = false;

    public void Init()
    {
        GetComponent<CarMovement>().SetNewModel();
        Gas = MaxGas;
        foreach (var rescue in _rescued)
        {
            _usedSpots.Remove(rescue.RescueSpot);
            RescueSpots.Add(rescue.RescueSpot);
            rescue.DestroyUnit();
        }
        _rescued.Clear();
        Gas = MaxGas;
        transform.position = GameManager.Instance.PlayerSpawn.position;
        currentPosition = PlayerMovement.Middle;
        _canMove = true;
        _moveForward = false;
        engine.volume = 0.6f;
        engine.Play();
    }

    public void StopEngine()
    {
        engine.Stop();
    }

    private void Update()
    {
        HandleMovement();
        HandleGas();
        if (_moveForward)
        {
            transform.Translate(Vector3.right * GameManager.Instance.Speed * Time.deltaTime, Space.World);
            foreach (var rescue in _rescued)
            {
                rescue.gameObject.transform.Translate(Vector3.right * GameManager.Instance.Speed * Time.deltaTime, Space.World);
            }
            if (transform.position.x > 100f)
            {
                _moveForward = false;
            }
        }
    }

    public void EndOfLevel(bool won)
    {
        _canMove = false;
        if (won)
        {
            _moveForward = true;
        }
    }

    // Handling the gas tank of the player.
    private void HandleGas()
    {
        Gas = Mathf.Clamp(Gas - GasDecreaseSpeed * Time.deltaTime, 0, MaxGas);
    }
    // Handling the keyboard input for the player.
    private void HandleMovement()
    {
        if (!_canMove) return;

        if (Input.GetKeyDown(KeyCode.A) && currentPosition != PlayerMovement.Left)
        {
            StopCoroutine("ChangeLane");
            switch (currentPosition)
            {
                case PlayerMovement.Middle: StartCoroutine(ChangeLane(leftLane, PlayerMovement.Left)); break;
                case PlayerMovement.Right: StartCoroutine(ChangeLane(midLane, PlayerMovement.Middle)); break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentPosition != PlayerMovement.Right)
        {        
            StopCoroutine("ChangeLane");
            switch (currentPosition)
            {
                case PlayerMovement.Middle: StartCoroutine(ChangeLane(rightLane, PlayerMovement.Right)); break;
                case PlayerMovement.Left: StartCoroutine(ChangeLane(midLane, PlayerMovement.Middle)); break;
            }
        }
    }
    // Rescue unit and put it in the rescued pool behind player's car.
    private void RescueUnit(RescueUnit unit)
    {
        var randomSpot = RescueSpots[Random.Range(0, RescueSpots.Count)];
        unit.Rescue(randomSpot);
        RescueSpots.Remove(randomSpot);
        _usedSpots.Add(randomSpot);
        _rescued.Add(unit);
        GiveGas(unit, 0.1f);
        GameManager.Instance.ChangeKarma(GameManager.Instance.KarmaPerCar);
    }
    // Abandon unit
    public void AbandonUnit(RescueUnit unit)
    {
        // Free the spot in the rescue spot list and remove the unit form the rescued list.
        _rescued.Remove(unit);
        _usedSpots.Remove(unit.RescueSpot);
        RescueSpots.Add(unit.RescueSpot);
        // Get the gas back from the car.
        Gas += unit.Gas / 4;
        GameManager.Instance.ChangeKarma(-GameManager.Instance.KarmaPerCar);
    }
    // Give some gas to a rescued unit
    public void GiveGas(RescueUnit unit, float amount)
    {
        Gas -= amount / 4;
        unit.Gas += amount;
    }
    // Smooth switch of lane.
    private IEnumerator ChangeLane(Vector3 to, PlayerMovement move)
    {
        currentPosition = move;
        var t = 0f;
        var from = transform.position;

        while (t <= 1f)
        {
            transform.position = Vector3.Slerp(from, to, t);
            t += Time.deltaTime * 5f;
            yield return null;
        }
        transform.position = to;
    }
    private void OnTriggerStay(Collider other)
    {
        // If we are rescuing an unit
        if (other.tag == "rescueUnit")
        {
            var unit = other.GetComponent<RescueUnit>();
            if (!unit.Rescued && Input.GetKey(KeyCode.Space))
            {
                unit.Outline(false);
                RescueUnit(unit);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "rescueUnit")
        {
            var unit = other.GetComponent<RescueUnit>();
            unit.Outline(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "rescueUnit")
        {
            var unit = other.GetComponent<RescueUnit>();
            unit.Outline(false);
        }
    }
}

public enum PlayerMovement
{
    Right = 1,
    Middle = 0,
    Left = -1
}