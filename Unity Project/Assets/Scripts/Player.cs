using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int laneOffset;
    [SerializeField] private PlayerMovement currentPosition;

    public float Gas = 1f;
    public float GasDecreaseSpeed = 1f;

    private bool _canMove = true;

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
        if (_canMove && Input.GetKeyDown(KeyCode.LeftArrow) && currentPosition != PlayerMovement.Left)
        {
            var move = new Vector3(transform.position.x, transform.position.y, transform.position.z + laneOffset);
            StartCoroutine(ChangeLane(move, PlayerMovement.Left));
        }
        else if (_canMove && Input.GetKeyDown(KeyCode.RightArrow) && currentPosition != PlayerMovement.Right)
        {
            var move = new Vector3(transform.position.x, transform.position.y, transform.position.z - laneOffset);
            StartCoroutine(ChangeLane(move, PlayerMovement.Right));
        }
    }
    // Rescue unit and put it in the rescued pool behind player's car.
    private void RescueUnit(RescueUnit unit)
    {

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RescueUnit(other.GetComponent<RescueUnit>());
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