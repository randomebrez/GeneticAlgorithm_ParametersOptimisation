using GeneticAlgorithm.SimulationRun.Parameters_DTO;

namespace GeneticAlgorithm.SimulationRun.Interfaces
{
    public  interface ISimulationBuilder
    {
        void Initialize(List<SearchableParameter> simulationParameters);
        ISimulation GetInstance();
    }
}
