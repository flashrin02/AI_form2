using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using Microsoft.ML;
using Newtonsoft.Json;

namespace AI_form2
{
    public static class GestoreRicette
    {
        public static List<CRicetta> Ricette { get; private set; }
        public static List<RicettaInput> TrainingSet { get; private set; } 
        public static List<RicettaInput> TestSet { get; private set; } 
        public static List<string> Vocabolario { get; private set; } 

        private static readonly string pathDataset = "full_dataset_test.csv";
        private static readonly string pathPreferenze = "preferences.json";
        private static readonly MLContext mlContext;

        public static ITransformer Modello { get; private set; }   //Modello ML addestrato
        public static PredictionEngine<RicettaInput, RicettaPrediction> PredictionEngine { get; private set; }      //Permette di fare previsioni

        static GestoreRicette()
        {
            Ricette = new();
            TrainingSet = new();
            TestSet = new();
            Vocabolario = new();
            mlContext = new(seed: 0);     //Stessi input => stessi risultati

            CaricaRicette();
            CostruisciVocabolario();
            CaricaPreferenze();
            DividiDataset();
            AddestraModello();
        }

        private static void CaricaRicette()
        {
            Ricette.Clear();
            if (File.Exists(pathDataset))
            {
                HashSet<int> ricetteCaricate = new();   //Per evitare duplicati
                //Lettura file csv
                using var sr = new StreamReader(pathDataset);
                using var csv = new CsvReader(sr, CultureInfo.InvariantCulture);

                csv.Read();     //Legge la prima riga del csv
                csv.ReadHeader();   //Indica che la riga letta è l'intestazione

                while (csv.Read())
                {
                    try
                    {
                        int ID = csv.GetField<int>(0);

                        //Salta se ID è già presente
                        if (ricetteCaricate.Contains(ID))
                            continue;

                        string title = csv.GetField<string>(1).Trim();
                        List<string> ingredients = JsonConvert.DeserializeObject<List<string>>(csv.GetField<string>(2));
                        List<string> directions = JsonConvert.DeserializeObject<List<string>>(csv.GetField<string>(3));
                        string link = csv.GetField<string>(4);
                        string source = csv.GetField<string>(5);
                        List<string> ner = JsonConvert.DeserializeObject<List<string>>(csv.GetField<string>(6));

                        if (string.IsNullOrEmpty(title) || ingredients.Count == 0 || directions.Count == 0 ||
                            string.IsNullOrEmpty(link) || string.IsNullOrEmpty(source) || ner.Count == 0)
                            continue;

                        Ricette.Add(new CRicetta(ID, title, ingredients, directions, link, source, ner));
                        ricetteCaricate.Add(ID);    //Registra l'ID
                    }
                    catch { }
                }
            }
        }

        private static void CostruisciVocabolario()
        {
            List<(string ingrediente, int conteggio)> listaIngredienti = new();

            //Conta le occorrenze
            foreach (var r in Ricette)
            {
                foreach (var ing in r.ner)
                {
                    string ingrediente = ing.Trim().ToLower();

                    //Cerca se è già presente
                    bool trovato = false;
                    for (int i = 0; i < listaIngredienti.Count; i++)
                    {
                        if (listaIngredienti[i].ingrediente == ingrediente)
                        {
                            listaIngredienti[i] = (ingrediente, listaIngredienti[i].conteggio + 1);
                            trovato = true;
                            break;
                        }
                    }

                    if (!trovato)
                    {
                        listaIngredienti.Add((ingrediente, 1));
                    }
                }
            }

            //Ordina per frequenza decrescente
            for (int i = 0; i < listaIngredienti.Count - 1; i++)
            {
                for (int j = i + 1; j < listaIngredienti.Count; j++)
                {
                    if (listaIngredienti[j].conteggio > listaIngredienti[i].conteggio)
                    {
                        var temp = listaIngredienti[i];
                        listaIngredienti[i] = listaIngredienti[j];
                        listaIngredienti[j] = temp;
                    }
                }
            }

            //Prende i primi 50 ingredienti più comuni
            for (int i = 0; i < 50 && i < listaIngredienti.Count; i++)
            {
                Vocabolario.Add(listaIngredienti[i].ingrediente);
            }
        }

