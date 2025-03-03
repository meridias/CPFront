using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using onnaMUD;

public class LoginWindow : MonoBehaviour
{
    public ComboBox serverComboBox;
    public InputField serverNameInput;
    public GameObject serverOverlay;//this is the background/scrollrect for the options in the combobox dropdown, so if we click outside the rect, close it
    public ScrollRect serverDropdownScrollRect;//the scrollRect for the combobox dropdown
    public RectTransform serverItems;//for adding listeners to the objects in the dropdown list for getting their passwords, and setting scroll to specific objects
    public ComboBox accountComboBox;//for clearing and adding to the dropdown list
    public InputField accountNameInput;//the account name inputfield text box in the combo box
    public GameObject accountOverlay;//this is the background/scrollrect for the options in the combobox dropdown, so if we click outside the rect, close it
    public ScrollRect accountDropdownScrollRect;//the scrollRect for the combobox dropdown
    public RectTransform accountItems;//for adding listeners to the objects in the dropdown list for getting their passwords, and setting scroll to specific objects

    public TMP_InputField accountPasswordInput;
    public Toggle savePasswordToggle;

    private MainController.Servers matchedServer = null;
    private int lastServerNameMatch = 0;
    private int lastAccountMatch = 0;//this is for matching manually typed account names in the combobox to the names in the accounts list

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetConnectedServer()
    {
        MainController.instance.connectedServer = matchedServer;
    }

    public void OpenLoginWindow()
    {
        List<string> serverNames = new List<string>();

        MainController.Servers defaultServer = CheckForDefaultServer();

        for (int i = 0; i < MainController.instance.serversList.Count; i++)
        {
            serverNames.Add(MainController.instance.serversList[i].name);
        }
        serverNames.Sort();
        serverComboBox.SetAvailableOptions(serverNames);

        for (int j = 0; j < serverComboBox.AvailableOptions.Count; j++)//  serverItems.childCount; j++)//add our listener to the dropdown objects so we can set the account passwords to the inputfield
        {
            Button serverButton = serverItems.GetChild(j).gameObject.GetComponent<Button>();
            Guid serverGuidToFind = Guid.Empty;
            //Debug.Log(serverComboBox.AvailableOptions[j]);
            for (int k = 0; k < MainController.instance.serversList.Count; k++)
            {
                //Debug.Log(MainController.instance.serversList[k].name);
                if (MainController.instance.serversList[k].name == serverComboBox.AvailableOptions[j])
                {
                    //Debug.Log("match?");
                    serverGuidToFind = MainController.instance.serversList[j].serverGuid;
                    break;
                }
            }
            //Debug.Log(serverGuidToFind);
            //Guid serverGuidToFind = MainController.instance.serversList[j].serverGuid;
            //Debug.Log(accountNameToFind);
            if (serverGuidToFind != Guid.Empty)
            {
                serverButton.onClick.AddListener(() =>
                {
                    //Debug.Log(serverGuidToFind);
                    GetAccountsForServer(serverGuidToFind);
                });
            }
        }

        //gameObject.SetActive(true);

        if (defaultServer != null)
        {
            matchedServer = defaultServer;
            serverNameInput.text = defaultServer.name;
            ConnectionController.instance.SetServerConnectInfo(defaultServer.serverIPDomain, defaultServer.port);
            MainController.Accounts defaultAccount = CheckForDefaultAccount(defaultServer);
            if (defaultAccount != null)
            {
                accountNameInput.text = defaultAccount.accountName;
                accountPasswordInput.text = defaultAccount.accountPassword;
            }
            if (serverOverlay.activeSelf)
            {
                serverComboBox.ToggleDropdownPanel(false);
            }
        }
    }


