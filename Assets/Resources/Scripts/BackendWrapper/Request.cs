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
					//Debug.Log(request.url);
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
	public static readonly string REQUEST_URL = BackendWrapperConf.APIURL + "/feeling";
	private UnityWebRequest request;
	private AsyncOperation webRequestStatus;
	private System.IDisposable requestEvent;
	private System.IDisposable matchingRequester;

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
		public Comment[] comments;
		public bool result;
	}

	public System.Action<Comment[]> SetComments;
	public System.Action<MatchingState> SetMatchingState;
	public System.Action<UIState> SetUIState;

	private void OnSendRequest(RequestData data)
	{
		//前回のリクエストをが処理中の場合エラー
		if (status == RequestStatus.Sending)
		{
			Debug.LogWarning("you cannnot sending request many times.Please wait previous request end");
			return;
		}

		WWWForm postData = new WWWForm();
		postData.AddField("userid", data.userid);
		postData.AddField("comment_body", data.comment);
		postData.AddField("here_lat", data.here_lat.ToString());
		postData.AddField("here_lng", data.here_lng.ToString());
		postData.AddField("obj_lat", data.obj_lat.ToString());
		postData.AddField("obj_lng", data.obj_lng.ToString());
		//dataAsDictionary = MiniJSON.Json.Deserialize(JsonUtility.ToJson(data)) as Dictionary<string,string>;

		//リクエスト設定し、飛ばす。
		request = UnityWebRequest.Post(REQUEST_URL, postData);
		webRequestStatus = request.Send();
		status = RequestStatus.Sending;

		//リクエストの結果を受け取る。
		requestEvent = this.ObserveEveryValueChanged(x => x.webRequestStatus.isDone)
			.Where(isDone => isDone == true)
			.Subscribe(isDone =>
			{
				if (request.isError)
				{
					//Debug.Log(request.url);
					Debug.LogError(request.error);
					status = RequestStatus.Failure;
				}
				else
				{
					if (request.responseCode == 200)
					{
						string rawJson = request.downloadHandler.text;
						var answer = JsonUtility.FromJson<AnswerData>(rawJson);
						if (!answer.result)
						{
							Debug.LogError("APIError: matching failure");
							status = RequestStatus.Failure;
							SetMatchingState(MatchingState.CannotSendMatchingRequest);
						}
						else
						{
							status = RequestStatus.Success;
							SetMatchingState(MatchingState.Matching);
							SetComments(answer.comments);
							SetUIState(UIState.Map);
						}
					}
					else
					{
						Debug.LogError("MatchingRequest:API doesn't return responseCode 200");
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
		SetUIState(UIState.Title);
	}

	//Action
	public System.Action<float, float> SetObjPostiion;
	public System.Action<UIState> SetUIState;
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
		SetUIState(UIState.WriteComment);
	}

	//Action
	public System.Action<UIState> SetUIState;
}

public class AddressToGeometryRequest : Request
{
	public static readonly string GOOGLE_API_URL = "http://maps.google.com/maps/api/geocode/json?address={0}";
	private UnityWebRequest request;
	private AsyncOperation webRequestStatus;
	private System.IDisposable requestEvent;

	public AddressToGeometryRequest()
	{
		RequestSender.Instance.SubmitAddressToGeometryRequest += OnSendRequest;
	}

	[System.Serializable]
	public class RequestAnswerData
	{
		public GoogleAPIResult[] results;
		public string status;
	}

	[System.Serializable]
	public class GoogleAPIResult
	{
		public AddressComponent[] address_components;
		public string formatted_address;
		public Geometry geometry;
		public bool partial_match;
		public string place_id;
		public string[] types;
	}

	[System.Serializable]
	public class AddressComponent
	{
		public string long_name;
		public string short_name;
		public string[] types;
	}

	[System.Serializable]
	public class Geometry
	{
		public Location location;
		public string location_type;
		public ViewPort viewPort;
	}

	[System.Serializable]
	public class Location
	{
		public float lat;
		public float lng;
	}

	[System.Serializable]
	public class ViewPort
	{
		public Location northeast;
		public Location southwest;
	}


	private void OnSendRequest(string address)
	{

		var REQUEST_URL = GOOGLE_API_URL + address;
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
					Debug.LogError(request.error);
					status = RequestStatus.Failure;
				}
				else
				{
					if (request.responseCode == 200)
					{
						string rawJson = request.downloadHandler.text;
						RequestAnswerData answer = JsonUtility.FromJson<RequestAnswerData>(rawJson);
						Debug.Log(answer.results[0].geometry.location.lat);
						SetAddressPosition(answer.results[0].geometry.location.lat, answer.results[0].geometry.location.lng);
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
	public System.Action<float, float> SetAddressPosition;
}

public class CountNogashiTimesRequest : Request { 

	public CountNogashiTimesRequest()
	{
		RequestSender.Instance.SubmitCountNogashiTimesRequest += OnSendRequest;
	}

	public void OnSendRequest()
	{
		status = RequestStatus.Sending;
		SetNogashiTimes(StateManager.Instance.nogashiTimes + 1);
		SetUIState(UIState.Title);
		status = RequestStatus.Success;
	}

	//Action
	public System.Action<int> SetNogashiTimes;
	public System.Action<UIState> SetUIState;

}