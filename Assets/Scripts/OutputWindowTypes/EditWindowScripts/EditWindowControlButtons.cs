using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EditWindowControlButtons : MonoBehaviour
{
    public EditWindowController windowController;


    public void SendUpdates()
    {
        windowController.SendUpdates();

    }

    public void SaveEditChanges()
    {
        windowController.SendSaveEdit();

    }

    public void CloseEdit()
    {
        windowController.panel.OnPanelClose();

    }

}

