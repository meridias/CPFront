using Newtonsoft.Json.Linq;
using onnaMUD;
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

public class EditWindowController: BaseOutput
{
    public RectTransform editWindowObjects;
    private string editLayoutString = "";
    private string editType = "";//room, character, etc
    private string editId = "";//guid for whatever we're editing

    //basic editing object
    public GameObject basicObject;

    //input field or dropdown objects for basic editing
    public GameObject inputFieldStretchedPF;
    public GameObject dropdownPF;
    public GameObject foldoutPF;
    public GameObject newButtonPF;
    public GameObject updateButtonPF;
    //public 
    public Button updateButton;
    public Button saveButton;
    public Button closeButton;

    public List<EditWindowButtons> editNewButtons = new List<EditWindowButtons>();

    //list of all added GameObjects (inputfields, dropdowns) so we can find matches for incoming json IDs in order to set values
    public List<EditWindowValueObjects> editValueObjects = new List<EditWindowValueObjects>();

    public override string Input
    {
        set
        {
            //Debug.Log(value);
            FindEditObjects(value, editWindowObjects);//incoming values so we look up the objects to find a match

        }
    }

    public void FindEditObjects(string input, Transform parent)
    {
        string[] names = input.Split(":", StringSplitOptions.RemoveEmptyEntries);

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
        }

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
        for (int i = 0; i < editNewButtons.Count; i++)
        {
            //go through all the buttons added and add listeners
            editNewButtons[i].buttonToPress.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        if (updateButton != null)
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
        }
        //now remove any and all objects from the window and start new
        for (int i = editWindowObjects.childCount - 1; i >= 0; i--)
        {
            Transform childTransform = editWindowObjects.transform.GetChild(i);
            GameObject childGameObject = childTransform.gameObject;
            Destroy(childGameObject);
        }
        editNewButtons.Clear();
        editValueObjects.Clear();

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
        //Debug.Log(editLayoutString);
        EditLayoutParse(0, editWindowObjects, "");
        for (int i = 0; i < editNewButtons.Count; i++)
        {
            //go through all the buttons added and add listeners
            editNewButtons[i].buttonToPress.GetComponent<Button>().onClick.AddListener(SendButtonCommand);
        }
        //add 'Update' button at the end
        GameObject saveUpdateLG = Instantiate(basicObject, editWindowObjects, false);
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

