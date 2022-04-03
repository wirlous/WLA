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

    public void FinishDamageHandler()
    {
        GameReferences.player.EndDamage();
    }

    // Sword damage
    public void DoDamageSwordDownHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.DOWN) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.DOWN);
    }

    public void DoDamageSwordLeftHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.LEFT) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.LEFT);
    }

    public void DoDamageSwordRightHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.RIGHT) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.RIGHT);
    }

    public void DoDamageSwordUpHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.UP) return;
        GameReferences.player.DoDamage(WeaponType.SWORD, Direction.UP);
    }


    // Bow damage
    public void DoDamageBowDownHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.DOWN) return;
        GameReferences.player.DoDamage(WeaponType.BOW, Direction.DOWN);
    }

    public void DoDamageBowLeftHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.LEFT) return;
        GameReferences.player.DoDamage(WeaponType.BOW, Direction.LEFT);
    }

    public void DoDamageBowRightHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.RIGHT) return;
        GameReferences.player.DoDamage(WeaponType.BOW, Direction.RIGHT);
    }

    public void DoDamageBowUpHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.UP) return;
        GameReferences.player.DoDamage(WeaponType.BOW, Direction.UP);
    }


    // Magic damage
    public void DoDamageMagicDownHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.DOWN) return;
        GameReferences.player.DoDamage(WeaponType.MAGIC, Direction.DOWN);
    }

    public void DoDamageMagicLeftHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.LEFT) return;
        GameReferences.player.DoDamage(WeaponType.MAGIC, Direction.LEFT);
    }

    public void DoDamageMagicRightHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.RIGHT) return;
        GameReferences.player.DoDamage(WeaponType.MAGIC, Direction.RIGHT);
    }

    public void DoDamageMagicUpHandler()
    {
        Direction direction = GetDirection();
        if (direction != Direction.UP) return;
        GameReferences.player.DoDamage(WeaponType.MAGIC, Direction.UP);
    }


    // Other
    private Direction GetDirection()
    {
        float vertical   = animator.GetFloat("FacingVertical");
        float horizontal = animator.GetFloat("FacingHorizontal");

        // IMPORTANT: Vertical priority over Horizontal
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
