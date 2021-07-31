using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackTargetHitConfirmation : MonoBehaviour
{ 
    private TargetSearch _targetSearch;

    protected void Start()
    {
        _targetSearch = gameObject.SearchByTagName("SingletonManager", "TargetSearchs").GetComponent<TargetSearch>();
    }


    protected GameObject[] HitConfirmation_Sphere(Vector3 attackCenterPosition, float attackArea_Radius, string[] attackTags)
    {
       return _targetSearch.SphereSearch( attackCenterPosition, attackArea_Radius,  attackTags);
    }

    public GameObject[] HitConfirmation_Rectangle(Vector3 attackStartPosition, Vector3 attackArea, Transform attakRotation,string[] attackTags)
    {

      return  _targetSearch.RectangleSearch(attackStartPosition, attackArea, attakRotation, attackTags);
    }

}
