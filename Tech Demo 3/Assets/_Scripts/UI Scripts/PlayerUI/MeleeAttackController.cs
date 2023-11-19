using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeAttackController : MonoBehaviour
{
    [SerializeField] private Image outsideRangeImage;

    private Image buttonImage;
    private Color defaultColor;
    private Color activeColor;
    private bool isMeleeOn;

    public bool GetIsMeleeOn() => isMeleeOn;
    public void IsOutsideRangeImageActive(bool isActive) { outsideRangeImage.enabled = isActive; }

    private void Start()
    {
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
            isMeleeOn = true;
            buttonImage.color = activeColor;
        }
        else
        {
            isMeleeOn = false;
            buttonImage.color = defaultColor;
        }
    }
}
