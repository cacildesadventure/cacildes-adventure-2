namespace AF
{
    using UnityEngine;
    using AF.Health;

    public class AttackStatManager : MonoBehaviour
    {

        [Header("Status attack bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalAttackBonus = 0f;

        [Header("Physical Attack")]
        public int basePhysicalAttack = 100;
        public float levelMultiplier = 2.55f;

        [Header("Buff Bonuses")]
        public ParticleSystem increaseNextAttackDamageFX;
        bool increaseNextAttackDamage = false;
        readonly float nextAttackMultiplierFactor = 1.3f;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;

        public void ResetStates() { }

        public bool HasBowEquipped()
        {
            return equipmentDatabase.IsBowEquipped();
        }

        public Damage GetAttackDamage()
        {
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();
            if (playerManager.playerCombatController.isAttackingWithLeftHand)
            {
                currentWeapon = equipmentDatabase.GetCurrentSecondaryWeapon();
            }
            else if (currentWeapon == null)
            {
                currentWeapon = equipmentDatabase.unarmedWeapon;
            }

            Damage damage = GetDamageForWeapon(currentWeapon);

            damage = GetNextAttackBonusDamage(damage);

            return damage;
        }

        Damage GetDamageForWeapon(Weapon weapon)
        {
            Damage weaponDamage = weapon.weaponClass.GetCurrentDamage(
                playerManager,
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence(),
                weapon.level);

            return playerManager.playerWeaponsManager.GetBuffedDamage(weaponDamage);
        }

        public int GetCurrentPhysicalAttackForGivenStrengthAndDexterity(int strength, int dexterity)
        {
            return (int)Mathf.Round(
                Mathf.Ceil(
                    basePhysicalAttack
                        + (strength * levelMultiplier)
                        + (dexterity * levelMultiplier)
                    )
                );
        }


        public int CompareWeapon(Weapon weaponToCompare)
        {
            if (equipmentDatabase.GetCurrentWeapon() == null)
            {
                return 1;
            }

            var weaponToCompareAttack = GetDamageForWeapon(weaponToCompare).GetTotalDamage();
            var currentWeaponAttack = GetDamageForWeapon(equipmentDatabase.GetCurrentWeapon()).GetTotalDamage();

            if (weaponToCompareAttack > currentWeaponAttack)
            {
                return 1;
            }

            if (weaponToCompareAttack == currentWeaponAttack)
            {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetBonusPhysicalAttack(int value)
        {
            physicalAttackBonus = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ResetBonusPhysicalAttack()
        {
            physicalAttackBonus = 0f;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetIncreaseNextAttackDamage(bool value)
        {
            increaseNextAttackDamage = value;
            SetBuffDamageFXLoop(value);
        }

        void SetBuffDamageFXLoop(bool isLooping)
        {
            var main = increaseNextAttackDamageFX.main;
            main.loop = isLooping;
        }

        Damage GetNextAttackBonusDamage(Damage damage)
        {
            if (increaseNextAttackDamage)
            {
                increaseNextAttackDamage = false;
                SetBuffDamageFXLoop(false);

                damage.physical = (int)(damage.physical * nextAttackMultiplierFactor);

                if (damage.fire > 0)
                {
                    damage.fire = (int)(damage.fire * nextAttackMultiplierFactor);
                }
                if (damage.frost > 0)
                {
                    damage.frost = (int)(damage.frost * nextAttackMultiplierFactor);
                }
                if (damage.lightning > 0)
                {
                    damage.lightning = (int)(damage.lightning * nextAttackMultiplierFactor);
                }
                if (damage.magic > 0)
                {
                    damage.magic = (int)(damage.magic * nextAttackMultiplierFactor);
                }
                if (damage.darkness > 0)
                {
                    damage.darkness = (int)(damage.darkness * nextAttackMultiplierFactor);
                }
                if (damage.water > 0)
                {
                    damage.water = (int)(damage.water * nextAttackMultiplierFactor);
                }
            }

            return damage;
        }

    }
}
