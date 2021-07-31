using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleSceneEndProcedure : SingletonMonoBehaviour<BattleSceneEndProcedure>
{
    /// <summary>trueにするとトリガー変数がnullになったらゲーム終了処理を始める</summary>
    public bool TriggerActive { get;private set; } = false;

    private UIManager _uIManager;
    private PlayerInputManager _playerInputManager;
    private GameObject _gameClearTriggerObject  = null;
    private GameObject _gameOverTriggerObject  = null;
    /// <summary>バトルシーンを終了する際にクリア画面を表示するかゲームオーバー画面なのかなどを決める</summary>
   // private EndRoot _endRoot= EndRoot.Gaming;
    /// <summary>エンディング処理を開始する</summary>
    private bool _endingStart=false;
    enum EndRoot
    {
        Gaming,
        GameClear,
        GameOver,
    }

    /// <summary>
    /// トリガーに設定したオブジェクトがnull(削除)になったらゲーム終了処理に進む
    /// </summary>
    /// <param name="clearTriggerObject"></param>
    /// <param name="overTriggerObject"></param>
    public void TriggerSet(GameObject clearTriggerObject,GameObject overTriggerObject)
    {
        //設定した時点でnullだとすぐに条件を達成して終了処理に進んでしまうため警告する
       if(clearTriggerObject==null||overTriggerObject==null) Debug.LogWarning("Triggerオブジェクトがnullです");

       //トリガー変数に代入
        _gameClearTriggerObject = clearTriggerObject;
        _gameOverTriggerObject = overTriggerObject;

        TriggerActive = true;
    }

    private void Start()
    {
        _playerInputManager = gameObject.SearchByTagName("SingletonManager", "PlayerOnlyManager").GetComponent<PlayerInputManager>();
        _uIManager = gameObject.SearchByTagName("SingletonManager", "CanvasAndUIManager").GetComponent<UIManager>();
    }

    private void FixedUpdate()
    {
        if (TriggerActive && _endingStart==false) TriggerCheck();

        //終了画面を表示する際は物理時間を止める
        if (_endingStart==true)Time.timeScale = 0;
    }

    private void Update()
    {
        //エンターキーが押されたらタイトルに戻る
        if (Input.GetKey(KeyCode.Return)&& _endingStart)
        {
            //タイトルシーンに移動
            SceneManager.LoadScene("TitleScene");

            //タイトルに移動する際に物理時間を動かす(戻す)。
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// ゲームを終了させる条件を達成しているか確認して、達成していたらその条件に合うエンドルートの準備処理をする
    /// </summary>
    private void TriggerCheck()
    {
        //オブジェクトがnullになったらクリア条件を達成したことになるので、クリア処理に進める処理をする
        if(_gameClearTriggerObject == null)
        {
            //セーブデータに勝利回数を一つ追加する
            SaveDataManager saveDataManager = gameObject.SearchByTagName("SingletonManager", "SaveDataManager").GetComponent<SaveDataManager>();
            SaveDataManager.SaveData[0].TotalNumberOfWins++;
            //セーブする
            saveDataManager.Save();
            
            //エンドルートをクリアにする
            //_endRoot = EndRoot.GameClear;
            //プレイヤーの入力を無効化する
            _playerInputManager.RejectPlayerInput = true;
            //ゲーム終了準備を始める
            _endingStart = true;
            //クリアUIを表示する
            _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.GameClear);
            return;
        }

        //オブジェクトがnullになったらゲームオーバー条件を達成したことになるので、ゲームオーバー処理に進める処理をする
        if (_gameOverTriggerObject == null)
        {
            //エンドルートをゲームオーバーにする
            //_endRoot = EndRoot.GameOver;
            //プレイヤーの入力を無効化する
            _playerInputManager.RejectPlayerInput = true;
            //ゲーム終了準備を始める
            _endingStart = true;
            //ゲームオーバーUIを表示する
            _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.GameOver);
            return;
        }

    }


}
