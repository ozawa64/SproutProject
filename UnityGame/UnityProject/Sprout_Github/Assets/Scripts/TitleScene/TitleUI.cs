using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : CanvasChildUIBase
{
    /// <summary>入力を受け付ける(これがfalseの時、入力受付速度関係なく受付を拒否する)</summary>
    public bool IsInputReception { get; set; } = true;
    /// <summary>入力受付速度</summary>
    [SerializeField] float m_InputReceptionSpeed = 0.7f;
    /// <summary>選択されるテキスト達(GameObject)</summary>
    private GameObject[] _selectTextObjects=new GameObject[3];
    /// <summary>入力受付速度の計測</summary>
    private float inputReceptionSpeedTimeCount=0;
    /// <summary>選択している番号</summary>
    private int selectTextNum=0;
    /// <summary>一フレーム前に選択していた番号</summary>
    private int oneFrameAgoSelectTextNum = 0;

    /// <summary>
    /// 選択テキスト番号の列挙型
    /// </summary>
    enum SelectTextNum_enum:int
    {
        GameStrat=0,
        Setting,
        SaveDataReset
    }

    //Update関数の場合pcによっては不自然な動きになるのでfixedにして対処。次回ではアニメーションを見直して改善する
    private void FixedUpdate()
    {
        if(!ThisUIDisplayModeCheck(transform.parent.GetComponent<UIManager>().CurrentDisplayMode))return;

        //UIオブジェクトの中にnullがある場合はUIの処理が上手くできない出来ない可能性があるのでUIオブジェクトを子から探し、このフレームは処理しない
        if(UIObjectContainsNull())
        {
           _selectTextObjects = FindsTheObjectWithTheSpecifiedName();

            return;
        }

        

        //入力受付時間の計測
        InputReceptionTimeCheck(true);
        
        //入力を受け付けている
        if (IsInputReception)
        {
            //入力に応じた選択番号の変更
            InputToSelectNumber();

            SelectNumExecution();
        }

        //選択テキストのアニメーション
        SelectTextAnimation();

        //変数の機能上プログラムの最後に処理
        oneFrameAgoSelectTextNum = selectTextNum;
    }

    private void LateUpdate()
    {
        //変数の機能上プログラムの最後に処理
      //  oneFrameAgoSelectTextNum = selectTextNum;
    }

    protected override bool UIObjectContainsNull()
    {
        foreach (var textObject in _selectTextObjects)
        {
            if (textObject == null) return true;
        }

        return false;
    }

    /// <summary>
    /// 選択している番号に応じた処理をする
    /// </summary>
    private void SelectNumExecution()
    {
       if(Input.GetButton("Decision"))
        {
            switch (selectTextNum)//選択されたテキストの番号に応じた処理をする
            {
                case (int)SelectTextNum_enum.GameStrat://ゲームを開始する

                    //戦闘sceneに移動
                    SceneManager.LoadScene("BattleScene");
                    
                    break;
                case (int)SelectTextNum_enum.Setting://設定画面を表示する

                    break;
                case (int)SelectTextNum_enum.SaveDataReset:
                    //決定ボタンが押されているかをアニメーターに伝える
                    _selectTextObjects[selectTextNum].GetComponent<Animator>().SetBool("SignificantDecision", true);
                    
                    break;
            }
        }
       else
        {
            switch (selectTextNum)//選択されたテキストの番号に応じた処理をする
            {
                case (int)SelectTextNum_enum.GameStrat:
                    break;
                case (int)SelectTextNum_enum.Setting:
                    break;
                case (int)SelectTextNum_enum.SaveDataReset:
                    //決定ボタンが離されたかをアニメーターに伝える
                    _selectTextObjects[selectTextNum].GetComponent<Animator>().SetBool("SignificantDecision", false);

                    break;
            }
        }
    }


    /// <summary>
    /// 選択しているテキストにアニメーションを実行させる
    /// </summary>
    private void SelectTextAnimation()
    {
        //選択番号に変化があった時のみ処理をする
        if (oneFrameAgoSelectTextNum != selectTextNum)
        {
            //過去に選択していたテキストのアニメーションを終了する
            _selectTextObjects[oneFrameAgoSelectTextNum].GetComponent<Animator>().SetBool("TextSeleting", false);
        }

        //選択しているテキストのアニメーションを実行
        _selectTextObjects[selectTextNum].GetComponent<Animator>().SetBool("TextSeleting", true);
    }

    /// <summary>
    /// Inputを受け付けて選択番号を変更する
    /// </summary>
    private void InputToSelectNumber()
    {
        //上入力と受付時間内の時に処理
        if(Input.GetAxisRaw("Vertical") >0&& InputReceptionTimeCheck())
        {
            //受付時間のリセット
            InputReceptionTimeCheck(false,true);

            selectTextNum--;

        }//下入力と受付時間内の時に処理
        else if(Input.GetAxisRaw("Vertical") <0 && InputReceptionTimeCheck())
        {
            //受付時間のリセット
            InputReceptionTimeCheck(false, true);

            selectTextNum++;

        }

        //選択番号を選択テキスト配列の長さの範囲で収める
        selectTextNum = Mathf.Clamp(selectTextNum, 0, (_selectTextObjects.Length-1));

        
    }

    /// <summary>
    /// 現在が入力受付時間かどうか判定する
    /// </summary>
    /// <param name="tiemMeasure">計測をする。falseで計測を止める(時間のリinputReceptionSpeedTimeCountセット)</param>
    /// <returns>入力受付時間の場合はtrueを返す</returns>
    private bool InputReceptionTimeCheck(bool timeCount=false,bool timeReset=false)
    {
        //時間の計測
        if (timeCount) inputReceptionSpeedTimeCount += Time.deltaTime;
       
        //時間ののリセット
           if(timeReset) inputReceptionSpeedTimeCount = 0;

        //Input入力がない(キーから手が離れた)場合、入力受付時間を受付可能時間にする。これによって素早く連打入力をするプレイヤーの入力にも対応できる。
        if (Input.GetAxisRaw("Vertical") == 0) inputReceptionSpeedTimeCount = m_InputReceptionSpeed;

        //入力を受け付ける時間になったらtrueを返す
        if (inputReceptionSpeedTimeCount >= m_InputReceptionSpeed) return true;

        //入力を受付る時間ではない場合falseを返す
        return false;
    }

        
   
}
