using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public static UIAnimator Instance;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void LevelChangePlayed()
    {
        Game.Instance.MoveToNextLevel(true);
    }
    public void ResetPlayed()
    {
        Game.Instance.MoveToNextLevel(false);

    }
    public void PlayLevelChange()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("LevelChange");
    }
    public void PlayLose()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Lose");
    }
}
