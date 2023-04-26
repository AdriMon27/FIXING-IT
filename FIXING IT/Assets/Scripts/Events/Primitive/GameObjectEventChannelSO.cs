using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Primitive/GameObject Event Channel")]
public class GameObjectEventChannelSO : DescriptionBaseSO
{
    public UnityAction<GameObject> OnEventRaised;

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
