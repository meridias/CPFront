using onnaMUD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class TextSelectHighlight : Selectable, IUpdateSelectedHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    //IScrollHandler,
    ICanvasElement,
    IPointerClickHandler
{
    #region Variables
    //public ScrollRect scrollView;

    //[SerializeField]
    private TextWindowController windowController;
    //public TextMeshProUGUI textObject;
    public TMP_Text textObject;
    //public GameObject contentObject;
    //private RectTransform contentRect;
    [SerializeField]
    private Color m_SelectionColor = new Color(168f / 255f, 206f / 255f, 255f / 255f, 192f / 255f);

    //private RectTransform contentRect;

    private bool m_UpdateDrag = false;
    private bool m_DragPositionOutOfBounds = false;
    private Coroutine m_DragCoroutine = null;
    private const float kVScrollSpeed = 0.10f;
    private WaitForSecondsRealtime m_WaitForSecondsRealtime;
//    private int m_CaretWidth = 1;//do we need this?
    private CanvasRenderer m_HighLightRenderer;
    private RectTransform highlightRectTrans = null;
    private KeyCode m_LastKeyCode;
    private Event m_ProcessingEvent = new Event();
    private bool m_PreventCallback = false;
    private Rect viewportWSRect;
    //private RectTransform contentRect;

    protected RectTransform textViewport;
    protected int m_SelectedStringStart = 0;
    protected int m_SelectedStringEnd = 0;
    protected int m_CaretStart = 0;
    protected int m_CaretEnd = 0;
    protected Mesh m_Mesh;

    [HideInInspector]
    public Vector2 viewportPosBase;

    private bool m_IsTextComponentUpdateRequired = false;
    /*   public string text
       {
           get
           {
               return textObject.text;
           }
           set
           {
               SetText(value);
           }
       }*/

    public void SetText()//string value)
    {
        //      if (this.text == value)
        //          return;

        //      if (value == null)
        //          value = "";

        Rect textViewportRect = textViewport.rect;//textViewport is set during OnEnable, fyi so it's easier to get the rect instead of always GetComponent
        viewportWSRect = new Rect(viewportPosBase.x + textViewportRect.x, viewportPosBase.y + textViewportRect.y, textViewportRect.width, textViewportRect.height);

        //value = value.Replace("\0", string.Empty); // remove embedded nulls
        m_IsTextComponentUpdateRequired = true;
        LayoutRebuilder.MarkLayoutForRebuild(windowController.scrollView.content);// contentRect);
        //      textObject.text = value;
        UpdateLabel();
    }

    protected Mesh mesh
    {
        get
        {
            if (m_Mesh == null)
                m_Mesh = new Mesh();
            return m_Mesh;
        }
    }

    protected int selectedStringStart { get { return m_SelectedStringStart; } set { m_SelectedStringStart = value; ClampStringPos(ref m_SelectedStringStart); } }
    protected int selectedStringEnd { get { return m_SelectedStringEnd; } set { m_SelectedStringEnd = value; ClampStringPos(ref m_SelectedStringEnd); } }
    protected int highlightStringStart { get { return m_CaretStart; } set { m_CaretStart = value; ClampCaretPos(ref m_CaretStart); } }
    protected int highlightStringEnd { get { return m_CaretEnd; } set { m_CaretEnd = value; ClampCaretPos(ref m_CaretEnd); } }

    public Color selectionColor { get { return m_SelectionColor; } set { if (SetPropertyUtility.SetColor(ref m_SelectionColor, value)) MarkGeometryAsDirty(); } }

    private bool hasSelection { get { return selectedStringStart != selectedStringEnd; } }

    #endregion

    protected override void OnEnable()
    {
        textViewport = gameObject.GetComponent<RectTransform>();

        //Debug.Log(windowController.gameObject.name);

        //if (contentObject != null)
       // {
         //   contentRect = contentObject.GetComponent<RectTransform>();
       // }

        if (textObject.text == null)
            textObject.text = string.Empty;

        if (Application.isPlaying)
        {
            windowController = GetComponentInParent<TextWindowController>();
            //Debug.Log(windowController.gameObject.name);
            if (m_HighLightRenderer == null && textObject != null)
            {
                GameObject go = new GameObject("Caret", typeof(TMP_SelectionCaret));

                go.hideFlags = HideFlags.DontSave;
                go.transform.SetParent(textObject.transform.parent);
                //go.transform.SetParent(textObject.transform);
                go.transform.SetAsFirstSibling();
                go.layer = gameObject.layer;

                highlightRectTrans = go.GetComponent<RectTransform>();
                m_HighLightRenderer = go.GetComponent<CanvasRenderer>();
                m_HighLightRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);

                // Needed as if any layout is present we want the caret to always be the same as the text area.
                go.AddComponent<LayoutElement>().ignoreLayout = true;

                //AssignPositioningIfNeeded();
            }
        }

        // If we have a cached renderer then we had OnDisable called so just restore the material.
        if (m_HighLightRenderer != null)
            m_HighLightRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);

        //contentRect = transform.GetChild(0).gameObject.GetComponent<RectTransform>();

        if (textObject != null)
        {
            //textObject.RegisterDirtyVerticesCallback(MarkGeometryAsDirty);
            //textObject.RegisterDirtyVerticesCallback(UpdateLabel);

            // Cache reference to Vertical Scrollbar RectTransform and add listener.
   //         if (m_VerticalScrollbar != null)
   //         {
   //             m_VerticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChange);
            }

