using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonViewCameraMode<T> : TrackingCameraMode<T> where T : MonoBehaviour
{
    [SerializeField] private Vector3 m_firstPersonViewDifferenceFromCenter = Vector3.zero;
    [SerializeField] private float m_firstPersonViewRotationSpeed = 3.5f;

    /// <summary>
    /// 一人称視点時の移動後座標を返す。※速度計算済み
    /// </summary>
    /// <returns></returns>
    protected Vector3 FirstPersonView_Move()
    {
        //ターゲットの中心ポジションからの差分(視点の位置)を足す
        return CameraTarget.transform.position + m_firstPersonViewDifferenceFromCenter;
    }

    /// <summary>
    /// 一人称視点時の回転。関数の実行で反映
    /// </summary>
    protected void FirstPersonView_Rotation()
    {
        //回転方向と速度を求める
        Vector2 moveAngle = new Vector2(_inputAxis.y, _inputAxis.x);
        moveAngle = moveAngle.normalized * m_firstPersonViewRotationSpeed;

        //X軸の回転
        Vector3 cameraLocalAngle = Camera.transform.localEulerAngles;
        //X軸回転の移動角度の反転
        cameraLocalAngle.x += -moveAngle.x;
        //限界X軸角度の調整
        if (cameraLocalAngle.x > 80 && cameraLocalAngle.x < 180) cameraLocalAngle.x = 80;
        if (cameraLocalAngle.x < 280 && cameraLocalAngle.x > 180) cameraLocalAngle.x = 280;
        //X軸角度の反映
        Camera.transform.localEulerAngles = cameraLocalAngle;

        //Y軸回転
        Vector3 angle = Camera.transform.eulerAngles;
        //Y軸の移動角度を足す
        angle.y += moveAngle.y;
        //Y軸の角度の反映
        Camera.transform.eulerAngles = angle;
    }


}