    public void MatchServer()
    {
        if (serverComboBox.AvailableOptions.Count == 0)
            return;

        switch (serverNameInput.text.Length)
        {
            case 0:
                lastServerNameMatch = 0;
                break;
            case 1:
                bool foundMatch = false;
                string tempString = serverNameInput.text.ToLower();
                char firstChar = tempString[0];
                while (!foundMatch)
                {
                    if (firstChar.Equals('a'))
                    {
                        //if we ended up back at the start of the alphabet, give up and set index of match to 0
                        lastServerNameMatch = 0;
                        foundMatch = true;
                    }
                    else
                    {
                        for (int i = 0; i < serverComboBox.AvailableOptions.Count; i++)
                        {
                            if (serverComboBox.AvailableOptions[i].StartsWith(firstChar.ToString(), true, null))
                            {
                                //we found a match starting with the first char
                                lastServerNameMatch = i;
                                foundMatch = true;
                                break;
                            }
                        }
                        //didn't find a match with this character, step back a character in the alphabet and try again
                        firstChar = (char)(firstChar - 1);
                    }
                }
                break;
            case > 1:
                for (int i = 0; i < serverComboBox.AvailableOptions.Count; i++)
                {
                    if (serverComboBox.AvailableOptions[i].StartsWith(serverNameInput.text, true, null))
                    {
                        //if we remove all the characters from the inputfield, it basically becomes blank and so matches the first option? ie, i = 0
                        lastServerNameMatch = i;
                        break;
                    }
                }
                break;
        }
        //NEED TO CHECK AND REDO THE 'COMBOBOXITEMS' AND 'DROPDOWNSCROLLRECT' OBJECTS FOR THIS METHOD
        //set the scroll view to the matched account index in the list
        float scrollValue;
        if (lastServerNameMatch == 0)
        {
            //if the match is the first index, set the scroll to the top
            scrollValue = 1;
        }
        else
        {
            if (serverComboBox.AvailableOptions.Count - lastServerNameMatch <= 4)
            {
                //if the match is 1 of the last 4 indexes, set the scroll to the bottom
                scrollValue = 0;
            }
            else
            {
                //else find the index in the list and set the scroll to that spot
                RectTransform matchedItemRT = serverItems.GetChild(lastServerNameMatch).gameObject.GetComponent<RectTransform>();
                scrollValue = 1 + matchedItemRT.anchoredPosition.y / serverDropdownScrollRect.content.rect.height;
            }
        }

        serverDropdownScrollRect.verticalScrollbar.value = scrollValue;
    }

    public void MatchAccount()//this is where we match what is typed in the login combo and look for matches in the combobox list
    {
        //int lastMatch = 0;
        if (accountComboBox.AvailableOptions.Count == 0)
            return;

        switch (accountNameInput.text.Length)
        {
            case 0:
                //empty input field, just set list to top
                lastAccountMatch = 0;
                break;
            case 1:
                bool foundMatch = false;
                string tempString = accountNameInput.text.ToLower();
                char firstChar = tempString[0];
                //check for the first character in the list, if no match find the next match going backward in the alphabet til find one?
                while (!foundMatch)
                {
                    if (firstChar.Equals('a'))
                    {
                        //if we ended up back at the start of the alphabet, give up and set index of match to 0
                        lastAccountMatch = 0;
                        foundMatch = true;
                    }
                    else
                    {
                        for (int i = 0; i < accountComboBox.AvailableOptions.Count; i++)
                        {
                            if (accountComboBox.AvailableOptions[i].StartsWith(firstChar.ToString(), true, null))
                            {
                                //we found a match starting with the first char
                                lastAccountMatch = i;
                                foundMatch = true;
                                break;
                            }
                        }
                        //didn't find a match with this character, step back a character in the alphabet and try again
                        firstChar = (char)(firstChar - 1);
                    }
                }
                break;
            case > 1:
                //check for matching account name, starting from the character match from 'case 1'
                for (int i = 0; i < accountComboBox.AvailableOptions.Count; i++)
                {
                    if (accountComboBox.AvailableOptions[i].StartsWith(accountNameInput.text, true, null))
                    {
                        //if we remove all the characters from the inputfield, it basically becomes blank and so matches the first option? ie, i = 0
                        lastAccountMatch = i;
                        break;
                    }
                }
                break;
        }

        //set the scroll view to the matched account index in the list
        float scrollValue;
        if (lastAccountMatch == 0)
        {
            //if the match is the first index, set the scroll to the top
            scrollValue = 1;
        }
        else
        {
            if (accountComboBox.AvailableOptions.Count - lastAccountMatch <= 4)
            {
                //if the match is 1 of the last 4 indexes, set the scroll to the bottom
                scrollValue = 0;
            }
            else
            {
                //else find the index in the list and set the scroll to that spot
                RectTransform matchedItemRT = accountItems.GetChild(lastAccountMatch).gameObject.GetComponent<RectTransform>();
                scrollValue = 1 + matchedItemRT.anchoredPosition.y / accountDropdownScrollRect.content.rect.height;
            }
        }

        accountDropdownScrollRect.verticalScrollbar.value = scrollValue;
    }

