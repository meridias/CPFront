using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using onnaMUD;
using onnaMUD.BaseClasses;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Sockets;
using System;
using TMPro;

public class AdminController : MonoBehaviour
{
    public static AdminController instance;

    //public List<GameObject> editFormObjects = new List<GameObject>();

    public GameObject editWindow;
    public TextMeshProUGUI editWindowTitle;

    private IEnumerator editWindowCO;
    private string currentlyEditing;

    public GameObject editWindowContent;
    private GameObject editingRoom;
    private GameObject editingChar;
    private GameObject editingObject;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for (int i = 0; i < editWindowContent.transform.childCount; i++)
        {
            switch (editWindowContent.transform.GetChild(i).name)
            {
                case string a when a.ToLower().Contains("character"):
                    editingChar = editWindowContent.transform.GetChild(i).gameObject;
                    break;
                case string a when a.ToLower().Contains("room"):
                    editingRoom = editWindowContent.transform.GetChild(i).gameObject;
                    break;
                case string a when a.ToLower().Contains("object"):
                    editingObject = editWindowContent.transform.GetChild(i).gameObject;
                    break;

            }


        }

        editWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool ProcessMessage(TcpClient client, string fullMessage)
    {
        //index 0 is message code 
        //index 1 is message

        //we return true or false if we did admin code processing or not

        //Debug.Log(message);
        //string code = "0";
        string[] delimiter = { "::" };
        string[] splitMessage = fullMessage.Split(delimiter, StringSplitOptions.None);

        //keep code and message as basic messages, we'll do specific variables as needed in the different code blocks
        string code = splitMessage[0];
        string message = splitMessage[1];
        //string thingToEdit = "";
        //string editJson = "";

 /*       for (int i = 0; i < splitMessage.Length; i++)
        {
            switch (i)
            {
                case 0:
                    code = splitMessage[0];
                    break;
                case 1:
                    message = splitMessage[1];
                    break;
                case 2:
                    thingToEdit = splitMessage[2];
                    break;
                case 3:
                    editJson = splitMessage[3];
                    break;
            }
        }*/

        //Debug.Log(code);
        switch (code)
        {
            case "052":
                //Debug.Log("getting stuff...");
                string thingToEdit = "";
                string editJson = "";
                for (int i = 0; i < splitMessage.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            code = splitMessage[0];
                            break;
                        case 1:
                            message = splitMessage[1];
                            break;
                        case 2:
                            thingToEdit = splitMessage[2];
                            break;
                        case 3:
                            editJson = splitMessage[3];
                            break;
                    }
                }

                //if (splitMessage.Length < 3)
                // {
                //didn't get a splitMessage[2] so we don't know what we're editing (room, character, etc)
                //     return false;
                // }
                //turn off all the sub-objects for forms: room, character, object, etc
                //  for (int i = 0; i < editFormObjects.Count; i++)
                //  {
                //      editFormObjects[i].SetActive(false);
                //  }
                //Debug.Log(message);
                switch (message.ToLower())
                {
                 //   case "edit-char":
                        //turn on sub-object for character
                   //     editWindowTitle.text = "Editing character";
                     //   editFormObjects[1].SetActive(true);
                       // goto case "edit";
                    case "edit":
                        switch (thingToEdit.ToLower())
                        {
                            case "character":
                            case "room":
                                editWindowCO = EditWindowCO(thingToEdit, editJson);
                                StartCoroutine(editWindowCO);

                                //break;
                                return true;

       /*                     case "char":
                                editWindowTitle.text = "Editing character";
                                editWindow.SetActive(true);
                                editFormObjects[1].gameObject.SetActive(true);
                                //EditCharacter editCharacter = editFormObjects[1].GetComponent<EditCharacter>();
                                return true;
                            case "room":
                                editWindowTitle.text = "Editing room";
                                editWindow.SetActive(true);
                                editFormObjects[0].SetActive(true);
                                //EditRoom editRoom = editFormObjects[0].GetComponent<EditRoom>();
                                return true;*/
                                //break;
                            case "close":
                                editWindow.SetActive(false);
                                return true;
                            default:
                                break;
                        }
                        
                        return false;
                }
                return false;

            case "120":
                //getting edit info to put into edit window
                //'message' is what we're editing: room, account, region, etc.
                //splitMessage[2] is json
                if (splitMessage.Length < 3)
                {
                    return false;
                }
                switch (message.ToLower())
                {
                    case "room":
                        //Debug.Log(splitMessage[2]);
                        EditRoom editRoom = editingRoom.GetComponent<EditRoom>();
                        Room room = JsonConvert.DeserializeObject<Room>(splitMessage[2]);
                        return true;
                    case "character":
                        EditCharacter editCharacter = editingChar.GetComponent<EditCharacter>();
                        Character character = JsonConvert.DeserializeObject<Character>(splitMessage[2]);
                        editCharacter.FillForm(character);
                        return true;
                        //break;

                }

                //dynamic editClass = JObject.Parse(message);
                //var output = JsonConvert.SerializeObject(editClass, Formatting.None);
                //Debug.Log(output);

                //MainController.instance.ShowOutput(message);//for now
                //return true;
                break;

            case "121"://nah, not gonna use this
                //message is what we're editing: room, player, region, etc...
                //splitmessage 2 is the class string
                //Debug.Log(message);
                switch (message.ToLower())
                {
                    case "room":

  //                      MainController.instance.ShowOutput($"\"{splitMessage[2]}\"");
                        return true;


                }

                return true;

        }
        //if we haven't returned, then send fullMessage back to base ProcessMessage
        //MainController.instance.ProcessMessage(client, fullMessage);
        return false;
    }

    public IEnumerator EditWindowCO(string thingToEdit, string jsonString)
    {
        if (jsonString.Length == 0)
        {
            //there is nothing in the json string
            yield return null;
        }

        //turn off all the sub-objects for forms: room, character, object, etc
        for (int i = 0; i < editWindowContent.transform.childCount; i++)
        {
            editWindowContent.transform.GetChild(i).gameObject.SetActive(false);
        }
        editWindowTitle.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        currentlyEditing = thingToEdit.ToLower();

        switch (thingToEdit.ToLower())
        {
            case "room":
                editWindow.SetActive(true);
                editWindowTitle.text = "Editing room";
                editingRoom.SetActive(true);
                //editFormObjects[0].SetActive(true);
                yield return new WaitForEndOfFrame();
                editWindowTitle.gameObject.SetActive(true);
                EditRoom editRoom = editingRoom.GetComponent<EditRoom>();
                Room room = JsonConvert.DeserializeObject<Room>(jsonString);// splitMessage[2]);
                editRoom.FillForm(room);
                break;
            case "character":
                Character character = JsonConvert.DeserializeObject<Character>(jsonString);// splitMessage[2]);
                editWindow.SetActive(true);
                editWindowTitle.text = $"Editing {character.Name}";// character";
                editingChar.SetActive(true);
                //editFormObjects[1].gameObject.SetActive(true);
                yield return new WaitForEndOfFrame();
                editWindowTitle.gameObject.SetActive(true);
                EditCharacter editCharacter = editingChar.GetComponent<EditCharacter>();
                //Character character = JsonConvert.DeserializeObject<Character>(jsonString);// splitMessage[2]);
                editCharacter.FillForm(character);
                
                break;




        }


        yield return null;
    }

    public void SaveEdit()
    {
        switch (currentlyEditing)
        {
            case "room":
                break;

            case "character":
                EditCharacter editCharacter = editingChar.GetComponent<EditCharacter>();
                //editCharacter.SaveCharacter();
                ConnectionController.instance.SendData("100", $"edit update character {JsonConvert.SerializeObject(editCharacter.SaveCharacter())}");
                break;




        }


    }
}
