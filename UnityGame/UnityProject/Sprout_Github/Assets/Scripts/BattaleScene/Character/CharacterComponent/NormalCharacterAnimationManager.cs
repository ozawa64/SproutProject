using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCharacterAnimationManager : CharacterAndBuildingCommonAnimation
{
    private Animator _animator;
    private CharacterRunOrWalkMove _characterRunOrWalkMove;
    private CharacterJump _characterJump;
    private CharacterFall _characterFall;
    private AttackManager _attackManager;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterRunOrWalkMove = GetComponent<CharacterRunOrWalkMove>();
        _characterJump = GetComponent<CharacterJump>();
        _characterFall = GetComponent<CharacterFall>();
        _attackManager = GetComponent<AttackManager>();
    }

    private void FixedUpdate()
    {
        _animator.SetBool("Moveing", _characterRunOrWalkMove.Moveing);
        _animator.SetBool("Falling", _characterFall.Falling);
        _animator.SetBool("RemainingJump", _characterJump.WhetherJumpPossible());
    }

    /// <summary>
    /// 姿を消すアニメーションを実行する
    /// </summary>
    public void DisappearAnimation()
    {
        _animator.SetBool("Disappearing", true);
        _animator.SetTrigger("Disappear");
    }

    /// <summary>
    /// ジャンプアニメーションを実行。※こちら側でジャンプスクリプトに実行確認をとります
    /// </summary>
    public void JumpAnimation()
    {
        //ジャンプが出来るかジャンプスクリプトで確認。出来なければアニメーションも行わない
       if(_characterJump.WhetherJumpPossible())_animator.SetTrigger("Jump");
    }

    /// <summary>
    /// 攻撃アニメーションを実行。※こちら側で攻撃スクリプトに実行確認をとります
    /// </summary>
    public void AttackAnimation()
    {
        //攻撃処理を受け付けている時にアニメーションも実行する
     if(_attackManager.Suspension==false) _animator.SetTrigger("Attack");
    }

    /// <summary>
    /// アニメーションから変数を変更する用
    /// </summary>
    /// <param name="trueOrfalse"></param>
    public void NextAttackGraceTime(string trueOrfalse)
    {
        _animator.SetBool("NextAttackGraceTime", trueOrfalse=="true" ? true:false);
    }

    /// <summary>
    /// アニメーションから変数を変更する用
    /// </summary>
    /// <param name="trueOrfalse"></param>
    public void Attacking(string trueOrfalse)
    {
        _animator.SetBool("Attacking", trueOrfalse == "true" ? true : false);
    }

    /// <summary>
    /// アニメーションから変数を変更する用
    /// </summary>
    /// <param name="trueOrfalse"></param>
    public void CharacterRunOrWalkMoveSuspension(string trueOrfalse)
    {
        _characterRunOrWalkMove.Suspension = trueOrfalse == "true" ? true : false;
    }
}
