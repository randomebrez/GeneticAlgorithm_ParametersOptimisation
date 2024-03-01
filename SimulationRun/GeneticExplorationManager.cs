using GeneticAlgorithm.SimulationRun.Interfaces;
using GeneticAlgorithm.SimulationRun.Parameters_DTO;
using Newtonsoft.Json;

namespace GeneticAlgorithm.SimulationRun
{
    public class GeneticExplorationManager
    {
        private GlobalParameters _globalParameters;
        private SimulationParameters_Manager _parametersManager;
        private ISimulationBuilder _simulationBuilder;
        private List<SimulationEnvironment> _simulationEnvironments = new List<SimulationEnvironment>();
        private (double score, string config) _bestGenomeEver = (0d, string.Empty);


        public async Task ExploreSimulationAsync(ISimulationBuilder simulationBuilder, string configFilePath)
        {
            _simulationBuilder = simulationBuilder;

            // Map config file
            _globalParameters = ReadConfigFile(configFilePath);

            // Initialize ParameterManager
            _parametersManager = new SimulationParameters_Manager(_globalParameters);

            // Build configuration file with exploratory parameters' value set. If no genome is required return a list of a single element with null genome
            var (geneticIteration, currentSimulationEnvironments) = _parametersManager.SimulationEnvironmentsFromParameters();

            // If no genetic iteration required, execute a single environment then return
            if (geneticIteration == false)
            {
                await ExecuteSimulationEnvironmentAsync(currentSimulationEnvironments.First(), _globalParameters.StoragePath);
                return;
            }

            // Otherwise loop until max generation
            for (int generationId = 0; generationId < _globalParameters.GeneticParameters.GenerationNumber; generationId++)
            {
                var start = DateTime.UtcNow;

                // Run all simulation for all different config files (genomes)
                await ExecuteALLSimulationEnvironmentsAsync(currentSimulationEnvironments, generationId);

                var average = 0d;
                foreach (var simulationEnvironment in _simulationEnvironments)
                {
                    if (string.IsNullOrEmpty(_bestGenomeEver.config) || _bestGenomeEver.score < simulationEnvironment.Score)
                        _bestGenomeEver = (simulationEnvironment.Score, $"Generation: {generationId} - Config: {simulationEnvironment.Id}");

                    average += simulationEnvironment.Score;
                }

                average /= _simulationEnvironments.Count;
                Console.WriteLine($"Iteration : {generationId} - Score {Math.Round(average, 4)}");
                Console.WriteLine($"BestGenome ==> {_bestGenomeEver.config} - Score: {_bestGenomeEver.score}");

                // Get next generation
                currentSimulationEnvironments = _parametersManager.NextGenerationGet(_simulationEnvironments);

                _simulationEnvironments.Clear();
                var tick = DateTime.UtcNow;
                var delta = tick - start;
                Console.WriteLine($"Execution time : {delta.Minutes}:{delta.Seconds}");
            }
        }

        private async Task ExecuteALLSimulationEnvironmentsAsync(List<SimulationEnvironment> simulationEnvironments, int generation)
        {
            // Create directory to store results of that generation
            var sub_directory = Path.Combine(simulationEnvironments[0].Parameters.StoragePath, $"GeneticIteration_{generation}");
            if (Directory.Exists(sub_directory) == false)
                Directory.CreateDirectory(sub_directory);

            // For each different config file built from genomes
            List<Task> simulationResults = new List<Task>();
            for (int j = 0; j < simulationEnvironments.Count; j++)
                simulationResults.Add(ExecuteSimulationEnvironmentAsync(simulationEnvironments[j], sub_directory));

            await Task.WhenAll(simulationResults);
        }

        private async Task ExecuteSimulationEnvironmentAsync(SimulationEnvironment simulationEnvironment, string subDirectory = "")
        {
            // Class that handle multiple simulations with a single config file
            var simuManager = new Simulation_Runner(_simulationBuilder, simulationEnvironment.Parameters);

            // Run the simulations
            await simuManager.RunSimulationsAsync();

            // Store results if subDirectory parameter is filled
            if (string.IsNullOrEmpty(subDirectory) == false)
            {
                // Add subfolder to store results of simulation with that set of parameters
                simulationEnvironment.Parameters.StoragePath = Path.Combine(subDirectory, $"ParametersConfig_{simulationEnvironment.Id}");
                // Store results
                await simuManager.StoreSimulationResultsAsync();
            }

            // Evaluate parameters
            simulationEnvironment.Score = await simuManager.EvaluateParametersAsync();
        }


        private GlobalParameters ReadConfigFile(string configFilePath)
        {
            var jsonFile = File.ReadAllText(configFilePath);

            return JsonConvert.DeserializeObject<GlobalParameters>(jsonFile);
        }
    }
}
