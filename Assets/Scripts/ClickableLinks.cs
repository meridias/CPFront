using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using onnaMUD;

public class ClickableLinks : MonoBehaviour, IPointerClickHandler
{
    public List<AvailableLinksToClick> availableLinks = new List<AvailableLinksToClick>();
    public TextMeshProUGUI text;
    //public TMP_InputField textOutput;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        //textOutput = GetComponent<TMP_InputField>();
    }

    public async Task AddLinkToList(string linkID, string clickable)
    {
        //Debug.Log(linkID + " " + clickable);
        AvailableLinksToClick tempLink = new AvailableLinksToClick();
        tempLink.ID = linkID;
        tempLink.clickableText = clickable;
        availableLinks.Add(tempLink);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void LateUpdate()
    {
        //Debug.Log(TMP_TextUtilities.IsIntersectingRectTransform(MainController.instance.textOutput.rectTransform, Input.mousePosition, MainController.instance.mainCamera));
        //Debug.Log($"{gameObject.name} " + gameObject.GetComponent<TextMeshProUGUI>().textInfo.linkCount);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("blahbity");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("doh");
            //TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
            //Debug.Log(TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, MainController.instance.mainCamera));
            //Vector2 blah = Mouse.current.position.ReadValue();//was Input.mousePosition
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Mouse.current.position.ReadValue(), MainController.instance.mainCamera);// Input.mousePosition, MainController.instance.mainCamera);
            if (linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
                ConnectionController.instance.SendData("105", $"{linkInfo.GetLinkID()}");
                //Debug.Log(linkInfo.GetLinkID());
            }

        }


    }

    public class AvailableLinksToClick
    {
        public string ID;
        public string clickableText;
        public string highlightedText;
    }
}
