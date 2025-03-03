using Newtonsoft.Json;
using onnaMUD;

//using onnaMUD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SettingsController;

public class SettingsController : MonoBehaviour
{
    public static SettingsController instance;

    private string exeDirectory;
    private string jsonConfigFilename = "CPFront.json";
    private string accountFileName = "accounts.json";
    private string serversFileName = "servers.json";
    private string jsonConfigFileFull;
    private string accountFileFull;
    private string serversFilenameFull;

    //private int textSize = 14;
    //private int currentSettingsVersion = 1;

    public static Settings settings = new Settings();
    //public List<Accounts> accounts = new List<Accounts>();

    private string jsonString;
    //private IEnumerator listAccountsCO;

    //public GameObject optionsWindow;
    //public GameObject accountsWindow;

    public GameObject optionsBackground;
    //public TMP_InputField serverIP;
    //public TMP_InputField serverPort;
    public TMP_InputField fontSize;
    public Toggle autoConnect;

    //public GameObject accountsListObject;
    //public GameObject accountObjectPrefab;
    //public ToggleGroup defaultAccountsToggles;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //Debug.Log(settings.serverIPDomain);
        optionsBackground.SetActive(false);
        //check for settings file...
        exeDirectory = Directory.GetCurrentDirectory();
        jsonConfigFileFull = Path.Combine(exeDirectory, jsonConfigFilename);
        accountFileFull = Path.Combine(exeDirectory, accountFileName);
        serversFilenameFull = Path.Combine(exeDirectory, serversFileName);

        //Debug.Log(jsonConfigFileFull);
        //        for (int i = 0; i < 3; i++)
        //        {
        //            Accounts tempAccount = new Accounts();
        //            tempAccount.accountName = "blah" + i.ToString();
        //            accounts.Add(tempAccount);
        //        }
        //        Debug.Log(accounts);

        ReadSettingsFile();
        ReadServersFile();
        //ReadAccountsFile();

  /*      if (autoConnect)
        {
            //if the autoconnect setting is on
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].defaultAccount)
                {
                    //if this account is set for default, then do the connect stuff here


                }
            }
        }*/
        //Debug.Log(Directory.GetParent(Application.dataPath));
        //Debug.Log(Directory.GetCurrentDirectory());
        UpdateSettingsInGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
/*
#nullable enable
    public Accounts? CheckForDefaultAccount()
    {
        if (accounts.Count > 0)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].defaultAccount)
                {
                    return accounts[i];
                }


            }
        }


        return null;
    }
#nullable disable*/

    public void ConvertOldSettings(int oldVersion)
    {
        /*
        //if the version of the loaded json file is the same as the current version, don't do anything
        if (oldVersion == settings.settingsVersion)
            return;

        if (oldVersion > settings.settingsVersion)
        {
            Debug.Log("something got messed up... why do we have a 'oldversion' bigger than our current version?");
            return;
        }
*/
        //parse old version to new version
        switch (oldVersion)
        {
            case 1:
                //changing SettingsV1 to current Settings classes
                break;



        }


    }

    public void ShowConfigVariables()
    {
        //optionsBackground.gameObject.SetActive(!optionsBackground.gameObject.activeSelf);

        //if (optionsBackground.gameObject.activeSelf)
        //{
        //Debug.Log("blah");
        //Debug.Log(settings.serverIPDomain);
        //serverIP.text = settings.serverIPDomain;
        //Debug.Log(serverIP.text);
        //serverPort.text = settings.serverPort.ToString();
        fontSize.text = settings.fontSize.ToString();
        autoConnect.isOn = settings.autoConnectWithAccount;
        //}
    }

 /*   public void ListAccounts()
    {
        listAccountsCO = ListAccountsCO();
        StartCoroutine(listAccountsCO);
    }

    public IEnumerator ListAccountsCO()
    {
        accountsWindow.SetActive(true);
        accountsListObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        if (accounts.Count > 0)
        {

            int numOfAccountObjects = accountsListObject.transform.childCount;
            
            //Debug.Log(numOfAccountObjects);

            //there is at least 1 account in the list
            for (int i = 0; i < accounts.Count; i++)
            {
                //check for already created account object
                if (i+1 <= numOfAccountObjects)
                {
                    //if the count of i is less than or equal to the number of already created objects, don't need to make another one
                    //count means ie: i = 0. count is 0+1 = 1 meaning 0 is the first one. i=1 is the second one, etc.


                } else
                {
                    //if the count of i is more than the number of objects, we need to spawn another one
                    Instantiate(accountObjectPrefab, accountsListObject.transform);
                    //accountsListObject.gameObject.GetComponent<VerticalLayoutGroup>().

                }

                AccountListObject tempAccount = accountsListObject.transform.GetChild(i).GetComponent<AccountListObject>();
                tempAccount.accountName.text = accounts[i].accountName;
                tempAccount.accountPassword.text = accounts[i].accountPassword;
                tempAccount.defaultAccountToggle.group = defaultAccountsToggles;
                tempAccount.defaultAccountToggle.isOn = accounts[i].defaultAccount;

            }

            if (numOfAccountObjects > accounts.Count)
            {
                //if somehow there are more account objects than accounts, let's turn the rest of them off
                for (int j = accounts.Count; j < numOfAccountObjects; j++)
                {
                    //I THINK I have the variables right?
                    accountsListObject.transform.GetChild(j).gameObject.SetActive(false);

                }

            }

        } else
        {
            //no accounts in the list


        }
        accountsListObject.SetActive(true);

        yield return null;
        //accountsWindow.SetActive(true);

    }*/

