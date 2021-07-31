using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharactersAutomaticControlManager : FollowCharactersAutomaticControl_Move
{
    private FollowCharactersManager _followCharactersManager;

    private void Start()
    {
        _followCharactersManager = GetComponent<FollowCharactersManager>();
    }

    private void FixedUpdate()
    {
        
        for (int i = 0; i < _followCharactersManager.DisplayCharacters.Length; i++)
        {
            //移動するキャラクターと目的地、どちらもnullではない時
            if (_followCharactersManager.DisplayCharacters[i] != null && _followCharactersManager.Arrangements[i] != null)

                //キャラクターの移動とジャンプ命令
                CharacterMove(_followCharactersManager.DisplayCharacters[i], (Vector3)_followCharactersManager.Arrangements[i], CharacterCheckIfNecessaryJump(_followCharactersManager.DisplayCharacters[i].transform.Find("BodyCollider").gameObject, (Vector3)_followCharactersManager.Arrangements[i]));
        }
        
    }


}
