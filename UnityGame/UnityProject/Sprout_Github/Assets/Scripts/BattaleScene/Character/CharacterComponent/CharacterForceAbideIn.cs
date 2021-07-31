using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの入力方向以外のベクトルに抵抗を付けるクラス
/// </summary>
public class CharacterForceAbideIn : MonoBehaviour
{
    [SerializeField] Rigidbody m_ForceAbideInRigidbody=null;
    /// <summary>抵抗力</summary>
    [SerializeField] float m_Resist = 1;

    private CharacterRunOrWalkMove _CharacterRunOrWalkMove;

    private void Start()
    {
        _CharacterRunOrWalkMove = gameObject.GetComponent<CharacterRunOrWalkMove>();
    }

    private void FixedUpdate()
    {
        //抵抗を加えるRidigdBodyが存在しない場合何もしない
        if (m_ForceAbideInRigidbody == null) return;

        Resist();

    }

    /// <summary>
    /// 移動入力した方向の反対に力があった場合、抵抗する
    /// </summary>
    private void Resist()
    {
        Vector3 velocity = m_ForceAbideInRigidbody.velocity;

        m_ForceAbideInRigidbody.velocity = new Vector3(ResistConfirmation(velocity.x, _CharacterRunOrWalkMove.CurrentMovingVector.x), velocity.y, ResistConfirmation(velocity.z, _CharacterRunOrWalkMove.CurrentMovingVector.z));


        float ResistConfirmation(float v, float input)
        {
            if (input == 0)
            {
                return Mathf.MoveTowards(v, 0, m_Resist);
            }
            else if (input > 0)
            {
                if (v < 0) return Mathf.MoveTowards(v, 0, m_Resist);
            }
            else
            {
                if (v > 0) return Mathf.MoveTowards(v, 0, m_Resist);
            }

           
            return v;
        }

    }

    
}
