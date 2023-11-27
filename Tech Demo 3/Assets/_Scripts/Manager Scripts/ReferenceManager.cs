using UnityEngine;

/// <summary>
/// Holds references that are needed across the entire scope of the game
/// </summary>
public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    public GameObject playerObject;
    public GameObject damagePopupPrefab;
    public GameObject spellPrefab;
    public GameObject statusEffectPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
