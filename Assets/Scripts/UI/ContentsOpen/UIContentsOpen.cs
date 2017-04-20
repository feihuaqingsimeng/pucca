using UnityEngine;
using System.Collections;
using Common.Packet;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIContentsOpen : UIObject
{
    [HideInInspector]
    public Transform TargetParent;


    public void LoadContentsPrefab(string ContentsName)
    {
        if (ContentsName == string.Empty)
            return;

        if (TargetParent == null)
        {
            TargetParent = transform.FindChild("ParentObject");

            GameObject ContentsPrefab = Instantiate(Resources.Load("Prefabs/UI/ContentsOpen/" + ContentsName), TargetParent) as GameObject;
            RectTransform pRectTransform = ContentsPrefab.GetComponent<RectTransform>();
            pRectTransform.localPosition = Vector3.zero;
            pRectTransform.offsetMin = new Vector2(0, 0);
            pRectTransform.offsetMax = new Vector2(0, 0);
            pRectTransform.localScale = Vector3.one;

            int ContentsStringIdx = 0;
            switch (ContentsName)
            {
                case "Turorial_OpenContents_0":
                    ContentsStringIdx = 10;
                    break;
            }

            DBStr_TutorialExplain.Schema ContentsData = DBStr_TutorialExplain.Query(DBStr_TutorialExplain.Field.Index, ContentsStringIdx);
            ContentsPrefab.GetComponent<UITutorialExplain>().SetPopupText(ContentsData.ContentsName, ContentsData.ContentsExplain);
        }
    }


}
