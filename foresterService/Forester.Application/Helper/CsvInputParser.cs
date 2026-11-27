using Forester.Domain.Entities;

namespace Forester.Application.Helper;

public static class CsvInputParser
{
    public static List<UserInput> Parse(IEnumerable<string> lines)
    {
        var result = new List<UserInput>();

        foreach (var line in lines)
        {
            int[] values = line.Split(',').Select(v => int.Parse(v)).ToArray();
            if (values.Count() != 54)
                throw new FormatException($"Expected 54 values per row, but found {values.Count()}");

            int wilderness = Array.IndexOf(values, 1, 10, 4);
            int soil = Array.IndexOf(values, 1, 14, 40);

            result.Add(new UserInput()
            {
                Elevation = values[0],
                Aspect = values[1],
                Slope = values[2],
                Horizontal_Distance_To_Hydrology = values[3],
                Vertical_Distance_To_Hydrology = values[4],
                Horizontal_Distance_To_Roadways = values[5],
                Hillshade_9am = values[6],
                Hillshade_Noon = values[7],
                Hillshade_3pm = values[8],
                Horizontal_Distance_To_Fire_Points = values[9],
                Wilderness_Area = wilderness != -1 ? wilderness - 10 + 1 : 0,
                Soil_Type = soil != -1 ? soil - 14 + 1 : 0
            });
        }
        return result;
    }
}
