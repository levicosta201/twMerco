namespace twMerco
{
    partial class frmLogadoControle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.chkShowGroundAI = new System.Windows.Forms.CheckBox();
            this.chkShowAirborneAI = new System.Windows.Forms.CheckBox();
            this.cbxRadarRange = new System.Windows.Forms.ComboBox();
            this.chkEnableAIRadar = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtICAOsearchChat = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFrequenciaOperar = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AIObjectListView = new System.Windows.Forms.ListView();
            this.ObjectType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ESPModel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ATCModel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TailNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Airline = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FlightNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Latitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Longitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Altitude = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Heading = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GroundSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nav1FreqAtiv = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nav1FreqStby = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nav2FreqAtiv = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nav2FreqStby = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.transponderCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnFinalizaControle = new System.Windows.Forms.Button();
            this.richtextboxChat = new System.Windows.Forms.RichTextBox();
            this.btnIniciaControle = new System.Windows.Forms.Button();
            this.btnEnviaChat = new System.Windows.Forms.Button();
            this.rtxtBoxMsgATC = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnConnectar = new System.Windows.Forms.Button();
            this.AIRadarTimer = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.timerChat = new System.Windows.Forms.Timer(this.components);
            this.bgwChatATC = new System.ComponentModel.BackgroundWorker();
            this.radaATC = new twMerco.DoubleBufferPanel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1283, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1283, 579);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.radaATC);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.chkShowGroundAI);
            this.tabPage1.Controls.Add(this.chkShowAirborneAI);
            this.tabPage1.Controls.Add(this.cbxRadarRange);
            this.tabPage1.Controls.Add(this.chkEnableAIRadar);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1275, 553);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RADAR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Range (Nautical Miles)";
            // 
            // chkShowGroundAI
            // 
            this.chkShowGroundAI.AutoSize = true;
            this.chkShowGroundAI.Location = new System.Drawing.Point(344, 38);
            this.chkShowGroundAI.Name = "chkShowGroundAI";
            this.chkShowGroundAI.Size = new System.Drawing.Size(116, 17);
            this.chkShowGroundAI.TabIndex = 19;
            this.chkShowGroundAI.Text = "Aeronaves em solo";
            this.chkShowGroundAI.UseVisualStyleBackColor = true;
            // 
            // chkShowAirborneAI
            // 
            this.chkShowAirborneAI.AutoSize = true;
            this.chkShowAirborneAI.Checked = true;
            this.chkShowAirborneAI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowAirborneAI.Location = new System.Drawing.Point(220, 38);
            this.chkShowAirborneAI.Name = "chkShowAirborneAI";
            this.chkShowAirborneAI.Size = new System.Drawing.Size(115, 17);
            this.chkShowAirborneAI.TabIndex = 18;
            this.chkShowAirborneAI.Text = "Aeronaves em vôo";
            this.chkShowAirborneAI.UseVisualStyleBackColor = true;
            // 
            // cbxRadarRange
            // 
            this.cbxRadarRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRadarRange.FormattingEnabled = true;
            this.cbxRadarRange.Items.AddRange(new object[] {
            "50",
            "40",
            "30",
            "20",
            "10",
            "5",
            "1"});
            this.cbxRadarRange.Location = new System.Drawing.Point(133, 36);
            this.cbxRadarRange.Name = "cbxRadarRange";
            this.cbxRadarRange.Size = new System.Drawing.Size(65, 21);
            this.cbxRadarRange.TabIndex = 17;
            // 
            // chkEnableAIRadar
            // 
            this.chkEnableAIRadar.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkEnableAIRadar.Enabled = false;
            this.chkEnableAIRadar.Location = new System.Drawing.Point(8, 6);
            this.chkEnableAIRadar.Name = "chkEnableAIRadar";
            this.chkEnableAIRadar.Size = new System.Drawing.Size(440, 24);
            this.chkEnableAIRadar.TabIndex = 16;
            this.chkEnableAIRadar.Text = "ATIVAR RADAR";
            this.chkEnableAIRadar.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkEnableAIRadar.UseVisualStyleBackColor = true;
            this.chkEnableAIRadar.CheckedChanged += new System.EventHandler(this.chkEnableAIRadar_CheckedChanged_1);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.txtICAOsearchChat);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1275, 553);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "FREQUENCIAS";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(114, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = "BUSCAR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ICAO:";
            // 
            // txtICAOsearchChat
            // 
            this.txtICAOsearchChat.Location = new System.Drawing.Point(8, 19);
            this.txtICAOsearchChat.Name = "txtICAOsearchChat";
            this.txtICAOsearchChat.Size = new System.Drawing.Size(100, 20);
            this.txtICAOsearchChat.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 45);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1259, 502);
            this.dataGridView1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.txtFrequenciaOperar);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.textBox3);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.textBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1275, 553);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PLANO CONTROLADOR";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(248, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "FREQUÊNCIA:";
            // 
            // txtFrequenciaOperar
            // 
            this.txtFrequenciaOperar.Location = new System.Drawing.Point(333, 17);
            this.txtFrequenciaOperar.Name = "txtFrequenciaOperar";
            this.txtFrequenciaOperar.Size = new System.Drawing.Size(138, 20);
            this.txtFrequenciaOperar.TabIndex = 23;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.AIObjectListView);
            this.groupBox1.Location = new System.Drawing.Point(8, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1259, 503);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ONLINE NO MUTPLAYER";
            // 
            // AIObjectListView
            // 
            this.AIObjectListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AIObjectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ObjectType,
            this.ESPModel,
            this.ATCModel,
            this.TailNumber,
            this.Airline,
            this.FlightNumber,
            this.Latitude,
            this.Longitude,
            this.Altitude,
            this.Heading,
            this.GroundSpeed,
            this.nav1FreqAtiv,
            this.nav1FreqStby,
            this.nav2FreqAtiv,
            this.nav2FreqStby,
            this.transponderCode});
            this.AIObjectListView.FullRowSelect = true;
            this.AIObjectListView.HideSelection = false;
            this.AIObjectListView.Location = new System.Drawing.Point(3, 19);
            this.AIObjectListView.MultiSelect = false;
            this.AIObjectListView.Name = "AIObjectListView";
            this.AIObjectListView.Size = new System.Drawing.Size(1253, 478);
            this.AIObjectListView.TabIndex = 7;
            this.AIObjectListView.Tag = "Airplane";
            this.AIObjectListView.UseCompatibleStateImageBehavior = false;
            this.AIObjectListView.View = System.Windows.Forms.View.Details;
            // 
            // ObjectType
            // 
            this.ObjectType.Text = "Object Type";
            this.ObjectType.Width = 78;
            // 
            // ESPModel
            // 
            this.ESPModel.Text = "ESP Model";
            this.ESPModel.Width = 164;
            // 
            // ATCModel
            // 
            this.ATCModel.Text = "ATC Model";
            this.ATCModel.Width = 68;
            // 
            // TailNumber
            // 
            this.TailNumber.Text = "Tail Number";
            this.TailNumber.Width = 95;
            // 
            // Airline
            // 
            this.Airline.Text = "Airline";
            // 
            // FlightNumber
            // 
            this.FlightNumber.Text = "Flight Number";
            this.FlightNumber.Width = 84;
            // 
            // Latitude
            // 
            this.Latitude.Text = "Latitude";
            // 
            // Longitude
            // 
            this.Longitude.Text = "Longitude";
            // 
            // Altitude
            // 
            this.Altitude.Text = "Altitude";
            // 
            // Heading
            // 
            this.Heading.Text = "Heading";
            // 
            // GroundSpeed
            // 
            this.GroundSpeed.Text = "Ground Speed";
            this.GroundSpeed.Width = 83;
            // 
            // nav1FreqAtiv
            // 
            this.nav1FreqAtiv.Text = "NAV 1 ATIVO";
            // 
            // nav1FreqStby
            // 
            this.nav1FreqStby.Text = "NAV 1 STAND BY";
            // 
            // nav2FreqAtiv
            // 
            this.nav2FreqAtiv.Text = "NAV 2 ATIVO";
            // 
            // nav2FreqStby
            // 
            this.nav2FreqStby.Text = "NAV 2 STAND BY";
            // 
            // transponderCode
            // 
            this.transponderCode.Text = "Tansponder";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(477, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "TEMPO DE CONTROLE:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(611, 17);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "TEMPO DE CONTROLE:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(142, 17);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnFinalizaControle);
            this.tabPage4.Controls.Add(this.richtextboxChat);
            this.tabPage4.Controls.Add(this.btnIniciaControle);
            this.tabPage4.Controls.Add(this.btnEnviaChat);
            this.tabPage4.Controls.Add(this.rtxtBoxMsgATC);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1275, 553);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "CHAT";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnFinalizaControle
            // 
            this.btnFinalizaControle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFinalizaControle.Location = new System.Drawing.Point(465, 524);
            this.btnFinalizaControle.Name = "btnFinalizaControle";
            this.btnFinalizaControle.Size = new System.Drawing.Size(138, 23);
            this.btnFinalizaControle.TabIndex = 8;
            this.btnFinalizaControle.Text = "FINALIZAR CONTROLE";
            this.btnFinalizaControle.UseVisualStyleBackColor = true;
            this.btnFinalizaControle.Click += new System.EventHandler(this.btnFinalizaControle_Click);
            // 
            // richtextboxChat
            // 
            this.richtextboxChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richtextboxChat.Cursor = System.Windows.Forms.Cursors.No;
            this.richtextboxChat.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richtextboxChat.Location = new System.Drawing.Point(10, 6);
            this.richtextboxChat.Name = "richtextboxChat";
            this.richtextboxChat.Size = new System.Drawing.Size(1259, 454);
            this.richtextboxChat.TabIndex = 6;
            this.richtextboxChat.Text = "";
            // 
            // btnIniciaControle
            // 
            this.btnIniciaControle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIniciaControle.Location = new System.Drawing.Point(465, 495);
            this.btnIniciaControle.Name = "btnIniciaControle";
            this.btnIniciaControle.Size = new System.Drawing.Size(138, 23);
            this.btnIniciaControle.TabIndex = 3;
            this.btnIniciaControle.Text = "INICIAR CONTROLE";
            this.btnIniciaControle.UseVisualStyleBackColor = true;
            this.btnIniciaControle.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnEnviaChat
            // 
            this.btnEnviaChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEnviaChat.Location = new System.Drawing.Point(465, 466);
            this.btnEnviaChat.Name = "btnEnviaChat";
            this.btnEnviaChat.Size = new System.Drawing.Size(138, 23);
            this.btnEnviaChat.TabIndex = 2;
            this.btnEnviaChat.Text = "ENVIAR";
            this.btnEnviaChat.UseVisualStyleBackColor = true;
            this.btnEnviaChat.Click += new System.EventHandler(this.btnEnviaChat_Click);
            // 
            // rtxtBoxMsgATC
            // 
            this.rtxtBoxMsgATC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rtxtBoxMsgATC.Location = new System.Drawing.Point(8, 466);
            this.rtxtBoxMsgATC.Name = "rtxtBoxMsgATC";
            this.rtxtBoxMsgATC.Size = new System.Drawing.Size(451, 81);
            this.rtxtBoxMsgATC.TabIndex = 1;
            this.rtxtBoxMsgATC.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 635);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1283, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // btnConnectar
            // 
            this.btnConnectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnectar.Location = new System.Drawing.Point(12, 609);
            this.btnConnectar.Name = "btnConnectar";
            this.btnConnectar.Size = new System.Drawing.Size(75, 23);
            this.btnConnectar.TabIndex = 4;
            this.btnConnectar.Text = "CONECTAR";
            this.btnConnectar.UseVisualStyleBackColor = true;
            this.btnConnectar.Click += new System.EventHandler(this.btnConnectar_Click);
            // 
            // AIRadarTimer
            // 
            this.AIRadarTimer.Interval = 250;
            this.AIRadarTimer.Tick += new System.EventHandler(this.AIRadarTimer_Tick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDisconnect.Location = new System.Drawing.Point(93, 609);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(103, 22);
            this.buttonDisconnect.TabIndex = 5;
            this.buttonDisconnect.Text = "DESCONECTAR";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // timerChat
            // 
            this.timerChat.Interval = 5000;
            this.timerChat.Tick += new System.EventHandler(this.timerChat_Tick);
            // 
            // bgwChatATC
            // 
            this.bgwChatATC.WorkerSupportsCancellation = true;
            this.bgwChatATC.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // radaATC
            // 
            this.radaATC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radaATC.Location = new System.Drawing.Point(9, 63);
            this.radaATC.Name = "radaATC";
            this.radaATC.Size = new System.Drawing.Size(1258, 484);
            this.radaATC.TabIndex = 21;
            this.radaATC.Paint += new System.Windows.Forms.PaintEventHandler(this.radaATC_Paint);
            // 
            // frmLogadoControle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1283, 657);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.btnConnectar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmLogadoControle";
            this.Text = "twMerco || Controlador";
            this.Load += new System.EventHandler(this.frmLogadoControle_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnConnectar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkShowGroundAI;
        private System.Windows.Forms.CheckBox chkShowAirborneAI;
        private System.Windows.Forms.ComboBox cbxRadarRange;
        private System.Windows.Forms.CheckBox chkEnableAIRadar;
        private System.Windows.Forms.Timer AIRadarTimer;
        private System.Windows.Forms.Timer timer1;
        
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtICAOsearchChat;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView AIObjectListView;
        private System.Windows.Forms.ColumnHeader ObjectType;
        private System.Windows.Forms.ColumnHeader ESPModel;
        private System.Windows.Forms.ColumnHeader ATCModel;
        private System.Windows.Forms.ColumnHeader TailNumber;
        private System.Windows.Forms.ColumnHeader Airline;
        private System.Windows.Forms.ColumnHeader FlightNumber;
        private System.Windows.Forms.ColumnHeader Latitude;
        private System.Windows.Forms.ColumnHeader Longitude;
        private System.Windows.Forms.ColumnHeader Altitude;
        private System.Windows.Forms.ColumnHeader Heading;
        private System.Windows.Forms.ColumnHeader GroundSpeed;
        private System.Windows.Forms.ColumnHeader nav1FreqAtiv;
        private System.Windows.Forms.ColumnHeader nav1FreqStby;
        private System.Windows.Forms.ColumnHeader nav2FreqAtiv;
        private System.Windows.Forms.ColumnHeader nav2FreqStby;
        private System.Windows.Forms.ColumnHeader transponderCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox rtxtBoxMsgATC;
        private System.Windows.Forms.Button btnEnviaChat;
        private System.Windows.Forms.Button btnIniciaControle;
        private System.Windows.Forms.Timer timerChat;
        private System.ComponentModel.BackgroundWorker bgwChatATC;
        private System.Windows.Forms.Button btnFinalizaControle;
        public System.Windows.Forms.RichTextBox richtextboxChat;
        private System.Windows.Forms.TextBox txtFrequenciaOperar;
        private System.Windows.Forms.Label label6;
        private DoubleBufferPanel radaATC;
        
    }
}