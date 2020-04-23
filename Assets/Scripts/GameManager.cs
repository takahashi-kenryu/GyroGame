using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  // UI系
  private Text ui_highScore;
  private Text ui_count;
  private Text ui_score;

  // シーン状態
  private int state; // 0:Title 1:Counting 2:InGame 3:Result

  // シーン跨いで使う変数
  private int g_highScore;
  private float g_score;

  // 各シーンで使う変数
  //--------- Title
  //---------- Counting
  private int count;
  private float count_start_time;
  //---------- InGame
  private float ingame_start_time;
  //---------- Result
  private float result_start_time;


  private Gyroscope m_Gyro;
  private GUIStyle debugStyle;
  private bool isTouchDown = false;
  private bool isTouchUp = false;

  // Start is called before the first frame update
  void Start()
  {
    m_Gyro = Input.gyro;
    m_Gyro.enabled = true;

    debugStyle = new GUIStyle();
		debugStyle.fontSize = 20;

    // UI系
    this.ui_highScore = GameObject.Find("Canvas/UI/HighScore").GetComponent<Text>();
    this.ui_count = GameObject.Find("Canvas/UI/Count").GetComponent<Text>();
    this.ui_score = GameObject.Find("Canvas/UI/Score").GetComponent<Text>();

    this._Initialize();

  }
  void _Initialize(){
    this._StartTitle();
  }

  // Update is called once per frame
  void Update()
  {
    if(Input.touchCount > 0 || Input.GetMouseButton(0)){
      this.isTouchDown = true;
      this.isTouchUp = false;
    }else{
      if (this.isTouchDown)
      {
        this.isTouchUp = true;
        this.isTouchDown = false;
      }
      else
      {
        this.isTouchUp = false;
      }
    }

    if(this.state == 0){
      this._UpdateTitle();
    }
    else if(this.state == 1){
      this._UpdateCounting();
    }
    else if(this.state == 2){
      this._UpdateInGame();
    }
    else if(this.state == 3){
      this._UpdateResult();
    }
  }
  void FixedUpdate(){
    if(this.state == 2){
      this._FixedUpdateInGame();
    }
  }

  void _StartTitle(){
    Debug.Log("StartTitle");
    this.state = 0;
    this.g_score = 0;
    this.ui_highScore.text = this.g_highScore + "";
    this.ui_count.text = "タップするとスタート";
    this.ui_score.text = "0";
  }
  void _UpdateTitle(){
    if(this.isTouchUp) {
      this._StartCounting();
    }
  }

  void _StartCounting(){
    Debug.Log("StartCounting");
    this.state = 1;
    this.count = 3;
    this.count_start_time = Time.time;
    AndroidVibraotr.Vibrate(10);
  }
  void _UpdateCounting(){
    if(Time.time - this.count_start_time > 1){
      this.count = this.count - 1;
      this.count_start_time = Time.time;
      AndroidVibraotr.Vibrate(10);
    }
    if(this.count > 0)
      this.ui_count.text = this.count + "";
    else{
      this._StartInGame();
    }

  }
  void _StartInGame(){
    Debug.Log("StartInGame");
    this.state = 2;
    this.ui_count.text = "スマホを振る！！";
    this.ingame_start_time = Time.time;
    AndroidVibraotr.Vibrate(500);
  }
  void _UpdateInGame(){
    this.ui_score.text = Mathf.Floor(this.g_score) + "";

    if(Time.time - this.ingame_start_time > 10){
      this._StartResult();
    }
  }
  void _FixedUpdateInGame(){
    Vector3 rot = this.m_Gyro.rotationRate;

    float x = Mathf.Abs(rot.x);
    float y = Mathf.Abs(rot.y);
    float z = Mathf.Abs(rot.z);
    float s = (x + y + z) * 0.1f;
    this.g_score += s;
  }
  void _StartResult(){
    Debug.Log("Result");
    this.state = 3;
    if(this.g_score > this.g_highScore){
      this.g_highScore = Mathf.FloorToInt(this.g_score);
    }

    this.ui_count.text = "タップしてもう一度";

    // 誤タップ防止
    this.isTouchDown = false;
    this.isTouchUp = false;

    AndroidVibraotr.Vibrate(1500);
    this.result_start_time = Time.time;
  }
  void _UpdateResult(){
    if(this.isTouchUp) {
      this._StartTitle();
    }
  }

  void OnGUI(){
      GUI.Label(new Rect(5, 800, 200, 40), "Gyro rotation rate " + m_Gyro.rotationRate, debugStyle);
      GUI.Label(new Rect(5, 825, 200, 40), "Gyro attitude" + m_Gyro.attitude, debugStyle);
      GUI.Label(new Rect(5, 850, 200, 40), "Gyro enabled : " + m_Gyro.enabled, debugStyle);
      GUI.Label(new Rect(5, 875, 200, 40), "Gyro gravity " + m_Gyro.gravity, debugStyle);
      GUI.Label(new Rect(5, 900, 200, 40), "Gyro rotationRateUnbiased" + m_Gyro.rotationRateUnbiased, debugStyle);
      GUI.Label(new Rect(5, 925, 200, 40), "Gyro userAcceleration : " + m_Gyro.userAcceleration, debugStyle);
  }
}