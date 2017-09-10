using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiCommingChecker : SingletonMonoBehaviour<TaxiCommingChecker> {

	private float elapsedSeconds;
	private const float TAXI_COMING_TIME = 10f;

	// Use this for initialization
	void Start () {
		elapsedSeconds = 0;
	}

	public System.Action<MatchingState> SetMatchingState;
	
	// Update is called once per frame
	void Update () {
		if (StateManager.Instance.matchingState != MatchingState.Matched)
		{
			elapsedSeconds = 0;
			return;
		}
		elapsedSeconds += Time.deltaTime;

		if (elapsedSeconds > TAXI_COMING_TIME)
		{
			SetMatchingState(MatchingState.TaxiCome);
		}
	}
}
