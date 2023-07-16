using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Space(10), Header("Input")]
    [SerializeField] private InputActionProperty triggerAction;
    [SerializeField] private InputActionProperty gripAction;

    void Update()
    {
        animator.SetFloat("Trigger", triggerAction.action.ReadValue<float>());
        animator.SetFloat("Grip", gripAction.action.ReadValue<float>());
    }
}
