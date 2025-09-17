using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.EventSystems;
//using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Panels
{
    public class Panel : MonoBehaviour, IPointerDownHandler
    {
        public PanelsCanvas Canvas { get; set; }//forgot, this gets set when panel is created
        //public PanelsCanvas Canvas { get { return Group.Canvas; } }
        public PanelGroup Group { get; set; }
        public RectTransform RectTransform { get; private set; }
        public List<PanelResize> resizeZones = new List<PanelResize>();
        public List<PanelResize> draggedZones = new List<PanelResize>();

        //[HideInInspector]
       // public TextWindowController text;
        [HideInInspector]
        public BaseOutput output;

        public bool isDockable = true;

        [SerializeField]
        private PanelHeader header;
        public PanelHeader Header { get { return header; } }
        //[SerializeField]
        public RectTransform content;

        private string titleTag = "";
        public string Title
        {
            get
            {
                return header.panelTitle.text;
            }
        }

        public string input
        {
            set
            {
                if (output != null)
                {
                    output.Input = value;
                }

            }
        }

        //[SerializeField]
        //private 
        private Vector2 m_minSize = new Vector2(30f, 30f);
        public Vector2 MinSize
        {
            get { return m_minSize; }
            set
            {
                m_minSize = value;
                if (Group != null && Group.position != Direction.Main)
                {
                    //if this is in a group and that group is one of the side panelGroups, not Main
                    if (Group.layout == Layout.Down)
                    {
                        //width will get taken care of by the panelGroup
                        if (m_minSize.y > RectTransform.sizeDelta.y)
                        {
                            OnResize(Direction.Bottom, m_minSize.y - RectTransform.sizeDelta.y);
                            //Group.SetMinSize();
                        }
                    } else
                    {
                        //height will get taken care of by the panelGroup
                        if (m_minSize.x > RectTransform.sizeDelta.x)
                        {
                            OnResize(Direction.Right, m_minSize.x - RectTransform.sizeDelta.x);
                            //Group.SetMinSize();
                        }
                    }
                    Group.SetMinSize();
                    Group.CheckGroupDropAnchors();
                }
                else
                {
                    //group is Main or null (floater)
                    //Debug.Log($"min: {m_minSize.x}, size: {RectTransform.sizeDelta.x}");
                    if (m_minSize.y > RectTransform.sizeDelta.y)
                    {
                        OnResize(Direction.Bottom, m_minSize.y - RectTransform.sizeDelta.y);
                    }
                    if (m_minSize.x > RectTransform.sizeDelta.x)
                    {
                        OnResize(Direction.Right, m_minSize.x - RectTransform.sizeDelta.x);
                    }
                    if (Group != null)
                        Group.SetMinSize();
                }

             //   if (m_minSize != value)
             //   {
             //       m_minSize = value;
                    //Group.Internal.SetDirty();????
             //   }
            }
        }

        public Vector2 SetSizeDelta(Vector2 newSizeDelta)
        {//definately going to need to run tests on this?
            float newX = RectTransform.sizeDelta.x;
            float newY = RectTransform.sizeDelta.y;

            if (Group != null)// && Group.layout == Layout.Down)
            {
                if ((Group.layout == Layout.Down || Group.layout == Layout.Unrestricted) && newSizeDelta.y != newY)
                {
                    //only worry about y
                    if (newSizeDelta.y < MinSize.y)
                    {
                        newY = MinSize.y;
                    }
                    else
                    {
                        newY = newSizeDelta.y;
                    }
                }
                if ((Group.layout == Layout.Right || Group.layout == Layout.Unrestricted) && newSizeDelta.x != newX)
                {
                    //only worry about x
                    if (newSizeDelta.x < MinSize.x)
                    {
                        newX = MinSize.x;
                    }
                    else
                    {
                        newX = newSizeDelta.x;
                    }
                }
                RectTransform.sizeDelta = new Vector2(newX, newY);
                Group.UpdateContentRect();
            }
            else//no group/undockable floater
            {
                if (newSizeDelta.y != newY)
                {
                    //only worry about y
                    if (newSizeDelta.y < MinSize.y)
                    {
                        newY = MinSize.y;
                    }
                    else
                    {
                        newY = newSizeDelta.y;
                    }
                }
                if (newSizeDelta.x != newX)
                {
                    //only worry about x
                    if (newSizeDelta.x < MinSize.x)
                    {
                        newX = MinSize.x;
                    }
                    else
                    {
                        newX = newSizeDelta.x;
                    }
                }
                RectTransform.sizeDelta = new Vector2(newX, newY);
            }
            return RectTransform.sizeDelta;
        }

        private void Awake()
        {
            RectTransform = (RectTransform)transform;
            
        }

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log(output.GetType());

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetAsLast();
        }

        public void SetAsLast()
        {
            if (Group == null || Group.layout == Layout.Unrestricted)
            {
                RectTransform.SetAsLastSibling();
                //Debug.Log("last");
            }

        }

        public void OnResize(Direction direction, float dragDelta)
        {
            dragDelta = this.CheckPanelExpansionInDirection(direction, dragDelta);
            //Debug.Log(dragDelta);
            ResizePanel(direction, dragDelta);
        }

        public void ResizePanel(Direction direction, float deltaSize)
        {
            Vector2 anchorPos = Vector2.zero;
            Vector2 size = Vector2.zero;

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
                anchorPos = new Vector2(RectTransform.anchoredPosition.x, RectTransform.anchoredPosition.y + deltaSize);
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
            else
            {
                //RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, RectTransform.anchoredPosition.y + deltaSize);
                //RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y - deltaSize);
                //anchorPos = new Vector2(RectTransform.anchoredPosition.x, RectTransform.anchoredPosition.y + deltaSize);
                anchorPos = RectTransform.anchoredPosition;
                size = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.y - deltaSize);
            }

            RectTransform.anchoredPosition = anchorPos;
            RectTransform.sizeDelta = size;

            //not sure if I still need this or not??????????
//            if (output != null)
//            {
//                output.SetText();
//            }

            if (Group != null && Group.layout != Layout.Unrestricted)
            {
                Group.CheckGroupDropAnchors();
                Group.UpdateContentRect();
                //Group.UpdatePanelGroupPanels(Group.panelObjects.sizeDelta);
            }
        }

        public void SetTitleTag(string tag)
        {
            titleTag = tag;
            SetTitle("");
        }

        public void SetTitle(string title)
        {
            string tag = "";
            if (titleTag != "")
            {
                tag = $" {titleTag} - {title}";
            } else
            {
                tag = $" {title}";
            }

            header.panelTitle.text = tag;

        }
    }
}
