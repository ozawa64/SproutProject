using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharactersBasicStatusDataSet : MonoBehaviour
{
    /// <summary>
    /// 使用されていないインデックス番号を返す
    /// </summary>
    /// <param name="displayCharacters"></param>
    /// <param name="teamData"></param>
    /// <returns>返すインデックス番号がなければnullを返す</returns>
    protected int? UnusedBasicStatusDataIndex(GameObject[] displayCharacters, ref CharacterTeamsData.TeamData teamData)
    {
        //使用しているインデックス番号のリストを準備
        List<int> useIndex = new List<int>(displayCharacters.Length);
        foreach (var character in displayCharacters)
        {
            //nullの場合は次に
            if (character == null) continue;

            //アクセスしているインデックス番号を使用リストに追加
            if (character.GetComponent<BasicStatusDataAccess>().BasicStatusDataIndex != null) useIndex.Add((int)character.GetComponent<BasicStatusDataAccess>().BasicStatusDataIndex);
        }

        //使用出来るインデックス番号を決める、なければnullを返す
        int? returnIndexNum = null;
        for (int index = 0; index < teamData.FollowCharacters_BasicStatusDatas.Count; index++)
        {
            //現在のループの番号が使えるかどうか確認する処理をこれから行い、使えない場合はnullが代入される
            returnIndexNum = index;

            //インデックス番号が使用されていないか使用インデックス番号リストをループして確認
            for (int useNum = 0; useNum < useIndex.Count; useNum++)
            {
                //既に使用しているインデックス番号の場合はnullを代入して次に進める
                if (useNum == returnIndexNum)
                {
                    returnIndexNum = null;

                    break;
                }
                
            }

            //渡すインデックス番号が決定されたらループを抜ける
            if (returnIndexNum != null) break;
        }

        return returnIndexNum;
    }

}
