namespace AF
{
    using System.Collections;
    using UnityEngine;
    using AF.Health;

    public class EV_DamagePlayer : EventBase
    {
        public Damage damage;

        PlayerManager _playerManager;

        public override IEnumerator Dispatch()
        {
            GetPlayerManager().damageReceiver.ApplyDamage(null, damage);

            yield return null;
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null)
            {
                _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);

            }

            return _playerManager;
        }
    }
}
