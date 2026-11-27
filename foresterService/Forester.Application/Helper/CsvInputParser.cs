using System.Runtime.CompilerServices;
using Forester.Application.Exceptions;
using Forester.Domain.Entities;

namespace Forester.Application.Helper;

public static class CsvInputParser
{
    public static List<UserInput> Parse(IEnumerable<string> lines)
    {
        var result = new List<UserInput>();

        foreach (var line in lines.Where(l => l.Length > 0))
        {
            int[] values = line.Split(',').Select(v => int.Parse(v)).ToArray();
            if (values.Count() != 54)
                throw new FormatException($"Expected 54 values per row, but found {values.Count()}");

            ValidateInputValues(values);

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

    private static void ValidateInputValues(int[] values)
    {
        if (!HillshadeBetween0And255(values[6]) || !HillshadeBetween0And255(values[7]) ||
            !HillshadeBetween0And255(values[8]))
        {
            throw new InvalidInputException("Hillshade values must be between 0 and 255");
        }
        if (values[2] < 0 || values[2] > 360)
        {
            throw new InvalidInputException("Slope values must be between 0 and 360");
        }
        ValidateOneHot(values, 10, 4, "Wilderness Area");
        ValidateOneHot(values, 14, 40, "Soil Type");
    }

    private static void ValidateOneHot(int[] values, int start, int length, string name)
    {
        var block = values.Skip(start).Take(length).ToArray();
        if (block.Any(v => v != 0 && v != 1)) throw new InvalidInputException($"{name} column should contain only values 0 and 1");

        if (block.Count(v => v == 1) != 1) throw new InvalidInputException($"{name} column should only containt one value set to 1 per row");
    }

    private static bool HillshadeBetween0And255(int hillshade)
    {
        return (hillshade >= 0 && hillshade <= 255);
    }
}
