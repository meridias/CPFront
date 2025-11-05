using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ListDragDropZone : MonoBehaviour
{
    //public GameObject dropZone;
    public ListDropZone dropZone;
    public EditWindowList mainList;

    public void ActivateZone(bool isActive)
    {
        dropZone.gameObject.SetActive(isActive);

    }

}

