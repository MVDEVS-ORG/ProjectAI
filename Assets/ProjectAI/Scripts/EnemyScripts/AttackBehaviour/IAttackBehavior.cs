using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    public interface IAttackBehavior
    {
        void Execute(EnemyAI enemy, ObjectPoolManager op);
        bool CanExecute(EnemyAI enemy);
        void ResetState();
    }
}