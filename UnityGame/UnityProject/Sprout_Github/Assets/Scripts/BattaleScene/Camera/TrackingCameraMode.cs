using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingCameraMode<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{
    public Transform CameraTarget { get; set; } = null;
    public GameObject Camera { get; protected set; } 

    /// <summary>追尾時の移動速度</summary>
    [SerializeField] float m_TrackingSpeed=20;
    /// <summary>追尾時の移動速度</summary>
    [SerializeField] Vector2 m_TrackingCameraSensitivity = new Vector2(10,60);
    /// <summary>見上げる高さと見下す高さの比率</summary>
    [SerializeField] float m_LookUpAndDownRatio = -1.5f;
    /// <summary>追尾時のターゲットとの距離間</summary>
    [SerializeField] Vector2 m_TrackingDistance=new Vector2(15,6);

    protected Vector2 _inputAxis = Vector2.zero;

    private Vector2 _trackingAngle=Vector2.zero;

    /// <summary>
    /// 追尾時の移動後座標を返す。※速度計算済み
    /// </summary>
    /// <returns></returns>
    protected Vector3 Tracking_Move()
    {
        //マウス入力を角度変数に反映
        _trackingAngle.x += _inputAxis.x * m_TrackingCameraSensitivity.x;
        _trackingAngle.y += _inputAxis.y * m_TrackingCameraSensitivity.y;

        //角度値の調整
        if (_trackingAngle.y > 360) _trackingAngle.y = 360;
        if (_trackingAngle.y < -500) _trackingAngle.y = -500;
        if (_trackingAngle.x < 0) _trackingAngle.x += 360;
        if (_trackingAngle.x > 360) _trackingAngle.x -= 360;

        //角度からカメラの移動場所を計算
        float radian_xz = _trackingAngle.x * Mathf.Deg2Rad;
        Vector3 velocity_xz = new Vector3(Mathf.Cos(radian_xz), 0, Mathf.Sin(radian_xz));
        Vector3 movePoint = velocity_xz * m_TrackingDistance.x;

        //高さ
        movePoint.y = ((_trackingAngle.y / 360) - m_LookUpAndDownRatio) * m_TrackingDistance.y;

        //ターゲットを中心として計算する必要があるのでその座標を足す
        movePoint += CameraTarget.position;

        //移動後の座標を返す
        return Vector3.MoveTowards(Camera.transform.position, movePoint, m_TrackingSpeed);
    }

    /// <summary>
    /// 追尾時の回転。関数の実行で反映
    /// </summary>
    protected void Tracking_Rotation()
    {
      Camera.transform.LookAt(CameraTarget);
    }

   
    
}
