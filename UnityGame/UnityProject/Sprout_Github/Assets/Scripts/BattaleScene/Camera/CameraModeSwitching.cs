using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModeSwitching<T> : OverlookFromSkyCameraMode<T> where T : MonoBehaviour
{
    /// <summary>現在のフレームで切り替え(モードの)が発生したかどうか※正確にはCurrentCameraModeが呼び出されて切り替わり(既に切り替わっている場合はtrueにならない)CameraModeSwitchingクラスのFixedUpdateが呼び出されるまでtrueになる</summary>
    //public bool ModeSwitchingThisFlame { get; set; } = false;

    private CameraMode _currentCameraMode;
    /// <summary>現在のカメラモード</summary>
    public CameraMode CurrentCameraMode
    {
        set 
        {
            if (value != _currentCameraMode)
            {
                _currentCameraMode = value;

               // ModeSwitchingThisFlame = true;

                //カメラのモードに応じて正投影(真)遠近法(偽)を切り替える
                Camera.GetComponent<Camera>().orthographic = (CurrentCameraMode == CameraMode.Tracking || CurrentCameraMode == CameraMode.FirstPersonView) ? false : true;

                //表示レイヤーの変更
                ChangeDisplayLayerInMode(CurrentCameraMode);

                //カメラモードOverlookFromSkyに切り替わり時のみの処理。正投影サイズを(開始位置に)変更
                if (CurrentCameraMode == CameraMode.OverlookFromSky) Camera.GetComponent<Camera>().orthographicSize = m_overlookFromSky_orthographicSizeFluctuation.y;//現在はバグ回避用に変更
            }
        } 

        get { return _currentCameraMode;}
    }
    public enum CameraMode
    {
        Tracking = 0,
        FirstPersonView,
        OverlookFromSky
    }

    private void FixedUpdate()
    {
       // ModeSwitchingThisFlame = false;
    }

    /// <summary>
    /// カメラに回転を反映させる
    /// </summary>
    /// <param name="cameraMode"></param>
    protected void ReflectRotation(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.Tracking:
                Tracking_Rotation();
                return;
            case CameraMode.FirstPersonView:
                FirstPersonView_Rotation();
                return;
            case CameraMode.OverlookFromSky:
                OverlookFromSkyCamera_Rotation();
                return;
        }

        Debug.LogWarning("このルートに到達することは想定していません");
    }

    /// <summary>
    /// 目的の場所に移動するためのベクトルを返す
    /// </summary>
    /// <param name="cameraMode"></param>
    /// <returns></returns>
    protected Vector3 GetMoveVector(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.Tracking:
                return Tracking_Move();
            case CameraMode.FirstPersonView:
                return FirstPersonView_Move();
            case CameraMode.OverlookFromSky:
                return OverlookFromSkyCamera_Move();
        }

        Debug.LogError("このルートに到達することは想定していません");
        return Vector3.zero;
    }

    /// <summary>
    /// CurrentCameraModeに応じて表示するレイヤーを変更する
    /// </summary>
    /// <param name="cameraMode"></param>
    private void ChangeDisplayLayerInMode(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.Tracking:
                //FirstPersonViewで非表示にされたレイヤーを表示する。
                Camera.GetComponent<Camera>().cullingMask |= 1 << 10;
                break;

            case CameraMode.FirstPersonView:
                //ターゲットのレイヤーをカメラで非表示にする。
                Camera.GetComponent<Camera>().cullingMask = ~(1 << 10);
                break;

            case CameraMode.OverlookFromSky:
                //FirstPersonViewで非表示にされたレイヤーを表示する。
                Camera.GetComponent<Camera>().cullingMask |= 1 << 10;
                break;
        }
    }
}
