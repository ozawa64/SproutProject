using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRunOrWalkMove : MonoBehaviour
{
    /// <summary>移動入力を一時停止(処理はされる)</summary>
    public bool Suspension = false;
    /// <summary>移動中</summary>
    public bool Moveing { get; private set; } = false;
    /// <summary>移動させるオブジェクトのRigidbody</summary>
    [SerializeField] Rigidbody m_MoveRigidbody=null;
    /// <summary>カメラの向きで移動方向が変化する</summary>
    [SerializeField] public bool RelatedToCameraOrientation { get; set; }= false;
    /// <summary>現在の移動ベクトル</summary>
    public Vector3 CurrentMovingVector { get; private set; } = Vector3.zero; 
    /// <summary>入力された移動ベクトル</summary>
    public Vector3 InputMovingVector { get; private set; } = Vector3.zero;
    
    private BasicStatusDataAccess _basicStatusDataAccess;
    /// <summary>横移動</summary>
    private float _horizontal;
    /// <summary>縦移動</summary>
    private float _vertical;

    private void Awake()
    {
        
        _basicStatusDataAccess = gameObject.GetComponent<BasicStatusDataAccess>();
    }

    /// <summary>
    /// 指定した方向に(Rigidbodyで)移動する。※毎フレーム呼び出しを推奨
    /// </summary>
    public void Move(float horizontal, float vertical)
    {
        if (Suspension)
        {
            Moveing = false;

            //入力が反映されないようにする
            horizontal = 0;
            vertical = 0;
        }

        //速度関連のデータにアクセス出来ないと移動処理が出来ないのでFalseの場合はなにもしない
        if (!_basicStatusDataAccess.Access) return;

        
        _horizontal = horizontal;
        _vertical = vertical;

        CurrentMovingVector= MovingVector();

        //速度調整前(入力)のベクトルを代入
        InputMovingVector = CurrentMovingVector;

        CurrentMovingVector =MovingVectorMaxSpeedRetouch(CurrentMovingVector);

        //RigidBodyに反映する
        m_MoveRigidbody.AddForce(CurrentMovingVector,ForceMode.Acceleration);

        //移動ベクトルがゼロでfalse(停止)違う場合はtrue(移動中)
        Moveing = (CurrentMovingVector == Vector3.zero) ?false :true;
    }

    /// <summary>
    /// 移動ベクトルを求める
    /// </summary>
    /// <returns></returns>
    private Vector3 MovingVector()
    {
        //移動方向
        Vector3 moveDirection = (Vector3.right * _horizontal + Vector3.forward * _vertical).normalized;

        //移動方向(入力)がない場合は0を返す
        if (moveDirection == Vector3.zero) return Vector3.zero;

        //ベクトルをカメラに合わせる
        if (RelatedToCameraOrientation)
        {
            //カメラの回転を正面としてベクトルを求める
            moveDirection = Camera.main.transform.TransformDirection(moveDirection);

            moveDirection.y = 0;
        }

        //速度をかけて、移動ベクトルにする
        return moveDirection.normalized * _basicStatusDataAccess.MoveSpeed;
    }

    /// <summary>
    /// 移動させているオブジェクトのベクトルが上限速度を超過していた場合は移動ベクトルを修正して返す。
    /// </summary>
    private Vector3 MovingVectorMaxSpeedRetouch( Vector3 movingVector)
    {

        return new Vector3(ZeroAtExcessSpeed(m_MoveRigidbody.velocity.x,movingVector.x), 0, ZeroAtExcessSpeed(m_MoveRigidbody.velocity.z, movingVector.z));

        float ZeroAtExcessSpeed(float rBSpeed,float movingVector_float)
        {
            //速度が超過していた場合は0を返す
            if (rBSpeed != Mathf.Clamp(rBSpeed, -_basicStatusDataAccess.MoveMaxSpeed, _basicStatusDataAccess.MoveMaxSpeed)) return 0;

            return movingVector_float;
        }

    }


}
