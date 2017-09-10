using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UniRx;

public class Positions {
    public double Latitude;
    public double Longitude;
}

public class MeterPos {
    public float x;
    public float y;
}

public class CreatingMap : MonoBehaviour {
    private int zoom = 14;          //半径3kmぐらい
    private Positions GPS = new Positions();
    private MeterPos pos = new MeterPos();
    public Image icon;
    public Image taxi;


    public static CreatingMap Instance;
    void Awake() {
        GPS.Longitude = 35.70902;
        GPS.Latitude = 139.73199;
        Instance = (CreatingMap)this;
        //GetMap();
    }

    public void GetMap () {
        StartCoroutine(GetStreetViewImage(GPS.Longitude, GPS.Latitude, zoom));
        //UpDateComment();
    }

    public void UpdateTaxi(float lat, float lng) {
        int distance = CalculateDistance(lat, lng);
        GameObject tmp = (GameObject)Instantiate (taxi.gameObject, GameObject.Find("CursorCanvas").transform);
        tmp.GetComponent<RectTransform>().localPosition +=  new Vector3(pos.x, pos.y, 0);
        tmp.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/icon/HELP");
        tmp.SetActive(true);
        Debug.Log("taxiIcon");
    }


    public void UpDateComment() {
        foreach ( Transform n in icon.transform.parent ) { //子オブジェクトを全て破壊
            if (n.gameObject != icon.gameObject && n.gameObject != taxi.gameObject)GameObject.Destroy(n.gameObject);
        }

        //if (StateManager.Instance.commentList != null)Debug.Log("commentNum"+StateManager.Instance.commentList[3].comment_body);
        if (StateManager.Instance.commentList != null) {
            for (int i = 0; i < StateManager.Instance.commentList.Length; i++) {
                pos = new MeterPos();
                //Debug.Log("Comlat"+StateManager.Instance.commentList[i].comment_lat);
                int distance = CalculateDistance(StateManager.Instance.commentList[i].comment_lat, StateManager.Instance.commentList[i].comment_lng);
                GameObject tmp = (GameObject)Instantiate (icon.gameObject, GameObject.Find("CursorCanvas").transform);
                tmp.GetComponent<RectTransform>().localPosition +=  new Vector3(pos.x, pos.y, 0);
                tmp.GetComponent<UICusor>().Comment = StateManager.Instance.commentList[i].comment_body;
                tmp.SetActive(true);
            }
        }else{
            Debug.Log("Not Comment");
        }


        // pos = new MeterPos();
        // int distance = CalculateDistance(139.73372,35.70663);
        // GameObject tmp = (GameObject)Instantiate (icon.gameObject, GameObject.Find("CursorCanvas").transform);
        // tmp.GetComponent<RectTransform>().localPosition +=  new Vector3(pos.x, pos.y, 0);
        // tmp.SetActive(true);

        // pos = new MeterPos();
        // distance = CalculateDistance(GPS.Latitude, GPS.Longitude);
        // tmp = (GameObject)Instantiate (icon.gameObject, GameObject.Find("CursorCanvas").transform);
        // tmp.GetComponent<RectTransform>().localPosition +=  new Vector3(pos.x, pos.y, 0);
        // tmp.SetActive(true);

    }

    private IEnumerator GetStreetViewImage(double latitude, double longitude, double zoom) {
        string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + 400 + "x" + 400 + "";
        WWW www = new WWW(url);
        yield return www;
        GetComponent<Renderer>().material.mainTexture = www.texture;
    }

    public IEnumerator GetGPS() {
        if (!Input.location.isEnabledByUser) {
            yield break;
        }
        Input.location.Start();
        int maxWait =  120;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1) {
            print("Timed out");
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed) {
            print("Unable to determine device location");
            yield break;
        } else {
            print("Location: " +
                  Input.location.lastData.latitude + " " +
                  Input.location.lastData.longitude + " " +
                  Input.location.lastData.altitude + " " +
                  Input.location.lastData.horizontalAccuracy + " " +
                  Input.location.lastData.timestamp);
        }
        Input.location.Stop();
    }


    static double deg2rad(double deg){
        return (deg / 180.0) * Math.PI;
    }

    public int CalculateDistance(double lat, double lng)
    {
        double earth_r = 6378.137;
        double laRe, loRe, NSD, EWD, distance;

        laRe = deg2rad(lat - GPS.Latitude);
        loRe = deg2rad(lng - GPS.Longitude);

        NSD = earth_r * laRe;
        EWD = Math.Cos(deg2rad(lat)) * earth_r * loRe;
        distance = Math.Sqrt(Math.Pow(NSD, 2) + Math.Pow(EWD, 2));

        pos.x = (float)(NSD / 10000 * (System.Math.Pow(2f, zoom)) ) ;
        pos.y -= (float)(EWD / 10000 * (System.Math.Pow(2f, zoom)) );

        return (int)Math.Round(distance);
    }



}