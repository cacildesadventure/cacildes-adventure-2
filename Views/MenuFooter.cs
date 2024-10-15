namespace AF
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MenuFooter : MonoBehaviour
    {
        [HideInInspector] public VisualElement root;

        [Header("Inputs")]

        VisualElement tooltipContainer, footerActionsContainer;
        Label tooltipLabel;

        public void SetupReferences()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            tooltipContainer = root.Q<VisualElement>("Tooltip");
            tooltipContainer.style.display = DisplayStyle.None;

            tooltipLabel = root.Q<Label>("TooltipInfo");

            footerActionsContainer = root.Q<VisualElement>("FooterActionsContainer");
            footerActionsContainer.Clear();
        }

        public void DisplayTooltip(string text)
        {

            tooltipLabel.text = text;
            tooltipContainer.style.display = DisplayStyle.Flex;
        }

        public void HideTooltip()
        {
            tooltipContainer.style.display = DisplayStyle.None;
        }

        public VisualElement GetFooterActionsContainer()
        {

            return footerActionsContainer;
        }

    }
}
