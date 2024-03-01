using System.Text;

namespace GeneticAlgorithm.Examples.TSP
{
    public struct City
    {
        public string Name { get; set; }
        public double X_position { get; set; }
        public double Y_position { get; set; }

        public double ComputeDistanceTo(City other)
        {
            var x_difference = Math.Pow(X_position - other.X_position, 2);
            var y_difference = Math.Pow(Y_position - other.Y_position, 2);
            return Math.Sqrt(x_difference + y_difference);
        }

        public bool Equals(City other)
        {
            return other.Name == Name;
        }
    }

    public class EdgeValue
    {
        public double Distance { get; set; }
        public double P_Density { get; set; } = 1;
    }

    public class CityGraph
    {
        public CityGraph(List<City> cities)
        {
            Cities = cities;
            Edges = new Dictionary<string, EdgeValue>();
            for (int i = 0; i < Cities.Count - 1; i++)
            {
                var first = cities[i];
                for (int j = i + 1; j < Cities.Count; j++)
                {
                    var other = cities[j];
                    var distance = first.ComputeDistanceTo(other);
                    Edges.Add(EdgeNameGet(first, other), new EdgeValue { Distance = distance, P_Density = 1 });
                    Edges.Add(EdgeNameGet(other, first), new EdgeValue { Distance = distance, P_Density = 1 });
                }
            }
        }
        public List<City> Cities { get; }

        public Dictionary<string, EdgeValue> Edges { get; }

        public double GetDistanceBetween(City city_A, City city_B)
        {
            return Edges[EdgeNameGet(city_A, city_B)].Distance;
        }

        public double GetPheromoneBetween(City city_A, City city_B)
        {
            return Edges[EdgeNameGet(city_A, city_B)].P_Density;
        }

        public void UpdateGraph(Agent agent, double powerCoefficient, int agentNumber)
        {
            var path = agent.GetResult.Path;
            var firstCity = path.First();
            for(int i = 0; i < path.Count - 1; i++)
            {
                var otherCity = path[i + 1];
                var edgeName_1 = EdgeNameGet(firstCity, otherCity);
                var edgeName_2 = EdgeNameGet(otherCity, firstCity);
                Edges[edgeName_1].P_Density += Math.Pow(1d / agent.GetResult.TotalLength, powerCoefficient);
                Edges[edgeName_2].P_Density += Math.Pow(1d / agent.GetResult.TotalLength, powerCoefficient);
                firstCity = path[i + 1];
            }
        }

        public void Evaporate(double rate, int agentNumber, double mapArea)
        {
            foreach (var edge in Edges.Values)
                //edge.P_Density = Math.Max(0, edge.P_Density - rate * agentNumber / mapArea);
                edge.P_Density = (1 - rate) * edge.P_Density;
        }


        public string CitiesStorageFormatGet()
        {
            var citiesString = new StringBuilder("Name;x;y\n");
            foreach (var city in Cities)
                citiesString.AppendLine($"{city.Name};{Math.Round(city.X_position)};{Math.Round(city.Y_position)}");

            return citiesString.ToString();
        }

        public string EdgeHeaderGet()
        {
            var header = new StringBuilder();
            for (int i = 0; i < Cities.Count - 1; i++)
            {
                var first = Cities[i];
                for (int j = i + 1; j < Cities.Count; j++)
                {
                    var other = Cities[j];
                    header.Append($"{EdgeNameGet(first, other)};");
                }
            }

            return header.ToString();
        }

        public string EdgePheromoneDensityGet()
        {
            var edgeValues = new StringBuilder();
            for (int i = 0; i < Cities.Count - 1; i++)
            {
                var first = Cities[i];
                for (int j = i + 1; j < Cities.Count; j++)
                {
                    var other = Cities[j];
                    edgeValues.Append($"{Math.Round(GetPheromoneBetween(first, other), 3)};");
                }
            }

            return edgeValues.ToString();
        }

        private string EdgeNameGet(City city_A, City city_B)
        {
            return $"{city_A.Name}-{city_B.Name}";
        }
    }
}
