using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private BoxCollider2D _boxCollider;
        [SerializeField] private float _closingTime = 3f;
        public void Interact()
        {
            _boxCollider.enabled = false;
            StartCoroutine(CloseDoor());
        }

        IEnumerator CloseDoor()
        {
            yield return new WaitForSeconds(_closingTime);
            _boxCollider.enabled = true;
        }
    }
}