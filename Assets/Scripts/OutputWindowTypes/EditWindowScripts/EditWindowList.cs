using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Timeline.TimelineAsset;

public class EditWindowList : EditWindowObjectBase
{
    //public List<GameObject> listObjects = new List<GameObject>();
    public FoldoutController foldout;
    public GameObject listDragZonePF;

    public EditWindowNewButton addElementButton;
    //public EditListContentChild listContent;

    public List<EditWindowObjectBase> childObjects = new List<EditWindowObjectBase>();
    public List<ListDragDropZone> dragZones = new List<ListDragDropZone>();

    //public ListDragDropZone hoveredDropZone = null;
    private ListDragDropZone _hoveredDropZone = null;

    public ListDragDropZone hoveredDropZone
    {
        get
        {
            return _hoveredDropZone;
        }
        set
        {
            //Debug.Log(value);
            _hoveredDropZone = value;
        }

    }

    private int _draggedIndex;
    public int draggedIndex
    {
        get
        {
            return _draggedIndex;
        }
        set
        {
            if (value >= 0)
            {
                _draggedIndex = value;
            }
        }
    }

    //public int droppedIndex;
    public override TextMeshProUGUI Label
    {
        get
        {
            return foldout.label;
        }
    }

    public bool isExpanded
    {
        get
        {
            return foldout.isExpanded;
        }
        set
        {
            foldout.isExpanded = value;
        }
    }

    public void ExpandContent(bool isExpanded)
    {
        foldout.ExpandContent(isExpanded);
    }

    public override bool NeedToSendUpdate(out string updatedValue)
    {

        updatedValue = "";
        return false;

    }

    public override void Initialize()
    {
        objectType = EditObjectType.List;
        dragObject.gameObject.SetActive(false);
    }

    public override void SendRemoveCommand()
    {
        EditController.SendButtonCommand(removeCommand);
    }

    public void StartDraggingElement()
    {
        for (int i = 0; i < dragZones.Count; i++)
        {
            dragZones[i].ActivateZone(true);
        }
        for (int i = 0; i < childObjects.Count; i++)
        {
            childObjects[i].dragObject.SetRaycast(false);
        }
    }

    public void StopDraggingElement()
    {
        if (hoveredDropZone != null)
        {
            int dropIndex = dragZones.IndexOf(hoveredDropZone);
            bool doReorder = false;
            //int current
            //int currentChildIndex = childObjects[draggedIndex].transform.GetSiblingIndex();
            //int droppedChildIndex = dragZones[dropIndex].transform.GetSiblingIndex();
            //Debug.Log($"dropped on: {dropIndex}, current child: {currentChildIndex}, dropped child: {droppedChildIndex}");
            //Debug.Log($"current child: {draggedIndex}, dropped to {dropIndex}");
            EditWindowObjectBase tempObject = childObjects[draggedIndex];
            if (dropIndex < draggedIndex)
            {
                childObjects.RemoveAt(draggedIndex);
                childObjects.Insert(dropIndex, tempObject);
                doReorder = true;
            }
            else if (dropIndex > draggedIndex + 1)
            {
                dropIndex--;
                childObjects.RemoveAt(draggedIndex);
                childObjects.Insert(dropIndex, tempObject);
                doReorder = true;
            }

            if (doReorder)
            {
               // Debug.Log($"edit {EditController.editType} {EditController.editId} reorder {path}{draggedIndex}:{dropIndex}");
                EditController.SendButtonCommand($"edit {EditController.editType} {EditController.editId} reorder {path}{draggedIndex}:{dropIndex}");
            }
        }

        for (int i = 0; i < dragZones.Count; i++)
        {
            dragZones[i].ActivateZone(false);
        }
        for (int i = 0; i < childObjects.Count; i++)
        {
            childObjects[i].dragObject.SetRaycast(true);
            childObjects[i].ObjectName = i.ToString();
        }

        ReorderChildObjects();
        //UpdateContentObjects();// false);
        SetPathOnChildObjects();
    }

