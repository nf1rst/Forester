using System;
using Forester.Application.Exceptions;
using Forester.Application.Helper;

namespace Forester.TESTS.ApplicationLayer.Helper;

public class CsvInputParserTest
{
    private static string Row(int wilderness = 1, int soil = 1, int slope = 0, int hillshade = 0)
    {
        var v = new int[54];
        v[0] = 2800;
        v[2] = slope;
        v[6] = v[7] = v[8] = hillshade;
        v[10 + wilderness - 1] = 1;
        v[14 + soil - 1] = 1;
        return string.Join(",", v);
    }
    [Theory]
    [InlineData(1, 1)]
    [InlineData(4, 40)]
    [InlineData(2, 17)]
    public void Parse_ShouldDecodeOneHotColumns(int wilderness, int soil)
    {
        var result = CsvInputParser.Parse(new[] { Row(wilderness, soil) });

        Assert.Equal(wilderness, result[0].Wilderness_Area);
        Assert.Equal(soil, result[0].Soil_Type);
        Assert.Equal(2800, result[0].Elevation);
    }
    [Fact]
    public void Parse_ShouldIgnoreBlankLines()
    {
        Assert.Single(CsvInputParser.Parse(new[] { Row(1, 1), "" }));
    }

    [Fact]
    public void Parse_ShouldThrowFormatException_WhenRowHasLessThan54Values()
    {
        Assert.Throws<FormatException>(() => CsvInputParser.Parse(new[] { "1,2,3" }));
    }

    [Theory]
    [InlineData(361)]
    [InlineData(-1)]
    public void Parse_ShouldThrowInvalidInputException_WhenSlopeNotBetween0and360(int slope)
    {
        Assert.Throws<InvalidInputException>(() => CsvInputParser.Parse(new[] { Row(slope: slope) }));
        Assert.Throws<InvalidInputException>(() => CsvInputParser.Parse(new[] { Row(slope: slope) }));
    }

    [Fact]
    public void Parse_ShouldAcceptSlopeValues_WhenBetween0And360()
    {
        var result = CsvInputParser.Parse(new[] { Row(slope: 0), Row(slope: 360) });
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Parse_ShouldThrowInvalidInputException_WhenOneHotColumnContainsValueOtherThan0Or1()
    {
        var v = Row().Split(',').Select(n => int.Parse(n)).ToArray();
        v[15] = 5;
        Assert.Throws<InvalidInputException>(() => CsvInputParser.Parse(new[] { string.Join(",", v) }));
    }

    [Fact]
    public void Parse_ShouldThrowInvalidInputException_WhenWildernessAreaHasMoreThanOne1()
    {
        var v = Row().Split(',').Select(int.Parse).ToArray();
        v[11] = 1;
        v[12] = 1;
        Assert.Throws<InvalidInputException>(() => CsvInputParser.Parse(new[] { string.Join(",", v) }));
    }
    [Fact]
    public void Parse_ShouldThrowInvalidInputException_WhenSoilTypeHasMoreThanOne1()
    {
        var v = Row().Split(',').Select(int.Parse).ToArray();
        v[20] = 1;
        v[21] = 1;
        Assert.Throws<InvalidInputException>(() => CsvInputParser.Parse(new[] { string.Join(",", v) }));
    }
}
