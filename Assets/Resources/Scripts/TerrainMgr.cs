using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainMgr : MonoBehaviour {

    public Terrain St;
    public TerrainData TerrainD;
    public TerrainData TerrainDats;
    public TerrainCollider Terrainc;

    private float _relief = 0.51f;

	// Use this for initialization
	void Start () {
        Debug.Log("treeInstanceCount"+TerrainD.treeInstanceCount);
        int x=2;
        int y=1;
        float[,] height = new float[10,10];
        for (int i=0;i<10;i++){
            for(int j=0;j<10;j++){
                float _seedX = Random.value * 100f;
                float _seedZ = Random.value * 100f;

                float xHeight = (_seedX) / _relief;
                float yHeight = (_seedZ) / _relief;

                height[i,j] =  Mathf.PerlinNoise(xHeight, yHeight);
            }
        }

        TerrainDats = new TerrainData();
        //TerrainDats = St.terrainData;
        TerrainDats.SetHeights(3,3,height);

        //TerrainD.SetHeights(0,0,height);
        St.terrainData = TerrainDats;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
