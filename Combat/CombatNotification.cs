namespace AF
{
    using AF.Health;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class CombatNotification : MonoBehaviour
    {
        public CharacterBaseManager character;

        public TextMeshPro textMeshPro => GetComponent<TextMeshPro>();

        public float initialScale = 0.85f;
        public float popupDuration = 0.3f;
        public Ease popupEase = Ease.OutBack;

        public float maxDuration = 2f;
        private float currentDuration = 0f;

        private Tween fadeTween;
        private Tween scaleTween;

        void Awake()
        {
            character.damageReceiver.onDamageEvent += OnDamageEvent;

            textMeshPro.text = "";
            this.gameObject.SetActive(false);
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            if (damage != null)
            {
                ShowDamage(damage);
            }

            return damage;
        }

        public void ShowDamage(Damage damage)
        {
            this.gameObject.SetActive(false);
            currentDuration = 0f;
            textMeshPro.alpha = 1f;

            textMeshPro.text = GetDamageText(damage);

            transform.localScale = Vector3.one * initialScale;
            scaleTween = transform.DOScale(Vector3.one, popupDuration)
                                  .SetEase(popupEase);

            this.gameObject.SetActive(true);

            fadeTween?.Kill();
            fadeTween = textMeshPro.DOFade(0f, maxDuration * 0.5f)
                                  .SetDelay(maxDuration * 0.5f)
                                  .SetEase(Ease.InQuad)
                                  .OnComplete(() =>
                                  {
                                      this.gameObject.SetActive(false);
                                  });
        }

        private void OnDisable()
        {
            fadeTween?.Kill();
            scaleTween?.Kill();
        }

        private void FaceCamera()
        {
            if (Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
        }

        private void Update()
        {
            FaceCamera();
        }

        string GetDamageText(Damage damage)
        {
            string text = "";

            if (damage.physical > 0)
            {
                text += $"<color=#DD1A5B>-{damage.physical}</color>";
            }

            if (damage.fire > 0)
            {
                text += $" <color=#FFB631>-{damage.fire}</color>";
            }

            if (damage.frost > 0)
            {
                text += $" <color=#3884E7>-{damage.frost}</color>";
            }

            if (damage.lightning > 0)
            {
                text += $" <color=#FFEF5F>-{damage.lightning}</color>";
            }

            if (damage.magic > 0)
            {
                text += $" <color=#F160FF>-{damage.magic}</color>";
            }

            if (damage.darkness > 0)
            {
                text += $" <color=#522A9C>-{damage.darkness}</color>";
            }

            if (damage.water > 0)
            {
                text += $" <color=#1CFFCD>-{damage.water}</color>";
            }

            return text;
        }
    }
}
