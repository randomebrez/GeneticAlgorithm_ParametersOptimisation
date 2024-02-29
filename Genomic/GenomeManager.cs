using GeneticAlgorithm.Genomic.Model;

namespace GeneticAlgorithm.Genomic
{
    public class GenomeManager
    {
        public Dictionary<string, double> TranslateGenome(Genome genome)
        {
            var translatedGenome = new Dictionary<string, double>();

            foreach (var geneKey in genome.GeneKeysList)
                translatedGenome.Add(geneKey, genome.GeneGetByParameter(geneKey).LinearValueGet());

            return translatedGenome;
        }

        public List<Genome> RandomGenomesGet(int genomeNumber, List<ParameterDetails> parameters)
        {
            var genomes = new List<Genome>();

            for (var i = 0; i < genomeNumber; i++)
                genomes.Add(new Genome(parameters, 10));

            return genomes;
        }

        public List<Genome> ReproduceFromGenome(List<Genome> genomes, double geneMutationProbability)
        {
            var result = new List<Genome>();

            while (genomes.Count > 1)
            {
                var randomNumber = new Random().Next(genomes.Count);
                var parent_A = genomes[randomNumber];
                genomes.RemoveAt(randomNumber);

                randomNumber = new Random().Next(genomes.Count);
                var parent_B = genomes[randomNumber];
                genomes.RemoveAt(randomNumber);

                var children = parent_A.CrossOver(parent_B);
                parent_A.Mutate(geneMutationProbability);
                result.Add(parent_A);
                parent_B.Mutate(geneMutationProbability);
                result.Add(parent_B);
                foreach (var child in children)
                {
                    child.Mutate(geneMutationProbability);
                    result.Add(child);
                }
            }

            if (genomes.Count == 1)
            {
                var children = genomes[0].CrossOver(genomes[0]);
                genomes[0].Mutate(geneMutationProbability);
                result.Add(genomes[0]);
                foreach (var child in children)
                {
                    child.Mutate(geneMutationProbability);
                    result.Add(child);
                }
            }

            return result;
        }
    }
}
