using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAnimationManager : CharacterAndBuildingCommonAnimation
{
    private Animator _animator;
    private BuildingDisappear _buildingDisappear;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _buildingDisappear = GetComponent<BuildingDisappear>();
    }

    private void FixedUpdate()
    {
        if (_buildingDisappear.Disappearing) _animator.SetBool("Disappear",true);
    }

    public override void WeakDamageAnimation()
    {
        _animator.SetTrigger("WeakDamage");
    }

    public override void HeavyDamageAnimation()
    {
        _animator.SetTrigger("HeavyDamage");
    }
}