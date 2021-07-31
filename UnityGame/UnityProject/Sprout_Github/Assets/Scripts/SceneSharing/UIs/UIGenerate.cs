using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGenerate<T> : CanvaAdditionalSupport<T> where T : MonoBehaviour
{
    /// <summary>Resources/+ファイルパス</summary>
    //[SerializeField] private string m_generateUIPrefabsFaldaPath;
    /// <summary>生成に必要なプレハブのファイルパス達。※Resources/+ファイルパス</summary>
    [SerializeField] private string[] m_generateUIPrefabsPaths;
    /// <summary>生成に必要なGameObjectのプレハブ達</summary>
    [SerializeField] private GameObject[] generateObjects;

    /// <summary>ロード前のプレハブとそのファイルパス。※使用する際はGenerateUI関数を通してインスタンスオブジェクトを取得する</summary>
    private SortedList<string, GameObject> _prefabList = new SortedList<string,GameObject>();


    protected new void Awake()
    {
        base.Awake();


        //_prefabListにプレハブの名前とGameObjectを追加していく
        foreach (var prefabPath in m_generateUIPrefabsPaths)
        {
           GameObject loadObject = (GameObject)Resources.Load(prefabPath);

            _prefabList.Add(loadObject.name, loadObject);
        }
       
    }


    /// <summary>
    /// 指定した名前のUIGameObjectを生成してcanvasの子オブジェクトにする、なければnullを返す
    /// ※注意:同じ名前のUIを何回も生成した場合、重複しない
    /// </summary>
    /// <returns>生成したGameObject</returns>
    protected GameObject GenerateUI(string prefabName)
    {
        GameObject generateUI=null;

        //指定した名前のプレハブがあればそれをロードして生成(インスタンス化)する
        if (_prefabList.ContainsKey(prefabName))
        {
            generateUI = Instantiate(_prefabList[prefabName]);
            generateUI.name = prefabName;
        }

        //UIオブジェクトを生成出来なかったらnullを返す
        if (generateUI == null)
        {
           Debug.LogError(prefabName + " という名前のプレハブは見つかりません。");
            Debug.Log(prefabName + " という名前のプレハブは見つかりません。");
            return null;
        }

        CanvasAdd(generateUI);

        //既に同じ名前のUIがある場合は既存のUIオブジェクトの親(canvasの一段下の子オブジェクト)が子オブジェクトを引き継ぐ
        for (int canvasChild_i = 0; canvasChild_i < transform.childCount; canvasChild_i++)
        {

            //子オブジェクトは既存のUIオブジェクトに移すので同じ名前でも古い方を探す
            if (transform.GetChild(canvasChild_i).name== generateUI.name &&!transform.GetChild(canvasChild_i).gameObject.Equals(generateUI))
            {
                //移動先の親のCanvasChildUIBaseクラスコンポーネントを取得
                CanvasChildUIBase generateUIDestinationParent = (CanvasChildUIBase)transform.GetChild(canvasChild_i).GetComponents(typeof(CanvasChildUIBase))[0];

                //子の重複を許可されているか。許可されていない場合は移す親オブジェクトの子が0の時のみ移す
                if (generateUIDestinationParent.PermissionDuplicateChildObjects  ||  transform.GetChild(canvasChild_i).childCount==0)
                {
                    //子オブジェクトを移す
                    for (int generateUIChild_i = 0; generateUI.transform.childCount > 0; generateUIChild_i++)
                    {
                        generateUI.transform.GetChild(0).transform.SetParent(transform.GetChild(canvasChild_i));
                    }

                    //子オブジェクトを全て移したら親は削除
                    Destroy(generateUI);
                }
                else
                {
                    //移す事が出来ない場合は削除する
                    Destroy(generateUI);
                }

                return transform.GetChild(canvasChild_i).gameObject;
            }
        }

        return generateUI;

    }



}
