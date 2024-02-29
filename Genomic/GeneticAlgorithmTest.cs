using GeneticAlgorithm.Genomic.Model;

namespace GeneticAlgorithm.Genomic
{
    public class GeneticAlgorithmTest
    {
        public static double EvaluateGenome(Genome genome)
        {
            var x_value = genome.GeneGetByParameter("x").LinearValueGet();
            var y_value = genome.GeneGetByParameter("y").LinearValueGet();


            return function_2(x_value, y_value);
        }

       

        public static double function_2(double x, double y)
        {
            return Math.Sin(x) * Math.Sin(y);
        }

        public struct ScoredGenome
        {
            public Genome Genome { get; set; }
            public double Score { get; set; }
        }



        public void TestGeneticAlgorithm()
        {
            var parameters = new List<ParameterDetails>
            {
                new ParameterDetails
                {
                    Name = "x",
                    MinValue = -10,
                    MaxValue = 10,
                    ValueType = DataTypeEnum.Float
                },
                new ParameterDetails
                {
                    Name = "y",
                    MinValue = -10,
                    MaxValue = 10,
                    ValueType = DataTypeEnum.Float
                }
            };


            var genomes = new List<Genome>();

            for (var i = 0; i < 500; i++)
                genomes.Add(new Genome(parameters, 30));

            var maternity = new GenomeManager();

            var currentIteration = 0;
            var bestScore = -1000d;
            var bestGenomes = new List<ScoredGenome>();
            while (1 - bestScore > 0.0001)
            {
                bestGenomes.Clear();
                foreach (var genome in genomes)
                {
                    var score = EvaluateGenome(genome);
                    bestGenomes.Add(new ScoredGenome { Genome = genome, Score = score });
                    bestGenomes = bestGenomes.OrderByDescending(t => t.Score).ToList();
                }
                bestScore = bestGenomes[0].Score;
                currentIteration++;
                if (currentIteration % 50 == 0)
                {
                    Console.WriteLine($"Iteration : {currentIteration} \nBest Score : {bestScore} | {bestGenomes[0].Genome}\nWorst Score : {bestGenomes.Last().Score}\n");
                    Console.WriteLine("");
                }

                genomes = maternity.ReproduceFromGenome(bestGenomes.Select(t => t.Genome).Take(100).ToList(), 0.2f);
            }

            Console.WriteLine($"Iteration required : {currentIteration}\nBest Score : {bestScore} | {bestGenomes[0].Genome}");
        }
    }
}
