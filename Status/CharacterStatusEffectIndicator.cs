namespace AF
{
    using AF.StatusEffects;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CharacterStatusEffectIndicator : MonoBehaviour
    {
        public Slider slider;

        public TextMeshProUGUI label;

        public void UpdateUI(AppliedStatusEffect statusEffect, float currentMaximumResistanceToStatusEffect)
        {
            label.text = statusEffect.hasReachedTotalAmount ? statusEffect.statusEffect.GetAppliedName() : statusEffect.statusEffect.GetName();
            slider.value = statusEffect.currentAmount;
            slider.maxValue = currentMaximumResistanceToStatusEffect;

            var colors = slider.colors;
            colors.normalColor = statusEffect.statusEffect.barColor;
            colors.highlightedColor = statusEffect.statusEffect.barColor;
            colors.pressedColor = statusEffect.statusEffect.barColor;
            colors.selectedColor = statusEffect.statusEffect.barColor;
            colors.disabledColor = Color.gray;
            slider.colors = colors;
        }
    }

}
