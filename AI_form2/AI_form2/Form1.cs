namespace AI_form2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //var risultati = GestoreRicette.TestaModelloConTrueFalse();

            //if (risultati.Count == 0)
            //{
            //    MessageBox.Show("Nessun risultato disponibile.");
            //    return;
            //}

            //var messaggio = string.Join(Environment.NewLine, risultati);
            //MessageBox.Show(messaggio, "Test del modello");
            //string risultato = GestoreRicette.ValutaModello();
            //MessageBox.Show(risultato, "Valutazione del modello");

            //alessio per cercare di sistemare

            // Prima mostra le informazioni di debug
            string debugInfo = GestoreRicette.TestaModelloConDebug();
            MessageBox.Show(debugInfo, "Debug del modello");

            // Poi testa il modello normalmente
            var risultati = GestoreRicette.TestaModelloConTrueFalse();
            if (risultati.Count == 0)
            {
                MessageBox.Show("Nessuna ricetta prevista come gradita. Prova a dare più feedback positivi alle ricette!");
                return;
            }

            var messaggio = string.Join(Environment.NewLine, risultati);
            MessageBox.Show(messaggio, "Ricette che potrebbero piacerti");

            // Valutazione del modello
            string risultato = GestoreRicette.ValutaModello();
            MessageBox.Show(risultato, "Valutazione del modello");
        }
    }
}
