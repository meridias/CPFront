using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListDragObject : Graphic, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler// MonoBehaviour, IPointerDownHandler
{//objectType.NewButton doesn't have this
    public EditWindowObjectBase mainObject;
    public EditWindowList mainList;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(mainObject.objectName);
        mainList.draggedIndex = mainList.childObjects.IndexOf(mainObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("now dragging");
        mainList.StartDraggingElement();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("now dragging");

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("stopped dragging");
        mainList.StopDraggingElement();
    }

    public void SetRaycast(bool isTarget)
    {
        GetComponent<Graphic>().raycastTarget = isTarget;
    }
}