        GameObject closeGO = Instantiate(basicObject, editWindowObjects, false);
        //Debug.Log(editNewButtons.Count);
        closeGO.transform.GetChild(0).gameObject.SetActive(false);
        closeGO.transform.GetChild(1).gameObject.SetActive(false);
        //add 'Close' button at the end
        GameObject saveLG = Instantiate(updateButtonPF, editWindowObjects, false);
        closeButton = saveLG.GetComponentInChildren<Button>(true);
        closeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Close");
        closeButton.onClick.AddListener(SendClose);
    }

    public void SendSaveEdit()
    {
        ConnectionController.instance.SendData("100", $"edit {editType} {editId} save:button");
    }
    public void SendClose()
    {
        ConnectionController.instance.SendData("100", $"edit {editType} {editId} done:button");
        for (int i = 0; i < MainController.instance.editWindows.Count; i++)
        {
            if (MainController.instance.editWindows[i].editType == editType && MainController.instance.editWindows[i].editID.ToString() == editId)
            {
                //found a match to type and guid in an open edit window
                //editPanel = editWindows[i].editPanel;
                //foundMatch = true;
                for (int j = 0; j < editNewButtons.Count; j++)
                {
                    //go through all the buttons added and add listeners
                    editNewButtons[j].buttonToPress.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                if (updateButton != null)
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
                }
                Destroy(MainController.instance.editWindows[i].editPanel.gameObject);//we need to work on a Panel.Close method...
                MainController.instance.editWindows.RemoveAt(i);
                break;
            }
        }

    }

    public void SendButtonCommand()
    {
        for (int i = 0; i < editNewButtons.Count; i++)
        {
            if (editNewButtons[i].buttonToPress == EventSystem.current.currentSelectedGameObject)
            {
                //deselect button so enter doesn't work on it?
                EventSystem.current.SetSelectedGameObject(null);
                //this is the button we just pressed, so send the command without game window echo
                if (ConnectionController.instance.isConnected)
                {
                    ConnectionController.instance.SendData("100", $"{editNewButtons[i].command}:button");
                    //Debug.Log($"{editNewButtons[i].command}");
                }
            }
        }
    }

    public void SendUpdates()
    {
        //string tag = $"{editType}:{editId},";
        string value = "";

        for (int i = 0; i < editValueObjects.Count; i++)
        {
            //Debug.Log($"{tag}{editValueObjects[i].valueObject.name}");
            switch (editValueObjects[i].objectType)
            {
                case "text":
                    value = editValueObjects[i].valueObject.GetComponent<TMP_InputField>().text;
                    break;
                case "dropdown":
                case "bool":
                    value = editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value.ToString();
                    break;


            }
            if (value == "")
            {
                //ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}button");
                value = "nullvalue";
            }// else
           // {
           //     ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}:button");
         //   }
            ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}:button");
            //Debug.Log($"edit {editType} {editId} update {editValueObjects[i].valueObject.name}{value}");
            
        }
        //ConnectionController.instance.SendData("100", $"edit {editType} {editId} update done:done:button");
    }

    public int EditLayoutParse(int startIndex, RectTransform parentObject, string path)//, bool keepGoing = true)
    {
        //int endIndex = 0;
        int lastCheckIndex = startIndex;
        //int minIndex = -1;
        bool parseTags = true;
        string matchedTag = "";

        string[] layoutTags = { "text", "noedit", "enum", "newbutton", "group", "bool", "}"};
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
            } else
            {
                tagStart = minIndex;
            }
            //Debug.Log(tagStart);
            //Debug.Log($"Blah: {editLayoutString.Substring(tagStart)}");
                string layoutSubstring = editLayoutString.Substring(tagStart);
            string[] layoutTag = layoutSubstring.Split(",", StringSplitOptions.RemoveEmptyEntries);
            string[] tag = layoutTag[0].Split(":", StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log(layoutSubstring);

            switch (matchedTag)
            {
                case "}":
                    parseTags = false;
                    lastCheckIndex += 1;
                    break;
                case "text":
                   // string[] textTag = tagTest.Split(",", StringSplitOptions.RemoveEmptyEntries);
                   // string[] text = textTag[0].Split(":", StringSplitOptions.RemoveEmptyEntries);
                    GameObject textGO = Instantiate(basicObject, parentObject, false);
                    textGO.name = tag[0];
                    TextMeshProUGUI textLabel = textGO.GetComponentInChildren<TextMeshProUGUI>();
                    textLabel.SetText(tag[0]);
                    textObjects.Add(textLabel);
                    GameObject textIF = Instantiate(inputFieldStretchedPF, textGO.transform.GetChild(1), false);
                    //textIF.name = tag[0];
                    tempObject.valueObject = textIF;
                    tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectType = "text";
                    editValueObjects.Add(tempObject);
                    lastCheckIndex += 1;
                    break;
                case "noedit":
                  //  string[] noeditTag = tagTest.Split(",", StringSplitOptions.RemoveEmptyEntries);
                  //  string[] noedit = noeditTag[0].Split(":", StringSplitOptions.RemoveEmptyEntries);
                    GameObject noeditGO = Instantiate(basicObject, parentObject, false);
                    noeditGO.name = tag[0];
                    TextMeshProUGUI noeditLabel = noeditGO.GetComponentInChildren<TextMeshProUGUI>();
                    noeditLabel.SetText(tag[0]);
                    textObjects.Add(noeditLabel);
                    GameObject noeditIF = Instantiate(inputFieldStretchedPF, noeditGO.transform.GetChild(1), false);
                    noeditIF.GetComponent<TMP_InputField>().readOnly = true;
                    //noeditIF.name = tag[0];
                    tempObject.valueObject = noeditIF;
                    tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectType = "text";
                    editValueObjects.Add(tempObject);
                    lastCheckIndex += 1;
                    break;
                case "bool":
                    GameObject boolBase = Instantiate(basicObject, editWindowObjects, false);
                    boolBase.name = tag[0];
                    TextMeshProUGUI boolLabel = boolBase.GetComponentInChildren<TextMeshProUGUI>();
                    boolLabel.SetText(tag[0]);
                    textObjects.Add(boolLabel);
                    GameObject boolGO = Instantiate(dropdownPF, boolBase.transform.GetChild(1), false);
                    TMP_Dropdown boolDP = boolGO.GetComponent<TMP_Dropdown>();
                    boolDP.ClearOptions();
                    List<string> boolOptions = new List<string>();
                    boolOptions.Add("True");
                    boolOptions.Add("False");
                    boolDP.AddOptions(boolOptions);
                    tempObject.valueObject = boolGO;
                    tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectType = "bool";
                    editValueObjects.Add(tempObject);
                    lastCheckIndex += 1;
                    break;
                case "enum":
                    int enumStart = layoutSubstring.IndexOf("{");
                    int enumEnd = layoutSubstring.IndexOf("}");
                    string enumString = layoutSubstring.Substring(enumStart + 1, enumEnd - enumStart - 1);
                    //Debug.Log(enumString);
                    GameObject dropdownBase = Instantiate(basicObject, editWindowObjects, false);
                    dropdownBase.name = tag[0];
                    TextMeshProUGUI dropdownLabel = dropdownBase.GetComponentInChildren<TextMeshProUGUI>();
                    dropdownLabel.SetText(tag[0]);
                    textObjects.Add(dropdownLabel);
                    GameObject dropdownGO = Instantiate(dropdownPF, dropdownBase.transform.GetChild(1), false);
                    TMP_Dropdown dropdownDP = dropdownGO.GetComponent<TMP_Dropdown>();
                    dropdownDP.ClearOptions();
                    //dropdownGO.name = tag[0];

                    if (enumString.Length > 0)
                    {
                        string[] enumValues = enumString.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        dropdownDP.AddOptions(enumValues.ToList());
                    }
                    //Debug.Log(enumString);
                    lastCheckIndex = editLayoutString.IndexOf("}", tagStart);
                    tempObject.valueObject = dropdownGO;
                    tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectType = "dropdown";
                    editValueObjects.Add(tempObject);
                    lastCheckIndex += 1;
                    break;
                case "newbutton":
                    //Debug.Log(layoutTag[0]);
                    //goto case "text";
                    if (tag[0] == "newbutton")
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
                        EditWindowButtons buttonNew = new EditWindowButtons();
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
                        editNewButtons.Add(buttonNew);
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
                        EditWindowButtons buttonNew = new EditWindowButtons();
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
                        editNewButtons.Add(buttonNew);
                    }
                    lastCheckIndex += 1;
                    break;
                case "group":
                    GameObject groupGO = Instantiate(foldoutPF, parentObject, false);
                    FoldoutController foldout = groupGO.GetComponent<FoldoutController>();
                    foldout.ExpandContent(false);
                    foldout.label.text = tag[0];
                    groupGO.name = tag[0];
                    //Debug.Log($"Blah:{editLayoutString.Substring(editLayoutString.IndexOf("{", tagStart))}");
                    lastCheckIndex = EditLayoutParse(editLayoutString.IndexOf("{", tagStart), foldout.content, $"{path}{tag[0]}:");//, false);

             //       int groupStart = tagTest.IndexOf("{");
             //       int groupEnd = tagTest.IndexOf("}");
             //       string groupString = tagTest.Substring(groupStart + 1, groupEnd - groupStart - 1);
             //       Debug.Log(groupString);

                    // noeditGO.GetComponentInChildren<TextMeshProUGUI>().SetText(tag[0]);
                    // GameObject noeditIF = Instantiate(inputFieldStretchedPF, noeditGO.transform.GetChild(1), false);
                    // noeditIF.GetComponent<TMP_InputField>().readOnly = true;
                    // noeditIF.name = tag[0];

                    //EditLayoutParse();


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
            textObjects[i].GetComponentInChildren<LayoutElement>().preferredWidth = prefWidth;
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
        //return ending index of what we just parsed so next parse knows where to start from
        return lastCheckIndex;
    }

    public class EditWindowButtons
    {
        public GameObject buttonToPress;
        public string command = "";
    }

    [Serializable]
    public class EditWindowValueObjects
    {
        public GameObject valueObject;
        public string objectType = "";
    }

}
