using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// Add these two statements to all SimConnect clients
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Xml;
using System.IO;
using FSUIPC;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.XPath;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading;
using System.Resources;
using System.Reflection;
using Microsoft.CSharp;
using System.Drawing.Drawing2D;







namespace twMerco
{



         


    public partial class frmLogadoPiloto : Form
    {


        // Private Static Members
        private static readonly string AppTitle = "twMerco";

        // Create the Offsets we're interested in for this Application
        private Offset<int> airspeed = new Offset<int>(0x02BC);  // Basic integer read example
        private Offset<int> avionics = new Offset<int>(0x2E80);  // Basic integer read and write example
        private Offset<byte[]> fsLocalDateTime = new Offset<byte[]>(0x0238, 10); // Example of reading an arbitary set of bytes. 
        private Offset<string> aircraftType = new Offset<string>("AircraftInfo", 0x3160, 24); // Example of string and use of a group
        private Offset<BitArray> lights = new Offset<BitArray>(0x0D0C, 2); // Example of BitArray used to manage a bit field type offset.
        private Offset<Double> compass = new Offset<double>(0x02CC); // Example for disconnecting/reconnecting
        private Offset<short> pause = new Offset<short>(0x0262, true); // Example of a write only offset.
        private Offset<short> com2bcd = new Offset<short>(0x3118); // Example of reading a frequency coded in Binary Coded Decimal
        private Offset<long> playerLatitude = new Offset<long>(0x0560); // Offset for Lat/Lon features
        private Offset<long> playerLongitude = new Offset<long>(0x0568); // Offset for Lat/Lon features
        private Offset<short> onGround = new Offset<short>(0x0366); // Offset for Lat/Lon features
        private Offset<short> magVar = new Offset<short>(0x02A0); // Offset for Lat/Lon features
        private Offset<uint> playerHeadingTrue = new Offset<uint>(0x0580); // Offset for moving the plane
        private Offset<long> playerAltitude = new Offset<long>(0x0570); // Offset for moving the plane
        private Offset<short> slewMode = new Offset<short>(0x05DC, true); // Offset for moving the plane
        private Offset<int> sendControl = new Offset<int>(0x3110, true); // Offset for moving the plane

       

        private FsLatLonPoint EGLL; // Holds the position of London Heathrow (EGLL)
        private FsLatLonQuadrilateral runwayQuad; // defines the four corners of the runway (27L at EGLL)
        private AITrafficServices AI; // Holds a reference to the AI Traffic Services object


        private MySqlConnection mConn;
       // private MySqlDataAdapter mAdapter;
       // private DataSet mDataSet;

      
        public frmLogadoPiloto()
        {
            response = 1;
            output = "\n\n\n\n\n\n\n\n\n\n";
            InitializeComponent();
            setButtons(true, false);

            // Setup the example data for London Heathrow
            // 1. The position
            //    This shows an FsLongitude and FsLatitude class made from the Degrees/Minutes/Seconds constructor.
            //    The Timer1_Tick() method shows a different contructor (using the RAW FSUIPC values).
            FsLatitude lat = new FsLatitude(51, 28, 39.0d);
            FsLongitude lon = new FsLongitude(0, -27, -41.0d);
            EGLL = new FsLatLonPoint(lat, lon);
            // Now define the Quadrangle for the 27L (09R) runway.
            // We could just define the four corner Lat/Lon points if we knew them.
            // In this example however we're using the helper function to calculate the points
            // from the runway information.  This is the kind of info you can find in the output files
            // from Pete Dowson's MakeRunways program.
            FsLatitude rwyThresholdLat = new FsLatitude(51.464943d);
            FsLongitude rwyThresholdLon = new FsLongitude(-0.434046d);
            double rwyMagHeading = 272.7d;
            double rwyMagVariation = -3d;
            double rwyLength = 11978d;
            double rwyWidth = 164d;
            // Call the static helper on the FsLatLonQuarangle class to generate the Quadrangle for this runway...
            FsLatLonPoint thresholdCentre = new FsLatLonPoint(rwyThresholdLat, rwyThresholdLon);
            double trueHeading = rwyMagHeading + rwyMagVariation;
            runwayQuad = FsLatLonQuadrilateral.ForRunway(thresholdCentre, trueHeading, rwyWidth, rwyLength);
            // Set the default value for the distance units and AI Radar range
            //this.cbxDistanceUnits.Text = "Nautical Miles";
           
           
        }



