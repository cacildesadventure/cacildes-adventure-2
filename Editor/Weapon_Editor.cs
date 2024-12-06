namespace AF
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.Localization.Tables;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : Editor
    {
        public VisualTreeAsset VisualTree;
        public StringTable englishTable;
        public StringTable portugueseTable;

        Weapon weapon;

        private void OnEnable()
        {
            weapon = (Weapon)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            VisualTree.CloneTree(root);

            SetupRoot(root);

            return root;
        }

        void SetupRoot(VisualElement root)
        {
            root.Q<PropertyField>("WeaponNameEN").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = englishTable.GetEntry(weapon.name);
                if (existingEntry == null)
                    englishTable.AddEntry(weapon.name, weapon.englishName);
                else
                    existingEntry.Value = weapon.englishName;

                SaveAll();
            });

            root.Q<PropertyField>("WeaponNamePT").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = portugueseTable.GetEntry(weapon.name);
                if (existingEntry == null)
                    portugueseTable.AddEntry(weapon.name, weapon.portugueseName);
                else
                    existingEntry.Value = weapon.portugueseName;

                SaveAll();
            });

            string descriptionKey = weapon.name + "_Description";

            root.Q<PropertyField>("WeaponDescriptionEN").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = englishTable.GetEntry(descriptionKey);
                if (existingEntry == null)
                    englishTable.AddEntry(descriptionKey, weapon.englishDescription);
                else
                    existingEntry.Value = weapon.englishDescription;

                SaveAll();
            });

            root.Q<PropertyField>("WeaponDescriptionNamePT").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = portugueseTable.GetEntry(descriptionKey);
                if (existingEntry == null)
                    portugueseTable.AddEntry(descriptionKey, weapon.portugueseDescription);
                else
                    existingEntry.Value = weapon.portugueseDescription;

                SaveAll();
            });

            root.Q<PropertyField>("WeaponSpriteField").RegisterValueChangeCallback(ev =>
                {
                    root.Q<IMGUIContainer>("WeaponSprite").style.backgroundImage =
                        new StyleBackground(weapon.sprite ?? null);
                });

            root.Q<ToolbarButton>("General").RegisterCallback<ClickEvent>((ev) =>
            {
                HideToolbarPanels(root);

                root.Q<VisualElement>("GeneralInfo").style.display = DisplayStyle.Flex;
            });
            root.Q<ToolbarButton>("Speed").RegisterCallback<ClickEvent>((ev) =>
            {
                HideToolbarPanels(root);

                root.Q<VisualElement>("SpeedInfo").style.display = DisplayStyle.Flex;
            });

            HideToolbarPanels(root);
            root.Q<VisualElement>("GeneralInfo").style.display = DisplayStyle.Flex;
        }

        void HideToolbarPanels(VisualElement root)
        {

            root.Q<VisualElement>("GeneralInfo").style.display = DisplayStyle.None;
            root.Q<VisualElement>("SpeedInfo").style.display = DisplayStyle.None;
        }

        public void SaveAll()
        {
            var portugueseExistingEntry =
                portugueseTable.GetEntry(name);
            if (portugueseExistingEntry == null)
            {
                portugueseTable.AddEntry(name, weapon.name);
            }
            else
            {
                portugueseExistingEntry.Value = weapon.name;
            }

            EditorUtility.SetDirty(englishTable);
            EditorUtility.SetDirty(englishTable.SharedData);
            EditorUtility.SetDirty(portugueseTable);
            EditorUtility.SetDirty(portugueseTable.SharedData);

            AssetDatabase.SaveAssets();
        }
    }
}
