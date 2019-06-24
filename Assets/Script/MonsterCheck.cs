using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCheck : MonoBehaviour
{
    public MainCharacter mainBody;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
            mainBody.StartAttack(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Monster")
            mainBody.RemoveAttackWaitingMonsterList(collision.gameObject);
    }
}
