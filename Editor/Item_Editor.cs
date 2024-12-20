namespace AF
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.Localization.Tables;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Item))]
    public class ItemEditor : Editor
    {
        public VisualTreeAsset VisualTree;
        public StringTable englishTable;
        public StringTable portugueseTable;

        Item item;

        private void OnEnable()
        {
            item = (Item)target;
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
                var existingEntry = englishTable.GetEntry(item.name);
                if (existingEntry == null)
                    englishTable.AddEntry(item.name, item.englishName);
                else
                    existingEntry.Value = item.englishName;

                SaveAll();
            });

            root.Q<PropertyField>("WeaponNamePT").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = portugueseTable.GetEntry(item.name);
                if (existingEntry == null)
                    portugueseTable.AddEntry(item.name, item.portugueseName);
                else
                    existingEntry.Value = item.portugueseName;

                SaveAll();
            });

            string descriptionKey = item.name + "_Description";

            root.Q<PropertyField>("WeaponDescriptionEN").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = englishTable.GetEntry(descriptionKey);
                if (existingEntry == null)
                    englishTable.AddEntry(descriptionKey, item.englishDescription);
                else
                    existingEntry.Value = item.englishDescription;

                SaveAll();
            });

            root.Q<PropertyField>("WeaponDescriptionNamePT").RegisterCallback<FocusOutEvent>(ev =>
            {
                var existingEntry = portugueseTable.GetEntry(descriptionKey);
                if (existingEntry == null)
                    portugueseTable.AddEntry(descriptionKey, item.portugueseDescription);
                else
                    existingEntry.Value = item.portugueseDescription;

                SaveAll();
            });

            root.Q<PropertyField>("WeaponSpriteField").RegisterValueChangeCallback(ev =>
                {
                    root.Q<IMGUIContainer>("WeaponSprite").style.backgroundImage =
                        new StyleBackground(item.sprite ?? null);
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
                portugueseTable.AddEntry(name, item.name);
            }
            else
            {
                portugueseExistingEntry.Value = item.name;
            }

            EditorUtility.SetDirty(englishTable);
            EditorUtility.SetDirty(englishTable.SharedData);
            EditorUtility.SetDirty(portugueseTable);
            EditorUtility.SetDirty(portugueseTable.SharedData);

            AssetDatabase.SaveAssets();
        }
    }
}
