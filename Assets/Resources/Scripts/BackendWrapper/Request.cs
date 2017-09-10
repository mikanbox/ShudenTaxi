using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public enum RequestStatus
{
	BeforeSend,
	Sending,
	Success,
	Failure,
}

public abstract class Request
{
	public Request()
	{
		status = RequestStatus.BeforeSend;
	}

	private RequestStatus _status;
	public RequestStatus status { get; protected set; }
}

public class MakeIDRequest : Request
{
	public static readonly string REQUEST_URL = BackendWrapperConf.APIURL + "/user/new";
	private UnityWebRequest request;
	private AsyncOperation webRequestStatus;
	private System.IDisposable requestEvent;

	public MakeIDRequest()
	{
		RequestSender.Instance.SubmitMakeIDRequest += OnSendRequest;
	}

	public class RequestAnswerData
	{
		public int userid;
	}

	private void OnSendRequest()
	{
		//前回のリクエストをが処理中の場合エラー
		if (status == RequestStatus.Sending)
		{
			Debug.LogWarning("you cannnot sending request many times.Please wait previous request end");
			return;
		}

		//リクエスト設定し、飛ばす。
		request = UnityWebRequest.Get(REQUEST_URL);
		webRequestStatus = request.Send();
		status = RequestStatus.Sending;

		//リクエストの結果を受け取る。
		requestEvent = this.ObserveEveryValueChanged(x => x.webRequestStatus.isDone)
			.Where(isDone => isDone == true)
			.Subscribe(isDone =>
			{
				if (request.isError)
				{
					Debug.Log(request.url);
					Debug.LogError(request.error);
					status = RequestStatus.Failure;
				}
				else
				{
					if (request.responseCode == 200)
					{
						string rawJson = request.downloadHandler.text;
						RequestAnswerData answer = JsonUtility.FromJson<RequestAnswerData>(rawJson);
						SetUserID(answer.userid);
						status = RequestStatus.Success;
					}
					else
					{
						Debug.LogError("MakeUserIDRequest:API doesn't return responseCode 200");
						status = RequestStatus.Failure;
					}
				}

				if (requestEvent != null)
					requestEvent.Dispose();
			});
	}

	//Action
	public System.Action<int> SetUserID;
}

public class MatchingRequest : Request
{
	public static readonly string REQUEST_URL = BackendWrapperConf.APIURL + "/matching/" + StateManager.Instance.userid;
	private UnityWebRequest request;
	private AsyncOperation webRequestStatus;
	private System.IDisposable requestEvent;

	public MatchingRequest()
	{
		RequestSender.Instance.SubmitMatchingRequest += OnSendRequest;
	}

	public class RequestData
	{
		public int userid;
		public float here_lat;
		public float here_lng;
		public float obj_lat;
		public float obj_lng;
		public string comment;
	}

	public class AnswerData
	{
		public Comment[] commentList;
		public bool isMatchingOK;
		public string taxi_number;
		public float? taxi_lat;
		public float? taxi_lng;
	}

	private void OnSendRequest(RequestData data)
	{
		//前回のリクエストをが処理中の場合エラー
		if (status == RequestStatus.Sending)
		{
			Debug.LogWarning("you cannnot sending request many times.Please wait previous request end");
			return;
		}

		var dataAsDictionary = MiniJSON.Json.Deserialize(JsonUtility.ToJson(data)) as Dictionary<string,string>;

		//リクエスト設定し、飛ばす。
		request = UnityWebRequest.Post(REQUEST_URL,dataAsDictionary);
		webRequestStatus = request.Send();
		status = RequestStatus.Sending;

		//リクエストの結果を受け取る。
		requestEvent = this.ObserveEveryValueChanged(x => x.webRequestStatus.isDone)
			.Where(isDone => isDone == true)
			.Subscribe(isDone =>
			{
				if (request.isError)
				{
					Debug.Log(request.url);
					Debug.LogError(request.error);
					status = RequestStatus.Failure;
				}
				else
				{
					if (request.responseCode == 200)
					{
						string rawJson = request.downloadHandler.text;
						var answer = MiniJSON.Json.Deserialize(rawJson) as Dictionary<string, string>;
						
						status = RequestStatus.Success;
					}
					else
					{
						Debug.LogError("MakeUserIDRequest:API doesn't return responseCode 200");
						status = RequestStatus.Failure;
					}
				}

				if (requestEvent != null)
					requestEvent.Dispose();
			
			});
	}
}

public class SettingConfirmRequest : Request
{
	public SettingConfirmRequest()
	{
		RequestSender.Instance.SubmitSettingConfirmRequest += OnSendRequest;
	}

	public class RequestData
	{
		public float obj_lat;
		public float obj_lng;
	}

	public void OnSendRequest(SettingConfirmRequest.RequestData data)
	{
		SetObjPostiion(data.obj_lat, data.obj_lng);
	}

	//Action
	public System.Action<float, float> SetObjPostiion;
}

public class ChangeToSettingUIRequest : Request
{
	public ChangeToSettingUIRequest()
	{
		RequestSender.Instance.SubmitChangeToSettingUIRequest += OnSendRequest;
	}

	public void OnSendRequest()
	{
		status = RequestStatus.Sending;
		status = RequestStatus.Success;
		SetUIState(UIState.Setting);
	}

	//Action
	public System.Action<UIState> SetUIState;
}

public class ChangeToCommentUIRequest : Request
{
	public ChangeToCommentUIRequest()
	{
		RequestSender.Instance.SubmitChangeToCommentUIRequest += OnSendRequest;
	}

	public void OnSendRequest()
	{
		status = RequestStatus.Sending;
		status = RequestStatus.Success;
		SetUIState(UIState.Setting);
	}

	//Action
	public System.Action<UIState> SetUIState;
}