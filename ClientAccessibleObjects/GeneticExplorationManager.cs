﻿using GeneticAlgorithm.SimulationRun;
using GeneticAlgorithm.SimulationRun.Model;
using Newtonsoft.Json;

namespace GeneticAlgorithm.ClientAccessibleObjects
{
    public class GeneticExplorationManager<T> where T : ISimulation, new()
    {
        private GlobalParameters _globalParameters;
        private SimuParamManager _parametersManager;
        private SimuBuilder<T> _simulationBuilder;
        private List<SimulationEnvironment> _simulationEnvironments = new List<SimulationEnvironment>();
        private (double score, string config) _bestGenomeEver = (0d, string.Empty);


        public async Task ExploreSimulationAsync(string configFilePath)
        {

            _simulationBuilder = new SimuBuilder<T>();

            // Map config file
            _globalParameters = ReadConfigFile(configFilePath);

            if (Directory.Exists(_globalParameters.StoragePath) == false)
                Directory.CreateDirectory(_globalParameters.StoragePath);

            // Initialize ParameterManager
            _parametersManager = new SimuParamManager(_globalParameters);

            // Build configuration file with exploratory parameters' value set. If no genome is required return a list of a single element with null genome
            var (geneticIteration, currentSimulationEnvironments) = await _parametersManager.SimulationEnvironmentsFromParametersAsync();

            _simulationEnvironments = currentSimulationEnvironments;
            // If no genetic iteration required, execute a single environment then return
            if (geneticIteration == false)
            {
                await ExecuteSimulationEnvironmentAsync(0, _globalParameters.StoragePath);
                return;
            }

            // Otherwise loop until max generation
            for (int generationId = 0; generationId < _globalParameters.GeneticParameters.GenerationNumber; generationId++)
            {
                var start = DateTime.UtcNow;

                // Run all simulation for all different config files (genomes)
                await ExecuteALLSimulationEnvironmentsAsync(generationId);

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
                _simulationEnvironments = _parametersManager.NextGenerationGet(_simulationEnvironments);

                var tick = DateTime.UtcNow;
                var delta = tick - start;
                Console.WriteLine($"Execution time : {delta.Minutes}:{delta.Seconds}");
            }
        }

        private async Task ExecuteALLSimulationEnvironmentsAsync(int generation)
        {
            // Create directory to store results of that generation
            var sub_directory = Path.Combine(_simulationEnvironments[0].Parameters.StoragePath, $"GeneticIteration_{generation}");
            if (Directory.Exists(sub_directory) == false)
                Directory.CreateDirectory(sub_directory);

            // For each different config file built from genomes
            List<Task> simulationResults = new List<Task>();
            for (int j = 0; j < _simulationEnvironments.Count; j++)
                simulationResults.Add(ExecuteSimulationEnvironmentAsync(j, sub_directory));

            await Task.WhenAll(simulationResults);
        }

        private async Task ExecuteSimulationEnvironmentAsync(int index, string subDirectory = "")
        {
            var simulationEnvironment = _simulationEnvironments[index];

            // Class that handle multiple simulations with a single config file
            var simuManager = new SimuRunner(_simulationBuilder, simulationEnvironment.Parameters);

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
