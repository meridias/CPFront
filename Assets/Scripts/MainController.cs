namespace onnaMUD
{
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions.Tweens;
    using UnityEngine.UI.Extensions;
    using System.Linq;
    using UnityEditor.PackageManager;
    using UnityEditor;
    //using DynamicPanels;
    using Panels;
    using UnityEngine.InputSystem;

    //using UnityEngine.UIElements;

    //using static SettingsController;

    public class MainController : MonoBehaviour
    {
        public static MainController instance;

        //public ClickableLinks clickable;
        public Camera mainCamera;
        //public GameObject outputWindow;
        //public TMP_InputField textOutput;
        //public TextWindowController mainOutput;
        //public GameObject mainOutputTest;
        public TextMeshProUGUI titleBarText;
        //public Scrollbar mainOutputScrollbar;
        public GameObject optionsWindow;
        public GameObject accountsWindow;
        //public DynamicPanelsCanvas mainDynamicCanvas;
        public PanelsCanvas panelsCanvas;
        public Panel[] outputPanels = new Panel[2];
        //0 is main output
        //1 is room output

        //server list stuff
        public GameObject serversWindow;
        public GameObject serversListScroll;
        public GameObject serversListScrollContent;
        public GameObject serverObjectPrefab;
        public List<Servers> serversList = new List<Servers>();
        public ToggleGroup defaultServerToggles;
        [HideInInspector]
        public Servers connectedServer;

        //new account stuff
        public GameObject newAccountWindow;
        //public GameObject editWindow;
        public TMP_InputField newAccountName;
        public TMP_InputField newPassword1;
        public TMP_InputField newPassword2;
        public GameObject trialConfirmButton;

        //account list stuff
        //public List<Accounts> accounts = new List<Accounts>();
        //public GameObject accountsListObject;
    //    public GameObject accountObjectPrefab;
    //    public ToggleGroup defaultAccountsToggles;
   //     public GameObject accountsScroll;
   //     public GameObject accountsListScrollContent;
   //     private IEnumerator listAccountsCO;
        private IEnumerator listServersCO;

        //public TextMeshProUGUI titleBarText;
        //public Scrollbar mainOutputScrollbar;

        //standard login stuff
        public GameObject loginWindowGO;
        private LoginWindow loginWindow;

//        public ComboBox accountComboBox;//for clearing and adding to the dropdown list
        //public ComboBox serverComboBox;
//        public InputField accountNameInput;//the account name inputfield text box in the combo box
        //public InputField serverNameInput;
        //public TMP_InputField accountPasswordInput;
        //public GameObject comboBoxOverlay;//this is the background/scrollrect for the options in the combobox dropdown, so if we click outside the rect, close it
        //public RectTransform comboBoxItems;//for adding listeners to the objects in the dropdown list for getting their passwords, and setting scroll to specific objects
        //public ScrollRect dropdownScrollRect;//the scrollRect for the combobox dropdown
        //public Toggle savePasswordToggle;
        //private int lastAccountMatch = 0;//this is for matching manually typed account names in the combobox to the names in the accounts list
        //private int lastServerNameMatch = 0;

        public string linkColor = "#959595";//  "#00ccff";
        public string openLink = "<u><#00ccff>";
        public string closeLink = "</color></u>";

        private string gameNameTitle = "Unknown";
        private string charNameTitle = "Nobody";
        private AdminController adminController = null;
        //private InputAction inputControls;
        private CPFrontControls inputControls;

        public Guid characterGuid;
        public List<EditWindows> editWindows = new List<EditWindows>();

        //command bar stuff
        public TMP_InputField commandLine;
        public Slider rtSlider;
        public float rtCounter;
        public bool isInRT = false;

        //bool isDone = false;
        //public TMP_InputField tempOutput;
        //public TMP_InputField anotherTry;

        private void OnEnable()
        {
            inputControls.Enable();
        }

        private void OnDisable()
        {
            inputControls.Disable();
        }

        private void Awake()
        {
            inputControls = new CPFrontControls();
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            instance = this;

            var adminObject = GameObject.Find("AdminController");
            if (adminObject != null)
            {
                adminController = adminObject.GetComponent<AdminController>();
            }

            loginWindow = loginWindowGO.GetComponent<LoginWindow>();
            //Debug.Log(adminController);

            //clickable = outputWindow.GetComponent<ClickableLinks>();
            //spaceWidth = spaceWidthSource.GetPreferredValues().x;
            //textOutput.text = " ";
            //tempOutput.text = "";
            //textOutput = outputWindow.GetComponent<TextMeshProUGUI>();
            //Vector2 textSizes = textOutput.GetPreferredValues();
//            mainOutput.text = "";
            //textOutput.text = "";
            //anotherTest.text = "";

            commandLine.onSubmit.AddListener(SendCommand);

            rtSlider.value = 0;

            UpdateTitleBarText();
            //titleBarText.text = $"{gameNameTitle} - {charNameTitle}";
            //anotherTry.text = "";
            //Debug.Log(textSizes.x);
            //int textWidth = textOutput.
            //textOutput.text = "here we go again <link=\"ARGH\">ARGH</link>......";
            //ShowOutput("testing testing testing <link id=\"doh\">blah</link> moo moo");
            //ShowOutput("blah blah blah <link id=\"testing\">moo?</link> bah");

            //Debug.Log(TMP_TextUtilities.FindIntersectingLink(textOutput, Input.mousePosition, mainCamera));

            //Debug.Log(pos);
            //textOutput.text += $"<style=\"highlightLine\">Blahblah</style><br>";
            //textOutput.text += $"<mark=#ffff00aa>Blahblah<pos=50%></mark><br>";
            //tempOutput.text += $"<mark=#ffff00aa>Blah<pos=99.9%><size=0px>.</size></mark><br>";   //$"<mark=#ffff00aa>Blahblah<pos=50%> </mark><br>";
            //textOutput.text += "Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah ";
            //tempOutput.text += "Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah Blahblah ";

            //           Panel mainOutputPanel = PanelUtils.CreatePanelFor(mainOutput.gameObject.GetComponent<RectTransform>(), mainDynamicCanvas);
            //           mainOutputPanel[0].Label = "Main";
            //           Panel mainTestPanel = PanelUtils.CreatePanelFor(mainOutputTest.GetComponent<RectTransform>(), mainDynamicCanvas);

            //           mainOutputPanel[0].MinSize = new Vector2(300f, 300f);
            //           mainTestPanel[0].MinSize = new Vector2(300f, 300f);

            //           PanelGroup mainPanelGroup = new PanelGroup(mainDynamicCanvas, Direction.Top);
            //           mainPanelGroup.AddElement(mainTestPanel);//first is on bottom
            //           mainPanelGroup.AddElement(mainOutputPanel);//last is on top
            //           mainPanelGroup.DockToRoot(Direction.Bottom);

            //Panel mainOutputPanel = PanelUtils.CreatePanelFor(mainOutput.gameObject.GetComponent<RectTransform>(), mainDynamicCanvas);
            //Panel mainTestPanel = PanelUtils.CreatePanelFor(mainOutputTest.GetComponent<RectTransform>(), mainDynamicCanvas);
            //mainOutputPanel.DockToRoot(Direction.Top);
            //mainTestPanel.DockToRoot(Direction.Top);
            //          mainDynamicCanvas.ForceRebuildLayoutImmediate();//need to turn off/disable dummy panel
            //mainOutputPanel.ResizeTo(new Vector2(mainOutputPanel.Size.x, 400f));
            //mainTestPanel.ResizeTo(new Vector2(mainOutputPanel.Size.x, 400f));
            //          mainPanelGroup.ResizeTo(new Vector2(mainPanelGroup.Size.x, 500f));
            //            Debug.Log(mainOutputPanel.IsDocked);

            yield return new WaitUntil(IsCanvasReady);
            //Test();
            InitializePanelLayout();
        }

        private bool IsCanvasReady()
        {
            return panelsCanvas.isStartDone;
        }

        // Update is called once per frame
        void Update()
        {
            //if ()
            if (Keyboard.current.enterKey.wasPressedThisFrame && !commandLine.isFocused)
            {
                commandLine.ActivateInputField();
 //               Debug.Log("focused");//yeah, this always fires, even when we're already in the command line?
            }
  //          if (Keyboard.current.shiftKey.IsPressed())
  //          {
  //              Debug.Log("shift is pressed");//works
  //          }
  //          if (Input.GetKeyDown(KeyCode.Return) && !commandLine.isFocused)// && !isChatFocused)
  //          {
                //isChatFocused = true;
  //              commandLine.ActivateInputField();
  //              Debug.Log("focused");
                //inputField.ActivateInputField();
  //          }
            //         else if (Input.GetKeyDown(KeyCode.Return))// && isChatFocused)
            //         {
            //isChatFocused = false;

            //inputField.DeactivateInputField();
            //         }

            if (isInRT)// && rtCounter >= 0)
            {
                rtCounter -= Time.deltaTime;

                //round the float up to the nearest int
                int rtSliderCount = (int)Math.Ceiling(rtCounter);
                if (rtSliderCount >= rtSlider.maxValue)
                {
                    //if the count int is the same or higher than the slider max value, max the slider
                    rtSlider.value = rtSlider.maxValue;
                } else
                {
                    //if the count int is less than the slider max, set the slider to the count int
                    rtSlider.value = rtSliderCount;
                }

                if (rtCounter <= 0)
                {
                    rtCounter = 0;
                    rtSlider.value = 0;
                    isInRT = false;
                }

            }

            /*   if (!isDone)
               {
                   if (textOutput.textInfo.linkCount == 0)
                   {
                       Debug.Log(textOutput.textInfo.linkCount);
                   } else
                   {
                       Debug.Log(textOutput.textInfo.linkCount);
                       isDone = true;
                   }
               }*/
        }

        public void SendCommand(string typedCommand)
        {
            //Debug.Log(typedCommand);
            if (ConnectionController.instance.isConnected && !String.IsNullOrEmpty(typedCommand))//isConnected
            {
                //SendData(client, "moo", "100", typedCommand);
                ConnectionController.instance.SendData("100", typedCommand);
                ShowOutput("main", typedCommand, false);//this will get shown by way of server 'echo'ing it back, no?
            }
            //else if (!isConnected)
            // {
            //   mainController.ShowOutput("Server not responding!");
            // }
            commandLine.text = "";
        }

  /*      public void KeepScrollAtBottom(string window, string text)
        {
            switch (window)
            {
                case "main":
                    if (mainOutputScrollbar.size == 1f)
                    {
                        //meaning, we haven't gotten more than 1 screen worth of text yet
                        textOutput.text += text;
                        mainOutputScrollbar.value = 1f;
                    } else
                    {
                        if (mainOutputScrollbar.value >= .98)
                        {
                            textOutput.text += text;
                            mainOutputScrollbar.value = 1f;
                        }
                        else
                        {
                            textOutput.text += text;
                        }
                    }
                    break;
            }
            //testing.text = text;
            //anotherTest.text += text;
        }*/

 /*       public void Test()
        {
 //           DynamicPanels.Panel mainOutputPanel = DynamicPanels.PanelUtils.CreatePanelFor(mainOutput.gameObject.GetComponent<RectTransform>(), mainDynamicCanvas);
            mainOutputPanel[0].Label = "Main";
 //           DynamicPanels.Panel mainTestPanel = DynamicPanels.PanelUtils.CreatePanelFor(mainOutputTest.GetComponent<RectTransform>(), mainDynamicCanvas);

            mainOutputPanel[0].MinSize = new Vector2(300f, 300f);
            mainTestPanel[0].MinSize = new Vector2(300f, 300f);

 //           DynamicPanels.PanelGroup mainPanelGroup = new DynamicPanels.PanelGroup(mainDynamicCanvas, DynamicPanels.Direction.Top);
            mainPanelGroup.AddElement(mainTestPanel);//first is on bottom
            mainPanelGroup.AddElement(mainOutputPanel);//last is on top
            mainPanelGroup.DockToRoot(DynamicPanels.Direction.Top);

            //Panel mainOutputPanel = PanelUtils.CreatePanelFor(mainOutput.gameObject.GetComponent<RectTransform>(), mainDynamicCanvas);
            //Panel mainTestPanel = PanelUtils.CreatePanelFor(mainOutputTest.GetComponent<RectTransform>(), mainDynamicCanvas);
            //mainOutputPanel.DockToRoot(Direction.Top);
            //mainTestPanel.DockToRoot(Direction.Top);
  //          mainDynamicCanvas.ForceRebuildLayoutImmediate();//need to turn off/disable dummy panel
            //mainOutputPanel.ResizeTo(new Vector2(mainOutputPanel.Size.x, 400f));
            //mainTestPanel.ResizeTo(new Vector2(mainOutputPanel.Size.x, 400f));
            mainPanelGroup.ResizeTo(new Vector2(mainPanelGroup.Size.x, 500f));
  //          for (int i = 0; i < mainDynamicCanvas.transform.childCount; i++)
  //          {
  //              if (mainDynamicCanvas.transform.GetChild(i).gameObject.name == "DummyPanel" || mainDynamicCanvas.transform.GetChild(i).gameObject.name == "DynamicPanel")
  //              {
  //                  Debug.Log(mainDynamicCanvas.transform.GetChild(i).gameObject.GetComponent<Panel>().Group);

//                }
 //           }
            //Debug.Log()
        }*/

        public void InitializePanelLayout()
        {
            PanelGroup mainGroup = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Main, Layout.Unrestricted);
            //testing with 2 on each side
            PanelGroup leftGroup1 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Left, Layout.Down);
            PanelGroup rightGroup1 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Right, Layout.Down);
            //top
            PanelGroup topGroup1 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Top, Layout.Right);
            //Panels.PanelUtils.AddPanelGroup(panelsCanvas, Panels.Direction.Top, Layout.Right);
            //bottom
            PanelGroup bottomGroup1 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Bottom, Layout.Right);
            //Panels.PanelUtils.AddPanelGroup(panelsCanvas, Panels.Direction.Bottom, Layout.Right);
            //left
            //Panels.PanelUtils.AddPanelGroup(panelsCanvas, Panels.Direction.Left, Layout.Down);
            PanelGroup leftGroup2 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Left, Layout.Down);
            //right
            //Panels.PanelUtils.AddPanelGroup(panelsCanvas, Panels.Direction.Right, Layout.Down);
            PanelGroup rightGroup2 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Right, Layout.Down);

            //testing different layouts
            PanelGroup topGroup2 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Top, Layout.Right);
            PanelGroup bottomGroup2 = PanelUtils.AddPanelGroup(panelsCanvas, Direction.Bottom, Layout.Right);
            //main panelgroup
            //Panels.PanelUtils.AddPanelGroup(panelsCanvas, Panels.Direction.None, Layout.Unrestricted);



            Panel mainPanel = panelsCanvas.NewPanel();// panelsCanvas.NewPanel(leftGroup1);
            mainPanel.SetTitleTag("Main");
            mainPanel.AddPanelToGroup(mainGroup);
            mainPanel.SetContentInPanel("text");
            outputPanels[0] = mainPanel;

            Panel roomPanel = panelsCanvas.NewPanel();
            roomPanel.SetTitleTag("Room");
            roomPanel.AddPanelToGroup(mainGroup);
            roomPanel.SetContentInPanel("textreplace");
            outputPanels[1] = roomPanel;

            //mainOutput = (TextWindowController)mainPanel.output;
            Panel test3Panel = panelsCanvas.NewPanel();
            test3Panel.AddPanelToGroup(leftGroup1);
            test3Panel.SetContentInPanel("text");
            test3Panel.SetTitle("Testing1");

            Panel test1Panel = panelsCanvas.NewPanel();
            test1Panel.AddPanelToGroup(bottomGroup1);
            test1Panel.SetContentInPanel("text");
            Panel test2Panel = panelsCanvas.NewPanel();
            test2Panel.AddPanelToGroup(rightGroup2);
            test2Panel.SetContentInPanel("text");
            Panel test4Panel = panelsCanvas.NewPanel();
            test4Panel.AddPanelToGroup();
            test4Panel.SetContentInPanel("text");
            Panel test5Panel = panelsCanvas.NewPanel();
            test5Panel.AddPanelToGroup(leftGroup1);
            test5Panel.SetContentInPanel ("text");
            test5Panel.SetTitle("Blahblah");

            /*Panel test6Panel = panelsCanvas.NewPanel();
            test6Panel.SetContentInPanel("text");
            test6Panel.AddPanelToGroup();
            test6Panel.SetTitle("Blahbibity");
            test6Panel.isDockable = false;*/
            //         Panels.Panel mainOutputPanel = UnityEngine.Object.Instantiate(Resources.Load<Panels.Panel>("WindowGroup"), canvas.RectTransform, false);
            //         TextWindowController mainOutput = UnityEngine.Object.Instantiate(Resources.Load<TextWindowController>("WindowGroup"), canvas.RectTransform, false);
            //         leftGroup1.NewPanelInGroup()
            leftGroup1.SetSizeDelta(new Vector2(100f, 0f));

            roomPanel.SetSizeDelta(new Vector2(mainGroup.RectTransform.sizeDelta.x, 400f));
            roomPanel.SetPanelPos(new Vector2(0f, 0f));
            //roomPanel.output.replaceText = true;//this is done with SetContent textreplace now
            mainPanel.SetSizeDelta(new Vector2(mainGroup.RectTransform.sizeDelta.x, 400f));
            mainPanel.SetPanelPos(new Vector2(0f, 400f));
            //mainPanel.RectTransform.sizeDelta = new Vector2(mainGroup.RectTransform.sizeDelta.x, 150f);
            //test4Panel.RectTransform.sizeDelta = new Vector2(150f, 150f);
            //test5Panel.RectTransform.sizeDelta = new Vector2(150f, 150f);
        }

        public void ShowRoomName(string roomName)
        {
            //KeepScrollAtBottom("main", $"<mark=#959595aa>{roomName}<pos=99.9%><size=0px>.</size></mark>");
            //testing.text = $"<mark=#959595aa>{roomName}<pos=99.9%><size=0px>.</size></mark>";
/*            mainOutput.text = $"<mark=#959595aa>{roomName}<pos=99.9%><size=0px>.</size></mark>";*/
            //textOutput.text += $"<mark=#959595aa>{roomName}<pos=99.9%><size=0px>.</size></mark><br>";

            //tempOutput.text += $"<mark=#ffff00aa>{roomName}<pos=99.9%><size=0px>.</size></mark><br>";
            //anotherTry.text += $"<mark=#ffff00aa>{roomName}<pos=99.9%><size=0px>.</size></mark><br>";
        }

        public void ShowOutput(string output, string outputToShow, bool addBreak = true)
        {
            //Debug.Log("boo");
            //            bool noLink = false;
            //            int startIndex = 0;
            //            string linkText = "";
            //check message for highlighting

            switch (output.ToLower())
            {
                case "main":
                    if (outputPanels[0] != null)
                    {
                        if (outputToShow.StartsWith("<br>["))
                        {
                            //right now, we're assuming that the only text coming from the server that starts with <br>[ is the room name
                            string roomName = outputToShow.Substring(0, outputToShow.IndexOf("]") + 1);
                            //Debug.Log(roomName);
                            string newName = roomName.Replace("<br>[", "<mark=#959595aa><br>[");
                            newName = newName.Replace("]", "]<pos=99.9%><size=0px>.</size></mark>");
                            //outputToShow = outputToShow.Replace("<br>[", $"<mark=#959595aa><br>[");

                            outputToShow = outputToShow.Replace(roomName, newName);
                        }
                        outputToShow = outputToShow.Replace("<link", $"{openLink}<link");
                        outputToShow = outputToShow.Replace("</link>", $"{closeLink}</link>");

                        if (Application.isPlaying)
                        {
                            //  testing.text = outputToShow;
                            outputPanels[0].input = outputToShow;
                        }
                    }
                    //if 0 is null, don't send it to any other panel
                    break;
                case "room":
                    if (outputPanels[1] != null)
                    {
                        if (outputToShow.StartsWith("["))
                        {
                            //right now, we're assuming that the only text coming from the server that starts with [ is the room name
                            string roomName = outputToShow.Substring(0, outputToShow.IndexOf("]") + 1);
                            //Debug.Log(roomName);
                         //   string newName = roomName.Replace("<br>[", "<mark=#959595aa><br>[");
                         //   newName = newName.Replace("]", "]<pos=99.9%><size=0px>.</size></mark>");
                            //outputToShow = outputToShow.Replace("<br>[", $"<mark=#959595aa><br>[");

                            //outputToShow = outputToShow.Replace(roomName, newName);
                            outputPanels[1].SetTitle(roomName);
                        }
                        outputToShow = outputToShow.Replace("<link", $"{openLink}<link");
                        outputToShow = outputToShow.Replace("</link>", $"{closeLink}</link>");
                        outputToShow = outputToShow.Substring(outputToShow.IndexOf("]") + 5);//so we ignore the following <br> too

                        if (Application.isPlaying)
                        {
                            //  testing.text = outputToShow;
                            outputPanels[1].input = outputToShow;
                        }


                    }
                    //if 1 is null, don't send it to any other panel

                    break;

            }


     /*       if (outputToShow.StartsWith("<br>["))
            {
                //right now, we're assuming that the only text coming from the server that starts with <br>[ is the room name
                string roomName = outputToShow.Substring(0, outputToShow.IndexOf("]")+1);
                //Debug.Log(roomName);
                string newName = roomName.Replace("<br>[", "<mark=#959595aa><br>[");
                newName = newName.Replace("]", "]<pos=99.9%><size=0px>.</size></mark>");
                //outputToShow = outputToShow.Replace("<br>[", $"<mark=#959595aa><br>[");

                outputToShow = outputToShow.Replace(roomName, newName);
            }*/


       //     outputToShow = outputToShow.Replace("<link", $"{openLink}<link");
       //     outputToShow = outputToShow.Replace("</link>", $"{closeLink}</link>");

            //            while (!noLink)
            //            {
            //                if (outputToShow.IndexOf("<link", startIndex) > -1)
            //               {//if this message has a link tag
            //get index of <link>
            //Debug.Log($"has link tag! {outputToShow}");
            //Debug.Log(outputToShow);
            //                    int linkStart = outputToShow.IndexOf("<link");//index of the start of the link
            //int endOfStart = outputToShow.IndexOf("\">", linkStart);//index of the end of the open link
            //int idLength = (endOfStart - 1) - (linkStart + 7);//length of the link id
            //Debug.Log(linkStart);
            //get index of the end of the opening link tag
            //int closeStart = outputToShow.IndexOf(">", linkStart);
            //Debug.Log(closeStart);
            //                    int linkEnd = outputToShow.IndexOf("</link>", linkStart);//index of the close link
            //                    outputToShow = outputToShow.Insert(linkEnd + 7, "</color></u>");//  </style>");
            //                    outputToShow = outputToShow.Insert(linkStart, $"<u><{linkColor}>");//  style=\"Link\">");//<color={linkColor}>");
            //int idLength = (linkEnd - 2) - (linkStart + 7);
            //                    int linkLength = (linkEnd + 7) - linkStart;
            //string openLink = outputToShow.Substring(linkStart, linkEnd - linkStart);
            //string closeLink = outputToShow.Substring(linkEnd, 7);
            //string linkID = outputToShow.Substring(linkStart + 7, idLength);

            //                    string fullLink = outputToShow.Substring(linkStart, linkLength);
            //Debug.Log(fullLink);
            //                    string highlightLink = $"{openLink}{fullLink}{closeLink}";
            //Debug.Log(highlightLink);
            //string replaceText = $"{openLink}<u><color={linkColor}>{clickable.availableLinks[i].clickableText}</color></u>{closeLink}";
            //clickable.availableLinks[i].highlightedText = replaceText;
            //outputToShow = outputToShow.Replace($"{openLink}{closeLink}", replaceText);// $"{openLink}<u><color={linkColor}>{linkText}</color></u>{closeLink}");
            //                    outputToShow = outputToShow.Replace(fullLink, highlightLink);
            //for some reason, unity is choking on the $"<u><color={linkColor}>" bit?
            //Debug.Log(tempBlah);
            //Debug.Log(openLink);
            //Debug.Log(linkID);
            //Debug.Log(closeLink);

            /*      if (clickable.availableLinks.Count > 0)
                  {
                      for (int i = 0; i < clickable.availableLinks.Count; i++)
                      {
                          if (linkID == clickable.availableLinks[i].ID)
                          {
                              //linkText = clickable.availableLinks[i].clickableText;
                              string replaceText = $"{openLink}<u><color={linkColor}>{clickable.availableLinks[i].clickableText}</color></u>{closeLink}";
                              clickable.availableLinks[i].highlightedText = replaceText;
                              outputToShow = outputToShow.Replace($"{openLink}{closeLink}", replaceText);// $"{openLink}<u><color={linkColor}>{linkText}</color></u>{closeLink}");
                          }
                      }
                  }*/

            //Debug.Log("still checking?");
            //Debug.Log(outputToShow);
            //                    startIndex = linkEnd;
            //                }
            //                else
            //                {
            //Debug.Log("no link tag "+outputToShow);//means that we're past the first match of <link and didn't find a next one after that
            //                    noLink = true;
            //                }
            //           }

            //Debug.Log(outputToShow);
            //check output for Roundtime text to set RT timer under commandline
            //Debug.Log(outputToShow);
            /*            if (outputToShow.StartsWith("<br>Roundtime:"))
                        {
                            string roundtimeText = "<br>Roundtime: ";
                            int rtTextLength = roundtimeText.Length;
                            int secEnd = outputToShow.IndexOf(" sec.");
                            //Debug.Log(rtTextLength);
                            //Debug.Log(outputToShow.Substring(rtTextLength, secEnd - rtTextLength));
                            int lengthOfRT =  Convert.ToInt16(outputToShow.Substring(rtTextLength, secEnd - rtTextLength));
                            rtSlider.value = lengthOfRT;
                            //Debug.Log(lengthOfRT);
                        }
                        */

            //          if (addBreak)
            //          {
            //if addBreak is true (the default), add the <br> at the beginning of the text for a new line
            //              outputToShow = $"<br>{outputToShow}";
            //Debug.Log(outputToShow);
            //          } //if !addBreak, then do nothing and add the text to the end of the current line (mainly for showing entered command next to >)

            //KeepScrollAtBottom("main", $"{outputToShow}");

 //           if (Application.isPlaying)
 //           {
              //  testing.text = outputToShow;
 //               mainOutput.text = outputToShow;
 //           }

            //testing.text = outputToShow;
            //textOutput.text += $"{outputToShow}<br>";


            //tempOutput.text += $"{outputToShow}<br>";
            //anotherTry.text += $"{outputToShow}<br>";
            //return;
        }

        public void RemoveLinks()//not sure if we need this. since we're just going to send clicked links to the server to let it handle it, the links themselves can just stay?
        {//no, gonna need this, pretty sure
   //         if (clickable.availableLinks.Count > 0)
   //         {
   //             for (int i = 0; i < clickable.availableLinks.Count; i++)
   //             {
   //                 mainOutput.text = mainOutput.text.Replace(clickable.availableLinks[i].highlightedText, clickable.availableLinks[i].clickableText);
                    //textOutput.text = textOutput.text.Replace(clickable.availableLinks[i].highlightedText, clickable.availableLinks[i].clickableText);
   //             }
   //         }
        }

        public void NewAccountButtons(string whichButton)
        {
            switch (whichButton)
            {
                case "cancel":
                    ConnectionController.instance.SendData("105", "clicklink trialnewcancel");
                    break;
                case "confirm":
                case "verify":
                    if (string.IsNullOrWhiteSpace(newAccountName.text) || string.IsNullOrEmpty(newAccountName.text))//  newAccountName.text.Length == 0)
                    {
                        newAccountName.text = " ";
                    }
                    string newAccountInfo = newAccountName.text + "::" + newPassword1.text + "::" + newPassword2.text;
                    if (whichButton == "verify")
                    {
                        ConnectionController.instance.SendData("105", $"clicklink trialverify {newAccountInfo}");
                    }
                    else
                    {
                        ConnectionController.instance.SendData("105", $"clicklink trialconfirm {newAccountInfo}");
                    }
                    //ConnectionController.instance.SendData("055", newAccountInfo);

                    break;
                //case "confirm":


                  //  break;

            }


        }

        public void SendNewAccountInfo()
        {
            if (string.IsNullOrWhiteSpace(newAccountName.text) || string.IsNullOrEmpty(newAccountName.text))//  newAccountName.text.Length == 0)
            {
                newAccountName.text = " ";
            }
            string newAccountInfo = newAccountName.text + "::"+ newPassword1.text + "::"+ newPassword2.text;
            ConnectionController.instance.SendData("055", newAccountInfo);
        }

        public void UpdateTitleBarText()
        {
            titleBarText.text = $"{gameNameTitle} - {charNameTitle}";
            //Debug.Log(gameNameTitle);
        }

        public void IsAccountNameInList(string accountName)
        {
            bool newAccount = true;
            Accounts tempAccount = new Accounts();
            //check account name if is in list already
            if (connectedServer.accounts.Count > 0 && accountName.Length > 0)
            {
                //if there are account names in the list to check against AND the input account name is not blank
                for (int i = 0; i < connectedServer.accounts.Count; i++)
                {
                    if (connectedServer.accounts[i].accountName == accountName)
                    {//we found a match so this is not a new account name
                        newAccount = false;
                        tempAccount = connectedServer.accounts[i];
                        break;
                    }
                }
            }

            if (newAccount)
            {
                //if there was no match in the previous IF, then it's a new account we need to add to the list
                //Accounts tempAccount = new Accounts();
                tempAccount.accountName = accountName;
                if (loginWindow.savePasswordToggle)
                {
                    tempAccount.accountPassword = loginWindow.accountPasswordInput.text;
                }
                else
                {
                    tempAccount.accountPassword = "";
                }
                connectedServer.accounts.Add(tempAccount);

                //put the account names in a list, sort it, then sort the accounts based on the sorted list
                //                List<string> tempNames = new List<string>();
                //               for (int i = 0; i<SettingsController.instance.accounts.Count; i++)
                //               {
                //                  tempNames.Add(SettingsController.instance.accounts[i].accountName);
                //             }
                //             tempNames.Sort();

                connectedServer.accounts = connectedServer.accounts.OrderBy(x => x.accountName).ToList();

                //SettingsController.instance.SaveAccountsFile();
               // SettingsController.instance.SaveServersFile();
            } else
            {
                //account name exists in server list already, so we'll just save the password just in case
                tempAccount.accountPassword = loginWindow.accountPasswordInput.text;
            }
            SettingsController.instance.SaveServersFile();
        }

        public void CloseServerWindow()
        {
            //since the server variable on the ServerListObject is a direct reference, we need to make sure that we null that variable before removing it from the serversList
            if (serversListScrollContent.transform.childCount > 0)
            {
                //update servers from server objects
                for (int i = 0; i < serversListScrollContent.transform.childCount; i++)
                {
                    //tempServer is the info we're getting from the gui boxes, fyi
                    ServerListObject temp = serversListScrollContent.transform.GetChild(i).GetComponent<ServerListObject>();
                    temp.ExpandAccountList(false);
                    Servers tempServer = temp.GetServer();//this actually updates the server object referenced from serversList so this is all we really need to do
                }

                //go backwards through the server objects (and their accounts, backwards) and remove any that have been tagged
                for (int j = serversListScrollContent.transform.childCount - 1; j >= 0; j--)
                {
                    ServerListObject tempServer = serversListScrollContent.transform.GetChild(j).GetComponent<ServerListObject>();
                    for (int m = tempServer.accountsListObject.transform.childCount - 1; m >= 0; m--)
                    {
                        AccountListObject tempAccount = tempServer.accountsListObject.transform.GetChild(m).GetComponent<AccountListObject>();
                        if (tempAccount.removeAccountToggle.isOn)
                        {
                            tempServer.RemoveAccount(m);
                        }
                    }
                    for (int n = serversList.Count -1; n >= 0; n--)
                    {
                        if (serversList[n] == tempServer.server && tempServer.removeServerToggle.isOn)
                        {
                            tempServer.RemoveServer();
                            serversList.RemoveAt(n);
                        }
                    }
                }
            }
            SettingsController.instance.SaveServersFile();
        }

        public Guid NewServerGuid()
        {
            Guid newGuid = Guid.NewGuid();
            bool usedGuid = true;

            while (usedGuid)
            {
                if (serversList.Count > 0)
                {
                    for (int i = 0; i < serversList.Count; i++)
                    {
                        if (serversList[i].serverGuid == newGuid)
                        {
                            //guid is being used
                            newGuid = Guid.NewGuid();
                            usedGuid = true;//just in case
                            break;
                        }
                        usedGuid = false;
                    }
                }
                return newGuid;
            }
            return newGuid;
        }

        public void ListServers(bool redraw)
        {
            listServersCO = ListServersCO(redraw);
            StartCoroutine(listServersCO);
        }

        public IEnumerator ListServersCO(bool redraw)
        {
            if (!redraw)
            {
                //redraw is for if we're redrawing the already open window or not, so right now this is opening the window
                serversWindow.SetActive(true);
            }

            if (serversList.Count > 0)
            {
                serversListScroll.SetActive(true);
                yield return new WaitForEndOfFrame();
                //serversListScrollContent.SetActive(false);
                int numOfServerObjects = serversListScrollContent.transform.childCount;

                for (int i = 0; i < serversList.Count; i++)
                {
                    //check the number of already created objects against how many servers we have in the list
                    if (numOfServerObjects < i + 1)
                    {
                        //i + 1, (0+1 is the first server, 1+1 is the second server, etc.)
                        //so if num of objects is less than the server number, make a new one
                        Instantiate(serverObjectPrefab, serversListScrollContent.transform);
                    }

                    //serversListScrollContent.transform.GetChild(i).GetComponent<ServerListObject>().SetServer(serversList[i]);
                    ServerListObject tempServer = serversListScrollContent.transform.GetChild(i).GetComponent<ServerListObject>();
                    tempServer.SetServer(serversList[i]);//this sets all the gui objects text to the server info

                    //Debug.Log(i);
                    if (!redraw)//just in case
                        tempServer.ExpandAccountList(false);
                    //tempServer.accountsListObject.SetActive(false);
                }
                if (numOfServerObjects > serversList.Count)
                {
                    //if somehow there are more account objects than accounts, let's turn the rest of them off
                    for (int j = serversList.Count; j < numOfServerObjects; j++)
                    {
                        //I THINK I have the variables right?
                        //accountsListObject.transform.GetChild(j).gameObject.SetActive(false);
                        serversListScrollContent.transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
                //serversListScroll.SetActive(true);
                //serversListScrollContent.SetActive(true);
            }
            else
            {
                serversListScroll.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
            //yield return null;
            AdjustServerWindow();

            yield return null;
            //Debug.Log(scrollContent.y);
            listServersCO = null;
            SettingsController.instance.SaveServersFile();
        }

        public void AdjustServerWindow()
        {
            RectTransform scrollContent = serversListScrollContent.GetComponent<RectTransform>();//get the size of the full content object with all the accounts in it
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);
            //Debug.Log($"{scrollContent.rect.height}:{scrollContent.sizeDelta.y}");
            //Debug.Log(scrollContent.);
            if (scrollContent.sizeDelta.y >= 350)
            {
                serversListScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollContent.sizeDelta.x, 350f);
            }
            else
            {
                serversListScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollContent.sizeDelta.x, scrollContent.sizeDelta.y);
            }
        }

        public void AddNewServer()
        {
            Servers newServer = new Servers();
            newServer.name = CheckNewServerName(newServer.name);
            newServer.serverGuid = NewServerGuid();
            serversList.Add(newServer);
            ListServers(true);
        }

        public void UpdateServer(Servers updatedServer)
        {
            for (int i = 0; i < serversList.Count; i++)
            {
                if (serversList[i].serverGuid == updatedServer.serverGuid)
                {
                    serversList[i] = updatedServer;
                }
            }
        }

        //checking names against list of objects in the server window, NOT the serversList
        public string CheckNewServerName(string newName)
        {
            bool match = false;
            int nameInc = 0;

            while (!match)
            {
                for (int i = 0; i < serversListScrollContent.transform.childCount; i++)
                {
                    ServerListObject tempServer = serversListScrollContent.transform.GetChild(i).GetComponent<ServerListObject>();

                    if (nameInc == 0)
                    {
                        if (tempServer.serverName.text == newName)
                        {
                            //nameInc++;
                            match = true;
                        }
                    }
                    else
                    {
                        if (tempServer.serverName.text.Contains($"{newName}({nameInc})"))
                        {
                            //nameInc++;
                            match = true;
                        }
                    }
                }
                if (match)
                {
                    match = false;
                    nameInc++;
                } else
                {
                    if (nameInc > 0)
                    {
                        newName += $"({nameInc})";
                    }
                    match = true;
                }
            }
            //Debug.Log(newName);
            return newName;
        }

        public string StripRichTextTags(string input)
        {
            string output = input;

            // room name line highlight tags: "<mark=#959595aa>{roomName}<pos=99.9%><size=0px>.</size></mark>"

            output = output.Replace("<pos=99.9%><size=0px>.</size>", "");

            output = output.Replace("<br>", Environment.NewLine);


            return output;
        }

        public void ProcessMessage(TcpClient client, string fullMessage)
        {
            bool didAdminProcessing = false;
            //index 0 is message code 
            //index 1 is message



            //if the admin stuff is there, do the admin processmessage first for any admin codes 
            //if not, or we did them in admin, continue on with the normal codes
            if (adminController != null)
            {
                didAdminProcessing = AdminController.instance.ProcessMessage(client, fullMessage);
            }

            //Debug.Log(message);
            //string code = "0";
            string[] delimiter = { "::" };
            string[] splitMessage = fullMessage.Split(delimiter, StringSplitOptions.None);

            string code = splitMessage[0];
            string message = splitMessage[1];

            //Debug.Log(code);
            switch (code)
            {
                case "000":
                    //just a check from the server to see if this client is still connected or not, ignore it
                    break;
                case "052":
                    //stuff we're getting from server: current player guid, logged in account name, selected character, game name for title bar, etc
                    switch (message.ToLower())//"guid", "account"name, or "character"name
                    {
                        case "guid":
                            //we connected and the server sent the connection guid so we send back our account name and password to attempt login
                            //Debug.Log(fullMessage);
                            characterGuid = new Guid(splitMessage[2]);//this sets the guid to the string
                            if (loginWindow.accountNameInput.text.Length == 0)
                            {
                                //we're sending a blank accountName. new trial account?
                                //SendData( " ", "050", " ");
                                ConnectionController.instance.SendData(client, "050", " :: ");//send a blank space for accountName and password
                            }
                            else
                            {
                                ConnectionController.instance.SendData(client, "050", $"{loginWindow.accountNameInput.text}::{loginWindow.accountPasswordInput.text}");
                                //SendData($"{accountNameInput.text}", "050", $"{accountPasswordInput.text}");
                            }
                            break;
                        case "account":
                            //we've connected and logged in with/created new account and we're getting it back so we can add it into our account list if we need to

                            IsAccountNameInList(splitMessage[2]);//  message);
                            break;
                        case "character":
                            //we chose a current/created new character and getting the name back so we can put it up in the title bar of the frontend?
                            charNameTitle = splitMessage[2];
                            UpdateTitleBarText();
                            break;
                        case "gamename":
                            //get the name of the game (not the server names: live, test, etc... the game name: Onna:IDLE) for the title bar
                            //Debug.Log("we got the game name");
                            gameNameTitle = splitMessage[2];
                            UpdateTitleBarText();
                            break;
                        case "roundtime":
                            float lengthOfRT = float.Parse(splitMessage[2]);//  Convert.ToInt16(splitMessage[2]);

                            if (lengthOfRT > 0)
                            {
                                rtCounter += lengthOfRT;
                                isInRT = true;

                            }
                            else
                            {
                                if (rtCounter <= 0)
                                {
                                    //incoming RT is 0
                                    //isInRT = false;
                                }
                            }

                            break;
                        case "newaccount":
                            switch (splitMessage[2].ToLower())
                            {
                                case "open":
                                    newAccountWindow.SetActive(true);
                                    break;
                                case "close":
                                    newAccountWindow.SetActive(false);
                                    break;
                            }
                            break;
                        case "trialconfirm":
                            switch (splitMessage[2].ToLower())
                            {
                                case "open":
                                    trialConfirmButton.SetActive(true);
                                    break;
                                case "close":
                                    trialConfirmButton.SetActive(false);
                                    break;
                            }
                            break;
                        //  case "ready":
                        //      KeepScrollAtBottom("main", ">");
                        //      break;
                        default:
                            //Debug.Log($"we got something wrong? ({message})");
                            break;
                    }

                    break;
                case "054"://not used anymore?
                    //open or close the new account window
                    if (message == "open")
                    {
                        newAccountWindow.SetActive(true);
                    }
                    else
                    {
                        newAccountWindow.SetActive(false);
                    }
                    break;
                case "060":
                    if (splitMessage.Length < 3)//not used anymore
                    {
                        //didn't get a splitMessage[2] so we don't know if we're opening or closing a window
                        return;
                    }
                    switch (message.ToLower())
                    {
                        case "newaccount":
                            switch (splitMessage[2].ToLower())
                            {
                                case "open":
                                    newAccountWindow.SetActive(true);
                                    break;
                                case "close":
                                    newAccountWindow.SetActive(false);
                                    break;
                            }
                            break;
                        case "trialconfirm":
                            switch (splitMessage[2].ToLower())
                            {
                                case "open":
                                    trialConfirmButton.SetActive(true);
                                    break;
                                case "close":
                                    trialConfirmButton.SetActive(false);
                                    break;


                            }
                            break;
                    }
                    break;
                case "090":
                    //add clickable link to list
                    string[] clickLink = message.Split(',');
                    //await MainController.instance.clickable.AddLinkToList(clickLink[0], clickLink[1]);
                    break;
                case "092":
                    RemoveLinks();
                    break;
                case "110":
                    //standard window output
                    ShowOutput("main", message);
                    break;
                case "111":
                    //whole line highlight for room name
                    ShowOutput("room", message);
                    //ShowRoomName(message);
                    break;
                case "120"://edit window layout
                    //let's get the type of thing we're editing and its ID to check if we have a window open for it already
                    int layoutTypeIndex = message.IndexOf(":");
                    if (layoutTypeIndex > -1)
                    {
                        Panel editPanel = null;
                        //we found the first :, which hopefully is what type of thing we're editing
                        string editType = message.Substring(0, layoutTypeIndex);
                        //ShowOutput("main", "<br>" + editType);
                        int thingIndex = message.IndexOf(",", layoutTypeIndex);
                        string editThing = message.Substring(layoutTypeIndex + 1, thingIndex - layoutTypeIndex - 1);
                        //ShowOutput("main", "<br>" + editThing);
                        Guid.TryParse(editThing, out Guid thingGuid);
                        bool foundMatch = false;

                        for (int i = 0; i < editWindows.Count; i++)
                        {
                            if (editWindows[i].editType == editType && editWindows[i].editID == thingGuid)
                            {
                                //found a match to type and guid in an open edit window
                                editPanel = editWindows[i].editPanel;
                                foundMatch = true;
                                break;
                            }
                        }
                        if (!foundMatch)
                        {
                            //open a new edit window
                            EditWindows tempWindow = new EditWindows();
                            editPanel = panelsCanvas.NewPanel();
                            editPanel.SetContentInPanel("edit");
                            editPanel.AddPanelToGroup();
                            editPanel.SetTitle($"Edit {editType}");
                            editPanel.isDockable = false;
                            editPanel.SetSizeDelta(new Vector2(350f, 400f));
                            tempWindow.editPanel = editPanel;
                            tempWindow.editType = editType;
                            tempWindow.editID = thingGuid;
                            editWindows.Add(tempWindow);
                        }

                        //now that we have a panel, send the layout to it
                        if (editPanel != null)
                        {
                            //now we need to get the EditWindowController so we can send the layout to it
                            //and save the panel.input for the actual json data
                            EditWindowController editOutput = editPanel.output.GetComponent<EditWindowController>();
                            if (editOutput != null)
                            {
                                editOutput.GetLayoutString(message);
                                //editPanel.output.Input = message;
                            }
                        }

                    }

                    break;
                case "121"://edit window json string
                    //Debug.Log(message);
                    int jsonTypeIndex = message.IndexOf(":");
                    if (jsonTypeIndex > -1)
                    {
                        Panel editPanel = null;
                        //we found the first :, which hopefully is what type of thing we're editing
                        string editType = message.Substring(0, jsonTypeIndex);
                        //ShowOutput("main", "<br>" + editType);
                        int thingIndex = message.IndexOf(",", jsonTypeIndex);
                        string editThing = message.Substring(jsonTypeIndex + 1, thingIndex - jsonTypeIndex - 1);
                        //ShowOutput("main", "<br>" + editThing);
                        Guid.TryParse(editThing, out Guid thingGuid);
                        bool foundMatch = false;

                        for (int i = 0; i < editWindows.Count; i++)
                        {
                            if (editWindows[i].editType == editType && editWindows[i].editID == thingGuid)
                            {
                                //found a match to type and guid in an open edit window
                                editPanel = editWindows[i].editPanel;
                                foundMatch = true;
                                break;
                            }
                        }
                        if (foundMatch)
                        {
                            //we found the edit window panel for this json
                            editPanel.input = message.Substring(thingIndex + 1);
                        }
                    }
                    break;

                case "10000":
                    //Debug.Log("blah?");
                    ShowOutput("main", message);
                    ConnectionController.instance.connectTokenSource.Cancel();
                    //DisconnectFromServer();
                    break;
                default:
                    if (!didAdminProcessing)
                    {
                        //if we didn't do admin processing of this code, then it's an unknown
                        Debug.Log($"Unknown code: {code}");
                    }
                    //Debug.Log($"Unknown code: {code}");
                    break;
            }

        }

        [Serializable]
        public class Servers
        {
            public Guid serverGuid = Guid.Empty;//some guid just so we have something unique to match against if we're changing the server name
            public string name = "New Server";
            public string serverIPDomain = "localhost";
            public int port = 0;
            public bool defaultServer = false;
            public List<Accounts> accounts = new List<Accounts>();
        }


        [Serializable]
        public class Accounts
        {
            public string accountName;
            public string accountPassword;
            public bool defaultAccount = false;


        }

        public class EditWindows
        {
            public string editType = "";//"room", "character", etc
            public Guid editID = Guid.Empty;//Guid of whatever we're editing
            public Panel editPanel;
        }

    }
}
