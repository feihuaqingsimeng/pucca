using UnityEngine;
using Common.Packet;
using System.Collections;
using System.Collections.Generic;
using System;

public class Detect : Node
{
    public string DetectFieldName
    {
        get;
        set;
    }

    public int DetectBoxIndex
    {
        get;
        private set;
    }

    public int islandNum
    {
        get;
        private set;
    }

    public delegate void OnDetectStartAR();
    public OnDetectStartAR onDetectStartAR;

    public delegate void OnDetectOpenBox(PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK packet);
    public OnDetectOpenBox onDetectOpenBox;

    public delegate void OnUpdateIslandInfo();
    public OnUpdateIslandInfo onUpdateIslandInfo;


    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_START_TREASURE_DETECT_ACK>(RCV_PACKET_CG_GAME_START_TREASURE_DETECT_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK>(RCV_PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK);


        return base.OnCreate();
    }





    //보물찾기 시작.
    public void REQ_PACKET_CG_GAME_START_TREASURE_DETECT_SYN(int IslandNum)
    {
        islandNum = IslandNum;

        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_START_TREASURE_DETECT_SYN() 
        {
            m_iIslandNumber = IslandNum,
        });
    }


    public void RCV_PACKET_CG_GAME_START_TREASURE_DETECT_ACK(PACKET_CG_GAME_START_TREASURE_DETECT_ACK packet)
    {
        Log("{0}", packet.Result);

        Kernel.entry.account.SetValue(packet.m_RemainGoods.m_eGoodsType, packet.m_RemainGoods.m_iRemainAmount);
        DetectBoxIndex = packet.m_iDetectedBoxIndex;

        onDetectStartAR();
    }





    //보물찾기 결과.
    public void REQ_PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_SYN());
    }



    public void RCV_PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK(PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK packet)
    {
        if (onDetectOpenBox != null)
            onDetectOpenBox(packet);
    }


}
