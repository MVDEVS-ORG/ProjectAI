using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.Interfaces
{
    public interface IDungeonGenerator
    {
        Awaitable GenerateDungeon();
    }
}