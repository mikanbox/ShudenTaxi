using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UICusor : MonoBehaviour {

    public string Comment;
    public int id;
    public int like;
    public int fight;
    public bool isLiked = false;
    public bool isFighted = false;

    public void OnTap() {
        if (ItemWindow.userid != id) {
            GameObject tmp = Instantiate(Resources.Load("Prefabs/ItemWindow")) as GameObject;
            tmp.GetComponent<ItemWindow>().setText("exp", Comment);
            tmp.GetComponent<ItemWindow>().setText("exp1num", "" + fight);
            tmp.GetComponent<ItemWindow>().setText("exp2num", "" + like);
            tmp.GetComponent<ItemWindow>().child = fightPush;
            tmp.GetComponent<ItemWindow>().child2 = likePush;
            tmp.GetComponent<ItemWindow>().itemNo = id;
            if (isFighted==true)tmp.GetComponent<ItemWindow>().setColor("icon1",new Color(0.9f,0.6f,0.3f));
            if (isLiked == true)tmp.GetComponent<ItemWindow>().setColor("icon2",new Color(0.9f,0.6f,0.3f));
        }
    }


    public void fightPush(int i, GameObject win) {
        if (isFighted ==false){
            fight++;
            UIManager.Instance.SubmitCommentLikeFightAdd(LikeFightSendRequest.CommentType.fight,id);
            win.GetComponent<ItemWindow>().setText("exp1num", "" + (fight)  );
            win.GetComponent<ItemWindow>().setColor("icon1",new Color(0.9f,0.6f,0.3f));
            isFighted = true;
        }
    }

    public void likePush(int i, GameObject win) {
        if (isLiked ==false){
            like++;
            UIManager.Instance.SubmitCommentLikeFightAdd(LikeFightSendRequest.CommentType.like,id);
            win.GetComponent<ItemWindow>().setText("exp2num", "" + (like)  );
            win.GetComponent<ItemWindow>().setColor("icon2",new Color(0.9f,0.6f,0.3f));
            isLiked = true;
        }
    }





}