    public void GetAccountsForServer(Guid serverGuid)
    {
        //Debug.Log(serverGuid);
        //MainController.Servers matchedServer = null;
        //go through the servers, find the one we clicked on in the list, update the name in the dropdown
        //and set the account stuff to that servers' default account, if it has one
        for (int i = 0; i < MainController.instance.serversList.Count; i++)
        {
            if (MainController.instance.serversList[i].serverGuid == serverGuid)
            {
                //Debug.Log(MainController.instance.serversList[i].name);
                matchedServer = MainController.instance.serversList[i];
                serverNameInput.text = MainController.instance.serversList[i].name;
                ConnectionController.instance.SetServerConnectInfo(MainController.instance.serversList[i].serverIPDomain, MainController.instance.serversList[i].port);
            }
        }

        //if we clicked on a server from the dropdown, populate the account name list dropdown
        if (matchedServer != null)
        {
            List<string> serverAccounts = new List<string>();
            for (int j = 0; j < matchedServer.accounts.Count; j++)
            {
                serverAccounts.Add(matchedServer.accounts[j].accountName);
                if (matchedServer.accounts[j].defaultAccount)
                {
                    accountNameInput.text = matchedServer.accounts[j].accountName;
                    accountPasswordInput.text = matchedServer.accounts[j].accountPassword;
                    //break;
                }
            }

            serverAccounts.Sort();
            accountComboBox.SetAvailableOptions(serverAccounts);

            for (int k = 0; k < accountItems.childCount; k++)//add our listener to the dropdown objects so we can set the account passwords to the inputfield
            {
                Button accountButton = accountItems.GetChild(k).gameObject.GetComponent<Button>();

                string accountNameToFind = serverAccounts[k];
                //Debug.Log(accountNameToFind);
                accountButton.onClick.AddListener(() =>
                {
                    GetAccountPassword(accountNameToFind);
                    //OnItemClicked(textOfItem);
                });
            }
        }
    }

    public void GetAccountPassword(string accountName)
    {
        //Debug.Log(accountName);
        //if (SettingsController.instance.accounts.Count == 0)// accountComboBox.AvailableOptions.Count == 0)
        //return;// "";
        if (matchedServer != null)
        {
            for (int i = 0; i < matchedServer.accounts.Count; i++)
            {
                if (accountName == matchedServer.accounts[i].accountName)
                {
                    accountPasswordInput.text = matchedServer.accounts[i].accountPassword;
                    //return SettingsController.instance.accounts[i].accountPassword;
                }

            }
        }
        //return "";
    }

    public MainController.Servers? CheckForDefaultServer()
    {
        if (MainController.instance.serversList.Count > 0)
        {
            for (int i = 0; i < MainController.instance.serversList.Count; i++)
            {
                if (MainController.instance.serversList[i].defaultServer)
                    return MainController.instance.serversList[i];
            }
        }
        return null;
    }

    public MainController.Accounts? CheckForDefaultAccount(MainController.Servers server)
    {
        if (server.accounts.Count > 0)
        {
            for (int i = 0; i < server.accounts.Count; i++)
            {
                if (server.accounts[i].defaultAccount)
                {
                    return server.accounts[i];
                }
            }
        }
        return null;
    }
}
