
namespace AF
{
    using System.Collections.Generic;
    using AF.Animations;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Animation")]
    public class WeaponAnimation : ScriptableObject
    {
        [Header("One Handing")]
        public List<AnimationOverride> oneHandAnimationOverrides;
        [Header("Two Handing")]
        public List<AnimationOverride> twoHandOverrides;
        [Header("Block")]
        public List<AnimationOverride> blockOverrides;
        [Header("Dual Wielding")]
        public List<AnimationOverride> dualWieldingOverrides;

        [Header("Combo Count")]
        public int lightAttackCombos = 2;
        public int heavyAttackCombos = 1;
    }
}
