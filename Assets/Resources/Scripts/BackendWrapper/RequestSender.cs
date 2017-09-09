using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestSender : SingletonMonoBehaviour<RequestSender> {

	public System.Action SubmitMakeIDRequest;
	public System.Action<MatchingRequest.RequestData> SubmitMatchingRequest;
	public System.Action<SettingConfirmRequest.RequestData> SubmitSettingConfirmRequest;
	public System.Action SubmitChangeToSettingUIRequest;
	public System.Action SubmitChangeToCommentUIRequest;
}


