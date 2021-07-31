using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveDirectionRotation : MonoBehaviour
{
    [SerializeField] Transform m_RotationTransform=null;

    private CharacterRunOrWalkMove _CharacterRunOrWalkMove;

    private void Start()
    {
        _CharacterRunOrWalkMove = GetComponent<CharacterRunOrWalkMove>();
    }

    private void FixedUpdate()
    {
        //回転させるTransformがない場合何もしない
        if (m_RotationTransform == null) return;

        //移動方向がない時回転しない
        if (_CharacterRunOrWalkMove.CurrentMovingVector == Vector3.zero) return;

        // 移動方向にオブジェクトを向ける
        m_RotationTransform.transform.forward = _CharacterRunOrWalkMove.InputMovingVector;


    }
}
