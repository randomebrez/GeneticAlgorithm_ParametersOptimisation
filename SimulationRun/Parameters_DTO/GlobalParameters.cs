using GeneticAlgorithm.Genomic.Model;

namespace GeneticAlgorithm.SimulationRun.Parameters_DTO
{
    public class GlobalParameters
    {
        public int SimulationNumberByParameters { get; set; }
        public string StoragePath { get; set; }

        public GeneticParameters GeneticParameters { get; set; }

        public List<SearchableParameter> SimulationParameters { get; set; } = new List<SearchableParameter>();


        public GlobalParameters Copy()
        {
            var parametersCopy = SimulationParameters.Select(t => t.Copy()).ToList();
            return new GlobalParameters
            {
                SimulationNumberByParameters = SimulationNumberByParameters,
                StoragePath = StoragePath,
                GeneticParameters = GeneticParameters,
                SimulationParameters = parametersCopy
            };
        }
    }
}
