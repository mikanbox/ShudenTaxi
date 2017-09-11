using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MiniJSON;

public class UIManager : SingletonMonoBehaviour<UIManager> {

    [SerializeField]private InputField SendingMessage;
    [SerializeField]private InputField Adress;
    [SerializeField]private GameObject[] WindowObj;
    [SerializeField]private GameObject StatusBar;
    [SerializeField]private Text ErrorMes;
    [SerializeField]private Text nogashiTimesMess;


    private static float interval = 0;
    private UIState uiState = 0;

    void Update () {
        if (interval < 0) {
            CompleteSendLikeandGettingComment();

            interval = 5f;
        }
        interval -= Time.deltaTime;
    }


    void Start() {
        CheckInstance();
        StateManager.Instance.ObserveEveryValueChanged(x => x.uiState).Subscribe( _ => WindowChange());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingState).Subscribe( _ => StatusBarManager());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingRequestStatus).Subscribe( _ => LoadComment());
        if (StateManager.Instance.userid == 0)RequestSender.Instance.SubmitMakeIDRequest(); //ユーザーidないとき
        if (StateManager.Instance.nogashiTimes != 0) {
            nogashiTimesMess.text = "" + (StateManager.Instance.nogashiTimes + 1) + "回目!";
        } else {
            nogashiTimesMess.text = "初めて!";
        }
    }

    public void StatusBarManager() {//MatchingStateが変わった時よばれる
        StatusBar.SetActive((StateManager.Instance.matchingState == MatchingState.Matching));
        if (StateManager.Instance.matchingState == MatchingState.Matched) {
            CreateMessWindow("マッチング過料", "相乗り相手が見つかったのでタクシーを呼びました。地図の赤い点に移動してタクシーを待ちましょう");
            CreatingMap.Instance.UpdateTaxi(StateManager.Instance.taxi_lat, StateManager.Instance.taxi_lng);
        }
        if (StateManager.Instance.matchingState == MatchingState.TaxiCome) {
            WindowObj[2].SetActive(false);
            WindowObj[4].SetActive(true);
        }
    }

    public void LoadComment() {
        //if (StateManager.Instance.matchingRequestStatus == RequestStatus.Failure) {
        if (uiState == UIState.Map)CreatingMap.Instance.UpDateComment();
        //Debug.Log("TriggercommentLoad");
        //}
    }

    private void WindowChange() {
        WindowObj[(int)uiState].SetActive(false);
        WindowObj[(int)StateManager.Instance.uiState].SetActive(true);
        //Debug.Log("from:" + uiState + "to:" + StateManager.Instance.uiState);
        uiState = StateManager.Instance.uiState;
        if (uiState == UIState.WriteComment)CreatingMap.Instance.GetMap(StateManager.Instance.here_lat, StateManager.Instance.here_lng);
        if (uiState == UIState.Map) {   //マップに遷移した時はコメント読みこみ
            CreatingMap.Instance.GetMap(StateManager.Instance.here_lat, StateManager.Instance.here_lng);
            CreatingMap.Instance.UpDateComment();
        }
    }

    public void SubmitMatchingRequest() {
        if (SendingMessage.text != "") {
            MatchingRequest.RequestData data = new MatchingRequest.RequestData();
            data.userid = StateManager.Instance.userid; //送るid
            data.here_lat = StateManager.Instance.here_lat;
            data.here_lng = StateManager.Instance.here_lng;
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
        if (Adress.text != "") {
            CreateMessWindow("登録完了", "目的地を登録しました。戻るボタンで戻ってください。");
            RequestSender.Instance.SubmitAddressToGeometryRequest(Adress.text);
            StateManager .Instance.ObserveEveryValueChanged(x => x.addressToGeometryRequestStatus).Subscribe( _ => CompleteSettingRequest());
            }else{
                CreateMessWindow("", "住所を入力してください");
            }
    }
    public void CompleteSettingRequest() {
        CreatingMap.Instance.GetMap(StateManager.Instance.ad_lat, StateManager.Instance.ad_lng);
    }

    public void GetAddressLatLng() {
        if (StateManager.Instance.addressToGeometryRequestStatus == RequestStatus.Success) {
            SettingConfirmRequest.RequestData data = new SettingConfirmRequest.RequestData();
            data.obj_lat = StateManager.Instance.ad_lat;
            data.obj_lng = StateManager.Instance.ad_lng;
            RequestSender.Instance.SubmitSettingConfirmRequest(data);
        }
    }



    public void SubmitCommentLikeFightAdd(LikeFightSendRequest.CommentType type, int commentid) {
        Debug.Log("type" + type);
        LikeFightSendRequest.RequestData data = new LikeFightSendRequest.RequestData();
        data.comment_id = commentid;
        data.type = type;
        RequestSender.Instance.SubmitLikeFightSendRequest(data);
        //StateManager .Instance.ObserveEveryValueChanged(x => x.likeFightSendRequestStatus).Subscribe( _ => CompleteSendLikeandGettingComment());
    }

    public void CompleteSendLikeandGettingComment() {
        //if (StateManager.Instance.likeFightSendRequestStatus == RequestStatus.Success){
        //Debug.Log("GetComment");
        CreatingMap.Instance.UpDateComment();
        //}
    }

    private void CreateMessWindow(string title, string mess) {
        GameObject tmp = Instantiate(Resources.Load("Prefabs/messWindow")) as GameObject;
        tmp.GetComponent<ItemWindow>().setText("exp", mess);
        tmp.GetComponent<ItemWindow>().setText("title", title);
    }


}
