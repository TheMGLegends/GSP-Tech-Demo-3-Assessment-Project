using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : CharacterHUDController
{
    public override void SetMana(float newMana)
    {
        manaSlider.value = newMana;
        manaText.text = manaSlider.value + "/" + maxMana;
    }
}
