using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class NearDistanceDamageDealer : MonoBehaviour
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private Light2D _aura;
        [SerializeField] private float _auraIntensity = 1.41f;

        private bool _dealDamage = true;
        private Coroutine _auraCoroutine;

        private void Start()
        {
            StartCoroutine(AuraManager(0f, _auraIntensity));
            _dealDamage = true;
        }
        public void TurnOffAura()
        {
            if (_auraCoroutine != null)
                StopCoroutine(_auraCoroutine);

            _dealDamage = false;
            _auraCoroutine = StartCoroutine(AuraManager(_aura.intensity, 0f));
        }
        public void TurnOnAura()
        {
            if (_auraCoroutine != null)
                StopCoroutine(_auraCoroutine);

            _dealDamage = true;
            _auraCoroutine = StartCoroutine(AuraManager(_aura.intensity, _auraIntensity));
        }

        private IEnumerator AuraManager(float initialAura, float finalAura)
        {
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t);

                _aura.intensity = Mathf.Lerp(initialAura, finalAura, t);
                yield return null;
            }

            _aura.intensity = finalAura;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            DealDamageToTarget(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            DealDamageToTarget(collision);
        }

        private void DealDamageToTarget(Collider2D collision)
        {
            if (_dealDamage && collision.transform.CompareTag("Player"))
            {
                var playerHealth = collision.GetComponent<CharacterView>();
                playerHealth?.TakeDamage(_damage, transform.position, 1.2f);
            }
        }
    }
}