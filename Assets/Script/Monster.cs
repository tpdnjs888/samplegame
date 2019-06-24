using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int maxHp;
    public SpriteRenderer sprite;
    
    private int hp;

    public void Hit(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Dying();
            return;
        }

        StartCoroutine(BlinkColor());
    }

    public bool IsDie()
    {
        return (hp <= 0);
    }

    public void Dying()
    {
        StopAllCoroutines();
        StartCoroutine(Die());
    }

    public void Spawn()
    {
        hp = maxHp;
        gameObject.SetActive(true);
    }

    private IEnumerator BlinkColor()
    {
        Color color = Color.white;
        float blinkValue = -0.1f;

        yield return new WaitForSeconds(0.7f);

        while (true)
        {
            color.r += blinkValue;
            sprite.color = color;

            if (sprite.color.r <= 0)
                blinkValue *= -1;
            else if (sprite.color.r >= 1)
                break;

            yield return null;
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.7f);
        sprite.color = Color.white;
        gameObject.SetActive(false);
    }
}
