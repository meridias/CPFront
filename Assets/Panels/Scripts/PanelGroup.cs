using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class PanelGroup : MonoBehaviour
    {
        public PanelsCanvas Canvas { get; set; }
        public RectTransform RectTransform { get; private set; }
        public Direction position;//this is the direction of this group in reference to the edges of the panelsCanvas
        public Layout layout;//this is the direction of the panels inside this group

        public List<Panel> dockedPanels = new List<Panel>();
        public RectTransform panelObjects;
        public RectTransform anchorObjects;
        public ScrollRect scrollRect;

        //list of groups to the sides of this group
        public List<PanelGroup> groupsOnTop = new List<PanelGroup>();
        public List<PanelGroup> groupsOnRight = new List<PanelGroup>();
        public List<PanelGroup> groupsOnBottom = new List<PanelGroup>();
        public List<PanelGroup> groupsOnLeft = new List<PanelGroup>();

        [HideInInspector]
        public RectTransform resizeZoneRect;
        //resize zones
        public List<PanelGroupResize> resizeZones = new List<PanelGroupResize>();
        public List<PanelGroupResize> draggedZones = new List<PanelGroupResize>();
        public List<AnchorDropZone> anchorZones = new List<AnchorDropZone>();
        //public PanelGroupResize resizeZoneTop;
        //public PanelGroupResize resizeZoneLeft;
        //public PanelGroupResize resizeZoneRight;
        //public PanelGroupResize resizeZoneBottom;

        [HideInInspector]
        public bool lockWidth = false;
        [HideInInspector]
        public bool lockHeight = false;
        //[HideInInspector]
        //public bool isDocked = false;
        //[HideInInspector]
        //public bool beingDragged = false;

        protected const float MIN_SIZE_TOLERANCE = 1E-4f;

        private Coroutine updateContentRectCO = null;

        //        public PanelGroup(PanelsCanvas canvas, Direction direction)
        //      {
        //Canvas = canvas;
        //          Internal = new InternalSettings(this);

        //        this.direction = direction;

        //          elements = new List<IPanelGroupElement>(2);
        //          surroundings = new IPanelGroupElement[4];
        //  }
        //private Coroutine resizeCO = null;

        public Vector2 baseMinSize = new Vector2(30f, 30f);
        public Vector2 m_minSize = new Vector2(30f, 30f);
        public Vector2 MinSize
        {
            get { return m_minSize; }
          /*  set
            {
                float minX = 0f;
                float minY = 0f;
                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    if (dockedPanels[i].MinSize.x > minX)
                        minX = dockedPanels[i].MinSize.x;
                    if (dockedPanels[i].MinSize.y > minY)
                        minY = dockedPanels[i].MinSize.y;
                }
                m_minSize = new Vector2(Mathf.Max(baseMinSize.x, minX), Mathf.Max(baseMinSize.y, minY));

                if (m_minSize != value)
                {
                    m_minSize = value;
                    //Group.Internal.SetDirty();????
                }
            }*/
        }
        public Vector2 SetMinSize()
        {
            float minX = 0f;
            float minY = 0f;
            for (int i = 0; i < dockedPanels.Count; i++)
            {
                if (dockedPanels[i].MinSize.x > minX)
                    minX = dockedPanels[i].MinSize.x;
                if (dockedPanels[i].MinSize.y > minY)
                    minY = dockedPanels[i].MinSize.y;
            }
            m_minSize = new Vector2(Mathf.Max(baseMinSize.x, minX), Mathf.Max(baseMinSize.y, minY));

            if (position == Direction.Left || position == Direction.Right)
            {
                if (m_minSize.x > RectTransform.sizeDelta.x)
                {
                    float sizeDifX = m_minSize.x - RectTransform.sizeDelta.x;//this is how much we need to adjust our group size
                                                                                       //if our new MinSize is greater than our current actual size
                    if (position == Direction.Right)
                        sizeDifX = -sizeDifX;
                    sizeDifX = this.CheckGroupExpansionInDirection(position.Opposite(), sizeDifX, false);

                    //panel.RectTransform.sizeDelta = new Vector2(minX, minY);
                    //Vector2 temp = new Vector2(sizeDifX, 0f);
                    OnResize(position.Opposite(), sizeDifX, false);
                }
            }
            else if (position == Direction.Top || position == Direction.Bottom)
            {
                if (m_minSize.y > RectTransform.sizeDelta.y)
                {
                    float sizeDifY = m_minSize.y - RectTransform.sizeDelta.y;
                    if (position == Direction.Top)
                        sizeDifY = -sizeDifY;
                    sizeDifY = this.CheckGroupExpansionInDirection(position.Opposite(), sizeDifY, false);
                    //panel.RectTransform.sizeDelta = new Vector2(minX, minY);
                    //Vector2 temp = new Vector2(0f, sizeDifY);
                    OnResize(position.Opposite(), sizeDifY, false);
                }
            }
            return m_minSize;
        }

        public Vector2 SetSizeDelta(Vector2 newSizeDelta)
        {
            float sizeDif = 0f;
            if (position == Direction.Left || position == Direction.Right)
            {
                if (newSizeDelta.x > RectTransform.sizeDelta.x)
                {
                    sizeDif = newSizeDelta.x - RectTransform.sizeDelta.x;
                    //float sizeDifX = newSizeDelta.x - RectTransform.sizeDelta.x;
                    if (position == Direction.Right)
                        sizeDif = -sizeDif;
                    sizeDif = this.CheckGroupExpansionInDirection(position.Opposite(), sizeDif, false);
                    //OnResize(position.Opposite(), sizeDifX, false);
                }
            }
            else if (position == Direction.Top || position == Direction.Bottom)
            {
                if (newSizeDelta.y > RectTransform.sizeDelta.y)
                {
                    sizeDif = newSizeDelta.y - RectTransform.sizeDelta.y;
                    //float sizeDifY = newSizeDelta.y - RectTransform.sizeDelta.y;
                    if (position == Direction.Top)
                        sizeDif = -sizeDif;
                    sizeDif = this.CheckGroupExpansionInDirection(position.Opposite(), sizeDif, false);
                    //OnResize(position.Opposite(), sizeDifY, false);
                }
            }
            OnResize(position.Opposite(), sizeDif, false);
            return RectTransform.sizeDelta;
        }

        public Vector2 AnchorPos
        {
            get { return RectTransform.anchoredPosition; }
            set
            {

            }
        }

        public Vector2 SizeDelta
        {
            get { return RectTransform.sizeDelta; }
            set
            {
                //SetSizeDelta(value);//?
                RectTransform.sizeDelta = value;
                //Debug.Log(RectTransform.sizeDelta);
            }
        }

        public void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(OnScrolled);
        }

        public void OnDisable()
        {
            scrollRect.onValueChanged.RemoveListener(OnScrolled);
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            //resizeZoneTop.Initialize(this);//, Direction.Top);
            //resizeZoneLeft.Initialize(this);//, Direction.Left);
            //resizeZoneRight.Initialize(this);//, Direction.Right);
            //resizeZoneBottom.Initialize(this);//, Direction.Bottom);

            foreach (Transform child in transform)
            {
                if (child.name == "ResizeObjects")
                {
                    resizeZoneRect = child.gameObject.GetComponent<RectTransform>();
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnScrolled(Vector2 scrolledAmt)
        {
            //Debug.Log(scrolledAmt);
            anchorObjects.anchoredPosition = panelObjects.anchoredPosition;

        }

        public void OnResize(Direction direction, float dragDelta, bool neighbors = true)//Vector2 dragDeltas
        {
            //neighbors bool is for "we're only resizing/dragging our immediate neighbor panelGroups, not extending the drag to others
            //so that we hit min sizes for only the groups around our drag point
            //false will be for when we force resize to updated minSizes when adding panels to a group

            //dragDeltas is how much we're wanting to expand/shrink in a given direction, not how much we can, yet.
            //do check for how much we actually can expand/shrink here...
            //Vector2 newDrag = new Vector2(PanelUtils.AdjustXDragDelta(this, dragDeltas.x), PanelUtils.AdjustYDragDelta(this, dragDeltas.y));
            dragDelta = this.CheckGroupExpansionInDirection(direction, dragDelta, neighbors);
     //       if (direction == Direction.Left || direction == Direction.Right)
     //       {
                //Debug.Log(dragDelta);
     //       }
            //     Vector2 localPoint;
            //     RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, dragPoint, Canvas.worldCamera, out localPoint);
            Vector2 sizeDelta = RectTransform.sizeDelta;
            Vector2 anchoredPosition = RectTransform.anchoredPosition;

            //localPoint is where in the Rect of this group the dragPoint is at, either inside or outside of it
            //Debug.Log($"{gameObject.name}:{localPoint}");

            if (direction == Direction.Left)
            {
                for (int i = 0; i < groupsOnLeft.Count; i++)
                {
                    for (int j = 0; j < groupsOnLeft[i].groupsOnRight.Count; j++)
                    {
                        if (groupsOnLeft[i].groupsOnRight[j] != this)
                        {
                            groupsOnLeft[i].groupsOnRight[j].ResizeGroup(direction, dragDelta);
                        }
                    }
                    groupsOnLeft[i].ResizeGroup(direction.Opposite(), dragDelta);
                    if (!neighbors)
                    {
                        if (groupsOnLeft[i].RectTransform.sizeDelta.x - dragDelta < groupsOnLeft[i].MinSize.x)
                        {
                            groupsOnLeft[i].OnResize(direction, dragDelta, false);
                        }
                    }
                }
                //deltaSize = -localPoint.x;
                //               RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x + dragDeltas.x, RectTransform.anchoredPosition.y);
                //               RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x - dragDeltas.x, RectTransform.sizeDelta.y);
                //               for (int i = 0; i < dockedPanels.Count; i++)
                //               {
                //                   dockedPanels[i].RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, dockedPanels[i].RectTransform.sizeDelta.y);
                //               }

                ResizeGroup(direction, dragDelta);
            }
            else if (direction == Direction.Top)
            {
                for (int i = 0; i < groupsOnTop.Count; i++)
                {
                    for (int j = 0; j < groupsOnTop[i].groupsOnBottom.Count; j++)
                    {
                        if (groupsOnTop[i].groupsOnBottom[j] != this)
                        {
                            groupsOnTop[i].groupsOnBottom[j].ResizeGroup(direction, dragDelta);
                        }
                    }
                    groupsOnTop[i].ResizeGroup(direction.Opposite(), dragDelta);
                    if (!neighbors)
                    {
                        if (groupsOnTop[i].RectTransform.sizeDelta.y - dragDelta < groupsOnTop[i].MinSize.y)
                        {
                            groupsOnTop[i].OnResize(direction, dragDelta, false);
                        }
                    }
                }

                //deltaSize = localPoint.y - sizeDelta.y;
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y + dragDeltas.y);
                ResizeGroup(direction, dragDelta);
            }
            //properties.sizeY += deltaSize;
            else if (direction == Direction.Right)
            {
                for (int i = 0; i < groupsOnRight.Count; i++)
                {
                    for (int j = 0; j < groupsOnRight[i].groupsOnLeft.Count; j++)
                    {
                        if (groupsOnRight[i].groupsOnLeft[j] != this)
                        {
                            groupsOnRight[i].groupsOnLeft[j].ResizeGroup(direction, dragDelta);
                        }
                    }
                    groupsOnRight[i].ResizeGroup(direction.Opposite(), dragDelta);
                    if (!neighbors)
                    {
                        if (groupsOnRight[i].RectTransform.sizeDelta.x - dragDelta < groupsOnRight[i].MinSize.x)
                        {
                            groupsOnRight[i].OnResize(direction, dragDelta, false);
                        }
                    }
                }
                //deltaSize = localPoint.x - sizeDelta.x;
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x + dragDeltas.x, RectTransform.sizeDelta.y);
                //for (int i = 0; i < dockedPanels.Count; i++)
                // {
                //     dockedPanels[i].RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, dockedPanels[i].RectTransform.sizeDelta.y);
                // }
                ResizeGroup(direction, dragDelta);
            }
            //properties.sizeX += deltaSize;
            else//Direction.Bottom
            {
                for (int i = 0; i < groupsOnBottom.Count; i++)
                {
                    for (int j = 0; j < groupsOnBottom[i].groupsOnTop.Count; j++)
                    {
                        if (groupsOnBottom[i].groupsOnTop[j] != this)
                        {
                            groupsOnBottom[i].groupsOnTop[j].ResizeGroup(direction, dragDelta);
                        }
                    }
                    groupsOnBottom[i].ResizeGroup(direction.Opposite(), dragDelta);
                    if (!neighbors)
                    {
                        if (groupsOnBottom[i].RectTransform.sizeDelta.y - dragDelta < groupsOnBottom[i].MinSize.y)
                        {
                            groupsOnBottom[i].OnResize(direction, dragDelta, false);
                        }
                    }
                }

                //deltaSize = -localPoint.y;
                //RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, RectTransform.anchoredPosition.y + dragDeltas.y);
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y - dragDeltas.y);
                //properties.posY -= deltaSize;
                //properties.sizeY += deltaSize;
                ResizeGroup(direction, dragDelta);
            }




            /*              float deltaSize;
                          if (direction == Direction.Left)
                              deltaSize = -localPoint.x;
                          else if (direction == Direction.Top)
                              deltaSize = localPoint.y - sizeDelta.y;
                          else if (direction == Direction.Right)
                              deltaSize = localPoint.x - sizeDelta.x;
                          else
                              deltaSize = -localPoint.y;

               //           if (deltaSize >= 0f)
                              TryChangeSizeOf(this, direction, deltaSize);*/
            //            else
            //          {
            //     if (gameObject.name == "Main Panel Group")
            //     {
            //         Debug.Log($"{gameObject.name}:{deltaSize}");
            //     }
            //             List<PanelGroup> neighborGroups = this.GetNeighborGroups(direction);
            //             for (int i = 0; i < neighborGroups.Count; i++)
            //             {
            //                 TryChangeSizeOf(neighborGroups[i], direction.Opposite(), -deltaSize);
            //             }
            //        }
            //    } else
            //    {
            //drag/resize to extended groups to a side


            //    }
        }

 /*       public float CheckDragValueOnGroups(PanelGroup group, Direction direction, float difference)
        {
            float adjustedDir = difference;
            //for now, assuming we just added a panel to this group, check all neighboring groups in a direction for adjustment if we need to?
            switch (direction)
            {
                case Direction.Top:
                    for (int i = 0; i < groupsOnBottom.Count; i++)
                    {
                        if (groupsOnBottom[i].groupsOnBottom.Count == 0)
                        {
                            if (groupsOnBottom[i].RectTransform.sizeDelta.y - adjustedDir < groupsOnBottom[i].MinSize.x)
                            {

                            }
                        }
                    }
                    break;
                case Direction.Left:
                    break;
                case Direction.Right:
                    break;
                case Direction.Bottom:
                    break;


            }

            return adjustedDir;
        }*/

  /*      public void ResizeGroup(Direction direction, float deltaSize)
        {
            if (resizeCO == null)
                resizeCO = StartCoroutine(TryChangeSize(direction, deltaSize));
        }*/

        public void ResizeGroup(Direction direction, float deltaSize)//was TryChangeSize
        {
            //   if (group.IsNull())// || deltaSize <= MIN_SIZE_TOLERANCE)// || group.GetSurroundingElement(direction).IsNull())
            //        return;
            //REMINDER: This 'direction' variable is not for the direction of this group in relation to the canvas
            //It is the direction we're dragging, just FYI

            if (deltaSize == 0f)
            {
                //if there is no change to the size of this panelGroup, all we need to do is update any panels in the group to the group size, just in case
 //               UpdatePanelGroupPanels(RectTransform.sizeDelta);
           /*     if (layout == Layout.Down)
                {
                    for (int i = 0; i < dockedPanels.Count; i++)
                    {
                        dockedPanels[i].RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, dockedPanels[i].RectTransform.sizeDelta.y);
                    }
                }
                else if (layout == Layout.Right)
                {
                    for (int i = 0; i < dockedPanels.Count; i++)
                    {
                        dockedPanels[i].RectTransform.sizeDelta = new Vector2(dockedPanels[i].RectTransform.sizeDelta.x, RectTransform.sizeDelta.y);
                    }
                }*/
                return;
            }

            bool rectFirst = false;
            Vector2 anchorPos = Vector2.zero;
            Vector2 size = Vector2.zero;

            //this is to help deal with scrollbar flicker
            if (((direction == Direction.Top || direction == Direction.Right) && deltaSize > 0f) ||
                ((direction == Direction.Left || direction == Direction.Bottom) && deltaSize < 0f))
                rectFirst = true;

            if (direction == Direction.Left)
            {
                anchorPos = new Vector2(RectTransform.anchoredPosition.x + deltaSize, RectTransform.anchoredPosition.y);
                size = new Vector2(RectTransform.sizeDelta.x - deltaSize, RectTransform.sizeDelta.y);
                //RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x + deltaSize, RectTransform.anchoredPosition.y);
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x - deltaSize, RectTransform.sizeDelta.y);
            }
            else if (direction == Direction.Top)
            {
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y + deltaSize);
                anchorPos = RectTransform.anchoredPosition;
                size = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y + deltaSize);
            }
            //properties.sizeY += deltaSize;
            else if (direction == Direction.Right)
            {
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x + deltaSize, RectTransform.sizeDelta.y);
                anchorPos = RectTransform.anchoredPosition;
                size = new Vector2(RectTransform.sizeDelta.x + deltaSize, RectTransform.sizeDelta.y);
            }
            //properties.sizeX += deltaSize;
            else if (direction == Direction.Bottom)
            {
                //RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, RectTransform.anchoredPosition.y + deltaSize);
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y - deltaSize);
                anchorPos = new Vector2(RectTransform.anchoredPosition.x, RectTransform.anchoredPosition.y + deltaSize);
                size = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y - deltaSize);
            }

            //if (gameObject.name == "Top Panel Group #?")
           // {
           //     Debug.Log(rectFirst);
           // }

 //           if (rectFirst)
 //           {
                RectTransform.anchoredPosition = anchorPos;
                SizeDelta = size;
            //RectTransform.sizeDelta = size;
            //yield return new WaitForEndOfFrame();
            //           }

            //UpdatePanelGroupPanels(size);
   /*         if (layout == Layout.Down)
            {
                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    dockedPanels[i].RectTransform.sizeDelta = new Vector2(size.x, dockedPanels[i].RectTransform.sizeDelta.y);
                }
            }
            else if (layout == Layout.Right)
            {
                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    dockedPanels[i].RectTransform.sizeDelta = new Vector2(dockedPanels[i].RectTransform.sizeDelta.x, size.y);
                }
            }*/
            //LayoutRebuilder.ForceRebuildLayoutImmediate(panelObjects);
            //UpdateContentRect();

  //          if (!rectFirst)
  //          {
                //yield return new WaitForEndOfFrame();
  //              RectTransform.anchoredPosition = anchorPos;
  //              SizeDelta = size;
                //RectTransform.sizeDelta = size;
  //          }
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelObjects);
            UpdateContentRect();
 //           UpdatePanelGroupPanels(size);
            //Debug.Log($"{gameObject.name}: pos:{anchorPos}, size:{size}");
        }

        public void CheckGroupDropAnchors()
        {
            int numDrops = anchorObjects.childCount;
            int numPanels = dockedPanels.Count;//  panelObjects.childCount;//wondering if we should just use the count for dockedPanels list instead of this
            float posOffset = 0f;

            if (layout == Layout.Down)
                posOffset = -3f;//adjust the anchorzones up just a bit to make them 'look' better
                                //Debug.Log(numPanels);
            /*    if (numDrops == 0)
                {
                    //add the drop that all side panelGroups have
                    AnchorDropZone newDrop = UnityEngine.Object.Instantiate(Resources.Load<AnchorDropZone>("AnchorDropZone"), anchorObjects, false);
                    newDrop.SetAnchor(this, 12f);
                    numDrops += 1;
                    if (layout == Layout.Right)
                        newDrop.AdjustPos(new Vector2(0f, -6f));
                    else if (layout == Layout.Down)
                        newDrop.AdjustPos(new Vector2(6f, 0f));
                }*/

            if (layout == Layout.Right || layout == Layout.Down)
            {
                for (int i = 0; i < numPanels; i++)
                {
                    //Panel panel = panelObjects.GetChild(i).gameObject.GetComponent<Panel>();
                    //Transform panelTF = panelObjects.GetChild(i);
                    //Debug.Log($"{i}: {panelTF.GetComponent<RectTransform>().anchoredPosition.y}");
                    //posOffset += panelObjects.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
                    //Debug.Log($"{i}: {posOffset}");
                    //Debug.Log(panel.Title);
                    if (i + 1 >= numDrops)
                    {
                        //we need to add another drop anchor
                        AnchorDropZone newDrop = Instantiate(Resources.Load<AnchorDropZone>("AnchorDropZone"), anchorObjects, false);
                        AddAnchorZoneToGroup(newDrop);
                        newDrop.SetAnchor(this, 20f);
                        numDrops++;
                    }
                    AnchorDropZone dropAnchor = anchorObjects.GetChild(i + 1).gameObject.GetComponent<AnchorDropZone>();
                    dropAnchor.gameObject.SetActive(true);

                    if (layout == Layout.Down)
                    {
                        //make sure that panel[i] is child[i]
                        dockedPanels[i].gameObject.transform.SetSiblingIndex(i);

                        posOffset += panelObjects.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
                        //Debug.Log(panel.Title);
                        //Debug.Log($"{i+1}:{panel.GetComponent<RectTransform>().anchoredPosition.y}:{panel.RectTransform.sizeDelta.y}");
                        Vector2 newPos = new Vector2(0f, -posOffset);
                        dropAnchor.transform.localPosition = newPos;

                    }
                    else if (layout == Layout.Right)
                    {
                        dockedPanels[i].gameObject.transform.SetSiblingIndex(i);
                        posOffset += panelObjects.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
                        Vector2 newPos = new Vector2(posOffset, 0f);
                        dropAnchor.transform.localPosition = newPos;
                    }
                }

                if (numDrops > numPanels + 1)
                {
                    //there are extra drop anchors that need to be disabled
                    for (int i = numPanels + 1; i < numDrops; i++)
                    {
                        anchorObjects.GetChild(i).gameObject.SetActive(false);
                    }
                }

            } else
            {
                //layout == Layout.Unrestricted, main
                
            }


        //    if (layout == Layout.Down)
        //    {

         //       anchorObjects.GetChild(0).localPosition = new Vector2(0f, -6f);
        //    }

            //for (int i = 0, i < )

        }

        public void AddAnchorZoneToGroup(AnchorDropZone anchor)
        {
            if (!anchorZones.Contains(anchor))
            {
                //if this anchor isn't already in the list, add it?
                anchorZones.Add(anchor);
            }

        }

        public void UpdatePanelGroupPanels(Vector2 newSize)
        {
            if (layout == Layout.Down)
            {
                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    dockedPanels[i].RectTransform.sizeDelta = new Vector2(newSize.x, dockedPanels[i].RectTransform.sizeDelta.y);
                }
            }
            else if (layout == Layout.Right)
            {
                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    dockedPanels[i].RectTransform.sizeDelta = new Vector2(dockedPanels[i].RectTransform.sizeDelta.x, newSize.y);
                }
            }

        }

        //      public void UpdateContentRect()
        //      {
        //          if (updateContentRectCO == null)
        //          {
        //              updateContentRectCO = StartCoroutine(UpdateContentRectCO());
        //          }
        //      }

        public void UpdateContentRect()// IEnumerator UpdateContentRectCO()
        {
            HorizontalOrVerticalLayoutGroup layoutGroup = null;
            RectTransform rect = (RectTransform)panelObjects.transform;
            float newX = 0f;
            float newY = 0f;

            if (layout == Layout.Right)
            {
                layoutGroup = panelObjects.GetComponent<HorizontalLayoutGroup>();
                if (layoutGroup.preferredWidth > RectTransform.sizeDelta.x)
                {
                    //if our content has gotten bigger than the group panel
                    newY = RectTransform.sizeDelta.y - 17;
                } else
                {
                    newY = RectTransform.sizeDelta.y;
                }
                newX = layoutGroup.preferredWidth;
            //    if (dockedPanels.Count == 0)
            //    {
                    //newY = RectTransform.sizeDelta.y;
                //    } else
                //    {
                //        newY = layoutGroup.preferredHeight;
                //    }
                //Debug.Log(scrollRect.viewport.rect.size.y);
                //newY = newY + scrollRect.viewport.sizeDelta.y;
                Vector2 temp = new Vector2(newX, newY);
                anchorObjects.sizeDelta = temp;
                rect.sizeDelta = temp;
                //yield return new WaitForEndOfFrame();
                UpdatePanelGroupPanels(temp);
                //LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect);
            }
            else if (layout == Layout.Down)
            {
                layoutGroup = panelObjects.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup.preferredHeight > RectTransform.sizeDelta.y)
                {
                    newX = RectTransform.sizeDelta.x - 17;
                } else
                {
                    newX = RectTransform.sizeDelta.x;
                }
                //yield return new WaitForEndOfFrame();
               // if (dockedPanels.Count == 0)
               // {
               //     newX = RectTransform.sizeDelta.x;
               // } else
               // {
                    //newX = layoutGroup.preferredWidth;
              //      newX = RectTransform.sizeDelta.x;
                    //Debug.Log(layoutGroup.preferredWidth);
             //   }
                //adjust for vertical scrollbar, if active or not (0 for not, -20 if it is)
                //if (scrollRect.verticalScrollbar.ac)
                //Debug.Log(scrollRect.viewport.sizeDelta.x);
                //newX = newX + scrollRect.viewport.sizeDelta.x;
                //Debug.Log(newX);
                newY = layoutGroup.preferredHeight;
                Vector2 temp = new Vector2(newX, newY);
                anchorObjects.sizeDelta = temp;
                //Vector2 temp = new Vector2(newX, newY);
                rect.sizeDelta = temp;

                //Debug.Log(temp.x);
                UpdatePanelGroupPanels(temp);
                //Debug.Log(scrollRect.viewport.sizeDelta.x);
            } else
            {
                //main group/unrestricted so full size of panel group
                RectTransform rectT = (RectTransform)panelObjects.transform;
                //Debug.Log(rectT.sizeDelta);
      //          if (dockedPanels.Count == 0)
      //          {
                    //if there are no docked panels in the main group, just set content Rect to size of group and be done with it
      //              rectT.sizeDelta = RectTransform.sizeDelta;
      //              Debug.Log(RectTransform.sizeDelta);
      //              return;
      //          }
                
                //Rect newRect = rectT.rect;
                float minX = rectT.sizeDelta.x;
                float maxX = float.NegativeInfinity;// 0f;
                float minY = float.PositiveInfinity;//0 is at top of panel and goes positive up
                //float maxY = float.PositiveInfinity;
                float sizeX = 0f;
                float sizeY = 0f;

                if (dockedPanels.Count == 0)
                {
                    minX = 0f;
                    minY = RectTransform.sizeDelta.y;// 0f;
                    //maxX = RectTransform.sizeDelta.x;
                }
                //Debug.Log(rectT.sizeDelta.x);
                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    //Debug.Log(dockedPanels[i].RectTransform.localPosition);
                    //Debug.Log(dockedPanels[i].RectTransform.rect.max);
                    if (dockedPanels[i].RectTransform.localPosition.x < minX)
                    {
                        minX = dockedPanels[i].RectTransform.localPosition.x;
                        //Debug.Log(rectT.offsetMin);
                    }
                    if (dockedPanels[i].RectTransform.localPosition.x + dockedPanels[i].RectTransform.sizeDelta.x - rectT.sizeDelta.x > maxX)
                    {
                        maxX = dockedPanels[i].RectTransform.localPosition.x + dockedPanels[i].RectTransform.sizeDelta.x - rectT.sizeDelta.x;// - maxX;// rectT.rect.max.x;
                        //Debug.Log(maxX);
                    }
                    if (-dockedPanels[i].RectTransform.localPosition.y + dockedPanels[i].RectTransform.sizeDelta.y > sizeY)// < minY)
                    {
                        sizeY = -dockedPanels[i].RectTransform.localPosition.y + dockedPanels[i].RectTransform.sizeDelta.y;
                        //minY = dockedPanels[i].RectTransform.localPosition.y - dockedPanels[i].RectTransform.sizeDelta.y;
                    }
                    //Debug.Log(dockedPanels[i].RectTransform.sizeDelta.x);
                    //Debug.Log(sizeY);
                }
                //Debug.Log($"{minX},{maxX}");
                if (minX < 0)// || maxX > 0)//add the y values when we get them
                {
                    //a panel is outside the content rect, get the numbers to resize it bigger
                    // rectT.anchoredPosition = new Vector2(minX, rectT.anchoredPosition.y);
                    sizeX = rectT.sizeDelta.x - minX;// + maxX;
                  //  Debug.Log(sizeX);
                  
                }
                else if (maxX > 0)
                {
                    sizeX = rectT.sizeDelta.x + maxX;
                }
                else
                {
                    //all panels are inside the rect, get the numbers to shrink it down to fit panels
                    //Debug.Log("doh");

                    /*   float smallestX = rectT.sizeDelta.x;//start with the whole size of the rect
                       float biggestX = 0f;
                       for (int i = 0; i < dockedPanels.Count; i++)
                       {
                           if (dockedPanels[i].RectTransform.localPosition.x < smallestX)
                           {
                               smallestX = dockedPanels[i].RectTransform.localPosition.x;
                           }
                           if (dockedPanels[i].RectTransform.localPosition.x + dockedPanels[i].RectTransform.sizeDelta.x > biggestX)
                           {
                               biggestX = dockedPanels[i].RectTransform.localPosition.x + dockedPanels[i].RectTransform.sizeDelta.x;
                           }

                       }*/
                    sizeX = rectT.sizeDelta.x - minX + maxX;//  maxX - minX;// biggestX - smallestX;
                }

                //Debug.Log(minX);
                //rectT.offsetMin = new Vector2(minX, RectTransform.sizeDelta.y);
                //Debug.Log(rectT.offsetMin);
                //rectT.anchoredPosition = new Vector2(minX, rectT.anchoredPosition.y);
                //Debug.Log(dockedPanels.Count);
                //Debug.Log(rectT.sizeDelta.x);
                //newX = rectT.sizeDelta.x - minX + maxX;
                sizeX = Mathf.Max(sizeX, RectTransform.sizeDelta.x);

                if (sizeY + 20f < RectTransform.sizeDelta.y)
                {
                    //scrollbars are 20 pix wide/tall
                    sizeY = RectTransform.sizeDelta.y - 20f;
                }

                //sizeY = Mathf.Max(sizeY, RectTransform.sizeDelta.y);
                //newY = RectTransform.sizeDelta.y;

               // rectT.anchoredPosition = new Vector2(minX, rectT.anchoredPosition.y);
               // rect.sizeDelta = new Vector2(sizeX, sizeY);



                for (int i = 0; i < dockedPanels.Count; i++)
                {
                    
                    //Debug.Log(dockedPanels[i].RectTransform.localPosition.x);
            //        if (minX < 0f)
            //        {
                        //  Debug.Log($"local: {dockedPanels[i].RectTransform.localPosition}, anchor: {dockedPanels[i].RectTransform.anchoredPosition}");
                        //Debug.Log($"local: {dockedPanels[i].RectTransform.localPosition.x}, minX: {minX}");
                        dockedPanels[i].RectTransform.localPosition = new Vector2(dockedPanels[i].RectTransform.localPosition.x - minX, dockedPanels[i].RectTransform.localPosition.y);//dockedPanels[i].RectTransform.localPosition.x - minX
                        // Debug.Log(dockedPanels[i].RectTransform.localPosition.x - minX);
                      //  Debug.Log($"local: {dockedPanels[i].RectTransform.localPosition}, anchor: {dockedPanels[i].RectTransform.anchoredPosition}");
            //        }

                }

                maxX = rectT.anchoredPosition.x + sizeX;
                if (maxX < RectTransform.sizeDelta.x)
                {
                    //if our current anchor.x + size is less than the main group sizeDelta, that means we're to the left of where we need to be
                    float diff = RectTransform.sizeDelta.x - maxX;//this should be a + number of what we need to adjust to the right
                    minX = rectT.anchoredPosition.x + diff;

                }

                rectT.anchoredPosition = new Vector2(minX, rectT.anchoredPosition.y);
                rect.sizeDelta = new Vector2(sizeX, sizeY);
                //Debug.Log(newX);
            }
            //Debug.Log($"{gameObject.name}: {newX}, {newY}");
            //RectTransform rect = (RectTransform)panelObjects.transform;
            //  rect.sizeDelta = new Vector2(newX, newY);
            updateContentRectCO = null;
        }

        public void SetAnchorZonesActive(bool active)
        {
            for (int i = 0; i < anchorZones.Count; i++)
            {
                anchorZones[i].SetActive(active);


            }

        }

        public void RemoveScrollbar(string whichScrollbar)
        {
            if (whichScrollbar == "vertical")
            {
                Destroy(scrollRect.verticalScrollbar.gameObject);
                scrollRect.verticalScrollbar = null;
            }
            else if (whichScrollbar == "horizontal")
            {
                Destroy(scrollRect.horizontalScrollbar.gameObject);
                scrollRect.horizontalScrollbar = null;
            }
        }

        public void UpdateBounds(Vector2 position, Vector2 size)
        {
            RectTransform.anchoredPosition = position;
            SizeDelta = size;
            //RectTransform.sizeDelta = size;
        }

    }
}
