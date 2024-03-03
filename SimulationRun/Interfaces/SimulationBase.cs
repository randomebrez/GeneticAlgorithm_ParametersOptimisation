using GeneticAlgorithm.SimulationRun.Parameters_DTO;

namespace GeneticAlgorithm.SimulationRun.Interfaces
{
    public abstract class SimulationBase : ISimulation
    {
        protected List<SearchableParameter> _searchableParameters;

        public void Initialize(List<SearchableParameter> searchableParameters)
        {
            _searchableParameters = searchableParameters;
            InitializeAsync();
        }

        protected abstract Task InitializeAsync();
        public abstract Task RunAsync();
        public abstract Task<Dictionary<string, string>> GetResultAsync();
        public abstract Task<double> EvaluateAsync();

    }
}
