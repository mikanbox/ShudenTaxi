using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIManager : SingletonMonoBehaviour<UIManager> {

    [SerializeField]private InputField SendingMessage;
    [SerializeField]private InputField Adress;
    [SerializeField]private GameObject[] WindowObj;
    [SerializeField]private GameObject StatusBar;
    [SerializeField]private Text ErrorMes;

    private float here_lat,here_lng;
    private float test_lat = 139.73099f,test_lng = 35.70902f;

    private static float interval = 0;
    private UIState uiState = 0;

    void Update () {
        if (interval < 0) {
#if UNITY_ANDROID || UNITY_IPHONE
            StartCoroutine (CreatingMap.Instance.GetGPS());
            here_lng = Input.location.lastData.longitude;
            here_lat = Input.location.lastData.latitude;
#elif UNITY_EDITOR 
            here_lng = test_lng;
            here_lat = test_lat;
#endif
            if (uiState == UIState.Map)CreatingMap.Instance.UpDateComment();
            interval = 60;
            //Debug.Log("commentLoad");
        }
        interval -= Time.deltaTime;
    }


    void Start() {
        CheckInstance();
        StateManager.Instance.ObserveEveryValueChanged(x => x.uiState).Subscribe( _ => WindowChange());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingState).Subscribe( _ => StatusBarManager());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingRequestStatus).Subscribe( _ => LoadComment());
        if (StateManager.Instance.userid==0)RequestSender.Instance.SubmitMakeIDRequest();//ユーザーidないとき
    }

    public void StatusBarManager() {//MatchingStateが変わった時よばれる
        StatusBar.SetActive((StateManager.Instance.matchingState == MatchingState.Matching));
        if (StateManager.Instance.matchingState == MatchingState.Matched){
            CreatingMap.Instance.UpdateTaxi(StateManager.Instance.taxi_lat,StateManager.Instance.taxi_lng);
        }
    }

    public void LoadComment(){
        //if (StateManager.Instance.matchingRequestStatus==RequestStatus.Failure){
            //CreatingMap.Instance.UpDateComment();
            //Debug.Log("TriggercommentLoad");
        //}
    }

    private void WindowChange() {
        WindowObj[(int)uiState].SetActive(false);
        WindowObj[(int)StateManager.Instance.uiState].SetActive(true);
        Debug.Log("from:" + uiState + "to:" + StateManager.Instance.uiState);
        uiState = StateManager.Instance.uiState;
        if (uiState == UIState.Map){    //マップに遷移した時はコメント読みこみ
            CreatingMap.Instance.GetMap();
            CreatingMap.Instance.UpDateComment();
        }
    }

    public void SubmitMatchingRequest() {
        if (SendingMessage.text != "") {
            MatchingRequest.RequestData data = new MatchingRequest.RequestData();
            data.userid = 13;//StateManager.Instance.userid;
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
        GetLatLngfromString(Adress.text);
        SettingConfirmRequest.RequestData data = new SettingConfirmRequest.RequestData();
        data.obj_lat = 139.73199f;
        data.obj_lng = 35.70902f;
        RequestSender.Instance.SubmitSettingConfirmRequest(data);
    }


    private IEnumerator GetLatLngfromString(string adress) {
        string url = "http://maps.google.com/maps/api/geocode/json?address={0}&"+adress;
        WWW www = new WWW(url);
        yield return www;
        Debug.Log(www);
    }

    // var url = string.Format("http://maps.google.com/maps/api/geocode/json?address={0}",
    //                 HttpUtility.UrlEncode("松山市二番町2丁目9-1エフショコラビル2階"));

    // byte[] result;
    // using (var wc = new WebClient())
    // {
    //     result = wc.DownloadData(url);
    // }

    // var jsonString = Encoding.UTF8.GetString(result);

    // var placeInfo = DynamicJson.Parse(jsonString);
    // if (placeInfo.status == "OK")
    // {
    //     var location = placeInfo.results[0].geometry.location;
    //     Console.WriteLine(location.lat.ToString());
    //     Console.WriteLine(location.lng.ToString());
    // }

}
