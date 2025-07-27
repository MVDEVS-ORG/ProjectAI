using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IEnemyState
{
    private EnemyAI _enemy;
    private ObjectPoolManager _objectPoolmanager;
    public void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _enemy = enemy;
        _objectPoolmanager = op;


        // Set animator trigger or bool to play death animation
        _enemy.animator.SetTrigger("Dead");

        // Start coroutine to wait for animation and then release
        _enemy.StartCoroutine(PlayDeathAndRelease());
    }

    private IEnumerator PlayDeathAndRelease()
    {
        float timer = 0f;
        float maxWaitTime = 5f; // fallback in case animation never plays

        // Wait until animation starts
        while (!_enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && timer < maxWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Wait until animation finishes
        while (_enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && timer < maxWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _enemy.ResetEnemyAI();
        _objectPoolmanager.ReleaseGameObject(_enemy.gameObject, ObjectPoolManager.PoolType.Enemies);
    }

    public void Update() { }
    public void Exit() 
    {
        _enemy.animator.Rebind();
        _enemy.animator.Update(0);
        _enemy = null;
        _objectPoolmanager = null;
    }
}
