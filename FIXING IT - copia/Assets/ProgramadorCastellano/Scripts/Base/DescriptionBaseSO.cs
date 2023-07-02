using UnityEngine;

namespace ProgramadorCastellano.Base
{
    /// <summary>
    /// Base class for ScriptableObjects that need a public description field.
    /// </summary>
    public class DescriptionBaseSO : ScriptableObject
    {
        [SerializeField, TextArea] string _description;

        protected string errorMessage => $"Someone Raise the event {name} but nobody was Listening";
    }
}