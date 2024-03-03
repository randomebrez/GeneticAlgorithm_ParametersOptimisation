using GeneticAlgorithm.ClientAccessibleObjects;
using GeneticAlgorithm.SimulationRun.Model;

namespace GeneticAlgorithm.SimulationRun
{
    public class SimuBuilder<T> : ISimulationBuilder where T : ISimulation, new()
    {
        public ISimulation GetSimulationInstance(Dictionary<string, SearchableParameter> searchableParameters)
        {
            var newSimulation = new T();
            newSimulation.InitializeAsync(searchableParameters);
            return newSimulation;
        }
    }
}
