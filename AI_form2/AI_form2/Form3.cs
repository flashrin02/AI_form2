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

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            GestoreRicette.RegistraFeedback(ricetta, true); // Pass the CRicetta object instead of its ID
            MessageBox.Show("Feedback registrato: Ti è piaciuta la ricetta!");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GestoreRicette.RegistraFeedback(ricetta, false); // Pass the CRicetta object instead of its ID
            MessageBox.Show("Feedback registrato: Non ti è piaciuta la ricetta.");
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
