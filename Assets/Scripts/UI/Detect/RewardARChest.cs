using UnityEngine;
using System.Collections;
using Common.Packet;

public class RewardARChest : MonoBehaviour
{
    public GameObject  ChestParentObject;

    public Animator    ChestOpenAnimator;

	// Use this for initialization
	void Awake()
    {
	}

    public void MakeChest(int ChestIdx)
    {
        DB_TreasureDetectBoxGet.Schema BoxGetData = DB_TreasureDetectBoxGet.Query(DB_TreasureDetectBoxGet.Field.Index, ChestIdx);

        GameObject ChestObject = Instantiate(Resources.Load("Prefabs/Detect/" + BoxGetData.Box_IdentificationName)) as GameObject;
        ChestObject.transform.parent = ChestParentObject.transform;
        ChestObject.transform.localScale = Vector3.one;
        ChestObject.transform.localPosition = Vector3.zero;
        ChestObject.transform.localRotation = Quaternion.identity;

        ChestOpenAnimator = ChestObject.transform.FindChild("AR_RewardBox").GetComponent<Animator>();

        ChestParentObject.SetActive(false);
    }


	// Update is called once per frame
	void Update () 
    {
        transform.LookAt(Vector3.zero);	
	}

    public void SpawnChest()
    {
        ChestParentObject.SetActive(true);
        Invoke("PlayDropSound", 0.4f);
    }

    public void OpenChest()
    {
        Kernel.entry.detect.onDetectOpenBox += ResultDetectBox;
        Kernel.entry.detect.REQ_PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_SYN();
    }

    private void PlayDropSound()
    {
        Kernel.soundManager.PlayUISound(SOUND.SND_UI_TREASUREDETECT_BOX);
    }

    
    public void ResultDetectBox(PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK packet)
    {
        Kernel.entry.detect.onDetectOpenBox -= ResultDetectBox;
        Kernel.entry.account.gold = packet.m_iTotalGold;

        for (int i = 0; i < packet.m_CardList.Count; i++)
        {
            Kernel.entry.character.UpdateCardInfo(packet.m_CardList[i]);
        }

        for (int i = 0; i < packet.m_SoulList.Count; i++)
        {
            Kernel.entry.character.UpdateSoulInfo(packet.m_SoulList[i]);
        }


        UIDetectChestDirection chestDirector = Kernel.uiManager.Open<UIDetectChestDirection>(UI.DetectChestDirection);
        chestDirector.SetReward(packet.m_iEarnGold, packet.m_BoxResultList);
        chestDirector.DirectionByCoroutine();
        ChestOpenAnimator.SetTrigger("OpenChest");
    }

}
