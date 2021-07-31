using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : MonoBehaviour
{
    /// <summary>ジャンプ処理を一時停止</summary>
    public bool Suspension = false;

    /// <summary>ジャンプ中</summary>
    public bool Jumping { get; private set; } = false;

    [SerializeField] GameObject m_JumpObject;
    [SerializeField] GameObject m_JumpEffect;
    /// <summary>ジャンプ力の最大強さ</summary>
    [SerializeField] float m_MaximumPower = 20;
    /// <summary>ジャンプ時間(秒)</summary>
    [SerializeField] float m_JumpingTime = 0.25f;
    /// <summary>キャラクターの中心から下に伸びる(地面判定の)線、(1で足元、+で許容範囲)</summary>
    [SerializeField] float m_isGroundedLength = 1.1f;
    /// <summary>地面判定にするレイヤーマスク</summary>
    [SerializeField] LayerMask m_GroundLayerMask;

    private BasicStatusDataAccess _basicStatusDataAccess;
    /// <summary>ジャンプ時間(秒)の計測</summary>
    private float _jumpingTimeCount = 0;
    private Rigidbody _jumpObjectRigidbody;
    /// <summary>残りジャンプ回数を計測。地面に着くとリセット</summary>
    private int _jumpCount = 0;

    /// <summary>
    /// ジャンプのエフェクトを出す
    /// </summary>
    public void JumpEffectGenerate()
    {
        //エフェクトオブジェクトがない場合は処理を行わない
        if (m_JumpEffect == null) return;

       GameObject generateEffect= Instantiate(m_JumpEffect);

        generateEffect.transform.position = m_JumpEffect.transform.position;

        generateEffect.SetActive(true);
    }

    private void Awake()
    {
        _jumpingTimeCount = m_JumpingTime;
    }

    private void Start()
    {
        _basicStatusDataAccess = gameObject.GetComponent<BasicStatusDataAccess>();
        _jumpObjectRigidbody = m_JumpObject.GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        if(Suspension)
        {
            //現在のジャンプを終了させるため
            _jumpingTimeCount = m_JumpingTime;

            Jumping = false;

            //以下のジャンプ処理はしない
            return;
        }

        //ジャンプさせるオブジェクトがなければ何もしない
        if (m_JumpObject == null) return;


        //ジャンプ回数のリセット //条件:地面についていてジャンプが終了している
        if (IsGrounded() && !(_jumpingTimeCount <= m_JumpingTime)) _jumpCount = 0;

        //ジャンプ時間の計測
        _jumpingTimeCount += Time.deltaTime;

        //ジャンプ処理を有効にする時間かどうか
        if (_jumpingTimeCount <= m_JumpingTime)
        {
            Jumping = true;
        }
        else
        {
            Jumping = false;
        }

        //ジャンプ変数がtrueのときのみジャンプ処理をする
        if (Jumping) Jump();

        //お互いベクトルに直接干渉するため、ジャンプ中は停止させる
       // _characterFall.Suspension = Jumping;
    }

    public void JumpInput()
    {

        //ジャンプが出来ない状態の場合も処理をしない
        if (!WhetherJumpPossible()) return;

        //ジャンプを開始するので時間をリセット
        _jumpingTimeCount = 0;

        _jumpCount++;
    }

    /// <summary>
    /// ジャンプ中の処理
    /// </summary>
    private void Jump()
    {
        // Debug.Log("時間"+ _jumpingTimeCount+"_ベクトル"+ JumpVelocity());

        _jumpObjectRigidbody.velocity = new Vector3(_jumpObjectRigidbody.velocity.x, JumpVelocity(), _jumpObjectRigidbody.velocity.z);

        float JumpVelocity()
        {
            //二次関数 -2(x-m_JumpingTime)^2+m_MaximumPower

            float a = -1;

            //一回目以降のジャンプは空中ジャンプなので、空中用のジャンプ表現になるようにaの変数を調整
            if (_jumpCount > 1) a = -500;

            //(x-(m_JumpingTime))
            float square_OneDimensional = _jumpingTimeCount - m_JumpingTime;
            //(x-m_JumpingTime)^2
            float square = square_OneDimensional * square_OneDimensional;

            //(x-m_JumpingTime)^2 + m_MaximumPower
            float velocity = a * square + m_MaximumPower;

            return velocity;

        }


    }

    /// <summary>
    /// ジャンプが出来るか確認する。出来ればtrue
    /// </summary>
    /// <returns></returns>
    public bool WhetherJumpPossible()
    {
        //アクセスできない場合、ジャンプ回数が分からないのでジャンプ出来ない判定にする(falseを返す)
        if (!_basicStatusDataAccess.Access) return false;
        //残りジャンプ回数があるか
        if (!(_jumpCount < _basicStatusDataAccess.JumpFrequency)) return false;
        //既にジャンプ中
        if (Jumping) return false;

        //全てのif文を抜けると言う事はジャンプが可能と言う事なのでtrueを返す
        return true;


    }

    /// <summary>
    /// 地面に接触しているか判定する。接触時trueを返す。
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        //ジャンプオブジェクトのBodyコライダーを取得※カプセルコライダー以外を使用する場合は修正が必要
        CapsuleCollider bodyCollider = m_JumpObject.GetComponent<CapsuleCollider>();
        //体の中心の場所
        Vector3 centerBody = m_JumpObject.transform.position + bodyCollider.center;
        //体の中心から真下の足元の位置
        Vector3 underFeet = centerBody + Vector3.down * m_isGroundedLength;
        // 体の中心から足元にレイを飛ばし地面レイヤーに衝突したらTrue
        bool isGrounded = Physics.Linecast(centerBody, underFeet, m_GroundLayerMask);

        Debug.DrawLine(centerBody, underFeet);

        return isGrounded;
    }

}
