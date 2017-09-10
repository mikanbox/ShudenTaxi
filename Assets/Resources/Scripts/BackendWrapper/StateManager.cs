using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : SingletonMonoBehaviour<StateManager>
{
	private const string PLAYER_PREFS_USER_ID_KEY = "userid";
	private const string PLAYER_PREFS_OBJ_LAT_KEY = "obj_lat";
	private const string PLAYER_PREFS_OBJ_LNG_KEY = "obj_lng";

	//リクエストデータは取得不要だが、どこかでインスタンスを作らないといけないので、ここで保持
	private MakeIDRequest makeIDRequest;
	private MatchingRequest matchingRequest;
	private SettingConfirmRequest settingConfirmRequest;
	private ChangeToSettingUIRequest changeToSettingUIRequest;
	private ChangeToCommentUIRequest changeToCommentUIRequest;

	//以下取得するもの
	public int? userid { get; private set; }
	public float? here_lat { get; private set; }
	public float? here_lng { get; private set; }
	public float? obj_lat { get; private set; }
	public float? obj_lng { get; private set; }
	public string taxi_number { get; private set;}
	public float? taxi_lat { get; private set; }
	public float? taxi_lng { get; private set; }
	public Comment[] commentList { get; private set; }
	public UIState uiState { get; private set; }
	public MatchingState matchingState { get; private set; }

	public RequestStatus makeIDRequestStatus { get { return makeIDRequest.status; } }
	public RequestStatus matchingRequestStatus { get { return matchingRequest.status; } }
	public RequestStatus settingConfirmRequestStatus { get { return settingConfirmRequest.status; } }
	public RequestStatus changeToSettingUIRequestStatus { get { return changeToSettingUIRequest.status;} }
	public RequestStatus changeToCommentUIRequestStatus { get { return changeToCommentUIRequest.status;} }

	// Use this for initialization
	protected override void Awake()
	{
		CheckInstance();
		MakeRequestInstance();
		SetRequestEvent();
        LoadData();
	}

	private void Start() {
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void MakeRequestInstance()
	{
		makeIDRequest = new MakeIDRequest();
		matchingRequest = new MatchingRequest();
		settingConfirmRequest = new SettingConfirmRequest();
		changeToSettingUIRequest = new ChangeToSettingUIRequest();
		changeToCommentUIRequest = new ChangeToCommentUIRequest();
	}

	private void SetRequestEvent() {
		makeIDRequest.SetUserID += (id) => {
			userid = id;
			PlayerPrefs.SetInt(PLAYER_PREFS_USER_ID_KEY, id);
			PlayerPrefs.Save();
		};
		settingConfirmRequest.SetObjPostiion += (lat, lng) => { 
			obj_lat = lat; obj_lng = lng;
			PlayerPrefs.SetFloat(PLAYER_PREFS_OBJ_LAT_KEY, lat);
			PlayerPrefs.SetFloat(PLAYER_PREFS_OBJ_LNG_KEY, lng);
		};
		changeToCommentUIRequest.SetUIState += (UIState obj) => uiState = obj;
		changeToSettingUIRequest.SetUIState += (UIState obj) => uiState = obj;
	}

	private void LoadData()
	{
		bool hasUserID = PlayerPrefs.HasKey(PLAYER_PREFS_USER_ID_KEY);
		if (hasUserID)
			userid = PlayerPrefs.GetInt(PLAYER_PREFS_USER_ID_KEY);

		bool hasObj = PlayerPrefs.HasKey(PLAYER_PREFS_OBJ_LAT_KEY) && PlayerPrefs.HasKey(PLAYER_PREFS_OBJ_LNG_KEY);
		if (hasObj)
		{
			obj_lat = PlayerPrefs.GetFloat(PLAYER_PREFS_OBJ_LAT_KEY);
			obj_lng = PlayerPrefs.GetFloat(PLAYER_PREFS_OBJ_LNG_KEY);
		}
	}
}

public class Comment
{
	public int id;
	public string content;
	public float lat;
	public float lng;
}

public enum UIState
{
	Title,
	WriteComment,
	Map,
	Setting
}

public enum MatchingState
{
	BeforeMatching,
	Matching,
	Matched,
	TaxiCome,
}

