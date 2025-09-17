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
    //public Panel panel;

    //public TMP_Text textObject;

    //public Scrollbar outputVScrollbar;

    public bool replaceText = false;//do we add text to what's already there or replace it?

    //private ScrollRectOverride scrollRectOverride;

    private Scrollbar outputVScrollbar;
    private Coroutine waitForScrollBarCO = null;

//    [HideInInspector]
//    public ClickableLinks clickable;

    public override string Input
    {
        set
        {
            if (gameObject == null)
                return;

            if (replaceText)
            {
                textSelectObject.textObject.text = value;
            }
            else
            {
                textSelectObject.textObject.text += value;
            }

            //only do all this if not replacing text?

            if (!replaceText)
            {
                if (textSelectObject.textObject.preferredHeight < scrollView.viewport.rect.height && scrollView.content.pivot.y != 0f)
                {
                    scrollView.content.pivot = new Vector2(scrollView.content.pivot.x, 0f);
                }
                else if (textSelectObject.textObject.preferredHeight >= scrollView.viewport.rect.height && scrollView.content.pivot.y != 1f)
                {
                    scrollView.content.pivot = new Vector2(scrollView.content.pivot.x, 1f);
                }

                if (IsScrollAtBottom() && scrollView.content.pivot.y == 1f)
                {
                    SetScrollToBottom();
                }
            }
            textSelectObject.SetText();
        }
    }

 /*   public string text
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
         //       isScrollAtBottom = true;
         //   }

            if (replaceText)
            {
                textSelectObject.textObject.text = value;
            } else
            {
                textSelectObject.textObject.text += value;
            }

            if (textSelectObject.textObject.preferredHeight < scrollView.viewport.rect.height && scrollView.content.pivot.y != 0f)
            {
                scrollView.content.pivot = new Vector2(scrollView.content.pivot.x, 0f);
            } else if (textSelectObject.textObject.preferredHeight >= scrollView.viewport.rect.height && scrollView.content.pivot.y != 1f)
            {
                scrollView.content.pivot = new Vector2(scrollView.content.pivot.x, 1f);
            }

            if (IsScrollAtBottom() && scrollView.content.pivot.y == 1f)
            {
                SetScrollToBottom();
                //waitForScrollBarCO = StartCoroutine(WaitForScrollBarUpdate());
                //scrollView.normalizedPosition = new Vector2(0f, 0f);
                //scrollView.content.anchoredPosition = new Vector2(scrollView.content.anchoredPosition.x, textSelectObject.textObject.preferredHeight - scrollView.viewport.rect.height);// gameObject.GetComponent<RectTransform>().rect.height);
                //outputVScrollbar.value = 0f;
                //scrollView.verticalNormalizedPosition = 0f;
            }
            //outputVScrollbar.value = 0f;
            //textSelectObject.SetText();
            //value = value.Replace("\0", string.Empty); // remove embedded nulls
            //textSelectObject.textObject.ForceMeshUpdate();
            textSelectObject.SetText();// ?
            //SetViewportLocalPos();
        }
    }*/

    //testing
 //   public override void SetText()
 //   {
 //       textSelectObject.SetText();
 //   }

    public bool IsScrollAtBottom()
    {
        if (textSelectObject.textObject.preferredHeight < scrollView.viewport.rect.height || outputVScrollbar.value <= .02f || outputVScrollbar.value > 1f)
        {
            return true;
        }
        return false;
    }

    public void SetScrollToBottom()
    {
        waitForScrollBarCO = StartCoroutine(WaitForScrollBarUpdate());
    }

    IEnumerator WaitForScrollBarUpdate()//bool isBottom)
    {
        yield return null;
        //outputVScrollbar.value = 0f;
        scrollView.normalizedPosition = new Vector2(0f, 0f);
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

    public override void OnPanelResize(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        bool scrollIsAtBottom = IsScrollAtBottom();
     //   if (panel.output != null)
     //   {
     //       scrollIsAtBottom = IsScrollAtBottom();//need to make sure this panel actually HAS a text object in it?
     //   }
        if (panel.Group != null && panel.Group.position == Direction.Main)
        {//check drag point against outside edge of Main panelGroup and adjust drag Vector2 to keep it inside the lines
            Vector2 localPoint;
            float modX = eventData.position.x;
            float modY = eventData.position.y;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.Group.RectTransform, eventData.position, panel.Canvas.worldCamera, out localPoint);
            if (localPoint.x > panel.Group.RectTransform.sizeDelta.x)
            {
                //drag point x is outside the main group to the right
                float xDif = localPoint.x - panel.Group.RectTransform.sizeDelta.x;//this is how far out we are
                modX = modX - xDif;
            }
            else if (localPoint.x < 0f)
            {
                modX = modX - localPoint.x;
            }
            if (localPoint.y < 0f)
            {
                float yDif = -localPoint.y;
                modY = modY + yDif;
            }
            else if (localPoint.y > panel.Group.RectTransform.sizeDelta.y)
            {
                float yDif = localPoint.y - panel.Group.RectTransform.sizeDelta.y;
                modY = modY - yDif;
            }
            pos = new Vector2(modX, modY);
        }

        float draggingX = PanelUtils.GetXDragDelta(panel, pos);// eventData.position);
        float draggingY = PanelUtils.GetYDragDelta(panel, pos);// eventData.position);
                                                               //Debug.Log(draggingY);
        for (int i = 0; i < panel.draggedZones.Count; i++)
        {
            if ((panel.draggedZones[i].direction == Direction.Left || panel.draggedZones[i].direction == Direction.Right) && draggingX == 0f)
                continue;//not actually moving to the left/right so skip
            if ((panel.draggedZones[i].direction == Direction.Top || panel.draggedZones[i].direction == Direction.Bottom) && draggingY == 0f)
                continue;//not actually moving up/down so skip
            if (panel.draggedZones[i].direction == Direction.Left || panel.draggedZones[i].direction == Direction.Right)
            {
                panel.OnResize(panel.draggedZones[i].direction, draggingX);
              //  if (scrollIsAtBottom && panel.output != null && scrollView.content.pivot.y == 1f)
              //  {
              //      SetScrollToBottom();
              //  }
            }
            else
            {
                panel.OnResize(panel.draggedZones[i].direction, draggingY);
              //  if (scrollIsAtBottom && panel.output != null && scrollView.content.pivot.y == 1f)
              //  {
              //      SetScrollToBottom();
              //  }
            }
            if (scrollIsAtBottom && scrollView.content.pivot.y == 1f)
            {
                SetScrollToBottom();
            }
            textSelectObject.SetText();
        }
    }

    public void OnDisable()
    {
        if (waitForScrollBarCO != null)
            StopCoroutine(waitForScrollBarCO);
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
