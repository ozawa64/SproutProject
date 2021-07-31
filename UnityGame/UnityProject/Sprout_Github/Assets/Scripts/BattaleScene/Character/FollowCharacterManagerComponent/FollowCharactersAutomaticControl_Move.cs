using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharactersAutomaticControl_Move : MonoBehaviour
{
    /// <summary>許容する高低差。この高さの高低差ではジャンプしない</summary>
    [SerializeField] float m_ToleranceOfDifferenceOfElevation = 1;

    protected void CharacterMove(GameObject moveObject, Vector3 destination, bool jump = false)
    {
        //移動オブジェクトのBodyを取得
        GameObject moveObjectBody = moveObject.transform.Find("BodyCollider").gameObject;

        //移動するベクトル
        Vector3 moveVector = (destination - moveObjectBody.transform.position).normalized;
        
        //移動入力
        moveObject.GetComponent<CharacterRunOrWalkMove>().Move(moveVector.x, moveVector.z);

        //ジャンプ
       if(jump) moveObject.GetComponent<CharacterJump>().JumpInput();
    }

    /// <summary>
    /// ジャンプが必要か判断する
    /// </summary>
    /// <param name="jumpObject"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    protected bool CharacterCheckIfNecessaryJump(GameObject jumpObjectBody, Vector3 destination)
    {
        //目的地との高さの差
       float difference = destination.y  - jumpObjectBody.transform.position.y - m_ToleranceOfDifferenceOfElevation;

        //目的地の高さが0より大きい(ジャンプを必要とするy軸の差の)時は
        if (difference > 0) return true;

        return false;
    }


}
