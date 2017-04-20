using UnityEngine;
using System.Collections;
using Common.Packet;
using UnityEngine.UI;
using System.Collections.Generic;

public class UITutorial : UIObject
{
    public  GameObject      TutorialObject;

    public  GameObject      ScreenShadow;
    public  GameObject      ScreenBlock;

    public  GameObject      TalkObject;
    public  Text            Talk_Message;
    public  GameObject      Talk_EnterObj;

    public  GameObject      HighlightObject;
    public  Transform       Highlight_Parent;
    public  Transform       Highlight_ScaleParent;
    private Transform       Highlight_OldParent;
    private GameObject      Highlight_TargetObj;
    public  GameObject      Highlight_Effect;
    private Vector3         Highlight_TargetPos;
    private Vector3         Highlight_BaseLocalPos;
    private Transform       Highlight_PositionTaret;

    public  GameObject      HighlightFinger_T;
    public  GameObject      HighlightFinger_B;
    public  GameObject      HighlightFinger_L;
    public  GameObject      HighlightFinger_R;


    public  GameObject      ExplainObject;
    public  Transform       Explain_Parent;
    private GameObject      LoadExplainObject;

    public  GameObject      OpenContentsObject;
    public  Transform       OpenContents_Parent;
    private GameObject      LoadOpenContentsObject;




    public  GameObject      SkipObject;
    public  Button          SkipButton_One;
    public  Button          SkipButton_All;


    private ChangeDrawOrder ChangeDrawOrderMng;


    private bool            ForceWaitMode;
    private float           CurWaitTime;
    private float           MaxWaitTime;



    //터치방지.
    public  bool            ScreenBlockMode;



    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry.tutorial.GroupNumber == 0)
            TutorialObject.SetActive(false);
        else
        {
            if (Kernel.entry.tutorial.TutorialActive == false)
            {
                DB_TutorialGroup.Schema GroupData = DB_TutorialGroup.Query(DB_TutorialGroup.Field.Index, Kernel.entry.account.TutorialGroup);

                if (Kernel.entry.account.level >= GroupData.ActiveLevel)
                {
                    ActiveTutorial(Kernel.entry.tutorial.GroupNumber);
                    TutorialObject.SetActive(true);
                }
                else
                    TutorialObject.SetActive(false);
            }
        }

        SkipButton_One.gameObject.SetActive(false);
