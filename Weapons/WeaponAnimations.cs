namespace AF
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Animations")]
    public class WeaponAnimations : ScriptableObject
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
    }
}
