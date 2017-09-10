using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow : MonoBehaviour {

    [SerializeField] public Dictionary<string,Text> Tlist = new Dictionary<string,Text>();
    [SerializeField] public Dictionary<string,GameObject> Oblist = new Dictionary<string,GameObject>();
    [SerializeField] private  Text title;
    [SerializeField] private  Text exp;
    [SerializeField] private  Text exp1;
    [SerializeField] private  Text exp1num;

    [SerializeField] private  Text exp2;
    [SerializeField] private  Text exp2num;
    //[SerializeField] private  Text middle;
    [SerializeField] private  Image icon1;
    [SerializeField] private  Image icon2;

    [SerializeField] private  GameObject button1;
    [SerializeField] private  GameObject button2;

    //[SerializeField] private  GameObject buyButton;



    public static ItemWindow isHd;
    public static int userid = -1;
    public int itemNo=-1;

    public delegate void Delefunc(int i,GameObject thisObj);
    public Delefunc child;
    public Delefunc child2;
    public Delefunc destchild;

	// Use this for initialization
	void Awake () {
		isHd = this.GetComponent<ItemWindow>();
        Tlist.Add("exp",exp);
        Tlist.Add("exp1",exp1);
        Tlist.Add("exp1num",exp1num);
        Tlist.Add("exp2",exp2);
        Tlist.Add("exp2num",exp2num);


        Oblist.Add("exp",exp.gameObject);
        Oblist.Add("exp1",exp1.gameObject);
        Oblist.Add("exp1num",exp1num.gameObject);
        Oblist.Add("exp2",exp2.gameObject);
        Oblist.Add("exp2num",exp2num.gameObject);
        Oblist.Add("Image",icon1.gameObject);
        Oblist.Add("Image2",icon2.gameObject);
        Oblist.Add("button1",button1.gameObject);
        Oblist.Add("button2",button2.gameObject);

	}
	
	public virtual void Onclick() {
        if (child!=null)child(itemNo,this.gameObject);
    }

    public virtual void Onclick2() {
        if (child2!=null)child2(itemNo,this.gameObject);
    }

    public bool OffObj(string key){
        bool ret=false;
        if (Oblist.ContainsKey( key )){
            ret =true;
            Oblist[key].SetActive(false);
        }
        return ret;
    }

    public bool setText(string key,string mes){
        bool ret=false;
        if (Tlist.ContainsKey( key )){
            ret =true;
            Tlist[key].text = mes;
        }
        return ret;
    }

    public bool setImage(string key ,string source){
        bool ret=false;
        if (Oblist.ContainsKey( key )){
            //Debug.Log("setcallback");
            ret =true;
            Oblist[key].GetComponent<Image>().sprite = Resources.Load<Sprite>(source);
            Oblist[key].SetActive(true);
        }
        return ret;
    }

    public bool setColor(string key ,Color color){
        bool ret=false;
        if (Oblist.ContainsKey( key )){
            //Debug.Log("setcallback");
            ret =true;
            Oblist[key].GetComponent<Image>().color = color;
            Oblist[key].SetActive(true);
        }
        return ret;
    }


    public void Destroythis() {
        userid = -1;
        if (destchild!=null)destchild(itemNo,this.gameObject);
        Destroy(this.gameObject);
    }
}
