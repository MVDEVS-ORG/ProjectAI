using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.GameController
{
    public interface IBossRoomController
    {
        Task InitializeBossRoom();
    }
}