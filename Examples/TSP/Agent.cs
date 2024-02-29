using System.Text;

namespace GeneticAlgorithm.Examples.TSP
{
    public struct AgentReport
    {
        public List<City> Path { get; set; }
        public string StringPath { get; set; }
        public double TotalLength { get; set; }
    }

    public class Agent
    {
        private CityGraph _graph;
        private TSP_Parameters _parameters;
        private AgentReport _results { get; set; }

        public Agent(CityGraph graph, TSP_Parameters parameters)
        {
            _graph = graph;
            _parameters = parameters;
            _results = new AgentReport();
        }

        public AgentReport GetResult => _results;

        public async Task FindAWayAsync()
        {
            await Task.Run(() =>
            {
                var current_path = new List<City>();
                var toVisit = _graph.Cities.ToList();

                var stringPath = new StringBuilder();
                var totalLength = 0d;

                var currentPosition = GetStartingPosition();
                current_path.Add(currentPosition);
                stringPath.Append($"{currentPosition.Name};");
                toVisit.Remove(currentPosition);

                while (toVisit.Count > 0)
                {
                    var probabilities = ComputeProbabilities(currentPosition, toVisit);
                    var nextPosition = Draw(toVisit, probabilities);

                    current_path.Add(nextPosition);
                    stringPath.Append($"{nextPosition.Name};");
                    toVisit.Remove(nextPosition);

                    totalLength += _graph.GetDistanceBetween(currentPosition, nextPosition);

                    currentPosition = nextPosition;
                }

                // Add first one to make a loop
                current_path.Add(current_path.First());
                stringPath.Append($"{current_path.First().Name};");
                totalLength += _graph.GetDistanceBetween(currentPosition, current_path.First());

                _results = new AgentReport { Path = current_path, StringPath = stringPath.ToString(), TotalLength = totalLength };
            });
        }

        public AgentReport PheromoneTrail()
        {
            var current_path = new List<City>();
            var toVisit = _graph.Cities.ToList();

            var stringPath = new StringBuilder();
            var totalLength = 0d;

            var currentPosition = GetStartingPosition();
            current_path.Add(currentPosition);
            stringPath.Append($"{currentPosition.Name};");
            toVisit.Remove(currentPosition);

            while (toVisit.Count > 0)
            {
                var nextPosition = DrawMaxPheromoneDensity(currentPosition, toVisit);

                current_path.Add(nextPosition);
                stringPath.Append($"{nextPosition.Name};");
                toVisit.Remove(nextPosition);

                totalLength += _graph.GetDistanceBetween(currentPosition, nextPosition);

                currentPosition = nextPosition;
            }

            current_path.Add(current_path.First());
            stringPath.Append($"{current_path.First().Name};");
            totalLength += _graph.GetDistanceBetween(currentPosition, current_path.First());

            var result = new AgentReport { Path = current_path, StringPath = stringPath.ToString(), TotalLength = totalLength };
            return result;
        }

        public City GetStartingPosition()
        {
            var randomNumber = new Random().Next(_graph.Cities.Count);
            return _graph.Cities[randomNumber];
        }

        public List<double> ComputeProbabilities(City currentPosition, List<City> toVisit)
        {
            var result = new List<double>();
            var sum = 0d;
            for (int i = 0; i < toVisit.Count; i++)
            {
                var other = toVisit[i];
                var distance = _graph.GetDistanceBetween(currentPosition, other);
                var p_density = _graph.GetPheromoneBetween(currentPosition, other);

                var numerator = Math.Pow(1 / distance, _parameters.AlphaCoefficient) * (1 + Math.Pow(p_density, _parameters.BetaCoefficient));

                sum += numerator;
                result.Add(numerator);
            }

            for (int i = 0; i < result.Count; i++)
                result[i] /= sum;

            return result;
        }

        public City Draw(List<City> toVisit, List<double> probabilities)
        {
            var randomNumber = new Random().Next(100000) / 100000f;
            var found = false;
            var counter = probabilities[0];
            var index = 0;
            while (found == false)
            {
                if (randomNumber < counter)
                    found = true;
                else
                {
                    index++;
                    counter += probabilities[index];
                }
            }
            return toVisit[index];
        }        

        public City DrawMaxPheromoneDensity(City currentPosition, List<City> toVisit)
        {
            var max = _graph.GetPheromoneBetween(currentPosition, toVisit[0]);
            City nextPosition = toVisit[0];
            foreach(var city in toVisit)
            {
                var p_density = _graph.GetPheromoneBetween(currentPosition, city);
                if (p_density > max)
                {
                    nextPosition = city;
                    max = p_density;
                }
            }

            return nextPosition;
        }

        public string ResultPrettyPrint()
        {
            var stringResult = new StringBuilder();

            foreach (var city in _results.StringPath)
                stringResult.Append($"{city} - ");

            stringResult.AppendLine($"Total length : {_results.TotalLength}");

            return stringResult.ToString();
        }
    }
}