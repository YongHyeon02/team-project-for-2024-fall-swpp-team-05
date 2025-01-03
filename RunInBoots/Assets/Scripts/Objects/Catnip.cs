using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catnip : Interactable
{
    public int catnipID;

    private void Start()
    {
        //gameObject.name = "Catnip_" + catnipID;
        Initialize();
    }

    public override void Initialize()
    {
        if (GameManager.Instance.GetCurrentStageState()?.GetIsCatnipCollected()[catnipID - 1] ?? false)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void OnInteract(GameObject interactor)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        BattleModule battleModule = player.GetComponent<BattleModule>();
        TransformModule transformModule = player.GetComponent<TransformModule>();

        Animator playerAnimator = player.GetComponent<AnimatableUI>().animator;
        player.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_PLAYER_CATNIP);
        ProducingEvent catnipEvent = new AnimatorEvent(playerAnimator);
        catnipEvent.AddStartEvent(() =>
        {
            // Debug.Log("Catnip Event Start");
            transformModule.LookAhead();
            actionSystem.ResumeSelf(false);
            transform.position = player.transform.position + Vector3.up * 4.0f;
        });
        catnipEvent.AddEndEvent(() =>
        {
            actionSystem.ResumeSelf(true);
            battleModule.BeInvincible();
            OnCatnipCollected();
            transformModule.LookAhead(false);
            // Debug.Log("Catnip Event End");
        });
        GameManager.Instance.AddEvent(catnipEvent);
    }

    private void OnCatnipCollected()
    {
        
        GameManager.Instance.GetCurrentStageState().CollectCatnipInStageState(catnipID);
        gameObject.SetActive(false);
    }
}
