using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFall : MonoBehaviour
{
    /// <summary>落下処理を一時停止</summary>
    public bool Suspension = false;
    /// <summary>落下中かどうか ※重力が働いているかどうかではない</summary>
    public bool Falling { get; private set; } = false;

    [SerializeField] Rigidbody m_FallRigidbody= null;

    [SerializeField] float m_FallSpeed=40;
    /// <summary>キャラクターの中心から下に伸びる(地面判定の)線、(1で足元、+で許容範囲)</summary>
    [SerializeField] float m_isGroundedLength = 1.1f;
    /// <summary>地面判定にするレイヤーマスク</summary>
    [SerializeField] LayerMask m_GroundLayerMask;

    private float _maxSpeed = 50f;

    private void FixedUpdate()
    {
        Falling = !IsGrounded();

        if (Suspension) return;

        //落下させるオブジェクトのRigidbodyがなければ何もしない
        if (m_FallRigidbody == null) return;

        GiveFallSpeed();

        MaxFallSpeedRetouch();
    }

    private void GiveFallSpeed()
    {
        m_FallRigidbody.AddForce(0,-m_FallSpeed,0);
    }

    /// <summary>
    /// 落下速度の限界を越えていた場合に限界速度に抑える
    /// </summary>
    private void MaxFallSpeedRetouch()
    {
        if (m_FallRigidbody.velocity.y < (-_maxSpeed)) m_FallRigidbody.velocity = new Vector3(m_FallRigidbody.velocity.x, -_maxSpeed, m_FallRigidbody.velocity.z);
    }

    /// <summary>
    /// 地面に接触しているか判定する。接触時trueを返す。
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        //ジャンプオブジェクトのBodyコライダーを取得※カプセルコライダー以外を使用する場合は修正が必要
        CapsuleCollider bodyCollider = m_FallRigidbody.GetComponent<CapsuleCollider>();
        //体の中心の場所
        Vector3 centerBody = m_FallRigidbody.transform.position + bodyCollider.center;
        //体の中心から真下の足元の位置
        Vector3 underFeet = centerBody + Vector3.down * m_isGroundedLength;
        // 体の中心から足元にレイを飛ばし地面レイヤーに衝突したらTrue
        bool isGrounded = Physics.Linecast(centerBody, underFeet, m_GroundLayerMask);

        Debug.DrawLine(centerBody, underFeet);

        return isGrounded;
    }
}
