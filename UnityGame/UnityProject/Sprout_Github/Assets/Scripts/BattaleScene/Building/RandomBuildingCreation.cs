using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全てのグリッドに指定したリストの中からランダムな建物を建築する
/// </summary>
public class RandomBuildingCreation : SingletonMonoBehaviour<RandomBuildingCreation>
{
    /// <summary>現在生成中かどうか</summary>
    public bool Creating { get; private set; } = false;
    /// <summary>ランダムに生成する際に参照するリスト 引数(生成物の名前,確率,生成される高さの上限)</summary>
    public List<(BuildingGenerateManager.Buildings buildingName, int probability, int? altitudeLimit )> CreationList { get; set; } = new List<(BuildingGenerateManager.Buildings, int, int?)>();

    /// <summary>一フレーム当たりの生成個数</summary>
    [SerializeField] int m_GenerationSpeed = 3;

    private BuildingGenerateManager _buildingGenerateManager;
    private GridData _gridData;
    /// <summary>現在の建設場所を保持するための変数</summary>
    private (int x,int y, int z) _constructionSite=(0,0,0);

    private new void Awake()
    {
        base.Awake();

        //このクラスで使用するクラスを探して変数に代入する
        _buildingGenerateManager = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<BuildingGenerateManager>();
        _gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();

        //GenerationSpeed変数は1以上に設定。0以下だとクラスが機能しない
        if (m_GenerationSpeed <= 0)
        {
            m_GenerationSpeed = 1;
            Debug.LogWarning("GenerationSpeed変数は1以上に設定してください");
        }

        
    }

    private void FixedUpdate()
    {
        //ランダム生成する際に生成リストが空の場合は初期設定用のオブジェクトをセットする
        if(CreationList.Count==0)
        {
            CreationList.Add((BuildingGenerateManager.Buildings.BigRock, 40,0));
            CreationList.Add((BuildingGenerateManager.Buildings.Vegetation, 40,0));
            CreationList.Add((BuildingGenerateManager.Buildings.Air, 20,null));
        }

        for (int times = 0; times < m_GenerationSpeed; times++)
        {
            if (Creating)
            {
                RandomCreating();
            }
            else
            {
                break;
            }
        }
        


    }
    /// <summary>
    /// ランダム生成を開始する。既に開始している場合は無視
    /// </summary>
    /// <returns>開始した場合はtrue  既に開始していて改めて開始できない場合はfalse</returns>
    public bool CreationStart()
    {
        //無視
        if (Creating) return false;

        //開始
        Creating = true;
        return true;
    }

    /// <summary>
    /// グリッドに順番にランダム生成する
    /// </summary>
   private void RandomCreating()
    {

        //建物と回転をランダムに決め指定した座標に生成する
        _buildingGenerateManager.Generate(RandomSelect(_constructionSite.y), _constructionSite,Random.Range(0,4));

        //次のグリッドを指定
        _constructionSite.x++;

        //グリッド番号を繰り上げる
        GridAdvanceDimension(ref _constructionSite.x,ref  _constructionSite.z, _gridData.GridSize.x);
        GridAdvanceDimension(ref _constructionSite.z, ref _constructionSite.y, _gridData.GridSize.z);

        //最後の生成が終わったら終了処理をする
        if (_constructionSite.y== _gridData.GridSize.y)
        {
            _constructionSite = (0, 0, 0);

            Creating = false;
        }

        //グリッド番号を繰り上げる
        bool GridAdvanceDimension(ref int resetDimension,ref int advanceDimension, int judgmentNum)
        {
            if (resetDimension == judgmentNum)
            {
                resetDimension = 0;
                advanceDimension++;

                return true;
            }

            return false;
        }

        //建物をランダムに選び名前を返す
        BuildingGenerateManager.Buildings RandomSelect(int altitude)
        {
            //ランダムに出る数字の上限の計算
            int probabilityLimit = 0;
            foreach (var item in CreationList)
            {
                //選択から除外されるべきオブジェクトの判定
                if (altitude > item.altitudeLimit) continue;
               // Debug.Log(item.altitudeLimit);
                probabilityLimit += item.probability;
            }

            //ランダムに決める
            int probabilityNum = Random.Range(1, (probabilityLimit + 1));

            //決めた数字がどの建物オブジェクトを示しているか計算して求め、返す
            foreach (var item in CreationList)
            {
                //選択から除外されるべきオブジェクトの判定
                if (altitude > item.altitudeLimit) continue;

                probabilityNum -= item.probability;

                if (probabilityNum <= 0)
                {
                    return item.buildingName;
                }
            }

            Debug.LogError("到達してはいけないルートです");
            return BuildingGenerateManager.Buildings.Air;
        }
    
    }

}