        private void openFSUIPC()
        {
            try
            {
                // Attempt to open a connection to FSUIPC (running on any version of Flight Sim)
                FSUIPCConnection.Open();
                // Opened OK so disable the Connect button
                this.button_Connect.Enabled = false;
               
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
        }


        

        private void button_Connect_Click(object sender, EventArgs e)
        {
            openFSUIPC();

            // Aircraft type is in the "AircraftInfo" data group so we only want to proccess that here.
            try
            {
                FSUIPCConnection.Process("AircraftInfo");
                // OK so display the string
                // With strings the DLL automatically handles the 
                // ASCII/Unicode conversion and deals with the 
                // zero terminators.
                //this.txtAircraftType.Text = aircraftType.Value;
                tsslGetAeronave.Text = aircraftType.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //timerFechaProcessos.Enabled = true;
            if (my_simconnect == null)
            {
                try
                {
                    my_simconnect = new Microsoft.FlightSimulator.SimConnect.SimConnect("Managed Data Request", base.Handle, 0x402, null, 0);
                    setButtons(false, true);
                    initDataRequest();
                    timer1.Enabled = true;
                }
                catch (COMException)
                {
                    label_status.Text = "Impossível conectar ao FSX.";
                    label_status.ForeColor = Color.Red;
                }
            }
            else
            {
                label_status.Text = "Erro, tente novamente";
                label_status.ForeColor = Color.Green;
                closeConnection();
                setButtons(true, false);
                timer1.Enabled = false;
            }
        }



        private void chkNavigation_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Navigation] = this.chkNavigation.Checked;
            //this.lights.Value[0] = this.chkNavigation.Checked;
        }

