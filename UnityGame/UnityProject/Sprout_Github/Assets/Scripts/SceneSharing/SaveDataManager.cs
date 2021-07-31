/*
 *プログラム参考Twitter@aaabaaab222
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    public class SaveDataClass
    {
        public int TotalNumberOfWins = 0;
    }
    public static List<SaveDataClass> SaveData = new List<SaveDataClass>();

    /// <summary>セーブファイルの場所(パス)</summary>
    public string SaveFilePath { get; private set; }

    [SerializeField] GameObject m_saveFilePathTextBackground;
    [SerializeField] Text m_saveFilePathText;

    /// <summary>セーブデータのファイル名</summary>
    private string _fileName = "/UnityGameSproutSaveFile";
    /// <summary>セーブデータの名前</summary>
    private string _dataName = "/SaveData.json";


    public void Save()
    {
        
        //セーブデータをJson化
        string saveData_Json = JsonUtility.ToJson(SaveData[0]);

        //Json化したセーブデータの書き込み
        File.WriteAllText(GetSaveFilePath() + _dataName, saveData_Json);
    }

    public void Delete()
    {

        //json化したセーブデータを削除
        File.Delete(GetSaveFilePath() + _dataName);
    }

    private new void Awake()
    {
        base.Awake();

        //全てのシーンで共有するためシーン移動で破棄しない
        DontDestroyOnLoad(gameObject);

        SaveFilePath = GetSaveFilePath();
        Debug.Log(GetSaveFilePath());

        //セーブデータを初期化(読み込み)する
        SaveDataLoadOrCreate();

    }

    private void Update()
    {

        if(Input.GetKey(KeyCode.P))
        {
            m_saveFilePathText.text = SaveFilePath;
            m_saveFilePathText.gameObject.SetActive(true);
            m_saveFilePathTextBackground.SetActive(true);
        }
        else
        {
            m_saveFilePathText.gameObject.SetActive(false);
            m_saveFilePathTextBackground.SetActive(false);
        }
    }

    private void Load()
    {

        //json化したセーブデータを読み込む
        string JsonData = File.ReadAllText(GetSaveFilePath() + _dataName);

         //json化したセーブデータをオブジェクト化
         SaveData[0] = JsonUtility.FromJson<SaveDataClass>(JsonData);

    }

    /// <summary>
    /// セーブデータを読み込む。なければディレクトリごと新規作成する。
    /// </summary>
    private void SaveDataLoadOrCreate()
    {
        //読み込むデータの格納場所を準備
        if(SaveData.Count==0) SaveData.Add(new SaveDataClass()); 

        //ディレクトリがあればロード、なければ新規作成(セーブデータを含む)
        if (Directory.Exists(GetSaveFilePath()))
        {
            //データをロード
            Load();

        }
        else
        {
            //ディレクトリの作成
            Directory.CreateDirectory(GetSaveFilePath());

            //セーブ関数を実行しているがここでは新規作成する目的で実行。
            Save();
        }
    }



    /// <summary>
    /// セーブデータを格納するファイルパスを返す。
    /// </summary>
    /// <returns></returns>
    string GetSaveFilePath()
    {
        return Application.persistentDataPath +_fileName;
    }
}
