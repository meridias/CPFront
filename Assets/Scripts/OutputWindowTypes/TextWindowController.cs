using onnaMUD;
using Panels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextWindowController : BaseOutput// MonoBehaviour
{
    public TextSelectHighlight textSelectObject;
    public ScrollRect scrollView;
    public Panel panel;

    //public TMP_Text textObject;

    //public Scrollbar outputVScrollbar;

    public bool replaceText = false;//do we add text to what's already there or replace it?

    //private ScrollRectOverride scrollRectOverride;

    private Scrollbar outputVScrollbar;
    private Coroutine waitForScrollBarCO = null;

//    [HideInInspector]
//    public ClickableLinks clickable;

    public string text
    {
        get
        {
            return textSelectObject.textObject.text;
        }
        set
        {
            if (gameObject == null)
                return;

         //   bool isScrollAtBottom = false;

         //   if (textSelectObject.textObject.preferredHeight < scrollView.viewport.rect.height || outputVScrollbar.value <= .02f || outputVScrollbar.value > 1f)
         //   {
                //Debug.Log(textSelectObject.textObject.preferredHeight);
         //       isScrollAtBottom = true;
         //   }
            //Debug.Log($"before: size={outputVScrollbar.size}, value={outputVScrollbar.value}, {isScrollAtBottom}");

            if (replaceText)
            {
                textSelectObject.textObject.text = value;
            } else
            {
                textSelectObject.textObject.text += value;
            }
            //Debug.Log($"pref: {textSelectObject.textObject.preferredHeight}, view: {scrollView.viewport.rect.height}");

            if (textSelectObject.textObject.preferredHeight < scrollView.viewport.rect.height && scrollView.content.pivot.y != 0f)
            {
                scrollView.content.pivot = new Vector2(scrollView.content.pivot.x, 0f);
            } else if (textSelectObject.textObject.preferredHeight >= scrollView.viewport.rect.height && scrollView.content.pivot.y != 1f)
            {
                scrollView.content.pivot = new Vector2(scrollView.content.pivot.x, 1f);
            }

            if (IsScrollAtBottom() && scrollView.content.pivot.y == 1f)
            {
                //Debug.Log("blah");
                SetScrollToBottom();
                //waitForScrollBarCO = StartCoroutine(WaitForScrollBarUpdate());
                //scrollView.normalizedPosition = new Vector2(0f, 0f);

                //Debug.Log(scrollView.content.anchoredPosition);
                //scrollView.content.anchoredPosition = new Vector2(scrollView.content.anchoredPosition.x, textSelectObject.textObject.preferredHeight - scrollView.viewport.rect.height);// gameObject.GetComponent<RectTransform>().rect.height);
                //outputVScrollbar.value = 0f;
                //scrollView.verticalNormalizedPosition = 0f;
                //Debug.Log(scrollView.verticalNormalizedPosition);
                //Debug.Log(scrollView.content.anchoredPosition);
            }
            //outputVScrollbar.value = 0f;

            //textSelectObject.SetText();

            //Debug.Log($"after: size={outputVScrollbar.size}, value={outputVScrollbar.value}, {isScrollAtBottom}");

            //value = value.Replace("\0", string.Empty); // remove embedded nulls
            //textSelectObject.textObject.ForceMeshUpdate();
            textSelectObject.SetText();// ?
            //Debug.Log(textSelectObject.textObject.text.Length);
            //Debug.Log(textSelectObject.textObject.textInfo.characterCount);
            //SetViewportLocalPos();
        }
    }

    public void SetScrollToBottom()
    {
        waitForScrollBarCO = StartCoroutine(WaitForScrollBarUpdate());

    }

    IEnumerator WaitForScrollBarUpdate()//bool isBottom)
    {
        yield return null;
        //Debug.Log("doh");
        //if (isBottom)
        //outputVScrollbar.value = 0f;
        scrollView.normalizedPosition = new Vector2(0f, 0f);

        //yield return null;
        //Debug.Log($"after: size={outputVScrollbar.size}, value={outputVScrollbar.value}, {isBottom}");
        waitForScrollBarCO = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        outputVScrollbar = scrollView.verticalScrollbar;
        SetViewportLocalPos();
        //        clickable = textSelectObject.textObject.GetComponent<ClickableLinks>();
        //scrollRectOverride = scrollView.gameObject.GetComponent<ScrollRectOverride>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(outputVScrollbar.value);
    }

    public void OnDisable()
    {
        if (waitForScrollBarCO != null)
            StopCoroutine(waitForScrollBarCO);
    }

    public bool IsScrollAtBottom()
    {
        if (textSelectObject.textObject.preferredHeight < scrollView.viewport.rect.height || outputVScrollbar.value <= .02f || outputVScrollbar.value > 1f)
        {
            return true;
        }

        return false;
    }

    public void SetViewportLocalPos()
    {
        //let's try something
        textSelectObject.viewportPosBase = textSelectObject.gameObject.transform.localPosition;

 /*       Vector2 tempPos = new Vector2 (0f, 0f);
        //we're going to assume that textSelectObject is on the scrollView viewport
        GameObject go = textSelectObject.gameObject;

        while (go != null)
        {
            tempPos = new Vector2(tempPos.x + go.transform.localPosition.x, tempPos.y + go.transform.localPosition.y);
            if (go != gameObject)
            {
                //if the gameObject we just added the localPosition from isn't the gameObject that is the top-level parent of this window tree
                //then set it to be the next gameObject to add from
                go = go.transform.parent.gameObject;
            } else
            {
                //if this is the top-level parent, quit
                break;
            }
        }
        textSelectObject.viewportPosBase = tempPos;*/
        //Debug.Log(tempPos);
    }

    /*   public override void OnPointerDown(PointerEventData eventData)
       {
           EventSystem.current.SetSelectedGameObject(gameObject, eventData);
           bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

           CaretPosition insertionSide;

           int insertionIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textObject, eventData.position, eventData.pressEventCamera, out insertionSide);
           //Debug.Log(insertionIndex);
           if (shift)
           {
               if (insertionSide == CaretPosition.Left)
               {
                   scrollRectOverride.selectedStringEnd = insertionIndex == 0
                       ? textObject.textInfo.characterInfo[0].index
                       : textObject.textInfo.characterInfo[insertionIndex - 1].index + textObject.textInfo.characterInfo[insertionIndex - 1].stringLength;
               }
               else if (insertionSide == CaretPosition.Right)
               {
                   scrollRectOverride.selectedStringEnd = textObject.textInfo.characterInfo[insertionIndex].index + textObject.textInfo.characterInfo[insertionIndex].stringLength;
               }
           }
           else
           {
               if (insertionSide == CaretPosition.Left)
               {
                   scrollRectOverride.selectedStringStart = scrollRectOverride.selectedStringEnd = insertionIndex == 0
                       ? textObject.textInfo.characterInfo[0].index
                       : textObject.textInfo.characterInfo[insertionIndex - 1].index + textObject.textInfo.characterInfo[insertionIndex - 1].stringLength;
               }
               else if (insertionSide == CaretPosition.Right)
               {
                   scrollRectOverride.selectedStringStart = scrollRectOverride.selectedStringEnd = textObject.textInfo.characterInfo[insertionIndex].index + textObject.textInfo.characterInfo[insertionIndex].stringLength;
               }
           }

           //UpdateLabel();
           eventData.Use();//?
       }*/


}
