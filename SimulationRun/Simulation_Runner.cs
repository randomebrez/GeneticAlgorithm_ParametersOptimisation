using GeneticAlgorithm.SimulationRun.Interfaces;
using GeneticAlgorithm.SimulationRun.Parameters_DTO;
using Newtonsoft.Json;
using System.Text;

namespace GeneticAlgorithm.SimulationRun
{
    public class Simulation_Runner
    {
        private GlobalParameters _parameters;
        private ISimulationBuilder _builder;
        private List<ISimulation> _simulations;

        private double _score;

        public Simulation_Runner(ISimulationBuilder builder, GlobalParameters parameters)
        {
            _parameters = parameters;
            _builder = builder;

            _builder.Initialize(_parameters.SimulationParameters);

            _simulations = new List<ISimulation>();
        }

        public void RunSimulations()
        {
            for (int i = 0; i < _parameters.SimulationNumberByParameters; i++)
            {
                var simu = _builder.GetInstance();
                simu.Run();
                _simulations.Add(simu);
            }
        }

        public void StoreSimulationResults()
        {
            var storagePath = _parameters.StoragePath;
            for (int i = 0; i < _simulations.Count; i++)
            {
                var stringResults = _simulations[i].GetResult();
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

        public double EvaluateParameters()
        {
            var result = 0d;
            var n = _simulations.Count;
            for (int i = 0; i < n; i++)
                result += _simulations[i].Evaluate();
            _score = result / n;
            return _score;
        }

        public string GetStringResult()
        {
            var text = new StringBuilder($"Score {_score}\n");
            foreach(var parameter in _parameters.SimulationParameters.Where(t => t.Search))
                text.Append($"{parameter.Name} : {parameter.Value} - ");

            return text.ToString();
        }
    }
}
