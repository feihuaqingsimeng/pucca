using UnityEngine;
using System.Collections;

public class UIFranchiseZoomInOut : MonoBehaviour 
{
    private const float F_ZOOM_SPEAD    = 0.5f;     // 줌, 아웃 될때 속도

    private float   m_fZoomMuti;                    // 줌, 아웃 됬을때의 배수
    public  float   ZoomMuti
    {
        get { return m_fZoomMuti; }
        set { m_fZoomMuti = value; }
    }

    private RectTransform   m_rtrsBackGroundImage;
    private Transform       m_trsBackGroundImage;
    private Vector2         m_vecStartMovePosition; // 아웃으로 움직이기 직전의 위치.

    public void SetTransForm(GameObject go)
    {
        m_rtrsBackGroundImage   = go.GetComponent<RectTransform>();
        m_trsBackGroundImage    = go.GetComponent<Transform>();
    }

    //** 줌, 아웃 배수값 저장
    public void SaveZoomMutiple(int screenSizeX, int screenSizeY, float backGroundSizeX, float backGroundSizeY)
    {
        float mutiX = backGroundSizeX / screenSizeX;
        float mutiY = backGroundSizeY / screenSizeY;

        m_fZoomMuti = mutiX > mutiY ? mutiX : mutiY;
        m_fZoomMuti = 1 / m_fZoomMuti;
    }

    //** 줌 / 아웃 사이즈 조절
    private IEnumerator ZoomInOut(bool zoom)
    {
        Vector3 currentSize = m_trsBackGroundImage.localScale;

        float m_fElapsedTime = 0.0f;
        float fValue = 0.0f;
        float fEndSize = zoom ? 1.0f : m_fZoomMuti;
        float fnextXSize = 0.0f;
        float fnextYSize = 0.0f;

        while (zoom ? currentSize.x < 1.0f || currentSize.y < 1.0f : currentSize.x > m_fZoomMuti || currentSize.y > m_fZoomMuti)
        {
            m_fElapsedTime += Time.deltaTime;

            if (m_fElapsedTime > F_ZOOM_SPEAD)
                m_fElapsedTime = F_ZOOM_SPEAD;

            fValue = m_fElapsedTime / F_ZOOM_SPEAD;

            fnextXSize = EaseInQuart(currentSize.x, fEndSize, fValue);
            fnextYSize = EaseInQuart(currentSize.y, fEndSize, fValue);

            m_trsBackGroundImage.localScale = new Vector3(fnextXSize, fnextYSize, 1.0f);
            currentSize = m_trsBackGroundImage.localScale;

            yield return null;
        }

        m_trsBackGroundImage.localScale = new Vector3(zoom ? 1.0f : m_fZoomMuti, zoom ? 1.0f : m_fZoomMuti, 1.0f);

    }

    // 줌 / 아웃 이동
    private IEnumerator MoveCenter(bool zoom)
    {
        Vector2 currentPosition = m_rtrsBackGroundImage.anchoredPosition;
        Vector2 endPosition = zoom ? m_vecStartMovePosition : Vector2.zero;

        float m_fElapsedTime = 0.0f;
        float fValue = 0.0f;
        float fnextXPos = 0.0f;
        float fnextYPos = 0.0f;

        while (zoom
            ? currentPosition.x != m_vecStartMovePosition.x || currentPosition.y != m_vecStartMovePosition.y
            : currentPosition.x != 0.0f || currentPosition.y != 0.0f)
        {

            m_fElapsedTime += Time.deltaTime;

            if (m_fElapsedTime > F_ZOOM_SPEAD)
                m_fElapsedTime = F_ZOOM_SPEAD;

            fValue = m_fElapsedTime / F_ZOOM_SPEAD;

            fnextXPos = EaseInQuart(currentPosition.x, endPosition.x, fValue);
            fnextYPos = EaseInQuart(currentPosition.y, endPosition.y, fValue);

            m_rtrsBackGroundImage.anchoredPosition = new Vector2(fnextXPos, fnextYPos);
            currentPosition = m_rtrsBackGroundImage.anchoredPosition;

            yield return null;
        }

        m_rtrsBackGroundImage.anchoredPosition = new Vector2(zoom ? m_vecStartMovePosition.x : 0.0f, zoom ? m_vecStartMovePosition.y : 0.0f);
    }


    //** 화면 줌/ 아웃 버튼
    public bool OnClickZoomInOut()
    {
        bool zoom = m_trsBackGroundImage.localScale.x <= m_fZoomMuti;

        if (!zoom)
            m_vecStartMovePosition = m_rtrsBackGroundImage.anchoredPosition;

        StartCoroutine(ZoomInOut(zoom));
        StartCoroutine(MoveCenter(zoom));

        return zoom;
    }

    private float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }
}
