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

    public float here_lat, here_lng;
    //private float test_lat = 35.70902f, test_lng = 139.73099f;
    private float test_lat = 35.25289f, test_lng = 139.0007f;

    private static float interval = 0;
    private UIState uiState = 0;

    void Update () {
        if (interval < 0) {
// #if UNITY_ANDROID || UNITY_IPHONE
//             StartCoroutine (CreatingMap.Instance.GetGPS());
//             here_lng = Input.location.lastData.longitude;
//             here_lat = Input.location.lastData.latitude;
// #elif UNITY_EDITOR
            here_lng = test_lng;
            here_lat = test_lat;
// #endif
            interval = 10;
            //Debug.Log("commentLoad");
            //if (StateManager.Instance.commentList != null)Debug.Log("commentNum"+StateManager.Instance.commentList.Length);
            //if (StateManager.Instance.commentList != null)Debug.Log("commentNum"+StateManager.Instance.commentList[3].comment_body);
        }
        interval -= Time.deltaTime;
    }


    void Start() {
        CheckInstance();
        StateManager.Instance.ObserveEveryValueChanged(x => x.uiState).Subscribe( _ => WindowChange());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingState).Subscribe( _ => StatusBarManager());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingRequestStatus).Subscribe( _ => LoadComment());
        if (StateManager.Instance.userid == 0)RequestSender.Instance.SubmitMakeIDRequest(); //ユーザーidないとき
    }

    public void StatusBarManager() {//MatchingStateが変わった時よばれる
        StatusBar.SetActive((StateManager.Instance.matchingState == MatchingState.Matching));
        if (StateManager.Instance.matchingState == MatchingState.Matched) {
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
        if (uiState == UIState.WriteComment)CreatingMap.Instance.GetMap(here_lat,here_lng);
        if (uiState == UIState.Map) {   //マップに遷移した時はコメント読みこみ
            CreatingMap.Instance.GetMap(here_lat,here_lng);
            CreatingMap.Instance.UpDateComment();
        }
    }

    public void SubmitMatchingRequest() {
        if (SendingMessage.text != "") {
            MatchingRequest.RequestData data = new MatchingRequest.RequestData();
            data.userid = StateManager.Instance.userid; //送るid
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
        RequestSender.Instance.SubmitAddressToGeometryRequest(Adress.text);
        StateManager .Instance.ObserveEveryValueChanged(x => x.addressToGeometryRequestStatus).Subscribe( _ => CompleteSettingRequest());
    }
    public void CompleteSettingRequest(){
        CreatingMap.Instance.GetMap(StateManager.Instance.ad_lat,StateManager.Instance.ad_lng);
    }

    public void GetAddressLatLng() {
        if (StateManager.Instance.addressToGeometryRequestStatus == RequestStatus.Success) {
            SettingConfirmRequest.RequestData data = new SettingConfirmRequest.RequestData();
            data.obj_lat = StateManager.Instance.ad_lat;
            data.obj_lng = StateManager.Instance.ad_lng;
            RequestSender.Instance.SubmitSettingConfirmRequest(data);
        }
    }

    public void SubmitCommentLikeFightAdd(){
        //RequestSender.Instance.SubmitAddressToGeometryRequest(Adress.text);
        //StateManager .Instance.ObserveEveryValueChanged(x => x.addressToGeometryRequestStatus).Subscribe( _ => CompleteSettingRequest());
    }

    public void CompleteSendLikeandGettingComment(){

        CreatingMap.Instance.UpDateComment();
    }


}
