using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class Door : MonoBehaviour
    {

        [SerializeField] private BoxCollider2D _boxCollider;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _checkRadius = 3f;
        [SerializeField] private float _closeDelay = 2f;
        [SerializeField] private LayerMask _entityLayer;

        private bool _isOpen = false;
        private float _lastDetectedTime;
        private bool _isAnimating = false;

        private void Update()
        {
            // Detect if any entity is nearby
            Collider2D target = Physics2D.OverlapCircle(transform.position, _checkRadius, _entityLayer);

            if (target)
            {
                _lastDetectedTime = Time.time;
                if (!_isOpen && !_isAnimating)
                    OpenDoor();
            }
            else if (_isOpen && !_isAnimating && Time.time > _lastDetectedTime + _closeDelay)
            {
                CloseDoor();
            }
        }

        private void OpenDoor()
        {
            _isAnimating = true;
            _animator.SetBool("IsOpen", true);
            StartCoroutine(WaitForAnimationToEnd("DoorOpen"));
        }

        private void CloseDoor()
        {
            _isAnimating = true;
            _animator.SetBool("IsOpen", false);
            StartCoroutine(WaitForAnimationToEnd("DoorClose"));
        }

        private IEnumerator WaitForAnimationToEnd(string stateName)
        {
            // Wait until the animator is in the correct state
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                yield return null;

            // Wait until the animation has fully played
            float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);

            // Perform action after animation finishes
            if (stateName == "DoorOpen")
            {
                _isOpen = true;
                _boxCollider.enabled = false;
            }
            else if (stateName == "DoorClose")
            {
                _isOpen = false;
                _boxCollider.enabled = true;
            }

            _isAnimating = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _checkRadius);
        }
    }
}