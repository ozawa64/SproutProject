using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticControlManager : AutomaticControlAttack
{
    protected GridData gridData;
    /// <summary>次のグリッドに進む際に建物があった場合、コントロールオブジェクトの中心よりどれくらい高いと破壊するか</summary>
    [SerializeField] private float m_nextGridBuildingAttackHeight;
    /// <summary>攻撃対象の建物のレイヤーマスク</summary>
    [SerializeField] private LayerMask m_AttackBuildingsLayer;
    private CharacterRunOrWalkMove _characterRunOrWalkMove;
    private StageBuildingData _stageBuildingData;
    private CharacterTeamManager _characterTeamManager;
    

 
   

    protected new void Start()
    {
        base.Start();

        _characterRunOrWalkMove = transform.parent.GetComponent<CharacterRunOrWalkMove>();
        gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();
        _stageBuildingData = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<StageBuildingData>();
        _targetSearch = gameObject.SearchByTagName("SingletonManager", "TargetSearchs").GetComponent<TargetSearch>();
        _characterTeamManager = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamManager>();

    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        //優先順位の高いGatherToLeader命令があればそれを実行していく
        bool existGatherToLeaderPurpose = false;
        foreach (var purpose in teamMemberManager.Purposes)
        {
            if (purpose.purposeEnum == TeamPurposeList.PurposeEnum.GatherToLeader)
            {
                existGatherToLeaderPurpose = true;
                break;
            }
        }

        //優先順位の高いGatherToLeader命令があるか
        if (existGatherToLeaderPurpose)
        {

            //陣営リーダーがいればその場所を最終目的地に設定する
            if(teamMemberManager.LeaderTeamData.LeaderObject!=null)SettingTheMovingLocation(teamMemberManager.LeaderTeamData.LeaderObject.transform.Find("BodyCollider").transform);

            //リーダーに合流したらリーダーのチームに入る(統合する)
            if (controlBody.transform.position.CheckIfItIsWithinTheRange(teamMemberManager.LeaderTeamData.LeaderObject.transform.Find("BodyCollider").transform.position, m_arrivalJudgmentRange))
            {
                _characterTeamManager.TeamIntegration(ref teamMemberManager.ThisTeamData, ref teamMemberManager.LeaderTeamData);
            }
        }
        else
        {
            //チームの目的(命令された仕事)があるか
            if (teamMemberManager.PurposeIsThere())
            {
                //命令リストは追加された順に実行していく。
                //命令の内容によって制御を分けてく。
                switch (teamMemberManager.Purposes[0].purposeEnum)
                {
                    case TeamPurposeList.PurposeEnum.Standby:
                    case TeamPurposeList.PurposeEnum.AttackTarget:
                    case TeamPurposeList.PurposeEnum.Architecture:
                        SettingTheMovingLocation(teamMemberManager.Purposes[0].destinationTf);
                        break;
                }
            }
            else
            {
                //チームの攻撃対象がいなければ探し代入
                if (teamMemberManager.AttentionTarget == null)
                {
                    GameObject searchTarget = SearchForSurroundingTarget();

                    if (searchTarget != null) teamMemberManager.AttentionTarget = searchTarget.transform.parent.gameObject;
                }

                //目的がない時はチームのターゲットになっているオブジェクトを追いかけ攻撃する
                if (teamMemberManager.AttentionTarget != null)
                {
                    SettingTheMovingLocation(teamMemberManager.AttentionTarget.transform.Find("BodyCollider"));
                    Attack(teamMemberManager.AttentionTarget.transform.Find("BodyCollider"));
                }
            }
        }

        //道のりの間にある建物を破壊して通るか
        if (NextGridAttackCheck())
        {
            Attack(_stageBuildingData.GameObjectDataGet(((int x, int y, int z))NextGrid).transform.Find("BodyCollider"));
        }

        //-----------------------入力-----------------------//
        //移動入力を出す
        Vector3? movingVector = MovingVector();
        if (movingVector == null)
        {
            //移動入力方向が計算出来ない場合は0を渡す
            _characterRunOrWalkMove.Move(0,0);
        }
        else
        {
            _characterRunOrWalkMove.Move(movingVector.Value.x, movingVector.Value.z);
        }
    }


    /// <summary>
    /// 次に目指すべき場所に建物があった場合にそれを壊すかどうか
    /// </summary>
    /// <returns>壊すtrue,無視false</returns>
    private bool NextGridAttackCheck()
    {

        //次に目指すグリッドがなく、壊す判断が出来ないのでfalse
        if (NextGrid == null) return false;

        //破壊対象のオブジェクトを取得
        GameObject targetBodyOfDestruction = _stageBuildingData.GameObjectDataGet(((int x, int y, int z))NextGrid);
        //破壊対象のオブジェクトの親オブジェクトがnullではなかったら当たり判定のあるBodyColliderオブジェクトを取得する
        if (targetBodyOfDestruction!=null) targetBodyOfDestruction=targetBodyOfDestruction.transform.Find("BodyCollider").gameObject;

        //壊す建物がないのでfalse
        if (targetBodyOfDestruction == null) return false;

        //破壊対象のオブジェクトが攻撃対象ではない場合はfalseを返して処理を終了する
        bool attackTagExist = false;
        foreach (var attackTag in teamMemberManager.ThisTeamData.AttackTargetTags)
        {
            if (attackTag == targetBodyOfDestruction.tag)
            {
                attackTagExist = true;
                break;
            }
        }
        if (attackTagExist == false) return false;

        //コントロールオブジェクトのBodyコライダーを取得※カプセルコライダー以外を使用する場合は修正が必要
        CapsuleCollider bodyCollider = controlBody.GetComponent<CapsuleCollider>();
        //体の中心の場所
        Vector3 centerBody = controlBody.transform.position + bodyCollider.center;
        //攻撃判定の開始高さと位置
        Vector3 attackHeight = centerBody + Vector3.up * m_nextGridBuildingAttackHeight;
        //攻撃判定の飛ばす先のベクトル
        Vector3 flyVector = (targetBodyOfDestruction.transform.position -controlBody.transform.position).normalized;
        flyVector.y = 0;//判定は平行であってほしいのでY軸は0にする
        // コントロールオブジェクトの上(attackHeight)から攻撃範囲の半径の長さで攻撃対象に向けて(Y軸はattackHeight)Linecastを飛ばす。衝突してtrueを出したら攻撃する必要がある
        bool attack = Physics.Raycast(attackHeight, flyVector, m_attackStartRange, m_AttackBuildingsLayer);
        Debug.DrawLine(centerBody, attackHeight);
        Debug.DrawRay(attackHeight, flyVector * m_attackStartRange);

        return attack;
    }

}
