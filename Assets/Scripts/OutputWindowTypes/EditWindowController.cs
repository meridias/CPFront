using Newtonsoft.Json.Linq;
using onnaMUD;
using onnaMUD.BaseClasses;
using Panels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static onnaMUD.MainController;
using static System.Net.Mime.MediaTypeNames;
using static TMPro.Examples.TMP_ExampleScript_01;

public class EditWindowController: BaseOutput
{
    public bool isNewWindow = false;

    public RectTransform editWindowBaseRect;
    private string editLayoutString = "";
    public string editType = "";//room, character, etc
    public string editId = "";//guid for whatever we're editing

    //basic editing object
    //public GameObject basicObject;

    //input field or dropdown objects for basic editing
    //public GameObject inputFieldStretchedPF;
    public GameObject inputField;
    public GameObject dropdown;
    public GameObject foldout;
    //public GameObject foldoutPF;
    public GameObject newButton;
    public GameObject editList;
    //public GameObject editListDragZonePF;
 //   public GameObject updateButtonPF;
    public GameObject controlButtons;

    public GameObject controlButtonsObject;
    //public 
    //   public Button updateButton;
    //   public Button saveButton;
    //   public Button closeButton;

    public List<EditWindowObjectBase> editWindowObjects = new List<EditWindowObjectBase>();

    //public List<EditWindowButtonCommands> editButtons = new List<EditWindowButtonCommands>();

    //list of all added GameObjects (inputfields, dropdowns) so we can find matches for incoming json IDs in order to set values
 //   public List<EditWindowValueObjects> editValueObjects = new List<EditWindowValueObjects>();
    //public List<EditWindowDropdowns> editDropdowns = new List<EditWindowDropdowns>();
  //  public List<EditWindowDropdowns> tempEditDropdowns = new List<EditWindowDropdowns>();

    public override string Input
    {
        set
        {
            //Debug.Log(value);
            FindEditObjects(value, editWindowBaseRect);//incoming values so we look up the objects to find a match

        }
    }

    public override void OnPanelClose()
    {
        ConnectionController.instance.SendData("100", $"edit {editType} {editId} done:button");
   //     for (int j = 0; j < editButtons.Count; j++)
   //     {
            //go through all the buttons added and remove listeners
   //         editButtons[j].buttonToPress.GetComponent<Button>().onClick.RemoveAllListeners();
   //     }
   /*     if (updateButton != null)
        {
            updateButton.onClick.RemoveAllListeners();
        }
        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners();
        }
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }*/

    }

    public void FindEditObjects(string input, Transform parent)
    {
        //Debug.Log(input);
        int lastIndex = input.LastIndexOf(":");
        string value = input.Substring(lastIndex + 1);
        //Debug.Log(value);
        input = input.Remove(lastIndex + 1);
        //Debug.Log(input);

        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            if (editWindowObjects[i].path == input)
            {
                editWindowObjects[i].SetValue(value);
            }
        }

  /*      for (int i = 0; i < editValueObjects.Count; i++)
        {
            if (editValueObjects[i].objectPath == input)
            {
                //Debug.Log($"match found for {input}");
                switch (editValueObjects[i].objectType)
                {
             //       case "text":
             //           editValueObjects[i].valueObject.GetComponent<TMP_InputField>().text = value;// names[1];
                        //Debug.Log($"input: {input}");
             //           break;
             //       case "bool":
                        //Debug.Log($"bool value: {value}");
             //           if (value.ToLower() == "true")
             //               editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value = 1;
             //           else
             //               editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value = 0;
                        //editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value = value;
             //           break;
             //       case "dropdown":
             //           if (int.TryParse(value, out int dropdownOption) && dropdownOption < editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().options.Count && dropdownOption >= 0)
             //           {
             //               editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value = dropdownOption;
             //           }
             //           break;
                    default:
                        Debug.Log($"{input}:{value}");
                        break;

                }

            }
        }*/

