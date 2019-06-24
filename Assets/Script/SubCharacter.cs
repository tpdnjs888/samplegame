using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCharacter : Character
{
    public void HelpMainCharacter(GameObject targetMonster)
    {
        StartAttack(targetMonster);
    }
}
