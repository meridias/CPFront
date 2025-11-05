using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EditWindowText : EditWindowObjectBase
{
    //public string Path { get; set; }
    //public string RemoveCommand { get; set; } = "";
    //public EditWindowController EditController { get; set; }
    [SerializeField]
    private TextMeshProUGUI _label;//this is the label next to whatever we're instantiating
    public string oldValue = "";
    //public string removeCommand = "";

    //public TextMeshProUGUI label;
    public TMP_InputField inputField;
    public GameObject remove;
    public bool isMulti = false;
    public HorizontalLayoutGroup layoutGroup;
//    public RectTransform inputRect;
//    public RectTransform layoutRect;
    public LayoutElement layoutElement;
    public RectTransform caretRect;
    public RectTransform textRect;
    public RectTransform viewportRect;

    public override TextMeshProUGUI Label
    {
        get
        {
            return _label;
        }
    }

    //   public EditWindowController editController;
    public override void Initialize()
    {
        objectType = EditObjectType.Text;
        dragObject.gameObject.SetActive(false);
        inputField.onValueChanged.AddListener(EmptyGuidText);
    }

    void OnDisable()
    {
        inputField.onValueChanged.RemoveListener(EmptyGuidText);
    }

    public void EmptyGuidText(string value)
    {
        if (value == "" && layoutElement.flexibleWidth == 1)
        {
            //this is a guid field and is now empty
            inputField.text = oldValue;
        }
    }

    public void Awake()
    {
     //   horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
      //  inputRect = inputField.GetComponent<RectTransform>();
     //   layoutRect = horizontalLayoutGroup.GetComponent<RectTransform>();
        //layoutElement = GetComponent<LayoutElement>();
        //layoutElement.minHeight = inputField.preferredHeight;
        if (inputField.text == "")
        {
            //RectTransform tempRect = (RectTransform)inputField.textViewport.Find("Placeholder");
            TextMeshProUGUI tempText = inputField.textViewport.Find("Placeholder").GetComponent<TextMeshProUGUI>();
            RectTransform viewRect = inputField.textViewport.GetComponent<RectTransform>();
            //Debug.Log($"{label.text}: {tempText.preferredHeight}, {viewRect.offsetMin.y}, {viewRect.offsetMax.y}");
            layoutElement.minHeight = tempText.preferredHeight + viewRect.offsetMin.y + MathF.Abs(viewRect.offsetMax.y);
        }
    }

    public void OnEnable()
    {
        if (caretRect == null)
        {
            caretRect = (RectTransform)viewportRect.Find("Caret");
            //caretRect.offsetMin = textRect.offsetMin;
            //caretRect.offsetMax = textRect.offsetMax;
        }

        if (isMulti)
        {
            EditController.UpdateLayoutForObject(this);
        }

        if (layoutGroup.childControlWidth == false && inputField.text.Length > 0)
        {
            StartCoroutine(SetInputFieldWidthCR());
        }

    }

    public IEnumerator SetInputFieldWidthCR()
    {
        RectTransform tempRect = inputField.gameObject.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tempRect);
        yield return new WaitForEndOfFrame();
        //LayoutRebuilder.MarkLayoutForRebuild(tempRect);
        float preferredWidth = LayoutUtility.GetPreferredWidth(tempRect);
        //Debug.Log(preferredWidth);
        tempRect.sizeDelta = new Vector2(preferredWidth, tempRect.sizeDelta.y);
    }

    public override void SetValue(string newValue)
    {
        if (newlyCreated)
        {
            oldValue = newValue;
            inputField.text = newValue;
            //try to update the size of the input field based on incoming value
            //if this isn't a guid type text, then the 'control child size' of the layout group is still active and it won't matter
            RectTransform tempRect = inputField.gameObject.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tempRect);
            float preferredWidth = LayoutUtility.GetPreferredWidth(tempRect);
            tempRect.sizeDelta = new Vector2(preferredWidth, tempRect.sizeDelta.y);
            newlyCreated = false;
        }
    }

    public override void SetOldValue(string value)
    {
        oldValue = value;
    }

    public void SetTextType(string textType)
    {
        layoutElement.flexibleWidth = -1;
        inputField.readOnly = false;
        inputField.lineType = TMP_InputField.LineType.SingleLine;
        isMulti = false;
        remove.SetActive(false);

        switch (textType)
        {
            case "noedit":
                inputField.readOnly = true;
                break;
            case "multi":
                inputField.lineType = TMP_InputField.LineType.MultiLineSubmit;
                isMulti = true;
                break;
            case "txtnull":
                remove.SetActive(true);
                break;
            case "guid":
                gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = false;
                layoutElement.flexibleWidth = 1;
                break;
            case "idnull":
                remove.SetActive(true);
                gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = false;
                layoutElement.flexibleWidth = 1;
                break;
        }

    }

    public override bool NeedToSendUpdate(out string updatedValue)
    {
        //Debug.Log(inputField.text);
        if (inputField.text != oldValue)
        {
            //Debug.Log($"true: {Path}:{inputField.text}");
            updatedValue = inputField.text;
            return true;

        } else
        {
            //Debug.Log($"false: {Path}:{inputField.text}");
            updatedValue = "";
            return false;
        }

    }

    public override void SendRemoveCommand()
    {
        EditController.SendButtonCommand(removeCommand);
    }

    public void InputFieldValueChanged()
    {
        if (isMulti)
        {
            //StartCoroutine(UpdateLayoutObject());
            EditController.UpdateLayoutForObject(this);
        }
        else if (!isMulti && layoutElement.preferredHeight != -1)
        {
            //Debug.Log(label.text);
            layoutElement.preferredHeight = -1;
        }

    }

    public IEnumerator UpdateLayoutObject()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log(label.text);
        //Debug.Log($"{inputField.textComponent.preferredHeight}");
        //Debug.Log($"IF: {inputField.preferredHeight}, LE: {layoutElement.preferredHeight}");
        if (inputField.preferredHeight != layoutElement.preferredHeight)
        {
            // Debug.Log(inputField.preferredHeight);
            layoutElement.preferredHeight = inputField.preferredHeight;
            if (caretRect != null)
            {
                caretRect.offsetMin = textRect.offsetMin = new Vector2(textRect.offsetMin.x, 0f);
                caretRect.offsetMax = textRect.offsetMax = new Vector2(textRect.offsetMax.x, 0f);
            }
            else
            {
                textRect.offsetMin = new Vector2(textRect.offsetMin.x, 0f);
                textRect.offsetMax = new Vector2(textRect.offsetMax.x, 0f);
            }
            //yield return new WaitForEndOfFrame();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRect);
        }

    }

}

