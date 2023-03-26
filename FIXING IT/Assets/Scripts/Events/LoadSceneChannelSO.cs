using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Load Scene Channel")]
public class LoadSceneChannelSO : DescriptionBaseSO
{
    public UnityAction<GameSceneSO> OnEventRaised;

    public void RaiseEvent(GameSceneSO gameSceneSO)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(gameSceneSO);
        }
        else {
            Debug.LogError($"{errorMessage} with parameter {gameSceneSO}");
        }
    }
}
