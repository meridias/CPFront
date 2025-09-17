namespace onnaMUD
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Unity.VisualScripting;
    using System.IO;
    using TMPro;
    using Newtonsoft.Json.Linq;
    using System.Threading;
    using UnityEngine.UI.Extensions;
    using UnityEngine.UI;
    using System.Linq;
    using Newtonsoft.Json;

    public class ConnectionController : MonoBehaviour
    {
        public static ConnectionController instance;

//        public static int lastAccountMatch = 0;
        //public delegate Task ConnectToServer(CancellationToken cancelToken);
        //ConnectToServer serverConnect;
        //public delegate Task TempDele();
        //public TempDele tempTry;

        private Task tryingToConnect;

        //private IPHostEntry host;// = Dns.GetHostEntry("localhost");
        //private IPAddress ipAddress;// = host.AddressList[0];
        //private int port;
        //private IPEndPoint remoteEP;// = new IPEndPoint(ipAddress, 11000);
        //private Socket sender;
        private TcpClient client;
        //private NetworkStream clientStream;
        //private IEnumerator serverConnectCO;
        //private Guid characterGuid;

        private string serverIPDomain = "localhost";
        private int serverPort = 0;

        //private byte[] bytes;// = new byte[numOfBytes];
        //private string receivedDataBuffer = "";
        private int numOfBytes = 1024;

        public CancellationTokenSource connectTokenSource;
        //private CancellationToken tryConnectToken;

        public MainController mainController;
        //public TMP_InputField commandLine;
        //public TMP_InputField serverIP;
        //public TMP_InputField serverPort;

        //public bool readyToReceive = false;
        public bool isConnecting = false;
        public bool isConnected = false;
        public GameObject connectButton;
        public GameObject disconnectButton;

//        public ComboBox accountComboBox;//for clearing and adding to the dropdown list
//        public InputField accountNameInput;//the account name inputfield text box in the combo box
//        public TMP_InputField accountPasswordInput;
//        public Toggle savePasswordToggle;
//        public RectTransform comboBoxItems;//so we can get rid of old objects since comboBox doesn't do it and new ones keep being added
//        public GameObject comboBoxOverlay;
//        public ScrollRect dropdownScrollRect;//the scrollRect for the combobox dropdown

//        public GameObject loginWindow;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            
            //commandLine.onEndEdit.AddListener(SendCommand);
//            commandLine.onSubmit.AddListener(SendCommand);
            //serverIP.onSubmit.AddListener(SetServerIP);
            //serverPort.onSubmit.AddListener(SetServerPort);

            //bytes = new byte[numOfBytes];
            //host = Dns.GetHostEntry("localhost");
            //IPAddress tempIPAddress = host.AddressList[0];
            //port = 11000;
            //ipAddress = IPAddress.Parse("127.0.0.1");
            //remoteEP = new IPEndPoint(ipAddress, 11000);
            //remoteEP = new IPEndPoint(tempIPAddress, port);
            //client = new TcpClient(AddressFamily.InterNetworkV6);// new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Debug.Log("testing start?");
            //clientStream = client.GetStream();
            //            if (ClientLogin())
            //           {
            //             Debug.Log("moo");
            //clientStream = client.GetStream();
            //SendData(clientStream, "We're logged in!");
            //         }
            /*          switch (tempipAddress.AddressFamily)
                      {
                          case AddressFamily.InterNetwork:
                              Debug.Log("IPv4");
                              break;
                          case AddressFamily.InterNetworkV6:
                              Debug.Log("IPv6");
                              //ipAddress = ipAddress.MapToIPv4();
                              break;


                      }*/
            //Debug.Log(characterGuid);
        }

        // Update is called once per frame
        void Update()
        {
            if (isConnecting || isConnected)
            {
                //if we're connected or trying to be connected
                if (connectButton.activeSelf)
                {//if the connect button is active, turn it off
                    connectButton.SetActive(false);
                }
                if (!disconnectButton.activeSelf)
                {//if the disconnect button is not on, turn it on
                    disconnectButton.SetActive(true);
                }

            } else
            {
                //if we're not currently connected or not currently in the attempt to connect
                if (!connectButton.activeSelf)
                {//if the connect button is not on, turn it on
                    connectButton.SetActive(true);
                }
                if (disconnectButton.activeSelf)
                {//if the disconnect button is on, turn it off
                    disconnectButton.SetActive(false);
                }
            }

     /*       if (receivedDataBuffer.Length > 0)
            {
                int eofIndex = 0;

                if (receivedDataBuffer.IndexOf("::") == 5)//was -1 (index is somewhere in string), now is 5 to make sure :: is in the right place since the index of :: should be 5
                {//if we found the first set of :: means that we've gotten the index int for <EOF>
                    try
                    {
                        eofIndex = Int32.Parse(receivedDataBuffer.Substring(0, 5));//parse the first 5 characters for the index of EOF
                    }
                    catch (FormatException)
                    {
                        //mainController.ShowOutput("Disconnecting...");
                        Debug.Log("error parsing message length int!");
                        //client.Close();
                        return;
                    }

                    if (eofIndex > 0 && receivedDataBuffer.Length >= eofIndex + 4)
                    {//if we go the index correctly and we've got the whole message
                     //first, make sure we're only dealing with JUST the first message, if multiple messages got crammed together
                        string firstMessage = receivedDataBuffer.Substring(0, eofIndex + 5);
                        //strip the message length from the front of the message
                        firstMessage = firstMessage.Substring(7);
                        //strip the <EOF> from the end of the message
                        firstMessage = firstMessage.Remove(firstMessage.IndexOf("<EOF>"));
                        //clear the buffer of the first message
                        receivedDataBuffer = receivedDataBuffer.Remove(0, eofIndex + 5);

                        //before sending the message on to the ProcessMessage block, check if we're receiving a disconnect message from server
                        //      string[] delimiter = { "::" };
                        //    string[] splitMessage = firstMessage.Split(delimiter, StringSplitOptions.None);
                        //  if (splitMessage[1] == "10000")
                        //{
                        //incoming message is a socket disconnect command from server
                        //Debug.Log("disconnect message from server");
                        //     mainController.ShowOutput(splitMessage[2]);
                        //     DisconnectFromServer();
                        //     return;
                        // }
                        //Debug.Log(receivedDataBuffer);
                        //Debug.Log(firstMessage);
                        mainController.ProcessMessage(client, firstMessage);//hopefully this means that it won't continue until we get back from the processing
                                                                   //await ProcessMessage(client, receivedDataBuffer.Substring(0, eofIndex + 5));//index + 5 is length of <EOF>
                                                                   //receivedDataBuffer = receivedDataBuffer.Remove(0, eofIndex + 5);
                    }
          //          else
            //        {
                        //we've gotten the first :: but not a full message so we need to go back and get the rest
              //          checkMessage = false;
                //    }
                }
            }*/


        }

        public void SetServerConnectInfo(string ipDomain, int port)
        {
            serverIPDomain = ipDomain;
            serverPort = port;
        }

        /*       public async Task<bool> ClientLogin()
               {
                   //bool isTryingToConnect = false;
                   int numOfRetries = 10;
                   //byte[] bytes = new byte[1024];

                   //            try
                   //            {
                   // Connect to a Remote server
                   // Get Host IP Address that is used to establish a connection
                   // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                   // If a host has multiple addresses, you will get a list of addresses
                   //IPHostEntry host = Dns.GetHostEntry("localhost");
                   //IPAddress ipAddress = host.AddressList[0];
                   //IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                   // Create a TCP/IP  socket.
                   //Socket sender = new Socket(ipAddress.AddressFamily,
                   //  SocketType.Stream, ProtocolType.Tcp);

                   // Connect the socket to the remote endpoint. Catch any errors.
                   while (!isConnected)
                   {
                       for (int i = 0; i < numOfRetries; i++)
                       {
                           try
                           {
                               Debug.Log("Trying to connect...");
                               mainController.ShowOutput("Trying to connect...");
                               //Debug.Log("connecting");
                               // Connect to Remote EndPoint
                               //                    Debug.Log(remoteEP);
                               //Debug.Log(client.Client.DualMode);
                               //                    Debug.Log(client.Client.AddressFamily);
                               //client.ConnectAsync(remoteEP);
                               await client.ConnectAsync(ipAddress, port);

                               //Debug.Log(sender.RemoteEndPoint.ToString());
                               Debug.Log("Connected to login server!");
                               //Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                               //clientStream = client.GetStream();
                               // Encode the data string into a byte array.


                               //this is just for now
                               //SendData(client, "moo", "100", "This is a test");
                               //SendData("This is a test<EOF>");
                               //byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                               // Send the data through the socket.
                               //int bytesSent = clientStream.Write(msg, 0, numOfBytes);
                               //clientStream.Write(msg);
                               //ReceiveFromServer();
                               // Receive the response from the remote device.
                               //int bytesRec = clientStream.Read(bytes, 0, numOfBytes);

                               //mainController.ShowOutput(string.Format("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec)));
                               //Debug.Log(string.Format("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec)));
                               // Console.WriteLine("Echoed test = {0}",
                               //   Encoding.ASCII.GetString(bytes, 0, bytesRec));
                               //MainController.PrintOutput();

                               // Release the socket.
                               //sender.Shutdown(SocketShutdown.Both);
                               //sender.Close();
                               readyToReceive = true;
                               isConnected = true;
                               ReceiveFromServer();
                               return true;
                           }
                           catch (ArgumentNullException)// ane)
                           {
                               //Debug.Log(string.Format("ArgumentNullException : {0}", ane.ToString()));
                               mainController.ShowOutput("Unable to connect. Retrying...");

                               //return false;
                           }
                           catch (SocketException se)
                           {
                               if (se.ToString().IndexOf("No connection could be made because the target machine actively refused it") > -1)
                               {
                                   //mainController.ShowOutput("Login server is down! Please try again later.");
                                   mainController.ShowOutput("Unable to connect. Is server down? Retrying...");
                                   //Task.Delay(10000);
                               }
                               //Debug.Log(string.Format("SocketException : {0}", se.ToString()));
                               //return false;
                           }
                           catch (Exception)// e)
                           {
                               //Debug.Log(string.Format("Unexpected exception : {0}", e.ToString()));
                               mainController.ShowOutput("Unable to connect. Retrying...");
                               //return false;
                           }

                           if (i < 9)
                           {
                               await Task.Delay(10000);//not sure if this works. some kind of await command for x seconds?
                           } else
                           {
                               //tried to connect 10 times, failed. do this:
                               mainController.ShowOutput("Unable to connect. Please try again later.");
                               return false;
                           }
                       }

                   }
                   return false;
               }*/

        /*       public void SendData(Guid guid, string msgCode, string dataToSend)
               {
                   if (guid == Guid.Empty)// .ToString().Length == 0)
                   {//since we're sending back the guid from the server immediately after connecting, there's no reason why this guid should be empty
                    //no guid set, means we haven't logged into a character yet. so we send the account name if we have one, might be a new trial account with no account name
                    //apparently I forgot that as soon as the client connects to the server, a guid is set. just haven't sent it back to the user yet
                            if (accountNameInput.text.Length == 0)
                            {
                                //we're sending a blank accountName. new trial account?
                                SendData(" ", msgCode, dataToSend);
                            }
                            else
                            {
                                SendData($"{accountNameInput.text}", msgCode, dataToSend);
                            }
                       Debug.Log("bad guid!");
                   } else
                   {
                       string guidString = guid.ToString();
                       SendData(guidString, msgCode, dataToSend);
                   }
               }*/

        public void SendData(string msgCode, string dataToSend)
        {
            SendData(client, msgCode, dataToSend);

        }

        public bool SendData(TcpClient client, string msgCode, string dataToSend)//TcpClient client, string sender, string msgCode, string dataToSend)
        {
            //Debug.Log(dataToSend);
            //we're always going to send what we have for a guid, it'll be up to the server to verify it or not
            NetworkStream clientStream = null;
            //if (isConnected)
            //{
            try
            {
                clientStream = client.GetStream();

            }
            catch (InvalidOperationException)
            {
                //Error in getting the NetworkStream for the client. server down?
                //Debug.Log("clientStream error");
                mainController.ShowOutput("main", "<br>There seems to be an issue with the connection to the server. Please check your internet connection and reconnect.");
                DisconnectFromServer();
                return false;
            }

            int tempIndex = 0;
            string indexString = tempIndex.ToString("D5");

            if (dataToSend.IndexOf("<EOF>") == -1)//dataToSend.IndexOf("::") == -1 && 
            {//if there is no <EOF> tag already in the message, which we shouldn't, then we're good
                //had to remove the dataToSend.IndexOf("::") == -1 part because we're sending the accountname and password like that
                //accountname::password

                //make sure data doesn't contain bad characters, gonna need to work on this
                string msgString = indexString + "::" + mainController.characterGuid + "::" + msgCode + "::" + dataToSend + "<EOF>";
                tempIndex = msgString.IndexOf("<EOF>");
                indexString = tempIndex.ToString("D5");
                msgString = indexString + "::" + mainController.characterGuid + "::" + msgCode + "::" + dataToSend + "<EOF>";
                byte[] msg = Encoding.ASCII.GetBytes(msgString);
                //Debug.Log(msgString);
                try
                {
                    clientStream.Write(msg);
                    //Debug.Log(Encoding.ASCII.GetString(msg));
                }
                catch (IOException)
                {
                    //Error in trying to send data. server down?
                    //Debug.Log("clientStream.Write error");
                    mainController.ShowOutput("main", "<br>There seems to be an issue with the connection to the server. Please check your internet connection and reconnect.");
                    DisconnectFromServer();
                    return false;
                }
            }
            else
            {
                //data contains invalid characters
                return false;

            }
            //}
            return true;
        }

        public void SendCommand(string typedCommand)
        {
            //Debug.Log(typedCommand);
            if (isConnected && !String.IsNullOrEmpty(typedCommand))//isConnected
            {
                //SendData(client, "moo", "100", typedCommand);
                SendData(client, "100", typedCommand);
  //              mainController.ShowOutput(typedCommand, false);
            }
            //else if (!isConnected)
           // {
             //   mainController.ShowOutput("Server not responding!");
           // }
            mainController.commandLine.text = "";
        }

 /*       public void SetServerConnection(string ipToSet, int portToSet)// string portToSet)
        {
            try
            {
                host = Dns.GetHostEntry(ipToSet);
                ipAddress = host.AddressList[0];
                //Debug.Log("doh");
            }
            catch (ArgumentException)
            {
                mainController.ShowOutput("Unable to resolve host address. Please check the address.");
                Debug.Log("blah");
                return;
            }

            port = portToSet;
            try
            {
                port = Int32.Parse(portToSet);
            }
            catch (FormatException)
            {
                mainController.ShowOutput("Unable to resolve host address port. Please check the address.");
                return;
            }
        }*/

 /*       public void SetServerIP(string ipToSet)
        {
            try
            {
                host = Dns.GetHostEntry(ipToSet);
            }
            catch (ArgumentException)
            {
                mainController.ShowOutput("Unable to resolve host address. Please check the address.");
                Debug.Log("blah");
                return;
            }
            ipAddress = host.AddressList[0];
            //Debug.Log(ipAddress);
        }

        public void SetServerPort(string portToSet)
        {
            try
            {
                port = Int32.Parse(portToSet);
            }
            catch (FormatException)
            {
                mainController.ShowOutput("Unable to resolve host address port. Please check the address.");
                return;
            }
            //Debug.Log(port);
        }

        public void SetServerPort(int portToSet)
        {
            port = portToSet;
        }*/

        public void SendDisconnectRequest()//no
        {
            if (!SendData(client, "10000", "blah"))//if this message being sent failed, then manually disconnect
            {
                DisconnectFromServer();
            }
            //SendData(client, "moo", "10000", "blah");
        }

        public void DisconnectButton()
        {
            if (isConnected)
            {//if we're currently actually connected to the server, then warn
                SendData(client, "10000", "blah");
                //mainController.ShowOutput("Warning! Forcing a disconnect from the game will not guarantee that your character will be logged out immediately.");
            }
            if (isConnecting)
            {
                mainController.ShowOutput("main", "<br>Disconnected!");
                DisconnectFromServer();
            }
            //DisconnectFromServer();
        }

        public void DisconnectFromServer()
        {
            //if (isConnected)
           // {
             //   mainController.ShowOutput("Disconnected!");
           // }
            mainController.characterGuid = Guid.Empty;
            //receivedDataBuffer = "";//need to clear this out so it's not still there if we want to reconnect
            //SendData(client, "moo", "10000", "blah");
            //Debug.Log("disconnecting?");
            //need to figure out how to cancel the login attempt
            //tryingToConnect.
            //tryingToConnect.Dispose();
            if (connectTokenSource != null)
            {
                connectTokenSource.Cancel();//send cancellation request to tokens
            }
            //client.GetStream().Close();
            if (isConnecting || isConnected)
            {
                client.Close();
            }
            //mainController.ShowOutput("Disconnected!");
            //Debug.Log(tryConnectSource.IsCancellationRequested);
            //client.Close();
            isConnecting = false;
            isConnected = false;
        }

        //this should be in MainController
 /*       public void LoginAccount()//this brings up the login screen from the 'connect' button
        {
            List<string> tempList = new List<string>();
            //checking for default account?
#nullable enable
            SettingsController.Accounts? defaultAccount = SettingsController.instance.CheckForDefaultAccount();
#nullable disable

            //accountComboBox.AvailableOptions.Clear();//clear the list from previous opening, just in case
            //Debug.Log(accountComboBox.AvailableOptions.Count);
            //if (accountComboBox.AvailableOptions.Count > 0)
            //{
//                accountComboBox.ResetItems();//clear the availableoptions list and empty the dropdown of objects
                //Debug.Log(accountComboBox.AvailableOptions.Count);
            //}
            //Debug.Log(comboBoxItems.childCount);
//            if (comboBoxItems.childCount > 0)
//            {
//                for (int i = comboBoxItems.childCount-1; i >= 0; i--)
//                {//go in reverse order so it doesn't get confused in removing the objects
//                    Button accountButton = comboBoxItems.GetChild(i).gameObject.GetComponent<Button>();
//                    accountButton.onClick.RemoveAllListeners();
//                    Destroy(comboBoxItems.GetChild(i).gameObject);
//                }
//            }

            if (SettingsController.instance.accounts.Count > 0)//get the list of accounts from settings, no matter the order
            {
                for (int i = 0; i < SettingsController.instance.accounts.Count; i++)
                {
                    //put the account names in a temp list
                    tempList.Add(SettingsController.instance.accounts[i].accountName);

                    //accountComboBox.AvailableOptions.Add(SettingsController.instance.accounts[i].accountName);
                    //accountComboBox.AddItem(SettingsController.instance.accounts[i].accountName);
                }
            }
            tempList.Sort();//sort the temp list

            accountComboBox.SetAvailableOptions(tempList);//this method forces the dropdown to have unique objects so we don't need to manually get rid of duplicates

 //           for (int i = 0; i < tempList.Count; i++)
 //           {
                //for all the account names in the sorted list, add them to the combo box
 //               accountComboBox.AddItem(tempList[i]);
 //           }
            //Debug.Log(comboBoxItems.childCount);
            for (int j = 0; j < comboBoxItems.childCount; j++)//add our listener to the dropdown objects so we can set the account passwords to the inputfield
            {
                Button accountButton = comboBoxItems.GetChild(j).gameObject.GetComponent<Button>();
                //Debug.Log(tempList[i]);
                //Debug.Log(accountButton.gameObject.name);
                //Debug.Log(j + " " + comboBoxItems.childCount);
                string accountNameToFind = tempList[j];
                //Debug.Log(accountNameToFind);
                accountButton.onClick.AddListener(() =>
                {
                    //Debug.Log(accountNameToFind);
                    //Debug.Log(gameObject.name);
//                    GetAccountPassword(accountNameToFind);
                    //OnItemClicked(textOfItem);
                });


            }

            loginWindow.SetActive(true);

            if (defaultAccount != null)
            {//if there is a default account
                //if (defaultAccount.accountPassword)
                //put the account info into the boxes
                accountNameInput.text = defaultAccount.accountName;
                accountPasswordInput.text = defaultAccount.accountPassword;
                if (comboBoxOverlay.activeSelf)
                {
                    accountComboBox.ToggleDropdownPanel(false);
                    //Debug.Log("blah");
                }
            }

            //tryingToConnect = Task.Run(ConnectToServer, tryConnectToken);
            //tryingToConnect = Task.Run(ConnectToServer);
            //tryingToConnect = TaskFactory.StartNew(ConnectToServer);
            //tryingToConnect = Task.Factory.StartNew(ConnectToServer(tryConnectToken))
            //Debug.Log(tryConnectToken.IsCancellationRequested);
            //tryingToConnect = Task.Run(() => ConnectToServer(tryConnectToken), tryConnectToken);//this won't ever work because Task.Run uses threadpool, not just main thread and await doesn't like that
            //           tryingToConnect = ConnectToServer(tryConnectSource.Token);//  tryConnectToken);
            //Debug.Log(tryingToConnect.CreationOptions);
            //serverConnectCO = ConnectToServer();
            //StartCoroutine(serverConnectCO);
            //tryingToConnect.
            //Debug.Log("finished");
        }*/

        public void Connect()//this is the login button on the login screen
        {
  /*          bool newAccount = true;
            //check account name if is in list already
            if (SettingsController.instance.accounts.Count > 0 && accountNameInput.text.Length > 0)
            {
                //if there are account names in the list to check against AND the input account name is not blank
                for (int i = 0; i < SettingsController.instance.accounts.Count; i++)
                {
                    if (SettingsController.instance.accounts[i].accountName == accountNameInput.text)
                    {//we found a match so this is not a new account name
                        newAccount = false;
                    }
                }
            }

            if (newAccount)
            {
                //if there was no match in the previous IF, then it's a new account we need to add to the list
                SettingsController.Accounts tempAccount = new SettingsController.Accounts();
                tempAccount.accountName = accountNameInput.text;
                if (savePasswordToggle)
                {
                    tempAccount.accountPassword = accountPasswordInput.text;
                } else
                {
                    tempAccount.accountPassword = "";
                }
                SettingsController.instance.accounts.Add(tempAccount);

                //put the account names in a list, sort it, then sort the accounts based on the sorted list
//                List<string> tempNames = new List<string>();
 //               for (int i = 0; i<SettingsController.instance.accounts.Count; i++)
 //               {
  //                  tempNames.Add(SettingsController.instance.accounts[i].accountName);
   //             }
   //             tempNames.Sort();

                SettingsController.instance.accounts = SettingsController.instance.accounts.OrderBy(x => x.accountName).ToList();

                SettingsController.instance.SaveAccountsFile();
            }
            */

            connectTokenSource = new CancellationTokenSource();
            //tryConnectToken = tryConnectSource.Token;

            //SetServerConnection(SettingsController.settings.serverIPDomain, SettingsController.settings.serverPort);//  serverIP.text, serverPort.text);//set the server connection from the settings values

            client = new TcpClient(AddressFamily.InterNetworkV6);// new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Debug.Log($"{accountNameInput.text}:{accountPasswordInput.text}");
            //tryingToConnect = ConnectToServer(SettingsController.settings.serverIPDomain, SettingsController.settings.serverPort, connectTokenSource.Token, "login");
            if (serverPort != 0)
            {
                tryingToConnect = ConnectToServer(serverIPDomain, serverPort, connectTokenSource.Token);//, "login");
            } else
            {
                mainController.ShowOutput("main", "<br>Invalid port number. Please check your server information.");
            }

            //SendData(client, "moo", "050", $"{accountNameInput.text}:{accountPasswordInput.text}");
        }

        public void Test()
        {
            //Debug.Log(tryingToConnect.Exception);
            Debug.Log(tryingToConnect.Status);


        }

        public async Task ConnectToServer(string server, int port, CancellationToken cancelToken)//, string serverType)
        {
            isConnecting = true;
            //Debug.Log(tryConnectToken);
            if (cancelToken.IsCancellationRequested)// in case we canceled already
                return;

            //isConnected = true;
            //SetServerIP(serverIP.text);
            //SetServerPort(serverPort.text);
            //remoteEP = new IPEndPoint(ipAddress, port);
            //Debug.Log(remoteEP);
            //await ClientLogin();

            int numOfRetries = 10;
            //byte[] bytes = new byte[1024];

            //            try
            //            {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            //IPHostEntry host = Dns.GetHostEntry("localhost");
            //IPAddress ipAddress = host.AddressList[0];
            //IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.
            //Socket sender = new Socket(ipAddress.AddressFamily,
            //  SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            //while (!isConnected)
            //{

            //while loop for the cancellation token?

            //mainController.ShowOutput("Connecting...");
            for (int i = 0; i < numOfRetries; i++)
            {
                //Debug.Log(cancelToken.IsCancellationRequested);
                if (cancelToken.IsCancellationRequested)//  tryConnectToken.IsCancellationRequested)
                {
                    mainController.ShowOutput("main", "<br>Disconnecting...");
                    client.Close();
                    return;
                }

                var taskSource = new TaskCompletionSource<bool>();//?

                try
                {
                    mainController.ShowOutput("main", "<br>Connecting...");//yes we want this to show up before every connection attempt
                    CancellationTokenSource tokenSource = new CancellationTokenSource(5000);//cancel token after 5 seconds
                    //Debug.Log($"Trying to connect...{server},{port}");
                    Task clientConnect = client.ConnectAsync(server, port);
                    //Debug.Log($"Trying to connect...{server},{port}");
                    //                   await client.ConnectAsync(server, port);//  ipAddress, port);//, cancelToken);
                    using (tokenSource.Token.Register(() => taskSource.TrySetResult(true)))//when token is timed out, tries to set taskSource to true
                    {
                        if (clientConnect != await Task.WhenAny(clientConnect, taskSource.Task))
                        {// whenany returns the task that completed first, so if taskSource gets timed out by the tokenSource, then
                            throw new OperationCanceledException(tokenSource.Token);
                        }
                        //if the client timed out itself
                        if (clientConnect.Exception?.InnerException != null)
                        {
                            throw clientConnect.Exception.InnerException;
                        }
                    }
                    //if we connected
                    isConnecting = false;
                    isConnected = true;
                    //Debug.Log("moo?");
                    ReceiveFromServer();
                    //Debug.Log("harhar");
                    return;
                    //return true;
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("timed out, by connection or token");
                    mainController.ShowOutput("main", "<br>Unable to connect.");
                }
 /*               catch (ArgumentNullException)// ane)
                {
                    //Debug.Log(string.Format("ArgumentNullException : {0}", ane.ToString()));
                    mainController.ShowOutput("Unable to connect.blah");
                    //return false;
                }
                catch (SocketException se)
                {
                    //Debug.Log(se.ErrorCode);
                    if (se.ToString().IndexOf("No connection could be made because the target machine actively refused it") > -1)
                    {//this is when the server ip and port aren't even there?
                        //mainController.ShowOutput("Login server is down! Please try again later.");
                        mainController.ShowOutput("Unable to connect. Is server down?");
                        //Task.Delay(10000);
                    }
                    else
                    {
                        mainController.ShowOutput("Unable to connect.");
                    }
                    //Debug.Log(string.Format("SocketException : {0}", se.ToString()));
                    //return false;
                }*/
                catch (Exception e)
                {
                    Debug.Log($"Unexpected exception: {e}");
                    mainController.ShowOutput("main", "<br>Unable to connect.");
                    DisconnectFromServer();
                    //Debug.Log(string.Format("Unexpected exception : {0}", e.ToString()));
                    //client.
                    //Debug.Log(client.Client);
                    //if (client.Client != null)
                   // {
                     //   mainController.ShowOutput("Unable to connect.general");
                   // }
                    if (cancelToken.IsCancellationRequested)// in case we canceled already
                        return;
                    //return false;
                    //Debug.Log("general exception");
                }

                if (i < 9)
                {
                    //Debug.Log("waiting for 10 seconds?");
                    try
                    {
                        //Debug.Log(client.Client);
                        mainController.ShowOutput("main", "<br>Retrying in 10 seconds...");
                        await Task.Delay(10000, cancelToken);//not sure if this works. some kind of await command for x seconds?
                        //Debug.Log("10 seconds are up");
                    }
                    catch (TaskCanceledException)
                    {
                        //Debug.Log("Connection attempt canceled");
                        return;
                    }
                }
                else
                {
                    //tried to connect 10 times, failed. do this:
                    mainController.ShowOutput("main", "<br>Unable to connect. Please try again later.");
                    //client.Close();
                    DisconnectFromServer();
                    //break;
                    return;
                }
            }
        }

        //this should be in MainController
 /*       public void IsAccountNameInList(string accountName)
        {
            bool newAccount = true;
            //check account name if is in list already
            if (SettingsController.instance.accounts.Count > 0 && accountName.Length > 0)
            {
                //if there are account names in the list to check against AND the input account name is not blank
                for (int i = 0; i < SettingsController.instance.accounts.Count; i++)
                {
                    if (SettingsController.instance.accounts[i].accountName == accountName)
                    {//we found a match so this is not a new account name
                        newAccount = false;
                    }
                }
            }

            if (newAccount)
            {
                //if there was no match in the previous IF, then it's a new account we need to add to the list
                SettingsController.Accounts tempAccount = new SettingsController.Accounts();
                tempAccount.accountName = accountName;
                if (savePasswordToggle)
                {
                    tempAccount.accountPassword = accountPasswordInput.text;
                }
                else
                {
                    tempAccount.accountPassword = "";
                }
                SettingsController.instance.accounts.Add(tempAccount);

                //put the account names in a list, sort it, then sort the accounts based on the sorted list
                //                List<string> tempNames = new List<string>();
                //               for (int i = 0; i<SettingsController.instance.accounts.Count; i++)
                //               {
                //                  tempNames.Add(SettingsController.instance.accounts[i].accountName);
                //             }
                //             tempNames.Sort();

                SettingsController.instance.accounts = SettingsController.instance.accounts.OrderBy(x => x.accountName).ToList();

                SettingsController.instance.SaveAccountsFile();
            }


        }*/

        private async void ReceiveFromServer()
        {
            //index 0 is the length of message to EOF index
            //index 1 is message code (was index 2 but we're not needing to send the server guid)
            //index 2 is message

            //still need to put cancelation tokens here when disconnecting
            //Debug.Log("receiving?");
            NetworkStream serverStream = client.GetStream();
            string receivedDataBuffer = "";
        bool checkMessage = false;
        int eofIndex = 0;
        byte[] bytes = new byte[numOfBytes];

            while (true)
            {
                try
                {
                    //byte[] bytes = new byte[numOfBytes];
                    //Debug.Log("waiting");
                    //Debug.Log(Encoding.ASCII.GetString(bytes));
                    int bytesRec = await serverStream.ReadAsync(bytes, 0, numOfBytes, connectTokenSource.Token);
                    //Debug.Log(bytesRec);
                    if (bytesRec > 0)
                    {
                        receivedDataBuffer += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        //Debug.Log(receivedDataBuffer);
                        checkMessage = true;
                    }
                    else
                    {
                        //Debug.Log("socket closed on server side?");
                        mainController.ShowOutput("main", "<br>There seems to be a connection issue from the server. Please try to reconnect.");
                        DisconnectFromServer();
                        return;
                    }
                    /*                   if (receivedDataBuffer.IndexOf("<EOF>") > -1)
                                       {
                                           Debug.Log(receivedDataBuffer);
                                           //SendData(connections[j].clientSocket, "Message received!");
                                           mainController.ShowOutput(receivedDataBuffer);
                                       }
                                       mainController.ShowOutput(string.Format("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec)));
                                       Debug.Log(string.Format("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec)));
                    */
                }
                catch (SocketException e)
                {
                    //Console.WriteLine(i);
                    Debug.Log(e.ToString());
                    DisconnectFromServer();
                    //readyToReceive = false;
                    return;// false;
                           //throw;
                }
                catch (OperationCanceledException)
                {
                    //Debug.Log("connection canceled!");
                    DisconnectFromServer();
                    mainController.ShowOutput("main", "<br>Disconnected!");
                    //readyToReceive = false;
                    return;
                }
                catch (IOException)// ioe)
                {
                    //Debug.Log(ioe.InnerException);
                    //Debug.Log("Server not responding!");
                    DisconnectFromServer();
                    mainController.ShowOutput("main", "<br>Error with communication to server. Disconnected!");
                    //serverStream.Dispose();
                    //client.Close();
                    //readyToReceive = false;
                    return;
                }
                catch (Exception ex)
                {
                    //Debug.Log(ex);
                    DisconnectFromServer();
                    return;
                }

                while (checkMessage)
                {

                    if (receivedDataBuffer.IndexOf("::") == 5)//was -1 (index is somewhere in string), now is 5 to make sure :: is in the right place since the index of :: should be 5
                    {//if we found the first set of :: means that we've gotten the index int for <EOF>
                        try
                        {
                            eofIndex = Int32.Parse(receivedDataBuffer.Substring(0, 5));//parse the first 5 characters for the index of EOF
                        }
                        catch (FormatException)
                        {
                            //mainController.ShowOutput("Disconnecting...");
                            Debug.Log("error parsing message length int!");
                            checkMessage = false;
                            //client.Close();
                            return;
                        }

                        if (eofIndex > 0 && receivedDataBuffer.Length >= eofIndex + 5)
                        {//if we go the index correctly and we've got the whole message
                         //first, make sure we're only dealing with JUST the first message, if multiple messages got crammed together
                            if (eofIndex + 5 > receivedDataBuffer.Length)
                            {
                                Debug.Log(receivedDataBuffer);
                            }
                            string firstMessage = receivedDataBuffer.Substring(0, eofIndex + 5);
                            //strip the message length from the front of the message
                            firstMessage = firstMessage.Substring(7);
                            //strip the <EOF> from the end of the message
                            firstMessage = firstMessage.Remove(firstMessage.IndexOf("<EOF>"));
                            //clear the buffer of the first message
                            receivedDataBuffer = receivedDataBuffer.Remove(0, eofIndex + 5);
                            //eofIndex = 0;
                            //before sending the message on to the ProcessMessage block, check if we're receiving a disconnect message from server
                            //      string[] delimiter = { "::" };
                            //    string[] splitMessage = firstMessage.Split(delimiter, StringSplitOptions.None);
                            //  if (splitMessage[1] == "10000")
                            //{
                            //incoming message is a socket disconnect command from server
                            //Debug.Log("disconnect message from server");
                            //     mainController.ShowOutput(splitMessage[2]);
                            //     DisconnectFromServer();
                            //     return;
                            // }
                            //Debug.Log(receivedDataBuffer);
                            //Debug.Log(firstMessage);
                            mainController.ProcessMessage(client, firstMessage);//hopefully this means that it won't continue until we get back from the processing
                                                                                //await ProcessMessage(client, receivedDataBuffer.Substring(0, eofIndex + 5));//index + 5 is length of <EOF>
                                                                                //receivedDataBuffer = receivedDataBuffer.Remove(0, eofIndex + 5);
                        }
                        else
                        {
                            //we've gotten the first :: but not a full message so we need to go back and get the rest
                            checkMessage = false;
                        }
                    }
                    else
                    {
                        checkMessage = false;
                    }
                }
            }

        }

  /*      public void ProcessMessage(TcpClient client, string fullMessage)
        {
            //index 0 is message code 
            //index 1 is message

            //Debug.Log(message);
            //string code = "0";
            string[] delimiter = { "::"};
            string[] splitMessage = fullMessage.Split(delimiter, StringSplitOptions.None);

            string code = splitMessage[0];
            string message = splitMessage[1];
            
            //Debug.Log(code);
            switch (code)
            {
                case "052":
                    switch (message)//"guid", "account"name, or "character"name
                    {
                        case "guid":
                            //we connected and the server sent the connection guid so we send back our account name and password to attempt login
                            //Debug.Log(fullMessage);
                            characterGuid = new Guid(splitMessage[2]);//this sets the guid to the string
                            if (accountNameInput.text.Length == 0)
                            {
                                //we're sending a blank accountName. new trial account?
                                //SendData( " ", "050", " ");
                                SendData(client, "050", " :: ");//send a blank space for accountName and password
                            }
                            else
                            {
                                SendData(client, "050", $"{accountNameInput.text}::{accountPasswordInput.text}");
                                //SendData($"{accountNameInput.text}", "050", $"{accountPasswordInput.text}");
                            }
                            break;
                        case "account":
                            //we've connected and logged in with/created new account and we're getting it back so we can add it into our account list if we need to
                            IsAccountNameInList(message);
                            break;
                        case "character":
                            //we chose a current/created new character and getting the name back so we can put it up in the title bar of the frontend?

                            break;
                    }

                    break;
                case "054":
                    //open or close the new account window
                    if (message == "open")
                    {
                        MainController.instance.newAccountWindow.SetActive(true);
                    } else
                    {
                        MainController.instance.newAccountWindow.SetActive(false);
                    }
                    break;
                case "060":
                    if (splitMessage.Length < 3)
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
                                    MainController.instance.newAccountWindow.SetActive(true);
                                    break;
                                case "close":
                                    MainController.instance.newAccountWindow.SetActive(false);
                                    break;
                            }
                            break;
                        case "edit":
                            switch (splitMessage[2].ToLower())
                            {
                                case "open":
                                    MainController.instance.editWindow.SetActive(true);
                                    break;
                                case "close":
                                    MainController.instance.editWindow.SetActive(false);
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
                    MainController.instance.RemoveLinks();
                    break;
                case "110":
                    //standard window output
                    mainController.ShowOutput(message);
                    break;
                case "111":
                    //whole line highlight for room name
                    mainController.ShowRoomName(message);
                    break;
                case "120":
                    //getting edit info to put into edit window
                    dynamic editClass = JObject.Parse(message);
                    var output = JsonConvert.SerializeObject(editClass, Formatting.None);
                    Debug.Log(output);

                    mainController.ShowOutput(message);//for now

                    break;

                case "10000":
                    //Debug.Log("blah?");
                    mainController.ShowOutput(message);
                    connectTokenSource.Cancel();
                    //DisconnectFromServer();
                    break;
                default:
                    Debug.Log($"Unknown code: {code}");
                    break;
            }

        }*/

        //this should be in MainController
  /*      public void GetAccountPassword(string accountName)
        {
            //Debug.Log(accountName);
            //if (SettingsController.instance.accounts.Count == 0)// accountComboBox.AvailableOptions.Count == 0)
                //return;// "";

            for (int i = 0; i < SettingsController.instance.accounts.Count; i++)
            {
                if (accountName == SettingsController.instance.accounts[i].accountName)
                {
                    accountPasswordInput.text = SettingsController.instance.accounts[i].accountPassword;
                    //return SettingsController.instance.accounts[i].accountPassword;
                }

            }
            //return "";
        }*/

        //this should be in MainController
 /*       public void MatchAccount()//this is where we match what is typed in the login combo and look for matches in the combobox list
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
                        } else
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
            } else
            {
                if (accountComboBox.AvailableOptions.Count - lastAccountMatch <= 4)
                {
                    //if the match is 1 of the last 4 indexes, set the scroll to the bottom
                    scrollValue = 0;
                } else
                {
                    //else find the index in the list and set the scroll to that spot
                    RectTransform matchedItemRT = comboBoxItems.GetChild(lastAccountMatch).gameObject.GetComponent<RectTransform>();
                    scrollValue = 1 + matchedItemRT.anchoredPosition.y / dropdownScrollRect.content.rect.height;
                }
            }

            dropdownScrollRect.verticalScrollbar.value = scrollValue;
        }*/

        public void OnDisable()
        {
            //since this should never be disabled during normal run-time, this is for when exiting play mode
            //or closing the program? probably

            //canceling all tasks here...
            //tryConnectSource.Cancel();
            //DisconnectFromServer();

        }

        public void OnApplicationQuit()
        {
            DisconnectFromServer();
           // if (connectTokenSource != null)
           // {
             //   connectTokenSource.Cancel();
           // }
            //DisconnectFromServer();
            //Debug.Log("tasks canceled?");
        }

    }
}
