using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UICusor : MonoBehaviour {

    public string Comment;

    public void OnTap() {
        GameObject tmp2 = Instantiate(Resources.Load("Prefabs/ItemWindow")) as GameObject;
        tmp2.GetComponent<ItemWindow>().setText("exp", Comment);
    }

}
