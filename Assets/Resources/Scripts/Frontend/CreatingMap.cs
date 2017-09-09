using UnityEngine;
using System.Collections;

public class CreatingMap : MonoBehaviour {
    GUITexture mapGuiTexture;
    private int width = 400;
    private int height = 800;
    private double longitudeMetor;
    private double latitudeMetor;

    private int zoom = 6;
    void Start () {
        mapGuiTexture = this.GetComponent<GUITexture> ();
        //GetMap();
    }

    void GetMap () {
        StartCoroutine(GetStreetViewImage(45, 135, zoom));
    }


    private IEnumerator GetStreetViewImage(double latitude, double longitude, double zoom) {
        　　　　　　　　//現在地マーカーはここの「&markers」以下で編集可能
        string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height + "&markers=size:mid%7Ccolor:red%7C" + latitude + "," + longitude;
        WWW www = new WWW(url);
        yield return www;
        　　　　　　　　//マップの画像をTextureとして貼り付ける
        mapGuiTexture.texture = www.texture;
        　　　　　　　　//ミニマップに透明度を与える
        mapGuiTexture.color = new Color (mapGuiTexture.color.r, mapGuiTexture.color.g, mapGuiTexture.color.b, 0.4f);
    }

    private void GetlatMeter(double latitude, double longitude) {
        // laRe = (B.latitude - A.latitude)/M_PI * 180.0;
        // loRe = (B.longitude - A.longitude)/M_PI * 180.0;
        // longitudeMetor = earth_r * laRe;
        // latitudeMetor = cos(deg2rad(A.latitude)) * earth_r * loRe;
    }

    private int getRealDistance(int metre) {
        double equator = 40075334.2563;
        double disPerPix = equator / 256;
        double disInPix = metre * (System.Math.Pow(2, zoom)) / disPerPix;
        return (int) disInPix;
    }



}