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

        public static List<CRicetta> Ricette { get; private set; } = new();
        public static List<RicettaInput> TrainingSet { get; private set; } = new();
        public static List<string> Vocabolario { get; private set; } = new();

        private static readonly string pathDataset = "full_dataset_test.csv";
        private static readonly string pathPreferenze = "preferences.json";
        private static readonly MLContext mlContext = new(seed: 0);

        public static ITransformer Modello { get; private set; } = null!;
        public static PredictionEngine<RicettaInput, RicettaPrediction> PredictionEngine { get; private set; } = null!;

        static GestoreRicette()
        {
            CaricaRicette();
            CostruisciVocabolario();
            CaricaPreferenze();
            AddestraModello();
        }



        private static void CaricaRicette()
        {
            Ricette.Clear();

            if (File.Exists(pathDataset))
            {
                using var sr = new StreamReader(pathDataset);
                using var csv = new CsvReader(sr, CultureInfo.InvariantCulture);

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    try
                    {
                        int ID = csv.GetField<int>(0);
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
                    }
                    catch { }
                }
            }
        }

        private static void CostruisciVocabolario()
        {
            HashSet<string> tuttiIngredienti = new();
            foreach (var r in Ricette)
                foreach (var ing in r.ner)
                    tuttiIngredienti.Add(ing.Trim().ToLower());

            Vocabolario = tuttiIngredienti.Take(50).ToList();
        }

        private static RicettaInput CreaInputDaRicetta(CRicetta r)
        {
            var input = new RicettaInput
            {
                NumeroIngredienti = r.ingredients.Count,
                NumeroPassaggi = r.directions.Count,
                IngredientiVector = Vocabolario.Select(v => r.ner.Any(n => n.Trim().ToLower() == v) ? 1f : 0f).ToArray()
            };
            return input;
        }

        public static List<CRicetta> RicetteConsigliate(List<string> ingredientiDisponibili)
        {
            var disponibiliSet = new HashSet<string>(ingredientiDisponibili.Select(i => i.Trim().ToLower()));

            return Ricette
                .Where(r => r.ingredients.Any(i => disponibiliSet.Contains(i.Trim().ToLower())))
                .OrderByDescending(r => ContaIngredientiComuni(r.ingredients, ingredientiDisponibili))
                .Take(10)
                .ToList();
        }

        public static bool RicettaSuggerita(CRicetta ricetta)
        {
            if (TrainingSet.Count < 2) return false;
            var input = CreaInputDaRicetta(ricetta);
            return PredictionEngine.Predict(input).predictedLabel;
        }

        private static int ContaIngredientiComuni(List<string> ingr1, List<string> ingr2)
        {
            var set1 = new HashSet<string>(ingr1.Select(i => i.Trim().ToLower()));
            var set2 = new HashSet<string>(ingr2.Select(i => i.Trim().ToLower()));
            set1.IntersectWith(set2);
            return set1.Count;
        }

        public static void RegistraFeedback(CRicetta ricetta, bool piace)
        {
            var input = CreaInputDaRicetta(ricetta);
            input.Label = piace;
            TrainingSet.Add(input);
            AddestraModello();
            SalvaPreferenze();
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
            string json = JsonConvert.SerializeObject(TrainingSet);
            File.WriteAllText(pathPreferenze, json);
        }

        private static void AddestraModello()
        {
            if (TrainingSet.Count == 0)
                TrainingSet.Add(new RicettaInput(0, 0, new float[50], false));

            var dataView = mlContext.Data.LoadFromEnumerable(TrainingSet);

            var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "NumeroIngredienti", "NumeroPassaggi" })
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Transforms.Concatenate("FeaturesFinal", new[] { "Features", "IngredientiVector" }))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "FeaturesFinal"));

            Modello = pipeline.Fit(dataView);
            PredictionEngine = mlContext.Model.CreatePredictionEngine<RicettaInput, RicettaPrediction>(Modello);
        }


        public static List<(CRicetta ricetta, int score, bool suggerita)> TrovaRicette(List<string> ingredienti)
        {
            var ricette = RicetteConsigliate(ingredienti);

            var result = new List<(CRicetta, int, bool)>();
            foreach (var r in ricette)
            {
                int score = ContaIngredientiComuni(r.ingredients, ingredienti);
                bool suggerita = RicettaSuggerita(r);
                result.Add((r, score, suggerita));
            }

            return result;
        }

    }
}