//            UpdateLabel();
  //      }
    }

    protected override void OnDisable()
    {
        CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

        if (m_HighLightRenderer != null)
            m_HighLightRenderer.Clear();

        if (m_Mesh != null)
            DestroyImmediate(m_Mesh);

        m_Mesh = null;
    }

    public void ProcessEvent(Event e)
    {
        KeyPressed(e);
    }

    //keyboard hotkeys, cntrl-c
    public virtual void OnUpdateSelected(BaseEventData eventData)
    {
        bool consumedEvent = false;
        EditState editState = EditState.Continue;

        while (Event.PopEvent(m_ProcessingEvent))
        {
            //Debug.Log("Event: " + m_ProcessingEvent.ToString() + "  IsCompositionActive= " + m_IsCompositionActive + "  Composition Length: " + compositionLength);

            EventType eventType = m_ProcessingEvent.rawType;

            if (eventType == EventType.KeyUp)
                continue;

            if (eventType == EventType.KeyDown)
            {
                consumedEvent = true;

                editState = KeyPressed(m_ProcessingEvent);

                //m_IsTextComponentUpdateRequired = true;
      //          UpdateLabel();

                continue;
            }

            switch (eventType)
            {
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    switch (m_ProcessingEvent.commandName)
                    {
                        case "SelectAll":
 //                           SelectAll();
                            consumedEvent = true;
                            break;
                    }
                    break;
            }

        }

        if (consumedEvent)
        {
  //          UpdateLabel();
            eventData.Use();
        }
    }

    protected virtual void LateUpdate()
    {
        AssignPositioningIfNeeded();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        bool shift = Keyboard.current.shiftKey.IsPressed();
        windowController.panel.SetAsLast();
        //bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

  /*      int charIndex = TMP_TextUtilities.FindNearestCharacter(textObject, Input.mousePosition, MainController.instance.mainCamera, true);
        Debug.Log(charIndex);
        if (charIndex > -1)
        {
            Debug.Log($"blah:{textObject.textInfo.characterInfo[charIndex].character}");
        }*/

        CaretPosition insertionSide;

        int insertionIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textObject, eventData.position, eventData.pressEventCamera, out insertionSide);
        //Debug.Log($"insert:{insertionIndex}");
        if (shift)
        {
            if (insertionSide == CaretPosition.Left)
            {
                selectedStringEnd = insertionIndex == 0
                    ? textObject.textInfo.characterInfo[0].index
                    : textObject.textInfo.characterInfo[insertionIndex - 1].index + textObject.textInfo.characterInfo[insertionIndex - 1].stringLength;
            }
            else if (insertionSide == CaretPosition.Right)
            {
                selectedStringEnd = textObject.textInfo.characterInfo[insertionIndex].index + textObject.textInfo.characterInfo[insertionIndex].stringLength;
            }
        } else
        {
            if (insertionSide == CaretPosition.Left)
            {
                selectedStringStart = selectedStringEnd = insertionIndex == 0
                    ? textObject.textInfo.characterInfo[0].index
                    : textObject.textInfo.characterInfo[insertionIndex - 1].index + textObject.textInfo.characterInfo[insertionIndex - 1].stringLength;
            }
            else if (insertionSide == CaretPosition.Right)
            {
                selectedStringStart = selectedStringEnd = textObject.textInfo.characterInfo[insertionIndex].index + textObject.textInfo.characterInfo[insertionIndex].stringLength;
            }
        }

        highlightStringStart = highlightStringEnd = GetCaretPositionFromStringIndex(selectedStringStart);

        //Debug.Log(selectedStringEnd);
        
        UpdateLabel();
        eventData.Use();//?
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("doh");
        windowController.panel.SetAsLast();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("blah");
            //TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
            //Debug.Log(TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, MainController.instance.mainCamera));
            //Vector2 blah = Mouse.current.position.ReadValue();//was Input.mousePosition
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textObject, Mouse.current.position.ReadValue(), windowController.panel.Canvas.worldCamera);// MainController.instance.mainCamera);// Input.mousePosition, MainController.instance.mainCamera);
            if (linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = textObject.textInfo.linkInfo[linkIndex];
                //TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
                ConnectionController.instance.SendData("105", $"{linkInfo.GetLinkID()}");
                //Debug.Log(linkInfo.GetLinkID());
            }

        }


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_UpdateDrag = true;
        //Debug.Log("blah");
    }

    public void OnDrag(PointerEventData eventData)
    {
        CaretPosition insertionSide;

        int insertionIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textObject, eventData.position, eventData.pressEventCamera, out insertionSide);

        if (insertionSide == CaretPosition.Left)
        {
            selectedStringEnd = insertionIndex == 0
                ? textObject.textInfo.characterInfo[0].index
                : textObject.textInfo.characterInfo[insertionIndex - 1].index + textObject.textInfo.characterInfo[insertionIndex - 1].stringLength;
            //Debug.Log($"leftlast:{textObject.text[selectedStringEnd - 1]}{textObject.text[selectedStringEnd]}");

            //Debug.Log(textObject.textInfo.characterInfo[insertionIndex - 1].stringLength);
        }
        else if (insertionSide == CaretPosition.Right)
        {
            selectedStringEnd = textObject.textInfo.characterInfo[insertionIndex].index + textObject.textInfo.characterInfo[insertionIndex].stringLength;
            //Debug.Log($"rightlast:{textObject.text[selectedStringEnd - 1]}{textObject.text[selectedStringEnd]}");

            //Debug.Log(textObject.textInfo.characterInfo[insertionIndex].stringLength);
        }

        //Debug.Log($"selected:{selectedStringEnd+1}, length:{textObject.text.Length}");
        //Debug.Log(GetSelectedString());
        highlightStringEnd = GetCaretPositionFromStringIndex(selectedStringEnd);

        MarkGeometryAsDirty();

        m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(textViewport, eventData.position, eventData.pressEventCamera);
        if (m_DragPositionOutOfBounds && m_DragCoroutine == null)
            m_DragCoroutine = StartCoroutine(MouseDragOutsideRect(eventData));

        eventData.Use();

    }

    IEnumerator MouseDragOutsideRect(PointerEventData eventData)
    {
        //Vector3 localPosition = transform.localPosition;
        //Vector3 textComponentLocalPosition = m_TextComponent.rectTransform.localPosition;
        //Vector3 textViewportLocalPosition = m_TextViewport.localPosition;
        //Rect textViewportRect = m_TextViewport.rect;

//        Vector3 parentObjectPos = windowController.transform.localPosition;
//        Vector3 scrollViewPos = windowController.scrollView.transform.localPosition;
//        Vector3 viewportPos = transform.localPosition;
//        Vector3 contentPos = contentRect.transform.localPosition;
        //Rect textViewportRect = textViewport.rect;//textViewport is set during OnEnable, fyi so it's easier to get the rect instead of always GetComponent

//        Vector2 caretPosBase = new Vector2(viewportPosBase.x + contentPos.x, viewportPosBase.y + contentPos.y);
        //viewportWSRect = new Rect(viewportPosBase.x + textViewportRect.x, viewportPosBase.y + textViewportRect.y, textViewportRect.width, textViewportRect.height);

        //Debug.Log("out of bounds?");
        while (m_UpdateDrag && m_DragPositionOutOfBounds)
        {
            //Debug.Log("out of bounds");
            Vector2 localMousePos;
            bool doScroll = false;
            float offset = 0f;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(textViewport, eventData.position, eventData.pressEventCamera, out localMousePos);

            Rect rect = textViewport.rect;
            PointerEventData needScrolling = eventData;
            //Vector2 contentNewPos = contentRect.localPosition;
            //            needScrolling.

            if (localMousePos.y > rect.yMax)
            {
                offset = MoveUp(true, true);
                if (windowController.scrollView.normalizedPosition.y < 1f && offset < 0)//not at top
                {
                    //Debug.Log(windowController.scrollView.normalizedPosition.y);
                    //needScrolling.scrollDelta = new Vector2(0f, -offset);//maybe 1.5?
                    //windowController.scrollView.OnScroll(needScrolling);
                    //contentNewPos = new Vector2(0, contentRect.localPosition.y + offset);
                    doScroll = true;
                }
                
            }
            else if (localMousePos.y < rect.yMin)
            {
                MoveDown(true, true);
                if (windowController.scrollView.normalizedPosition.y > 0f)
                {
                    //Debug.Log(windowController.scrollView.normalizedPosition.y);
                    //needScrolling.scrollDelta = new Vector2(0f, -1.3f);
                    //windowController.scrollView.OnScroll(needScrolling);
                   // doScroll = true;
                }
                
            }
            if (doScroll)
            {
                //Debug.Log("scrolling?");

                //                windowController.scrollView.OnScroll(needScrolling);
                //windowController.scrollView.normalizedPosition = 
                //contentRect.localPosition = new Vector2(0f, contentRect.localPosition.y + offset);
            }

            //adjust scroll normalized position



            UpdateLabel();
            float delay = kVScrollSpeed;

            if (m_WaitForSecondsRealtime == null)
                m_WaitForSecondsRealtime = new WaitForSecondsRealtime(delay);
            else
                m_WaitForSecondsRealtime.waitTime = delay;

            yield return m_WaitForSecondsRealtime;

            //yield return null;
        }
        m_DragCoroutine = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_UpdateDrag = false;
    }

    public virtual void OnScroll(PointerEventData eventData)
    {
        Debug.Log(windowController.scrollView.content.anchoredPosition);
        //Debug.Log(eventData.delta);
        windowController.scrollView.OnScroll(eventData);
        //Debug.Log(windowController.scrollView.)
        //Debug.Log("we should be scrolling");
        //        windowController.scrollView.OnScroll(eventData);
        //scrollView.OnScroll(eventData);
        //Debug.Log(eventData.scrollDelta);
        Debug.Log(windowController.scrollView.content.anchoredPosition);
    }

    protected void ClampStringPos(ref int pos)
    {
        if (pos <= 0)
            pos = 0;
        else if (pos > textObject.text.Length)
            pos = textObject.text.Length;
//        Debug.Log($"pos:{pos},{textObject.text.Length}");
    }

    protected void ClampCaretPos(ref int pos)
    {
        if (pos > textObject.textInfo.characterCount - 1)
            pos = textObject.textInfo.characterCount - 1;

        if (pos <= 0)
            pos = 0;
    }

    private float MoveUp(bool shift, bool goToFirstChar)
    {
        //float offset = 0f;
        if (hasSelection && !shift)
        {
            // If we have a selection and press up without shift,
            // set caret position to start of selection before we move it up.
            highlightStringStart = highlightStringEnd = Mathf.Min(highlightStringStart, highlightStringEnd);
        }

        int position = LineUpCharacterPosition(highlightStringEnd, goToFirstChar);

        if (shift)
        {
            highlightStringEnd = position;
            selectedStringEnd = GetStringIndexFromCaretPosition(highlightStringEnd);
        }
        else
        {
            highlightStringEnd = highlightStringStart = position;
            selectedStringEnd = selectedStringStart = GetStringIndexFromCaretPosition(highlightStringEnd);
        }

        //we now have the caret where we want it?
 /*       Vector2 caretPosition;
        float height = 0;
        if (m_CaretEnd < textObject.textInfo.characterCount)
        {
            caretPosition = new Vector2(textObject.textInfo.characterInfo[m_CaretEnd].origin, textObject.textInfo.characterInfo[m_CaretEnd].descender);
            height = textObject.textInfo.characterInfo[m_CaretEnd].ascender - textObject.textInfo.characterInfo[m_CaretEnd].descender;
        }
        else
        {
            caretPosition = new Vector2(textObject.textInfo.characterInfo[m_CaretEnd - 1].xAdvance, textObject.textInfo.characterInfo[m_CaretEnd - 1].descender);
            height = textObject.textInfo.characterInfo[m_CaretEnd - 1].ascender - textObject.textInfo.characterInfo[m_CaretEnd - 1].descender;
        }

        Vector2 caretPos = new Vector2(caretPosition.x + caretPosBase.x, caretPosition.y + caretPosBase.y);

        float topOffset = viewportWSRect.yMax - (caretPos.y + height);
        //float topOffset = caretPos.y + height;
        //Debug.Log($"topOffset: {topOffset.ToString("f3")}");
        if (topOffset < -0.0001f)
        {
            Debug.Log(topOffset);
            //Vector2 temp = new Vector2(0f, contentRect.localPosition.y + topOffset);
            //Debug.Log($"new pos: {temp}");
            //Debug.Log($"topOffset: {topOffset.ToString("f3")}");
            //Debug.Log("Shifting text to Up " + topOffset.ToString("f3"));
            //  m_TextComponent.rectTransform.anchoredPosition += new Vector2(0, topOffset);
            //AssignPositioningIfNeeded();
            return topOffset;
        }

        float bottomOffset = caretPos.y - viewportWSRect.yMin;
        if (bottomOffset < 0f)
        {
            //Debug.Log("Shifting text to Down " + bottomOffset.ToString("f3"));
         //   m_TextComponent.rectTransform.anchoredPosition -= new Vector2(0, bottomOffset);
         //   AssignPositioningIfNeeded();
         return bottomOffset;
        }*/

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        return 0f;
    }

    private void MoveDown(bool shift, bool goToLastChar)
    {
        if (hasSelection && !shift)
        {
            // If we have a selection and press down without shift,
            // set caret to end of selection before we move it down.
            highlightStringStart = highlightStringEnd = Mathf.Max(highlightStringStart, highlightStringEnd);
        }

        int position = LineDownCharacterPosition(highlightStringEnd, goToLastChar); // text.Length;

        if (shift)
        {
            highlightStringEnd = position;
            selectedStringEnd = GetStringIndexFromCaretPosition(highlightStringEnd);
        }
        else
        {
            highlightStringEnd = highlightStringStart = position;
            selectedStringEnd = selectedStringStart = GetStringIndexFromCaretPosition(highlightStringEnd);
        }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
    }

    private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
    {
        if (originalPos >= textObject.textInfo.characterCount)
            originalPos -= 1;

        TMP_CharacterInfo originChar = textObject.textInfo.characterInfo[originalPos];
        int originLine = originChar.lineNumber;

        // We are on the first line return first character
        if (originLine - 1 < 0)
            return goToFirstChar ? 0 : originalPos;

        int endCharIdx = textObject.textInfo.lineInfo[originLine].firstCharacterIndex - 1;

        int closest = -1;
        float distance = TMP_Math.FLOAT_MAX;
        float range = 0;

        for (int i = textObject.textInfo.lineInfo[originLine - 1].firstCharacterIndex; i < endCharIdx; ++i)
        {
            TMP_CharacterInfo currentChar = textObject.textInfo.characterInfo[i];

            float d = originChar.origin - currentChar.origin;
            float r = d / (currentChar.xAdvance - currentChar.origin);

            if (r >= 0 && r <= 1)
            {
                if (r < 0.5f)
                    return i;
                else
                    return i + 1;
            }

            d = Mathf.Abs(d);

            if (d < distance)
            {
                closest = i;
                distance = d;
                range = r;
            }
        }

        if (closest == -1) return endCharIdx;

        //Debug.Log("Returning nearest character with Range = " + range);

        if (range < 0.5f)
            return closest;
        else
            return closest + 1;
    }

    private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
    {
        //Debug.Log($"check:{originalPos},{textObject.textInfo.characterCount}");
        if (originalPos >= textObject.textInfo.characterCount)
            return textObject.textInfo.characterCount - 1; // text.Length;

        TMP_CharacterInfo originChar = textObject.textInfo.characterInfo[originalPos];
        int originLine = originChar.lineNumber;

        //// We are on the last line return last character
        if (originLine + 1 >= textObject.textInfo.lineCount)
            return goToLastChar ? textObject.textInfo.characterCount - 1 : originalPos;

        // Need to determine end line for next line.
        int endCharIdx = textObject.textInfo.lineInfo[originLine + 1].lastCharacterIndex;

        int closest = -1;
        float distance = TMP_Math.FLOAT_MAX;
        float range = 0;

        for (int i = textObject.textInfo.lineInfo[originLine + 1].firstCharacterIndex; i < endCharIdx; ++i)
        {
            TMP_CharacterInfo currentChar = textObject.textInfo.characterInfo[i];

            float d = originChar.origin - currentChar.origin;
            float r = d / (currentChar.xAdvance - currentChar.origin);

            if (r >= 0 && r <= 1)
            {
                if (r < 0.5f)
                    return i;
                else
                    return i + 1;
            }

            d = Mathf.Abs(d);

            if (d < distance)
            {
                closest = i;
                distance = d;
                range = r;
            }
        }

        if (closest == -1) return endCharIdx;

        //Debug.Log("Returning nearest character with Range = " + range);

        if (range < 0.5f)
            return closest;
        else
            return closest + 1;
    }

    private int GetStringIndexFromCaretPosition(int caretPosition)
    {
        // Clamp values between 0 and character count.
        ClampCaretPos(ref caretPosition);

        return textObject.textInfo.characterInfo[caretPosition].index;
    }

    private int GetCaretPositionFromStringIndex(int stringIndex)
    {
        int count = textObject.textInfo.characterCount;

        for (int i = 0; i < count; i++)
        {
            if (textObject.textInfo.characterInfo[i].index >= stringIndex)
                return i;
        }

        return count;
    }

    private void GenerateHighlight(VertexHelper vbo, Vector2 roundingOffset)
    {
        // Update Masking Region
        //UpdateMaskRegions();

        // Make sure caret position does not exceed characterInfo array size.
        //if (caretSelectPositionInternal >= m_TextComponent.textInfo.characterInfo.Length)
        //    return;

        TMP_TextInfo textInfo = textObject.textInfo;

        // Return if character count is zero as there is nothing to highlight.
        if (textInfo.characterCount == 0)
            return;

        m_CaretStart = GetCaretPositionFromStringIndex(selectedStringStart);
        m_CaretEnd = GetCaretPositionFromStringIndex(selectedStringEnd);


        // Adjust text RectTranform position to make sure it is visible in viewport.
        //do scrollRect OnScroll here?

        Vector2 caretPosition;
        float height = 0;
        if (m_CaretEnd < textObject.textInfo.characterCount)
        {
            caretPosition = new Vector2(textObject.textInfo.characterInfo[m_CaretEnd].origin, textObject.textInfo.characterInfo[m_CaretEnd].descender);
            height = textObject.textInfo.characterInfo[m_CaretEnd].ascender - textObject.textInfo.characterInfo[m_CaretEnd].descender;
        }
        else
        {
            caretPosition = new Vector2(textObject.textInfo.characterInfo[m_CaretEnd - 1].xAdvance, textObject.textInfo.characterInfo[m_CaretEnd - 1].descender);
            height = textObject.textInfo.characterInfo[m_CaretEnd - 1].ascender - textObject.textInfo.characterInfo[m_CaretEnd - 1].descender;
        }

        Vector3 contentPos = windowController.scrollView.content.localPosition;// contentRect.transform.localPosition;
        Vector2 caretPos = new Vector2(caretPosition.x + viewportPosBase.x + contentPos.x, caretPosition.y + viewportPosBase.y + contentPos.y);

        //if ()
        //Debug.Log(viewportWSRect.yMax);

        float topOffset = viewportWSRect.yMax - (caretPos.y + height);
        //float topOffset = textViewport.rect.yMax - (caretPos.y + height);
        //float topOffset = caretPos.y + height;
        //Debug.Log($"topOffset: {topOffset.ToString("f3")}");
        //Debug.Log(topOffset);
        if (topOffset < -0.0001f)// && windowController.scrollView.content.pivot.y == 1f)
        {
            windowController.scrollView.content.anchoredPosition += new Vector2(0, topOffset);
            //contentRect.anchoredPosition += new Vector2(0, topOffset);
            //Debug.Log(topOffset +" "+height);
            //Vector2 temp = new Vector2(0f, contentRect.localPosition.y + topOffset);
            //Debug.Log($"new pos: {temp}");
            //Debug.Log($"topOffset: {topOffset.ToString("f3")}");
            //Debug.Log("Shifting text to Up " + topOffset.ToString("f3"));
            //  m_TextComponent.rectTransform.anchoredPosition += new Vector2(0, topOffset);
            //AssignPositioningIfNeeded();
            //           return topOffset;
            //contentRect.localPosition = new Vector2(0f, contentRect.localPosition.y + topOffset);
            //PointerEventData tempScroll = new();
            //windowController.
        }

        float bottomOffset = caretPos.y - viewportWSRect.yMin;
        //float bottomOffset = caretPos.y - textViewport.rect.yMin;
        //Debug.Log(caretPos.y);
        //Debug.Log(viewportWSRect.yMin);
        //Debug.Log(bottomOffset);
        if (bottomOffset < 0f && windowController.scrollView.content.pivot.y == 1f)
        {
            //had to add the 'windowController.scrollView.content.pivot.y == 1f' check since if we don't and the pivot is 0f, it always thinks it's negative
            //and always tries to adjust the anchoredPosition even though we don't need to and it makes the text/highlight jump

            //Debug.Log("Shifting text to Down " + bottomOffset.ToString("f3"));
            //   m_TextComponent.rectTransform.anchoredPosition -= new Vector2(0, bottomOffset);
            windowController.scrollView.content.anchoredPosition -= new Vector2(0, bottomOffset);
            //contentRect.anchoredPosition -= new Vector2(0, bottomOffset);
            //   AssignPositioningIfNeeded();
            //           return bottomOffset;
        }
        //Debug.Log(windowController.scrollView.content.anchoredPosition);
        int startChar = Mathf.Max(0, m_CaretStart);
        int endChar = Mathf.Max(0, m_CaretEnd);

        // Ensure pos is always less then selPos to make the code simpler
        if (startChar > endChar)
        {
            int temp = startChar;
            startChar = endChar;
            endChar = temp;
        }

        endChar -= 1;

        //Debug.Log("Updating Highlight... Caret Position: " + startChar + " Caret Select POS: " + endChar);


        int currentLineIndex = textInfo.characterInfo[startChar].lineNumber;
        int nextLineStartIdx = textInfo.lineInfo[currentLineIndex].lastCharacterIndex;

        UIVertex vert = UIVertex.simpleVert;
        vert.uv0 = Vector2.zero;
        vert.color = selectionColor;

        int currentChar = startChar;
        while (currentChar <= endChar && currentChar < textInfo.characterCount)
        {
            if (currentChar == nextLineStartIdx || currentChar == endChar)
            {
                TMP_CharacterInfo startCharInfo = textInfo.characterInfo[startChar];
                TMP_CharacterInfo endCharInfo = textInfo.characterInfo[currentChar];

                // Extra check to handle Carriage Return
                if (currentChar > 0 && endCharInfo.character == '\n' && textInfo.characterInfo[currentChar - 1].character == '\r')
                    endCharInfo = textInfo.characterInfo[currentChar - 1];

                Vector2 startPosition = new Vector2(startCharInfo.origin, textInfo.lineInfo[currentLineIndex].ascender);
                Vector2 endPosition = new Vector2(endCharInfo.xAdvance, textInfo.lineInfo[currentLineIndex].descender);

                var startIndex = vbo.currentVertCount;
                vert.position = new Vector3(startPosition.x, endPosition.y, 0.0f);
                vbo.AddVert(vert);

                vert.position = new Vector3(endPosition.x, endPosition.y, 0.0f);
                vbo.AddVert(vert);

                vert.position = new Vector3(endPosition.x, startPosition.y, 0.0f);
                vbo.AddVert(vert);

                vert.position = new Vector3(startPosition.x, startPosition.y, 0.0f);
                vbo.AddVert(vert);

                vbo.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                vbo.AddTriangle(startIndex + 2, startIndex + 3, startIndex + 0);

                startChar = currentChar + 1;
                currentLineIndex++;

                if (currentLineIndex < textInfo.lineCount)
                    nextLineStartIdx = textInfo.lineInfo[currentLineIndex].lastCharacterIndex;
            }
            currentChar++;
        }

        //#if TMP_DEBUG_MODE
        //    Debug.Log("Text selection updated at frame: " + Time.frameCount);
        //#endif
    }

    private void AssignPositioningIfNeeded()
    {
        if (textObject != null && highlightRectTrans != null &&
            (highlightRectTrans.localPosition != textObject.rectTransform.localPosition ||
             highlightRectTrans.localRotation != textObject.rectTransform.localRotation ||
             highlightRectTrans.localScale != textObject.rectTransform.localScale ||
             highlightRectTrans.anchorMin != textObject.rectTransform.anchorMin ||
             highlightRectTrans.anchorMax != textObject.rectTransform.anchorMax ||
             highlightRectTrans.anchoredPosition != textObject.rectTransform.anchoredPosition ||
             highlightRectTrans.sizeDelta != textObject.rectTransform.sizeDelta ||
             highlightRectTrans.pivot != textObject.rectTransform.pivot))
        {
            highlightRectTrans.localPosition = textObject.rectTransform.localPosition;
            highlightRectTrans.localRotation = textObject.rectTransform.localRotation;
            highlightRectTrans.localScale = textObject.rectTransform.localScale;
            highlightRectTrans.anchorMin = textObject.rectTransform.anchorMin;
            highlightRectTrans.anchorMax = textObject.rectTransform.anchorMax;
            highlightRectTrans.anchoredPosition = textObject.rectTransform.anchoredPosition;
            highlightRectTrans.sizeDelta = textObject.rectTransform.sizeDelta;
            highlightRectTrans.pivot = textObject.rectTransform.pivot;
        }
    }

    private void MarkGeometryAsDirty()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying || UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            return;
