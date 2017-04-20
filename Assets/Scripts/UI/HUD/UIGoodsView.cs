using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;



public class UIGoodsView : MonoBehaviour 
{
    private List<Goods_Type>        m_listGoodsType;
    private List<UIGoodsViewObject> m_listGoodsObject;

    public  RectTransform       m_rtrsBG;
    private float               m_fBGOriginYSize;

    public  UIGoodsViewObject   m_GoodsObject;
    public  Transform           m_Parent;
    private RectTransform       m_rtrsLastItem;

    public  Button              m_MoreViewButton;
    public  Image               m_MoreViewImage;
    public  Sprite              m_MoreViewOpenSprite;
    public  Sprite              m_MoreViewCloseSprite;

    private RectTransform       m_rtrsMoreViewButton;
    private float               m_fOriginYPosition;

    private void OnEnable()
    {
        SetGoodsView(Kernel.sceneManager.activeSceneObject.scene);
        m_MoreViewButton.onClick.AddListener(OnClickMoreView);

        SetInit();
    }

    private void OnDisable()
    {
        m_MoreViewButton.onClick.RemoveAllListeners();
    }

    private void SetInit()
    {
        if (m_rtrsMoreViewButton == null)
            m_rtrsMoreViewButton = m_MoreViewButton.GetComponent<RectTransform>();

        if (m_Parent.localScale.y > 0)
            m_Parent.localScale = new Vector3(1.0f, 0.0f, 1.0f);

        if (m_fOriginYPosition == null || m_fOriginYPosition == 0.0f)
            m_fOriginYPosition = m_rtrsMoreViewButton.anchoredPosition.y;

        if (m_rtrsMoreViewButton.anchoredPosition.y != m_fOriginYPosition)
            m_rtrsMoreViewButton.anchoredPosition = new Vector2(0.0f, m_fOriginYPosition);

        if (m_fBGOriginYSize == null || m_fBGOriginYSize == 0.0f)
            m_fBGOriginYSize = m_rtrsBG.sizeDelta.y;

        if (m_rtrsBG.sizeDelta.y != m_fBGOriginYSize)
            m_rtrsBG.sizeDelta = new Vector2(m_rtrsBG.sizeDelta.x, m_fBGOriginYSize);

        if (m_MoreViewImage.sprite != m_MoreViewOpenSprite)
            m_MoreViewImage.sprite = m_MoreViewOpenSprite;
    }

    public void SetGoodsView(Scene scene)
    {
        SetGoodstypeList(scene);
        CreateGoods();
    }

    public void OnGoodsUpdate()
    {
        if (m_listGoodsObject == null)
            return;

        for (int i = 0; i < m_listGoodsObject.Count; i++)
        {
            UIGoodsViewObject goodsObject = m_listGoodsObject[i];

            if(i == 0)
                m_GoodsObject.UpdateGoods(Kernel.entry.account.GetValue(goodsObject.m_eGoodsType));
            
            goodsObject.UpdateGoods(Kernel.entry.account.GetValue(goodsObject.m_eGoodsType));
        }
    }

    private void Update()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                if (m_Parent.localScale.y > 0)
                    OnClickMoreView();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (m_Parent.localScale.y > 0)
                OnClickMoreView();
        }
    }

    private void CreateGoods()
    {
        if (m_listGoodsType == null)
            return;

        if (m_listGoodsObject == null)
            m_listGoodsObject = new List<UIGoodsViewObject>();

        if (m_listGoodsObject.Count > 0)
            DestroyGoodsObjects();

        // Object Create
        for (int i = 0; i < m_listGoodsType.Count; i++)
        {
            Goods_Type goodsType = m_listGoodsType[i];

            //MainGoods
            if (i == 0)
            {
                m_GoodsObject.SetUI(goodsType);
                m_listGoodsObject.Add(m_GoodsObject);
                continue;
            }

            UIGoodsViewObject newGoodsObject = Instantiate<UIGoodsViewObject>(m_GoodsObject);
            UIUtility.SetParent(newGoodsObject.transform, m_Parent);

            newGoodsObject.SetUI(goodsType);
            m_listGoodsObject.Add(newGoodsObject);
        }

        if (m_listGoodsObject.Count > 0)
        {
            UIGoodsViewObject lastObject = m_listGoodsObject[m_listGoodsObject.Count - 1];
            m_rtrsLastItem = lastObject.GetComponent<RectTransform>();
        }
    }

    private void DestroyGoodsObjects()
    {
        if (m_listGoodsObject == null)
            return;

        for (int i = 0; i < m_listGoodsObject.Count; i++)
        {
            if (i == 0)
                continue;

            UIGoodsViewObject goodsObject = m_listGoodsObject[i];
            Destroy(goodsObject.gameObject);
        }

        m_listGoodsObject.Clear();
    }

    private void SetGoodstypeList(Scene scene)
    {
        if (m_listGoodsType == null)
            m_listGoodsType = new List<Goods_Type>();

        if (m_listGoodsType.Count > 0)
            m_listGoodsType.Clear();

        switch (scene)
        {
            case Scene.Franchise: m_listGoodsType = GetFranChiseGoodsList(); break;
        }
    }

    private void OnClickMoreView()
    {
        StartCoroutine(SetSize(m_Parent.localScale.y > 0));
    }

    private IEnumerator SetSize(bool zoom)
    {
        float time = 1.0f;

        float goodsStart;
        float goodsEnd = zoom ? 0 : 1;

        float buttonMaxPosition = (m_rtrsLastItem.anchoredPosition.y - m_rtrsLastItem.sizeDelta.y) + m_fOriginYPosition;

        float buttonStart;
        float buttonEnd = zoom ? m_fOriginYPosition : buttonMaxPosition - 6;

        float bgStart;
        float bgEnd = zoom ? m_fBGOriginYSize : Mathf.Abs(buttonMaxPosition) + m_rtrsLastItem.sizeDelta.y - 6; 

         
        while (zoom ? m_Parent.localScale.y > 0 : m_Parent.localScale.y < 1)
        {
            goodsStart = m_Parent.localScale.y;
            m_Parent.localScale = new Vector3(1.0f, Mathf.Lerp(goodsStart, goodsEnd, time), 1.0f);

            buttonStart = m_rtrsMoreViewButton.anchoredPosition.y;
            m_rtrsMoreViewButton.anchoredPosition = new Vector2(0.0f, Mathf.Lerp(buttonStart, buttonEnd, time));

            bgStart = m_rtrsBG.sizeDelta.y;
            m_rtrsBG.sizeDelta = new Vector2(m_rtrsBG.sizeDelta.x, Mathf.Lerp(bgStart, bgEnd, time));

            yield return null;
        }

        m_Parent.localScale = new Vector3(1.0f, goodsEnd, 1.0f);
        m_rtrsMoreViewButton.anchoredPosition = new Vector2(0.0f, buttonEnd);
        m_rtrsBG.sizeDelta = new Vector2(m_rtrsBG.sizeDelta.x, bgEnd);

        m_MoreViewImage.sprite = zoom ? m_MoreViewOpenSprite : m_MoreViewCloseSprite;

    }

    //** 가맹점
    private List<Goods_Type> GetFranChiseGoodsList()
    {
        List<Goods_Type> newGoodsList = new List<Goods_Type>();

        newGoodsList.Add(Goods_Type.SmilePoint);
        newGoodsList.Add(Goods_Type.StarPoint);
        newGoodsList.Add(Goods_Type.RevengePoint);
        newGoodsList.Add(Goods_Type.GuildPoint);

        return newGoodsList;
    }

}
