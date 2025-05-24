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
            //Mostra le informazioni di test
            string debugInfo = GestoreRicette.TestaModelloConDebug();
            MessageBox.Show(debugInfo, "Test del modello");

            //Valutazione del modello
            string risultato = GestoreRicette.ValutaModello();
            MessageBox.Show(risultato, "Valutazione del modello");
        }
    }
}
