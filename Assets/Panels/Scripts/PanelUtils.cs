using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public static class PanelUtils// : MonoBehaviour
    {
        public static Action<PanelGroup, Direction> GroupResize;

        public static PanelGroup AddPanelGroup(PanelsCanvas canvas, Direction position, Layout layout)
        {
            return AddPanelGroup(canvas, position, layout, 30f, 30f);
        }

        public static PanelGroup AddPanelGroup(PanelsCanvas canvas, Direction position, Layout layout, float minX, float minY)
        {
            HorizontalOrVerticalLayoutGroup autoLayout = null;
            PanelGroup group = null;
            PanelGroup topGroup = AnyGroupsInDirection(canvas, Direction.Top);// null;
            PanelGroup bottomGroup = AnyGroupsInDirection(canvas, Direction.Bottom);// null;
            PanelGroup leftGroup = AnyGroupsInDirection(canvas, Direction.Left);// null;
            PanelGroup rightGroup = AnyGroupsInDirection(canvas, Direction.Right);// null;

            //result = (Panel)Object.Instantiate(Resources.Load<Panel>("DynamicPanel"), canvas.RectTransform, false);
            group = UnityEngine.Object.Instantiate(Resources.Load<PanelGroup>("PanelGroup"), canvas.panelGroupsRect, false);

            group.Canvas = canvas;
         //   if (canvas != null)
         //   {
         //       group.isDocked = true;
         //   }
            group.layout = layout;

            group.baseMinSize = new Vector2(minX, minY);
            //group.SetMinSize();
            //group.baseMinSize = group.MinSize = new Vector2(minX, minY);
            //RectTransform rect = group.GetComponent<RectTransform>();
            switch (position)
            {
                case Direction.Top:
                    int numOfTop = 0;
                    for (int i = 0; i < canvas.panelGroups.Count; i++)
                    {
                        if (canvas.panelGroups[i].position == Direction.Top)
                            numOfTop++;
                    }
                    group.position = position;
                    autoLayout = group.panelObjects.gameObject.AddComponent<HorizontalLayoutGroup>();
                    //HorizontalLayoutGroup topLayoutGroup = group.panelObjects.gameObject.AddComponent<HorizontalLayoutGroup>();

                    //any groups at the top already?
                    if (topGroup != null)
                    {
                        group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, topGroup.RectTransform.anchoredPosition.y - minY);// - topGroup.RectTransform.sizeDelta.y);
                        //group.name = "Top Panel Group #?";
                    } else
                    {
                        group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, canvas.RectTransform.sizeDelta.y - minY);
                        group.GetResizeZone(Direction.Top).gameObject.SetActive(false);
                        //group.name = "Top Panel Group #1";
                        //group.RectTransform.anchoredPosition = Vector2.zero;
                    }
                    group.name = $"Top Panel Group #{numOfTop + 1}";
                    //any groups to the left?
                    if (leftGroup != null)
                    {
                        group.RectTransform.anchoredPosition = new Vector2(leftGroup.RectTransform.anchoredPosition.x + leftGroup.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y);
                    } else
                    {
                        group.GetResizeZone(Direction.Left).gameObject.SetActive(false);
                    }
                    //any groups to the right?
                    if (rightGroup != null)
                    {//group.RectTransform.sizeDelta is now group.SizeDelta
                        //float topTemp = -rightGroup.RectTransform.anchoredPosition.x + rightGroup.RectTransform.sizeDelta.x;//anchoredPosition.x is negative so we need to add it
                        group.SizeDelta = new Vector2(rightGroup.RectTransform.anchoredPosition.x - group.RectTransform.anchoredPosition.x, minY);// - topTemp, group.RectTransform.sizeDelta.y);
                        //group.RectTransform.sizeDelta = new Vector2(group.RectTransform.anchoredPosition.x + topTemp - canvas.RectTransform.sizeDelta.x, group.RectTransform.sizeDelta.y);
                    } else
                    {
                        group.SizeDelta = new Vector2(canvas.RectTransform.sizeDelta.x - group.RectTransform.anchoredPosition.x, minY);
                        group.GetResizeZone(Direction.Right).gameObject.SetActive(false);
                    }
                    break;
                case Direction.Bottom:
                    int numOfBottom = 0;
                    for (int i = 0; i < canvas.panelGroups.Count; i++)
                    {
                        if (canvas.panelGroups[i].position == Direction.Bottom)
                            numOfBottom++;
                    }
                    group.position = position;
                    autoLayout = group.panelObjects.gameObject.AddComponent<HorizontalLayoutGroup>();
                    //any groups to the bottom already?
                    if (bottomGroup != null)
                    {
                        group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, bottomGroup.RectTransform.anchoredPosition.y + bottomGroup.RectTransform.sizeDelta.y);
                        //group.name = "Bottom Panel Group #?";
                    }
                    else
                    {
                        group.RectTransform.anchoredPosition = Vector2.zero;
                        group.GetResizeZone(Direction.Bottom).gameObject.SetActive(false);
                        //group.name = "Bottom Panel Group #1";
                        //group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, -canvas.RectTransform.sizeDelta.y + 30f);// Vector2.zero;
                    }
                    group.name = $"Bottom Panel Group #{numOfBottom + 1}";
                    //any groups to the left?
                    if (leftGroup != null)
                    {
                        group.RectTransform.anchoredPosition = new Vector2(leftGroup.RectTransform.anchoredPosition.x + leftGroup.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y);
                    } else
                    {
                        group.GetResizeZone(Direction.Left).gameObject.SetActive(false);
                    }
                    //any groups to the right?
                    if (rightGroup != null)
                    {
                        //float bottomTemp = -rightGroup.RectTransform.anchoredPosition.x + rightGroup.RectTransform.sizeDelta.x;
                        //group.RectTransform.sizeDelta = new Vector2(canvas.RectTransform.sizeDelta.x - group.RectTransform.anchoredPosition.x - bottomTemp, group.RectTransform.sizeDelta.y);
                        group.SizeDelta = new Vector2(rightGroup.RectTransform.anchoredPosition.x - group.RectTransform.anchoredPosition.x, minY);
                        //group.RectTransform.sizeDelta = new Vector2(group.RectTransform.anchoredPosition.x - canvas.RectTransform.sizeDelta.x, group.RectTransform.sizeDelta.y);
                    } else
                    {
                        group.SizeDelta = new Vector2(canvas.RectTransform.sizeDelta.x - group.RectTransform.anchoredPosition.x, minY);
                        group.GetResizeZone(Direction.Right).gameObject.SetActive(false);
                    }
                    break;
                case Direction.Left:
                    int numOfLeft = 0;
                    for (int i = 0; i < canvas.panelGroups.Count; i++)
                    {
                        if (canvas.panelGroups[i].position == Direction.Left)
                            numOfLeft++;
                    }
                    group.position = position;
                    autoLayout = group.panelObjects.gameObject.AddComponent<VerticalLayoutGroup>();
                    //any groups to the left already?
                    if (leftGroup != null)
                    {
                        group.RectTransform.anchoredPosition = new Vector2(leftGroup.RectTransform.anchoredPosition.x + leftGroup.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y);
                        //group.name = "Left Panel Group #?";
                    }
                    else
                    {
                        group.RectTransform.anchoredPosition = Vector2.zero;
                        group.GetResizeZone(Direction.Left).gameObject.SetActive(false);
                        //group.name = "Left Panel Group #1";
                    }
                    group.name = $"Left Panel Group #{numOfLeft + 1}";
                    //any groups to the bottom?
                    if (bottomGroup != null)
                    {
                        group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, bottomGroup.RectTransform.anchoredPosition.y + bottomGroup.RectTransform.sizeDelta.y);
                    }
                    else
                    {
                        group.RectTransform.anchoredPosition = Vector2.zero;
                        group.GetResizeZone(Direction.Bottom).gameObject.SetActive(false);
                        //group.RectTransform.sizeDelta = new Vector2(30f, canvas.RectTransform.sizeDelta.y - group.RectTransform.anchoredPosition.y);
                    }
                    //any groups to the top?
                    if (topGroup != null)
                    {
                        group.SizeDelta = new Vector2(minX, topGroup.RectTransform.anchoredPosition.y - group.RectTransform.anchoredPosition.y);
                        //group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, topGroup.RectTransform.anchoredPosition.y - topGroup.RectTransform.sizeDelta.y);
                        //group.RectTransform.anchoredPosition = new Vector2(topGroup.RectTransform.anchoredPosition.x + topGroup.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y);
                    } else
                    {
                        group.SizeDelta = new Vector2(minX, canvas.RectTransform.sizeDelta.y - group.RectTransform.anchoredPosition.y);
                        group.GetResizeZone(Direction.Top).gameObject.SetActive(false);
                    }
                    break;
                case Direction.Right:
                    int numOfRight = 0;
                    for (int i = 0; i < canvas.panelGroups.Count; i++)
                    {
                        if (canvas.panelGroups[i].position == Direction.Right)
                            numOfRight++;
                    }
                    group.position = position;
                    autoLayout = group.panelObjects.gameObject.AddComponent<VerticalLayoutGroup>();
                    //any groups to the right already?
                    if (rightGroup != null)
                    {
                        //group.RectTransform.anchoredPosition = new Vector2()
                        group.RectTransform.anchoredPosition = new Vector2(rightGroup.RectTransform.anchoredPosition.x - rightGroup.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y);
                        //group.name = "Right Panel Group #?";
                    }
                    else
                    {
                        group.RectTransform.anchoredPosition = new Vector2(canvas.RectTransform.sizeDelta.x - minX, 0f);
                        group.GetResizeZone(Direction.Right).gameObject.SetActive(false);
                        //group.name = "Right Panel Group #1";
                        //group.RectTransform.anchoredPosition = Vector2.zero;
                    }
                    group.name = $"Right Panel Group #{numOfRight + 1}";
                    //any groups to the bottom?
                    if (bottomGroup != null)
                    {
                        //float rightTemp = bottomGroup.RectTransform.anchoredPosition.y + bottomGroup.RectTransform.sizeDelta.y;
                        //group.RectTransform.sizeDelta = new Vector2(group.RectTransform.sizeDelta.x, canvas.RectTransform.sizeDelta.y + group.RectTransform.anchoredPosition.y - rightTemp);
                        //group.RectTransform.sizeDelta = new Vector2(30f, -bottomGroup.RectTransform.anchoredPosition.y + group.RectTransform.anchoredPosition.y);
                        //group.RectTransform.sizeDelta = new Vector2(group.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y - canvas.RectTransform.sizeDelta.y);
                        group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, bottomGroup.RectTransform.anchoredPosition.y + bottomGroup.RectTransform.sizeDelta.y);
                    }
                    else
                    {
                        //group.RectTransform.sizeDelta = new Vector2(30f, canvas.RectTransform.sizeDelta.y - group.RectTransform.anchoredPosition.y);
                        group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, 0f);
                        group.GetResizeZone(Direction.Bottom).gameObject.SetActive(false);
                    }
                    //any groups to the top?
                    if (topGroup != null)
                    {
                        group.SizeDelta = new Vector2(minX, topGroup.RectTransform.anchoredPosition.y - group.RectTransform.anchoredPosition.y);
                        //group.RectTransform.anchoredPosition = new Vector2(group.RectTransform.anchoredPosition.x, topGroup.RectTransform.anchoredPosition.y - topGroup.RectTransform.sizeDelta.y);
                        //group.RectTransform.anchoredPosition = new Vector2(topGroup.RectTransform.anchoredPosition.x + topGroup.RectTransform.sizeDelta.x, group.RectTransform.anchoredPosition.y);
                    } else
                    {
                        group.SizeDelta = new Vector2(minX, canvas.RectTransform.sizeDelta.y - group.RectTransform.anchoredPosition.y);
                        group.GetResizeZone(Direction.Top).gameObject.SetActive(false);
                    }
                    break;
                case Direction.Main://unrestricted panel group in middle of canvas?
                    //do we already have one?
                    if (AnyGroupsInDirection(canvas, Direction.Main) != null)
                        return null;

                    group.position = position;
                    group.name = "Main Panel Group";
                    PanelManager.instance.mainGroup = group;
                    //when this panelGroup is first created, set all the resize zones to inactive
                    //so when a panelGroup is added to any side (before or after), that zone gets set active at that point so we don't have to keep setting the zone
                    //inactive/active every time
                    for (int i = 0; i < group.resizeZones.Count; i++)
                    {
                        group.resizeZones[i].gameObject.SetActive(false);
                    }
                    break;
            }

            canvas.panelGroups.Add(group);

            AdjustMainPanelGroup(canvas, AnyGroupsInDirection(canvas, Direction.Main));

            CheckForGroupNeighbors(canvas);

            if (autoLayout != null)
            {
                autoLayout.childControlHeight = false;
                autoLayout.childControlWidth = false;
                autoLayout.childForceExpandHeight = false;
                autoLayout.childForceExpandWidth = false;
                autoLayout.childScaleHeight = false;
                autoLayout.childScaleWidth = false;
                autoLayout.spacing = 0;
            }
            group.SetMinSize();

            if (group.layout == Layout.Right || group.layout == Layout.Down)
            {
                AnchorDropZone newDrop = UnityEngine.Object.Instantiate(Resources.Load<AnchorDropZone>("AnchorDropZone"), group.anchorObjects, false);
                group.AddAnchorZoneToGroup(newDrop);
                newDrop.SetAnchor(group, 12f);
                if (group.layout == Layout.Down)
                {
                    newDrop.RectTransform.localPosition = new Vector2(0f, -6f);
                    group.scrollRect.horizontal = false;
                    group.scrollRect.horizontalScrollbarSpacing = 0;
                    //group.scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                    //Debug.Log(group.scrollRect.verticalScrollbar.GetComponent<RectTransform>().sizeDelta.y);
             //       group.RemoveScrollbar("horizontal");
                    //group.scrollRect.horizontalScrollbar.gameObject
                    //group.scrollRect.horizontalScrollbar = null;
                }
                else// if (group.layout == Layout.Right)
                {
                    newDrop.RectTransform.localPosition = new Vector2(6f, 0f);
                    group.scrollRect.vertical = false;
                    group.scrollRect.verticalScrollbarSpacing = 0;
                    //group.scrollRect.hor
             //       group.RemoveScrollbar("vertical");
                    //group.scrollRect.verticalScrollbar = null;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(group.RectTransform);
            group.CheckGroupDropAnchors();

            return group;
        }

        public static bool rectOverlaps(this PanelGroup group1, PanelGroup group2)
        {
            float rectX = group1.RectTransform.localPosition.x + group1.resizeZoneRect.offsetMin.x;
            float rectY = group1.RectTransform.localPosition.y + group1.resizeZoneRect.offsetMin.y;
            float sizeX = group1.RectTransform.sizeDelta.x + group1.resizeZoneRect.sizeDelta.x;
            float sizeY = group1.RectTransform.sizeDelta.y + group1.resizeZoneRect.sizeDelta.y;

            Rect rect1 = new Rect(rectX, rectY, sizeX, sizeY);
            Rect rect2 = new Rect(group2.RectTransform.localPosition.x, group2.RectTransform.localPosition.y, group2.RectTransform.rect.width, group2.RectTransform.rect.height);

            return rect1.Overlaps(rect2, true);
        }

        public static void CheckForGroupNeighbors(PanelsCanvas canvas)
        {
            //go through all the groups and redo any new neighbors
            for (int i = 0; i < canvas.panelGroups.Count; i++)
            {
                PanelGroup currentGroup = canvas.panelGroups[i];//the group we're currently focused on to see if there's any groups around it

                currentGroup.groupsOnTop.Clear();
                currentGroup.groupsOnBottom.Clear();
                currentGroup.groupsOnLeft.Clear();
                currentGroup.groupsOnRight.Clear();

                for (int j = 0; j < canvas.panelGroups.Count; j++)
                {
                    PanelGroup checkGroup = canvas.panelGroups[j];//the group we're seeing if it's next to the currentGroup

                    if (currentGroup != checkGroup && currentGroup.rectOverlaps(checkGroup))
                    {
                        if (checkGroup.RectTransform.localPosition.y >= (currentGroup.RectTransform.localPosition.y + currentGroup.RectTransform.sizeDelta.y))
                        {
                            currentGroup.groupsOnTop.Add(checkGroup);
                        }
                        else if ((checkGroup.RectTransform.localPosition.x + checkGroup.RectTransform.sizeDelta.x) <= currentGroup.RectTransform.localPosition.x)
                        {
                            currentGroup.groupsOnLeft.Add(checkGroup);
                        }
                        else if (checkGroup.RectTransform.localPosition.x >= (currentGroup.RectTransform.localPosition.x + currentGroup.RectTransform.sizeDelta.x))
                        {
                            currentGroup.groupsOnRight.Add(checkGroup);
                        }
                        else if (checkGroup.RectTransform.localPosition.y < currentGroup.RectTransform.localPosition.y)
                        {
                            currentGroup.groupsOnBottom.Add(checkGroup);
                        } else
                        {
                            //shouldn't be here as all the groups should have been set
                            Debug.Log("Something isn't right with the sorting of panelgroups around other groups.");
                        }
                    }
                }
            }
        }

        public static void AdjustMainPanelGroup(PanelsCanvas canvas, PanelGroup mainPanel)
        {
            if (mainPanel == null) return;

            float anchorX = 0f;
            float anchorY = 0f;
            float sizeX = 0f;
            float sizeY = 0f;
            //get all/any groups on all sides of center
            PanelGroup lastOnTop = AnyGroupsInDirection(canvas, Direction.Top);
            PanelGroup lastOnBottom = AnyGroupsInDirection(canvas, Direction.Bottom);
            PanelGroup lastOnLeft = AnyGroupsInDirection(canvas, Direction.Left);
            PanelGroup lastOnRight = AnyGroupsInDirection(canvas, Direction.Right);
            mainPanel.groupsOnTop.Clear();
            mainPanel.groupsOnBottom.Clear();
            mainPanel.groupsOnLeft.Clear();
            mainPanel.groupsOnRight.Clear();

            if (lastOnLeft != null)
            {
                anchorX = lastOnLeft.RectTransform.anchoredPosition.x + lastOnLeft.RectTransform.sizeDelta.x;
                mainPanel.GetResizeZone(Direction.Left).gameObject.SetActive(true);
            }
            if (lastOnBottom != null)
            {
                anchorY = lastOnBottom.RectTransform.anchoredPosition.y + lastOnBottom.RectTransform.sizeDelta.y;
                mainPanel.GetResizeZone(Direction.Bottom).gameObject.SetActive(true);
            }
            if (lastOnTop != null)
            {
                sizeY = lastOnTop.RectTransform.anchoredPosition.y - anchorY;
                mainPanel.GetResizeZone(Direction.Top).gameObject.SetActive(true);
            } else
            {
                sizeY = canvas.RectTransform.sizeDelta.y - anchorY;
            }
            if (lastOnRight != null)
            {
                sizeX = lastOnRight.RectTransform.anchoredPosition.x - anchorX;
                mainPanel.GetResizeZone(Direction.Right).gameObject.SetActive(true);
                //mainPanel.resizeZoneRight.gameObject.SetActive(true);
                //sizeX = lastOnRight.RectTransform.anchoredPosition.x - anchorX;// canvas.RectTransform.sizeDelta.x - anchorX + lastOnRight.RectTransform.anchoredPosition.x - lastOnRight.RectTransform.sizeDelta.x;
            } else
            {
                sizeX = canvas.RectTransform.sizeDelta.x - anchorX;
            }

            mainPanel.RectTransform.anchoredPosition = new Vector2(anchorX, anchorY);
            mainPanel.SizeDelta = new Vector2(sizeX, sizeY);
            mainPanel.UpdateContentRect();
        }

    //    public static Panel NewPanel(this PanelsCanvas canvas, PanelGroup group)
    //    {
    //        return canvas.NewPanel(group, 50f, 50f);
    //    }



        public static Panel NewPanel(this PanelsCanvas canvas)//, PanelGroup group)//, float minX, float minY)
        {
            Panel newPanel = null;
            if (canvas == null)
                return newPanel;

            //default min sizes for a new panel
            float minX = 50f;
            float minY = 50f;

            newPanel = UnityEngine.Object.Instantiate(Resources.Load<Panel>("OutputPanel"));
            newPanel.Canvas = canvas;
            newPanel.MinSize = new Vector2(minX, minY);
            newPanel.RectTransform.sizeDelta = new Vector2(minX, minY);

            //start this panel as not in a panelGroup, just in case
            newPanel.AddPanelToGroup(null);

            return newPanel;
        }

        public static void SetContentInPanel(this Panel panel, string contentType)
        {
            switch (contentType)
            {
                case "text":
                    TextWindowController output = UnityEngine.Object.Instantiate(Resources.Load<TextWindowController>("TextWindowOutput"), panel.content, false);
                    panel.MinSize = new Vector2(100f, 80f);
                    panel.text = output;
                    output.panel = panel;
                    //output.gameObject
                    break;


            }

        //    if (panel.Group != null)
        //    {
         //       panel.Group.SetMinSize();
         //   }
            //CheckGroupMinSize(panel.Group);

        }

        public static void AddPanelToGroup(this Panel panel, PanelGroup group = null, Vector2? addPos = null, AnchorDropZone dropZone = null)
        {
            if (addPos == null)
                addPos = Vector2.zero;

            if (group != null)
            {//being added to a group
                panel.RectTransform.SetParent(group.panelObjects, false);
            }
            else
            {//being added to no group/floater
                panel.RectTransform.SetParent(panel.Canvas.RectTransform, false);
            }

            if (panel.Group != null && panel.Group != group)
            {
                //if this panel is already in a group, and we're not moving it to the same group
                panel.Group.dockedPanels.Remove(panel);
                panel.Group.SetMinSize();
                LayoutRebuilder.ForceRebuildLayoutImmediate(panel.Group.RectTransform);
                panel.Group.CheckGroupDropAnchors();
                panel.Group.UpdateContentRect();
            }

            for (int i = 0; i < panel.resizeZones.Count; i++)
            {
                if (group == null
                    || (group.layout == Layout.Down && panel.resizeZones[i].direction == Direction.Bottom)
                    || (group.layout == Layout.Right && panel.resizeZones[i].direction == Direction.Right)
                    || group.layout == Layout.Unrestricted)
                {
                    panel.resizeZones[i].gameObject.SetActive(true);
                }
                else
                {
                    panel.resizeZones[i].gameObject.SetActive(false);
                }
            }

            if (group != null && (group.layout == Layout.Right || group.layout == Layout.Down))
            {
                //we dragged to a sidebar group, not the main
                if (dropZone != null)
                {
                    //we dragged to a dropZone, so get index of the dropZone
                    int childIndex = dropZone.gameObject.transform.GetSiblingIndex();

                    //panel is in a group already
                    if (panel.Group != null)
                    {
                        if (panel.Group == group)
                        {
                            //if we're just 'moving' this panel to a different spot in the same panelGroup
                            int currentIndex = group.dockedPanels.IndexOf(panel);//  panel.gameObject.transform.GetSiblingIndex();

                            if (childIndex < currentIndex)
                            {
                                group.dockedPanels.RemoveAt(currentIndex);
                                group.dockedPanels.Insert(childIndex, panel);
                                //panel.gameObject.transform.SetSiblingIndex(childIndex);
                            }
                            else if (childIndex > currentIndex + 1)//childIndex = current, and current + 1 would both put object back in the same spot
                            {
                                group.dockedPanels.RemoveAt(currentIndex);
                                group.dockedPanels.Insert(childIndex -1, panel);
                                //panel.gameObject.transform.SetSiblingIndex(childIndex - 1);
                            }
                        }
                        else
                        {
                            //panel being added from another group
                            //panel.gameObject.transform.SetSiblingIndex(childIndex);
                            group.dockedPanels.Insert(childIndex, panel);
                            panel.Group = group;
                        }
                    }
                    else
                    {
                        //was a floater, now docking to panelGroup
                        //panel.gameObject.transform.SetSiblingIndex(childIndex);
                        group.dockedPanels.Insert(childIndex, panel);
                        panel.Group = group;
                    }
                } else
                {
                    //we straight added this panel to the group instead of dropZone so add it to the end
                    group.dockedPanels.Add(panel);
                    panel.Group = group;
                }
                group.SetMinSize();
                LayoutRebuilder.ForceRebuildLayoutImmediate(group.RectTransform);
                group.CheckGroupDropAnchors();
                group.UpdateContentRect();
             //   group.UpdatePanelGroupPanels(group.RectTransform.sizeDelta);
            }
            else if (group != null)
            {
                //we dragged to the main group
                if (panel.Group != group)
                {
                    //we're adding the panel to the main group and we weren't already in it
                    group.dockedPanels.Add(panel);
                    panel.Group = group;
                }
                panel.RectTransform.SetAsLastSibling();
                //group.dockedPanels.Add(panel);
                //panel.Group = group;
                if (addPos == Vector2.zero)
                {
                    panel.RectTransform.anchoredPosition = (Vector2)addPos;
                } else
                {
                    Vector2 position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(group.panelObjects.GetComponent<RectTransform>(), (Vector2)addPos, panel.Canvas.worldCamera, out position);
                    //Debug.Log(position);
                    panel.RectTransform.localPosition = position - panel.Header.InitialTouchPos;
                    //Debug.Log(panel.RectTransform.localPosition);
                }
                group.SetMinSize();
                LayoutRebuilder.ForceRebuildLayoutImmediate(group.RectTransform);
                group.CheckGroupDropAnchors();
                group.UpdateContentRect();
            } else
            {
                //we dragged to no group, ie: floater panel
                panel.RectTransform.SetAsLastSibling();
                panel.Group = null;
                if (addPos == Vector2.zero)
                {
                    float ranX = UnityEngine.Random.Range(-200f, 200f);
                    float ranY = UnityEngine.Random.Range(-150f, 150f);
                    panel.RectTransform.anchoredPosition = new Vector2((panel.Canvas.RectTransform.sizeDelta.x / 2) + ranX, (panel.Canvas.RectTransform.sizeDelta.y / -3) + ranY);
                } else
                {
                    Vector2 position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.Canvas.RectTransform, (Vector2)addPos, panel.Canvas.worldCamera, out position);
                    panel.RectTransform.localPosition = position - panel.Header.InitialTouchPos;
                }
            }


 /*           if (group != null)
            {
                if (panel.Group != group)
                {
                    //if the panel is being moved from one group to another
                    panel.RemovePanelFromGroup(panel.Group);
                }

                //adding new panel to panelGroup
                //newPanel = UnityEngine.Object.Instantiate(Resources.Load<Panel>("OutputPanel"), group.panelObjects, false);
                for (int i = 0; i < panel.resizeZones.Count; i++)
                {
                    if ((group.layout == Layout.Down && panel.resizeZones[i].direction == Direction.Bottom)
                        || (group.layout == Layout.Right && panel.resizeZones[i].direction == Direction.Right)
                        || group.layout == Layout.Unrestricted)
                    {
                        panel.resizeZones[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        panel.resizeZones[i].gameObject.SetActive(false);
                    }
                }
                group.dockedPanels.Add(panel);
                panel.Group = group;
                //panel.RectTransform.SetParent(group.panelObjects, false);
                if (addPos == Vector2.zero)
                {
                    panel.RectTransform.SetParent(group.panelObjects, false);
                    panel.RectTransform.anchoredPosition = (Vector2)addPos;
                } else
                {
                    Vector2 position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.Canvas.RectTransform, (Vector2)addPos, panel.Canvas.worldCamera, out position);
                    //Debug.Log($"addPos: {addPos}, pos: {position}, touch: {panel.Header.InitialTouchPos}");
                    //previewPanel.pivot = new Vector2(0f, 1f);
                    panel.RectTransform.localPosition = position - panel.Header.InitialTouchPos;
                    panel.RectTransform.SetParent(group.panelObjects, true);
                }

                group.SetMinSize();
                //CheckGroupMinSize(group);
                LayoutRebuilder.ForceRebuildLayoutImmediate(group.RectTransform);
                group.CheckGroupDropAnchors();
            }
            else
            {*/
                //adding new panel as undocked floater
//                panel.AddFloaterPanel();

           /*     panel.RectTransform.SetParent(panel.Canvas.RectTransform, false);
                panel.RectTransform.SetAsLastSibling();

                //set new anchoredPosition in canvas, so it doesn't default to upper left corner
                float ranX = UnityEngine.Random.Range(-200f, 200f);
                float ranY = UnityEngine.Random.Range(-150f, 150f);
                panel.RectTransform.anchoredPosition = new Vector2((panel.Canvas.RectTransform.sizeDelta.x / 2)+ranX, (panel.Canvas.RectTransform.sizeDelta.y / -3)+ranY);

                for (int i = 0; i < panel.resizeZones.Count; i++)
                {
                    panel.resizeZones[i].gameObject.SetActive(true);
                }*/
                //panel.RectTransform.sizeDelta = new Vector2(minX, minY);
//            }
        }

        public static void AddPanelToGroup(this Panel panel, PanelGroup group, int childIndex)//childIndex is the index that we are moving object to
        {
            //adding a panel to a panelGroup at a specific child index
            if (group != null)
            {
                //if this panel is in a group already
                if (panel.Group != null)
                {
                    if (panel.Group == group)
                    {
                        //if we're just 'moving' this panel to a different spot in the same panelGroup
                        int currentIndex = panel.gameObject.transform.GetSiblingIndex();

                        if (childIndex < currentIndex)
                        {
                            panel.gameObject.transform.SetSiblingIndex(childIndex);
                        }
                        else if (childIndex > currentIndex + 1)//childIndex = current, and current + 1 would both put object back in the same spot
                        {
                            panel.gameObject.transform.SetSiblingIndex(childIndex - 1);
                        }
                    } else
                    {
                        PanelGroup tempGroup = panel.Group;
                        panel.AddPanelToGroup(group);
                        panel.gameObject.transform.SetSiblingIndex(childIndex);
                        //panel.RemovePanelFromGroup(tempGroup);

                    }
                } else
                {
                    //was a floater, now docking to panelGroup
                    panel.AddPanelToGroup(group);
                    panel.gameObject.transform.SetSiblingIndex(childIndex);
                }
            }
        }

        /// <summary>
        /// Set the local position of a panel inside the main panelGroup
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="newPos"></param>
        public static void SetPanelPos(this Panel panel, Vector2 newPos)
        {
            //we need to adjust y values so they go the right direction
            //if it's positive, adjust to negative
            panel.RectTransform.localPosition = new Vector2(newPos.x, -newPos.y);
            panel.Group.UpdateContentRect();
        }

        public static void AddFloaterPanel(this Panel panel, Vector2? addPos = null)
        {
            //this is for making a panel a floater
 /*           if (group != null)
            {
                //adding new panel to panelGroup
                //newPanel = UnityEngine.Object.Instantiate(Resources.Load<Panel>("OutputPanel"), group.panelObjects, false);
                for (int i = 0; i < panel.resizeZones.Count; i++)
                {
                    if ((group.layout == Layout.Down && panel.resizeZones[i].direction == Direction.Bottom)
                        || (group.layout == Layout.Right && panel.resizeZones[i].direction == Direction.Right)
                        || group.layout == Layout.Unrestricted)
                    {
                        panel.resizeZones[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        panel.resizeZones[i].gameObject.SetActive(false);
                    }
                }
                group.dockedPanels.Add(panel);
                panel.Group = group;
                panel.RectTransform.SetParent(group.panelObjects, false);
                panel.RectTransform.anchoredPosition = Vector2.zero;

                group.SetMinSize();
                //CheckGroupMinSize(group);
                LayoutRebuilder.ForceRebuildLayoutImmediate(group.RectTransform);
                group.CheckGroupDropAnchors();
            }
            else
            {*/


                //adding new panel as undocked floater
                panel.RectTransform.SetParent(panel.Canvas.RectTransform, false);
                panel.RectTransform.SetAsLastSibling();

            if (addPos == null)
            {
                float ranX = UnityEngine.Random.Range(-200f, 200f);
                float ranY = UnityEngine.Random.Range(-150f, 150f);
                //addPos = new Vector2((panel.Canvas.RectTransform.sizeDelta.x / 2) + ranX, (panel.Canvas.RectTransform.sizeDelta.y / -3) + ranY);
                panel.RectTransform.anchoredPosition = new Vector2((panel.Canvas.RectTransform.sizeDelta.x / 2) + ranX, (panel.Canvas.RectTransform.sizeDelta.y / -3) + ranY);
            } else
            {
                Vector2 position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.Canvas.RectTransform, (Vector2)addPos, panel.Canvas.worldCamera, out position);
                //previewPanel.pivot = new Vector2(0f, 1f);
                panel.RectTransform.localPosition = position - panel.Header.InitialTouchPos;

                //panel.RectTransform.localPosition = (Vector2)addPos;// + panel.Header.InitialTouchPos;
                //Debug.Log(addPos);
            }

            //set new anchoredPosition in canvas, so it doesn't default to upper left corner
            //float ranX = UnityEngine.Random.Range(-200f, 200f);
            //float ranY = UnityEngine.Random.Range(-150f, 150f);
            //panel.RectTransform.anchoredPosition = (Vector2)addPos;// new Vector2((panel.Canvas.RectTransform.sizeDelta.x / 2)+ranX, (panel.Canvas.RectTransform.sizeDelta.y / -3)+ranY);

                for (int i = 0; i < panel.resizeZones.Count; i++)
                {
                    panel.resizeZones[i].gameObject.SetActive(true);
                }
            //panel.RectTransform.sizeDelta = new Vector2(minX, minY);
            //   }

            if (panel.Group != null)
            {
               // panel.RemovePanelFromGroup(panel.Group);
            }
            panel.Group = null;
        }

        public static void RemovePanelFromGroup(this Panel panel, PanelGroup group)
        {
            if (group != null)
            {
                //PanelGroup group = panel.Group;
                group.dockedPanels.Remove(panel);

                group.SetMinSize();
                LayoutRebuilder.ForceRebuildLayoutImmediate(group.RectTransform);
                group.CheckGroupDropAnchors();
                //panel.Group = null;
            }
        }

  /*      public static void CheckGroupMinSize(PanelGroup group)
        {
            float minX = 0f;
            float minY = 0f;

            if (group != null)
            {
                for (int i = 0; i < group.dockedPanels.Count; i++)
                {
                    if (group.dockedPanels[i].MinSize.x > minX)
                        minX = group.dockedPanels[i].MinSize.x;

                    if (group.dockedPanels[i].MinSize.y > minY)
                        minY = group.dockedPanels[i].MinSize.y;
                }

                float newMinX = new[] { group.baseMinSize.x, group.MinSize.x, minX }.Max();
                float newMinY = new[] { group.baseMinSize.y, group.MinSize.y, minY }.Max();
                //group.MinSize = new Vector2(newMinX, newMinY);
                //group.SetMinSize(new Vector2(newMinX, newMinY));

                if (group.position == Direction.Left || group.position == Direction.Right)
                {
                    if (group.MinSize.x > group.RectTransform.sizeDelta.x)
                    {
                        float sizeDifX = group.MinSize.x - group.RectTransform.sizeDelta.x;//this is how much we need to adjust our group size
                                                                                           //if our new MinSize is greater than our current actual size
                        if (group.position == Direction.Right)
                            sizeDifX = -sizeDifX;
                        sizeDifX = group.CheckDragValueOnGroups(group.position.Opposite(), sizeDifX);

                        //panel.RectTransform.sizeDelta = new Vector2(minX, minY);
                        Vector2 temp = new Vector2(sizeDifX, 0f);
                        group.OnResize(group.position.Opposite(), temp, false);
                    }
                }
                else if (group.position == Direction.Top || group.position == Direction.Bottom)
                {
                    if (group.MinSize.y > group.RectTransform.sizeDelta.y)
                    {
                        float sizeDifY = group.MinSize.y - group.RectTransform.sizeDelta.y;
                        if (group.position == Direction.Top)
                            sizeDifY = -sizeDifY;
                        sizeDifY = group.CheckDragValueOnGroups(group.position.Opposite(), sizeDifY);
                        //panel.RectTransform.sizeDelta = new Vector2(minX, minY);
                        Vector2 temp = new Vector2(0f, sizeDifY);
                        group.OnResize(group.position.Opposite(), temp, false);
                    }
                }
            }
        }*/

        public static float CheckGroupExpansionInDirection(this PanelGroup group, Direction direction, float difference, bool neighborsOnly = true)
        {
            float adjustedDif = difference;
            float sizeDelta = 0f;
            float minSize = 0f;
            bool doSecondary = false;
            List<PanelGroup> groupsInDirection = new List<PanelGroup>();
            //List<PanelGroup> groupSecondary = new List<PanelGroup>();

            if (((direction == Direction.Left || direction == Direction.Bottom) && difference > 0f) ||
                ((direction == Direction.Right || direction == Direction.Top) && difference < 0f))
            {
                doSecondary = true;
            }

            switch (direction)
            {
                case Direction.Top:
                    groupsInDirection.AddRange(group.groupsOnTop);
                    break;
                case Direction.Left:
                    groupsInDirection.AddRange(group.groupsOnLeft);
                    break;
                case Direction.Right:
                    groupsInDirection.AddRange(group.groupsOnRight);
                    break;
                case Direction.Bottom:
                    groupsInDirection.AddRange(group.groupsOnBottom);
                    break;
            }
            //Debug.Log(groupsInDirection.Count);
            if (groupsInDirection.Count > 0)
            {
                for (int i = 0; i < groupsInDirection.Count; i++)
                {
                    //if we need to check both sides of a group border for minSize, use the secondary groups for the check
                    if (doSecondary)
                    {
                        //Debug.Log(direction);
                        adjustedDif = groupsInDirection[i].CheckGroupExpansionInDirection(direction.Opposite(), adjustedDif, neighborsOnly);

                    } else
                    {
                        //if we only need to check 1 side of a group for minSize checks
                        if (direction == Direction.Left || direction == Direction.Right)
                        {
                            sizeDelta = groupsInDirection[i].RectTransform.sizeDelta.x;// group.RectTransform.sizeDelta.x;
                            minSize = groupsInDirection[i].MinSize.x;
                        }
                        else
                        {
                            sizeDelta = groupsInDirection[i].RectTransform.sizeDelta.y;
                            minSize = groupsInDirection[i].MinSize.y;
                        }
                        if (sizeDelta > minSize)
                        {
                            if (adjustedDif < 0f)
                            {
                                if (sizeDelta + adjustedDif <= minSize)
                                {
                                    adjustedDif = minSize - sizeDelta;
                                }
                            }
                            else if (adjustedDif > 0f)
                            {
                                if (sizeDelta - adjustedDif <= minSize)
                                {
                                    adjustedDif = sizeDelta - minSize;
                                }
                            }
                        }
                        else
                        {
                            if (!neighborsOnly)
                            {
                                adjustedDif = groupsInDirection[i].CheckGroupExpansionInDirection(direction, adjustedDif, false);
                            }
                            else
                            {
                                return 0f;
                            }
                        }
                    }
                }
            } else
            {
                if (direction == Direction.Left || direction == Direction.Right)
                {
                    sizeDelta = group.RectTransform.sizeDelta.x;
                    minSize = group.MinSize.x;
                } else
                {
                    sizeDelta = group.RectTransform.sizeDelta.y;
                    minSize = group.MinSize.y;
                }
                if (sizeDelta > minSize)// group.RectTransform.sizeDelta.y > group.MinSize.y)
                {
                    if (sizeDelta - adjustedDif <= minSize)// group.RectTransform.sizeDelta.y - adjustedDif <= group.MinSize.y)
                    {
                        adjustedDif = sizeDelta - minSize;
                        //adjustedDif = group.RectTransform.sizeDelta.y - group.MinSize.y;
                    }
                } else
                {
                    return 0f;
                }
            }

            //for now, assuming we just added a panel to this group, check all neighboring groups in a direction for adjustment if we need to?
  /*          switch (direction)
            {
                case Direction.Top:
                    if (group.groupsOnTop.Count > 0)
                    {
                        //there are groups to the top
                        for (int i = 0; i < group.groupsOnTop.Count; i++)
                        {
                            if (group.groupsOnTop[i].RectTransform.sizeDelta.y > group.groupsOnTop[i].MinSize.y)
                            {
                                if (group.groupsOnTop[i].RectTransform.sizeDelta.y - adjustedDif <= group.groupsOnTop[i].MinSize.y)
                                {
                                    adjustedDif = group.groupsOnTop[i].RectTransform.sizeDelta.y - group.groupsOnTop[i].MinSize.y;
                                }
                            }
                            else
                            {
                                adjustedDif = group.groupsOnTop[i].CheckGroupExpansionInDirection(direction, adjustedDif);
                            }
                        }
                    } else
                    {
                        //no groups to the top
                        if (group.RectTransform.sizeDelta.y > group.MinSize.y)
                        {
                            if (group.RectTransform.sizeDelta.y - adjustedDif <= group.MinSize.y)
                            {
                                adjustedDif = group.RectTransform.sizeDelta.y - group.MinSize.y;
                            }
                        }
                        else
                        {
                            return 0f;
                        }
                    }
                    break;
                case Direction.Left:
                    if (group.groupsOnLeft.Count > 0)
                    {
                        //there are groups to the left
                        for (int i = 0; i < group.groupsOnLeft.Count; i++)
                        {
                            if (group.groupsOnLeft[i].RectTransform.sizeDelta.x > group.groupsOnLeft[i].MinSize.x)
                            {
                                if (group.groupsOnLeft[i].RectTransform.sizeDelta.x - adjustedDif <= group.groupsOnLeft[i].MinSize.x)
                                {
                                    adjustedDif = group.groupsOnLeft[i].RectTransform.sizeDelta.x - group.groupsOnLeft[i].MinSize.x;
                                }
                            }
                            else
                            {
                                adjustedDif = group.groupsOnLeft[i].CheckGroupExpansionInDirection(direction, adjustedDif);
                            }
                        }
                        //Debug.Log($"left: {adjustedDif}");
                    } else
                    {
                        //no groups to the left
                        if (group.RectTransform.sizeDelta.x > group.MinSize.x)
                        {
                            if (group.RectTransform.sizeDelta.x - adjustedDif <= group.MinSize.x)
                            {
                                adjustedDif = group.RectTransform.sizeDelta.x - group.MinSize.x;
                            }
                        }
                        else
                        {
                            return 0f;
                        }
                    }
                    break;
                case Direction.Right:
                    if (group.groupsOnRight.Count > 0)
                    {
                        //there are groups to the right
                        for (int i = 0; i < group.groupsOnRight.Count; i++)
                        {
                            if (group.groupsOnRight[i].RectTransform.sizeDelta.x > group.groupsOnRight[i].MinSize.x)
                            {
                                if (group.groupsOnRight[i].RectTransform.sizeDelta.x - adjustedDif <= group.groupsOnRight[i].MinSize.x)
                                {
                                    adjustedDif = group.groupsOnRight[i].RectTransform.sizeDelta.x - group.groupsOnRight[i].MinSize.x;
                                }
                            } else
                            {
                                adjustedDif = group.groupsOnRight[i].CheckGroupExpansionInDirection(direction, adjustedDif);
                            }
                        }
                    } else
                    {
                        //no groups to the right
                        if (group.RectTransform.sizeDelta.x > group.MinSize.x)
                        {
                            if (group.RectTransform.sizeDelta.x - adjustedDif <= group.MinSize.x)
                            {
                                adjustedDif = group.RectTransform.sizeDelta.x - group.MinSize.x;
                            }
                        } else
                        {
                            return 0f;
                        }
                    }
                    break;
                case Direction.Bottom:
                    if (group.groupsOnBottom.Count > 0)
                    {
                        //there are groups to the bottom
                        for (int i = 0; i < group.groupsOnBottom.Count; i++)
                        {
                            if (group.groupsOnBottom[i].RectTransform.sizeDelta.y > group.groupsOnBottom[i].MinSize.y)
                            {
                                if (group.groupsOnBottom[i].RectTransform.sizeDelta.y - adjustedDif <= group.groupsOnBottom[i].MinSize.y)
                                {
                                    adjustedDif = group.groupsOnBottom[i].RectTransform.sizeDelta.y - group.groupsOnBottom[i].MinSize.y;
                                }
                            }
                            else
                            {
                                adjustedDif = group.groupsOnBottom[i].CheckGroupExpansionInDirection(direction, adjustedDif);
                            }
                            //Debug.Log($"bottom: {adjustedDif}");
                        }
                    } else
                    {
                        //no groups to the bottom
                        if (group.RectTransform.sizeDelta.y > group.MinSize.y)
                        {
                            if (group.RectTransform.sizeDelta.y - adjustedDif <= group.MinSize.y)
                            {
                                adjustedDif = group.RectTransform.sizeDelta.y - group.MinSize.y;
                            }
                        }
                        else
                        {
                            return 0f;
                        }
                    }
                    break;


            }*/
            return adjustedDif;
        }

        public static float CheckPanelExpansionInDirection(this Panel panel, Direction direction, float difference)
        {
            if (difference == 0f)
                return 0f;

            float adjustedDif = difference;
            //float sizeDelta = 0f;
            //float minSize = 0f;

            if (panel.Group != null && panel.Group.position != Direction.Main)
            {
                //if this panel is in a panelGroup(not a floater), AND it's not in the main, unrestricted panelGroup
                if (panel.Group.layout == Layout.Down && direction == Direction.Bottom)
                {
                    //for now, just let user make panel as big as they want, even if it is bigger than panelGroup size

                    //vertical group and we're dragging the bottom zone
                    adjustedDif = CheckPanelMinSize(panel, direction, adjustedDif);
                    //Debug.Log(panel.Group.dockedPanels.IndexOf(panel));
                    //if we've clicked on a drag zone, then we already know there is at least one panel in the list so we just use Last
                    //Panel temp = panel.Group.dockedPanels.Last();
                    //vertical group going down, anchored y is negative
                    //Debug.Log(temp.RectTransform.anchoredPosition.y - temp.RectTransform.sizeDelta.y);//this is the bottom edge of the last panel in group, negative

                }
                else if (panel.Group.layout == Layout.Right && direction == Direction.Right)
                {
                    //horizontal group and we're dragging the right zone
                    adjustedDif = CheckPanelMinSize(panel, direction, adjustedDif);

                }
            }
            else if (panel.Group != null && panel.Group.position == Direction.Main)
            {
                //if this panel IS in the main panelGroup
                // then we're gonna have to somehow check if we're hitting the edge of the panelGroup or other panels
                //and if we're hitting this panel's min sizes
                adjustedDif = CheckPanelMinSize(panel, direction, adjustedDif);


            } else
            {
                //this panel is a floater so we only need to check if we're hitting this panel's min sizes
                adjustedDif = CheckPanelMinSize(panel, direction, adjustedDif);
                //Debug.Log(adjustedDif);
              /*  if (direction == Direction.Left || direction == Direction.Right)
                {
                    sizeDelta = panel.RectTransform.sizeDelta.x;
                    minSize = panel.MinSize.x;
                    //Debug.Log(adjustedDif);
                    if (sizeDelta > minSize)
                    {
                        if (adjustedDif < 0f)
                        {
                            if (sizeDelta + adjustedDif <= minSize)
                            {
                                adjustedDif = minSize - sizeDelta;
                            }
                        }
                        else if (adjustedDif > 0f)
                        {
                            if (sizeDelta - adjustedDif <= minSize)
                            {
                                adjustedDif = sizeDelta - minSize;
                            }
                        }
                        Debug.Log(adjustedDif);
                    }
                    else
                    {
                        if (direction == Direction.Right)
                        {
                            if (adjustedDif > 0f)
                                return 0f;
                        } else
                        {
                            if (adjustedDif < 0f)
                                return 0f;
                        }
                    }
                } else//direction is Top or Bottom
                {
                    sizeDelta = panel.RectTransform.sizeDelta.y;
                    minSize = panel.MinSize.y;

                    if (sizeDelta > minSize)
                    {
                        if (adjustedDif > 0f)
                        {
                            if (sizeDelta - adjustedDif <= minSize)
                            {
                                adjustedDif = sizeDelta - minSize;
                            }
                        }
                        else if (adjustedDif < 0f)
                        {
                            if (sizeDelta + adjustedDif <= minSize)
                            {
                                adjustedDif = minSize - sizeDelta;
                            }
                        }
                    } else
                    {
                        if (direction == Direction.Top)
                        {
                            if (adjustedDif < 0f)
                                return 0f;
                        } else
                        {
                            if (adjustedDif > 0f)
                                return 0f;
                        }
                    }
                }*/
            }
            return adjustedDif;
        }

        public static float CheckPanelMinSize(Panel panel, Direction direction, float difference)
        {
            float adjustedDif = difference;
            float sizeDelta = 0f;
            float minSize = 0f;

            if (direction == Direction.Left || direction == Direction.Right)
            {
                sizeDelta = panel.RectTransform.sizeDelta.x;
                minSize = panel.MinSize.x;
            }
            else
            {
                sizeDelta = panel.RectTransform.sizeDelta.y;
                minSize = panel.MinSize.y;
            }
            //Debug.Log($"{sizeDelta}, {minSize}");
            if (sizeDelta > minSize)
            {
                if (adjustedDif > 0f)
                {
                    if (sizeDelta - adjustedDif <= minSize)
                    {
                        adjustedDif = sizeDelta - minSize;
                    }
                }
                else if (adjustedDif < 0f)
                {
                    if (sizeDelta + adjustedDif <= minSize)
                    {
                        adjustedDif = minSize - sizeDelta;
                    }
                }
            }
            else
            {
                if (direction == Direction.Top)
                {
                    if (adjustedDif < 0f)
                        return 0f;
                }
                else if (direction == Direction.Right)
                {
                    if (adjustedDif < 0f)
                        return 0f;
                }
                else if (direction == Direction.Bottom)
                {
                    if (adjustedDif > 0f)
                        return 0f;
                } else
                {
                    if (adjustedDif > 0f)
                        return 0f;
                }

            }
            return adjustedDif;
        }


        //do we have any panelGroups already made in this direction?
        public static PanelGroup AnyGroupsInDirection(PanelsCanvas canvas, Direction position)
        {
            for (int i = canvas.panelGroups.Count - 1; i >= 0; i--)
            {
                //go backwards through the list so we find the latest group in this direction so we can 'dock' to that one
                if (canvas.panelGroups[i].position == position)
                {
                    return canvas.panelGroups[i];
                }
            }
            return null;
        }

        public static PanelGroupResize GetResizeZone(this PanelGroup group, Direction direction)
        {
            for (int i = 0; i < group.resizeZones.Count; i++)
            {
                if (group.resizeZones[i].direction == direction)
                {
                    return group.resizeZones[i];
                }
            }
            return null;
        }

        public static List<PanelGroup> GetNeighborGroups(this PanelGroup group, Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return group.groupsOnLeft;
                case Direction.Right:
                    return group.groupsOnRight;
                case Direction.Top:
                    return group.groupsOnTop;
                case Direction.Bottom:
                    return group.groupsOnBottom;
                default:
                    return new List<PanelGroup>();
            }
        }

        public static List<PanelGroupResize> ClickedResizeZones(this PanelGroup group, Vector2 clickPoint)
        {
            List<PanelGroupResize> clickedZones = new List<PanelGroupResize>();

            for (int i = 0; i < group.resizeZones.Count; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(group.resizeZones[i].RectTransform, clickPoint, group.Canvas.worldCamera))
                {
                    clickedZones.Add(group.resizeZones[i]);
                }
            }
            return clickedZones;
        }

        public static List<PanelResize> ClickedResizeZones(this Panel panel, Vector2 clickPoint)
        {
            List<PanelResize> clickedZones = new List<PanelResize>();

            for (int i = 0; i < panel.resizeZones.Count; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(panel.resizeZones[i].RectTransform, clickPoint, panel.Canvas.worldCamera))
                {
                    clickedZones.Add(panel.resizeZones[i]);
                }
            }
            return clickedZones;
        }

 /*       public static void AddGroupToDrag(this PanelGroup group, Direction direction)
        {
            for (int i = 0; i < group.Canvas.draggedGroups.Count; i++)
            {
                if (group.Canvas.draggedGroups[i].group == group && group.Canvas.draggedGroups[i].direction == direction)
                {
                    //this group and direction is already in the list
                    return;
                }
            }
            //if we got to this point, group and direction aren't in the list so we add it
            DraggedGroups groupToDrag = new DraggedGroups();
            groupToDrag.group = group;
            groupToDrag.direction = direction;
            group.Canvas.draggedGroups.Add(groupToDrag);
            //check any neighborGroups in the direction of this group for any we need to add to the drag also
            List<PanelGroup> neighborGroups = group.GetNeighborGroups(direction);
            for (int i = 0; i < neighborGroups.Count; i++)
            {
                neighborGroups[i].AddGroupToDrag(direction.Opposite());
            }
        }*/

        public static float GetXDragDelta(PanelGroup group, Vector2 dragPoint)
        {
            Vector2 localPoint;
            float deltaX = 0f;
            for (int i = 0; i < group.draggedZones.Count; i++)
            {
                if (group.draggedZones[i].direction == Direction.Left)// Direction.Right)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(group.RectTransform, dragPoint, group.Canvas.worldCamera, out localPoint);
                    //get the offset from the RectTransform as to which direction we're dragging: left or right
                    return localPoint.x;
                    //deltaX = localPoint.x;
                    //break;
                }
                if (group.draggedZones[i].direction == Direction.Right)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(group.RectTransform, dragPoint, group.Canvas.worldCamera, out localPoint);
                    //Debug.Log(localPoint.x);
                    deltaX = localPoint.x - group.RectTransform.sizeDelta.x;
                    return deltaX;
                    //Debug.Log(deltaX);
                }
            }
            return deltaX;
        }

        public static float GetXDragDelta(Panel panel, Vector2 dragPoint)
        {
            Vector2 localPoint;
            float deltaX = 0f;
            for (int i = 0; i < panel.draggedZones.Count; i++)
            {
                if (panel.draggedZones[i].direction == Direction.Left)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.RectTransform, dragPoint, panel.Canvas.worldCamera, out localPoint);
                    //Debug.Log(localPoint.x);
                    return localPoint.x;
                }

                if (panel.draggedZones[i].direction == Direction.Right)
                {
                    //Debug.Log(panel.RectTransform.sizeDelta.x);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.RectTransform, dragPoint, panel.Canvas.worldCamera, out localPoint);
                    //Debug.Log(localPoint.x);
                    deltaX = localPoint.x - panel.RectTransform.sizeDelta.x;
                    //Debug.Log(deltaX);
                    return deltaX;
                }
            }
            //Debug.Log(deltaX);
            return deltaX;
        }

        public static float GetYDragDelta(PanelGroup group, Vector2 dragPoint)
        {
            Vector2 localPoint;
            float deltaY = 0f;
            for (int i = 0; i < group.draggedZones.Count; i++)
            {
                if (group.draggedZones[i].direction == Direction.Bottom)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(group.RectTransform, dragPoint, group.Canvas.worldCamera, out localPoint);
                    return localPoint.y;
                    //deltaY = localPoint.y;
                    //break;
                }
                if (group.draggedZones[i].direction == Direction.Top)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(group.RectTransform, dragPoint, group.Canvas.worldCamera, out localPoint);
                    deltaY = localPoint.y - group.RectTransform.sizeDelta.y;
                    return deltaY;
                }
            }
            return deltaY;
        }

        public static float GetYDragDelta(Panel panel, Vector2 dragPoint)
        {
            Vector2 localPoint;
            float deltaY = 0f;
            for (int i = 0; i < panel.draggedZones.Count; i++)
            {
                if (panel.draggedZones[i].direction == Direction.Top)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.RectTransform, dragPoint, panel.Canvas.worldCamera, out localPoint);
                    return localPoint.y;
                }

                if (panel.draggedZones[i].direction == Direction.Bottom)
                {
                    //Debug.Log(panel.RectTransform.anchoredPosition);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.RectTransform, dragPoint, panel.Canvas.worldCamera, out localPoint);
                    //Debug.Log(localPoint.y);
                    deltaY = localPoint.y + panel.RectTransform.sizeDelta.y;
                    //Debug.Log(deltaY);
                    return deltaY;
                }
            }
            return deltaY;
        }

        public static float AdjustXDragDelta(PanelGroup group, float dragDelta)
        {
            float adjustedDelta = dragDelta;
            Direction dir = Direction.None;

            for (int i = 0; i < group.draggedZones.Count; i++)
            {
                if (group.draggedZones[i].direction == Direction.Left || group.draggedZones[i].direction == Direction.Right)
                {
                    dir = group.draggedZones[i].direction;
                }
            }

            if (dragDelta > 0f)
            {
                //Debug.Log("dragging left or right border to the right");
                if (dir == Direction.Left)
                {
                    //left border to the right
                    for (int i = 0; i < group.groupsOnLeft.Count; i++)
                    {
                        PanelGroup panelOnLeft = group.groupsOnLeft[i];
                        for (int j = 0; j < panelOnLeft.groupsOnRight.Count; j++)
                        {
                            PanelGroup panelOnRight = panelOnLeft.groupsOnRight[j];
                            if (panelOnRight.RectTransform.sizeDelta.x > panelOnRight.MinSize.x)
                            {
                                //if there is some room to move, figure out how much
                                if (panelOnRight.RectTransform.sizeDelta.x - adjustedDelta <= panelOnRight.MinSize.x)
                                {
                                    //if the delta will take us less than the minSize, adjust the amount
                                    //if it won't, don't need to adjust it
                                    adjustedDelta = panelOnRight.RectTransform.sizeDelta.x - panelOnRight.MinSize.x;//this is how far we can actually go
                                }
                            } else
                            {
                                //no room to move, already at min size
                                return 0f;
                            }
                        }
                    }
                }
                else if (dir == Direction.Right)
                {
                    //right border to the right
                    for (int i = 0; i < group.groupsOnRight.Count; i++)
                    {
                        PanelGroup panelOnRight = group.groupsOnRight[i];
                        if (panelOnRight.RectTransform.sizeDelta.x > panelOnRight.MinSize.x)
                        {
                            //if there is some room to move, figure out how much
                            if (panelOnRight.RectTransform.sizeDelta.x - adjustedDelta <= panelOnRight.MinSize.x)
                            {
                                //if the delta will take us less than the minSize, adjust the amount
                                //if it won't, don't need to adjust it
                                adjustedDelta = panelOnRight.RectTransform.sizeDelta.x - panelOnRight.MinSize.x;//this is how far we can actually go
                            }
                        }
                        else
                        {
                            //no room to move, already at min size
                            return 0f;
                        }

                    }
                }

             /*   for (int i = 0; i < group.Canvas.draggedGroups.Count; i++)
                {
                    if (group.Canvas.draggedGroups[i].direction == Direction.Left)
                    {
                        if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x != group.Canvas.draggedGroups[i].group.MinSize.x)
                        {
                            //dragging to the right so we check the left side zone of this group
                            if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x - adjustedDelta <= group.Canvas.draggedGroups[i].group.MinSize.x)
                            {
                                //Debug.Log("less than min");
                                adjustedDelta = group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x - group.Canvas.draggedGroups[i].group.MinSize.x;
                                //adjustedDelta = group.Canvas.draggedGroups[i].group.MinSize.x - group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x + adjustedDelta;
                            }
                        } else
                        {
                            return 0f;
                        }
                    }
                }*/
            }
            else if (dragDelta < 0f)
            {
                //Debug.Log("dragging left or right border to the left");
                if (dir == Direction.Left)
                {
                    //left border to the left
                    for (int i = 0; i < group.groupsOnLeft.Count; i++)
                    {
                        PanelGroup panelOnLeft = group.groupsOnLeft[i];
                        if (panelOnLeft.RectTransform.sizeDelta.x > panelOnLeft.MinSize.x)
                        {
                            if (panelOnLeft.RectTransform.sizeDelta.x + adjustedDelta <= panelOnLeft.MinSize.x)
                            {
                                adjustedDelta = panelOnLeft.MinSize.x - panelOnLeft.RectTransform.sizeDelta.x;
                            }
                        }
                        else
                        {
                            return 0f;
                        }
                    }

                }
                else if (dir == Direction.Right)
                {
                    //right border to the left
                    for (int i = 0; i < group.groupsOnRight.Count; i++)
                    {
                        PanelGroup panelOnRight = group.groupsOnRight[i];
                        for (int j = 0; j < panelOnRight.groupsOnLeft.Count; j++)
                        {
                            PanelGroup panelOnLeft = panelOnRight.groupsOnLeft[j];
                            if (panelOnLeft.RectTransform.sizeDelta.x > panelOnLeft.MinSize.x)
                            {
                                if (panelOnLeft.RectTransform.sizeDelta.x + adjustedDelta <= panelOnLeft.MinSize.x)
                                {
                                    adjustedDelta = panelOnLeft.MinSize.x - panelOnLeft.RectTransform.sizeDelta.x;
                                }
                            } else
                            {
                                return 0f;
                            }
                        }
                    }
                }


          /*      for (int i = 0; i < group.Canvas.draggedGroups.Count; i++)
                {
                    if (group.Canvas.draggedGroups[i].direction == Direction.Right)
                    {
                        if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x != group.Canvas.draggedGroups[i].group.MinSize.x)
                        {
                            if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x + adjustedDelta <= group.Canvas.draggedGroups[i].group.MinSize.x)
                            {
                                //Debug.Log("less than min");
                                adjustedDelta = group.Canvas.draggedGroups[i].group.MinSize.x - group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x;
                                //adjustedDelta = group.Canvas.draggedGroups[i].group.MinSize.x - group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.x + adjustedDelta;
                            }
                        } else
                        {
                            return 0f;
                        }
                    }
                }*/
            }
            //Debug.Log(adjustedDelta);
            return adjustedDelta;
        }

        public static float AdjustYDragDelta(PanelGroup group, float dragDelta)
        {
            float adjustedDelta = dragDelta;
            Direction dir = Direction.None;

            for (int i = 0; i < group.draggedZones.Count; i++)
            {
                if (group.draggedZones[i].direction == Direction.Bottom || group.draggedZones[i].direction == Direction.Top)
                {
                    dir = group.draggedZones[i].direction;
                }
            }

            if (dragDelta > 0f)
            {
                //dragging bottom or top border up
                if (dir == Direction.Bottom)
                {
                    //bottom border up
                    for (int i = 0; i < group.groupsOnBottom.Count; i++)
                    {
                        PanelGroup panelOnBottom = group.groupsOnBottom[i];
                        for (int j = 0; j < panelOnBottom.groupsOnTop.Count; j++)
                        {
                            PanelGroup panelOnTop = panelOnBottom.groupsOnTop[j];
                            if (panelOnTop.RectTransform.sizeDelta.y > panelOnTop.MinSize.y)
                            {
                                //if there is some room to move, figure out how much
                                if (panelOnTop.RectTransform.sizeDelta.y - adjustedDelta <= panelOnTop.MinSize.y)
                                {
                                    //if the delta will take us less than the minSize, adjust the amount
                                    //if it won't, don't need to adjust it
                                    adjustedDelta = panelOnTop.RectTransform.sizeDelta.y - panelOnTop.MinSize.y;//this is how far we can actually go
                                }
                            }
                            else
                            {
                                //no room to move, already at min size
                                return 0f;
                            }
                        }
                    }
                }
                else if (dir == Direction.Top)
                {
                    //dragging top border up
                    for (int i = 0; i < group.groupsOnTop.Count; i++)
                    {
                        PanelGroup panelOnTop = group.groupsOnTop[i];
                        if (panelOnTop.RectTransform.sizeDelta.y > panelOnTop.MinSize.y)
                        {
                            //if there is some room to move, figure out how much
                            if (panelOnTop.RectTransform.sizeDelta.y - adjustedDelta <= panelOnTop.MinSize.y)
                            {
                                //if the delta will take us less than the minSize, adjust the amount
                                //if it won't, don't need to adjust it
                                adjustedDelta = panelOnTop.RectTransform.sizeDelta.y - panelOnTop.MinSize.y;//this is how far we can actually go
                            }
                        }
                        else
                        {
                            //no room to move, already at min size
                            return 0f;
                        }
                    }
                }
            }
            else if (dragDelta < 0f)
            {
                //dragging bottom or top border down
                if (dir == Direction.Bottom)
                {
                    //bottom border down
                    for (int i = 0; i < group.groupsOnBottom.Count; i++)
                    {
                        PanelGroup panelOnBottom = group.groupsOnBottom[i];
                        if (panelOnBottom.RectTransform.sizeDelta.y > panelOnBottom.MinSize.y)
                        {
                            if (panelOnBottom.RectTransform.sizeDelta.y + adjustedDelta <= panelOnBottom.MinSize.y)
                            {
                                adjustedDelta = panelOnBottom.MinSize.y - panelOnBottom.RectTransform.sizeDelta.y;
                            }
                        }
                        else
                        {
                            return 0f;
                        }
                    }
                }
                else if (dir == Direction.Top)
                {
                    //top border down
                    for (int i = 0; i < group.groupsOnTop.Count; i++)
                    {
                        PanelGroup panelOnTop = group.groupsOnTop[i];
                        for (int j = 0; j < panelOnTop.groupsOnBottom.Count; j++)
                        {
                            PanelGroup panelOnBottom = panelOnTop.groupsOnBottom[j];
                            if (panelOnBottom.RectTransform.sizeDelta.y > panelOnBottom.MinSize.y)
                            {
                                if (panelOnBottom.RectTransform.sizeDelta.y + adjustedDelta <= panelOnBottom.MinSize.y)
                                {
                                    adjustedDelta = panelOnBottom.MinSize.y - panelOnBottom.RectTransform.sizeDelta.y;
                                }
                            }
                            else
                            {
                                return 0f;
                            }
                        }
                    }
                }
            }

      /*      for (int i = 0; i < group.Canvas.draggedGroups.Count; i++)
            {
                if (group.Canvas.draggedGroups[i].direction == Direction.Bottom && dragDelta > 0)
                {
                    //dragging up
                    if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.y != group.Canvas.draggedGroups[i].group.MinSize.y)
                    {
                        if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.y - adjustedDelta <= group.Canvas.draggedGroups[i].group.MinSize.y)
                        {
                            adjustedDelta = group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.y - group.Canvas.draggedGroups[i].group.MinSize.y;
                        }
                    } else
                    {
                        return 0f;
                    }
                }
                else if (group.Canvas.draggedGroups[i].direction == Direction.Top && dragDelta < 0)
                {
                    //dragging down
                    if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.y != group.Canvas.draggedGroups[i].group.MinSize.y)
                    {
                        if (group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.y + adjustedDelta <= group.Canvas.draggedGroups[i].group.MinSize.y)
                        {
                            adjustedDelta = group.Canvas.draggedGroups[i].group.MinSize.y - group.Canvas.draggedGroups[i].group.RectTransform.sizeDelta.y;
                        }
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }*/
            return adjustedDelta;
        }

        public static Direction Opposite(this Direction direction)
        {
            if (direction == Direction.Top)
                return Direction.Bottom;
            else if (direction == Direction.Bottom)
                return Direction.Top;
            else if (direction == Direction.Left)
                return Direction.Right;
            else if (direction == Direction.Right)
                return Direction.Left;
            else
                return Direction.Main;
            //return (Direction)(((int)direction + 2) % 4);
        }

        public static bool IsNull(this PanelGroup element)
        {
            return element == null || element.Equals(null);
        }

    }
}