    public void StartPreviewHover(ListDragDropZone anchor)
    {
        hoveredDropZone = anchor;
    }

    public void StopPreviewHover(ListDragDropZone anchor)
    {
        if (hoveredDropZone && hoveredDropZone == anchor)
        {
            //if there is a hoveredAnchorZone set and it's the same one as we're not mousing over anymore, clear it
            //if it's not the same one, that means another zone got set as the one we're hovering over before this one got cleared
            hoveredDropZone = null;
        }
    }

    public void ReorderChildObjects()
    {
        List<Transform> objectTransforms = new List<Transform>();

        if (childObjects.Count > 0)
        {
            for (int i = 0; i < dragZones.Count; i++)
            {
                if (i < childObjects.Count)
                {
                    objectTransforms.Add(dragZones[i].transform);
                    objectTransforms.Add(childObjects[i].transform);
                }
                else
                {
                    objectTransforms.Add(dragZones[i].transform);
                }
            }
            //if ()
        }

        for (int i = 0; i < objectTransforms.Count; i++)
        {
            objectTransforms[i].SetSiblingIndex(i);
        }
        addElementButton.transform.SetAsLastSibling();
    }

    public override void UpdateContentObjects()//bool doClear)
    {
        List<Transform> objectTransforms = new List<Transform>();
        //   listContent.enabled = false;
        //listContent.dragZones.Clear();
        //  if (doClear)
        //  {
        childObjects.Clear();
        for (int i = 0; i < foldout.content.transform.childCount; i++)
        {
            EditWindowObjectBase tempChild = foldout.content.transform.GetChild(i).GetComponent<EditWindowObjectBase>();
            //if (tempChild != null)
            //Debug.Log(tempChild.objectType);

            if (tempChild != null && tempChild.objectType != EditObjectType.NewButton && !tempChild.doDelete)
            {
                childObjects.Add(tempChild);
            }
        }
        //   }

        for (int i = 0; i < childObjects.Count; i++)
        {
            childObjects[i].ObjectName = i.ToString();
            childObjects[i].ActivateDragObject(true);// dragObject.SetActive(true);
            childObjects[i].dragObject.mainList = this;
        }

        if (dragZones.Count < childObjects.Count + 1)
        {
            int zonesToMake = childObjects.Count + 1 - dragZones.Count;

            for (int i = 0; i < zonesToMake; i++)
            {
                GameObject newGO = Instantiate(listDragZonePF, foldout.content, false);
                ListDragDropZone dropZone = newGO.GetComponent<ListDragDropZone>();
                dragZones.Add(dropZone);
            }
        }
        //Debug.Log($"name: {objectName}, drag: {dragZones.Count}, child: {childObjects.Count}");
        if (dragZones.Count > childObjects.Count + 1)
        {
            int zonesToRemove = dragZones.Count - (childObjects.Count + 1);
            //Debug.Log(zonesToRemove);
            for (int i = 0; i < zonesToRemove; i++)
            {
                int lastIndex = dragZones.Count - 1;
                Destroy(dragZones[lastIndex].gameObject);
                dragZones.RemoveAt(lastIndex);
            }
        }

        ReorderChildObjects();

        /*   if (childObjects.Count > 0)
           {
               for (int i = 0; i < dragZones.Count; i++)
               {
                   if (i < childObjects.Count)
                   {
                       objectTransforms.Add(dragZones[i].transform);
                       objectTransforms.Add(childObjects[i].transform);
                   }
                   else
                   {
                       objectTransforms.Add(dragZones[i].transform);
                   }
               }
               //if ()
           }

           for (int i = 0; i < objectTransforms.Count; i++)
           {
               objectTransforms[i].SetSiblingIndex(i);
           }*/

        for (int i = 0; i < dragZones.Count; i++)
        {
            dragZones[i].ActivateZone(false);
            dragZones[i].mainList = this;
        }

        // addElementButton.transform.SetAsLastSibling();
        //listContent.enabled = true;
    }

}

