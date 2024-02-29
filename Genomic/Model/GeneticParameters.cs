namespace GeneticAlgorithm.Genomic.Model
{
    public class GeneticParameters
    {
        public int GenerationNumber { get; set; } = 1;
        public int GenomeNumber { get; set; }
        public int ParentGenomeNumber { get; set; }
        public double MutationRate { get; set; }
    }
}
