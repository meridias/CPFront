using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using onnaMUD.BaseClasses;
using TMPro;
using System;
using static onnaMUD.MainController;
using UnityEngine.TextCore.Text;

public class EditRoom : MonoBehaviour
{
    public TMP_InputField roomID;
    public TMP_InputField roomName;
    public TMP_InputField roomDesc;
    public TMP_InputField areaID;
    public TMP_Dropdown roomTypeDD;


    // Start is called before the first frame update
    void Start()
    {
        roomTypeDD.ClearOptions();

        List<string> raceDDOptions = new List<string>();
        for (int i = 0; i < Enum.GetNames(typeof(RoomType)).Length; i++)
        {
            raceDDOptions.Add(((RoomType)i).ToString());

        }
        roomTypeDD.AddOptions(raceDDOptions);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FillForm(Room roomToEdit)
    {
        roomID.text = roomToEdit.Id.ToString();
        roomName.text = roomToEdit.Name;
        roomDesc.text = roomToEdit.Description;
        areaID.text = roomToEdit.Area.ToString();
        roomTypeDD.value = (int)roomToEdit.RoomType;
    }

    public Room SaveRoom()
    {
        Room roomEdit = new Room();
        roomEdit.Id = new Guid(roomID.text);
        roomEdit.Name = roomName.text;
        roomEdit.Description = roomDesc.text;
        roomEdit.Area = new Guid(areaID.text);
        roomEdit.RoomType = (RoomType)roomTypeDD.value;

        return roomEdit;
    }


}
