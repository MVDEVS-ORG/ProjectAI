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

        _enemy.animator?.SetTrigger("Dead");

        _enemy.StartCoroutine(PlayDeathAndRelease());
    }

    private IEnumerator PlayDeathAndRelease()
    {
        float timer = 0f;
        float maxWaitTime = 5f;
        _ = SpawnXPParticles();
        while (!_enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && timer < maxWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

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
        _enemy.animator?.Rebind();
        _enemy.animator?.Update(0);
        _enemy = null;
        _objectPoolmanager = null;
    }

    private async Awaitable SpawnXPParticles()
    {
        int numberOfParticles = Random.Range(1, 5);
        int XpPerParticle = (int)_enemy.enemyModel.Xp / numberOfParticles;
        int RemainingXP = _enemy.enemyModel.Xp - (XpPerParticle * numberOfParticles);
        for (int i = 0; i <= numberOfParticles; i++)
        {
            GameObject particle = await _objectPoolmanager.SpawnObjectAsync(AddressableIds.Experience_Particle_Metal_Small, _enemy.transform.position, Quaternion.identity, ObjectPoolManager.PoolType.ParticleSystems);
            XPParticle xp = particle.GetComponent<XPParticle>();
            if (i == numberOfParticles)
            {
                xp.Initialize(_objectPoolmanager, RemainingXP);
            }
            else
            {
                xp.Initialize(_objectPoolmanager, XpPerParticle);
            }
        }
    }
}
