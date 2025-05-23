using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace AI_form2
{
    public class CRicetta
    {
        public int ID { get; set; }
        public string title { get; set; }
        public List<string> ingredients { get; set; }
        public List<string> directions { get; set; }
        public string link { get; set; }
        public string source { get; set; }
        public List<string> ner { get; set; }
        public bool liked { get; set; }

        public CRicetta(int ID, string title, List<string> ingredients, List<string> directions, string link, string source, List<string> ner)
        {
            this.ID = ID;
            this.title = title;
            this.ingredients = ingredients;
            this.directions = directions;
            this.link = link;
            this.source = source;
            this.ner = ner;
            liked = false;
        }

    }

    public class RicettaInput
    {
        public int ID { get; set; }
        public float NumeroIngredienti { get; set; }
        public float NumeroPassaggi { get; set; }

        //Attributo speciale che indica che si tratta di un array di features
        [VectorType(50)]
        public float[] IngredientiVector { get; set; }

        public bool Label { get; set; }

        public RicettaInput() { }

        public RicettaInput(int id, float numeroIngredienti, float numeroPassaggi, float[] ingredientiVector, bool label)
        {
            ID = id;
            NumeroIngredienti = numeroIngredienti;
            NumeroPassaggi = numeroPassaggi;
            IngredientiVector = ingredientiVector;
            Label = label;
        }
    }

    public class RicettaPrediction
    {
        //Per mappare le proprietà della classe con i nomi delle colonne nel modello ML
        [ColumnName("PredictedLabel")]
        public bool predictedLabel { get; set; }

        [ColumnName("Probability")]
        public float probability { get; set; }

        [ColumnName("Score")]
        public float score { get; set; }
    }
}
