namespace AF
{
    using UnityEngine;

    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(AudioSource))]
    public class DamageEffect : MonoBehaviour
    {
        new ParticleSystem particleSystem => GetComponent<ParticleSystem>();
        AudioSource audioSource => GetComponent<AudioSource>();

        public void Play()
        {
            particleSystem.Play();
            audioSource.Play();
        }
    }
}
