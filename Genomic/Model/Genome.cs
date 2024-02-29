using System.Text;

namespace GeneticAlgorithm.Genomic.Model
{
    public class Genome
    {
        private Dictionary<string, Gene> _genes { get; set; }


        public Genome(List<ParameterDetails> parametersToEncode, int bitNumber)
        {
            _genes = new Dictionary<string, Gene>();
            foreach (var parameter in parametersToEncode)
            {
                var newGene = new Gene(parameter, bitNumber);
                newGene.RandomInitialization();
                _genes.Add(newGene.ParameterName, newGene);
            }
        }

        public Genome(List<Gene> genes)
        {
            _genes = new Dictionary<string, Gene>();
            foreach (var gene in genes)
                _genes.Add(gene.ParameterName, gene);
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var gene in _genes)
                result.Append($"{gene.Key} : {gene.Value.LinearValueGet()} - ");

            return result.ToString();
        }
        public List<string> GeneKeysList => _genes.Keys.ToList();


        public Gene GeneGetByParameter(string parameterName)
        {
            if (_genes.ContainsKey(parameterName) == false)
                throw new Exception($"Unknown parameter name : {parameterName}");
            return _genes[parameterName];
        }

        public int Mutate(double probabilityByGene)
        {
            int mutationCounter = 0;

            foreach (var gene in _genes)
            {
                var randomNumber = new Random().Next(10000) / 10000f;
                if (randomNumber < probabilityByGene)
                {
                    gene.Value.Mutate();
                    mutationCounter++;
                }
            }

            return mutationCounter;
        }

        public List<Genome> CrossOver(Genome genomeB, int crossOverNumber = 1)
        {
            var A_child = new List<Gene>();
            var B_child = new List<Gene>();

            // Binomial law with average of first success around (GenomeLength/crossOverNumber) : Loi de poisson
            var crossOverTreshold = (float)(crossOverNumber + 1) / _genes.Count;
            bool crossOverOccured = false;
            foreach (var gene_key in _genes.Keys)
            {
                if (crossOverOccured)
                {
                    A_child.Add(new Gene(genomeB.GeneGetByParameter(gene_key)));
                    B_child.Add(new Gene(GeneGetByParameter(gene_key)));
                }
                else
                {
                    A_child.Add(new Gene(GeneGetByParameter(gene_key)));
                    B_child.Add(new Gene(genomeB.GeneGetByParameter(gene_key)));

                    var randomNumber = new Random().Next(10000) / 10000f;
                    if (randomNumber < crossOverTreshold)
                        crossOverOccured = true;
                }
            }

            var result = new List<Genome>
            {
                new Genome(A_child),
                new Genome(B_child)
            };

            return result;
        }
    }
}
