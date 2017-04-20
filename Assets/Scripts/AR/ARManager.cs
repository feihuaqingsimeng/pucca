using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ARManager : MonoBehaviour
{
    public  Camera          ARCamera;
    public  Transform       MainCamera;
    public  Camera          BackgroundCamera;

    public  RewardARChest   RewardChestMng;

    public  LRodManager     LRodMng;

    public  GyroCamera          GyroCameraMng;
    public  CameraAsBackground  CameraTextureMng;
         

    public  UIDetectAR      UIDetectARManager;


    public  float   MakeRange_Min = 2.0f;
    public  float   MakeRange_Max = 4.0f;

    public  float   FindAngle_Get = 5.0f;
    public  float   FindAngle_Find = 15.0f;

    public  bool    FindMode;
    public  bool    EndFindChest;
    public  float   ChestAngle;

    public  float   CurLookTime;
    public  float   MaxLookTime;



    void Awake()
    {
    }



    public void InitDetectAR(UIDetectAR pUI_AR)
    {
        UIDetectARManager = pUI_AR;

        RewardChestMng.MakeChest(Kernel.entry.detect.DetectBoxIndex);
        RewardChestMng.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        RewardChestMng.transform.position = MainCamera.transform.position + RewardChestMng.transform.forward * Random.RandomRange(MakeRange_Min, MakeRange_Max);
        RewardChestMng.transform.position = new Vector3(RewardChestMng.transform.position.x, 0.0f, RewardChestMng.transform.position.z);
        FindMode = true;

        UIDetectARManager.FieldNameText.text = Kernel.entry.detect.DetectFieldName;
        UIDetectARManager.AimObj.SetActive(false);

        GyroCameraMng.StartGyroCamera();
        CameraTextureMng.ShowCamBackground();

        EndFindChest = false;

        UIDetectARManager.FindGauge.SetActive(true);
        UIDetectARManager.FindGauge_Inner.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        UIDetectARManager.CurGaugeValue = 0;

        LRodMng.ShowLRod();
        SetChestFindText();


        CurLookTime = 0.0f;
        MaxLookTime = 1.0f;
    }




	
	// Update is called once per frame
	void Update ()
    {
        if (UIDetectARManager == null)
            return;

        if (EndFindChest)
            return;

        if (!FindMode)
        {
            if (!UIDetectARManager.TimeCircle.gameObject.activeInHierarchy)
                UIDetectARManager.TimeCircle.gameObject.SetActive(true);

            int x = Screen.width / 2;
            int y = Screen.height / 2;

            Ray ray = ARCamera.ScreenPointToRay(new Vector3(x, y));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 500))
            {
                if (hit.collider.gameObject.name == "ChestParent")
                {
                    CurLookTime += Time.deltaTime;
                    if (CurLookTime >= MaxLookTime)
                    {
                        GyroCameraMng.PauseGyroCamera();
                        MainCamera.transform.LookAt(new Vector3(RewardChestMng.transform.position.x, RewardChestMng.transform.position.y + 2.5f, RewardChestMng.transform.position.z));
                        RewardChestMng.transform.localScale = Vector3.one;
                        CameraTextureMng.UpdateCamBackground();
                        CameraTextureMng.PauseCamBackground();
                        UIDetectARManager.AimObj.SetActive(false);
                        UIDetectARManager.ExitButton.gameObject.SetActive(false);

                        ARCamera.fieldOfView = 20.0f;

                        UIDetectARManager.MessageText.text = "";
                        EndFindChest = true;
                        UIDetectARManager.TimeCircle.gameObject.SetActive(false);

                        RewardChestMng.OpenChest();
                    }
                }
                else
                {
                    CurLookTime -= Time.deltaTime / 2;
                    if (CurLookTime <= 0.0f)
                        CurLookTime = 0.0f;
                }

            }

            UIDetectARManager.TimeCircle.fillAmount = CurLookTime * 1.0f / MaxLookTime;
        }
        else
        {
            UIDetectARManager.TimeCircle.gameObject.SetActive(false);
            CheckChestFind();
        }

        float Angle = Quaternion.Angle(MainCamera.transform.rotation, RewardChestMng.transform.rotation) - 180.0f;

        ChestAngle = Mathf.Abs(Angle);

        UIDetectARManager.AngleText_H.text = Mathf.Abs((int)MainCamera.transform.localRotation.eulerAngles.y).ToString();

        if (ChestAngle <= FindAngle_Get)
        {
            LRodMng.CurAngle = LRodMng.RodMoveAngle_Max - (ChestAngle * LRodMng.RodMoveAngle_Max / FindAngle_Find);
        }
        else if(ChestAngle <= FindAngle_Find)
        {
            LRodMng.CurAngle = LRodMng.RodMoveAngle_Max - (ChestAngle * LRodMng.RodMoveAngle_Max / FindAngle_Find);
        }
        else
        {
            LRodMng.CurAngle = 0.0f;
        }
	}









    public void CheckChestFind()
    {
        if (ChestAngle <= FindAngle_Get)
        {
            UIDetectARManager.CurGaugeValue += UIDetectARManager.GaugeAddSpeed * Time.deltaTime;
            if (UIDetectARManager.CurGaugeValue >= UIDetectARManager.MaxGaugeLength)
            {
                UIDetectARManager.CurGaugeValue = UIDetectARManager.MaxGaugeLength;
                GetChest();
            }
        }
        else if (ChestAngle <= FindAngle_Find)
        {
            UIDetectARManager.CurGaugeValue += UIDetectARManager.GaugeAddSpeed / 2 * Time.deltaTime;
            if (UIDetectARManager.CurGaugeValue >= UIDetectARManager.MaxGaugeLength)
            {
                UIDetectARManager.CurGaugeValue = UIDetectARManager.MaxGaugeLength;
                GetChest();
            }
            else
                UIDetectARManager.MessageText.text = Languages.ToString(TEXT_UI.TREASURE_DETECT_DETECTING);
        }
        else
        {
            UIDetectARManager.CurGaugeValue -= UIDetectARManager.GaugeAddSpeed * 2 * Time.deltaTime;
            if (UIDetectARManager.CurGaugeValue <= 0.0f)
                UIDetectARManager.CurGaugeValue = 0.0f;

            SetChestFindText();
        }

        if(FindMode)
            UIDetectARManager.FindGauge_Inner.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UIDetectARManager.CurGaugeValue);
    }



    void GetChest()
    {
        UIDetectARManager.FindGauge.SetActive(false);

        RewardChestMng.SpawnChest();
        LRodMng.HideLRod();
        UIDetectARManager.AimObj.SetActive(true);
        FindMode = false;

        UIDetectARManager.MessageText.text = Languages.ToString(TEXT_UI.TREASURE_FIND);
        CancelInvoke("SetChestFindText");
    }


    void SetChestFindText()
    {
        UIDetectARManager.MessageText.text = Languages.ToString(TEXT_UI.TREASURE_DETECT_START);
    }



    public void ExitDetectAR()
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.TREASURE_DETECT_EXIT), UIAlerter.Composition.Confirm_Cancel, OnResponseCallback);
    }


    public void OnResponseCallback(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
            return;

        Kernel.sceneManager.LoadScene(Scene.Detect);
        Kernel.uiManager.Open(UI.HUD);
    }

}
