using GeneticAlgorithm.SimulationRun.Interfaces;
using GeneticAlgorithm.SimulationRun.Parameters_DTO;
using System.Text;

namespace GeneticAlgorithm.Examples.TSP
{
    public class TSP_Simulation : ISimulationBuilder
    {
        private TSP_Parameters _simulationParameters;
        public ISimulation GetInstance()
        {
            return new Simulation(_simulationParameters);
        }

        public void Initialize(List<SearchableParameter> searchableParameters)
        {
            _simulationParameters = new TSP_Parameters(searchableParameters);
        }
    }

    public struct IterationResult
    {
        public int IterationId { get; set; }
        public double MinLength { get; set; }
        public double MaxLength { get; set; }
        public double PathLengthAverage { get; set; }
    }

    public class SimulationResult
    {
        public int SimulationId { get; set; }
        public int LastIterationId { get; set; }
        public double MinLength { get; set; }
        public double MaxLength { get; set; }
        public List<double> PathLengthAverages { get; set; } = new List<double>();
    }

    public class Simulation : ISimulation
    {
        private List<City> _cities;
        private CityGraph _graph;
        private TSP_Parameters _simulationParameters;
        private List<Agent> _agents;
        private List<IterationResult> _iterationResults = new List<IterationResult>();
        private SimulationResult _simulationResult = new SimulationResult();
        private StringBuilder _graphValues;
        private AgentReport _lastRun;
        private AgentReport _minResult = new AgentReport();
        public List<IterationResult> GetRawResults => _iterationResults;


        public Simulation(TSP_Parameters simulationParameters)
        {
            _simulationParameters = simulationParameters;

            _cities = GenerateCities(simulationParameters.CityNumber, simulationParameters.X_lim, simulationParameters.Y_lim);
            _graph = new CityGraph(_cities);

            _graphValues = new StringBuilder();
            _graphValues.AppendLine(_graph.EdgeHeaderGet());

            _agents = new List<Agent>();
        }

        public async Task RunAsync()
        {
            var iterationNumber = 0;
            var stopCount = 0;
            while(iterationNumber < _simulationParameters.IterationNumber && stopCount < _simulationParameters.StopInARaw)
            {
                var simulationResult = new IterationResult { IterationId = iterationNumber + 1 };
                await RunAgentsAsync();

                // Update graph
                foreach (var agent in _agents)
                {
                    simulationResult = StoreAgentTry(simulationResult, agent.GetResult);
                    _graph.UpdateGraph(agent, _simulationParameters.DropCoefficient, _simulationParameters.AgentNumber);
                }

                _graph.Evaporate(_simulationParameters.EvaporationRate, _simulationParameters.AgentNumber, _simulationParameters.X_lim * _simulationParameters.Y_lim * 4);

                _graphValues.AppendLine(_graph.EdgePheromoneDensityGet());

                // Store iteration result
                simulationResult.PathLengthAverage /= _simulationParameters.AgentNumber;
                _iterationResults.Add(simulationResult);

                _simulationResult.PathLengthAverages.Add(simulationResult.PathLengthAverage);

                if (StopConditionCheck())
                    stopCount++;
                else
                    stopCount = 0;

                iterationNumber++;
            }
            _simulationResult.LastIterationId = iterationNumber - 1;

            var lastAgent = new Agent(_graph, _simulationParameters);
            _lastRun = lastAgent.PheromoneTrail();
        }

        private async Task RunAgentsAsync()
        {
            _agents = new List<Agent>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < _simulationParameters.AgentNumber; i++)
            {
                var agent = new Agent(_graph, _simulationParameters);
                _agents.Add(agent);
                tasks.Add(agent.FindAWayAsync());
            }

            await Task.WhenAll(tasks);
        }

