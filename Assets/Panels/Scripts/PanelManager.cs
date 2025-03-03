using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Panels
{
    public enum Layout { Unrestricted = -1, Right = 0, Down = 1 };
    public enum Direction { None = -1, Left = 0, Top = 1, Right = 2, Bottom = 3, Main = 4 };//which side of the canvas to anchor from?

    public class PanelManager : MonoBehaviour
    {
        public static PanelManager instance;
        public PanelGroup mainGroup = null;

        private List<PanelsCanvas> canvases = new List<PanelsCanvas>(8);

        private PanelsCanvas previewPanelCanvas;
        private RectTransform previewPanel = null;
        private AnchorDropZone hoveredAnchorZone = null;
        private Panel draggedPanel = null;
        //public Camera camera;

        private void Awake()
        {
            instance = this;

            InitializePreviewPanel();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool OnPanelBeginDrag(Panel panel)
        {
            draggedPanel = panel;
            //            currentPanelCanvas = draggedPanel.Canvas;

            previewPanelCanvas = draggedPanel.Canvas;
            if (previewPanel.parent != previewPanelCanvas.RectTransform)
                previewPanel.SetParent(previewPanelCanvas.RectTransform, false);

            previewPanel.gameObject.SetActive(true);
            previewPanel.SetAsLastSibling();

            return true;
        }

        public void OnPanelDrag(Panel panel, PointerEventData draggingPointer)//  PanelHeader panelHeader, PointerEventData draggingPointer)
        {
            //  Vector2 touchPos;
            //Debug.Log(draggedPanel != null);
            //Debug.Log(currentPanelCanvas != null);
            //  RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedPanel.RectTransform, draggingPointer.position, draggedPanel.Canvas.worldCamera, out touchPos);
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedPanel.RectTransform, draggingPointer.position, currentPanelCanvas.Internal.worldCamera, out touchPos);

     //       if (panel.isDockable)// panelHeader.Panel.isDockable)
     //       {
                //create/show panel preview ghost as we're dragging around
                if (hoveredAnchorZone && panel.isDockable)//if we're over an anchorZone
                {
                    if (hoveredAnchorZone.panelGroup.layout == Layout.Down)
                    {
                        previewPanel.pivot = new Vector2(0f, 0.5f);
                        previewPanel.position = hoveredAnchorZone.RectTransform.position;
                        
                    }
                    else if (hoveredAnchorZone.panelGroup.layout == Layout.Right)
                    {
                        previewPanel.pivot = new Vector2(0.5f, 1f);
                        previewPanel.position = hoveredAnchorZone.RectTransform.position;
                    }

                    previewPanel.sizeDelta = new Vector2(hoveredAnchorZone.RectTransform.rect.width, hoveredAnchorZone.RectTransform.rect.height);

                } else
                {
                    Vector2 position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(previewPanelCanvas.RectTransform, draggingPointer.position, previewPanelCanvas.worldCamera, out position);
                    previewPanel.pivot = new Vector2(0f, 1f);
                    previewPanel.localPosition = position - panel.Header.InitialTouchPos;
                    previewPanel.sizeDelta = panel.RectTransform.sizeDelta;// new Vector2(250f, 200f);
                }
     //       } else
     //       {
                //we can't dock this anywhere so we just move it to where we're dragging
//                Vector2 touchPos;
//                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedPanel.RectTransform, draggingPointer.position, draggedPanel.Canvas.worldCamera, out touchPos);
//                draggedPanel.RectTransform.anchoredPosition += touchPos - panel.Header.InitialTouchPos;// panelHeader.InitialTouchPos;
                //Debug.Log($"anchored: {draggedPanel.RectTransform.anchoredPosition}, local: {draggedPanel.RectTransform.localPosition}");
     //       }
        }

        public void OnPanelEndDrag(Panel panel, PointerEventData draggingPointer)
        {
            draggedPanel = null;
            bool isInMain = false;
            PanelGroup groupToAddTo = null;

            if (panel.isDockable)
            {
                if (mainGroup != null)
                {
                    isInMain = RectTransformUtility.RectangleContainsScreenPoint(mainGroup.RectTransform, draggingPointer.position, panel.Canvas.worldCamera);
                    if (isInMain)
                    {
                        groupToAddTo = mainGroup;
                    }
                }

                if (hoveredAnchorZone != null)
                {
                    groupToAddTo = hoveredAnchorZone.panelGroup;
                }
            }

            panel.AddPanelToGroup(groupToAddTo, draggingPointer.position, hoveredAnchorZone);

//            if (panel.isDockable)
//            {
                //figure out if we're ending the drag over the Main, unrestricted group
                //bool isInMain = false;

//                if (hoveredAnchorZone != null)// && panel.Group == null && panel.isDockable)
//                {
                    //currently hovering over a dropZone
//                    panel.AddPanelToGroup(hoveredAnchorZone.panelGroup, hoveredAnchorZone.gameObject.transform.GetSiblingIndex());

                    /*  if (panel.Group != null)
                      {
                          //currently part of a panelGroup
                          //panel.RemovePanelFromGroup();
                          panel.AddPanelToGroup(hoveredAnchorZone.panelGroup);
                      }
                      else if (panel.Group == null && panel.isDockable)
                      {
                          //currently a floater and is dockable so we need to add it to the panelGroup
                          panel.AddPanelToGroup(hoveredAnchorZone.panelGroup);

                      }*/
//                }
//                else
//                {
//                    if (mainGroup != null)
//                    {
//                        isInMain = RectTransformUtility.RectangleContainsScreenPoint(mainGroup.RectTransform, draggingPointer.position, panel.Canvas.worldCamera);
//                        Debug.Log(isInMain);
//                        if (isInMain)
//                        {
//                            panel.AddPanelToGroup(mainGroup, draggingPointer.position);


//                        } else
//                        {
                            //not hovering over a dropZone so we just move it to the spot we dragged it to
//                            panel.AddFloaterPanel(draggingPointer.position);
//                        }
//                    }

                    //not hovering over a dropZone so we just move it to the spot we dragged it to
                    //panel.AddFloaterPanel(draggingPointer.position);

                    /*  if (panel.Group != null)
                      {
                          //currently part of a panelGroup so we need to move it to where we dragged it and make it a floater
                          //panel.RemovePanelFromGroup();
                          panel.AddFloaterPanel(draggingPointer.position);
                      }
                      else if (panel.Group == null && panel.isDockable)
                      {
                          //not part of a panelGroup, but isDockable so we need to move it from where it was
                          //since !isDockable is moving the panel already

                      }*/
//                }
//            } else
//            {


//            }

            hoveredAnchorZone = null;

            previewPanel.gameObject.SetActive(false);
        }

        public void AnchorZonesSetActive(bool value)
        {
          //  for (int i = 0; i < panels.Count; i++)
          //      panels[i].Internal.AnchorZonesSetActive(value);

            for (int i = 0; i < canvases.Count; i++)
            {
                for (int j = 0; j < canvases[i].panelGroups.Count; j++)
                {
                    canvases[i].panelGroups[j].SetAnchorZonesActive(value);
                }
                //canvases[i].Internal.AnchorZonesSetActive(value);
                //canvases[i].Internal.ReceiveRaycasts(value);
            }
        }

        public void StartPreviewHover(AnchorDropZone anchor)
        {
            hoveredAnchorZone = anchor;
        }

        public void StopPreviewHover(AnchorDropZone anchor)
        {
            if (hoveredAnchorZone && hoveredAnchorZone == anchor)
            {
                //if there is a hoveredAnchorZone set and it's the same one as we're not mousing over anymore, clear it
                //if it's not the same one, that means another zone got set as the one we're hovering over before this one got cleared
                hoveredAnchorZone = null;
            }
        }

        public void RegisterCanvas(PanelsCanvas canvas)
        {
            if (!canvases.Contains(canvas))
                canvases.Add(canvas);
        }

        private void InitializePreviewPanel()
        {
            RectTransform previewPanel = Instantiate(Resources.Load<RectTransform>("PanelPreview"));
            previewPanel.gameObject.name = "DraggedPanelPreview";

            previewPanel.anchorMin = new Vector2(0.5f, 0.5f);
            previewPanel.anchorMax = new Vector2(0.5f, 0.5f);
            previewPanel.pivot = new Vector2(0.5f, 0.5f);

            previewPanel.gameObject.SetActive(false);

            this.previewPanel = previewPanel;

            previewPanel.SetParent(this.transform, false);
        }

    }
}
