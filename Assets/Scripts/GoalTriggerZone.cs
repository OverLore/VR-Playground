using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoalTriggerZone : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTriggerEnterAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
            OnTriggerEnterAction?.Invoke();
    }
}
