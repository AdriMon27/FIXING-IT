using FixingIt.Events;
using FixingIt.RoomObjects.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.CharacterCanvas
{
    public class CharacterCanvas : MonoBehaviour
    {
        private const string SHOW_REACTION = "ShowReaction";

        [SerializeField] private Image _reactionImage;

        [Header("Reaction Sprites")]
        [SerializeField] private Sprite _confusedSprite;
        [SerializeField] private Sprite _happySprite;

        [Header("Listening To")]
        [SerializeField]
        private RoomObjectParentChannelSO _confusedRoomObjectParentEvent;
        [SerializeField]
        private RoomObjectParentChannelSO _customerWithObjectFixedEvent;

        private Animator _animator;
        private IRoomObjectParent _owner;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _owner = GetComponentInParent<IRoomObjectParent>();
        }

        private void OnEnable()
        {
            _confusedRoomObjectParentEvent.OnEventRaised += ShowConfusedReaction;
            _customerWithObjectFixedEvent.OnEventRaised += ShowHappyReaction;
        }

        private void OnDisable()
        {
            _confusedRoomObjectParentEvent.OnEventRaised -= ShowConfusedReaction;
            _customerWithObjectFixedEvent.OnEventRaised -= ShowHappyReaction;
        }

        private void ShowReaction()
        {
            _animator.SetTrigger(SHOW_REACTION);
        }

        private void ShowConfusedReaction(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent != _owner)
                return;

            _reactionImage.sprite = _confusedSprite;
            ShowReaction();
        }

        private void ShowHappyReaction(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent != _owner)
                return;

            _reactionImage.sprite = _happySprite;
            ShowReaction();
        }
    }
}
