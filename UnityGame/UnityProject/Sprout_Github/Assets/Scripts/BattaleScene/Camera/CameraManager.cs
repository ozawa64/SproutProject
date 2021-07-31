using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : CameraModeSwitching<CameraManager>
{
    private new void Awake()
    {
        base.Awake();

        //子にカメラがあること前提
        Camera = transform.Find("MainCamera").gameObject;
    }

    private void FixedUpdate()
    {
        //カメラのターゲットがいない場合は処理しない
        if (CameraTarget == null) return;

        //カメラの移動
        Camera.transform.position = GetMoveVector(CurrentCameraMode);

        //カメラの回転
        ReflectRotation(CurrentCameraMode);

        

    }

    /// <summary>
    /// マウスの移動入力
    /// </summary>
    /// <param name="inputAxis"></param>
    public void InputToCamera(Vector2 inputAxis)
    {
        _inputAxis = new Vector2(inputAxis.x, inputAxis.y);
    }

  

}
