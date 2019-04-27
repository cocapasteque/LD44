using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int laneOffset;
    [SerializeField] private PlayerMovement currentPosition;
    public List<Transform> RescueSpots;
    private List<Transform> _usedSpots = new List<Transform>();
    private List<RescueUnit> _rescued = new List<RescueUnit>();

    public float Gas = 1f;
    public float GasDecreaseSpeed = 1f;

    private bool _canMove = true;
    private bool _keyUp = true;

    private void Update()
    {
        HandleMovement();
        HandleGas();
    }

    // Handling the gas tank of the player.
    private void HandleGas()
    {
        Gas = Mathf.Clamp01(Gas - ((GasDecreaseSpeed * Time.deltaTime) / 10));
    }
    // Handling the keyboard input for the player.
    private void HandleMovement()
    {
        var horizontal = Input.GetAxis("Horizontal");
        if (horizontal == 0) _keyUp = true;

        if (_canMove && _keyUp && horizontal < 0 && currentPosition != PlayerMovement.Left)
        {
            _keyUp = false;
            var move = new Vector3(transform.position.x, transform.position.y, transform.position.z + laneOffset);
            StartCoroutine(ChangeLane(move, PlayerMovement.Left));
        }
        else if (_canMove && _keyUp && horizontal > 0 && currentPosition != PlayerMovement.Right)
        {
            _keyUp = false;
            var move = new Vector3(transform.position.x, transform.position.y, transform.position.z - laneOffset);
            StartCoroutine(ChangeLane(move, PlayerMovement.Right));
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
    }
    // Abandon unit
    public void AbandonUnit(RescueUnit unit)
    {
        // Free the spot in the rescuespot list and remove the unit form the rescued list.
        _rescued.Remove(unit);
        _usedSpots.Remove(unit.RescueSpot);
        RescueSpots.Add(unit.RescueSpot);
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
        _canMove = false;
        currentPosition = (PlayerMovement)((int)currentPosition + (int)move);
        var t = 0f;
        var from = transform.position;

        while (t <= 1f)
        {
            transform.position = Vector3.Slerp(from, to, t);
            t += Time.deltaTime * 5f;
            yield return null;
        }
        transform.position = to;
        _canMove = true;
    }
    private void OnTriggerStay(Collider other)
    {
        // If we are rescuing an unit
        if (other.tag == "rescueUnit")
        {
            var unit = other.GetComponent<RescueUnit>();
            if (!unit.Rescued && Input.GetKeyDown(KeyCode.Space))
            {
                RescueUnit(unit);
            }
        }
    }
}

public enum PlayerMovement
{
    Right = 1,
    Middle = 0,
    Left = -1
}