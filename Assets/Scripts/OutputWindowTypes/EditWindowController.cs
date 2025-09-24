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

    public List<EditWindowButtons> editButtons = new List<EditWindowButtons>();

    //list of all added GameObjects (inputfields, dropdowns) so we can find matches for incoming json IDs in order to set values
    public List<EditWindowValueObjects> editValueObjects = new List<EditWindowValueObjects>();
    public List<EditWindowDropdowns> editDropdowns = new List<EditWindowDropdowns>();
    public List<EditWindowDropdowns> tempEditDropdowns = new List<EditWindowDropdowns>();

    public override string Input
    {
        set
        {
            //Debug.Log(value);
            FindEditObjects(value, editWindowObjects);//incoming values so we look up the objects to find a match

        }
    }

    public override void OnPanelClose()
    {
        ConnectionController.instance.SendData("100", $"edit {editType} {editId} done:button");
        for (int j = 0; j < editButtons.Count; j++)
        {
            //go through all the buttons added and remove listeners
            editButtons[j].buttonToPress.GetComponent<Button>().onClick.RemoveAllListeners();
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

    }

    public void FindEditObjects(string input, Transform parent)//we need to redo this since most of it is wrong
    {
        //Debug.Log(input);
        int lastIndex = input.LastIndexOf(":");
        string value = input.Substring(lastIndex + 1);
        //Debug.Log(value);
        input = input.Remove(lastIndex + 1);
        //Debug.Log(input);

        for (int i = 0; i < editValueObjects.Count; i++)
        {
            if (editValueObjects[i].objectPath == input)
            {
                //Debug.Log($"match found for {input}");
                switch (editValueObjects[i].objectType)
                {
                    case "text":
                        editValueObjects[i].valueObject.GetComponent<TMP_InputField>().text = value;// names[1];
                        //Debug.Log($"input: {input}");
                        break;
                    case "bool":
                        Debug.Log($"bool value: {value}");
                        break;
                    case "dropdown":
                        if (int.TryParse(value, out int dropdownOption) && dropdownOption < editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().options.Count && dropdownOption >= 0)
                        {
                            editValueObjects[i].valueObject.GetComponent<TMP_Dropdown>().value = dropdownOption;
                        }
                        break;
                    default:
                        Debug.Log($"{input}:{value}");
                        break;

                }

            }
        }

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
        for (int i = 0; i < editButtons.Count; i++)
        {
            //go through all the buttons added and add listeners
            editButtons[i].buttonToPress.GetComponent<Button>().onClick.RemoveAllListeners();
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
        editButtons.Clear();
        editValueObjects.Clear();
        if (editDropdowns.Count > 0)
        {
            tempEditDropdowns = editDropdowns.ToList();
            editDropdowns.Clear();
        } else
        {
            editDropdowns.Clear();
            tempEditDropdowns.Clear();
        }

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
        //Debug.Log(editDropdowns.Count);
        for (int i = 0; i < editButtons.Count; i++)
        {
            //go through all the buttons added and add listeners
            editButtons[i].buttonToPress.GetComponent<Button>().onClick.AddListener(SendButtonCommand);
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

        GameObject closeLG = Instantiate(basicObject, editWindowObjects, false);
        //Debug.Log(editNewButtons.Count);
        closeLG.transform.GetChild(0).gameObject.SetActive(false);
        closeLG.transform.GetChild(1).gameObject.SetActive(false);
        //add 'Close' button at the end
        GameObject closeGO = Instantiate(updateButtonPF, closeLG.transform, false);
        closeButton = closeGO.GetComponentInChildren<Button>(true);
        closeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Close");
        closeButton.onClick.AddListener(panel.OnPanelClose);//add the listener to the panel.OnPanelClose as that will do the panel AND this close stuff
    }

    public void SendSaveEdit()
    {
        ConnectionController.instance.SendData("100", $"edit {editType} {editId} save:button");
    }

    public void SendButtonCommand()
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
            ConnectionController.instance.SendData("100", $"edit {editType} {editId} update {editValueObjects[i].objectPath}{value}:button");
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

        string[] layoutTags = { "text", "noedit", "enum", "newbutton", "group", "bool", "grpnull", "txtnull", "}"};
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
                case "noedit":
                case "txtnull"://can we assume that no 'txtnull' object will be a base level? so all of them will be a list element with space to the left?
                    GameObject textGO = Instantiate(basicObject, parentObject, false);
                    textGO.name = tag[0];
                    TextMeshProUGUI textLabel = textGO.GetComponentInChildren<TextMeshProUGUI>();
                    textLabel.SetText(tag[0]);
                    textObjects.Add(textLabel);
                    GameObject textIF = Instantiate(inputFieldStretchedPF, textGO.transform.GetChild(1), false);
                    if (matchedTag == "noedit")
                    {
                        textIF.GetComponent<TMP_InputField>().readOnly = true;
                    }
                    if (matchedTag == "txtnull")
                    {
                        Vector2 tempVec = new Vector2(-18f, 0f);
                        Transform tempT = textGO.transform.Find("Remove");
                        tempT.gameObject.SetActive(true);
                        tempT.localPosition = tempVec;
                        EditWindowButtons textRemoveButtonNew = new EditWindowButtons();
                        textRemoveButtonNew.buttonToPress = tempT.gameObject;
                        //Debug.Log($"{path}{tag[0]}");
                        textRemoveButtonNew.command = $"edit {editType} {editId} update remove {path}{tag[0]}";
                        editButtons.Add(textRemoveButtonNew);
                    }
                    //textIF.name = tag[0];
                    tempObject.valueObject = textIF;
                    //tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectPath = $"{path}{tag[0]}:";
                    tempObject.objectType = "text";
                    editValueObjects.Add(tempObject);
                    lastCheckIndex += 1;
                    break;
        /*        case "noedit":
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
                    break;*/
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
                    //tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectPath = $"{path}{tag[0]}:";
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
                    //tempObject.valueObject.name = $"{path}{tag[0]}:";
                    tempObject.objectPath = $"{path}{tag[0]}:";
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
                        editButtons.Add(buttonNew);
                    }
                    lastCheckIndex += 1;
                    break;
                case "group":
                case "grpnull":
                    //Debug.Log($"Path: {path}{tag[0]}:");
                    GameObject groupGO = Instantiate(foldoutPF, parentObject, false);
                    FoldoutController foldout = groupGO.GetComponent<FoldoutController>();
                    //foldout.ExpandContent(false);
                    foldout.label.text = tag[0];
                    if (matchedTag == "grpnull")
                    {
                        //Debug.Log(foldout.label.GetComponent<TextMeshProUGUI>().preferredWidth);
                        Vector2 tempVec = new Vector2(-18f, 0f);
                        Transform tempT = foldout.transform.GetChild(0).Find("Remove");
                        tempT.gameObject.SetActive(true);
                        tempT.localPosition = tempVec;
                        EditWindowButtons groupRemoveButtonNew = new EditWindowButtons();
                        groupRemoveButtonNew.buttonToPress = tempT.gameObject;
                        Debug.Log($"{path}{tag[0]}");
                        groupRemoveButtonNew.command = $"edit {editType} {editId} update remove {path}{tag[0]}";
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
                    editDropdowns.Add(tempDropdown);

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
        public string objectPath = "";
        public string objectType = "";
    }

    public class EditWindowDropdowns
    {
        public FoldoutController dropdown;
        public string dropdownPath = "";
    }

}
