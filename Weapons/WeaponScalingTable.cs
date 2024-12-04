namespace AF
{
    using System;

    public static class WeaponScalingTable
    {

        // Scaling values as constants
        private const float ScalingE = 0f;
        private const float ScalingD = 1.05f;
        private const float ScalingC = 1.4f;
        private const float ScalingB = 1.85f;
        private const float ScalingA = 2f;
        private const float ScalingS = 2.4f;

        /// <summary>
        /// Retrieves the scaling value for the specified key.
        /// </summary>
        /// <param name="scaling">The scaling category (e.g., E, D, C, B, A, S).</param>
        /// <returns>The scaling multiplier.</returns>
        static float GetScaling(WeaponScaling scaling)
        {
            return scaling switch
            {
                WeaponScaling.E => ScalingE,
                WeaponScaling.D => ScalingD,
                WeaponScaling.C => ScalingC,
                WeaponScaling.B => ScalingB,
                WeaponScaling.A => ScalingA,
                WeaponScaling.S => ScalingS,
                _ => throw new ArgumentOutOfRangeException(nameof(scaling), $"Invalid scaling: {scaling}")
            };
        }

        static float ATTRIBUTE_LEVEL_MULTIPLIER = 1.25f;

        public static float GetScalingBonus(AttributeType attribute, WeaponScaling weaponScaling, int currentAttributeLevel)
        {
            if (attribute == AttributeType.STRENGTH)
            {
                return currentAttributeLevel * ATTRIBUTE_LEVEL_MULTIPLIER * GetScaling(weaponScaling);
            }

            if (attribute == AttributeType.DEXTERITY)
            {
                return currentAttributeLevel * ATTRIBUTE_LEVEL_MULTIPLIER * GetScaling(weaponScaling);
            }

            if (attribute == AttributeType.INTELLIGENCE)
            {
                return currentAttributeLevel * ATTRIBUTE_LEVEL_MULTIPLIER * GetScaling(weaponScaling);
            }

            return 0;
        }
    }
}
