using GeneticAlgorithm.SimulationRun.Interfaces;
using GeneticAlgorithm.SimulationRun.Parameters_DTO;
namespace GeneticAlgorithm.Examples.MinimumFunction
{
    internal class MinFunc_Simulation : ISimulationBuilder
    {
        private List<SearchableParameter> _parameters;
        public ISimulation GetInstance()
        {
            return new Simulation(_parameters);
        }

        public void Initialize(List<SearchableParameter> simulationParameters)
        {
            _parameters = simulationParameters;
        }
    }

    internal class Simulation : ISimulation
    {
        private Parameters _parameters;
        public Simulation(List<SearchableParameter> parameters)
        {
            _parameters = new Parameters(parameters);
        }

        public double Evaluate()
        {
            return function(_parameters.x, _parameters.y);
        }

        public Dictionary<string, string> GetResult()
        {
            var result = new Dictionary<string, string>();
            result.Add("Score", $"{Evaluate()}");
            return result;
        }

        public void Run()
        {
            return;
        }

        private double function(double x, double y)
        {
            return - Math.Exp(-0.5f * (Math.Pow(x - 0.5f, 2) + Math.Pow(y + 0.5f, 2)));
        }
    }
}