#endif
        //AssignPositioningIfNeeded();
        CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
    }

    public virtual void Rebuild(CanvasUpdate update)
    {
        switch (update)
        {
            case CanvasUpdate.LatePreRender:
                UpdateGeometry();
                break;
        }
    }

    public virtual void LayoutComplete()
    { }

    public virtual void GraphicUpdateComplete()
    { }

    private void UpdateGeometry()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif

        if (m_HighLightRenderer == null)
            return;

        //AssignPositioningIfNeeded();

        OnFillVBO(mesh);

        m_HighLightRenderer.SetMesh(mesh);
    }

    private void OnFillVBO(Mesh vbo)
    {
        using (var helper = new VertexHelper())
        {
            GenerateHighlight(helper, Vector2.zero);

            helper.FillMesh(vbo);
        }
    }

    protected enum EditState
    {
        Continue,
        Finish
    }

    protected EditState KeyPressed(Event evt)
    {
        var currentEventModifiers = evt.modifiers;
        bool ctrl = (currentEventModifiers & EventModifiers.Control) != 0;
        bool shift = (currentEventModifiers & EventModifiers.Shift) != 0;
        bool alt = (currentEventModifiers & EventModifiers.Alt) != 0;
        bool ctrlOnly = ctrl && !alt && !shift;
        m_LastKeyCode = evt.keyCode;

        switch (evt.keyCode)
        {
            case KeyCode.Home:
                {
//                    MoveToStartOfLine(shift, ctrl);
                    return EditState.Continue;
                }

            case KeyCode.End:
                {
 //                   MoveToEndOfLine(shift, ctrl);
                    return EditState.Continue;
                }

            // Select All
            case KeyCode.A:
                {
                    if (ctrlOnly)
                    {
 //                       SelectAll();
                        return EditState.Continue;
                    }
                    break;
                }

            // Copy
            case KeyCode.C:
                {
                    if (ctrlOnly)
                    {
 //                       if (inputType != InputType.Password)
                            clipboard = GetSelectedString();
 //                       else
 //                           clipboard = "";
                        return EditState.Continue;
                    }
                    break;
                }

            case KeyCode.LeftArrow:
                {
 //                   MoveLeft(shift, ctrl);
                    return EditState.Continue;
                }

            case KeyCode.RightArrow:
                {
 //                   MoveRight(shift, ctrl);
                    return EditState.Continue;
                }

            case KeyCode.UpArrow:
                {
 //                   MoveUp(shift);
                    return EditState.Continue;
                }

            case KeyCode.DownArrow:
                {
 //                   MoveDown(shift);
                    return EditState.Continue;
                }

            case KeyCode.PageUp:
                {
 //                   MovePageUp(shift);
                    return EditState.Continue;
                }

            case KeyCode.PageDown:
                {
 //                   MovePageDown(shift);
                    return EditState.Continue;
                }

            case KeyCode.Escape:
                {
 //                   m_ReleaseSelection = true;
 //                   m_WasCanceled = true;
                    return EditState.Finish;
                }
        }

        char c = evt.character;

        // Don't allow return chars or tabulator key to be entered into single line fields.
 //       if (!multiLine && (c == '\t' || c == '\r' || c == '\n'))
 //           return EditState.Continue;

        // Convert carriage return and end-of-text characters to newline.
        if (c == '\r' || c == 3)
            c = '\n';

        // Convert Shift Enter to Vertical tab
        if (shift && c == '\n')
            c = '\v';

        return EditState.Continue;
    }

    private string GetSelectedString()
    {
        if (!hasSelection)
            return "";

        int startPos = selectedStringStart;
        int endPos = selectedStringEnd;

        // Ensure pos is always less then selPos to make the code simpler
        if (startPos > endPos)
        {
            int temp = startPos;
            startPos = endPos;
            endPos = temp;
        }
//        string blah = textObject.text.Substring(startPos, endPos - startPos);
//        Debug.Log($"blah:{endPos - startPos}, doh:{blah.Length}");
//        Debug.Log(textObject.text.Substring(startPos, endPos - startPos));
        return textObject.text.Substring(startPos, endPos - startPos);
    }

    static string clipboard
    {
        get
        {
            return GUIUtility.systemCopyBuffer;
        }
        set
        {
            GUIUtility.systemCopyBuffer = value;
        }
    }

    protected void UpdateLabel()
    {
        if (textObject != null && textObject.font != null && m_PreventCallback == false)
        {
            m_PreventCallback = true;
            //AssignPositioningIfNeeded();
            textObject.text += "\u200B";//this is what I needed to fix the text selection/highlight issue, you gotta be kidding me right? oi.

            if (m_IsTextComponentUpdateRequired)
            {
                textObject.ForceMeshUpdate();
                //MarkGeometryAsDirty();
                m_IsTextComponentUpdateRequired = false;
            }

            MarkGeometryAsDirty();
            m_PreventCallback = false;
        }
    }
}

static class SetPropertyUtility
{
    public static bool SetColor(ref Color currentValue, Color newValue)
    {
        if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
            return false;

        currentValue = newValue;
        return true;
    }

}

