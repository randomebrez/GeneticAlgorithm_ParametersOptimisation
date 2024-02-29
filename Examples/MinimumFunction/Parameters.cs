using GeneticAlgorithm.SimulationRun.Parameters_DTO;

namespace GeneticAlgorithm.Examples.MinimumFunction
{
    public class Parameters
    {
        public Parameters(List<SearchableParameter> parameters)
        {
            x = parameters.First(t => t.Name == "x").Value;
            y = parameters.First(t => t.Name == "y").Value;
        }

        public double x { get; set; }
        public double y { get; set; }
}
}
