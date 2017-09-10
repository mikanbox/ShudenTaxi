using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSRunner : SingletonMonoBehaviour<GPSRunner> {

	// Use this for initialization
	void Start () {
		Input.location.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.location.status == LocationServiceStatus.Running)
		{
			var lat = Input.location.lastData.latitude;
			var lng = Input.location.lastData.longitude;
			SetHereLocation(lat, lng);
		}
	}

	public System.Action<float, float> SetHereLocation;
}
