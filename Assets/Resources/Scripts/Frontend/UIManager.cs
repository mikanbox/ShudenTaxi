using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIManager : SingletonMonoBehaviour<UIManager> {

    [SerializeField]private InputField SendingMessage;
    [SerializeField]private GameObject[] WindowObj;
    [SerializeField]private GameObject StatusBar;
    [SerializeField]private Text ErrorMes;

    private float here_lat,here_lng;

    private static float interval = 0;
    private UIState uiState = 0;

    void Update () {
        if (interval < 0) {
            StartCoroutine (CreatingMap.Instance.GetGPS());
            here_lng = Input.location.lastData.longitude;
            here_lat = Input.location.lastData.latitude;

            if (uiState == UIState.Map)CreatingMap.Instance.GetMap();
            interval = 60;
        }
        interval -= Time.deltaTime;
    }


    void Start() {
        CheckInstance();
        StateManager.Instance.ObserveEveryValueChanged(x => x.uiState).Subscribe( _ => WindowChange());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingState).Subscribe( _ => StatusBarManager());
        if (StateManager.Instance.userid==0)RequestSender.Instance.SubmitMakeIDRequest();
    }
    

    public void StatusBarManager() {
        StatusBar.SetActive((StateManager.Instance.matchingState == MatchingState.Matching));
        if (StateManager.Instance.matchingState == MatchingState.Matched){
            CreatingMap.Instance.UpdateTaxi(StateManager.Instance.taxi_lat,StateManager.Instance.taxi_lng);
        }
    }

    private void WindowChange() {
        WindowObj[(int)uiState].SetActive(false);
        // switch(StateManager.Instance.uiState){
        //     case UIState.Title:
        //         WindowObj[0].SetActive(true);
        //     break;
        //     case UIState.WriteComment:
        //         WindowObj[1].SetActive(true);
        //     break;
        // }
        WindowObj[(int)StateManager.Instance.uiState].SetActive(true);
        Debug.Log("from:" + uiState + "to:" + StateManager.Instance.uiState);
        uiState = StateManager.Instance.uiState;
        if (uiState == UIState.Map)CreatingMap.Instance.GetMap();
    }

    public void SubmitMatchingRequest() {
        if (SendingMessage.text != "") {
            MatchingRequest.RequestData data = new MatchingRequest.RequestData();
            data.userid = 15;//StateManager.Instance.userid;
            data.here_lat = here_lat;
            data.here_lng = here_lng;
            data.obj_lat = StateManager.Instance.obj_lat;
            data.obj_lng = StateManager.Instance.obj_lng;
            data.comment = SendingMessage.text;
            RequestSender.Instance.SubmitMatchingRequest(data);
        } else {
            ErrorMes.text = "ちゃんとコメントを入力しましょう";
        }
    }

    public void SubmitChangeToCommentUIRequest() {
        RequestSender.Instance.SubmitChangeToCommentUIRequest();
    }

    public void SubmitChangeToSettingUIRequest() {
        RequestSender.Instance.SubmitChangeToSettingUIRequest();
    }

    public void SubmitSettingConfirmRequest() {
        SettingConfirmRequest.RequestData data = new SettingConfirmRequest.RequestData();
        data.obj_lat = 139.73199f;
        data.obj_lng = 35.70902f;
        RequestSender.Instance.SubmitSettingConfirmRequest(data);
    }

}
