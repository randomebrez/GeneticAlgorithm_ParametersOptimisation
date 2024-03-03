using GeneticAlgorithm.SimulationRun.Parameters_DTO;

namespace GeneticAlgorithm.SimulationRun.Interfaces
{
    public interface ISimulationBuilder
    {
        SimulationBase GetSimulationInstance(List<SearchableParameter> searchableParameters);
    }

    public class SimulationBuilder<T> : ISimulationBuilder where T : SimulationBase, new()
    {
        public SimulationBase GetSimulationInstance(List<SearchableParameter> searchableParameters)
        {
            var newSimulation = new T();
            newSimulation.Initialize(searchableParameters);
            return newSimulation;
        }
    }
}
