using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : SingletonMonoBehaviour<StateManager>
{
	//リクエストデータは取得不要だが、どこかでインスタンスを作らないといけないので、ここで保持
	private MakeIDRequest makeIDRequest;
	private MatchingRequest matchingRequest;
	private SettingConfirmRequest settinfConfirmRequest;
	private ChangeToSettingUIRequest changeToSettingUIRequest;
	private ChangeToCommentUIRequest changeToCommentUIRequest;

	//以下取得するもの
	public int userid { get; private set; }
	public float here_lat { get; private set; }
	public float here_lng { get; private set; }
	public float obj_lat { get; private set; }
	public float obj_lng { get; private set; }
	public float taxi_lat { get; private set; }
	public float taxi_lng { get; private set; }
	public Comment[] commentList { get; private set; }
	public UIState uiState { get; private set; }
	public MatchingState matchingState { get; private set; }

	// Use this for initialization
	protected override void Awake()
	{
		CheckInstance();
		MakeRequestInstance();
	}

	// Update is called once per frame
	void Update()
	{

	}


	private void MakeRequestInstance()
	{
		makeIDRequest = new MakeIDRequest();
		matchingRequest = new MatchingRequest();
		settinfConfirmRequest = new SettingConfirmRequest();
		changeToSettingUIRequest = new ChangeToSettingUIRequest();
		changeToCommentUIRequest = new ChangeToCommentUIRequest();
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

