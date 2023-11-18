using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    public GameObject playerObject;

    public GameObject damagePopupPrefab;
    private int hitChance = 80;
    private int critChance = 20;

    public int GetHitChance() => hitChance;
    public int GetCritChance() => critChance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
