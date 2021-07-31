using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticControlAttack : AutomaticControlMove
{
    /// <summary>円の半径、円の中に対象が入ったら攻撃開始</summary>
    [SerializeField] protected float m_attackStartRange = 5;
    /// <summary>攻撃対象が周囲にいないかサーチする範囲の半径(丸型)</summary>
    [SerializeField] private float m_targetSearchRange = 25;
    /// <summary>攻撃間隔(秒)</summary>
    [SerializeField] private float m_attackInterval = 2;
    /// <summary>攻撃対象をサーチする際に除外するタグ</summary>
    [SerializeField] private string[] m_targetSearchExclusionTags;
    
    protected NormalCharacterAnimationManager normalCharacterAnimationManager;
    protected TargetSearch _targetSearch;
    protected TeamMemberManager teamMemberManager;

    private List<string> attackTargetTags;
    private float _attackIntervalCount = 0;


    protected new void Start()
    {
        base.Start();

        normalCharacterAnimationManager = transform.parent.GetComponent<NormalCharacterAnimationManager>();
        teamMemberManager = transform.parent.parent.GetComponent<TeamMemberManager>();

        //攻撃対象のリストを作成する。※teamDataのものと違い、攻撃判定はあるがサーチからは除外するリスト
        attackTargetTags = new List<string>(teamMemberManager.ThisTeamData.AttackTargetTags.ToArray());
        for (int i = 0; i < m_targetSearchExclusionTags.Length; i++)
        {
            attackTargetTags.RemoveAll(tag => tag == m_targetSearchExclusionTags[i]);
        }
    }

    protected void FixedUpdate()
    {
        //時間の計測
        _attackIntervalCount += Time.deltaTime;
    }

    /// <summary>
    /// コントロールオブジェクトの周囲に攻撃対象がいないかサーチする。対象が複数いた場合でも一つのGameObjectを返す
    /// </summary>
    /// <returns>見つかったターゲットのBodyCollider、見つからない場合はnullを返す</returns>
    protected GameObject SearchForSurroundingTarget()
    {

        //サーチして見つけた最初のオブジェクトを返す
        foreach (var target in _targetSearch.SphereSearch(controlBody.transform.position, m_targetSearchRange, attackTargetTags.ToArray()))
        {
           return target.transform.Find("BodyCollider").gameObject;
        }

        return null;
    }
        
    /// <summary>
    /// 引数に用意された対象に向けて攻撃する。攻撃対象が攻撃範囲外の場合は攻撃しない
    /// </summary>
    /// <param name="attackBodyTransform"></param>
    protected void Attack(Transform attackBodyTransform)
    {
        //攻撃間隔の時間確認
        if(_attackIntervalCount >= m_attackInterval)
        {//攻撃の時間になっていたらカウントを0にして以下の攻撃処理に進める
            _attackIntervalCount = 0;
        }
        else
        {//攻撃する時間にまだ到達していない場合は以下の処理はせずに終了する
            return;
        }

        TurnToTheTarget(attackBodyTransform.position);

        //----------------攻撃対象が攻撃範囲内にいるか確認する----------------//
        //コントロールオブジェクトと攻撃対象との距離
        Vector3 distanceToTheAttackTarget = attackBodyTransform.position - controlBody.transform.position;
        //計算しやすいので方向をxz軸どちらも正にする。※距離は変わらないので問題はない
        distanceToTheAttackTarget.x = Mathf.Abs(distanceToTheAttackTarget.x);
        distanceToTheAttackTarget.z = Mathf.Abs(distanceToTheAttackTarget.z);
        distanceToTheAttackTarget.y =0;//Y軸は使わないので0
        //コントロールから攻撃範囲までの距離。
        Vector3 attackRange_Radius= distanceToTheAttackTarget.normalized * m_attackStartRange;
        //(範囲-対象)で範囲外でマイナス、範囲内でプラス
        //攻撃対象が攻撃範囲内の場合の処理
        if((attackRange_Radius.x - distanceToTheAttackTarget.x) >= 0 && (attackRange_Radius.z - distanceToTheAttackTarget.z) >= 0)
        {
            normalCharacterAnimationManager.AttackAnimation();
        }

    }

    /// <summary>
    /// 対象に向く
    /// </summary>
    /// <param name="targetPosition"></param>
    private void TurnToTheTarget(Vector3 targetPosition)
    {
        //向く方向の計算
       Vector3 forward = (targetPosition - controlBody.transform.position);

        forward.y = 0;

        //正面のベクトルを反映
        controlBody.transform.forward = forward.normalized;
    }

}
