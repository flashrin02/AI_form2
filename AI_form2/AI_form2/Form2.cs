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
        
        List<(CRicetta ricetta, int score, bool suggerita)> suggerimenti;
        public Form2()
        {
            InitializeComponent();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            var lista = new List<string>();
            foreach (var part in input.Split(','))
                lista.Add(part.Trim().ToLower());

            suggerimenti = GestoreRicette.TrovaRicette(lista);

            listBox1.Items.Clear();
            foreach (var (r, s, suggerita) in suggerimenti)
            {
                string messaggio = $"{r.title} (ingredienti in comune: {s})";
                if (suggerita) messaggio += " (potrebbe piacerti)";
                listBox1.Items.Add(messaggio);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int sel = listBox1.SelectedIndex;
            if (sel == -1) return;

            var ricetta = suggerimenti[sel].ricetta;
            Form3 f3 = new Form3(ricetta);
            f3.Show();
        }
    }
}
