using FixingIt.SceneManagement;
using ProgramadorCastellano.MyEvents;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Complex/Load Scene Channel")]
public class LoadSceneChannelSO : BaseComplexEventChannelSO<GameSceneSO>
{
    //public UnityAction<GameSceneSO> OnEventRaised { get; set; }

    //public void RaiseEvent(GameSceneSO gameSceneSO)
    //{
    //    if (OnEventRaised != null) {
    //        OnEventRaised.Invoke(gameSceneSO);
    //    }
    //    else {
    //        Debug.LogWarning($"{errorMessage} with parameter {gameSceneSO}");
    //    }
    //}
}
