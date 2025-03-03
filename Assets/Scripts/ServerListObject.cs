using onnaMUD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerListObject : MonoBehaviour
{
    //public Guid serverGuid;
    public Toggle removeServerToggle;
    public TMP_InputField serverName;
    public TMP_InputField ipDomain;
    public TMP_InputField port;
    public Toggle defaultServerToggle;

    public GameObject accountsListObject;
    public GameObject accountObjectPrefab;
    public GameObject addAccountButton;
    public ToggleGroup defaultAccountToggles;
    public GameObject collapseButton;
    public GameObject expandButton;

    //[SerializeField]
    public MainController.Servers server { get; private set; }//apparently, as I set this to the server in the serversList, it's a reference so changes here show there. oi.
    //private IEnumerator expandAccountListCO;
    //private LayoutElement accountListLayout;

    // Start is called before the first frame update
    void Start()
    {
        //accountListLayout = accountsListObject.GetComponent<LayoutElement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Set the object GUI info from the sent server variable
    /// </summary>
    /// <param name="server"></param>
    public void SetServer(MainController.Servers serverInfo)
    {
        server = serverInfo;
        serverName.text = server.name;
        ipDomain.text = server.serverIPDomain;
        port.text = server.port.ToString();
        defaultServerToggle.isOn = server.defaultServer;

        //set the account objects here from server.accounts
        int numOfAccountObjects = accountsListObject.transform.childCount;
        for (int j = 0; j < server.accounts.Count; j++)
        {
            if (numOfAccountObjects < j + 1)
            {
                Instantiate(accountObjectPrefab, accountsListObject.transform);
            }
            AccountListObject tempAccount = accountsListObject.transform.GetChild(j).GetComponent<AccountListObject>();
            tempAccount.accountName.text = server.accounts[j].accountName;
            tempAccount.accountPassword.text = server.accounts[j].accountPassword;
            tempAccount.defaultAccountToggle.group = defaultAccountToggles;// defaultAccountsToggles;
            tempAccount.defaultAccountToggle.isOn = server.accounts[j].defaultAccount;
        }
        if (numOfAccountObjects > server.accounts.Count)
        {
            for (int k = server.accounts.Count; k < numOfAccountObjects; k++)
            {
                accountsListObject.transform.GetChild(k).gameObject.SetActive(false);
            }
        }


    }

    /// <summary>
    /// Get the server variable from the object GUI info
    /// </summary>
    /// <returns></returns>
    public MainController.Servers GetServer()
    {
        //this updates the serversList server through the server variable reference, so we don't need to 'update' it again on the MainController side?
        //MainController.Servers server = new MainController.Servers();
        server.name = serverName.text;
        server.serverIPDomain = ipDomain.text;
        server.port = Int32.Parse(port.text);
        server.defaultServer = defaultServerToggle.isOn;

        //set the server.accounts list here from the account objects
        if (accountsListObject.transform.childCount > 0)
        {
            server.accounts.Clear();
            //serversList[k].accounts.Clear();

            for (int j = 0; j < accountsListObject.transform.childCount; j++)
            {
                //this is the info we're getting from the objects in the gui
                AccountListObject tempAccount = accountsListObject.transform.GetChild(j).GetComponent<AccountListObject>();

                MainController.Accounts account = new MainController.Accounts();
                account.accountName = tempAccount.accountName.text;
                account.accountPassword = tempAccount.accountPassword.text;
                account.defaultAccount = tempAccount.defaultAccountToggle.isOn;
                server.accounts.Add(account);
            }
        }

        return server;
    }


    public void ExpandAccountList(bool isExpanded)
    {
        //enable/disable arrow buttons as needed
        if (isExpanded)
        {
            //if we are expanded out, turn off expand button and turn on collapse button
            expandButton.gameObject.SetActive(false);
            collapseButton.gameObject.SetActive(true);
        } else
        {
            //we are collapsed down, need to turn on expand button, turn off collapse button
            expandButton.gameObject.SetActive(true);
            collapseButton.gameObject.SetActive(false);
        }

        accountsListObject.SetActive(isExpanded);
        addAccountButton.SetActive(isExpanded);
        //MainController.instance.ListServers(true);
        MainController.instance.AdjustServerWindow();
        //expandAccountListCO = ExpandAccountListCO(isExpanded);
        //StartCoroutine(expandAccountListCO);
        //Debug.Log(isExpanded);
    }

    public IEnumerator ExpandAccountListCO(bool isExpanded)
    {
        accountsListObject.SetActive(isExpanded);
        addAccountButton.SetActive(isExpanded);
 /*       if (isExpanded)
        {
            //Debug.Log("accounts list should be expanded");
            accountsListObject.SetActive(true);
            //accountListLayout.minHeight = 0;
        } else
        {
            //Debug.Log("accounts list should be collapsed down");
            accountsListObject.SetActive(false);
        }*/
        yield return new WaitForEndOfFrame();
        MainController.instance.AdjustServerWindow();
        //expandAccountListCO = null;
        yield return null;

    }

    public void AddNewAccount()
    {
        //get the current server info from this server object, BEFORE adding a new account object
        MainController.Servers tempServer = GetServer();
        //MainController.Accounts newAccount = new MainController.Accounts();
        tempServer.accounts.Add(new MainController.Accounts());//this apparently directly references the server in the serversList so this is being added directly
        //Servers newServer = new Servers();
        //newServer.name = CheckNewServerName(newServer.name);

        //MainController.instance.UpdateServer(tempServer);
        //serversList.Add(newServer);
        MainController.instance.ListServers(true);
    }

    public void RemoveAccount(int accountIndex)
    {
        server.accounts.RemoveAt(accountIndex);
    }

    public void RemoveServer()
    {
        server = null;
    }

}