        private void chkBeacon_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Beacon] = this.chkBeacon.Checked;
            //this.lights.Value[1] = this.chkBeacon.Checked;
        }

        private void chkLanding_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Landing] = this.chkLanding.Checked;
        }

        private void chkTaxi_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Taxi] = this.chkTaxi.Checked;
        }

        private void chkStrobes_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Strobes] = this.chkStrobes.Checked;
        }

        private void chkInstuments_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Instruments] = this.chkInstuments.Checked;
        }

        private void chkRecognition_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Recognition] = this.chkRecognition.Checked;
        }

        private void chkWing_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Wing] = this.chkWing.Checked;
        }

        private void chkLogo_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Logo] = this.chkLogo.Checked;
        }

        private void chkCabin_CheckedChanged(object sender, EventArgs e)
        {
            this.lights.Value[(int)LightType.Cabin] = this.chkCabin.Checked;
        }



        private void button_Disconnect_Click(object sender, EventArgs e)
        {

            // Disconnect immediatley.
            this.compass.Disconnect();
            closeConnection();
            setButtons(true, false);
            timer1.Enabled = false;
            timerFechaProcessos.Enabled = false;
            timerMsgAlerta.Enabled = false;
            textBox_latitude.Text = "";
            textBox_longitude.Text = "";
            textBox_trueheading.Text = "";           
           
            
        }

        private enum LightType
        {
            Navigation,
            Beacon,
            Landing,
            Taxi,
            Strobes,
            Instruments,
            Recognition,
            Wing,
            Logo,
            Cabin
        }


        private void closeConnection()
        {
            if (my_simconnect != null)
            {
                my_simconnect.Dispose();
                my_simconnect = null;
                label_status.Text = "Conexão Fechada.";
                label_status.ForeColor = Color.Red;
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 0x402)
            {
                if (my_simconnect != null)
                {
                    my_simconnect.ReceiveMessage();
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void displayText(string s)
        {
            output = output.Substring(output.IndexOf("\n") + 1);
            object obj1 = output;
            output = string.Concat(new object[] { obj1, "\n", response++, ": ", s });
            label_status.Text = output;
        }

        private void initDataRequest()
        {
            try
            {
                my_simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                my_simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
                my_simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Ground Altitude", "meters", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AI GROUNDTURNSPEED", "knots", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NUMBER OF ENGINES", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ENGINE TYPE", "enum", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "FUEL TANK LEFT MAIN LEVEL", "percent", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "FUEL TANK CENTER LEVEL", "percent", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "FUEL TANK RIGHT MAIN LEVEL", "percent", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GENERAL ENG RPM:1", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "VERTICAL SPEED", "Feet per second", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "STALL WARNING", null, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "OVERSPEED WARNING", null, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GEAR HANDLE POSITION", null, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GEAR CENTER POSITION", null, SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GEAR LEFT POSITION", null, SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GEAR RIGHT POSITION", null, SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AMBIENT TEMPERATURE", "Celsius", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AMBIENT PRESSURE", "inHg", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AMBIENT WIND VELOCITY", "Knots", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AMBIENT WIND DIRECTION", "Degrees", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "BAROMETER PRESSURE", "Millibars", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AMBIENT VISIBILITY", "Kilometer", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV ACTIVE FREQUENCY:1", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV STANDBY FREQUENCY:1", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV ACTIVE FREQUENCY:2", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV STANDBY FREQUENCY:2", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ATC FLIGHT NUMBER", null, SIMCONNECT_DATATYPE.STRING8, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM ACTIVE FREQUENCY:1", "Frequency BCD16", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
              
               
               
               
                
               
               

                my_simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);
                my_simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(simconnect_OnRecvSimobjectDataBytype);
            }
            catch (COMException exception1)
            {
                displayText(exception1.Message);
            }
        }

        private void setButtons(bool bConnect, bool bDisconnect)
        {
            button_Connect.Enabled = bConnect;
            button_Disconnect.Enabled = bDisconnect;
        }

        private void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            label_status.Text = "Exceção recebida: " + ((uint)data.dwException);
            label_status.ForeColor = Color.Blue;
        }

        private void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            label_status.Text = "Conectado ao FSX";
            label_status.ForeColor = Color.Green;
        }

        private void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            label_status.Text = "FSX foi fechado";
            label_status.ForeColor = Color.Blue;
            closeConnection();
            timer1.Enabled = false;
        }

        private void playSound(string path)
        {
            System.Media.SoundPlayer player =
                new System.Media.SoundPlayer();
            player.SoundLocation = path;
            player.Load();
            player.Play();
        }



        private void Caluculate(int i)
        {
            double pow = Math.Pow(i, i);
        }


        private void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            string DirNumberText = "";
            string DirNumber1Text = "";
            string DirNumber2Text = "";
            string FileNumberText = "";
            string FileNumber1Text = "";
            string FileNumber2Text = "";
            double DirNumber1 = 0;
            double DirNumber2 = 0;
            double FileNumber1 = 0;
            double FileNumber2 = 0;


            if (data.dwRequestID == 0)
            {
                Struct1 struct1 = (Struct1)data.dwData[0];
                // label_aircraft.Text = struct1.title.ToString();


              

                textBox_latitude.Text = struct1.latitude.ToString();
                textBox_longitude.Text = struct1.longitude.ToString();
                textBox_trueheading.Text = struct1.trueheading.ToString();
                
                /*MOTORES*/
                txtMotores.Text = struct1.numMotores.ToString();
                txtTipoMotor.Text = struct1.tipoMotor.ToString();
                txtRPMmotores.Text = "" + struct1.rpmMotores.ToString() + " RPM";  
                /*END MOTORES*/

                /*RADIO AERONAVE*/
                txtRadioNav1.Text = struct1.radioNav1.ToString();
                txtRadioNav1.BackColor = Color.Black;
                txtRadioNav1.ForeColor = Color.LightGreen;

                txtRadioNav1StandBy.Text = struct1.radioNav1Stand.ToString();
                txtRadioNav1StandBy.BackColor = Color.Black;
                txtRadioNav1StandBy.ForeColor = Color.LightGreen;

                txtRadioNav2Ativo.Text = struct1.radioNav2.ToString();
                txtRadioNav2Ativo.BackColor = Color.Black;
                txtRadioNav2Ativo.ForeColor = Color.LightGreen;

                txtNav2StandBy.Text = struct1.radioNav2Stand.ToString();
                txtNav2StandBy.BackColor = Color.Black;
                txtNav2StandBy.ForeColor = Color.LightGreen;

               /* txtCom2Ativo.Text = struct1.com1Freq.ToString();
                txtCom2Ativo.BackColor = Color.Black;
                txtCom2Ativo.ForeColor = Color.LightGreen;*/

                textBox1.Text = struct1.numeroDoVoo;

                    /*END RADIO AERONAVE*/

                /*TANQUES DE COMBÚSTIVEL*/


                txtTanqueCentral.Text = ""+ struct1.tanqueCentral.ToString() +" %";
                txtTanqueDireito.Text = ""+ struct1.tanqueDireito.ToString() +" %";
                txtTanqueEsquerdo.Text = ""+ struct1.tanqueEsquerdo.ToString()+" %";

                
                /*END TANQUES DE COMÚSTIVEL*/

                /*TEMPERATURAS, PRESSÃO, VISIBILIDADE*/
                txtTemperaturaAmbeinte.Text = "" + struct1.tempAmbiente.ToString() + " °C";
                txtPressaoAmbiente.Text = "" + struct1.pressAmbiente.ToString() + " inHg";
                txtVelocidadeVento.Text = "" + struct1.velocidade.ToString() + " Knots";
                txtDirecaoVento.Text = "" + struct1.direcaoVento.ToString() + " °Graus";
                txtPressaoBarometrica.Text = "" + struct1.pressaoBarometrica.ToString() + " Milibares";
                txtVisibilidadeAmbiente.Text = "" + struct1.visibilidadeAmbiente.ToString() + " Quilômetros";
                /*END TEMPERATURAS*/

                
                txtVs.Text = struct1.vs.ToString();
                tsslStall.Text = struct1.stall.ToString();
                tsslOverSpeed.Text = struct1.overSpeed.ToString();
                txtLandingGear.Text = struct1.landingGear.ToString();   
                /*TRENS DE POUSO AERONAVE*/
                int gearCentral = struct1.rodaCentralPosicao;
                if (gearCentral == 1)
                {
                    txtGearCentral.Text = "Roda Baixada";
                    txtGearCentral.BackColor = Color.Green;
                    txtGearCentral.ForeColor = Color.Black;
                }
                else
                {
                    txtGearCentral.Text = "Roda Levantada";
                    txtGearCentral.BackColor = Color.Red;
                    txtGearCentral.ForeColor = Color.Black;
                }

                int gearDireita = struct1.rodaDireitaPosicao;
                if (gearDireita == 1)
                {
                    txtGearDireita.Text = "Roda Baixada";
                    txtGearDireita.BackColor = Color.Green;
                    txtGearDireita.ForeColor = Color.Black;
                }
                else
                {
                    txtGearDireita.Text = "Roda Levantada";
                    txtGearDireita.BackColor = Color.Red;
                    txtGearDireita.ForeColor = Color.Black;
                }

               int gearEsquerda = struct1.rodaEsquerdaPosicao;
                if(gearEsquerda == 1)
                {

                    txtGearEsquerda.Text = "Roda Baixada";
                    txtGearEsquerda.BackColor = Color.Green;
                    txtGearEsquerda.ForeColor = Color.Black;
                }else{

                    txtGearEsquerda.Text = "Roda Levantada";
                        txtGearEsquerda.BackColor = Color.Red;
                        txtGearEsquerda.ForeColor = Color.Black;
                }



                /*TRENS DE POUSO AERONAVE*/

                


                //TANQUE DIREITO ADICIONADO NA PROGRESS BAR (228)

                int valueTanqueDireito = struct1.tanqueDireito;
                this.progbarTanqueDireito.Minimum = 0;
                this.progbarTanqueDireito.Maximum = 100;
                this.progbarTanqueDireito.Value = valueTanqueDireito;


                // END (228)


                //TANQUE CENTRAL ADICIONADO NA PROGRESS BAR (229)

                int valueTanqueCentral = struct1.tanqueCentral;
                progbarTanqueCentral.Minimum = 0;
                progbarTanqueCentral.Maximum = 100;
                progbarTanqueCentral.Value = valueTanqueCentral;

                //END (229)


                   
               // TANQUE ESQUERDO ADICIONADO NA PROGRESS BAR (230)

                int valueTanqeuEsquerdo = struct1.tanqueEsquerdo;
                progbarTanqueEsquerdo.Minimum = 0;
                progbarTanqueEsquerdo.Maximum = 100;
                progbarTanqueEsquerdo.Value = valueTanqeuEsquerdo;             

               //END (230)


                //Condição trem de pouso (231)

                string num0tremDePouso = "0";
               // string num1tremDePouso = "1";

                if (txtLandingGear.Text == num0tremDePouso)
                {
                    txtLandingGear.Text = "Trem de pouso levantado";
                    txtLandingGear.BackColor = Color.Red;

                                      

                }
                else
                {
                    txtLandingGear.Text = "Trem de pouso baixado";
                    txtLandingGear.BackColor = Color.Green;
                                     
                    
                    
                }

                //End (231)

                //Condição verificar se aeronave está em overspeed (232)

                string num0overspeed = "0";
               // string num1overspeed = "1";

                if (tsslOverSpeed.Text == num0overspeed)
                {
                    tsslOverSpeed.Text = "Fora de Over Speed";
                    tsslOverSpeed.ForeColor = Color.Green;
                    timer1.Interval = 250;
                }
                else
                {
                   
                        tsslOverSpeed.Text = "Over Speed";
                        tsslOverSpeed.ForeColor = Color.Red;
                        if (chbSonsDesativar.Checked == false)
                        {
                            System.Media.SoundPlayer MeuPlayer = new System.Media.SoundPlayer(@"http://voemercosul.com/network/br/twMerco/sound/beep.wav");
                            MeuPlayer.Play();
                            timer1.Interval = 3000;
                        }

                  
                    
                }

                //End (233)


                //Condição para verificar se aeronave está em stall (233)
                string num1Stall = "0";


                if (tsslStall.Text == num1Stall)
                {
                    tsslStall.Text = "Fora de Stall";
                    tsslStall.ForeColor = Color.Green;
                    timer1.Interval = 250;
                }
                else
                {
                    tsslStall.Text = "Stall";
                    tsslStall.ForeColor = Color.Red;
                    if (chbSonsDesativar.Checked == false)
                    {
                        System.Media.SoundPlayer MeuPlayer = new System.Media.SoundPlayer(@"http://voemercosul.com/network/br/twMerco/sound/stallalert.wav");
                        MeuPlayer.Play();
                        timer1.Interval = 3000;
                    }

                }
            

                //END (233)


                //váriaveis e condições de tipo de motores (234)
                string tipoMotoroA = txtTipoMotor.Text;
                string num0 = "0";
                string num1 = "1";
                string num2 = "2";
                string num3 = "3";
                string num4 = "4";
                string num5 = "5";

                if (txtTipoMotor.Text == num0)
                {
                    txtTipoMotor.Text = "Pistão";
                }
                else
                {
                    if (txtTipoMotor.Text == num1)
                    {
                        txtTipoMotor.Text = "Jato";
                    }
                    else
                    {
                        if (txtTipoMotor.Text == num2)
                        {

                            txtTipoMotor.Text = "Não Disponível";
                        }
                        else
                        {
                            if (txtTipoMotor.Text == num3)
                            {
                                txtTipoMotor.Text = "Turbina Helicóptero";
                            }
                            else
                            {
                                if (txtTipoMotor.Text == num4)
                                {
                                    txtTipoMotor.Text = "Não Suportado";
                                }
                                else
                                {
                                    if (txtTipoMotor.Text == num5)
                                    {
                                        txtTipoMotor.Text = "Turbo Propulsão";
                                    }
                                }
                            }
                        }
                    }
                }
            
                //#end (234)

                DirNumber1 = ((int)(((180.0 + (struct1.longitude)) * 12) / 360.0));
                if (DirNumber1 < 10)
                    DirNumber1Text = "0" + DirNumber1.ToString();
                else
                    DirNumber1Text = DirNumber1.ToString();
                DirNumber2 = ((int)(((90.0 - (struct1.latitude)) * 8) / 180.0));
                if (DirNumber2 < 10)
                    DirNumber2Text = "0" + DirNumber2.ToString();
                else
                    DirNumber2Text = DirNumber2.ToString();
                DirNumberText = DirNumber1Text + DirNumber2Text;

                FileNumber1 = ((int)(((180.0 + (struct1.longitude)) * 96) / 360.0));
                if (FileNumber1 < 10)
                    FileNumber1Text = "0" + FileNumber1.ToString();
                else
                    FileNumber1Text = FileNumber1.ToString();
                FileNumber2 = ((int)(((90.0 - (struct1.latitude)) * 64) / 180.0));
                if (FileNumber2 < 10)
                    FileNumber2Text = "0" + FileNumber2.ToString();
                else
                    FileNumber2Text = FileNumber2.ToString();
                FileNumberText = FileNumber1Text + FileNumber2Text;

                


            }
            else
            {
                label_status.Text = "Unknown request ID: " + ((uint)data.dwRequestID);
                textBox_latitude.Text = "";
                textBox_longitude.Text = "";
                textBox_trueheading.Text = "";
                
                
            }
        }




        private Microsoft.FlightSimulator.SimConnect.SimConnect my_simconnect;
        private string output;
        private int response;
        private const int WM_USER_SIMCONNECT = 0x402;

        private enum DATA_REQUESTS
        {
            REQUEST_1
        }

        private enum DEFINITIONS
        {
            Struct1
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Struct1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string title;
            public double latitude;
            public double longitude;
            public int trueheading;
            public int groundaltitude;
            public int velocidade;
            public double numMotores;
            public double tipoMotor;
            public int tanqueEsquerdo;
            public int tanqueCentral;
            public int tanqueDireito;
            public int rpmMotores;
            public int vs;
            public double stall;
            public double overSpeed;
            public double landingGear;
            public int rodaCentralPosicao;
            public int rodaEsquerdaPosicao;
            public int rodaDireitaPosicao;
            public int tempAmbiente;
            public int pressAmbiente;
            public int velocidadeVento;
            public int direcaoVento;
            public int pressaoBarometrica;
            public int visibilidadeAmbiente;
            public double radioNav1;
            public double radioNav1Stand;
            public double radioNav2;
            public double radioNav2Stand;
            public string numeroDoVoo;
            public double com1Freq;
           
          
            
           
        }


      


        private void progressBar1_Click(object sender, EventArgs e)
        {

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

        


        private void frmLogadoPiloto_Load(object sender, EventArgs e)
        {

            richtextboxChat.BackColor = Color.Black;
            richtextboxChat.ForeColor = Color.LightGreen;

            

            string localComputerName = Dns.GetHostName();

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            nOMEToolStripMenuItem.Text = localComputerName;

            iPToolStripMenuItem.Text = PegarIPExterno();


            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("fsx");

            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void timerFechaProcessos_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("fsx");

            foreach (Process process in processes)
            {
                process.Kill();
            }

            frmLogadoPiloto frmClose = new frmLogadoPiloto();

            frmClose.Close();
        }

        private void btnEnviarPlano_Click(object sender, EventArgs e)
        {
            timerFechaProcessos.Enabled = false;
            timerMsgAlerta.Enabled = false;
        }

        private void timerMsgAlerta_Tick(object sender, EventArgs e)
        {

            MessageBox.Show("Já se passaram 15 (Quinze) Minutos sem você enviar o plano de vôo! Você terá mais 15 (Quinze Minutos para poder enviar o plano, caso contratio o twMerco será fechado juntamente com FSX!", "",
               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
          /*  MySqlConnection cadastra_tecnico = new MySqlConnection();
            MySqlCommand comando_tecnico = new MySqlCommand();

            cadastra_tecnico.ConnectionString = ("server=localhost;User=root;pwd=wireless;database=ordem_servico");
            cadastra_tecnico.Open();

            try
            {
                comando_tecnico.Connection = cadastra_tecnico;
                comando_tecnico.CommandText = ("INSERT INTO chat_atc_twMerco (nome_tecnico,tel_tecnico,email_tecnico) VALUES (@nome_tecnico,@tel_tecnico,@email_tecnico)");
                comando_tecnico.Parameters.AddWithValue("@nome_tecnico", txtMsgChatATC.Text);
              

                MessageBox.Show("Cadastrado com Sucesso");

                dataGridChat.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro: " + ex.Message);
            }
            finally
            {
                cadastra_tecnico.Close();
            }

           /* txt_emailTecnico.Text = "";
            txt_nomeTecnico.Text = "";
            txt_telTecnico.Text = "";

            txt_nomeTecnico.Focus();*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            my_simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            //label_status.Text = "Aguardando conexão...";

            // Process the default group
            try
            {
                FSUIPCConnection.Process();


                // IAS - Simple integer returned so just divide as per the 
                // FSUIPC documentation for this offset and display the result.
                double airpeedKnots = ((double)airspeed.Value / 128d);
                //this.txtIAS.Text = airpeedKnots.ToString("f1");


                // Avionics Master Switch
               // this.chkAvionics.Checked = (avionics.Value > 0);  // 0 = Off, 1 = On.


                // Advanced Concept: Reading Raw Blocks of Data.
                // FS Local Date and Time
                // This demonstrates getting back an arbitrary number of bytes from an offset.
                // Here we're getting 10 back from Offset 0x0328 which contain info about the 
                // Local date and time in FS.
                // Because it's returned as a byte array we need to handle everything ourselves...
                // 1. Year (starts at Byte 8) for 2 bytes. (Int16)
                //    Use the BitConverter class to get it into a native Int16 variable
                short year = BitConverter.ToInt16(fsLocalDateTime.Value, 8);
                //    You could also do it manually if you know about such things...
                //    short year = (short)(fsLocalDateTime.Value[8] + (fsLocalDateTime.Value[9] * 0x100));
                // 2. Make new datetime with the the time value at 01/01 of the year...
                //    Time - in bytes 0,1 and 2. (Hour, Minute, Second):
                DateTime fsTime = new DateTime(year, 1, 1, fsLocalDateTime.Value[0], fsLocalDateTime.Value[1], fsLocalDateTime.Value[2]);
                // 3. Get the Day of the Year back (not given as Day and Month) 
                //    and add this on to the Jan 1 date we created above 
                //    to give the final date:
                short dayNo = BitConverter.ToInt16(fsLocalDateTime.Value, 6);
                fsTime = fsTime.Add(new TimeSpan(dayNo - 1, 0, 0, 0));
                // Now print it out
                this.tsstlHoraFSX.Text = fsTime.ToString("dddd, MMMM dd yyyy hh:mm:ss");


                // Lights
                // This demonstrates using the BitArray type to handle
                // a bit field type offset.  The lights are a 2 byte (16bit) bit field 
                // starting in offset 0D0C.
                // To make the code clearer and easier to write in the first
                // place - I created a LightType Enum (bottom of this file).
                // You could of course just use the literal values 0-9 if you prefer.
                // For the first three, I've put alternative lines in comments
                // that use a literal indexer instead of the enum.
                // Update each checkbox according to the relevent bit in the BitArray...
                this.chkBeacon.Checked = lights.Value[(int)LightType.Beacon];
                //this.chkBeacon.Checked = lights.Value[1];
                this.chkCabin.Checked = lights.Value[(int)LightType.Cabin];
                //this.chkCabin.Checked = lights.Value[9];
                this.chkInstuments.Checked = lights.Value[(int)LightType.Instruments];
                //this.chkInstuments.Checked = lights.Value[5];
                this.chkLanding.Checked = lights.Value[(int)LightType.Landing];
                this.chkLogo.Checked = lights.Value[(int)LightType.Logo];
                this.chkNavigation.Checked = lights.Value[(int)LightType.Navigation];
                this.chkRecognition.Checked = lights.Value[(int)LightType.Recognition];
                this.chkStrobes.Checked = lights.Value[(int)LightType.Strobes];
                this.chkTaxi.Checked = lights.Value[(int)LightType.Taxi];
                this.chkWing.Checked = lights.Value[(int)LightType.Wing];

                // COM2 frequency
            // Shows decoding a DCD frequency to a string
            // a. Convert to a string in Hexadecimal format
            string com2String = com2bcd.Value.ToString("X");
            // b. Add the assumed '1' and insert the decimal point
            com2String = "1" + com2String.Substring(0, 2) + "." + com2String.Substring(2, 2);
            this.txtCom2Ativo.Text = com2String;
          

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
                    this.button_Connect.Enabled = true;
                    
                   // this.AIRadarTimer.Enabled = false;
                    FSUIPCConnection.Close();
                    MessageBox.Show("A conexão com FSX foi perdida.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void btnEnviaChat_Click(object sender, EventArgs e)
        {
            string ICAOoperante = txtDepartureAirport.Text;
            string freqOperante = txtCom2Ativo.Text;
            string CallSing = txtCallSing.Text;
            string mensagem = rtxtBoxMsgATC.Text;
            string ip = PegarIPExterno();
            string strFormato = string.Empty;
            strFormato = DateTime.Now.Date.ToString("dd-MM-yyyy");
            string hora = DateTime.Now.ToShortTimeString();
            // Início da Conexão com indicação de qual o servidor, nome de base de dados e utilizar

            /* É aconselhável criar um utilizador com password. Para acrescentar a password é somente
            necessário acrescentar o seguinte código a seguir ao uid=root;password=xxxxx*/

            mConn = new MySqlConnection("Persist Security Info=False;server=localhost;database=xxx;uid=xxx;pwd=xxx");

            // Abre a conexão
            mConn.Open();
            //"INSERT INTO `chat_atc_twMerco`  VALUES "
            //Query SQL
            MySqlCommand command = new MySqlCommand("INSERT INTO chat_atc_twMerco (`id`, `pilotId`, `paraATCtipo`, `icao`, `frequencia`, `menssagem`, `ip`, `dia`, `hora`, `ativa`)" +
                                                    "VALUES('','" + CallSing + "', 'PILOTO', '" + ICAOoperante + "', '" + freqOperante + "', '" + mensagem + "', '" + ip + "', '" + strFormato + "', '" + hora + "', '1' )", mConn);

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

        private void btnIniciaControle_Click(object sender, EventArgs e)
        {
            string ICAOsearchChat = txtDepartureAirport.Text;
            



            string stringConnection = "Persist Security Info=False;server=localhost;database=xxx;uid=xxx;pwd=xxx";

            string ICAOoperante = txtDepartureAirport.Text;
           // string freqOperante = txtFrequenciaOperar.Text;

            MySqlConnection connection = new MySqlConnection(stringConnection);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "SELECT * FROM `frequencias_aeroportos` WHERE `airport_ident` = '" + ICAOoperante + "' ORDER BY frequency_mhz ASC LIMIT 1";
            connection.Open();
            Reader = command.ExecuteReader();
            // listBox1.Items.Clear();
            richtextboxChat.Clear();
            while (Reader.Read())
            {
                // listBox1.Items.Add("");

                //  listBox1.Items.Add("" + Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ")");


                // richTextBox2.Text = "";
                //comboBox1.Items.Add(Reader[2].ToString() + "(" + Reader[3].ToString() + " -" + Reader[5].ToString() + ")");
                
                //richtextboxChat.Text += String.Format(Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ") \n", Environment.NewLine);
                comboBox1.Items.Add(Reader[2].ToString());
            }
            connection.Close();

            
            timerChatPiloto.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            comboBox2.Visible = true;

           
            string ICAO = this.comboBox1.Text;

            string stringConnection = "Persist Security Info=False;server=localhost;database=xxx;uid=xxx;pwd=xxx";

            string ICAOoperante = txtDepartureAirport.Text;
            // string freqOperante = txtFrequenciaOperar.Text;

            MySqlConnection connection = new MySqlConnection(stringConnection);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "SELECT * FROM `frequencias_aeroportos` WHERE `airport_ident` = '" + ICAO + "' ORDER BY frequency_mhz ASC";
            connection.Open();
            Reader = command.ExecuteReader();
            // listBox1.Items.Clear();
            richtextboxChat.Clear();
            while (Reader.Read())
            {
                // listBox1.Items.Add("");

                //  listBox1.Items.Add("" + Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ")");


                // richTextBox2.Text = "";
                //comboBox1.Items.Add(Reader[2].ToString() + "(" + Reader[3].ToString() + " -" + Reader[5].ToString() + ")");

                //richtextboxChat.Text += String.Format(Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ") \n", Environment.NewLine);
                comboBox2.Items.Add(Reader[3].ToString());
            }
            connection.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {


            txtFreqChat.Visible = true;            
            txtFreqChat.BackColor = Color.Black;
            txtFreqChat.ForeColor = Color.LightGreen;


            string ICAO = this.comboBox1.Text;

            string stringConnection = "Persist Security Info=False;server=localhost;database=xxx;uid=xxx;pwd=xxx";

            string ICAOoperante = txtDepartureAirport.Text;
            // string freqOperante = txtFrequenciaOperar.Text;

            MySqlConnection connection = new MySqlConnection(stringConnection);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "SELECT * FROM `frequencias_aeroportos` WHERE `airport_ident` = '" + ICAO + "' ORDER BY frequency_mhz ASC";
            connection.Open();
            Reader = command.ExecuteReader();
            // listBox1.Items.Clear();
            richtextboxChat.Clear();
            while (Reader.Read())
            {
                // listBox1.Items.Add("");

                //  listBox1.Items.Add("" + Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ")");


                // richTextBox2.Text = "";
                //comboBox1.Items.Add(Reader[2].ToString() + "(" + Reader[3].ToString() + " -" + Reader[5].ToString() + ")");

                //richtextboxChat.Text += String.Format(Reader[1].ToString() + " -> " + Reader[5].ToString() + " (" + Reader[7].ToString() + " - " + Reader[8].ToString() + ") \n", Environment.NewLine);
                 
                txtFreqChat.Text = Reader[5].ToString();
            }
            connection.Close();

        }

        private void btnFinalizaControle_Click(object sender, EventArgs e)
        {
            if (!bgwChatPiloto.IsBusy)
                bgwChatPiloto.CancelAsync();
        }


        void bgwChatPiloto_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


            string stringConnection = "Persist Security Info=False;server=localhost;database=xxxx;uid=xxxx;pwd=xxxx";

            string ICAOoperante = txtDepartureAirport.Text;
            string freqOperante = txtCom2Ativo.Text;
            string CallSing = txtCallSing.Text;
            string mensagem = rtxtBoxMsgATC.Text;
            string ip = PegarIPExterno();
            string strFormato = string.Empty;
            strFormato = DateTime.Now.Date.ToString("dd-MM-yyyy");
            string hora = DateTime.Now.ToShortTimeString();

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

       

       

        private void bgwChatPiloto_DoWork(object sender, DoWorkEventArgs e)
        {
            //Do work process here.        
            // toolStripStatusLabel1.Text = "Background Worker do their work now";

            // MessageBox.Show("Background Worker do their work now");

            Thread.Sleep(1000);//sleep 10 seconds

        }

        private void timerChatPiloto_Tick(object sender, EventArgs e)
        {
            BackgroundWorker bgwChatATC = new BackgroundWorker();
            bgwChatATC.DoWork += new DoWorkEventHandler(bgwChatPiloto_DoWork);
            bgwChatATC.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwChatPiloto_RunWorkerCompleted);

            bgwChatATC.RunWorkerAsync();
        }



        /*  PARTE DO CÓDIGO DESTINADA AO K-MERCO PARA MERCOSUL LINHAS AÉREAS VIRTUAIS  */


        private void TmrGetDataFromFs_Tick(System.Object sender, System.EventArgs e)
        {
            FsuipcData.drivestarttmr();
            
        }


        /* FIM DA PARTE DO CÓDIGO DESTINADA AO K-MERCO */



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

