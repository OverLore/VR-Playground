using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Settings")]
    [SerializeField] private float shotSpeed;

    public void Hold()
    {
        PlayerController.AddLocker();
    }

    public void Throw()
    {
        PlayerController.RemoveLocker();
    }

    public void Shot()
    {
        GameObject go = Instantiate(bulletPrefab);
        go.transform.position = muzzle.position;
        go.transform.rotation = muzzle.rotation;

        go.GetComponent<Rigidbody>().velocity = muzzle.forward * shotSpeed;
        
        Destroy(go, 25f);
    }
}
