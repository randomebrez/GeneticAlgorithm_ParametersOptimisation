using GeneticAlgorithm.SimulationRun.Parameters_DTO;

namespace GeneticAlgorithm.Examples.TSP
{
    public class TSP_Parameters
    {
        public TSP_Parameters(List<SearchableParameter> parameters)
        {
            AlphaCoefficient = parameters.First(t => t.Name == "AlphaCoefficient").Value;
            BetaCoefficient = parameters.First(t => t.Name == "BetaCoefficient").Value;
            EvaporationRate = parameters.First(t => t.Name == "EvaporationRate").Value;
            DropCoefficient = parameters.First(t => t.Name == "DropCoefficient").Value;

            CityNumber = (int)parameters.First(t => t.Name == "CityNumber").Value;
            X_lim = parameters.First(t => t.Name == "X_lim").Value;
            Y_lim = parameters.First(t => t.Name == "Y_lim").Value;

            AgentNumber = (int)parameters.First(t => t.Name == "AgentNumber").Value;
            IterationNumber = (int)parameters.First(t => t.Name == "IterationNumber").Value;
            StopConditionThreshold = parameters.First(t => t.Name == "StopConditionThreshold").Value;
            StopInARaw = (int)parameters.First(t => t.Name == "StopInARaw").Value;
        }

        public double AlphaCoefficient { get; }
        public double BetaCoefficient { get; }
        public double EvaporationRate { get; }
        public double DropCoefficient { get; }

        public int CityNumber { get; }
        public double X_lim { get; }
        public double Y_lim { get; }

        public int AgentNumber { get; }
        public int IterationNumber { get; }
        public double StopConditionThreshold { get; }
        public int StopInARaw { get; }
    }
}
