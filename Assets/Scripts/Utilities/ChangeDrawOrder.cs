using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeDrawOrder : MonoBehaviour
{
    public int  CurSortingOrder;

    public  RendererUtility[]   RenderUtilityList;
    private int[]               BaseSortingOrder_RenderUtility;

    public  Canvas[]            CanvasList;
    private int[]               BaseSortingOrder_CanvasList;



    public void SetSortingOrder(int order)
    {
        BaseSortingOrder_RenderUtility = new int[RenderUtilityList.Length];
        for (int idx = 0; idx < RenderUtilityList.Length; idx++)
        {
            BaseSortingOrder_RenderUtility[idx] = RenderUtilityList[idx].sortingOrder;
            RenderUtilityList[idx].sortingOrder = order;
        }


        BaseSortingOrder_CanvasList = new int[CanvasList.Length];
        for (int idx = 0; idx < CanvasList.Length; idx++)
        {
            BaseSortingOrder_CanvasList[idx] = CanvasList[idx].sortingOrder;
            CanvasList[idx].sortingOrder = order;
        }
    }

    public void ReturnSortingOrder()
    {
        for (int idx = 0; idx < RenderUtilityList.Length; idx++)
        {
            RenderUtilityList[idx].sortingOrder = BaseSortingOrder_RenderUtility[idx];
        }

        for (int idx = 0; idx < CanvasList.Length; idx++)
        {
            CanvasList[idx].sortingOrder = BaseSortingOrder_CanvasList[idx];
        }
    }


}
