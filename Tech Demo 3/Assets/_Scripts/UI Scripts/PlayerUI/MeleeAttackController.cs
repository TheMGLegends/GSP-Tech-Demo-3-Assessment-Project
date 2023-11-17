using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeAttackController : MonoBehaviour
{
    [SerializeField] private Image outsideRangeImage;
    private Button meleeButton;

    private Image buttonImage;
    private Color defaultColor;
    private Color activeColor;
    private bool isMeleeOn;

    public bool GetIsMeleeOn() => isMeleeOn;
    public void IsOutsideRangeImageActive(bool isActive) { outsideRangeImage.enabled = isActive; }

    private void Start()
    {
        meleeButton = GetComponent<Button>();
        outsideRangeImage.enabled = true;

        buttonImage = GetComponent<Image>();
        defaultColor = buttonImage.color;
        activeColor = Color.green;

        gameObject.SetActive(false);
    }

    public void ToggleButton()
    {
        if (!isMeleeOn)
        {
            //buttonImage.color = activeColor;
            isMeleeOn = true;
            buttonImage.color = activeColor;
        }
        else
        {
            //buttonImage.color = defaultColor;
            isMeleeOn = false;
            buttonImage.color = defaultColor;
        }
    }
}
