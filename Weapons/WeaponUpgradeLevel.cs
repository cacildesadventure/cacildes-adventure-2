namespace AF
{
    using AF.Health;
    using AYellowpaper.SerializedCollections;

    [System.Serializable]
    public class WeaponUpgradeLevel
    {
        public int goldCostForUpgrade;
        public Damage damage;
        public SerializedDictionary<UpgradeMaterial, int> upgradeMaterials;

    }
}
