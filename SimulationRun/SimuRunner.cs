using GeneticAlgorithm.ClientAccessibleObjects;
using GeneticAlgorithm.SimulationRun.Model;
using Newtonsoft.Json;
using System.Text;

namespace GeneticAlgorithm.SimulationRun
{
    public class SimuRunner
    {
        private ISimulationBuilder _simulationBuilder;
        private GlobalParameters _parameters;
        private List<ISimulation> _simulations;

        private double _score;

        public SimuRunner(ISimulationBuilder builder, GlobalParameters parameters) 
        {
            _simulationBuilder = builder;
            _parameters = parameters;

            _simulations = new List<ISimulation>();
        }

        public async Task RunSimulationsAsync()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < _parameters.SimulationNumberByParameters; i++)
            {
                var simu = _simulationBuilder.GetSimulationInstance(_parameters.SimulationParametersDict());
                tasks.Add(simu.RunAsync());
                _simulations.Add(simu);
            }

            await Task.WhenAll(tasks);
        }

        public async Task StoreSimulationResultsAsync()
        {
            var storagePath = _parameters.StoragePath;
            for (int i = 0; i < _simulations.Count; i++)
            {
                var stringResults = await _simulations[i].GetResultAsync();
                var directory = Path.Combine(storagePath, $"Simulation_{i}");
                if (Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);

                foreach (var result in stringResults)
                {
                    using (var stream = File.Open(Path.Combine(storagePath, $"Simulation_{i}", result.Key), FileMode.Create))
                    {
                        var bytes = new UTF8Encoding().GetBytes(result.Value);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                    
                }
            }

            // Save parameters
            var parameters = JsonConvert.SerializeObject(_parameters);
            using (var stream = File.Open(Path.Combine(storagePath, "Parameters.json"), FileMode.Create))
            {
                var bytes = new UTF8Encoding().GetBytes(parameters);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        public async Task<double> EvaluateParametersAsync()
        {
            var result = 0d;
            var n = _simulations.Count;
            for (int i = 0; i < n; i++)
                result += await _simulations[i].EvaluateAsync();
            _score = result / n;
            return _score;
        }

        public string GetStringResult()
        {
            var text = new StringBuilder($"Score {_score}\n");
            foreach(var parameter in _parameters.SimulationParametersList.Where(t => t.Search))
                text.Append($"{parameter.Name} : {parameter.Value} - ");

            return text.ToString();
        }
    }
}
