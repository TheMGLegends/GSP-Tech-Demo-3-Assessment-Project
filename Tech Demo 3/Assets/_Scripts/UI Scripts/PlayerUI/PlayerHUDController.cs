/// <summary>
/// Controls things relevant to player hud only (Mana since enemies have none)
/// </summary>
public class PlayerHUDController : CharacterHUDController
{
    public override void SetMana(float newMana)
    {
        manaSlider.value = newMana;
        manaText.text = manaSlider.value + "/" + maxMana;
    }
}
