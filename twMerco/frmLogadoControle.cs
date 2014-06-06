using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using FSUIPC;
using MySql;
using MySql.Data.MySqlClient;
using MySql.Data;
using Microsoft.FlightSimulator.SimConnect;
// Add these two statements to all SimConnect clients
//using Microsoft.ESP.SimConnect;
using System.Runtime.InteropServices;
using System.Threading;








namespace twMerco
{
    public partial class frmLogadoControle : Form
    {




        private AITrafficServices AI; // Holds a reference to the AI Traffic Services object
        string AppTitle = "twMerco [PILOTO]";

        
        private MySqlConnection mConn;
        private MySqlDataAdapter mAdapter;
        private DataSet mDataSet;

        


        // User-defined win32 event
        const int WM_USER_SIMCONNECT = 0x0402;

        // SimConnect object
        SimConnect simconnect = null;

        // enumeration of structures that will be set/retrieved from ESP via SimConnect
        enum DEFINITIONS
        {
            PositionData_Definition,    // registered with SimConnect and mapped to the 'PositionData' structure
            AI_Entity_Definition,       // registered with SimConnect and mapped to the 'AI_Entity_Details' structure
        };

        // local client identifier that SimConnect uses for data requests
        // enables the client to differentiate between different sets of requests (if more than one)
        enum DATA_REQUESTS
        {
            REQUEST_AI_ADDED,                   // System event notifying if an AI has been added to the system
            REQUEST_AI_REMOVED,                 // System event notifying if an AI has been removed from the system
            REQUEST_USERAIRCRAFT_DATA,          // Request for User Aircraft information
            REQUEST_AI_OBJECT_ID_BYTYPE,        // Request for ID's of each AI Entity
        };

        // this is how you declare a data structure so that
        // simconnect knows how to fill it/read it.

        // structure used to receive overview object information
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct AI_Entity_Details
        {
            // Sizes of the strings must be of fixed length and will be truncated to the size you specify below
            public bool IsUserSim;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Category;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Model;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Airline;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string FlightNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Title;
        };

        // structure used to receive detailed data for each object
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct PositionData
        {
            public double latitude;
            public double longitude;
            public double altitude;
            public double pitch;
            public double bank;
            public double heading;
            public double headingDegrees;
            public double speed;
            public double nav1Freq;
            public double nav1FreqStby;
            public double nav2Freq;
            public double nav2FreqStby;
            public int transponder;
        };

        // Struct to store details about each AI object in our list/dictionary (below)
        struct AIObjectData
        {
            public uint dwIndex;
            public uint dwObjectID;
            public string itemText;
            public AI_Entity_Details ad;
        };

        // Radius (in meters) of objects we will requests data for
        // 200000 meters is maximum
        const uint MONITOR_RADIUS = 200000;

        // List of AI Objects that are currently being monitored
        Dictionary<string, AIObjectData> AIObjects = new Dictionary<string, AIObjectData>();

        // Keep track of the type of objects we are currently monitoring
        SIMCONNECT_SIMOBJECT_TYPE monitoredObjectType;


