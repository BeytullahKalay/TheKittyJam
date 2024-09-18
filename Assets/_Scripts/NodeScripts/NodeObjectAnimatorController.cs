using UnityEngine;
namespace _Scripts.Node
{
    public class NodeObjectAnimatorController : MonoBehaviour
    {
        [SerializeField] private NodeObject nodeObject;
        [field: SerializeField] public Animator CatAnimator { get; private set; }
        [field: SerializeField] public Animator DogAnimator { get; private set; }
        [field: SerializeField] public Animator FoxAnimator { get; private set; }

        private void OnEnable()
        {
            nodeObject.OnPathStepCompleted += CheckAnimatorStatesToIdle;
            nodeObject.OnCurrentPathChanged += SetState;
            nodeObject.OnCollected += OnCollected;
        }

        private void OnDisable()
        {
            nodeObject.OnPathStepCompleted -= CheckAnimatorStatesToIdle;
            nodeObject.OnCurrentPathChanged -= SetState;
            nodeObject.OnCollected -= OnCollected;
        }

        public void TriggerCatPunchAnimation()
        {
            // if its a cat
            if (nodeObject.AnimalType != AnimalType.NONE)
            {
                CatAnimator.SetBool(AnimatorVariableNames.Idle, false);
                CatAnimator.SetTrigger(AnimatorVariableNames.Punch);
            }
        }

        public void TriggerCatIdleAnimation()
        {
            // if its a cat
            if (nodeObject.AnimalType != AnimalType.NONE)
                CatAnimator.SetBool(AnimatorVariableNames.Idle, true);
        }

        public void TriggerCatJumpAnimation()
        {
            // if its a cat
            if (nodeObject.AnimalType != AnimalType.NONE)
            {
                CatAnimator.SetBool(AnimatorVariableNames.Idle, false);
                CatAnimator.SetTrigger(AnimatorVariableNames.Jump);
            }
        }

        public void TriggerDogWalk()
        {
            DogAnimator.SetBool(AnimatorVariableNames.Idle, false);
            DogAnimator.SetTrigger(AnimatorVariableNames.Move);
        }

        public void TriggerCatWalkAnimation()
        {
            CatAnimator.SetBool(AnimatorVariableNames.Idle, false);
            CatAnimator.SetTrigger(AnimatorVariableNames.Move);
        }

        private void SetAnimationStateIdle()
        {
            // if its a cat
            if (nodeObject.AnimalType != AnimalType.NONE)
                CatAnimator.SetBool(AnimatorVariableNames.Idle, true);
            else // should be fox then
                FoxAnimator.SetBool(AnimatorVariableNames.Idle, true);
        }

        private void CheckAnimatorStatesToIdle()
        {
            if (nodeObject.PathDataQueue.Count > 0) return;

            SetAnimationStateIdle();
        }

        private void SetState(AnimationType animationType)
        {
            if (nodeObject.IsCollected) return;

            CatAnimator.SetBool(AnimatorVariableNames.Idle, false);


            if (animationType == AnimationType.Jump)
            {
                CatAnimator.SetTrigger(AnimatorVariableNames.Jump);
                return;
            }

            if (nodeObject.AnimalType != AnimalType.NONE)
            {
                CatAnimator.SetTrigger(AnimatorVariableNames.Move);
                return;
            }

            FoxAnimator.SetTrigger(AnimatorVariableNames.Move);
        }

        private void OnCollected()
        {
            SetAnimationStateIdle();
        }
    }
}
