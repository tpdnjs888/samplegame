using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : Character
{
    public void CallSubCharacter(GameObject targetMonster)
    {
        List<SubCharacter> subChars = GameManager.instance.subChar;

        foreach (SubCharacter subChar in subChars)
        {
            subChar.StopAttack();
            subChar.StopMove();
            subChar.SetState(StateType.Idle);
            subChar.HelpMainCharacter(targetMonster);
        }
    }

    public override void StartAttack(GameObject targetMonster)
    {
        if (currentState == StateType.Attack)
        {
            if (attackWaitingMonster.Contains(targetMonster) == false)
                attackWaitingMonster.Add(targetMonster);
        }
        else
            CallSubCharacter(targetMonster);

        base.StartAttack(targetMonster);
    }

    public void RemoveAttackWaitingMonsterList(GameObject targetMonster)
    {
        attackWaitingMonster.Remove(targetMonster);
    }
}