 /*   public void CloseAccountWindow()
    {
        if (accountsListObject.transform.childCount > 0)
        {
            for (int i = 0; i < accountsListObject.transform.childCount; i++)
            {
                AccountListObject tempAccount = accountsListObject.transform.GetChild(i).GetComponent<AccountListObject>();

                accounts[i].accountName = tempAccount.accountName.text;
                accounts[i].accountPassword = tempAccount.accountPassword.text;
                accounts[i].defaultAccount = tempAccount.defaultAccountToggle.isOn;
            }
        }
        SaveAccountsFile();
    }*/

    public void ClearDefaultAccountToggle()
    {
        //EventSystem.current.currentSelectedGameObject.


    }

    public void SaveConfigVariables()
    {
        //settings.serverIPDomain = serverIP.text;
        //settings.serverPort = int.Parse(serverPort.text);
        settings.fontSize = int.Parse(fontSize.text);
        //SaveJsonFile();
        SaveSettingsFile();
    }

 /*   public void SaveJsonFile()
    {
        jsonString = JsonConvert.SerializeObject(settings);
        File.WriteAllText(jsonConfigFileFull, jsonString);
    }*/

    public void UpdateSettingsInGame()
    {
        //ConnectionController.instance.SetServerIP(settings.serverIPDomain);
        //ConnectionController.instance.SetServerPort(settings.serverPort);

//        MainController.instance.mainOutput.textSelectObject.textObject.fontSize = settings.fontSize;//  fontSize = settings.fontSize;

        //once we've read in the settings/accounts files, and updated any in-game settings from the settings file (ie: font size, etc)
        //then check if we're supposed to auto-connect
        if (autoConnect)
        {
            //if the autoconnect setting is on
            for (int i = 0; i < MainController.instance.serversList.Count; i++)
            {
   //             if (MainController.instance.accounts[i].defaultAccount)
   //             {
                    //if this account is set for default, then do the connect stuff here


   //             }
            }
        }


        //SaveConfigVariables();
    }

    public void ReadSettingsFile()
    {
        if (File.Exists(jsonConfigFileFull))// exeDirectory + "/CPFront.json"))
        {
            //WriteAllText creates the file if it isn't already there
            //read from json file and get the settings version number from json
            jsonString = File.ReadAllText(jsonConfigFileFull);
            settings = JsonConvert.DeserializeObject<Settings>(jsonString);//converting the json file info into the settings variable?

            //convert from old to new version
            //Debug.Log("config json file found!");
        }
        else
        {
            //file not found, create
            //File.WriteAllText(jsonConfigFileFull, "");// exeDirectory + "/CPFront.json", "");
            //Debug.Log("config json file not found. Creating...");
            //insert default settings for current settings version...
            jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(jsonConfigFileFull, jsonString);
            //Debug.Log(settings.serverIPDomain);
        }

    }

    public void SaveSettingsFile()
    {
        jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(jsonConfigFileFull, jsonString);

 /*       if (File.Exists(jsonConfigFileFull))
        {
            jsonString = JsonConvert.SerializeObject(settings);
            File.WriteAllText(jsonConfigFileFull, jsonString);

        } else
        {

        }*/

    }

 /*   public void ReadAccountsFile()
    {
        //check for accounts file...
        if (File.Exists(accountFileFull))// exeDirectory + "/settings.dll"))
        {
            //read from accounts binary data file
            //don't bother with binary, just json it and if add passwords later, do encryption
            jsonString = File.ReadAllText(accountFileFull);
            //Debug.Log(jsonString);
            MainController.instance.accounts = JsonConvert.DeserializeObject<List<MainController.Accounts>>(jsonString);
        }
        else
        {
            //figure out how to make a binary file and make it here, nah
            //File.WriteAllText(accountFileFull, "");
            jsonString = JsonConvert.SerializeObject(MainController.instance.accounts, Formatting.Indented);
            File.WriteAllText(accountFileFull, jsonString);
        }

    }*/

    public void ReadServersFile()
    {
        if (File.Exists(serversFilenameFull))
        {
            jsonString = File.ReadAllText(serversFilenameFull);
            MainController.instance.serversList = JsonConvert.DeserializeObject<List<MainController.Servers>>(jsonString);
        } else
        {
            jsonString = JsonConvert.SerializeObject(MainController.instance.serversList, Formatting.Indented);
            File.WriteAllText(serversFilenameFull, jsonString);
        }
    }

    public void SaveServersFile()
    {
        jsonString = JsonConvert.SerializeObject(MainController.instance.serversList, Formatting.Indented);
        File.WriteAllText (serversFilenameFull, jsonString);
    }

 //   public void SaveAccountsFile()
 //   {
 //       jsonString = JsonConvert.SerializeObject(MainController.instance.accounts, Formatting.Indented);
 //       File.WriteAllText(accountFileFull, jsonString);
 /*       if (File.Exists(accountFileFull))
        {
            jsonString = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(accountFileFull, jsonString);
        } else
        {

        }*/

 //   }


    //this is the settings that will always be up to date in the client, loaded versions will get parsed into this if they're different?
    //just so we have a base 'Settings' class for setting to the settings variable
    [Serializable]
    public class Settings
    {//defaults until json file gets parsed
        //public int settingsVersion = 1;//when we update this class, increment this int so we know we're using a newer version
        //public string serverIPDomain = "localhost";
        //public int serverPort = 11000;
        public int fontSize = 14;
        public bool autoConnectWithAccount = false;

    }

    //not sure if I need this, we'll see
    /*
    //this is for loading from json and such
    [Serializable]
    public class SettingsV1
    {
        public int settingsVersion = 1;
        public string serverIPDomain = "localhost";
        public int serverPort = 11000;
        public int fontSize = 14;
    }
    */

 /*   [Serializable]
    public class Accounts
    {
        public string accountName;
        public string accountPassword;
        public bool defaultAccount = false;


    }*/

}
