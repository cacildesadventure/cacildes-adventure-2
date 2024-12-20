namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Health;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Damage")]
    public class WeaponDamage : ScriptableObject
    {
        [Header("Upgrades")]
        public WeaponUpgradeLevel[] weaponUpgradeLevels;

        public bool IsRangeWeapon(int weaponLevel) => GetCurrentDamage(weaponLevel).weaponAttackType == WeaponAttackType.Range;
        public bool IsMagicStave(int weaponLevel) => GetCurrentDamage(weaponLevel).weaponAttackType == WeaponAttackType.Staff;

        public WeaponUpgradeLevel GetWeaponUpgradeLevel(int level)
        {
            if (weaponUpgradeLevels == null || weaponUpgradeLevels.Length <= 0)
            {
                throw new Exception($"No weapon level assigned to weapon damage {this.name}");
            }

            return weaponUpgradeLevels[level];
        }

        Damage GetCurrentDamage(int weaponLevel)
        {
            return GetWeaponUpgradeLevel(weaponLevel)?.damage;
        }

        public Damage GetCurrentDamage(
            PlayerManager playerManager,
            // Must be passed as parameters because we might be previewing raised stats from the level up screen
            int playerStrength,
            int playerDexterity,
            int playerIntelligence,
            Weapon currentWeapon)
        {
            Damage currentDamage = GetCurrentDamage(currentWeapon.level);

            return new(
                   physical: GetWeaponAttack(WeaponElementType.Physical, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   fire: GetWeaponAttack(WeaponElementType.Fire, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   frost: GetWeaponAttack(WeaponElementType.Frost, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   magic: GetWeaponAttack(WeaponElementType.Magic, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   lightning: GetWeaponAttack(WeaponElementType.Lightning, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   darkness: GetWeaponAttack(WeaponElementType.Darkness, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   water: GetWeaponAttack(WeaponElementType.Water, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeapon),
                   postureDamage: GetWeaponPostureDamage(playerManager, currentWeapon.level),
                   poiseDamage: GetWeaponPoiseDamage(playerManager, currentWeapon.level),
                   weaponAttackType: currentDamage.weaponAttackType,
                   statusEffects: GetWeaponStatusEffectsDamage(playerManager, currentWeapon),
                   pushForce: GetWeaponPushForceDamage(playerManager, currentWeapon.level),
                   canNotBeParried: currentDamage.canNotBeParried,
                   ignoreBlocking: currentDamage.ignoreBlocking
               );
        }

        int GetWeaponPostureDamage(PlayerManager playerManager, int weaponLevel)
        {
            int currentPostureDamage = GetCurrentDamage(weaponLevel).postureDamage;

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return (int)(currentPostureDamage * CombatSettings.twoHandingMultiplier);
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return (int)(currentPostureDamage * CombatSettings.jumpAttackMultiplier);
            }

            return currentPostureDamage;
        }

        int GetWeaponPoiseDamage(PlayerManager playerManager, int weaponLevel)
        {
            int currentPoiseDamage = GetCurrentDamage(weaponLevel).poiseDamage;

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return (int)(currentPoiseDamage * CombatSettings.twoHandingMultiplier);
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return (int)(currentPoiseDamage * CombatSettings.jumpAttackMultiplier);
            }

            return currentPoiseDamage;
        }

        float GetWeaponPushForceDamage(PlayerManager playerManager, int weaponLevel)
        {
            float currentPushForceDamage = GetCurrentDamage(weaponLevel).pushForce;

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return currentPushForceDamage * CombatSettings.heavyAttackMultiplier;
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return currentPushForceDamage * CombatSettings.jumpAttackMultiplier;
            }

            return currentPushForceDamage;
        }

        StatusEffectEntry[] GetWeaponStatusEffectsDamage(PlayerManager playerManager, Weapon currentWeapon)
        {
            List<StatusEffectEntry> effects = new();

            Damage currentDamage = GetCurrentDamage(currentWeapon.level);
            if (currentDamage == null)
            {
                return new List<StatusEffectEntry>().ToArray();
            }

            foreach (StatusEffectEntry status in currentDamage.statusEffects)
            {
                int newAmountPerHit = (int)status.amountPerHit;

                if (playerManager.playerCombatController.isHeavyAttacking)
                {
                    newAmountPerHit = (int)(newAmountPerHit * CombatSettings.heavyAttackMultiplier);
                }

                if (playerManager.playerCombatController.isJumpAttacking)
                {
                    newAmountPerHit = (int)(newAmountPerHit * CombatSettings.jumpAttackMultiplier);
                }

                effects.Add(new StatusEffectEntry() { statusEffect = status.statusEffect, amountPerHit = newAmountPerHit });
            }

            return effects.ToArray();
        }

        public int GetStrengthScalingBonus(int playerStrength, Weapon currentWeapon)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.STRENGTH, currentWeapon.strengthScaling, playerStrength);

        public int GetDexterityScalingBonus(int playerDexterity, Weapon currentWeapon)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.DEXTERITY, currentWeapon.dexterityScaling, playerDexterity);

        public int GetIntelligenceScalingBonus(int playerIntelligence, Weapon currentWeapon)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.INTELLIGENCE, currentWeapon.intelligenceScaling, playerIntelligence);

        int GetScalingBonus(int playerStrength, int playerDexterity, int playerIntelligence, Weapon currentWeapon)
            => GetStrengthScalingBonus(playerStrength, currentWeapon) + GetDexterityScalingBonus(playerDexterity, currentWeapon) + GetIntelligenceScalingBonus(playerIntelligence, currentWeapon);

        public int GetWeaponAttack(
            WeaponElementType element, PlayerManager playerManager,
            int playerStrength, int playerDexterity, int playerIntelligence, Weapon currentWeapon)
        {
            int elementAttack = 0;

            Damage currentDamage = GetCurrentDamage(currentWeapon.level);

            if (element == WeaponElementType.Physical) elementAttack = currentDamage.physical;
            if (element == WeaponElementType.Fire) elementAttack = currentDamage.fire;
            if (element == WeaponElementType.Frost) elementAttack = currentDamage.frost;
            if (element == WeaponElementType.Magic) elementAttack = currentDamage.magic;
            if (element == WeaponElementType.Lightning) elementAttack = currentDamage.lightning;
            if (element == WeaponElementType.Darkness) elementAttack = currentDamage.darkness;
            if (element == WeaponElementType.Water) elementAttack = currentDamage.water;

            return (int)(
                (elementAttack
                    + GetScalingBonus(playerStrength, playerDexterity, playerIntelligence, currentWeapon)
                    + GetFaithBonuses(playerManager, currentDamage)
                    + GetAttackBonuses(playerManager)
                ) * GetAttackMultipliers(playerManager));
        }

        int GetFaithBonuses(PlayerManager playerManager, Damage currentDamage)
        {
            int currentReputation = playerManager.statsBonusController.GetCurrentReputation();

            if (currentDamage.lightning > 0 || currentDamage.darkness > 0)
            {
                return (int)Math.Min(100, Mathf.Pow(Mathf.Abs(currentReputation), 1.25f)); ;
            }

            return 0;
        }

        public string GetFormattedStatusDamages(PlayerManager playerManager, Weapon currentWeapon)
        {
            string result = "";

            foreach (var statusEffect in GetWeaponStatusEffectsDamage(playerManager, currentWeapon))
            {
                if (statusEffect != null)
                {
                    result += $"+{statusEffect.amountPerHit} {statusEffect.statusEffect.GetName()} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Inflicted per Hit")}\n";
                }
            }

            return result.TrimEnd();
        }

        private int GetAttackBonuses(PlayerManager playerManager)
        {
            int bonusValue = 0;

            // Reputation-based attack bonus
            if (HasAccessory(playerManager, x => x.increaseAttackPowerTheLowerTheReputation))
            {
                int reputation = playerManager.statsBonusController.GetCurrentReputation();
                if (reputation < 0)
                {
                    int extraAttackPower = Mathf.Min(CombatSettings.maxReputationAttackBonus, (int)(Mathf.Abs(reputation) * CombatSettings.reputationMultiplier));
                    bonusValue += extraAttackPower;
                }
            }

            // Health-based attack bonus
            if (HasAccessory(playerManager, x => x.increaseAttackPowerWithLowerHealth))
            {
                float healthMultiplier = (playerManager.health as PlayerHealth)?.GetExtraAttackBasedOnCurrentHealth() ?? 0;
                int extraAttackPower = (int)(bonusValue * healthMultiplier);
                bonusValue += extraAttackPower;
            }

            // Generic accessory bonuses
            bonusValue += playerManager.equipmentDatabase.accessories
                .Where(x => x != null)
                .Sum(x => x.physicalAttackBonus);

            // Guard counter and parry bonuses
            if (playerManager.characterBlockController.IsWithinCounterAttackWindow())
            {
                float counterMultiplier = playerManager.characterBlockController.counterAttackMultiplier;
                bonusValue = (int)(bonusValue * counterMultiplier);
            }

            return bonusValue;
        }

        private bool HasAccessory(PlayerManager playerManager, Func<Accessory, bool> condition) =>
            playerManager.equipmentDatabase.accessories.Any(x => x != null && condition(x));


        private float GetWeaponTypeBonus(Weapon weapon, PlayerManager playerManager) =>
            weapon.weaponDamage.GetCurrentDamage(weapon.level).weaponAttackType switch
            {
                WeaponAttackType.Pierce => playerManager.statsBonusController.pierceDamageMultiplier,
                WeaponAttackType.Slash => playerManager.statsBonusController.slashDamageMultiplier,
                WeaponAttackType.Blunt => playerManager.statsBonusController.bluntDamageMultiplier,
                _ => 0
            };

        private float GetAttackMultipliers(PlayerManager playerManager)
        {
            // Multipliers based on weapon and state
            float attackMultiplier = 1;

            if (playerManager.equipmentDatabase.isTwoHanding)
            {
                attackMultiplier += CombatSettings.twoHandingMultiplier + playerManager.statsBonusController.twoHandAttackBonusMultiplier;
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                attackMultiplier += CombatSettings.jumpAttackMultiplier;
            }

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                attackMultiplier += CombatSettings.heavyAttackMultiplier;
            }

            Weapon currentWeapon = playerManager.equipmentDatabase.GetCurrentWeapon();

            if (currentWeapon == null)
            {
                if (playerManager.statsBonusController.increaseAttackPowerWhenUnarmed)
                {
                    attackMultiplier += CombatSettings.unarmedAttackMultiplier;
                }
            }
            else
            {
                attackMultiplier += GetWeaponTypeBonus(currentWeapon, playerManager);
            }

            return attackMultiplier;
        }

        public bool HasHolyDamage(int weaponLevel) => GetCurrentDamage(weaponLevel).lightning != 0;
        public bool HasHexDamage(int weaponLevel) => GetCurrentDamage(weaponLevel).darkness != 0;
    }
}
