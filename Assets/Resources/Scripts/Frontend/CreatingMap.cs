using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UniRx;


public class MeterPos {
    public float x;
    public float y;
}

public class CreatingMap : MonoBehaviour {
    private int zoom = 14;          //半径3kmぐらい
    private MeterPos pos = new MeterPos();
    public Image icon;
    public Image taxi;


    public static CreatingMap Instance;
    void Awake() {
        Instance = (CreatingMap)this;
        //GetMap(35.70118,139.71364);
    }

    public void GetMap (double lat, double lng) {
        StartCoroutine(GetStreetViewImage(lng, lat, zoom));
    }

    public void UpdateTaxi(float lat, float lng) {
        int distance = CalculateDistance(lat, lng);
        GameObject tmp = (GameObject)Instantiate (taxi.gameObject, GameObject.Find("CursorCanvas").transform);
        tmp.GetComponent<RectTransform>().localPosition +=  new Vector3(pos.x, pos.y, 0);
        //tmp.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/icon/HELP");
        tmp.SetActive(true);
        //Debug.Log("taxiIcon");
    }


    public void UpDateComment() {
        // foreach ( Transform n in icon.transform.parent ) { //子オブジェクトを全て破壊
        //     if (n.gameObject != icon.gameObject && n.gameObject != taxi.gameObject)GameObject.Destroy(n.gameObject);
        // }

        if (StateManager.Instance.commentList != null) {
            for (int i = 0; i < StateManager.Instance.commentList.Length; i++) {
                bool isexisted = false;
                foreach ( Transform n in icon.transform.parent ) {
                    if (n.gameObject != icon.gameObject && n.gameObject != taxi.gameObject) {
                        if (n.GetComponent<UICusor>().id == StateManager.Instance.commentList[i].id ) {
                            n.GetComponent<UICusor>().like  = StateManager.Instance.commentList[i].like;
                            n.GetComponent<UICusor>().fight = StateManager.Instance.commentList[i].fight;
                            isexisted = true;
                        }
                    }
                }
                if (isexisted == true)continue;
                pos = new MeterPos();
                int distance = CalculateDistance(StateManager.Instance.commentList[i].comment_lat, StateManager.Instance.commentList[i].comment_lng);
                GameObject tmp = (GameObject)Instantiate (icon.gameObject, GameObject.Find("CursorCanvas").transform);
                tmp.GetComponent<RectTransform>().localPosition +=  new Vector3(pos.x, pos.y, 0);
                tmp.GetComponent<UICusor>().Comment = StateManager.Instance.commentList[i].comment_body;
                tmp.GetComponent<UICusor>().id = StateManager.Instance.commentList[i].id;
                tmp.GetComponent<UICusor>().like  = StateManager.Instance.commentList[i].like;
                tmp.GetComponent<UICusor>().fight = StateManager.Instance.commentList[i].fight;
                tmp.SetActive(true);
            }
        } else {
            Debug.Log("Not Comment");
        }
    }

    private IEnumerator GetStreetViewImage(double longitude, double latitude, double zoom) {
        string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size="
                     + 400 + "x" + 400 + "&markers=size:mid%7Ccolor:red%7C" + latitude + "," + longitude;
        WWW www = new WWW(url);
        yield return www;
        GetComponent<Renderer>().material.mainTexture = www.texture;
    }

    static double deg2rad(double deg) {
        return (deg / 180.0) * Math.PI;
    }

    public int CalculateDistance(double lat, double lng)
    {
        double earth_r = 6378.137;
        double laRe, loRe, NSD, EWD, distance;

        laRe = deg2rad(lat - StateManager.Instance.here_lat);
        loRe = deg2rad(lng - StateManager.Instance.here_lng);

        NSD = earth_r * laRe;
        EWD = Math.Cos(deg2rad(lat)) * earth_r * loRe;
        distance = Math.Sqrt(Math.Pow(NSD, 2) + Math.Pow(EWD, 2));

        pos.x = (float)(NSD / 10000 * (System.Math.Pow(2f, zoom)) ) ;
        pos.y -= (float)(EWD / 10000 * (System.Math.Pow(2f, zoom)) );

        return (int)Math.Round(distance);
    }



}