namespace AF
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.Localization.Tables;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(EV_SimpleMessage))]
    public class EV_SimpleMessage_Editor : Editor
    {
        public VisualTreeAsset VisualTree;
        public StringTable englishDialogueTable;
        public StringTable portugueseDialogueTable;

        EV_SimpleMessage eV_SimpleMessage;

        private void OnEnable()
        {
            eV_SimpleMessage = (EV_SimpleMessage)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            VisualTree.CloneTree(root);
            root.Q<PropertyField>("CharacterField").RegisterValueChangeCallback(ev =>
            {
                root.Q<IMGUIContainer>("CharacterAvatar").style.backgroundImage = new StyleBackground(eV_SimpleMessage.character.avatar ?? null);
            });
            root.Q<PropertyField>("EnglishMessage").RegisterCallback<FocusOutEvent>(ev =>
            {
                UpdateLocalization();
            });
            root.Q<PropertyField>("PortugueseMessage").RegisterCallback<FocusOutEvent>(ev =>
            {
                UpdateLocalization();
            });

            return root;
        }

        public void UpdateLocalization()
        {
            var englishExistingEntry =
                englishDialogueTable.GetEntry(eV_SimpleMessage.monoBehaviourID.ID);
            if (englishExistingEntry == null)
            {
                englishDialogueTable.AddEntry(eV_SimpleMessage.monoBehaviourID.ID, eV_SimpleMessage.englishMessage);
            }
            else
            {
                englishExistingEntry.Value = eV_SimpleMessage.englishMessage;
            }

            var portugueseExistingEntry =
                portugueseDialogueTable.GetEntry(eV_SimpleMessage.monoBehaviourID.ID);
            if (portugueseExistingEntry == null)
            {
                portugueseDialogueTable.AddEntry(eV_SimpleMessage.monoBehaviourID.ID, eV_SimpleMessage.portugueseMessage);
            }
            else
            {
                portugueseExistingEntry.Value = eV_SimpleMessage.portugueseMessage;
            }

            EditorUtility.SetDirty(englishDialogueTable);
            EditorUtility.SetDirty(englishDialogueTable.SharedData);
            EditorUtility.SetDirty(portugueseDialogueTable);
            EditorUtility.SetDirty(portugueseDialogueTable.SharedData);

            AssetDatabase.SaveAssets();
        }
    }
}
