using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;


    private List<char> blockQueue = new();
    public static bool CanTeleport => Instance.blockQueue.Count == 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void AddLocker()
    {
        Instance.blockQueue.Add('0');
    }

    public static void RemoveLocker()
    {
        Instance.blockQueue.RemoveAt(0);
    }
}