        public frmLogadoControle()
        {
            InitializeComponent();
            this.cbxRadarRange.Text = "50";
          //  setButtons(true, false, false);

        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                if (simconnect != null)
                {
                    simconnect.ReceiveMessage();
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void btnConnectar_Click(object sender, EventArgs e)
        {
            try
            {
                // Attempt to open a connection to FSUIPC (running on any version of Flight Sim)
                FSUIPCConnection.Open();
                // Opened OK so disable the Connect button
                this.btnConnectar.Enabled = false;
                this.chkEnableAIRadar.Enabled = true;
                // Start the timer ticking to drive the rest of the application
                this.timer1.Interval = 200;
                this.timer1.Enabled = true;
                // Set the AI object
                AI = FSUIPCConnection.AITrafficServices;
            }
            catch (Exception ex)
            {
                // Badness occurred - show the error message
                MessageBox.Show(ex.Message, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                FSUIPCConnection.Close();
                Debug.WriteLine(ex.Message);
            }

            if (simconnect == null)
            {
                try
                {
                    // the constructor is similar to SimConnect_Open in the native API
                    // the ConfigIndex enables you to choose which Simconnect profile to use (in a SimConnect.cfg) if you are
                    // connecting over a network. Leaving it to zero for a local connection here.
                    simconnect = new SimConnect("C# Monitoring AI Objects", this.Handle, WM_USER_SIMCONNECT, null, 0);

                   // setButtons(false, true, true);

                    initDataRequest();

                }
                catch
                {
                    MessageBox.Show("Make sure Microsoft ESP is running and that you have the SimConnect client installed.", "Unable to connect to ESP");
                }
            }

            // Store the current object type being monitored
            monitoredObjectType = SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT;

            // Refresh the list of monitored objects for this specific request
            // first, Clear any current objects and stop all data requests for those objects
            ResetDataAndShutDownRequests();

            // Now, Initiate a request for the list of objects we are interested in monitoring in the given radius
            simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_AI_OBJECT_ID_BYTYPE,
                DEFINITIONS.AI_Entity_Definition, MONITOR_RADIUS, monitoredObjectType);


        }

        #region SimConnect Handlers
        // Set up all the SimConnect related data definitions and event handlers
        private void initDataRequest()
        {
            try
            {
                // listen to connect and quit msgs
                simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);

                // listen to exceptions
                simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);

                // map the data structure to ESP's Simulation Variables
                // there are more than 200 variables that are available to be queried and/or set via SimConnect
                // define & register PositionData, used for LLAPBH updates on Sim Objects
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Altitude", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Pitch Degrees", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Bank Degrees", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Heading Degrees True", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                    "Ground Velocity", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                   "NAV ACTIVE FREQUENCY:1", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                   "NAV STANDBY FREQUENCY:1", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                  "NAV ACTIVE FREQUENCY:2", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                   "NAV STANDBY FREQUENCY:2", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.PositionData_Definition,
                  "TRANSPONDER CODE:1", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                // IMPORTANT: register it with the simconnect managed wrapper marshaller
                // if you skip this step, you will only receive a uint in the .dwData field.
                simconnect.RegisterDataDefineStruct<PositionData>(DEFINITIONS.PositionData_Definition);

                // define & register AircraftData, used for info about Sim Objects
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "IS USER SIM", "Bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "CATEGORY", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "ATC TYPE", null, SIMCONNECT_DATATYPE.STRING32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "ATC MODEL", null, SIMCONNECT_DATATYPE.STRING32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "ATC ID", null, SIMCONNECT_DATATYPE.STRING32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "ATC AIRLINE", null, SIMCONNECT_DATATYPE.STRING64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "ATC FLIGHT NUMBER", null, SIMCONNECT_DATATYPE.STRING8, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.AI_Entity_Definition,
                    "TITLE", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                simconnect.RegisterDataDefineStruct<AI_Entity_Details>(DEFINITIONS.AI_Entity_Definition);

                // setup various data requests
                simconnect.SubscribeToSystemEvent(DATA_REQUESTS.REQUEST_AI_ADDED, "ObjectAdded");
                simconnect.SubscribeToSystemEvent(DATA_REQUESTS.REQUEST_AI_REMOVED, "ObjectRemoved");

                // enable the notifications of the above system events
                simconnect.SetSystemEventState(DATA_REQUESTS.REQUEST_AI_ADDED, SIMCONNECT_STATE.ON);
                simconnect.SetSystemEventState(DATA_REQUESTS.REQUEST_AI_REMOVED, SIMCONNECT_STATE.ON);

                // catch simobject data requests
                simconnect.OnRecvEventObjectAddremove +=
                    new SimConnect.RecvEventObjectAddremoveEventHandler(SimConnect_OnRecvEventObjectAddremove);
                simconnect.OnRecvSimobjectData +=
                    new SimConnect.RecvSimobjectDataEventHandler(SimConnect_OnRecvSimobjectData);
                simconnect.OnRecvSimobjectDataBytype +=
                    new SimConnect.RecvSimobjectDataBytypeEventHandler(SimConnect_OnRecvSimobjectDataBytype);

                // Initialize the list with current aircraft
                simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_AI_OBJECT_ID_BYTYPE,
                    DEFINITIONS.AI_Entity_Definition, MONITOR_RADIUS, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);
            }
            catch (COMException ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        void SimConnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            // Update the location of the AI entities and the user craft
            UpdateEntityPostionalData((PositionData)data.dwData[0], data.dwObjectID);
        }

        void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Trace.WriteLine("Connected to ESP");
        }

