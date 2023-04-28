using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Load Scene Channel")]
public class LoadSceneChannelSO : DescriptionBaseSO,IMyEventSO<GameSceneSO>
{
    public UnityAction<GameSceneSO> OnEventRaised { get; set; }

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
