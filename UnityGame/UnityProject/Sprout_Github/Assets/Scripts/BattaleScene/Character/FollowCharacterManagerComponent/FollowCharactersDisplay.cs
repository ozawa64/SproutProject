using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharactersDisplay : FollowCharactersBasicStatusDataSet
{
    /// <summary>リーダーの周りに表示されるフォローキャラクターのオブジェクト配列※人数に変動があっても配列のサイズは変わらない</summary>
    public GameObject[] DisplayCharacters{ get; private set; } 

    /// <summary>表示上限人数</summary>
    [SerializeField] protected int m_MaxDisplayNumberOfPeople = 9;

    private CharacterGenerateManager _characterGenerateManager;

    protected void Awake()
    {
        //配置上限数に合わせて配列のサイズを決定
        DisplayCharacters = new GameObject[m_MaxDisplayNumberOfPeople];
    }

    protected void Start()
    {
        //このクラスで使用するクラスを探して変数に代入する
        _characterGenerateManager = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterGenerateManager>();
    }

    /// <summary>
    /// 現在表示されている人数
    /// </summary>
    /// <returns></returns>
    protected int CurrentNumberOfPeopleDisplayed()
    {
        int numberOfPeople=0;

        //表示人数(nullではない)のカウント
        foreach (var item in DisplayCharacters)
        {
            if (item != null) numberOfPeople++;
        }

        return numberOfPeople;
    }

    /// <summary>
    /// 現在表示する人数を変更する
    /// </summary>
    protected void ChangeTheDisplayedNumberOfPeople(int displayedNumberOfPeople,ref CharacterTeamsData.TeamData teamData,Vector3 leaderPosition)
    {
        //あらかじめ決められている最大人数を超えている場合は最大人数に設定する
        if (displayedNumberOfPeople > DisplayCharacters.Length) displayedNumberOfPeople = DisplayCharacters.Length;

        //変更後の人数が現在の人数と同じ場合は処理を終了する
        if (CurrentNumberOfPeopleDisplayed() == displayedNumberOfPeople) return;

        //一時的に表示キャラクターを保存する配列
        List<GameObject> tempDisplayCharacterList = new List<GameObject>(DisplayCharacters);

        //一時的保存配列からnull要素を全て削除
        tempDisplayCharacterList.RemoveAll(displayCharacter => displayCharacter == null);

        //一時的保存リストに今あるGameObjectは渡したのでDisplayCharactersを初期化
        DisplayCharacters = new GameObject[DisplayCharacters.Length];

        //要求された表示人数分ループ(上限人数以下であること前提)してDisplayCharactersにキャラクターを追加していく
        for (int i = 0; i < displayedNumberOfPeople; i++)
        {
            //一時保存リストのフォローキャラクター達を先頭(0)から詰めていく
            if ( i < tempDisplayCharacterList.Count )
            {
                DisplayCharacters[i] = tempDisplayCharacterList[i];

                //設計ミスの強引な修正
                //フォローキャラクターのBasicStatusDataリストの体力が0になったデータは削除され参照先のインデックスが変更になるので合わせて変更する
                DisplayCharacters[i].GetComponent<BasicStatusDataAccess>().BasicStatusDataIndex = i;

                //渡したのでnull
                tempDisplayCharacterList[i] = null;
            }
            else
            {
                //一時保存リストにもうない(新規にキャラクターを生成する必要がある)ので生成して追加する
                DisplayCharacters[i] = _characterGenerateManager.FollowCharacterGenerate_GameObject((CharacterGenerateManager.Camp)teamData.Camp);

                //親オブジェクトをthisに設定
                DisplayCharacters[i].transform.parent = transform;

                //初期場所はリーダーの場所
                DisplayCharacters[i].transform.position = leaderPosition;

                //BasicStatusDataAccessがアクセスするBasicStatusDataを設定する
                DisplayCharacters[i].GetComponent<BasicStatusDataAccess>().CharacterTeamsDataSet(ref teamData, BasicStatusDataAccess.BasicStatusDataType.FollowCharacter, (int)UnusedBasicStatusDataIndex(DisplayCharacters,ref teamData));

                //TeamData(リーダーの場所情報)が必要なので設定する
                DisplayCharacters[i].GetComponent<IsSeperating_WarpToLeader>().TeamDataSet(ref teamData);
            }
        }

        //余りのキャラクターは削除
        foreach (var tempDisplayCharacter in tempDisplayCharacterList)
        {
            Destroy(tempDisplayCharacter);//あとで修正?
        }
    }
}