     /*   string[] names = input.Split(":", StringSplitOptions.RemoveEmptyEntries);
        //Debug.Log(input);
        List<Transform> matchedNames = new List<Transform>();
        //let's assume that names[0] is our name, in whatever recursive loop we're in 

        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject.name == names[0])
                matchedNames.Add(parent.GetChild(i));
        }

        if (matchedNames.Count == 1)//if there is 0 or more than 1, skip it cause we messed up?
        {
            if (names.Length > 2)
            {
                string tempMoo = input.Substring(input.IndexOf(":"));
                //Debug.Log(tempMoo);
                FoldoutController tempCont = matchedNames[0].GetComponent<FoldoutController>();
                FindEditObjects(tempMoo, tempCont.content);//if we have more than 2 in names, the next will be under a foldout
            }

            //now we look under this GO and try to find a match for any of the objects in editValueObjects
            Transform[] allChildren = matchedNames[0].GetComponentsInChildren<Transform>(true);
            EditWindowValueObjects foundEditObject = null;
            bool found = false;
            while (!found)
            {
                for (int i = 0; i < allChildren.Length; i++)
                {
                    for (int j = 0; j < editValueObjects.Count; j++)
                    {
                        if (allChildren[i].gameObject == editValueObjects[j].valueObject)
                        {
                            //Debug.Log($"{editValueObjects[j].objectType}+{editValueObjects[j].valueObject}");
                            foundEditObject = editValueObjects[j];
                            //Debug.Log($"{foundEditObject.objectType}+{foundEditObject.valueObject}");
                            found = true;
                            break;
                        }
                    }
                    //break;
                }
                break;
            }
            if (foundEditObject != null)//we found a match so we update the info on the object
            {
                switch (foundEditObject.objectType)
                {
                    case "text":
                        foundEditObject.valueObject.GetComponent<TMP_InputField>().text = names[1];
                        //Debug.Log($"input: {input}");
                        break;
                    case "bool":
                        break;
                    case "dropdown":
                        if (int.TryParse(names[1], out int dropdownOption) && dropdownOption < foundEditObject.valueObject.GetComponent<TMP_Dropdown>().options.Count && dropdownOption >= 0)
                        {
                            foundEditObject.valueObject.GetComponent<TMP_Dropdown>().value = dropdownOption;
                        }
                        break;

                }
            }
        }*/

    }


    /*  public void ParseJTokenString(JToken jsonString, string path, RectTransform parent)//nope
      {
          switch (jsonString.Type)
          {
              case JTokenType.Object:
                  foreach (JProperty property in ((JObject)jsonString).Properties())
                  {
                      Debug.Log($"{path}.{property.Name}:{property.Value}");
                      //now that we have the Name of the property, look for a matching gameObject.Name under
                      Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
                      //Console.WriteLine($"Path: {path}.{property.Name}, Type: {property.Value.Type}, Value: {property.Value}");
                      //TraverseJToken(property.Value, $"{path}.{property.Name}");
                 //     ParseJTokenString(property.Value, $"{path}.{property.Name}", parent);
                  }

                  break;
              case JTokenType.Array:
                  Debug.Log($"jtoken array? Array:{jsonString}");
                  break;
          }
      }*/

    public void GetLayoutString(string layoutString)
    {
        //edit window layout string parse method... public int EditLayoutParse(editWindowObjects)//send the base parent object to the first method call and go from there
        editLayoutString = layoutString;
        //first, clear the field
        //        for (int i = 0; i < editButtons.Count; i++)
        //        {
        //go through all the buttons added and add listeners
        //            editButtons[i].buttonToPress.GetComponent<Button>().onClick.RemoveAllListeners();
        //        }
        /*     if (updateButton != null)
             {
                 updateButton.onClick.RemoveAllListeners();
             }
             if (saveButton != null)
             {
                 saveButton.onClick.RemoveAllListeners();
             }
             if (closeButton != null)
             {
                 closeButton.onClick.RemoveAllListeners();
             }*/
        //now remove any and all objects from the window and start new
        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            editWindowObjects[i].doDelete = true;
        }


        //      for (int i = editWindowBaseRect.childCount - 1; i >= 0; i--)
        //      {
        //          Transform childTransform = editWindowBaseRect.transform.GetChild(i);
        //          GameObject childGameObject = childTransform.gameObject;
        //          Destroy(childGameObject);
        //      }
        //    editButtons.Clear();
        //editValueObjects.Clear();
        //     if (editDropdowns.Count > 0)
        //     {
        //         tempEditDropdowns = editDropdowns.ToList();
        //         editDropdowns.Clear();
        //     } else
        //    {
        //        editDropdowns.Clear();
        //        tempEditDropdowns.Clear();
        //    }

        //let's get the type and ID from the start of the editLayoutString
        if (editLayoutString.IndexOf(",") > -1)
        {
            int firstIndex = editLayoutString.IndexOf(",");
            string typeAndId = editLayoutString.Substring(0, firstIndex);
            string[] tag = typeAndId.Split(":");
            //Debug.Log(typeAndId);
            editType = tag[0];
            editId = tag[1];
        }
        //        Debug.Log(editLayoutString);
        EditLayoutParse(0, editWindowBaseRect, "");
        //   for (int i = 0; i < editWindowObjects.Count; i++)
        //   {
        //       Debug.Log(editWindowObjects[i].Path);
        //   }


        //Debug.Log(editWindowObjects.Count);
        //Debug.Log(editDropdowns.Count);
        //      for (int i = 0; i < editButtons.Count; i++)
        //      {
        //go through all the buttons added and add listeners
        //          editButtons[i].buttonToPress.GetComponent<Button>().onClick.AddListener(SendButtonCommand);
        //      }
        //add 'Update' button at the end
        /*       GameObject saveUpdateLG = Instantiate(basicObject, editWindowObjects, false);
               //Debug.Log(editNewButtons.Count);
               saveUpdateLG.transform.GetChild(0).gameObject.SetActive(false);
               saveUpdateLG.transform.GetChild(1).gameObject.SetActive(false);

               GameObject updateGO = Instantiate(updateButtonPF, saveUpdateLG.transform, false);
               updateButton = updateGO.GetComponentInChildren<Button>(true);
               updateButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Update");
               updateButton.onClick.AddListener(SendUpdates);
               //add 'Save' button at the end
               GameObject saveGO = Instantiate(updateButtonPF, saveUpdateLG.transform, false);
               saveButton = saveGO.GetComponentInChildren<Button>(true);
               saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Save");
               saveButton.onClick.AddListener(SendSaveEdit);

               GameObject closeLG = Instantiate(basicObject, editWindowObjects, false);
               //Debug.Log(editNewButtons.Count);
               closeLG.transform.GetChild(0).gameObject.SetActive(false);
               closeLG.transform.GetChild(1).gameObject.SetActive(false);
               //add 'Close' button at the end
               GameObject closeGO = Instantiate(updateButtonPF, closeLG.transform, false);
               closeButton = closeGO.GetComponentInChildren<Button>(true);
               closeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Close");
               closeButton.onClick.AddListener(panel.OnPanelClose);//add the listener to the panel.OnPanelClose as that will do the panel AND this close stuff*/

        if (isNewWindow)
        {
            controlButtonsObject = Instantiate(controlButtons, editWindowBaseRect, false);
            controlButtonsObject.GetComponent<EditWindowControlButtons>().windowController = this;
        }
        else
        {
            if (controlButtonsObject != null)
            {
                //controlButtonsObject.transform.SetParent(editWindowBaseRect, false);
                controlButtonsObject.transform.SetSiblingIndex(editWindowBaseRect.childCount - 1);
            }
        }

        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            if (editWindowObjects[i].objectType == EditObjectType.List && !editWindowObjects[i].doDelete)
            {
                editWindowObjects[i].UpdateContentObjects();// true);
            }

        }
        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            editWindowObjects[i].path = editWindowObjects[i].GetPathFromParent();
        }

        for (int i = editWindowObjects.Count - 1; i >= 0; i--)
        {
            //go through and remove any objects that are still marked for delete since there was no match in the layoutstring for it
            //delete object
            //remove from list
            if (editWindowObjects[i].doDelete)
            {
                //              Debug.Log($"Deleting: {editWindowObjects[i].Path}");
                Destroy(editWindowObjects[i].gameObject);
                editWindowObjects.RemoveAt(i);
            }
        }

        isNewWindow = false;
    }

    public void SendSaveEdit()
    {
        ConnectionController.instance.SendData("100", $"edit {editType} {editId} save:button");
    }

 /*   public void SendButtonCommand()
    {
        for (int i = 0; i < editButtons.Count; i++)
        {
            if (editButtons[i].buttonToPress == EventSystem.current.currentSelectedGameObject)
            {
                //deselect button so enter doesn't work on it?
                EventSystem.current.SetSelectedGameObject(null);
                //this is the button we just pressed, so send the command without game window echo
                if (ConnectionController.instance.isConnected)
                {
                    ConnectionController.instance.SendData("100", $"{editButtons[i].command}:button");
                    //Debug.Log($"{editNewButtons[i].command}");
                }
            }
        }
    }*/

    public void SendButtonCommand(string command)
    {
        EventSystem.current.SetSelectedGameObject(null);
        //this is the button we just pressed, so send the command without game window echo
        if (ConnectionController.instance.isConnected && command.Length > 0)
        {
            ConnectionController.instance.SendData("100", $"{command}:button");
            //Debug.Log(command);
            //Debug.Log($"{editNewButtons[i].command}");
        }
    }

    public void SendUpdates()
    {
        //string tag = $"{editType}:{editId},";
        string value = "";

        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            if (editWindowObjects[i].NeedToSendUpdate(out string updatedValue))
            {
                //Debug.Log("true?");
                if (editWindowObjects[i].GetType() == typeof(EditWindowText))
                {
                    EditWindowText tempText = (EditWindowText)editWindowObjects[i];
                    if (!tempText.inputField.readOnly)
                    {
                        value = updatedValue;
                        //now set the old value to the new in order to check for future changes
                        tempText.SetOldValue(value);
                        //tempText.SetValue(value);
                    }
                }
                else if (editWindowObjects[i].GetType() == typeof(EditWindowDropdown))
                {
                    EditWindowDropdown tempDrop = (EditWindowDropdown)editWindowObjects[i];
                    value = tempDrop.dropdown.value.ToString();
                    tempDrop.SetOldValue(value);
                    //tempDrop.SetValue(value);
                }

                if (value == "")
                {
                    //ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}button");
                    value = "nullvalue";
                }
                ConnectionController.instance.SendData("100", $"edit {editType} {editId} {editWindowObjects[i].path}{value}:button");
            }
        }

   /*     for (int i = 0; i < editValueObjects.Count; i++)
        {
            //Debug.Log($"{tag}{editValueObjects[i].valueObject.name}");
            switch (editValueObjects[i].objectType)
            {
         //       case "text":
         //           TMP_InputField tempIF = editValueObjects[i].valueObject.GetComponent<TMP_InputField>();
         //           if (tempIF.readOnly)
         //           {
                        //this is read only and therefore hasn't/shouldn't have been changed so we don't need to send an update back for this
         //               continue;
         //           }
         //           value = tempIF.text;
         //           break;
         //       case "dropdown":
         //       case "bool":
         //           value = editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value.ToString();
         //           break;


            }
            if (value == "")
            {
                //ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}button");
                value = "nullvalue";
            }// else
           // {
           //     ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}:button");
         //   }
            ConnectionController.instance.SendData("100", $"edit {editType} {editId} {editValueObjects[i].objectPath}{value}:button");
            //Debug.Log($"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}");
            
        }*/
        //ConnectionController.instance.SendData("100", $"edit {editType} {editId} update done:done:button");
    }

    public int EditLayoutParse(int startIndex, RectTransform parentObject, string path)//, bool keepGoing = true)
    {
        //int endIndex = 0;
        int lastCheckIndex = startIndex;
        //int minIndex = -1;
        bool parseTags = true;
        string matchedTag = "";

        string[] layoutTags = { "text", "noedit", "enum", "newbutton", "group", "bool", "grpnull", "txtnull", "multi", "list", "guid", "idnull", "}"};
        List<TextMeshProUGUI> textObjects = new List<TextMeshProUGUI>();//this is for adjusting text object width so they're all the same width

        //    if (!keepGoing)
        //    {
        //        Debug.Log("doh");
        //        return lastCheckIndex;
        //    }

        //Debug.Log(editLayoutString);
        while (parseTags)
        {
            int minIndex = -1;
            EditWindowValueObjects tempObject = new EditWindowValueObjects();
            //Debug.Log(lastCheckIndex);
            foreach (string checkTag in layoutTags)
            {
                if (lastCheckIndex < editLayoutString.Length)//then we haven't hit the end of the string } or no }
                {
                    int currentMatch = editLayoutString.IndexOf(checkTag, lastCheckIndex);
                    if (currentMatch != -1 && (minIndex == -1 || currentMatch < minIndex))
                    {
                        minIndex = currentMatch;
                        matchedTag = checkTag;
                    }
                }
            }
            //Debug.Log(minIndex);
            if (minIndex == -1)
            {
                parseTags = false;
                break;
            }
            lastCheckIndex = minIndex;
            //Debug.Log($"Blah: {editLayoutString.Substring(lastCheckIndex)}");
            //found a tag match, now we look backwards to find the name associated with the tag
            int tagStart = 0;
            if (matchedTag != "}")
            {

                int commaDelim = editLayoutString.LastIndexOf(",", minIndex);
                int bracDelim = editLayoutString.LastIndexOf("{", minIndex);
                tagStart = Math.Max(commaDelim, bracDelim);
                if (tagStart == -1)
                    tagStart = 0;
                else tagStart += 1;
            }
            else
            {
                tagStart = minIndex;
            }
            //Debug.Log(tagStart);
            //Debug.Log($"Blah: {editLayoutString.Substring(tagStart)}");
            string layoutSubstring = editLayoutString.Substring(tagStart);
            string[] layoutTag = layoutSubstring.Split(",", StringSplitOptions.RemoveEmptyEntries);
            string[] tag = layoutTag[0].Split(":", StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log(layoutSubstring);
            string objectPath = $"{path}{tag[0]}:";
            string commandForRemove = $"edit {editType} {editId} remove {path}{tag[0]}";
            EditWindowObjectBase newEditObject = null;

            void SetNewObjectFields()
            {
                if (newEditObject != null)
                {
                    newEditObject.EditController = this;
                    //newEditObject.Path = objectPath;// $"{path}{tag[0]}:";
                    //newEditObject.gameObject.name = tag[0];
                    newEditObject.removeCommand = commandForRemove;

                    if (FindParentObject(newEditObject, out EditWindowObjectBase foundParent))
                    {
                        foundParent.editChildObjects.Add(newEditObject);

                        // newEditObject.isParentObject = false;
                        //  newEditObject.parentObject = foundParent;
                        //   if (foundParent.objectType == EditObjectType.List)
                        //   {
                        //parent is a list so this object will have an index for name
                        //   newEditObject.SetObjectNameFromList();
                        //   }
                        //   else
                        //   {
                        //       newEditObject.ObjectName = tag[0];
                        //   }
                    }
                    // else
                    // {
                    //  newEditObject.isParentObject = true;
                    //   newEditObject.ObjectName = tag[0];
                    // }
                    newEditObject.ObjectName = tag[0];
                    newEditObject.path = newEditObject.GetPathFromParent();
                }
            }

            switch (matchedTag)
            {
                case "}":
                    parseTags = false;
                    lastCheckIndex += 1;
                    break;
                case "text":
                case "idnull":
                case "noedit":
                case "txtnull":
                case "multi":
                case "guid":
                    if (isNewWindow || !IsObjectCreatedAlready(objectPath, EditObjectType.Text, out newEditObject))// EditWindowObjectBase matchedText))
                    {
                        GameObject textGO = Instantiate(inputField, parentObject, false);
                        EditWindowText editText = textGO.GetComponent<EditWindowText>();
                        newEditObject = editText;
                        editText.Initialize();
                        SetNewObjectFields();
                        //editText.EditController = this;

                        //editText.Path = objectPath;// $"{path}{tag[0]}:";
                        //textGO.name = tag[0];
                        //editText.Label.SetText(tag[0]);
                        textObjects.Add(editText.Label);//for the resizing of the label so they all line up
                        editText.SetTextType(matchedTag);
                        //Debug.Log($"type: {editType}, id: {editId}");
                        //editText.RemoveCommand = commandForRemove;// $"edit {editType} {editId} remove {path}{tag[0]}";//just in case we need it
                        editWindowObjects.Add(editText);

                        //Debug.Log(objectPath == editText.GetPathFromParent());
                        //Debug.Log(editText.GetPathFromParent());
                    }
                    else
                    {
                        EditWindowText textObject = (EditWindowText)newEditObject;
                        textObjects.Add(textObject.Label);
                        //textObject.transform.SetParent(parentObject, false);
                        textObject.transform.SetSiblingIndex(parentObject.transform.childCount - 1);
                        textObject.doDelete = false;
                    }
                    lastCheckIndex += 1;
                    break;
                case "bool":
                case "enum":
                    if (isNewWindow || !IsObjectCreatedAlready(objectPath, EditObjectType.Dropdown, out EditWindowObjectBase matchedDropdown))
                    {
                        GameObject enumGO = Instantiate(dropdown, parentObject, false);
                        EditWindowDropdown editEnum = enumGO.GetComponent<EditWindowDropdown>();
                        newEditObject = editEnum;
                        editEnum.Initialize();
                        SetNewObjectFields();
                        //editEnum.EditController = this;
                        //editEnum.Path = objectPath;//$"{path}{tag[0]}:";
                        //editEnum.RemoveCommand = commandForRemove;//$"edit {editType} {editId} remove {path}{tag[0]}";//just in case we need it
                        //enumGO.name = tag[0];
                        //editEnum.label.SetText(tag[0]);

                        textObjects.Add(editEnum.Label);
                        editEnum.dropdown.ClearOptions();
                        List<string> dropdownOptions = new List<string>();
                        if (matchedTag == "enum")
                        {
                            int enumStart = layoutSubstring.IndexOf("{");
                            int enumEnd = layoutSubstring.IndexOf("}");
                            string enumString = layoutSubstring.Substring(enumStart + 1, enumEnd - enumStart - 1);
                            if (enumString.Length > 0)
                            {
                                string[] enumValues = enumString.Split(",", StringSplitOptions.RemoveEmptyEntries);
                                dropdownOptions.AddRange(enumValues);
                                //editEnum.dropdown.AddOptions(enumValues.ToList());
                                //dropdownDP.AddOptions(enumValues.ToList());
                            }
                            lastCheckIndex = editLayoutString.IndexOf("}", tagStart);
                        }
                        else if (matchedTag == "bool")
                        {
                            dropdownOptions.Add("False");
                            dropdownOptions.Add("True");
                        }
                        //    List<string> boolOptions = new List<string> { "False", "True" };
                        editEnum.dropdown.AddOptions(dropdownOptions);
                        //       tempObject.valueObject = editBool.dropdown.gameObject;
                        //  GameObject boolBase = Instantiate(basicObject, parentObject, false);
                        //  boolBase.name = tag[0];
                        //  TextMeshProUGUI boolLabel = boolBase.GetComponentInChildren<TextMeshProUGUI>();
                        //  boolLabel.SetText(tag[0]);
                        //  textObjects.Add(boolLabel);
                        //  GameObject boolGO = Instantiate(dropdownPF, boolBase.transform.GetChild(1), false);
                        //  TMP_Dropdown boolDP = boolGO.GetComponent<TMP_Dropdown>();
                        //   boolDP.ClearOptions();
                        // List<string> boolOptions = new List<string> { "False", "True"};
                        //boolOptions.Add("False");
                        //boolOptions.Add("True");
                        //   boolDP.AddOptions(boolOptions);
                        //tempObject.valueObject = boolGO;
                        //tempObject.valueObject.name = $"{path}{tag[0]}:";
                        //        tempObject.objectPath = $"{path}{tag[0]}:";
                        //        tempObject.objectType = "bool";
                        //        editValueObjects.Add(tempObject);
                        editWindowObjects.Add(editEnum);
                        //Debug.Log(objectPath == editEnum.GetPathFromParent());
                    }
                    else
                    {
                        EditWindowDropdown dropdownObject = (EditWindowDropdown)matchedDropdown;
                        textObjects.Add(dropdownObject.Label);
                        //boolObject.transform.SetParent(parentObject, false);
                        dropdownObject.transform.SetSiblingIndex(parentObject.transform.childCount - 1);
                        dropdownObject.doDelete = false;
                        if (matchedTag == "enum")
                            lastCheckIndex = editLayoutString.IndexOf("}", tagStart);
                    }
                    lastCheckIndex += 1;
                    break;
         /*       case "enum":
                    if (isNewWindow || !IsObjectCreatedAlready(objectPath, EditObjectType.Dropdown, out EditWindowObjectBase matchedEnum))
                    {
                        int enumStart = layoutSubstring.IndexOf("{");
                        int enumEnd = layoutSubstring.IndexOf("}");
                        string enumString = layoutSubstring.Substring(enumStart + 1, enumEnd - enumStart - 1);
                        //Debug.Log(enumString);
                        GameObject enumGO = Instantiate(dropdown, parentObject, false);
                        EditWindowDropdown editEnum = enumGO.GetComponent<EditWindowDropdown>();
                        editEnum.Initialize();
                        editEnum.EditController = this;
                        editEnum.Path = objectPath;//$"{path}{tag[0]}:";
                        enumGO.name = tag[0];
                        editEnum.label.SetText(tag[0]);
                        textObjects.Add(editEnum.label);
                        editEnum.dropdown.ClearOptions();

                        //GameObject dropdownBase = Instantiate(basicObject, parentObject, false);
                        //dropdownBase.name = tag[0];
                        //TextMeshProUGUI dropdownLabel = dropdownBase.GetComponentInChildren<TextMeshProUGUI>();
                        //dropdownLabel.SetText(tag[0]);
                        //textObjects.Add(dropdownLabel);
                        //GameObject dropdownGO = Instantiate(dropdownPF, dropdownBase.transform.GetChild(1), false);
                        //TMP_Dropdown dropdownDP = dropdownGO.GetComponent<TMP_Dropdown>();
                        //dropdownDP.ClearOptions();
                        //dropdownGO.name = tag[0];

                        if (enumString.Length > 0)
                        {
                            string[] enumValues = enumString.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            editEnum.dropdown.AddOptions(enumValues.ToList());
                            //dropdownDP.AddOptions(enumValues.ToList());
                        }
                        //Debug.Log(enumString);
                        lastCheckIndex = editLayoutString.IndexOf("}", tagStart);
                        //  tempObject.valueObject = editEnum.dropdown.gameObject;
                        //tempObject.valueObject = dropdownGO;
                        //tempObject.valueObject.name = $"{path}{tag[0]}:";
                        //  tempObject.objectPath = $"{path}{tag[0]}:";
                        //  tempObject.objectType = "dropdown";
                        //  editValueObjects.Add(tempObject);
                        editWindowObjects.Add(editEnum);
                    }
                    else
                    {
                        EditWindowDropdown enumObject = (EditWindowDropdown)matchedEnum;
                        textObjects.Add(enumObject.label);
                        lastCheckIndex = editLayoutString.IndexOf("}", tagStart);
                        //enumObject.transform.SetParent(parentObject, false);
                        enumObject.transform.SetSiblingIndex(parentObject.transform.childCount - 1);
                        enumObject.doDelete = false;
                    }
                    lastCheckIndex += 1;
                    break;*/
                case "newbutton":
                    //Debug.Log(layoutTag[0]);
                    //goto case "text";
                    if (isNewWindow || !IsObjectCreatedAlready(objectPath, EditObjectType.NewButton, out EditWindowObjectBase matchedButton))
                    {
                        GameObject buttonGO = Instantiate(newButton, parentObject, false);
                        EditWindowNewButton editNew = buttonGO.GetComponent<EditWindowNewButton>();
                        newEditObject = editNew;
                        editNew.Initialize();
                        SetNewObjectFields();
                        //editNew.EditController = this;
                        //editNew.Path = objectPath;//tag[0];

                        //TextMeshProUGUI buttonTagLabel = buttonGO.GetComponentInChildren<TextMeshProUGUI>();//the text next to the button
//                        if (tag[0] != "nolabel")
//                        {
                            //buttonTagLabel.SetText(tag[0]);
                            //editNew.label.SetText(tag[0]);
                            //textObjects.Add(buttonTagLabel);
                            textObjects.Add(editNew.Label);
//                        }
//                        else
//                        {
                            //buttonTagLabel.SetText("");
//                            editNew.label.SetText("");
                            //editNew.Path = path;
//                        }
                        //   GameObject buttonOB = Instantiate(newButtonPF, buttonGO.transform.GetChild(1), false);
                        tag[3] = tag[3].Replace("}", string.Empty);
                        editNew.labelOnbutton.text = tag[2];
                        string buttonNewCommand = "";
                        if (tag.Length > 3)
                        {
                            for (int i = 3; i < tag.Length; i++)//get the newbutton command and tack on the path at the end from the remainder
                            {
                                buttonNewCommand += tag[i];
                                if (i < tag.Length - 1)
                                {
                                    buttonNewCommand += ":";
                                }
                            }
                            //buttonNew.command = tag[3];
                            buttonNewCommand = buttonNewCommand.Replace("}", string.Empty);
                        }
                        //editButtons.Add(buttonNew);
                        editNew.buttonCommand = buttonNewCommand;
                        editWindowObjects.Add(editNew);
                        //Debug.Log(objectPath == editNew.GetPathFromParent());
                    }
                    else
                    {
                        EditWindowNewButton buttonObject = (EditWindowNewButton)matchedButton;
                     //   if (!buttonObject.path.EndsWith("nolabel:"))
                     //   {
                            textObjects.Add(buttonObject.Label);
                     //   }
                        //buttonObject.transform.SetParent(parentObject, false);
                        buttonObject.transform.SetSiblingIndex(parentObject.transform.childCount - 1);
                        buttonObject.doDelete = false;
                    }
                    /*      if (tag[0] == "newbutton")
                          {
                              //Debug.Log("doh doh doh");
                              GameObject buttonGO = Instantiate(basicObject, parentObject, false);
                              //buttonGO.name = tag[0];//do we need to even do this? from buttons we never worry about the name
                              TextMeshProUGUI buttonTagLabel = buttonGO.GetComponentInChildren<TextMeshProUGUI>();//the text next to the button
                              buttonTagLabel.SetText("");
                         //     textObjects.Add(buttonTagLabel);
                              GameObject buttonOB = Instantiate(newButtonPF, buttonGO.transform.GetChild(1), false);
                              //buttonOB.name = tag[0];
                              //tag[2] = tag[2].Replace("}", string.Empty);
                              buttonOB.GetComponentInChildren<TextMeshProUGUI>().text = tag[1];//the text ON the button
                              EditWindowButtonCommands buttonNew = new EditWindowButtonCommands();
                              buttonNew.buttonToPress = buttonOB;
                              if (tag.Length > 2)
                              {
                                  for (int i = 2; i < tag.Length; i++)
                                  {
                                      buttonNew.command += tag[i];
                                      if (i < tag.Length - 1)
                                      {
                                          buttonNew.command += ":";
                                      }
                                  }
                                  //buttonNew.command = tag[2];
                                  buttonNew.command = buttonNew.command.Replace("}", string.Empty);
                              }
                              editButtons.Add(buttonNew);
                          }
                          else
                          {
                              GameObject buttonGO = Instantiate(basicObject, parentObject, false);
                              //buttonGO.name = tag[0];//do we need to even do this? from buttons we never worry about the name
                              TextMeshProUGUI buttonTagLabel = buttonGO.GetComponentInChildren<TextMeshProUGUI>();//the text next to the button
                              buttonTagLabel.SetText(tag[0]);
                              textObjects.Add(buttonTagLabel);
                              GameObject buttonOB = Instantiate(newButtonPF, buttonGO.transform.GetChild(1), false);
                              //buttonOB.name = tag[0];
                              tag[3] = tag[3].Replace("}", string.Empty);
                              buttonOB.GetComponentInChildren<TextMeshProUGUI>().text = tag[2];//the text ON the button
                              EditWindowButtonCommands buttonNew = new EditWindowButtonCommands();
                              buttonNew.buttonToPress = buttonOB;
                              if (tag.Length > 3)
                              {
                                  for (int i = 3; i < tag.Length; i++)
                                  {
                                      buttonNew.command += tag[i];
                                      if (i < tag.Length - 1)
                                      {
                                          buttonNew.command += ":";
                                      }
                                  }
                                  //buttonNew.command = tag[3];
                                  buttonNew.command = buttonNew.command.Replace("}", string.Empty);
                              }
                              editButtons.Add(buttonNew);
                          }*/
                    lastCheckIndex += 1;
                    break;
                case "group":
                case "grpnull":
                    EditWindowFoldout newFoldout = null;
                    if (isNewWindow || !IsObjectCreatedAlready(objectPath, EditObjectType.Foldout, out EditWindowObjectBase matchedFoldout))
                    {
                      //  Debug.Log($"new foldout: {objectPath}");
                        GameObject newGO = Instantiate(foldout, parentObject, false);
                        newFoldout = newGO.GetComponent<EditWindowFoldout>();
                        newEditObject = newFoldout;
                        newFoldout.Initialize();
                        SetNewObjectFields();
                        //newFoldout.EditController = this;
                        //newFoldout.Path = objectPath;//tag[0];
                        //newFoldout.label.SetText(tag[0]);
                        //newGO.name = tag[0];
                        //newFoldout.RemoveCommand = commandForRemove;//$"edit {editType} {editId} remove {path}{tag[0]}";
                        if (matchedTag == "grpnull")
                        {
                            newFoldout.remove.gameObject.SetActive(true);
                            //newFoldout.RemoveCommand = commandForRemove;//$"edit {editType} {editId} remove {path}{tag[0]}";
                        }
                        newFoldout.ExpandContent(false);
                        editWindowObjects.Add(newFoldout);
                        //Debug.Log(objectPath == newFoldout.GetPathFromParent());
                    }
                    else
                    {
                        newFoldout = (EditWindowFoldout)matchedFoldout;
                        //newFoldout.transform.SetParent(parentObject, false);
                        newFoldout.transform.SetSiblingIndex(parentObject.transform.childCount - 1);
                        newFoldout.doDelete = false;
                    }
                    /*                GameObject groupGO = Instantiate(foldoutPF, parentObject, false);
                                    FoldoutController foldout = groupGO.GetComponent<FoldoutController>();
                                    //foldout.ExpandContent(false);
                                    foldout.label.text = tag[0];
                                    if (matchedTag == "grpnull")
                                    {
                                        //Debug.Log(foldout.label.GetComponent<TextMeshProUGUI>().preferredWidth);
                                        //Vector2 tempVec = new Vector2(-18f, 0f);
                                        Transform tempT = foldout.transform.GetChild(0).Find("Remove");
                                        tempT.gameObject.SetActive(true);
                                        //tempT.localPosition = tempVec;
                                        EditWindowButtonCommands groupRemoveButtonNew = new EditWindowButtonCommands();
                                        groupRemoveButtonNew.buttonToPress = tempT.gameObject;
                                        //Debug.Log($"{path}{tag[0]}");
                                        groupRemoveButtonNew.command = $"edit {editType} {editId} remove {path}{tag[0]}";
                                        editButtons.Add(groupRemoveButtonNew);
                                    }
                                    groupGO.name = tag[0];
                                    bool doExpand = false;
                                    for (int i = 0; i < tempEditDropdowns.Count; i++)
                                    {
                                        if (tempEditDropdowns[i].dropdownPath == $"{path}{tag[0]}:")
                                        {
                                            doExpand = tempEditDropdowns[i].dropdown.isExpanded;
                                            //foldout.ExpandContent(tempEditDropdowns[i].dropdown.isExpanded);
                                            //Debug.Log($"found dropdown match:{tempEditDropdowns[i].dropdown.isExpanded}");
                                            tempEditDropdowns.RemoveAt(i);
                                            break;
                                        }
                                    }
                                    foldout.ExpandContent(doExpand);
                                    EditWindowDropdowns tempDropdown = new EditWindowDropdowns();
                                    tempDropdown.dropdown = foldout;
                                    tempDropdown.dropdownPath = $"{path}{tag[0]}:";
                                    editDropdowns.Add(tempDropdown);*/

                    //Debug.Log($"Blah:{editLayoutString.Substring(editLayoutString.IndexOf("{", tagStart))}");
                    lastCheckIndex = EditLayoutParse(editLayoutString.IndexOf("{", tagStart), newFoldout.foldout.content, objectPath);// $"{path}{tag[0]}:");//, false);

                    break;
                case "list":
                    EditWindowList newList = null;
                    if (isNewWindow || !IsObjectCreatedAlready(objectPath, EditObjectType.List, out EditWindowObjectBase matchedList))
                    {
                        //  Debug.Log($"new foldout: {objectPath}");
                        GameObject newGO = Instantiate(editList, parentObject, false);
                        newList = newGO.GetComponent<EditWindowList>();
                        newEditObject = newList;
                        newList.Initialize();
                        SetNewObjectFields();
                        //newList.EditController = this;
                        //newList.Path = objectPath;//tag[0];
                        //newList.label.SetText(tag[0]);
                        //newGO.name = tag[0];
                        //newList.RemoveCommand = commandForRemove;//$"edit {editType} {editId} remove {path}{tag[0]}";
                        newList.ExpandContent(false);
                        //EditListContentChild tempComp = newList.foldout.content.AddComponent<EditListContentChild>();
                        //tempComp.listDragZonePF = editListDragZonePF;
                        //tempComp.editListController = newList;
                        //newList.listContent = tempComp;
                        newList.foldout.spacing = 2f;

                        //add the new element button here for this list
                        GameObject buttonGO = Instantiate(newButton);//, newList.foldout.content, false);
                        EditWindowNewButton editNew = buttonGO.GetComponent<EditWindowNewButton>();
                        newList.addElementButton = editNew;
                        editNew.objectType = EditObjectType.NewButton;
                        //tempComp.addElementButton = editNew;
                        buttonGO.transform.SetParent(newList.foldout.content, false);
                        //newEditObject = editNew;
                        //editNew.Initialize();
                        //SetNewObjectFields();
                        editNew.EditController = this;
//                        editNew.removeCommand = $"edit {editType} {editId} remove {path}{tag[0]}";//?
                        editNew.Label.SetText("");
                        //editNew.ObjectName = "";
                        editNew.parentObject = newList;
                        //$"nolabel:newbutton:New {typeProperties[i].Name}:edit {thingType} {thingId} new {path}{typeProperties[i].Name}";
                        editNew.labelOnbutton.text = $"New {newList.ObjectName}";//  tag[2];
                        editNew.buttonCommand = $"edit {editType} {editId} new {path}{newList.ObjectName}";//?//edit {thingType} {thingId} new {path}{typeProperties[i].Name};
                        //Debug.Log($"list path: {path}");
                        //newList.addElementButton = editNew;

                        editWindowObjects.Add(newList);
                        //Debug.Log(objectPath == newList.GetPathFromParent());
                    }
                    else
                    {
                        newList = (EditWindowList)matchedList;
                        //newFoldout.transform.SetParent(parentObject, false);
                        newList.transform.SetSiblingIndex(parentObject.transform.childCount - 1);
                        newList.doDelete = false;
                    }
                    lastCheckIndex = EditLayoutParse(editLayoutString.IndexOf("{", tagStart), newList.foldout.content, objectPath);// $"{path}{tag[0]}:");
                    break;
            }
            //lastCheckIndex += 1;
        }

        //now to go through and get the length of the text in order to set the text object actual width
        float prefWidth = 0;
        for (int i = 0; i < textObjects.Count; i++)
        {
            float tempWidth = textObjects[i].preferredWidth;
            if (tempWidth > prefWidth)
            {
                prefWidth = tempWidth;
            }
        }
        //Debug.Log(prefWidth);
        for (int i = 0; i < textObjects.Count; i++)
        {
            LayoutElement tempElement = textObjects[i].GetComponentInChildren<LayoutElement>();
            tempElement.preferredWidth = tempElement.minWidth = prefWidth;
            //and just in case, for guids and their manual width messing with the autosizing on the label text
            //if this isn't a guid, the width will already be this and the autosizing will 'override' it anyway so it wouldn't matter
            RectTransform tempRect = textObjects[i].gameObject.GetComponent<RectTransform>();
            tempRect.sizeDelta = new Vector2(prefWidth, tempRect.sizeDelta.y);

            //textObjects[i].GetComponentInChildren<LayoutElement>().preferredWidth = prefWidth;
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
        //return ending index of what we just parsed so next parse knows where to start from
        return lastCheckIndex;
    }

    public void UpdateLayoutForObject(EditWindowText editTextObject)
    {
        StartCoroutine(editTextObject.UpdateLayoutObject());
    }

    public bool FindParentObject(EditWindowObjectBase newObject, out EditWindowObjectBase foundParent)
    {
        foundParent = null;
     //   if (path == "")
     //       return false;

      //  if (newObject.parentObject != null)
      //  {
            Transform parentObject = newObject.transform.parent;

            foundParent = parentObject.GetComponentInParent<EditWindowObjectBase>(true);

            if (foundParent != null)
            {
                newObject.parentObject = foundParent;
                newObject.isParentObject = false;
                return true;
            }
        //   }
        newObject.parentObject = null;
        newObject.isParentObject = true;
        return false;
    }

    public bool FindParentObject(string path, out EditWindowObjectBase foundParent)
    {
        foundParent = null;
        if (path == "")
            return false;

        //assuming the only object capable of having children added to them are foldouts and lists
        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            if (editWindowObjects[i].objectType == EditObjectType.Foldout || editWindowObjects[i].objectType == EditObjectType.List)
            {
                //Debug.Log($"path: {path}, parent: {editWindowObjects[i].GetPathFromParent()}");
                if (editWindowObjects[i].GetPathFromParent() == path)
                {
                    foundParent = editWindowObjects[i];
                    //Debug.Log($"found parent with {path}");
                    return true;
                }

            }
        }
        return false;
    }    

    public bool IsObjectCreatedAlready(string path, EditObjectType typeToMatch, out EditWindowObjectBase foundObject)
    {
        foundObject = null;
     //   if (!isNewWindow)
     //   {
     //       Debug.Log($"path: {path}, type: {typeToMatch}");

     //   }

        for (int i = 0; i < editWindowObjects.Count; i++)
        {
            //Debug.Log(editWindowObjects[i].Path == path);
            if (editWindowObjects[i].path == path && editWindowObjects[i].objectType == typeToMatch)
            {
                foundObject = editWindowObjects[i];
               // Debug.Log($"matched object: {foundObject.Path}, {foundObject.objectType}");
                return true;
            }
        }
     //   if (typeToMatch == EditObjectType.Foldout)
     //   {
     //       Debug.Log($"could not match {path}");
     //   }
        return false;
    }

    public class EditWindowButtonCommands
    {
        public GameObject buttonToPress;
        public string command = "";
    }

    [Serializable]
    public class EditWindowValueObjects
    {
        public GameObject valueObject;
        public string objectPath = "";
        public string objectType = "";
    }

    public class EditWindowDropdowns
    {
        public FoldoutController dropdown;
        public string dropdownPath = "";
    }

}
