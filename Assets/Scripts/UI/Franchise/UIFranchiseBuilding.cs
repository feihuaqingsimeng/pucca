using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFranchiseBuilding : MonoBehaviour 
{
    //** 빌딩 번호
    public int m_nBuildingNumber;

    //** 빌딩의 방들 중 가장 큰 방의 가로 사이즈가 빌딩의 가로 사이즈
    private float m_fBuildingWidth;
    public float BuildingWidth
    {
        get { return m_fBuildingWidth; }
        set
        {
            if (m_fBuildingWidth <= value)
                m_fBuildingWidth = value;
        }
    }

    //** 빌딩 높이
    private float m_fBuildingHeight;
    public float BuildingHeight
    {
        get { return m_fBuildingHeight; }
        set { m_fBuildingHeight = value; }
    }

    //대문과 지붕
    public RectTransform m_rtrsRoofPosition;
    public RectTransform m_rtrsGatePosition;

    public RectTransform m_rtrsRoofImage;

    //** 방
    private List<UIFranchiseRoom>    m_listRooms;
    public  UIFranchiseRoom          m_RoomPrefab;
    public  RectTransform            m_rtrsRoomParent;

    private void Start()
    {
        if (m_RoomPrefab.gameObject.activeInHierarchy)
            m_RoomPrefab.gameObject.SetActive(false);
    }

    //** 방 만들기.
    public void CreateRooms(List<FranchiseRoomData> roomDatas)
    {
        if (roomDatas == null)
            return;

        if (m_listRooms == null)
            m_listRooms = new List<UIFranchiseRoom>();

        if (m_rtrsRoomParent != null && m_rtrsGatePosition != null)
            m_rtrsRoomParent.anchoredPosition = new Vector2(0.0f, m_rtrsGatePosition.sizeDelta.y);

        // 방 생성
        for (int i = 0; i < roomDatas.Count; i++)
        {
            FranchiseRoomData roomData = roomDatas[i];

            UIFranchiseRoom newRoom = Instantiate<UIFranchiseRoom>(m_RoomPrefab);
            UIUtility.SetParent(newRoom.transform, m_rtrsRoomParent.transform);

            newRoom.gameObject.SetActive(true);
            newRoom.SetRoomData(this, roomData);

            m_listRooms.Add(newRoom);
        }

        SetRoomReposition();
    }
    
    //** 방 위로 정렬
    private void SetRoomReposition()
    {
        if (m_listRooms == null || m_listRooms.Count <= 0)
        {
            Debug.LogError("[UIFranchiseBuilding] SetRoomReposition : m_listRooms(List<FranchiseRoomData>) is Null or Zero");
            return;
        }

        //이전 방 사이즈, 위치
        Vector2 preRoomSize = Vector2.zero;
        float preRoomHeight = 0.0f;

        for (int i = 0; i < m_listRooms.Count; i++)
        {
            UIFranchiseRoom room = m_listRooms[i];

            if (room == null)
                continue;

            RectTransform roomPosition = room.GetComponent<RectTransform>();

            roomPosition.anchoredPosition = new Vector2(0.0f, preRoomHeight + preRoomSize.y);

            preRoomHeight   = roomPosition.anchoredPosition.y;
            preRoomSize     = room.GetRoomSize();

            // 빌딩 사이즈
            BuildingWidth   = preRoomSize.x;
            BuildingHeight  = preRoomHeight + preRoomSize.y;
        }

        // 가장 끝 방 위치 구해서 지붕 올리기
        int lastRoomNum = m_listRooms.Count - 1;
        UIFranchiseRoom lastBuilding = lastRoomNum > 0 ? m_listRooms[m_listRooms.Count - 1] : null;

        if (lastBuilding != null)
        {
            RectTransform lastRoomPosition = lastBuilding.GetComponent<RectTransform>();
            float roofYPosition = lastRoomPosition.anchoredPosition.y + m_rtrsGatePosition.sizeDelta.y + lastBuilding.GetRoomSize().y;
            m_rtrsRoofPosition.anchoredPosition = new Vector2(0.0f, roofYPosition);
        }

        BuildingHeight = m_rtrsRoofPosition.anchoredPosition.y + m_rtrsRoofImage.sizeDelta.y;
    }

    //** 줌 인 아웃에 따른 UI 엑티브 변경
    public void SetZoomInOutUI(bool zoom)
    {
        for (int i = 0; i < m_listRooms.Count; i++)
        {
            UIFranchiseRoom room = m_listRooms[i];

            room.SetZoomInOutUI(zoom);
        }
    }

    //** index에 따른 Room 반환
    public UIFranchiseRoom GetRoom(int floorNum)
    {
        if (m_listRooms == null)
            return null;

        return m_listRooms.Find(item => item.m_nFloor == floorNum);
    }

    //** Sequence에 따른 Room 반환
    public UIFranchiseRoom GetRoom(long sequence)
    {
        if (m_listRooms == null)
            return null;

        return m_listRooms.Find(item => item.m_lSequence == sequence);
    }
    
    // 보상을 받고 오픈 가능한 방이 있는지를 위한 데이터 갱신
    public void CheckOpenableRooms()
    {
        for (int i = 0; i < m_listRooms.Count; i++)
        {
            UIFranchiseRoom room = m_listRooms[i];

            FranchiseRoomData roomData = Kernel.entry.franchise.FindRoomData(m_nBuildingNumber, room.m_nFloor);

            if (roomData == null)
                continue;

            room.SetRoomData(this, roomData);
        }
    }
}