        public async Task<Dictionary<string, string>> GetResultAsync()
        {
            var result = new Dictionary<string, string>();
            var cityResult = _graph.CitiesStorageFormatGet();
            result.Add("Cities.csv", cityResult);

            var pheromoneResults = _graphValues.ToString();
            result.Add("Pheromones.csv", pheromoneResults);

            var lastRunResult = new StringBuilder(_lastRun.StringPath);
            result.Add("LastRun.csv", lastRunResult.ToString());

            var bestRunResult = new StringBuilder(_minResult.StringPath);
            result.Add("BestRun.csv", bestRunResult.ToString());

            var globalResult = new StringBuilder();
            globalResult.AppendLine($"IterationId;MinLength;MaxLength;Average;\n");
            foreach (var iterationResult in _iterationResults)
                globalResult.AppendLine($"{iterationResult.IterationId};{Math.Round(iterationResult.MinLength, 3)};{Math.Round(iterationResult.MaxLength, 3)};{Math.Round(iterationResult.PathLengthAverage, 3)}");

            result.Add("GlobalResults.csv", globalResult.ToString());

            return result;
        }

        public Task<double> EvaluateAsync()
        {
            var last_average_mean = 0d;
            var last_min_average = 0d;
            var last_max_average = 0d;

            var meanNumber = 10;
            for (int i = 0; i < meanNumber; i++)
            {
                var index = _iterationResults.Count - 2 - i;
                last_average_mean += _iterationResults[index].PathLengthAverage;
                last_min_average += _iterationResults[index].MinLength;
                last_max_average += _iterationResults[index].MaxLength;
            }
            last_average_mean /= meanNumber;
            last_min_average /= meanNumber;
            last_max_average /= meanNumber;

            // The smaller the better
            var percentage = (last_average_mean - last_min_average) / (last_max_average - last_min_average);
            return Task.FromResult(1d / (_iterationResults.Count * percentage));
        }

        private bool StopConditionCheck()
        {
            var minIterationNumber = 10;
            if (_iterationResults.Count <= minIterationNumber)
                return false;

            var lastIteration = _iterationResults.Last();
            var last_average_mean = 0d;
            var last_min_average = 0d;

            for (int i = 0; i < minIterationNumber; i++)
            {
                var index = _iterationResults.Count - 2 - i;
                last_average_mean += _iterationResults[index].PathLengthAverage;
                last_min_average += _iterationResults[index].MinLength;
            }

            last_average_mean /= minIterationNumber;
            last_min_average /= minIterationNumber;

            var v_average = Math.Abs(last_average_mean - lastIteration.PathLengthAverage);
            var condition_1 = v_average < _simulationParameters.StopConditionThreshold * lastIteration.PathLengthAverage;

            var v_min = Math.Abs(last_min_average - lastIteration.MinLength);
            var condition_2 = v_min < _simulationParameters.StopConditionThreshold;

            //var condition_4 = lastIteration.MaxLength + lastIteration.MinLength - 2 * lastIteration.PathLengthAverage > 0;

            return condition_1; // && condition_2;
        }

        private IterationResult StoreAgentTry(IterationResult result, AgentReport agentReport)
        {
            // Store iteration results
            result.PathLengthAverage += agentReport.TotalLength;

            if (result.MinLength == 0 || agentReport.TotalLength < result.MinLength)
                result.MinLength = agentReport.TotalLength;

            if (agentReport.TotalLength > result.MaxLength)
                result.MaxLength = agentReport.TotalLength;
            
            // Store simulation global result
            if (agentReport.TotalLength < _simulationResult.MinLength)
                _simulationResult.MinLength = agentReport.TotalLength;
            if (agentReport.TotalLength > _simulationResult.MaxLength)
                _simulationResult.MaxLength = agentReport.TotalLength;

            if (_minResult.TotalLength == 0 || agentReport.TotalLength < _minResult.TotalLength)
                _minResult = agentReport;

            return result;
        }

        private List<City> GenerateCities(int number, double x_lim, double y_lim)
        {
            var cities = new List<City>();
            var rng = new Random();

            for (int i = 0; i < number; i++)
            {
                var x_random = (double)rng.Next(100000) / 100000f;
                var y_random = (double)rng.Next(100000) / 100000f;

                var city = new City
                {
                    Name = $"City_{i}",
                    X_position = 2 * x_lim * x_random - x_lim,
                    Y_position = 2 * y_lim * y_random - y_lim
                };
                cities.Add(city);
            }

            return cities;
        }
    }
}
