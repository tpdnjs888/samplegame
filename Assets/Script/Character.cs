using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum StateType
    {
        Idle,
        Move,
        ChangeMoveTarget,
        Attack,
    }

    public enum AttackType
    {
        Melee,
        ADC,
    }

    public float speed;
    public float attackDistance;
    public float attackDelay;
    public int attackDamage;

    public StateType currentState = StateType.Idle;
    public AttackType attackType;
    public SkeletonAnimation ani;
    public Rigidbody2D rigid2D;
    public Monster currentTargetMonster = null;
    public List<GameObject> attackWaitingMonster = new List<GameObject>();

    private IEnumerator MoveCoroutine = null;
    private IEnumerator AttackCoroutine = null;

    public IEnumerator Move(Vector3 target)
    {
        currentState = StateType.Move;
        if(ani.AnimationName != "03_run")
            ani.AnimationState.SetAnimation(0, "03_run", true);

        Vector3 rotation = transform.eulerAngles;
        if (target.x > transform.position.x)
            rotation.y = 180;
        else
            rotation.y = 0;

        transform.rotation = Quaternion.Euler(rotation);

        while (true)
        {
            Vector3 movePosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            rigid2D.MovePosition(movePosition);

            if (Vector2.Distance(transform.position, target) <= float.Epsilon)
                break;

            if(currentState != StateType.Move)
                yield break;

            yield return null;
        }

        currentState = StateType.Idle;
        ani.AnimationState.SetAnimation(0, "01_Idle", true);
    }
    
    public IEnumerator Attack()
    {
        if (currentTargetMonster == null)
            yield break;

        currentState = StateType.Attack;

        Vector3 rotation = transform.eulerAngles;
        if (currentTargetMonster.transform.position.x > transform.position.x)
            rotation.y = 180;
        else
            rotation.y = 0;

        transform.rotation = Quaternion.Euler(rotation);

        while (true)
        {
            Vector3 movePosition = Vector3.MoveTowards(transform.position, currentTargetMonster.transform.position, speed * Time.deltaTime);
            rigid2D.MovePosition(movePosition);

            if(Vector2.Distance(transform.position, currentTargetMonster.transform.position) <= attackDistance)
                break;

            yield return null;
        }

        float attackTime = attackDelay;
        while (true)
        {
            if (currentState != StateType.Attack || currentTargetMonster.IsDie())
                break;

            if(attackTime < attackDelay)
            {
                attackTime += Time.deltaTime;
                yield return null;
                continue;
            }

            attackTime = 0f;
            if (attackType == AttackType.Melee)
                ani.AnimationState.SetAnimation(0, "05_attack", false);
            else if (attackType == AttackType.ADC)
                ani.AnimationState.SetAnimation(0, "06_cast", false);

            currentTargetMonster.Hit(attackDamage);
            yield return null;
        }

        yield return new WaitForSeconds(0.7f);
        currentState = StateType.Idle;

        if (attackWaitingMonster.Count != 0)
        {
            GameObject nextMonster = attackWaitingMonster[0];
            StartAttack(nextMonster);
            attackWaitingMonster.Remove(nextMonster);
        }
        else
            ani.AnimationState.SetAnimation(0, "01_Idle", true);
    }

    public void StartMove(Vector3 target)
    {
        StopAttack();
        attackWaitingMonster.Clear();
        MoveCoroutine = Move(target);
        StartCoroutine(MoveCoroutine);
    }

    public void StopMove()
    {
        if (MoveCoroutine == null)
            return;

        StopCoroutine(MoveCoroutine);
    }

    public virtual void StartAttack(GameObject targetMonster)
    {
        if (currentState == StateType.Attack)
            return;

        StopMove();
        currentTargetMonster = targetMonster.GetComponent<Monster>();
        AttackCoroutine = Attack();
        StartCoroutine(AttackCoroutine);
    }

    public void StopAttack()
    {
        if (AttackCoroutine == null)
            return;

        StopCoroutine(AttackCoroutine);
    }

    public void SetState(StateType state)
    {
        currentState = state;
    }
}
