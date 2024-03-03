using GeneticAlgorithm.ClientAccessibleObjects;

namespace GeneticAlgorithm.Examples.TSP
{
    public class TSP_Parameters
    {
        public TSP_Parameters(Dictionary<string, SearchableParameter> parameters)
        {
            AlphaCoefficient = parameters["AlphaCoefficient"].Value;
            BetaCoefficient = parameters["BetaCoefficient"].Value;
            EvaporationRate = parameters["EvaporationRate"].Value;
            DropCoefficient = parameters["DropCoefficient"].Value;

            CityNumber = (int)parameters["CityNumber"].Value;
            X_lim = parameters["X_lim"].Value;
            Y_lim = parameters["Y_lim"].Value;

            AgentNumber = (int)parameters["AgentNumber"].Value;
            IterationNumber = (int)parameters["IterationNumber"].Value;
            StopConditionThreshold = parameters["StopConditionThreshold"].Value;
            StopInARaw = (int)parameters["StopInARaw"].Value;
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
