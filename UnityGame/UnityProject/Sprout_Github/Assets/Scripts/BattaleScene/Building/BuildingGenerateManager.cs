using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerateManager : PrefabListOfBuildings<BuildingGenerateManager>
{
    public enum Buildings
    {
        Air=0,
        BigRock,
        Vegetation,
            Vegetation_Recovery
    }

    private StageBuildingData _stageBuildingData;
    private GameObject _stageBuildingManager;
    private GridCalculation _gridCalculation;



    private void Start()
    {
        //それぞれ必要なクラスやGameObjectを準備
        _stageBuildingManager = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager");
        _stageBuildingData = _stageBuildingManager.GetComponent<StageBuildingData>();
        _gridCalculation = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridCalculation>();
    }
    
    /// <summary>
    /// 建物を生成する。既に建物がある場合は既存の物を削除して生成する
    /// </summary>
    /// <param name="gameObjectName"></param>
    /// <param name="grid"></param>
    /// <param name="rightAngleNumOfTimes">角度はオイラー角として考え、計算は 90*引数 で行う。引数は0-3の整数で指定すること</param>
    /// <returns>生成したGameObjectを返す、生成出来ない場合はnullを返す</returns>
    public GameObject Generate(Buildings buildingName, (int x,int y,int z) grid, int rightAngleNumOfTimes=0)
    {
        //生成する建物のプレハブを取得する 
        GameObject prefab = PrefabGet(buildingName.ToString());

        if(prefab == null)
        {
            Debug.LogError(buildingName.ToString() + " という名前のプレハブは見つかりません。");
            return null;
        }

        //既にBasicStatusDataがあるかもしれないので初期化(null代入)。
        _stageBuildingData.DataSetAndGenerate(null, grid);

        //建物を生成して建物データ管理クラスにセット出来たら場所や回転の情報をセット、出来なかったら削除
        GameObject createObject = Instantiate(prefab);
        createObject.name = buildingName.ToString();//名前につくcloneを取り除く
        if (_stageBuildingData.DataSet(createObject,grid))
        {
            //建物オブジェクトにBasicStatusDataAccessがある場合の処理
            if (createObject.GetComponent<BasicStatusDataAccess>())
            {
                //建物のBasicStatusDataも生成してセットする
                _stageBuildingData.DataSetAndGenerate(ReturnBasicStatusDataWithBuildingsEnum(buildingName), grid);
                //建物オブジェクト自体がBasicStatusDataにアクセス出来るようにセットする。
                createObject.GetComponent<BasicStatusDataAccess>().BuildingDataSet(grid.x, grid.y, grid.z);
            }

            createObject.transform.position= _gridCalculation.Vector3PositionGet(grid);

            createObject.transform.eulerAngles =new Vector3(0,90 * Mathf.Clamp(rightAngleNumOfTimes,0,3), 0);
        }
        else
        {
            Destroy(createObject);
        }

        return createObject;
    }
    
    private BasicStatusData ReturnBasicStatusDataWithBuildingsEnum(Buildings buildingEnum)
    {
        switch (buildingEnum)
        {
            case Buildings.Air:
                return null;
            case Buildings.BigRock:
                return new BasicStatusData
                {
                    Name = "巨大岩",
                    PhysicalFitness = 1000,
                    DefensePower = 40
                };
            case Buildings.Vegetation:
                return new BasicStatusData
                {
                    Name = "巨大な植物",
                    PhysicalFitness = 1500,
                    DefensePower = 15
                };
            case Buildings.Vegetation_Recovery:
                return new BasicStatusData
                {
                    Name = "巨大な回復植物",
                    PhysicalFitness = 1500,
                    DefensePower = 15
                };
        }
        return null;
    }

}



