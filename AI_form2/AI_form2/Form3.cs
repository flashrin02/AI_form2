using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI_form2
{
    public partial class Form3 : Form
    {
        private CRicetta ricetta;
        public Form3(CRicetta r)
        {
            InitializeComponent();
            ricetta = r;
            MostraDettagli();
        }

        private void MostraDettagli()
        {
            

            label1.Text = ricetta.title;
            label2.Text = string.Join(", ", ricetta.ingredients);
            label3.Text = string.Join(", ", ricetta.directions);
            label4.Text = ricetta.link;

            label1.Dock = DockStyle.Top;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Font = new Font("Segoe UI", 16, FontStyle.Bold);

            label2.Dock = DockStyle.Top;
            label2.TextAlign = ContentAlignment.MiddleCenter;

            label3.Dock = DockStyle.Top;
            label3.TextAlign = ContentAlignment.MiddleCenter;

            label4.Dock = DockStyle.Top;
            label4.TextAlign = ContentAlignment.MiddleCenter;

            label1.Text = ricetta.title;
            label2.Text = string.Join(", ", ricetta.ingredients);
            label3.Text = string.Join(", ", ricetta.directions);
            label4.Text = ricetta.link;

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ricetta.link,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Impossibile aprire il link.");
            }
        }

        private async void pictureBox4_Click(object sender, EventArgs e)
        {
            pictureBox4.Visible = false;
            pictureBox1.Visible = false;
            pictureBoxSpinner.Visible = true;

            await Task.Run(() => GestoreRicette.RegistraFeedback(ricetta, true));

            pictureBoxSpinner.Visible = false;
            MessageBox.Show("Feedback registrato: Ti è piaciuta la ricetta!");
            this.Close();
        }

        private async void pictureBox1_Click(object sender, EventArgs e)
        {

            pictureBox4.Visible = false;
            pictureBox1.Visible = false;
            pictureBoxSpinner.Visible = true;

            await Task.Run(() => GestoreRicette.RegistraFeedback(ricetta, false));

            pictureBoxSpinner.Visible = false;
            MessageBox.Show("Feedback registrato: Non ti è piaciuta la ricetta.");
            this.Close();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
