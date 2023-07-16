using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tpArray;
    [SerializeField] private XRRayInteractor ray;

    [Space(10), Header("Input")]
    [SerializeField] private InputActionProperty activationAction;

    public bool IsTryingToTeleport => activationAction.action.ReadValue<float>() > .3f;

    void Update()
    {
        bool isHitting = ray.TryGetHitInfo(out Vector3 pos, out Vector3 normal, out int number, out bool valid);

        tpArray.SetActive(!isHitting && IsTryingToTeleport && PlayerController.CanTeleport);
    }
}
