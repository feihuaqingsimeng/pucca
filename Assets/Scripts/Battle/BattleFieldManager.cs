using UnityEngine;
using System.Collections;

public class BattleFieldManager : MonoBehaviour
{
    public float    GroundPos_Up = -2.2f;
    public float    GroundPos_Down = -2.4f;
    public float    GroundPos_Center = -2.3f;
    
    public float    WallPos_Left = -5.3f;
    public float    WallPos_Right = 5.3f;

    //설정데이터.
    public float    StartPosLength = 8.0f;     //시작 지점의 거리.(중앙부터)
    public float    FirstMoveLength = 7.5f;     //시작 지점의 거리.(중앙부터)
    public float    SpawnRange = 0.7f;         //스폰 거리.


    public SpriteRenderer       pBackgroundSprite;
    
    
    //PVE 신규.
    public ScrollFieldManager   pBackground_PVE;






    public void LoadBattleField(int FieldNumber)
    {
        string AB_url = Kernel.entry.battle.AssetBundleURL_PVP_Field;
        int AB_Ver = Kernel.entry.battle.AssetBundleVer_PVP_Field;
        AssetBundle Bundle = AssetBundleManager.getAssetBundle(AB_url, AB_Ver);

        Sprite pFieldImg = Bundle.LoadAsset<Sprite>("BattleField_" + FieldNumber.ToString());
        pBackgroundSprite.sprite = pFieldImg;

        pBackground_PVE = null;
    }

    public void LoadBattleField(string FieldName)
    {
        string AB_url = Kernel.entry.battle.AssetBundleURL_PVP_Field;
        int AB_Ver = Kernel.entry.battle.AssetBundleVer_PVP_Field;
        AssetBundle Bundle = AssetBundleManager.getAssetBundle(AB_url, AB_Ver);

        Sprite pFieldImg = Bundle.LoadAsset<Sprite>(FieldName);
        pBackgroundSprite.sprite = pFieldImg;

        pBackground_PVE = null;
    }


    public void LoadBattleField_PVE(Transform CamTarget, string FieldName)
    {
        GameObject pPVE_Field = null;

//        pPVE_Field = Instantiate(Resources.Load("Prefabs/Battle/PVE_Field/" + FieldName)) as GameObject;

        string  AB_url = Kernel.entry.battle.AssetBundleURL_PVE_Field;
        int     AB_Ver = Kernel.entry.battle.AssetBundleVer_PVE_Field;
        AssetBundle Bundle = AssetBundleManager.getAssetBundle(AB_url, AB_Ver);

        pPVE_Field = Instantiate(Bundle.LoadAsset<GameObject>(FieldName));
        pPVE_Field.transform.parent = pBackgroundSprite.transform.parent;
        pPVE_Field.transform.localPosition = new Vector3(0.0f, -0.8f, 10.0f);
        pPVE_Field.transform.localScale = Vector3.one;

        pBackground_PVE = pPVE_Field.GetComponent<ScrollFieldManager>();
        pBackground_PVE.InitScrollFieldManager(CamTarget);

        pBackgroundSprite.gameObject.SetActive(false);

    }






	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