        private static void CaricaPreferenze()
        {
            TrainingSet.Clear();
            if (File.Exists(pathPreferenze))
            {
                string json = File.ReadAllText(pathPreferenze);
                TrainingSet = JsonConvert.DeserializeObject<List<RicettaInput>>(json);
            }
        }

        private static void SalvaPreferenze()
        {
            string json = JsonConvert.SerializeObject(TrainingSet, Formatting.Indented);
            File.WriteAllText(pathPreferenze, json);
        }

        private static void DividiDataset()
        {
            if (Ricette.Count == 0)
            {
                return;
            }

            //Carica gli oggetti RicettaInput, ottenuti trasformando CRicetta in RicettaInput, in IDataView
            IDataView fullData = mlContext.Data.LoadFromEnumerable(Ricette.Select(CreaInputDaRicetta));

            //Divisione in training set e test set 
            var splitData = mlContext.Data.TrainTestSplit(fullData, testFraction: 0.2, seed: 123);

            //Converte il TestSet in lista di oggetti RicettaInput
            TestSet = mlContext.Data.CreateEnumerable<RicettaInput>(splitData.TestSet, reuseRowObject: false).ToList();
        }

        private static void AddestraModello()
        {
            //Se il training set è vuoto, viene aggiunto un esempio fittizio per evitare errori
            if (TrainingSet.Count == 0)
                TrainingSet.Add(new RicettaInput(-1, 0, 0, new float[50], false));

            IDataView dataView = mlContext.Data.LoadFromEnumerable(TrainingSet);

            //Pipeline serve per preparare dati e addestrare il modello
            var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "NumeroIngredienti", "NumeroPassaggi" })    //Unisce le 2 colonne NumeroIngredienti e NumeroPassaggi in un'unica -> Features
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))           //Normalizzazione delle feature 
                .Append(mlContext.Transforms.Concatenate("FeaturesFinal", new[] { "Features", "IngredientiVector" }))       //Unisce alle feature anche gli ingredienti
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "FeaturesFinal"));          //Aggiunge l'algoritmo che impara i dati

            //Per creare un modello addestrato
            Modello = pipeline.Fit(dataView);
            //Per fare predizioni
            PredictionEngine = mlContext.Model.CreatePredictionEngine<RicettaInput, RicettaPrediction>(Modello);
        }

        private static RicettaInput CreaInputDaRicetta(CRicetta r)
        {
            float[] ingredientiVector = new float[Vocabolario.Count];
            for (int i = 0; i < Vocabolario.Count; i++)
            {
                string vocabTermine = Vocabolario[i];
                bool presente = false;

                foreach (var nerTermine in r.ner)
                {
                    if (nerTermine.Trim().ToLower() == vocabTermine)
                    {
                        presente = true;
                        break;
                    }
                }

                ingredientiVector[i] = presente ? 1f : 0f;
            }
            return new RicettaInput(r.ID, r.ingredients.Count, r.directions.Count, ingredientiVector, false);
        }

        public static List<(CRicetta ricetta, int score, bool suggerita, float probabilita)> TrovaRicette(List<string> ingredienti)
        {
            var ricette = RicetteConsigliate(ingredienti);

            List<(CRicetta, int, bool, float)> result = new();
            foreach (var r in ricette)
            {
                RicettaInput input = CreaInputDaRicetta(r);
                RicettaPrediction prediction = PredictionEngine.Predict(input);
                int score = ContaIngredientiComuni(r.ner, ingredienti);
                bool suggerita = RicettaSuggerita(r);
                float probabilita = prediction.probability;
                result.Add((r, score, suggerita, probabilita));
            }
            //Ordinamento
            for (int i = 0; i < result.Count - 1; i++)
            {
                for (int j = i + 1; j < result.Count; j++)
                {
                    //Ordina prima per suggerita (true prima di false)
                    if (result[j].Item3 && !result[i].Item3)
                    {
                        var temp = result[i];
                        result[i] = result[j];
                        result[j] = temp;
                    }
                    else if (result[j].Item3 == result[i].Item3)
                    {
                        //Se suggerita è uguale, ordina per score decrescente
                        if (result[j].Item2 > result[i].Item2)
                        {
                            var temp = result[i];
                            result[i] = result[j];
                            result[j] = temp;
                        }
                    }
                }
            }
            return result;
        }

        //Restituisce 10 ricette consigliate e ricercate
        public static List<CRicetta> RicetteConsigliate(List<string> ingredientiDisponibili)
        {
            HashSet<string> disponibiliSet = new();
            foreach (string i in ingredientiDisponibili)
            {
                disponibiliSet.Add(i.Trim().ToLower());
            }

            //Crea un set con gli ID delle ricette nel TestSet
            HashSet<int> testSetIds = new();
            foreach (RicettaInput r in TestSet)
            {
                testSetIds.Add(r.ID);
            }

            List<CRicetta> ricetteCompatibili = new();

            //Filtra ricette che contengono almeno un ingrediente disponibile
            foreach (CRicetta input in Ricette)
            {
                //Escludi le ricette nel TestSet
                if (testSetIds.Contains(input.ID))
                    continue;

                foreach (string ingrediente in input.ner)
                {
                    string ing = ingrediente.Trim().ToLower();
                    if (disponibiliSet.Contains(ing))
                    {
                        ricetteCompatibili.Add(input);
                        break;
                    }
                }
            }

            //Ordina per numero di ingredienti in comune (decrescente)
            for (int i = 0; i < ricetteCompatibili.Count - 1; i++)
            {
                for (int j = i + 1; j < ricetteCompatibili.Count; j++)
                {
                    int countI = ContaIngredientiComuni(ricetteCompatibili[i].ner, ingredientiDisponibili);
                    int countJ = ContaIngredientiComuni(ricetteCompatibili[j].ner, ingredientiDisponibili);
                    if (countJ > countI)
                    {
                        CRicetta temp = ricetteCompatibili[i];
                        ricetteCompatibili[i] = ricetteCompatibili[j];
                        ricetteCompatibili[j] = temp;
                    }
                }
            }

            //Restituisce al massimo 10 ricette
            List<CRicetta> risultato = new();
            int limite = ricetteCompatibili.Count < 10 ? ricetteCompatibili.Count : 10;
            for (int k = 0; k < limite; k++)
            {
                risultato.Add(ricetteCompatibili[k]);
            }

            return risultato;
        }

        private static int ContaIngredientiComuni(List<string> ingr1, List<string> ingr2)
        {
            var set1 = new HashSet<string>();
            foreach (string i in ingr1)
            {
                set1.Add(i.Trim().ToLower());
            }

            var set2 = new HashSet<string>();
            foreach (string i in ingr2)
            {
                set2.Add(i.Trim().ToLower());
            }

            //Lascia nel set1 solo gli elementi in comune a set2
            set1.IntersectWith(set2);
            return set1.Count;
        }

        public static bool RicettaSuggerita(CRicetta ricetta)
        {
            if (TrainingSet.Count < 2) return false;
            RicettaInput input = CreaInputDaRicetta(ricetta);
            //Usa il modello addestrato per fare una previsione sulla ricetta, restituendo true o false (predictedLabel)
            return PredictionEngine.Predict(input).predictedLabel;
        }

        public static void RegistraFeedback(CRicetta ricetta, bool piace)
        {
            RicettaInput input = CreaInputDaRicetta(ricetta);
            input.Label = piace;
            TrainingSet.Add(input);
            AddestraModello();
            SalvaPreferenze();
        }

        public static string ValutaModello()
        {
            if (TestSet.Count == 0 || Modello == null)
            {
                return "Il test set è vuoto o il modello non è stato addestrato";
            }

            IDataView dataView = mlContext.Data.LoadFromEnumerable(TestSet);
            //Applica il modello addestrato ai dati del test set, ottenendone le previsioni per ogni ricetta 
            var predictions = Modello.Transform(dataView);
            //Calcola le metriche di performance
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

            return $"Accuratezza: {metrics.Accuracy:P2}\n" +
                   $"Precisione: {metrics.PositivePrecision:P2}\n" +
                   $"Recall: {metrics.PositiveRecall:P2}\n" +
                   $"F1 Score: {metrics.F1Score:P2}\n";
        }

        public static List<string> TestaModelloConTrueFalse()
        {
            var risultati = new List<string>();

            if (Modello == null)
            {
                risultati.Add("Il modello non è stato addestrato");
                return risultati;
            }
            if (TestSet.Count == 0)
            {
                risultati.Add("Il test set è vuoto");
                return risultati;
            }

            IDataView testDataView = mlContext.Data.LoadFromEnumerable(TestSet);
            //Applica il modello addestrato ai dati del test set, ottenendone le previsioni per ogni ricetta 
            var predictions = Modello.Transform(testDataView);
            //Estrae i risultati in una lista
            var predictedResults = mlContext.Data.CreateEnumerable<RicettaPrediction>(predictions, reuseRowObject: false).ToList();

            for (int i = 0; i < predictedResults.Count; i++)
            {
                var result = predictedResults[i];
                if (!result.predictedLabel)
                    continue; //Salta se non è previsto che piaccia

                var input = TestSet[i];

                //Cerca nella lista delle ricette, la ricetta corrispondente
                CRicetta ricettaAssociata = null;
                foreach (var r in Ricette)
                {
                    if (r.ID == input.ID)  //Confronta gli ID
                    {
                        ricettaAssociata = r;
                        break;
                    }
                }

                string nomeRicetta = ricettaAssociata != null ? ricettaAssociata.title : $"Ricetta {i + 1}";
                risultati.Add(ricettaAssociata.title);
            }
            return risultati;
        }

        public static string TestaModelloConDebug()
        {
            var risultati = new List<string>();

            if (Modello == null)
            {
                return "Il modello non è stato addestrato";
            }

            if (TestSet.Count == 0)
            {
                return "Il test set è vuoto";
            }

            //Debug: controlla il training set
            int feedbackPositivi = TrainingSet.Count(t => t.Label == true);
            int feedbackNegativi = TrainingSet.Count(t => t.Label == false);

            string debug = $"Training Set: {TrainingSet.Count} ricette\n";
            debug += $"Feedback positivi: {feedbackPositivi}\n";
            debug += $"Feedback negativi: {feedbackNegativi}\n\n";

            IDataView testDataView = mlContext.Data.LoadFromEnumerable(TestSet.Take(10)); //Prende solo le prime 10 per test
            var predictions = Modello.Transform(testDataView);
            var predictedResults = mlContext.Data.CreateEnumerable<RicettaPrediction>(predictions, reuseRowObject: false).ToList();

            debug += "Previsioni su prime 10 ricette del test set:\n";

            for (int i = 0; i < predictedResults.Count && i < TestSet.Count; i++)
            {
                var result = predictedResults[i];
                var input = TestSet.Take(10).ToList()[i];

                CRicetta ricettaAssociata = Ricette.FirstOrDefault(r => r.ID == input.ID);
                string nomeRicetta = ricettaAssociata?.title ?? $"Ricetta {input.ID}";

                debug += $"- {nomeRicetta}: ";
                debug += $"Prevista gradita: {result.predictedLabel}, ";
                debug += $"Probabilità: {result.probability:P2}\n";
            }

            return debug;
        }
    }
}