using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlookFromSkyCameraMode<T> : FirstPersonViewCameraMode<T> where T : MonoBehaviour
{
    [SerializeField] private float m_overlookingHeight = 300f;
    //[SerializeField] private float m_overlookFromSkyCameraInputMoveSpeed = 1;
    [SerializeField] private float m_overlookFromSkyCameraMoveSpeed = 1;
    /// <summary>正投影サイズの変動速度</summary>
    //[SerializeField] private float m_overlookFromSkyCameraOrthographicSizeSpeed = 5;
    /// <summary>モード切り替え時に正投影サイズをxからyに変動させる</summary>
    [SerializeField] protected Vector2 m_overlookFromSky_orthographicSizeFluctuation;
    /// <summary>上空から見下ろす際にターゲットのXZ軸を軸にどれだけずれて移動する事が出来るか</summary>
    [SerializeField] private Vector2 m_overlookFromSky_CameraRangeOfMotion;
    /// <summary>現在のずれ移動場所</summary>
    // private Vector2 _cameraMotionPosition;


    /// <summary>
    /// 上空から見渡す視点時の移動後座標を返す。※速度計算済み
    /// </summary>
    /// <returns></returns>
    protected Vector3 OverlookFromSkyCamera_Move()
    {
        //-----------OrthographicSizeの計算-----------//

        /*//バグがあるため処理しない(エラー:Screen position out of view frustum)。
        //正投影サイズを指定した速度分足す
        Camera.GetComponent<Camera>().orthographicSize += m_overlookFromSkyCameraOrthographicSizeSpeed;
        //足したサイズが上限を超えていたら調整する
        Camera.GetComponent<Camera>().orthographicSize= Mathf.Clamp(Camera.GetComponent<Camera>().orthographicSize, m_overlookFromSky_orthographicSizeFluctuation.x, m_overlookFromSky_orthographicSizeFluctuation.y);
        */
        //-----------transform.positionの計算-----------//

        //目的地
        Vector3 destination =Vector3.zero;

        //高さと軸となるXZ座標の代入
        destination = new Vector3(CameraTarget.transform.position.x, m_overlookingHeight, CameraTarget.transform.position.z);

        /*
        //入力の計算
        _cameraMotionPosition.x = _inputAxis.x* m_overlookFromSkyCameraInputMoveSpeed;
        _cameraMotionPosition.y = _inputAxis.y* m_overlookFromSkyCameraInputMoveSpeed;
        //入力で求めた場所が可動可能範囲か確認。範囲外で修正。
        _cameraMotionPosition.x = Mathf.Clamp(_cameraMotionPosition.x , -m_overlookFromSky_CameraRangeOfMotion.x , m_overlookFromSky_CameraRangeOfMotion.x);
        _cameraMotionPosition.y = Mathf.Clamp(_cameraMotionPosition.y , -m_overlookFromSky_CameraRangeOfMotion.y , m_overlookFromSky_CameraRangeOfMotion.y);
        //入力の反映
        destination.x += _cameraMotionPosition.x;
        destination.y += _cameraMotionPosition.y;
        */

        //移動後の座標を返す
        return Vector3.MoveTowards(Camera.transform.position, destination, m_overlookFromSkyCameraMoveSpeed);
    }

    /// <summary>
    /// 上空から見渡す時の回転。関数の実行で反映
    /// </summary>
    protected void OverlookFromSkyCamera_Rotation()
    {
        Camera.transform.eulerAngles = new Vector3(90,0,0);
    }
}
