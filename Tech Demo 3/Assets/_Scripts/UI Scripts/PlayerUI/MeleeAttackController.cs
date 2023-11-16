using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeAttackController : MonoBehaviour
{
    [SerializeField] private GameObject unusableObject;
    private Button meleeButton;

    private Image buttonImage;
    private Color defaultColor;
    private Color activeColor;
    private bool isMeleeOn;

    public bool GetIsMeleeOn() => isMeleeOn;

    private void Start()
    {
        meleeButton = GetComponent<Button>();

        meleeButton.interactable = false;
        unusableObject.SetActive(true);

        buttonImage = GetComponent<Image>();
        defaultColor = buttonImage.color;
        activeColor = Color.green;

        gameObject.SetActive(false);
    }
    
    public void EnableMeleeAttack()
    {
        unusableObject.SetActive(false);
        meleeButton.interactable = true;

        if (isMeleeOn)
            buttonImage.color = activeColor;
    }

    public void DisableMeleeAttack()
    {
        meleeButton.interactable = false;
        unusableObject.SetActive(true);
        
        if (isMeleeOn)
            buttonImage.color = defaultColor;
    }

    public void ToggleButton()
    {
        if (!isMeleeOn)
        {
            buttonImage.color = activeColor;
            isMeleeOn = true;
        }
        else
        {
            buttonImage.color = defaultColor;
            isMeleeOn = false;
        }
    }
}
