using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEventHandler : MonoBehaviour
{

    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FinishDamageSwordHandler()
    {
        GameReferences.player.EndDamageSword();
    }

    public void DoDamageSwordUpHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.UP) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.UP);
    }

    public void DoDamageSwordDownHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.DOWN) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.DOWN);
    }

    public void DoDamageSwordRightHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.RIGHT) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.RIGHT);
    }

    public void DoDamageSwordLeftHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.LEFT) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.LEFT);
    }


    private Direction GetDirection()
    {
        float vertical   = animator.GetFloat("FacingVertical");
        float horizontal = animator.GetFloat("FacingHorizontal");

        // IMPORTANT: Vertical priority over Horizontal (math animation)
        if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal))
        {
            // Vertical attack
            if (vertical > 0)
                return Direction.UP;
            else
                return Direction.DOWN;
        }
        else
        {
            // Horizontal attack
            if (horizontal > 0)
                return Direction.RIGHT;
            else
                return Direction.LEFT;
        }
    }

}