//        SkipButton_One.onClick.AddListener(TutorialSkip_One);
        SkipButton_All.onClick.AddListener(TutorialSkip_All);

        Kernel.entry.tutorial.onStartTutorial += ActiveTutorial;
        Kernel.entry.tutorial.onTutorialComplete += RecvTutorialEnd;
    }


    protected override void OnDisable()
    {
        base.OnDisable();

//        SkipButton_One.onClick.RemoveListener(TutorialSkip_One);
        SkipButton_All.onClick.RemoveListener(TutorialSkip_All);

        Kernel.entry.tutorial.onStartTutorial -= ActiveTutorial;
        Kernel.entry.tutorial.onTutorialComplete -= RecvTutorialEnd;
   }



    protected override void Update()
    {
        base.Update();

        ScreenBlock.SetActive(ScreenBlockMode);

        if (Highlight_PositionTaret != null && Highlight_TargetObj != null)
        {
            Highlight_TargetObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(Highlight_TargetPos.x, Highlight_PositionTaret.transform.position.y * 100.0f, Highlight_TargetPos.z);
            Highlight_Effect.transform.position = Highlight_TargetObj.transform.position;
        }


        if (ForceWaitMode)
        {
            CurWaitTime += Time.deltaTime;
            if (CurWaitTime >= MaxWaitTime)
            {
                CurWaitTime = 0.0f;
                ForceWaitMode = false;
                SetNextTutorial();
            }
        }

        if (Kernel.gameServerType == GAME_SERVER_TYPE.TYPE_RELEASE)
            SkipObject.SetActive(false);
        else
        {
            if (Kernel.sceneManager.activeSceneObject.scene == Scene.Lobby && Kernel.entry.tutorial.CurTutorialType == TUTORIAL_TYPE.TUTORIAL_TALK)
                SkipObject.SetActive(true);
            else
                SkipObject.SetActive(false);
        }

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    TouchTutorial();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                TouchTutorial();
            }
        }
    }


    private void TouchTutorial()
    {
        switch (Kernel.entry.tutorial.CurTutorialType)
        {
            case TUTORIAL_TYPE.TUTORIAL_TALK:
            case TUTORIAL_TYPE.TUTORIAL_EXPLAIN:
                SetNextTutorial();

                if (Kernel.entry.tutorial.onTutorialBattleDelegate != null)
                    Kernel.entry.tutorial.onTutorialBattleDelegate();
                break;

            case TUTORIAL_TYPE.TUTORIAL_OPEN_CONTENTS:
                ChangeNextScene();
                break;
        }
    }













    public void ActiveTutorial(int GroupNum)
    {
        if (GroupNum < 10)
        {
            TutorialObject.SetActive(false);
            return;
        }
        TutorialObject.SetActive(true);

        Kernel.entry.tutorial.TutorialActive = true;
        Kernel.entry.tutorial.GroupNumber = GroupNum;
        Kernel.entry.tutorial.CurIndex = 1;

        if(Kernel.entry.tutorial.onUpdateTutorialUI != null)
            Kernel.entry.tutorial.onUpdateTutorialUI();
        SetTutorialData(Kernel.entry.tutorial.CurIndex);

        Kernel.entry.tutorial.onSetNextTutorial += SetNextTutorial;
        Kernel.entry.tutorial.onSetNextTutorial_Delay += SetNextTutorial_Delay;
    }

    public void EndTutorial()
    {
        Kernel.entry.tutorial.TutorialActive = false;
        Kernel.entry.tutorial.onSetNextTutorial -= SetNextTutorial;
        Kernel.entry.tutorial.onSetNextTutorial_Delay -= SetNextTutorial_Delay;
    }


    public void SetTutorialState(TUTORIAL_TYPE type, int seq)
    {
        Kernel.entry.tutorial.CurTutorialType = type;
        Kernel.entry.tutorial.WaitSeq = seq;
    }


    public void SetNextTutorial()
    {
        if(!Kernel.entry.tutorial.TutorialActive)
            return;

        if (Kernel.entry.account.TutorialGroup > 30)
            ScreenBlock.SetActive(true);
        else
            ScreenBlock.SetActive(false);

        if (LoadExplainObject != null)
            Destroy(LoadExplainObject);

        if (LoadOpenContentsObject != null)
            Destroy(LoadOpenContentsObject);

        DB_Tutorial.Schema TutorialData = DB_Tutorial.Query(DB_Tutorial.Field.Index, Kernel.entry.tutorial.CurIndex, DB_Tutorial.Field.GroupIndex, Kernel.entry.tutorial.GroupNumber);

        Kernel.entry.tutorial.CurIndex = TutorialData.NextIndex;
        SetTutorialData(Kernel.entry.tutorial.CurIndex);
    }


    public void SetNextTutorial_Delay()
    {
        Invoke("SetNextTutorial", 0.1f);
    }



    private void SetTutorialData(int index)
    {
        DB_Tutorial.Schema TutorialData = DB_Tutorial.Query(DB_Tutorial.Field.Index, index, DB_Tutorial.Field.GroupIndex, Kernel.entry.tutorial.GroupNumber);
        DBStr_TutorialExplain.Schema ExplainData = null;

        SetTutorialState(TutorialData.TUTORIAL_TYPE, TutorialData.WaitSeq);

        ScreenBlockMode = true;

        switch (TutorialData.TUTORIAL_TYPE)
        {
            case TUTORIAL_TYPE.TUTORIAL_TALK:
                ResetTutorialObject(true, false, false, false);
                DBStr_Tutorial.Schema stringData = DBStr_Tutorial.Query(DBStr_Tutorial.Field.Index, TutorialData.StringIndex);
                if (stringData != null)
                    Talk_Message.text = stringData.StringData;
                break;

            case TUTORIAL_TYPE.TUTORIAL_HIGHLIGHT:
                ScreenBlockMode = false;

                if(Kernel.entry.tutorial.onUpdateTutorialUI != null)
                    Kernel.entry.tutorial.onUpdateTutorialUI();

                ResetTutorialObject(false, true, false, false);

                GameObject TargetObj = GameObject.Find(TutorialData.TargetName) as GameObject;
                if (TargetObj != null)
                {
                    Highlight_TargetObj = TargetObj.transform.parent.gameObject;
                    Highlight_OldParent = Highlight_TargetObj.transform.parent;

                    Highlight_TargetPos = Highlight_TargetObj.transform.position;
                    Highlight_BaseLocalPos = Highlight_TargetObj.GetComponent<RectTransform>().localPosition;

                    Highlight_PositionTaret = null;
                    bool IgnorePosTarget = false;
                    switch (TutorialData.TargetName)
                    {
                        case "TutoTarget_PVE_Stage_1":
                            IgnorePosTarget = true;
                            break;

                        case "TutoTarget_Leader_Info":
                            IgnorePosTarget = true;
                            Highlight_TargetObj.transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
                            break;

                        case "TutoTarget_Leader":
                            GameObject PosTarget = GameObject.Find("TutoTarget_LeaderPosition");
                            if(PosTarget != null)
                                Highlight_PositionTaret = PosTarget.transform.parent;

                            IgnorePosTarget = true;
                            break;
                    }

                    UIUtility.SetParent(Highlight_TargetObj.transform, Highlight_Parent);

                    if (TutorialData.TargetName == "TutoTarget_Leader_Info")
                    {
                        Highlight_TargetObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(974.0f, -354.0f);
                    }
                    else
                    {
                        if (IgnorePosTarget)
                            Highlight_TargetObj.GetComponent<RectTransform>().anchoredPosition3D = Highlight_TargetPos;
                        else
                            Highlight_TargetObj.transform.position = Highlight_TargetPos;
                    }


                    if (TutorialData.TargetName == "TutoTarget_SmilePoint")
                    {
                        Highlight_Effect.transform.position = Highlight_TargetObj.transform.position;
                        Highlight_Effect.transform.localPosition = new Vector3(Highlight_Effect.transform.localPosition.x, Highlight_Effect.transform.localPosition.y + 33.5f, Highlight_Effect.transform.localPosition.z);
                    }
                    else
                        Highlight_Effect.transform.position = Highlight_TargetObj.transform.position;

                    Highlight_TargetObj.GetComponent<Button>().onClick.AddListener(HighlightButtonOnPress);

                    //상자처리.
                    ChangeDrawOrderMng = Highlight_TargetObj.GetComponent<ChangeDrawOrder>();
                    if (ChangeDrawOrderMng != null)
                        ChangeDrawOrderMng.SetSortingOrder(710);


                    HighlightFinger_B.SetActive(false);
                    HighlightFinger_T.SetActive(false);
                    HighlightFinger_L.SetActive(false);
                    HighlightFinger_R.SetActive(false);

                    switch (TutorialData.TargetName)
                    {
                        case "TutoTarget_PVP":
                        case "TutoTarget_Skill_0":
                        case "TutoTarget_Skill_1":
                        case "TutoTarget_Skill_2":
                        case "TutoTarget_Chest_1":
                        case "TutoTarget_Adventure":
                        case "TutoTarget_ResultExit":
                            HighlightFinger_B.SetActive(true);
                            break;

                        case "TutoTarget_Deck":
                        case "TutoTarget_PVE_Start":
                        case "TutoTarget_SmilePoint":
                            HighlightFinger_L.SetActive(true);
                            break;

                        case "TutoTarget_PopupExit":
                            HighlightFinger_R.SetActive(true);
                            break;

                        default:
                            HighlightFinger_T.SetActive(true);
                            break;

                    }

                }
                break;

            case TUTORIAL_TYPE.TUTORIAL_EXPLAIN:
                ResetTutorialObject(false, false, true, false);

                ExplainData = DBStr_TutorialExplain.Query(DBStr_TutorialExplain.Field.Index, TutorialData.ExplainID);

                LoadExplainObject = Instantiate(Resources.Load("Prefabs/UI/Tutorial/" + TutorialData.TargetName), Explain_Parent) as GameObject;

                LoadExplainObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                LoadExplainObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                LoadExplainObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                LoadExplainObject.GetComponent<RectTransform>().localScale = Vector3.one;
                LoadExplainObject.GetComponent<UITutorialExplain>().SetPopupText(ExplainData.ContentsName, ExplainData.ContentsExplain);
                break;

            case TUTORIAL_TYPE.TUTORIAL_OPEN_CONTENTS:
                ResetTutorialObject(false, false, false, true);

                ExplainData = DBStr_TutorialExplain.Query(DBStr_TutorialExplain.Field.Index, TutorialData.ExplainID);

                LoadOpenContentsObject = Instantiate(Resources.Load("Prefabs/UI/ContentsOpen/" + TutorialData.TargetName), OpenContents_Parent) as GameObject;
                LoadOpenContentsObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                LoadOpenContentsObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                LoadOpenContentsObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                LoadOpenContentsObject.GetComponent<RectTransform>().localScale = Vector3.one;
                LoadOpenContentsObject.GetComponent<UITutorialExplain>().SetPopupText(ExplainData.ContentsName, ExplainData.ContentsExplain);
                break;

            case TUTORIAL_TYPE.TUTORIAL_WAIT:
                ResetTutorialObject(false, false, false, false);

                switch (Kernel.entry.tutorial.WaitSeq)
                {
                    case 104:
                    case 105:
                    case 200:
                    case 302:
                        ScreenBlockMode = false;
                        break;
                }
                break;

            case TUTORIAL_TYPE.TUTORIAL_TIMEWAIT:
                ForceWaitMode = true;
                CurWaitTime = 0.0f;
                ResetTutorialObject(false, false, false, false, true);
                break;


            case TUTORIAL_TYPE.TUTORIAL_END:
                ResetTutorialObject(false, false, false, false);

                //통신.
                EndTutorial();

                if (Kernel.entry.tutorial.GroupNumber == 50)
                {
                    Kernel.entry.chest.REQ_PACKET_CG_READ_REWARD_BOX_LIST_SYN();
                }

                if (Kernel.entry.tutorial.GroupNumber != 10 && Kernel.entry.tutorial.GroupNumber != 20)
                {
                    DB_TutorialGroup.Schema GroupData = DB_TutorialGroup.Query(DB_TutorialGroup.Field.Index, Kernel.entry.tutorial.GroupNumber);
                    Kernel.entry.tutorial.REQ_PACKET_CG_GAME_COMPLETE_TUTORIAL_SYN(GroupData.NextIndex);
                }
                else
                {
                    TutorialSkip_One();
                }

//                TutorialSkip_One();
                break;
        }
    }




    private void ResetTutorialObject(bool TalkObj, bool HighlightObj, bool ExplainObj, bool OpenContentsObj, bool TimeWait = false)
    {
        TalkObject.SetActive(TalkObj);
        HighlightObject.SetActive(HighlightObj);
        ExplainObject.SetActive(ExplainObj);
        OpenContentsObject.SetActive(OpenContentsObj);

        if (TimeWait)
        {
            ScreenShadow.SetActive(true);
        }
        else
        {
            if (TalkObj || HighlightObj || OpenContentsObj)
                ScreenShadow.SetActive(true);
            else
                ScreenShadow.SetActive(false);
        }

        if (Kernel.gameServerType == GAME_SERVER_TYPE.TYPE_RELEASE)
            SkipObject.SetActive(false);
        else
        {
            if (Kernel.entry.tutorial.CurTutorialType == TUTORIAL_TYPE.TUTORIAL_TALK)
                SkipObject.SetActive(true);
            else
                SkipObject.SetActive(false);
        }
    }








    public void HighlightButtonOnPress()
    {
        if(Highlight_TargetObj == null)
            return;

        Highlight_TargetObj.GetComponent<Button>().onClick.RemoveListener(HighlightButtonOnPress);
        if (ChangeDrawOrderMng != null)
            ChangeDrawOrderMng.ReturnSortingOrder();

        UIUtility.SetParent(Highlight_TargetObj.transform, Highlight_OldParent);
        Highlight_TargetObj.GetComponent<RectTransform>().localPosition = Highlight_BaseLocalPos;

        Highlight_TargetObj = null;
        Highlight_PositionTaret = null;
        SetNextTutorial();

        if (Kernel.entry.tutorial.onTutorialBattleDelegate != null)
            Kernel.entry.tutorial.onTutorialBattleDelegate();
    }




    public void ChangeNextScene()
    {
        SetNextTutorial();

        switch (Kernel.entry.tutorial.GroupNumber)
        {
            case 60:    //복수전 이동.
                Kernel.sceneManager.LoadScene(Scene.RevengeBattle);
                break;

            case 70:    //보물수색 이동.
                Kernel.sceneManager.LoadScene(Scene.Detect);
                break;
            
            case 80:    //보물찾기 이동.
                Kernel.sceneManager.LoadScene(Scene.Treasure);
                break;
            
            case 90:    //가맹점 이동.
                Kernel.sceneManager.LoadScene(Scene.Franchise);
                break;
            
            case 100:   //길드 이동.
                Kernel.sceneManager.LoadScene(Scene.Guild);
                break;
            
            case 110:   //비밀거래 이동.
                Kernel.sceneManager.LoadScene(Scene.SecretBusiness);
                break;
        }
    }




    public void RecvTutorialEnd()
    {
        if (Kernel.entry.account.TutorialGroup >= 80)
        {
            ScreenBlockMode = false;
            return;
        }

        TutorialSkip_One();
    }





    //스킵.
    public void TutorialSkip_One()
    {
        ScreenBlockMode = true;

        if(Highlight_TargetObj != null)
        {
            Highlight_TargetObj.GetComponent<Button>().onClick.RemoveListener(HighlightButtonOnPress);
            if (ChangeDrawOrderMng != null)
                ChangeDrawOrderMng.ReturnSortingOrder();

            UIUtility.SetParent(Highlight_TargetObj.transform, Highlight_OldParent);
            Highlight_TargetObj = null;
            Highlight_PositionTaret = null;
        }

        if(LoadExplainObject != null)
        {
            Destroy(LoadExplainObject);
        }

        EndTutorial();


        ForceWaitMode = false;
        CurWaitTime = 0.0f;

        DB_TutorialGroup.Schema GroupData = DB_TutorialGroup.Query(DB_TutorialGroup.Field.Index, Kernel.entry.tutorial.GroupNumber);

        if (GroupData.NextIndex == 0 || GroupData.NextIndex == 10000)
        {
            Kernel.entry.tutorial.TutorialActive = false;
            Kernel.entry.tutorial.GroupNumber = 0;
            Kernel.uiManager.Close(UI.Tutorial, true);
            Kernel.entry.tutorial.onResetLobbyUI();
        }
        else
        {
            ActiveTutorial(GroupData.NextIndex);
        }
    }



    public void TutorialSkip_All()
    {
        if (Highlight_TargetObj != null)
        {
            Highlight_TargetObj.GetComponent<Button>().onClick.RemoveListener(HighlightButtonOnPress);
            if (ChangeDrawOrderMng != null)
                ChangeDrawOrderMng.ReturnSortingOrder();

            UIUtility.SetParent(Highlight_TargetObj.transform, Highlight_OldParent);
            Highlight_TargetObj = null;
            Highlight_PositionTaret = null;
        }

        if (LoadExplainObject != null)
        {
            Destroy(LoadExplainObject);
        }

        EndTutorial();

        ForceWaitMode = false;
        CurWaitTime = 0.0f;

        Kernel.entry.tutorial.TutorialActive = false;
        Kernel.entry.tutorial.GroupNumber = 0;
        Kernel.uiManager.Close(UI.Tutorial, true);

        Kernel.entry.tutorial.onResetLobbyUI();

        Kernel.entry.tutorial.REQ_PACKET_CG_GAME_COMPLETE_TUTORIAL_SYN(10000);

    }





}
