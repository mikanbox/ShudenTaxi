using UnityEngine;
using System.Collections;
using System;

public class Positions{
    public double Latitude;
    public double Longitude;
}


public class CreatingMap : MonoBehaviour {
    GUITexture mapGuiTexture;
    private int width = 400;
    private int height = 800;
    private double longitudeMetor;
    private double latitudeMetor;
    private int zoom = 13;          //半径3kmぐらい
    private string AddComment;
    private Positions GPS;

    public static CreatingMap Instance;
    void Awake()
    {
        Instance = (CreatingMap)this;
        mapGuiTexture = this.GetComponent<GUITexture> ();
    }

    public void GetMap () {
        StartCoroutine (GetGPS());
        GPS.Longitude = Input.location.lastData.longitude;
        GPS.Latitude = Input.location.lastData.latitude;

        AddComment = "";
        // for (int i=0;i<StateManager.Instance.commentList.Length;i++){
        //     AddComment += "&markers=size:mid%7Ccolor:blue%7C" + StateManager.Instance.commentList[i].lat + "," + StateManager.Instance.commentList[i].lng;
        // }
        StartCoroutine(GetStreetViewImage(GPS.Longitude, GPS.Latitude, zoom));
    }

    private IEnumerator GetStreetViewImage(double latitude, double longitude, double zoom) {
        //現在地マーカーはここの「&markers」以下で編集可能
        string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height +
                     "&markers=size:mid%7Ccolor:red%7C" + latitude + "," + longitude + AddComment;
        WWW www = new WWW(url);
        yield return www;
        mapGuiTexture.texture = www.texture;
        mapGuiTexture.color = new Color (mapGuiTexture.color.r, mapGuiTexture.color.g, mapGuiTexture.color.b, 0.4f);
    }

    private IEnumerator GetGPS() {
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


    static double deg2rad(double deg)
    {
        return (deg / 180) * Math.PI;
    }

    public static int CalculateDistance(Positions posA, Positions posB)
    {
        // 2点の緯度の平均
        double latAvg = deg2rad(posA.Latitude + ((posB.Latitude - posA.Latitude) / 2));
        // 2点の緯度差
        double latDifference = deg2rad(posA.Latitude - posB.Latitude);
        // 2点の経度差
        double lonDifference = deg2rad(posA.Longitude - posB.Longitude);

        double curRadiusTemp = 1 - 0.00669438 * Math.Pow(Math.Sin(latAvg), 2);
        // 子午線曲率半径
        double meridianCurvatureRadius = 6335439.327 / Math.Sqrt(Math.Pow(curRadiusTemp, 3));
        // 卯酉線曲率半径
        double primeVerticalCircleCurvatureRadius = 6378137 / Math.Sqrt(curRadiusTemp);

        // 2点間の距離
        double distance = Math.Pow(meridianCurvatureRadius * latDifference, 2)
                          + Math.Pow(primeVerticalCircleCurvatureRadius * Math.Cos(latAvg) * lonDifference, 2);

        distance = Math.Sqrt(distance);

        return (int)Math.Round(distance);
    }



    private int getRealDistance(int meter) {
        double equator = 40075334.2563;
        double disPerPix = equator / 256;
        double disInPix = meter * (System.Math.Pow(2, zoom)) / disPerPix;
        return (int) disInPix;
    }



}