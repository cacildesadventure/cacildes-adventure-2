namespace AF
{
    using AF.Events;
    using AF.Inventory;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;

    public class EquipmentHUD : MonoBehaviour
    {
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        VisualElement root;
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public Texture2D unequippedSpellSlot, unequippedWeaponSlot, unequippedConsumableSlot, unequippedShieldSlot, unequippedArrowSlot;
        public Vector3 popEffectWhenSwitchingSlots = new Vector3(0.8f, 0.8f, 0.8f);

        private Label quickItemName, arrowsLabel;
        private IMGUIContainer shieldBlockedIcon, spellSlotContainer, consumableSlotContainer, weaponSlotContainer, shieldSlotContainer;
        private VisualElement stanceIcon;
        VisualElement equipmentContainer;

        [Header("Stance Sprites")]
        public Sprite oneHandStanceIcon;
        public Sprite twoHandStanceIcon;

        [Header("Gamepad Sprites")]
        public Sprite switchSpellGamepad;
        public Sprite switchWeaponGamepad;
        public Sprite switchShieldGamepad;
        public Sprite switchConsumableGamepad;
        public Sprite useConsumableGamepad;
        public string switchSpellKey;
        public string switchWeaponKey;
        public string switchConsumableKey;
        public string switchShieldKey;
        public string useConsumableKey;

        private void Awake()
        {
            SetupRefs();

            EventManager.StartListening(EventMessages.ON_EQUIPMENT_CHANGED, UpdateUI);
            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, () => UpdateCombatStanceIndicator(true));
        }

        void OnEnable()
        {
            InputSystem.onDeviceChange += HandleDeviceChangeCallback;

            UpdateUI();
        }

        private void SetupRefs()
        {
            this.root = uIDocumentPlayerHUDV2.uIDocument.rootVisualElement;

            spellSlotContainer = root.Q<IMGUIContainer>("SpellSlot");
            consumableSlotContainer = root.Q<IMGUIContainer>("ConsumableSlot");
            weaponSlotContainer = root.Q<IMGUIContainer>("WeaponSlot");
            shieldSlotContainer = root.Q<IMGUIContainer>("ShieldSlot");
            shieldBlockedIcon = shieldSlotContainer.Q<IMGUIContainer>("Blocked");
            equipmentContainer = root.Q<VisualElement>("EquipmentContainer");
            stanceIcon = root.Q<VisualElement>("StanceIcon");

            quickItemName = root.Q<Label>("QuickItemName");
            arrowsLabel = root.Q<Label>("ArrowsLabel");
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= HandleDeviceChangeCallback;
        }

        private void HandleDeviceChangeCallback(InputDevice device, InputDeviceChange change)
        {
            UpdateUI();
        }
        public void UpdateUI()
        {
            if (!isActiveAndEnabled || root == null) return;

            UpdateSlotContents();
            UpdateArrowsAndQuickItemName();
            UpdateCombatStanceIndicator(false);
        }

        public void OnSwitchWeapon() => OnSwitchEquipment(weaponSlotContainer);
        public void OnSwitchShield() => OnSwitchEquipment(shieldSlotContainer);
        public void OnSwitchConsumable() => OnSwitchEquipment(consumableSlotContainer);
        public void OnSwitchSpell() => OnSwitchEquipment(spellSlotContainer);

        public bool IsVisible()
        {
            return equipmentContainer?.visible ?? false;
        }

        private void OnSwitchEquipment(VisualElement container)
        {
            UIUtils.PlayPopAnimation(container, popEffectWhenSwitchingSlots, 0.5f);
            UpdateUI();
        }

        public bool IsEquipmentDisplayed() => uIDocumentPlayerHUDV2.uIDocument.rootVisualElement.visible && equipmentContainer.visible;

        private void UpdateSlotContents()
        {
            UpdateSlot(spellSlotContainer, equipmentDatabase.IsBowEquipped() ? equipmentDatabase.GetCurrentArrow() : equipmentDatabase.GetCurrentSpell(), unequippedSpellSlot, unequippedArrowSlot);
            UpdateSlot(weaponSlotContainer, equipmentDatabase.GetCurrentWeapon(), unequippedWeaponSlot);
            UpdateSlot(consumableSlotContainer, equipmentDatabase.GetCurrentConsumable(), unequippedConsumableSlot);
            UpdateSlot(shieldSlotContainer,
                equipmentDatabase.GetCurrentSecondaryWeapon() != null
                    ? equipmentDatabase.GetCurrentSecondaryWeapon()
                    : equipmentDatabase.GetCurrentShield(),
                unequippedShieldSlot);
            shieldBlockedIcon.style.display = equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped() ? DisplayStyle.Flex : DisplayStyle.None;

            UpdateSlotInputUI(spellSlotContainer.Q<VisualElement>("Keyboard"), switchSpellGamepad, switchSpellKey);
            UpdateSlotInputUI(weaponSlotContainer.Q<VisualElement>("Keyboard"), switchWeaponGamepad, switchWeaponKey);
            UpdateSlotInputUI(consumableSlotContainer.Q<VisualElement>("Keyboard"), switchConsumableGamepad, switchConsumableKey);
            UpdateSlotInputUI(consumableSlotContainer.Q<VisualElement>("ConsumableInfo").Q<VisualElement>("UseKeyboard"), useConsumableGamepad, useConsumableKey);
            UpdateSlotInputUI(shieldSlotContainer.Q<VisualElement>("Keyboard"), switchShieldGamepad, switchShieldKey);
        }

        private void UpdateSlot(IMGUIContainer slot, Item item, Texture2D unequippedSlot, Texture2D alternativeUnequippedSlot = null)
        {
            slot.style.backgroundImage = item != null
                ? new StyleBackground(item.sprite)
                : new StyleBackground(alternativeUnequippedSlot ?? unequippedSlot);
        }


        private void UpdateSlotInputUI(VisualElement container, Sprite switchGamepad, string switchKey)
        {
            var gamepad = container.Q<VisualElement>("Gamepad");
            var key = container.Q<Label>("KeyValue");

            if (Gamepad.current != null)
            {
                gamepad.style.backgroundImage = new StyleBackground(switchGamepad);
                gamepad.style.display = DisplayStyle.Flex;
                key.text = "";
            }
            else
            {
                key.text = switchKey;
                gamepad.style.display = DisplayStyle.None;
            }
        }

        private void UpdateArrowsAndQuickItemName()
        {
            var consumable = equipmentDatabase.GetCurrentConsumable();
            quickItemName.text = consumable != null
                ? $"{consumable.GetName()} ({inventoryDatabase.GetItemAmount(consumable)})"
                : "";

            consumableSlotContainer.Q<VisualElement>("ConsumableInfo").style.display = consumable == null ? DisplayStyle.None : DisplayStyle.Flex;

            arrowsLabel.text = equipmentDatabase.IsBowEquipped() && equipmentDatabase.GetCurrentArrow() != null
                ? equipmentDatabase.GetCurrentArrow().GetName() + " (" + inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentArrow()) + ")"
                : "";
        }
        private void UpdateCombatStanceIndicator(bool playPopAnimation)
        {
            stanceIcon.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(equipmentDatabase.isTwoHanding ? twoHandStanceIcon : oneHandStanceIcon);
            if (playPopAnimation)
            {
                UIUtils.PlayPopAnimation(stanceIcon);
            }
        }
    }
}
