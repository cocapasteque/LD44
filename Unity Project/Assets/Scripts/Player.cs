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

    private void HandleGas()
    {
        Gas = Mathf.Clamp01(Gas - ((GasDecreaseSpeed * Time.deltaTime) / 10));
    }
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
}

public enum PlayerMovement
{
    Right = 1,
    Middle = 0,
    Left = -1
}