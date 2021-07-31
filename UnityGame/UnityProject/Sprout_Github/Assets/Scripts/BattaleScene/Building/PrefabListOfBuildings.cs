using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class PrefabListOfBuildings<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{
    /// <summary>Resources/+ファイルパス</summary>
    //[SerializeField] private string m_generateUIPrefabsFaldaPath;
    /// <summary>生成に必要なプレハブのファイルパス達。※Resources/+ファイルパス</summary>
    [SerializeField] private string[] m_generateUIPrefabsPaths;
    /// <summary>生成に必要なGameObjectのプレハブ達</summary>
    [SerializeField] private GameObject[] generateObjects;

    /// <summary>ロード前のプレハブとそのファイルパス。※使用する際はGenerateUI関数を通してインスタンスオブジェクトを取得する</summary>
    private SortedList<string, GameObject> _prefabList = new SortedList<string, GameObject>();


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
    /// 指定した名前のプレハブを返す、なければnullを返す
    /// </summary>
    /// <returns>prefabのGameObject</returns>
    public GameObject PrefabGet(string prefabName)
    {
        //指定した名前のプレハブがあればそれを返す//渡す際にロードする
        if (_prefabList.ContainsKey(prefabName)) return _prefabList[prefabName];

        return null;
    }

}
