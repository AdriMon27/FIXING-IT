using FixingIt.SceneManagement.ScriptableObjects;
using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.Events
{
    [CreateAssetMenu(menuName = "Events/Complex/Load Scene Channel")]
    public class LoadSceneChannelSO : BaseEventChannelSO<GameSceneSO>
    {
    }
}