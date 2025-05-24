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
    public partial class Form2 : Form
    {

        List<(CRicetta ricetta, int score, bool suggerita, float probabilita)> suggerimenti;
        public Form2()
        {
            InitializeComponent();
            this.Shown += Form2_Shown;
            this.Width = 440; // larghezza iniziale
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            this.ActiveControl = null; // Nessun controllo selezionato all’avvio
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private async void pictureBox2_ClickAsync(object sender, EventArgs e)
        {

            string input = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            pictureBoxSpinner.Visible = true;

            var lista = input.Split(',')
                             .Select(p => p.Trim().ToLower())
                             .Where(p => !string.IsNullOrWhiteSpace(p))
                             .ToList();

            // Trova le ricette in un thread secondario
            suggerimenti = await Task.Run(() => GestoreRicette.TrovaRicette(lista));

            listBox1.Items.Clear();
            foreach (var (r, s, suggerita, probabilita) in suggerimenti)
            {
                string messaggio = $"{r.title} (ingredienti in comune: {s})";
                messaggio += $" - probabilità: {probabilita}";
                if (suggerita) messaggio += " (potrebbe piacerti)";
                listBox1.Items.Add(messaggio);
            }

            pictureBoxSpinner.Visible = false;
            if(listBox1.Items.Count > 0)
            {
               this.Width = 900;
            }
            else
            {
                MessageBox.Show("Nessuna ricetta trovata");
            }
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            int sel = listBox1.SelectedIndex;
            if (sel == -1) return;

            var ricetta = suggerimenti[sel].ricetta;
            Form3 f3 = new Form3(ricetta);
            f3.Show();
        }

        private void pictureBoxSpinner_Click(object sender, EventArgs e)
        {

        }
    }
}
