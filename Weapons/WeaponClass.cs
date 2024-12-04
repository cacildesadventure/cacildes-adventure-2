
namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Animations;
    using AF.Health;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Class")]
    public class WeaponClass : ScriptableObject
    {

        [Header("One Handing")]
        public List<AnimationOverride> baseAnimationOverrides;
        [Header("Two Handing")]
        public List<AnimationOverride> twoHandOverrides;
        [Header("Block")]
        public List<AnimationOverride> blockOverrides;
        [Header("Secondary Weapon")]
        public List<AnimationOverride> secondaryWeaponOverrides;

        [Header("Combo Count")]
        public int lightAttackCombos = 2;
        public int heavyAttackCombos = 1;

        [Header("Damage")]
        public Damage damage;

        [Header("Scaling")]
        public WeaponScaling strengthScaling = WeaponScaling.E;
        public WeaponScaling dexterityScaling = WeaponScaling.E;
        public WeaponScaling intelligenceScaling = WeaponScaling.E;

        [Header("Upgrades")]
        public float[] upgradeLevelBonus;

        public bool IsRangeWeapon() => damage.weaponAttackType == WeaponAttackType.Range;
        public bool IsMagicStave() => damage.weaponAttackType == WeaponAttackType.Staff;

        float GetUpgradeLevel(int weaponLevel)
        {
            if (upgradeLevelBonus.Length <= 0)
            {
                return 1;
            }

            return upgradeLevelBonus[weaponLevel];
        }

        public Damage GetCurrentDamage(
            PlayerManager playerManager,
            // Must be passed as parameters because we might be previewing raised stats from the level up screen
            int playerStrength, int playerDexterity, int playerIntelligence,
            int weaponLevel)
        {
            return new(
                   physical: GetWeaponAttack(WeaponElementType.Physical, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   fire: GetWeaponAttack(WeaponElementType.Fire, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   frost: GetWeaponAttack(WeaponElementType.Frost, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   magic: GetWeaponAttack(WeaponElementType.Magic, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   lightning: GetWeaponAttack(WeaponElementType.Lightning, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   darkness: GetWeaponAttack(WeaponElementType.Darkness, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   water: GetWeaponAttack(WeaponElementType.Water, playerManager, playerStrength, playerDexterity, playerIntelligence, weaponLevel),
                   postureDamage: GetWeaponPostureDamage(playerManager, weaponLevel),
                   poiseDamage: GetWeaponPoiseDamage(playerManager, weaponLevel),
                   weaponAttackType: damage.weaponAttackType,
                   statusEffects: GetWeaponStatusEffectsDamage(playerManager, weaponLevel),
                   pushForce: GetWeaponPushForceDamage(playerManager, weaponLevel),
                   canNotBeParried: damage.canNotBeParried,
                   ignoreBlocking: damage.ignoreBlocking
               );
        }

        int GetWeaponPostureDamage(PlayerManager playerManager, int weaponLevel)
        {
            int currentPostureDamage = (int)(damage.postureDamage * GetUpgradeLevel(weaponLevel));

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
            int currentPoiseDamage = (int)(damage.poiseDamage * GetUpgradeLevel(weaponLevel));

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

        int GetWeaponPushForceDamage(PlayerManager playerManager, int weaponLevel)
        {
            int currentPushForceDamage = (int)(damage.pushForce * GetUpgradeLevel(weaponLevel));

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return (int)(currentPushForceDamage * CombatSettings.heavyAttackMultiplier);
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return (int)(currentPushForceDamage * CombatSettings.jumpAttackMultiplier);
            }

            return currentPushForceDamage;
        }

        StatusEffectEntry[] GetWeaponStatusEffectsDamage(PlayerManager playerManager, int weaponLevel)
        {
            List<StatusEffectEntry> effects = new();

            foreach (StatusEffectEntry status in damage.statusEffects)
            {
                int newAmountPerHit = (int)(status.amountPerHit * GetUpgradeLevel(weaponLevel));

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

        public int GetStrengthScalingBonus(int playerStrength)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.STRENGTH, strengthScaling, playerStrength);

        public int GetDexterityScalingBonus(int playerDexterity)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.DEXTERITY, dexterityScaling, playerDexterity);

        public int GetIntelligenceScalingBonus(int playerIntelligence)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.INTELLIGENCE, intelligenceScaling, playerIntelligence);

        int GetScalingBonus(int playerStrength, int playerDexterity, int playerIntelligence)
            => GetStrengthScalingBonus(playerStrength) + GetDexterityScalingBonus(playerDexterity) + GetIntelligenceScalingBonus(playerIntelligence);

        public int GetWeaponAttack(
            WeaponElementType element, PlayerManager playerManager,
            int playerStrength, int playerDexterity, int playerIntelligence, int weaponLevel)
        {
            int elementAttack = 0;
            if (element == WeaponElementType.Physical) elementAttack = damage.physical;
            if (element == WeaponElementType.Fire) elementAttack = damage.fire;
            if (element == WeaponElementType.Frost) elementAttack = damage.frost;
            if (element == WeaponElementType.Magic) elementAttack = damage.magic;
            if (element == WeaponElementType.Lightning) elementAttack = damage.lightning;
            if (element == WeaponElementType.Darkness) elementAttack = damage.darkness;
            if (element == WeaponElementType.Water) elementAttack = damage.water;

            int currentAttack = (int)(elementAttack * GetUpgradeLevel(weaponLevel));

            return (int)(
                (currentAttack
                    + GetScalingBonus(playerStrength, playerDexterity, playerIntelligence)
                    + GetFaithBonuses(playerManager)
                    + GetAttackBonuses(playerManager)
                ) * GetAttackMultipliers(playerManager));
        }

        int GetFaithBonuses(PlayerManager playerManager)
        {
            int currentReputation = playerManager.statsBonusController.GetCurrentReputation();

            if (damage.lightning > 0 || damage.darkness > 0)
            {
                return (int)Math.Min(100, Mathf.Pow(Mathf.Abs(currentReputation), 1.25f)); ;
            }

            return 0;
        }

        public string GetFormattedStatusDamages(PlayerManager playerManager, int weaponLevel)
        {
            string result = "";

            foreach (var statusEffect in GetWeaponStatusEffectsDamage(playerManager, weaponLevel))
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
            weapon.weaponClass.damage.weaponAttackType switch
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

    }
}
