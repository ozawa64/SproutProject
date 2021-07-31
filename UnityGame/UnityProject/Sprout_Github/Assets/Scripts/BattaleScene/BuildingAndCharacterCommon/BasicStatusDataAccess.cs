using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// CharacterTeamsDataから参照している自分の基本ステータスを渡す(参照渡し)。
/// </summary>
public class BasicStatusDataAccess : MonoBehaviour
{

    /// <summary>名前</summary>
    public ref string Name
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).Name : ref AccessBasicStatusData().Name;
        }
    }
    /// <summary>体力</summary>
    public ref int PhysicalFitness
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).PhysicalFitness : ref AccessBasicStatusData().PhysicalFitness;

        }
    }
    /// <summary>最大体力</summary>
    public ref int MaxPhysicalFitness
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).MaxPhysicalFitness : ref AccessBasicStatusData().MaxPhysicalFitness;

        }
    }
    /// <summary>防御力</summary>
    public ref int DefensePower
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).DefensePower : ref AccessBasicStatusData().DefensePower;

        }
    }
    /// <summary>攻撃力</summary>
    public ref int OffensivePower
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).OffensivePower : ref AccessBasicStatusData().OffensivePower;

        }
    }
    /// <summary>移動の最高速度</summary>
    public ref float MoveMaxSpeed
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).MoveMaxSpeed : ref AccessBasicStatusData().MoveMaxSpeed;

        }
    }
    /// <summary>移動速度</summary>
    public ref float MoveSpeed
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).MoveSpeed : ref AccessBasicStatusData().MoveSpeed;
        }
    }
    /// <summary>ジャンプ回数※地面から跳ぶ際も含む</summary>
    public ref float JumpFrequency
    {
        get
        {
            return ref (BasicStatusDataType.FollowCharacter == _basicStatusDataType) ? ref _teamData.FollowCharacters_BasicStatusDatas.ElementAt((int)BasicStatusDataIndex).JumpFrequency : ref AccessBasicStatusData().JumpFrequency;
        }
    }

    public enum BasicStatusDataType: int
    {
        Leader=0,
        FollowCharacter,
        Building
    }

    /// <summary>現在データにアクセスできるかどうか</summary>
    public bool Access { get; private set; } = false;
    /// <summary>どのインデックスにアクセスするか。・CharacterTeamsDataのBasicStatusData配列</summary>
    public int? BasicStatusDataIndex { get;  set; } = null;
    /// <summary>どのインデックスにアクセスするか。・</summary>
    public (int,int,int)? BasicStatusDataIndex_3d { get; private set; } = null;
    //キャラクターオブジェクトのBasicStatusDataが保管される変数型
    private CharacterTeamsData.TeamData _teamData = null;
    /// <summary>BasicStatusDataを保持するオブジェクトによってアクセス準備の仕方が変わる</summary>
    private BasicStatusDataType _basicStatusDataType;
    private StageBuildingData _stageBuildingData;

    public void CharacterTeamsDataSet(ref CharacterTeamsData.TeamData teamData, BasicStatusDataType basicStatusDataType, int? basicStatusDataIndex = null)
    {
        _teamData = teamData;
        _basicStatusDataType = basicStatusDataType;
        BasicStatusDataIndex = basicStatusDataIndex;

        Access = true;
    }

    public void BuildingDataSet(int indexX, int indexY, int indexZ)
    {
        BasicStatusDataIndex_3d = (indexX,indexY,indexZ);

        _basicStatusDataType = BasicStatusDataType.Building;

        Access = true;

    }

    public void DeletePreparation()
    {
        Access = false;
    }

    private void Start()
    {
        _stageBuildingData = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<StageBuildingData>();
    }

    /// <summary>
    /// BasicStatusDataTypeに応じてBasicStatusDataを返す
    /// </summary>
    private ref BasicStatusData AccessBasicStatusData()
    {

        /*
         FollowCharacterもここに書く予定だったが上手く参照渡しが出来なさそう(出来ても時間がかかる上に複雑化しそう)なので別で処理
         */

        switch (_basicStatusDataType)
        {
            case BasicStatusDataType.Leader:
                return ref _teamData.Leader_BasicStatusData;
            case BasicStatusDataType.Building:
                return ref _stageBuildingData.BasicStatusDataGet(((int x, int y, int z))BasicStatusDataIndex_3d);
        }

        Debug.LogError("想定外のルートです");
        return ref _teamData.Leader_BasicStatusData;
    }

}