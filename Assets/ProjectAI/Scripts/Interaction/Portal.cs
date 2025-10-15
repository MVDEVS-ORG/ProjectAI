
using Assets.ProjectAI.Scripts.GameEvent;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.Interaction
{
    public class Portal : MonoBehaviour, IPortal
    {
        [Inject] private ISceneManager _sceneManager;


        [SerializeField] private string _targetSceneName;
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider2D _trigger;

        private bool _isActive = false;
        public bool IsActive => _isActive;

        private void Awake()
        {
            if (_trigger != null)
                _trigger.enabled = false;
        }

        public void Activate()
        {
            _isActive = true;
            if (_trigger != null) _trigger.enabled = true;
            Debug.LogError("Portal Activated");
            _animator?.Play("Portal Open");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isActive) return;

            if (!collision.CompareTag("Player")) return;

            _ = _sceneManager.LoadSceneAsync(_targetSceneName);
            _isActive = false;    
        }

        void OnEnable() => GameEvents.OnPortalKeyAcquired += Activate;
        void OnDisable() => GameEvents.OnPortalKeyAcquired -= Activate;
    }
}