        // The case where the user closes ESP
        void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Trace.WriteLine("ESP has exited");
            closeConnection();
        }

        void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Trace.WriteLine("Exception received: " + data.dwException);
        }

        void SimConnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            // If we received a valid object, create a local copy of its data
            // Definitions of this data parameter located here: 
            // http://msdn.microsoft.com/en-us/library/cc526983.aspx#SIMCONNECT_RECV_SIMOBJECT_DATA
            if (data.dwentrynumber != 0 || data.dwoutof != 0)
            {
                CreateLocalEntityObject((AI_Entity_Details)data.dwData[0], data.dwObjectID);
            }
        }

        void SimConnect_OnRecvEventObjectAddremove(SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data)
        {
            // Handle the case where the AI system adds or removes objects
            switch ((DATA_REQUESTS)data.uEventID)
            {
                case DATA_REQUESTS.REQUEST_AI_REMOVED:
                    // Look through all the AIObjects in the list
                    foreach (System.Collections.Generic.KeyValuePair<string, AIObjectData> kvp in AIObjects)
                    {
                        // Attempt to find a match to the ObjectID that is getting deleted
                        if (kvp.Value.dwObjectID == data.dwData)
                        {
                            // If a match is found, delete the object from the AIObjects list
                            AIObjects.Remove(kvp.Key);
                            // Also delete the line item from the ListView control
                            ListViewItem[] lvis = AIObjectListView.Items.Find(data.dwData.ToString(), false);
                            if (lvis.Length == 1)
                            {
                                // If the item is found in the ListView, remove it
                                AIObjectListView.Items.Remove(lvis[0]);
                            }
                            break;
                        }
                    }
                    break;
                case DATA_REQUESTS.REQUEST_AI_ADDED:
                    // Refresh the list of monitored objects for this specific request
                    // first, Clear any current objects and stop all data requests for those objects
                    ResetDataAndShutDownRequests();

                    // Now, Initiate a request for the list of objects we are interested 
                    // in monitoring in the given radius
                    simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_AI_OBJECT_ID_BYTYPE,
                        DEFINITIONS.AI_Entity_Definition, MONITOR_RADIUS, monitoredObjectType);

                    break;
            }
        }
        #endregion

        #region Helper Functions

        private void ResetDataAndShutDownRequests()
        {
            // Disable requests for data on each individual object
            foreach (System.Collections.Generic.KeyValuePair<string, AIObjectData> kvp in AIObjects)
            {
                // Remove any outstanding requests for data on specific objects
                simconnect.RequestDataOnSimObject((DATA_REQUESTS)(AIObjects[kvp.Key].dwObjectID),
                    DEFINITIONS.PositionData_Definition, AIObjects[kvp.Key].dwObjectID,
                    SIMCONNECT_PERIOD.NEVER, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
            }
            // Clear the objects from our listview
            AIObjectListView.Items.Clear();
            // Clear the objects from our list container
            AIObjects.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // The case where the user closes the client
            closeConnection();
        }

        private void closeConnection()
        {
            if (simconnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                simconnect.Dispose();
                simconnect = null;
                Trace.WriteLine("Connection closed");
            }
        }

        private void CreateLocalEntityObject(AI_Entity_Details newAircraftData, uint objectID)
        {
            // SimConnect has notified the application that an entity exists and has provided its ObjectID
            // Create a local AIObjectData item to store this information and create a reocurring 
            // SimConnect request for positional data each second
            // Finally, display the objects information in the listview control

            AIObjectData aidata = new AIObjectData();
            aidata.dwIndex = 0;
            aidata.dwObjectID = objectID;
            aidata.itemText = aidata.dwObjectID.ToString();
            aidata.ad = newAircraftData;

            ListViewItem lvi = null;
            if (aidata.ad.IsUserSim)
            {
                // Request positional data for the user object
                simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_USERAIRCRAFT_DATA,
                    DEFINITIONS.PositionData_Definition, SimConnect.SIMCONNECT_OBJECT_ID_USER,
                    SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

                // Distinguish this item from AI objects with specific naming
                lvi = new ListViewItem("User");
            }
            else
            {
                // Request positional data for each object we encounter
                // Note that the request is for every second, but the interval is every 2nd one
                // so we will receive data every two seconds
                simconnect.RequestDataOnSimObject((DATA_REQUESTS)(objectID),
                    DEFINITIONS.PositionData_Definition, objectID,
                    SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 2, 0);
                lvi = new ListViewItem(aidata.ad.Category);
            }

            // Add this object to our dictionary list to keep track of it
            if (!AIObjects.ContainsKey(aidata.itemText))
            {
                AIObjects.Add(aidata.itemText, aidata);
            }
            else
            {
                // If the object has been added, update it
                AIObjects[aidata.itemText] = aidata;
            }

            // For this initial update, populate the items for this object with data which is available
            // The positional data will be updated as soon as the above 'RequestDataOnSimObject' 
            // request is processed
            // The remainder of items will be populated in SimConnect_OnRecvSimobjectDataBytype
            lvi.Name = objectID.ToString();
            lvi.SubItems.Add(aidata.ad.Title);
            lvi.SubItems.Add(aidata.ad.Model);
            lvi.SubItems.Add(aidata.ad.ID);
            lvi.SubItems.Add(aidata.ad.Airline);
            lvi.SubItems.Add(aidata.ad.FlightNumber);
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.Tag = aidata;
            lvi.Checked = true;

            // If this is the User's object, insert it at the top of the list
            // Add all other AI object in the order they are received
            if (aidata.ad.IsUserSim)
            {
                AIObjectListView.Items.Insert(0, lvi);
            }
            else if (AIObjects.ContainsKey(aidata.itemText))
            {
                AIObjectListView.Items.Add(lvi);
            }
        }

        private void UpdateEntityPostionalData(PositionData posData, uint objectID)
        {
            ListViewItem[] lvis = null;

            // Update an existing item with the reminder of positional data
            // first, by locating it in the listview and then by updating its subitems
            lvis = AIObjectListView.Items.Find(objectID.ToString(), false);

            if (lvis.Length == 1)
            {
                // Trim the values to a maximum of 7 digits
                lvis[0].SubItems[6].Text = posData.latitude.ToString("F07");
                lvis[0].SubItems[7].Text = posData.longitude.ToString("F07");
                lvis[0].SubItems[8].Text = posData.altitude.ToString("F02");
                lvis[0].SubItems[9].Text = posData.headingDegrees.ToString("F02");
                lvis[0].SubItems[10].Text = posData.speed.ToString("F02");
                lvis[0].SubItems[11].Text = posData.nav1Freq.ToString("F02");
                lvis[0].SubItems[12].Text = posData.nav1FreqStby.ToString("F02");
                lvis[0].SubItems[13].Text = posData.nav2Freq.ToString("F02");
                lvis[0].SubItems[14].Text = posData.nav2FreqStby.ToString("F02");


            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Process the default group
            try
            {
                FSUIPCConnection.Process();


            }
            catch (FSUIPCException ex)
            {
                if (ex.FSUIPCErrorCode == FSUIPCError.FSUIPC_ERR_SENDMSG)
                {
                    // Send message error - connection to FSUIPC lost.
                    // Show message, disable the main timer loop and relight the 
                    // connection button:
                    // Also Close the broken connection.
                    this.timer1.Enabled = false;
                    this.btnConnectar.Enabled = true;
                    this.chkEnableAIRadar.Enabled = false;
                    this.chkEnableAIRadar.Checked = false;
                    this.AIRadarTimer.Enabled = false;
                    FSUIPCConnection.Close();
                    MessageBox.Show("The connection to Flight Sim has been lost.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    // not the disonnect error so some other baddness occured.
                    // just rethrow to halt the application
                    throw ex;
                }
            }
            catch (Exception)
            {
                // Sometime when the connection is lost, bad data gets returned 
                // and causes problems with some of the other lines.  
                // This catch block just makes sure the user doesn't see any
                // other Exceptions apart from FSUIPCExceptions.
            }
        }


     


        

        private double degreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private void chkEnableAIRadar_CheckedChanged_1(object sender, EventArgs e)
        {
            // Turn on/off the radar timer to update the radar info  (runs every second)
            this.AIRadarTimer.Enabled = this.chkEnableAIRadar.Checked;
            // Force the panel to redraw now rather than wait one second
            this.radaATC.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ICAOsearchChat = txtICAOsearchChat.Text;
            

            //define o dataset
            mDataSet = new DataSet();

            //define string de conexao e cria a conexao
            mConn = new MySqlConnection(" Persist Security Info=False;server=voemercosul.com;database=voemerco_v2;uid=voemerco_v2;pwd=).+dTv$XT(U*");
            //server=localhost;database=Cadastro;uid=root;pwd=xxxx");

            try
            {
                //abre a conexao
                mConn.Open();
            }
            catch (System.Exception k)
            {
                MessageBox.Show(k.Message.ToString());
            }

            //verificva se a conexão esta aberta
            if (mConn.State == ConnectionState.Open)
            {
                //cria um adapter usando a instrução SQL para acessar a tabela Clientes
                mAdapter = new MySqlDataAdapter("SELECT * FROM frequencias_aeroportos WHERE `airport_ident` = '" + ICAOsearchChat + "'", mConn);
                //mAdapter = new MySqlDataAdapter("SELECT * FROM frequencias_aeroportos WHERE airport_ident ='SBGL'", mConn);
                //preenche o dataset via adapter
                mAdapter.Fill(mDataSet, "frequencias_aeroportos");
                //atribui a resultado a propriedade DataSource do DataGrid
                dataGridView1.DataSource = mDataSet;
                dataGridView1.DataMember = "frequencias_aeroportos";

            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            closeConnection();
            //setButtons(true, false, false);
            // Clear the listview and Dictionary data
            AIObjectListView.Items.Clear();
            AIObjects.Clear();
        }

        private void frmLogadoControle_Load(object sender, EventArgs e)
        {
           
            


            richtextboxChat.BackColor = Color.Black;
            richtextboxChat.ForeColor = Color.LightGreen;

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //timerChat.Enabled = true;
            /*BackgroundWorker bgwChatATC = new BackgroundWorker();
            bgwChatATC.DoWork += new DoWorkEventHandler(bgwChatATC_DoWork);
            bgwChatATC.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwChatATC_RunWorkerCompleted);

            bgwChatATC.RunWorkerAsync();*/
            timerChat.Enabled = true;
        }

        void bgwChatATC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          
           
                string stringConnection = "Persist Security Info=False;server=voemercosul.com;database=voemerco_v2;uid=voemerco_v2;pwd=).+dTv$XT(U*";

                string ICAOoperante = txtICAOsearchChat.Text;
                string freqOperante = txtFrequenciaOperar.Text;

                MySqlConnection connection = new MySqlConnection(stringConnection);
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader Reader;
                command.CommandText = "SELECT * FROM `chat_atc_twMerco` WHERE `icao` = '" + ICAOoperante + "' AND `frequencia` = '" + freqOperante + "' ORDER BY id DESC";
                connection.Open();
                Reader = command.ExecuteReader();
               // listBox1.Items.Clear();
                richtextboxChat.Clear();                
                while (Reader.Read())
                {
                   // listBox1.Items.Add("");
                    
                  //  listBox1.Items.Add("" + Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ")");

                   
                   // richTextBox2.Text = "";
                    richtextboxChat.Text += String.Format(Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ") \n", Environment.NewLine);
                }
                connection.Close();

          
          
           
        }
        void bgwChatATC_DoWork(object sender, DoWorkEventArgs e)
        {
            //Do work process here.        
           // toolStripStatusLabel1.Text = "Background Worker do their work now";
           
           // MessageBox.Show("Background Worker do their work now");
             
            Thread.Sleep(1000);//sleep 10 seconds
        }

       
            
         


     

        private void timer1_Tick_1(object sender, EventArgs e)
        {

        }

        private void AIRadarTimer_Tick(object sender, EventArgs e)
        {
            // Every second we update the Ground and Airborne AI trafic info
            AI.RefreshAITrafficInformation(this.chkShowGroundAI.Checked, this.chkShowAirborneAI.Checked);
            // Apply a filter according to the range set by the user
            // Filtering ground and airborne traffic, no bearing filter (include from 0-360)
            // No altitude filter (passing nulls)
            // Range as set by the combo box.
            AI.ApplyFilter(true, true, 0, 560, null, null, double.Parse(this.cbxRadarRange.Text));
            // Invalidate the radar panel so it redraws.
            this.radaATC.Invalidate();
        }

        


           

     

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) MessageBox.Show("Operation was canceled");
            else if (e.Error != null) MessageBox.Show(e.Error.Message);
            else MessageBox.Show(e.Result.ToString());
        }

        private void btnFinalizaControle_Click(object sender, EventArgs e)
        {
            // Cancel BackgroundWorker
            if (!bgwChatATC.IsBusy)
                bgwChatATC.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
          

        }

        private void timerChat_Tick(object sender, EventArgs e)
        {
            BackgroundWorker bgwChatATC = new BackgroundWorker();
            bgwChatATC.DoWork += new DoWorkEventHandler(bgwChatATC_DoWork);
            bgwChatATC.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwChatATC_RunWorkerCompleted);

            bgwChatATC.RunWorkerAsync();
        }


        public static string PegarIPExterno()
        {
            System.Net.WebClient t = new System.Net.WebClient();
            string meuip = t.DownloadString("http://meuip.datahouse.com.br");
            return meuip
                .Substring(
                meuip.IndexOf("o Meu IP? ") + "o Meu IP? ".Length,
                meuip.IndexOf("</title>") - meuip.IndexOf("o Meu IP? ")
                - "o Meu IP? ".Length);
        }
        

        private void btnEnviaChat_Click(object sender, EventArgs e)
        {


           

            string ICAOoperante = txtICAOsearchChat.Text;
            string freqOperante = txtFrequenciaOperar.Text;
            string mensagem = rtxtBoxMsgATC.Text;
            string ip = PegarIPExterno();
            string strFormato = string.Empty;
            strFormato = DateTime.Now.Date.ToString("dd-MM-yyyy");
            string hora = DateTime.Now.ToShortTimeString();
            // Início da Conexão com indicação de qual o servidor, nome de base de dados e utilizar

            /* É aconselhável criar um utilizador com password. Para acrescentar a password é somente
            necessário acrescentar o seguinte código a seguir ao uid=root;password=xxxxx*/

            mConn = new MySqlConnection("Persist Security Info=False;server=voemercosul.com;database=voemerco_v2;uid=voemerco_v2;pwd=).+dTv$XT(U*");

            // Abre a conexão
            mConn.Open();
            //"INSERT INTO `chat_atc_twMerco`  VALUES "
            //Query SQL
            MySqlCommand command = new MySqlCommand("INSERT INTO chat_atc_twMerco (`id`, `pilotId`, `paraATCtipo`, `icao`, `frequencia`, `menssagem`, `ip`, `dia`, `hora`, `ativa`)" +
                                                    "VALUES('','" + ICAOoperante + "', 'PILOTO', '" + ICAOoperante + "', '" + freqOperante + "', '" + mensagem + "', '" + ip + "', '" + strFormato + "', '" + hora + "', '1' )", mConn);

            //Executa a Query SQL
            command.ExecuteNonQuery();
            rtxtBoxMsgATC.Clear();
            rtxtBoxMsgATC.Focus();

            // Fecha a conexão
           /* mConn.Close();

            //Mensagem de Sucesso
            MessageBox.Show("Gravado com Sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Método criado para que quando o registo é gravado, automaticamente a dataGridView efectue um "Refresh"
           // mostraResultados();
          */


        }

        private void radaATC_Paint(object sender, PaintEventArgs e)
        {
            //This gets called whenever the panel needs to draw itself.
            if (this.chkEnableAIRadar.Checked)
            {
                // First Clear the panel and make a black background
                e.Graphics.Clear(Color.Black);
                // Start by working out the centre of the radar and draw the centre cross
                Point centre = new Point(this.radaATC.ClientSize.Width / 2, this.radaATC.ClientSize.Height / 2);
                e.Graphics.DrawLine(Pens.White, centre.X - 4, centre.Y, centre.X + 4, centre.Y);
                e.Graphics.DrawLine(Pens.White, centre.X, centre.Y - 4, centre.X, centre.Y + 4);
                double range = double.Parse(this.cbxRadarRange.Text);
                // work out the scale using the range and the smallest size of the panel
                double scale = range / (double)(this.radaATC.ClientSize.Width < this.radaATC.ClientSize.Height ? this.radaATC.ClientSize.Width : this.radaATC.ClientSize.Height) * 2d;
                // Go through each plane and draw it on the radar
                // Note: We are using the seperate collections for the ground and airborne
                // There is a collection of all AI traffic called 'AllTraffic' which can be used if
                // you do not want to deal with these seperatley.
                // First, draw the ground AI if required
                if (this.chkShowGroundAI.Checked)
                {
                    // Loop through the collection of plane objects in the GroundTraffic collection.
                    foreach (AIPlaneInfo plane in AI.GroundTraffic)
                    {
                        // Here we just pass the planeInfo off to the draw routine.
                        // There is quite a lot of information available in the AIPlaneInfo object.
                        // See the reference manual or Intellisense for details.
                        drawTarget(e.Graphics, scale, centre, plane);
                        
                    }
                }
                // Next, draw the Airborne AI if required
                if (this.chkShowAirborneAI.Checked)
                {
                    // Loop through the collection of plane objects in the GroundTraffic collection.
                    foreach (AIPlaneInfo plane in AI.AirbourneTraffic)
                    {
                        drawTarget(e.Graphics, scale, centre, plane);
                    }
                }
            }
            else
            {
                // Radar turned off so just clear it with white
                e.Graphics.Clear(Color.White);
            }
        }


        private void drawTarget(Graphics graphics, double scale, Point centre, AIPlaneInfo plane)
        {
            // We are going to use some of the info from the plane object to draw the target.
            // Lots more info is avilable for other application.  
            // See the reference manual or Intellisense for details.
            // Work out the range of the target in pixels by multiplying by the scale
            double distancePixels = plane.DistanceNM / scale;
            // Work out the position from the centre using this distance and the bearing
            double dx = Math.Cos(degreeToRadian(plane.BearingTo)) * distancePixels;
            double dy = Math.Sin(degreeToRadian(plane.BearingTo)) * distancePixels;
            PointF target = new PointF((float)centre.X + (float)dx, (float)centre.Y + (float)dy);
            // Draw the target circle around this point oriented to the plane's heading 
            graphics.DrawEllipse(Pens.LightGreen, target.X - 4f, target.Y - 4f, 8f, 8f);

            // Draw a line from the circle to indicate heading
            double tailHeading = 180d + plane.HeadingDegrees;
            dx = Math.Cos(degreeToRadian(tailHeading)) * 12;
            dy = Math.Sin(degreeToRadian(tailHeading)) * 12;
            PointF tailEnd = new PointF(target.X + (float)dx, target.Y + (float)dy);
            graphics.DrawLine(new Pen(new LinearGradientBrush(target, tailEnd, Color.LightGreen, Color.DarkGreen)), target, tailEnd);
            // Work out the position of the data block
            PointF dataBlock = new PointF(target.X + 20, target.Y - 20);
            // Draw the line to the datablock
            graphics.DrawLine(Pens.LightGreen, new PointF(target.X + 5, target.Y - 5), new PointF(dataBlock.X - 5, dataBlock.Y + 7));
            // Draw the data block
            // Line 1 - the Callsign
            graphics.DrawString(plane.ATCIdentifier, this.radaATC.Font, Brushes.LightGreen, dataBlock);
            // Line 2 - the Altitude (hundreds of feet) and speed
            string line2 = "";
            line2 += ((int)(plane.AltitudeFeet / 100d)).ToString("d3");
            // Put a +,- or = depending on if the plane is decending, climbing or level
            if (plane.VirticalSpeedFeet < 0)
            {
                line2 += "-";
            }
            else if (plane.VirticalSpeedFeet > 0)
            {
                line2 += "+";
            }
            else
            {
                line2 += "=";
            }

            graphics.DrawString(line2, this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 12));
            // Line 3 - origin, destination and assigned runway
            graphics.DrawString(plane.DepartureICAO + "->" + plane.DestinationICAO + " " + plane.RunwayAssigned.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 24));
            graphics.DrawString(plane.AircraftModel.ToString() + plane.AircraftTitle.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 36));
            graphics.DrawString(plane.PitchDegrees.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 48));
            graphics.DrawString(plane.FlightNumber.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 60));
            graphics.DrawString(plane.Com1String.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 72));
            graphics.DrawString(plane.AircraftModel.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 84));
            graphics.DrawString(plane.Heading.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 24));
            graphics.DrawString(plane.AircraftType.ToString(), this.radaATC.Font, Brushes.LightGreen, new PointF(dataBlock.X, dataBlock.Y + 36));
            graphics.DrawString(plane.Airline.ToString(), this.radaATC.Font, Brushes.Red, new PointF(dataBlock.X, dataBlock.Y + 48));
            graphics.DrawString(plane.ATCIdentifier.ToString(), this.radaATC.Font, Brushes.Red, new PointF(dataBlock.X, dataBlock.Y + 60));

        }


 

    }

    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint,
            true);

            this.UpdateStyles();
        }
    }
}
