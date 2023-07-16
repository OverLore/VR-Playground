using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class DangerBallPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform resetTransform; 
    [SerializeField] private GameObject player; 
    [SerializeField] private Camera playerHead;
    [SerializeField] private GameObject paddle;
    [SerializeField] private Transform paddleMin;
    [SerializeField] private Transform paddleMax;
    [SerializeField] private TMP_Text debugText;

    [Space(10), Header("Settings")]
    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private Vector2 forwardHeadMovementMinMax = new (0, 1.5f);
    [SerializeField] private Vector2 backwardHeadMovementMinMax = new (0, .5f);
    [SerializeField] private float headMovementForceMultiplier = 2f;

    private Vector3 previousPosition;
    private float headVelocity;
    
    private Vector3 previousPaddlePosition;
    private Vector3 paddleVelocity;

    private void Start()
    {
        previousPosition = transform.position;
        debugText.text = string.Empty;
    }

    private void HandleHeadVelocity()
    {
        Vector3 currentPosition = playerHead.transform.position;

        Vector3 velocity = (currentPosition - previousPosition) / Time.deltaTime;

        previousPosition = currentPosition;

        float sign = Mathf.Sign(Vector3.Dot(velocity, playerHead.transform.forward));
        headVelocity = Vector3.Project(velocity, playerHead.transform.forward).magnitude * sign;
    }

    private void HandlePaddleVelocity()
    {
        Vector3 currentPosition = paddle.transform.position;

        paddleVelocity = (currentPosition - previousPaddlePosition) / Time.deltaTime;
        paddleVelocity.z = 0;

        previousPaddlePosition = currentPosition;
    }

    public void SetDebug(string str)
    {
        debugText.text = str;
    }

    public float GetHitForce()
    {
        if (headVelocity > 0)
            return Mathf.Clamp(headVelocity, forwardHeadMovementMinMax.x, forwardHeadMovementMinMax.y) * headMovementForceMultiplier;

        return -Mathf.Clamp(Mathf.Abs(headVelocity), backwardHeadMovementMinMax.x, backwardHeadMovementMinMax.y) * headMovementForceMultiplier;
    }

    public Vector3 GetMoveForce()
    {
        return paddleVelocity;
    }

    public void ResetPosition()
    {
        float rotationAngleY = resetTransform.rotation.eulerAngles.y - playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(0, rotationAngleY, 0);
        Vector3 distanceDiff = resetTransform.position - playerHead.transform.position;
        player.transform.position += distanceDiff;
    }

    private void HandlePaddleMovement()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerHead.transform.position, playerHead.transform.forward, out hit, 100f, gridLayer))
        {
            paddle.transform.position = new Vector3(Mathf.Clamp(hit.point.x, paddleMin.position.x, paddleMax.position.x),
                Mathf.Clamp(hit.point.y, paddleMin.position.y, paddleMax.position.y),
                Mathf.Clamp(hit.point.z, paddleMin.position.z, paddleMax.position.z));
        }
    }

    private void Update()
    {
        HandlePaddleMovement();
        HandleHeadVelocity();
        HandlePaddleVelocity();
    }
}
