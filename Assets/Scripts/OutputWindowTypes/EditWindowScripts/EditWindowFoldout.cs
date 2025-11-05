using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditWindowFoldout : EditWindowObjectBase
{
    public FoldoutController foldout;
    public GameObject remove;

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

    public override void Initialize()
    {
        objectType = EditObjectType.Foldout;
        dragObject.gameObject.SetActive(false);
    }

    public override bool NeedToSendUpdate(out string updatedValue)
    {

        updatedValue = "";
        return false;
    }

    public override void SendRemoveCommand()
    {
        EditController.SendButtonCommand(removeCommand);
    }

}

