using System;

namespace Forester.Domain.Entities;

public class Log
{
    public int Id { get; set; }
    public string Text { get; set; } = String.Empty;
    public int LogTypeID { get; set; }
    public LogType LogType { get; set; }
    public string UserUsername { get; set; }=String.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    public int Elevation { get; set; }
    public int Aspect { get; set; }
    public int Slope { get; set; }
    public int Horizontal_Distance_To_Hydrology { get; set; }
    public int Vertical_Distance_To_Hydrology { get; set; }
    public int Horizontal_Distance_To_Roadways { get; set; }
    public int Hillshade_9am { get; set; }
    public int Hillshade_Noon { get; set; }
    public int Hillshade_3pm { get; set; }
    public int Horizontal_Distance_To_Fire_Points { get; set; }
    public int Wilderness_Area { get; set; }
    public int Soil_Type { get; set; }
    public int Prediction { get; set; }
}
