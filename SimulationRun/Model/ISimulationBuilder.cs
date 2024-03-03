using GeneticAlgorithm.ClientAccessibleObjects;

namespace GeneticAlgorithm.SimulationRun.Model
{
    public interface ISimulationBuilder
    {
        ISimulation GetSimulationInstance(Dictionary<string, SearchableParameter> searchableParameters);
    }
}
