namespace AF
{
    using AF.Health;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIWeaponStatsContainer : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public IconsDatabase iconsDatabase;


        [Header("Components")]
        public VisualTreeAsset weaponStatsContainer;
        public VisualTreeAsset attributeIndicator;

        public void ClearPreviews(VisualElement root)
        {
            root.Q<VisualElement>("WeaponStatsContainer").Clear();
        }

        public void PreviewWeaponUpgrade(Weapon weapon, VisualElement root)
        {
            ClearPreviews(root);

            var currentStatsRoot = weaponStatsContainer.CloneTree();
            currentStatsRoot.Q<VisualElement>("AttributeIndicatorContainer").Add(CreateLabel(weapon.GetName() + " +" + weapon.level, 10));

            root.Q<VisualElement>("WeaponStatsContainer").Add(currentStatsRoot);

            Damage currentWeaponDamage = weapon.weaponDamage.GetCurrentDamage(playerManager,
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence(),
                weapon);

            UpdateWeaponDamageUI(currentStatsRoot, currentWeaponDamage, weapon, playerManager);

            if (weapon.CanBeUpgradedFurther())
            {
                root.Q<VisualElement>("WeaponStatsContainer").Add(CreateLabel(" > ", 0));

                var nextStatsRoot = weaponStatsContainer.CloneTree();
                nextStatsRoot.Q<VisualElement>("AttributeIndicatorContainer").Add(CreateLabel(weapon.GetName() + " +" + (weapon.level + 1), 10));

                root.Q<VisualElement>("WeaponStatsContainer").Add(nextStatsRoot);


                // Serialize the original weapon into JSON
                string json = JsonUtility.ToJson(weapon);

                // Create a temporary ScriptableObject and deserialize the JSON into it
                Weapon nextWeapon = ScriptableObject.CreateInstance<Weapon>();
                JsonUtility.FromJsonOverwrite(json, nextWeapon);
                nextWeapon.level = weapon.level + 1;

                UpdateWeaponDamageUI(nextStatsRoot, currentWeaponDamage, nextWeapon, playerManager);
            }
        }

        Label CreateLabel(string text, int marginBottom)
        {
            Label newLabel = new() { text = text };

            newLabel.style.marginBottom = marginBottom;
            newLabel.AddToClassList("label-text");
            return newLabel;
        }

        void UpdateWeaponDamageUI(VisualElement root, Damage currentWeaponDamage, Weapon weapon, PlayerManager playerManager)
        {
            Damage desiredWeaponDamage = weapon.weaponDamage.GetCurrentDamage(
                playerManager,
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence(),
                weapon
            );

            bool isPortuguese = Glossary.IsPortuguese();

            // Update damage types
            UpdateDamageUI(root, isPortuguese ? "Ataque Físico" : "Physical Attack", iconsDatabase.physicalAttack, currentWeaponDamage.physical, desiredWeaponDamage.physical);
            UpdateDamageUI(root, isPortuguese ? "Ataque de Fogo" : "Fire Attack", iconsDatabase.fire, currentWeaponDamage.fire, desiredWeaponDamage.fire);
            UpdateDamageUI(root, isPortuguese ? "Ataque de Gelo" : "Frost Attack", iconsDatabase.frost, currentWeaponDamage.frost, desiredWeaponDamage.frost);
            UpdateDamageUI(root, isPortuguese ? "Ataque Mágico" : "Magic Attack", iconsDatabase.magic, currentWeaponDamage.magic, desiredWeaponDamage.magic);
            UpdateDamageUI(root, isPortuguese ? "Ataque Trovão" : "Lightning Attack", iconsDatabase.lightning, currentWeaponDamage.lightning, desiredWeaponDamage.lightning);
            UpdateDamageUI(root, isPortuguese ? "Ataque das Trevas" : "Darkness Attack", iconsDatabase.darkness, currentWeaponDamage.darkness, desiredWeaponDamage.darkness);
            UpdateDamageUI(root, isPortuguese ? "Ataque Mágico" : "Magic Attack", iconsDatabase.magic, currentWeaponDamage.magic, desiredWeaponDamage.magic);
            UpdateDamageUI(root, isPortuguese ? "Ataque Aquático" : "Water Attack", iconsDatabase.water, currentWeaponDamage.water, desiredWeaponDamage.water);
        }

        private void UpdateDamageUI(
            VisualElement root,
            string attributeName,
            Sprite icon,
            float currentValue,
            float desiredValue)
        {
            var label = attributeIndicator.CloneTree();
            label.Q<Label>("StatName").text = attributeName + ": ";

            Label currentValueLabel = label.Q<Label>("CurrentValue");
            currentValueLabel.text = desiredValue.ToString();

            currentValueLabel.style.marginLeft = 10;

            if (desiredValue > currentValue)
            {
                currentValueLabel.style.color = Color.green;
            }
            else if (desiredValue < currentValue)
            {
                currentValueLabel.style.color = Color.red;
            }

            label.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(icon);

            root.Q<VisualElement>("AttributeIndicatorContainer").Add(label);
        }

    }
}
