using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditWindowNewButton : EditWindowObjectBase
{
    public Button newButton;
    public string buttonCommand = "";
    public TextMeshProUGUI labelOnbutton;//the label that is ON the button
    [SerializeField]
    private TextMeshProUGUI _label;//this is the label next to whatever we're instantiating

    public override TextMeshProUGUI Label
    {
        get
        {
            return _label;
        }
    }

    public override void Initialize()
    {
        objectType = EditObjectType.NewButton;
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

    public void SendButtonCommand()
    {
        EditController.SendButtonCommand(buttonCommand);
    }

}

