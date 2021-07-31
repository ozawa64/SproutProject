using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerate<T> : PrefabListOfCharacters<T> where T : MonoBehaviour
{
    /// <summary>
    /// チームがどの陣営か
    /// </summary>
    public enum Camp : int
    {
        Player,
        Enemy,
    }

     /// <summary>
    /// 生成するオブジェクトのタイプ
    /// </summary>
    public enum CharacterType : int
    {
        CharacterTeam,
        FollowCharacter,
        FollowCharactersManager,
        NormalCharacter
    }

    /// <summary>
    /// キャラクター関係のオブジェクトを生成する
    /// </summary>
    /// <returns></returns>
    protected GameObject Generate(Camp camp, CharacterType characterType)
    {
        //チームオブジェクトを生成
        GameObject generateGo = Instantiate(PrefabGet(characterType + "_" + camp));

        //生成時名前にCloneがつくので消す
        generateGo.name = characterType + "_" + camp;

        //thisTransformを親オブジェクトに指定
        generateGo.transform.parent = transform;

        return generateGo;
    }
   
}
