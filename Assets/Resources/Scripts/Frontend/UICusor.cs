using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UICusor : MonoBehaviour {

    public string Comment;
    public int userid;
    public int like;
    public int fight;
    public bool isLiked = false;
    public bool isFighted = false;

    public void OnTap() {
        if (ItemWindow.userid != userid) {
            GameObject tmp = Instantiate(Resources.Load("Prefabs/ItemWindow")) as GameObject;
            tmp.GetComponent<ItemWindow>().setText("exp", Comment);
            tmp.GetComponent<ItemWindow>().setText("exp1num", "" + fight);
            tmp.GetComponent<ItemWindow>().setText("exp2num", "" + like);
            tmp.GetComponent<ItemWindow>().child = fightPush;
            tmp.GetComponent<ItemWindow>().child2 = likePush;
            tmp.GetComponent<ItemWindow>().itemNo = userid;
            //tmp.GetComponent<ItemWindow>().setColor("Image",new Color(0.9f,0.6f,0.3f));
            //tmp.GetComponent<ItemWindow>().setColor("Image2",new Color(0.9f,0.6f,0.3f));
        }
    }

    public void fightPush(int i, GameObject win) {
        //win.GetComponent<ItemWindow>().setText("exp1num", "" + fight);
    }

    public void likePush(int i, GameObject win) {
        //win.GetComponent<ItemWindow>().setText("exp2num", "" + like);
    }





}
