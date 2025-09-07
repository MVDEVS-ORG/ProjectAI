using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.Interaction
{
    public interface IPortal
    {
        void Activate();
        bool IsActive {  get; }
    }
}