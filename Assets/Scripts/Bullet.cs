using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private List<Collider> colliders;
    
    [Space(10), Header("Settings")]
    [SerializeField] private LayerMask collisionLayers;

    private Vector3 lastPos;
    
    private bool isFlying = true;

    private void Start()
    {
        lastPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (isFlying)
        {
            if (rigidbody.velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
            }

            //Check all position since last pos
            RaycastHit hit;
            if (Physics.Linecast(lastPos, transform.position, out hit, collisionLayers))
            {
                Collision(hit.transform, hit.point);
            }

            lastPos = transform.position;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Collision(other.gameObject.transform, other.GetContact(0).point);
    }

    private void Collision(Transform otherTransform, Vector3 pos)
    {
        isFlying = false;

        rigidbody.isKinematic = true;

        transform.position = pos;

        transform.SetParent(otherTransform);

        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        Destroy(gameObject, 10f);
    }
}
