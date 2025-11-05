using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ListDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ListDragDropZone zoneListObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("entered");
        zoneListObject.mainList.StartPreviewHover(zoneListObject);
        //zoneListObject.mainList.hoveredDropZone = zoneListObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exited");
        zoneListObject.mainList.StopPreviewHover(zoneListObject);
        //zoneListObject.mainList.hoveredDropZone = null;
    }
}

