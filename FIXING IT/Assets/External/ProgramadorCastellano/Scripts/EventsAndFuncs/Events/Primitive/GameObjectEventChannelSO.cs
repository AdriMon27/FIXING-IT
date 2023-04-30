using ProgramadorCastellano.Base;
using UnityEngine;
using UnityEngine.Events;

namespace ProgramadorCastellano.MyEvents
{
    [CreateAssetMenu(menuName = "Events/Primitive/GameObject Event Channel")]
    public class GameObjectEventChannelSO : DescriptionBaseSO, IMyEventSO<GameObject>
    {
        public UnityAction<GameObject> OnEventRaised { get; set; }

        public void RaiseEvent(GameObject gameObject)
        {
            if (OnEventRaised != null) {
                OnEventRaised.Invoke(gameObject);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameter {gameObject}");
            }
        }
    }
}