using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;



namespace twMerco
{
    public partial class frmLogin : Form
    {

        


        public frmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string stringConnection = "Persist Security Info=False;server=localhots;database=dv;uid=root;pwd=";


            string login = txtLogin.Text;
            string senha = txtSenha.Text;

            if (!radButtonControle.Checked && !rdButtonPiloto.Checked)
            {
                MessageBox.Show("Favor selecionar como deseja simular: Torre ou Piloto.");
            }

            if (txtLogin.Text == "")
            {
                MessageBox.Show("Favor inserir seu Login");
            }
           
            if (txtSenha.Text == "")
            {
                MessageBox.Show("Favor inserir sua senha");
            }



            MySqlConnection connection = new MySqlConnection(stringConnection);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;

            if (radButtonControle.Checked)
            {
                command.CommandText = "SELECT * FROM `phpvms_pilots` WHERE `email` = '" + login + "' AND `twMercopass` = '" + senha + "' AND `permissao_atc` = 1";
                connection.Open();
                Reader = command.ExecuteReader();
                // listBox1.Items.Clear();

                if (Reader.Rows > 0)
                {

                    frmLogadoPiloto frmPiloto = new frmLogadoPiloto();
                    frmPiloto.Show();

                }
                else
                {
                    MessageBox.Show("Lamentamos o ocorrido, mas parece que você não tem permissão para pode controlar. Por favor contate um Staff.");
                }
            }
            else if (rdButtonPiloto.Checked)
            {

                command.CommandText = "SELECT * FROM `phpvms_pilots` WHERE `email` = '" + login + "' AND `twMercopass` = '" + senha + "'";
                connection.Open();
                Reader = command.ExecuteReader();
                // listBox1.Items.Clear();

                if (Reader.FieldCount > 0)
                {

                    frmLogadoPiloto frmPiloto = new frmLogadoPiloto();
                    frmPiloto.Show();

                }
                else
                {
                    MessageBox.Show("Parece que alguns de seus dados estão inválidos, tente novamente.");
                }

            }
           
           
            connection.Close();

        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

            

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmLogadoControle frmControle = new frmLogadoControle();
            frmControle.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmLogadoPiloto frmPiloto = new frmLogadoPiloto();
            frmPiloto.Show();
        }
    }
}
