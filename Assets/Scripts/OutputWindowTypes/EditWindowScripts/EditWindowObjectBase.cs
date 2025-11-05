using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[Serializable]
public abstract class EditWindowObjectBase : MonoBehaviour
{
    public string path = "";
    public string removeCommand = "";
    private string _objectName = "";
    public bool doDelete = false;
    public bool isParentObject = false;//is this object the base parent of the tree we're looking up
    public bool newlyCreated = true;//if this object was just created in an edit window, then do a value update when we get one. if not, don't update from server if there are changes
    public EditWindowObjectBase parentObject;
    //public TextMeshProUGUI label;//this is the label next to whatever we're instantiating
    public EditObjectType objectType;
    public ListDragObject dragObject;
    public EditWindowController EditController { get; set; }
    public List<EditWindowObjectBase> editChildObjects = new List<EditWindowObjectBase>();

    public abstract bool NeedToSendUpdate(out string updatedValue);
    public abstract void SendRemoveCommand();
    public abstract void Initialize();

    public string ObjectName
    {
        get
        {
            return _objectName;
        }
        set
        {
            _objectName = value;
            gameObject.name = value;
            Label.SetText(value);
        }
    }

    public virtual TextMeshProUGUI Label { get; set; }

    public virtual void SetValue(string newValue) { }
    public virtual void SetOldValue(string newValue) { }

    public virtual string GetPathFromParent()
    {
        string parentPath = "";

        if (!isParentObject && parentObject != null)
        {
            parentPath = parentObject.GetPathFromParent();
         //   if (parentObject.objectType == EditObjectType.List)
         //   {
                //get the child index of this object under the list as the name
         //       parentPath += $"{transform.GetSiblingIndex()}:";
         //   }
         //   else
         //   {
                parentPath += $"{ObjectName}:";
         //   }
        }
        else if (isParentObject)
        {
            parentPath = $"{ObjectName}:";
        }
        else
        {
            //parentObject was somehow null and this isn't a base parent?

        }
        return parentPath;
    }

    public virtual void SetObjectNameFromList()
    {
        List<EditWindowObjectBase> childObjects = new List<EditWindowObjectBase>();

        if (!isParentObject && parentObject != null && parentObject.objectType == EditObjectType.List)
        {
            //objectName = transform.GetSiblingIndex().ToString();
            EditWindowList editList = (EditWindowList)parentObject;
            for (int i = 0; i < editList.foldout.content.childCount; i++)
            {
                EditWindowObjectBase tempChild = editList.foldout.content.GetChild(i).GetComponent<EditWindowObjectBase>();
                if (tempChild != null)
                {
                    childObjects.Add(tempChild);
                }
            }

            for (int i = 0; i < childObjects.Count; i++)
            {
                if (childObjects[i] == this)
                {
                    ObjectName = i.ToString();
                }
            }
        }
    }

    public virtual void UpdateContentObjects()//bool doClear)
    { }

    public virtual void ActivateDragObject(bool isActive)
    {
        if (dragObject != null)
        {
            dragObject.gameObject.SetActive(isActive);
        }
    }

    public virtual void SetPathOnChildObjects()
    {
        path = GetPathFromParent();
        for (int i = 0; i < editChildObjects.Count; i++)
        {
            editChildObjects[i].SetPathOnChildObjects();

        }

    }

}

public enum EditObjectType
{
    Text,
    Dropdown,
    Foldout,
    NewButton,
    List
}

