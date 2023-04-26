using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Load Scene Channel")]
public class LoadSceneChannelSO : DescriptionBaseSO
{
    public UnityAction<GameSceneSO> OnEventRaised;

    public void RaiseEvent(GameSceneSO gameSceneSO)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(gameSceneSO);
        }
        else {
            Debug.LogWarning($"{errorMessage} with parameter {gameSceneSO}");
        }
    }
}
