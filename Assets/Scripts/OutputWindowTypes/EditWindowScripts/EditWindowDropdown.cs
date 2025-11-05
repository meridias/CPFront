using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditWindowDropdown : EditWindowObjectBase
{
    //public string Path { get; set; }
    //public string RemoveCommand { get; set; } = "";
    //public EditWindowController EditController { get; set; }
    [SerializeField]
    private TextMeshProUGUI _label;//this is the label next to whatever we're instantiating
    public string oldValue = "";

    //public TextMeshProUGUI label;
    public TMP_Dropdown dropdown;

    public override TextMeshProUGUI Label
    {
        get
        {
            return _label;
        }
    }

    public override void Initialize()
    {
        objectType = EditObjectType.Dropdown;
        dragObject.gameObject.SetActive(false);
    }

    public override void SetValue(string newValue)
    {
        if (newlyCreated)
        {
            if (newValue.ToLower() == "true")
            {
                oldValue = "1";
                dropdown.value = 1;
            }
            else if (newValue.ToLower() == "false")
            {
                oldValue = "0";
                dropdown.value = 0;
            }
            else if (int.TryParse(newValue, out int dropdownOption) && dropdownOption < dropdown.options.Count && dropdownOption >= 0)
            {
                oldValue = newValue;
                dropdown.value = dropdownOption;
            }
            newlyCreated = false;
        }
        else
        {

        }
    }

    public override void SetOldValue(string value)
    {
        oldValue = value;
    }

    public override bool NeedToSendUpdate(out string updatedValue)
    {
      //  if (inputField.text != oldText)
     //   {
     //       updatedValue = inputField.text;
     //       return true;

     //   }
     //   else
     //   {
            updatedValue = "";
            return false;
     //   }

    }

    public override void SendRemoveCommand()
    {
        EditController.SendButtonCommand(removeCommand);
    }
}

