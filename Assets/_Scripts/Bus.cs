using _Scripts.Node;
using DG.Tweening;
using Pandoras.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [field: SerializeField] public AnimalType CollectibleAnimalType { get; private set; }
    [field: SerializeField] public Transform[] Slots { get; private set; } = new Transform[MAX_COLLECT_AMOUNT];
    public int CurrentCollectIndex { get; private set; } = 0;

    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Animator busAnimator;



    private const int MAX_COLLECT_AMOUNT = 3;
    private List<bool> _isTweenAlive = new List<bool>();
    private GameManager _gameManager;
    private Sequence _seq;


    private void Start()
    {
        _gameManager = GameManager.Instance;

        for (int i = 0; i < MAX_COLLECT_AMOUNT; i++)
        {
            _isTweenAlive.Add(false);
        }
    }

    public void InitilizeBuss(AnimalType animalType, Material busMaterial)
    {
        CollectibleAnimalType = animalType;
        meshRenderer.material = busMaterial;
    }

    public void CollectAnimal(NodeObject collectObject, Action onBussFulled)
    {
        var jumpPos = Slots[CurrentCollectIndex].position;

        collectObject.gameObject.transform.SetParent(Slots[CurrentCollectIndex]);

        var dir = (jumpPos - collectObject.gameObject.transform.position);
        var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);

        if (_isTweenAlive.Contains(true))
        {
            _seq.Insert(0.1f * CurrentCollectIndex, JumpTween(collectObject, onBussFulled, jumpPos, CurrentCollectIndex));
        }
        else
        {
            _seq?.Kill();
            _seq = null;
            _seq = DOTween.Sequence();

            _seq.Append(JumpTween(collectObject, onBussFulled, jumpPos, CurrentCollectIndex));

        }

        CurrentCollectIndex += 1;
    }

    public bool IsFull()
    {
        return MAX_COLLECT_AMOUNT <= CurrentCollectIndex;
    }

    private Tween JumpTween(NodeObject collectObject, Action onBussFulled, Vector3 jumpPos, int tempCollectNum)
    {
        var jumpRot = new Vector3(0, 180, 0);
        _isTweenAlive[tempCollectNum] = true;

        return collectObject.gameObject.transform.DOLocalJump(Vector3.zero + Vector3.one * .25f, _gameManager.GameSettingsDataSO.CatJumpPower, 1, _gameManager.GameSettingsDataSO.CatJumpDuration).OnStart(() =>
         {
             collectObject.NodeObjectAnimatorController.TriggerCatJumpAnimation();

             var dir = (jumpPos - collectObject.gameObject.transform.position);
             var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);
             collectObject.gameObject.transform.rotation = Quaternion.Euler(Vector3.up * angle);

         }).OnComplete(() =>
         {
             collectObject.NodeObjectAnimatorController.TriggerCatIdleAnimation();
             collectObject.gameObject.transform.DORotate(jumpRot, _gameManager.GameSettingsDataSO.CatRotateDuration, RotateMode.Fast);
             _isTweenAlive[tempCollectNum] = false;

             if (CurrentCollectIndex >= MAX_COLLECT_AMOUNT && !_isTweenAlive.Contains(true))
             {
                 onBussFulled?.Invoke();
             }
         });
    }

    public void TriggerMoveAnimation()
    {
        busAnimator.SetBool(AnimatorVariableNames.Idle, false);
        busAnimator.SetTrigger(AnimatorVariableNames.Move);
    }

    public void TriggerIdleAnimation()
    {
        busAnimator.SetBool(AnimatorVariableNames.Idle, true);
    }
}