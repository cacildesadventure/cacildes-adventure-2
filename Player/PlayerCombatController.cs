namespace AF
{
    using System.Collections;
    using AF.Combat;
    using AF.Ladders;
    using UnityEngine;

    public class PlayerCombatController : MonoBehaviour
    {
        public float crossFade = 0.1f;
        public readonly string hashLightAttack1 = "Light Attack 1";
        public readonly string hashLightAttack2 = "Light Attack 2";
        public readonly string hashLightAttack3 = "Light Attack 3";
        public readonly string hashLightAttack4 = "Light Attack 4";
        public readonly string hashLightSecondaryAttack1 = "Light Secondary Attack 1";
        public readonly string hashLightSecondaryAttack2 = "Light Secondary Attack 2";
        public readonly string hashLightSecondaryAttack3 = "Light Secondary Attack 3";
        public readonly string hashLightSecondaryAttack4 = "Light Secondary Attack 4";
        public readonly string hashLightPowerStancingAttack1 = "Light Power Stance Attack 1";
        public readonly string hashLightPowerStancingAttack2 = "Light Power Stance Attack 2";
        public readonly string hashLightPowerStancingAttack3 = "Light Power Stance Attack 3";
        public readonly string hashLightPowerStancingAttack4 = "Light Power Stance Attack 4";
        public readonly int hashHeavyAttack1 = Animator.StringToHash("Heavy Attack 1");
        public readonly string hashHeavyAttack2 = "Heavy Attack 2";
        public readonly int hashSpecialAttack = Animator.StringToHash("Special Attack");
        public readonly int hashJumpAttack = Animator.StringToHash("Jump Attack");

        [Header("Attack Combo Index")]
        float maxIdleCombo = 2f;
        [SerializeField] int lightAttackMainWeaponComboIndex, lightAttackSecondaryWeaponComboIndex, heavyAttackComboIndex = 0;

        [Header("Flags")]
        public bool isCombatting = false;
        public bool isLightAttacking = false;
        public bool isAttackingWithLeftHand = false;

        [Header("Components")]
        public PlayerManager playerManager;
        public Animator animator;
        public UIManager uIManager;

        [Header("Heavy Attacks")]
        public int unarmedHeavyAttackBonus = 35;

        [Header("UI")]
        public MenuManager menuManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Flags")]
        public bool isHeavyAttacking = false;
        public bool isJumpAttacking = false;

        // Coroutines
        Coroutine ResetLightAttackComboIndexCoroutine;

        public readonly string SpeedMultiplierHash = "SpeedMultiplier";

        private void Start()
        {
            animator.SetFloat(SpeedMultiplierHash, 1f);
        }

        public void ResetStates()
        {
            isJumpAttacking = false;
            isHeavyAttacking = false;
            isLightAttacking = false;
            isAttackingWithLeftHand = false;

            animator.SetFloat(SpeedMultiplierHash, 1f);
        }

        public void OnLightAttack()
        {
            if (CanLightAttack())
            {
                HandleLightAttack(false);
            }
        }

        public void OnHeavyAttack()
        {
            if (CanHeavyAttack())
            {
                HandleHeavyAttack(false);
            }
        }

        public bool IsAttacking()
        {
            return isLightAttacking || isHeavyAttacking || isJumpAttacking;
        }

        public void HandleSecondaryAttack()
        {
            if (!CanAttack())
            {
                return;
            }

            isAttackingWithLeftHand = true;

            HandleLightAttack(true);
        }

        public void HandleLightAttack(bool isSecondaryWeapon)
        {
            isHeavyAttacking = false;
            isLightAttacking = true;

            if (playerManager.thirdPersonController.Grounded)
            {
                if (playerManager.playerBackstabController.PerformBackstab())
                {
                    return;
                }

                if (isSecondaryWeapon)
                {
                    if (lightAttackSecondaryWeaponComboIndex > GetMaxLightSecondaryWeaponCombo())
                        lightAttackSecondaryWeaponComboIndex = 0;

                    if (lightAttackSecondaryWeaponComboIndex == 0)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(
                            equipmentDatabase.IsPowerStancing() ? hashLightPowerStancingAttack1 : hashLightSecondaryAttack1, crossFade);
                    else if (lightAttackSecondaryWeaponComboIndex == 1)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(
                            equipmentDatabase.IsPowerStancing() ? hashLightPowerStancingAttack2 : hashLightSecondaryAttack2, crossFade);
                    else if (lightAttackSecondaryWeaponComboIndex == 2)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(
                            equipmentDatabase.IsPowerStancing() ? hashLightPowerStancingAttack3 : hashLightSecondaryAttack3, crossFade);
                    else if (lightAttackSecondaryWeaponComboIndex == 3)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(
                            equipmentDatabase.IsPowerStancing() ? hashLightPowerStancingAttack4 : hashLightSecondaryAttack4, crossFade);

                    lightAttackSecondaryWeaponComboIndex++;
                }
                else
                {
                    if (lightAttackMainWeaponComboIndex > GetMaxLightCombo())
                        lightAttackMainWeaponComboIndex = 0;

                    if (lightAttackMainWeaponComboIndex == 0)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashLightAttack1, crossFade);
                    else if (lightAttackMainWeaponComboIndex == 1)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashLightAttack2, crossFade);
                    else if (lightAttackMainWeaponComboIndex == 2)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashLightAttack3, crossFade);
                    else if (lightAttackMainWeaponComboIndex == 3)
                        playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashLightAttack4, crossFade);

                    lightAttackMainWeaponComboIndex++;
                }

                HandleAttackSpeed();
            }
            else
            {
                HandleJumpAttack();
            }

            playerManager.staminaStatManager.DecreaseLightAttackStamina();

            ResetCombos();
        }

        int GetMaxLightCombo()
        {
            int maxCombo = 1;

            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon() ?? equipmentDatabase.unarmedWeapon;

            if (currentWeapon?.weaponClass != null)
            {
                maxCombo = currentWeapon.weaponClass.lightAttackCombos - 1;
            }

            return maxCombo;
        }

        int GetMaxLightSecondaryWeaponCombo()
        {
            int maxCombo = 1;

            Weapon secondaryWeapon = equipmentDatabase.GetCurrentSecondaryWeapon();

            if (secondaryWeapon?.weaponClass != null)
            {
                maxCombo = secondaryWeapon.weaponClass.lightAttackCombos - 1;
            }

            return maxCombo;
        }

        void HandleAttackSpeed()
        {
            Weapon currentWeapon = isAttackingWithLeftHand ? equipmentDatabase.GetCurrentSecondaryWeapon() : equipmentDatabase.GetCurrentWeapon();
            if (currentWeapon == null) currentWeapon = equipmentDatabase.unarmedWeapon;

            if (equipmentDatabase.isTwoHanding)
            {
                animator.SetFloat(SpeedMultiplierHash, currentWeapon.twoHandAttackSpeedPenalty);
                return;
            }

            animator.SetFloat(SpeedMultiplierHash, currentWeapon.oneHandAttackSpeedPenalty);
        }

        void HandleJumpAttack()
        {
            isHeavyAttacking = false;
            isLightAttacking = false;
            isJumpAttacking = true;

            playerManager.playerWeaponsManager.HideShield();

            playerManager.playerAnimationEventListener.OpenRightWeaponHitbox();

            playerManager.PlayBusyHashedAnimationWithRootMotion(hashJumpAttack);
            playerManager.playerComponentManager.DisableCollisionWithEnemies();
        }

        public void HandleHeavyAttack(bool isCardAttack)
        {
            if (isCombatting || playerManager.thirdPersonController.Grounded == false)
            {
                return;
            }

            isLightAttacking = false;
            isHeavyAttacking = true;

            playerManager.playerWeaponsManager.HideShield();


            if (heavyAttackComboIndex > GetMaxHeavyCombo())
            {
                heavyAttackComboIndex = 0;
            }

            if (isCardAttack)
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashSpecialAttack);
            }
            else
            {
                if (heavyAttackComboIndex == 0)
                {
                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashHeavyAttack1);
                }
                else if (heavyAttackComboIndex == 1)
                {
                    playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashHeavyAttack2, 0.05f);
                }
            }

            playerManager.staminaStatManager.DecreaseHeavyAttackStamina();

            HandleAttackSpeed();

            heavyAttackComboIndex++;
        }

        int GetMaxHeavyCombo()
        {
            int maxCombo = 0;

            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon() ?? equipmentDatabase.unarmedWeapon;
            if (currentWeapon?.weaponClass != null)
            {
                maxCombo = currentWeapon.weaponClass.heavyAttackCombos - 1;
            }

            return maxCombo;
        }

        public bool CanLightAttack()
        {
            if (!this.isActiveAndEnabled)
            {
                return false;
            }

            if (CanAttack() == false)
            {
                return false;
            }

            if (equipmentDatabase.IsStaffEquipped() || equipmentDatabase.IsBowEquipped())
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForLightAttack();
        }

        public bool CanHeavyAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForHeavyAttack();
        }

        bool CanAttack()
        {
            if (playerManager.IsBusy())
            {
                return false;
            }

            if (playerManager.characterBlockController.isBlocking)
            {
                return false;
            }

            if (menuManager.isMenuOpen)
            {
                return false;
            }

            if (playerManager.playerShootingManager.isAiming)
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            if (playerManager.dodgeController.isDodging)
            {
                return false;
            }

            if (uIManager.IsShowingGUI())
            {
                return false;
            }

            if (playerManager.thirdPersonController.isSwimming)
            {
                return false;
            }

            return true;
        }

        private void OnDisable()
        {
            ResetStates();
        }


        public void HandlePlayerAttack(IDamageable damageable, Weapon weapon)
        {
            if (damageable is not DamageReceiver damageReceiver)
            {
                return;
            }

            if (playerManager.playerBlockController.isCounterAttacking)
            {
                playerManager.playerBlockController.onCounterAttack?.Invoke();
            }

            damageReceiver?.character?.health?.onDamageFromPlayer?.Invoke();

            if (weapon != null && damageReceiver?.character?.health?.weaponRequiredToKill != null && damageReceiver?.character?.health.weaponRequiredToKill == weapon)
            {
                damageReceiver.character.health.hasBeenHitWithRequiredWeapon = true;
            }
        }
        void ResetCombos()
        {
            if (ResetLightAttackComboIndexCoroutine != null)
            {
                StopCoroutine(ResetLightAttackComboIndexCoroutine);
            }

            ResetLightAttackComboIndexCoroutine = StartCoroutine(_ResetLightAttackComboIndex());
        }
        IEnumerator _ResetLightAttackComboIndex()
        {
            yield return new WaitForSeconds(maxIdleCombo);
            lightAttackMainWeaponComboIndex = 0;
            lightAttackSecondaryWeaponComboIndex = 0;
        }
    }
}
