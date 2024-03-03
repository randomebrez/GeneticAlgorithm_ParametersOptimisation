using GeneticAlgorithm.Genomic.Model;

namespace GeneticAlgorithm.SimulationRun.Model
{
    public class SimulationEnvironment
    {
        public int Id { get; set; }
        public Genome Genome { get; set; }
        public GlobalParameters Parameters { get; set; }

        public double Score { get; set; }
    }
}
