using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class MatchingChecker : SingletonMonoBehaviour<MatchingChecker>
{
	//idが初回から変わる可能性があるのでconst,static readonly等にせずにgetアクセサで遅延評価。
	public string REQUEST_URL { 
		get {
			return BackendWrapperConf.APIURL + "/matching/" + StateManager.Instance.userid;
		} 
	}

	private const float POLLING_RATE_SECONDS = 5.0f;
	private float elapesedSeconds;
	private UnityWebRequest request;
	private AsyncOperation webRequestStatus;
	private System.IDisposable requestEvent;

	public class AnswerData {
		public bool result;
		public string taxi_number;
		public float taxi_lat;
		public float taxi_lng;
	}

	public System.Action<string, float, float> SetTaxiInfo;
	public System.Action<MatchingState> SetMatchingState;

	// Use this for initialization
	void Start()
	{
		elapesedSeconds = 0;
	}

	void Update()
	{
		if (StateManager.Instance.matchingState != MatchingState.Matching)
		{
			elapesedSeconds = 0;
			return;
		}
		elapesedSeconds += Time.deltaTime;

		bool isElapsedSeconds = elapesedSeconds > POLLING_RATE_SECONDS;
		bool isWebRequestNull = requestEvent == null;
		bool isPreviousWebRequestIsEnd = true;
		if (!isWebRequestNull)
			isPreviousWebRequestIsEnd = webRequestStatus.isDone;
		if (isElapsedSeconds && isPreviousWebRequestIsEnd)
		{
			elapesedSeconds = 0;
			//リクエスト設定し、飛ばす。
			//Debug.Log(REQUEST_URL);
			request = UnityWebRequest.Get(REQUEST_URL);
			webRequestStatus = request.Send();

			//リクエストの結果を受け取る。
			requestEvent = this.ObserveEveryValueChanged(x => x.webRequestStatus.isDone)
				.Where(isDone => isDone == true)
				.Subscribe(isDone =>
				{
					if (request.isNetworkError)
					{
						Debug.LogError(request.error);
					}
					else
					{
						if (request.responseCode == 200)
						{
							string rawJson = request.downloadHandler.text;
							//Debug.Log(rawJson);
							AnswerData answer = JsonUtility.FromJson<AnswerData>(rawJson);
							if (answer.result)
							{
								SetTaxiInfo(answer.taxi_number, answer.taxi_lat, answer.taxi_lng);
								SetMatchingState(MatchingState.Matched);
							}
						}
						else
						{
							Debug.LogError("MatchingChecker:API doesn't return responseCode 200");
						}
					}

					if (requestEvent != null)
						requestEvent.Dispose();
				});
		}
	}
}
