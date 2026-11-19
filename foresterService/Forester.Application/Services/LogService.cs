using System;
using AutoMapper;
using Forester.Application.DTO.Logs;
using Forester.Application.Interfaces;
using Forester.Domain.Entities;
using Forester.Domain.Interfaces;

namespace Forester.Application.Services;

public class LogService : ILogService
{
    private readonly IUnitOfWork uow;
    private readonly IMapper mapper;
    public LogService(IUnitOfWork uow, IMapper mapper)
    {
        this.uow = uow;
        this.mapper = mapper;
    }
    public async Task<ICollection<LogDTO>> GetAllLogsByUserUsername(string userUsername)
    {
        var logs = await uow.Logs.GetAllLogsByUserUsername(userUsername);
        return mapper.Map<ICollection<LogDTO>>(logs);

    }

    public async Task LogUserBatchPredictionRequest(string fileName, long fileSize, string username)
    {
        string text = $"Filename: {fileName},  FileSize: {fileSize}";
        Log log = new Log() { Text = text, LogTypeID = 1, };
        await uow.Logs.InsertLogAsync(log);
    }

    public async Task LogUserPredictionRequest(UserInput userInput, int predictionResult, string userUsername)
    {
        //String text = $"{userInput.Elevation},{userInput.Aspect},{userInput.Slope}, {userInput.Horizontal_Distance_To_Hydrology}, {userInput.Vertical_Distance_To_Hydrology}, {userInput.Horizontal_Distance_To_Roadways}, {userInput.Hillshade_9am}, {userInput.Hillshade_Noon}, {userInput.Hillshade_3pm}, {userInput.Horizontal_Distance_To_Fire_Points}, {userInput.Wilderness_Area},  {userInput.Soil_Type}";
        Log log = new Log()
        {
            UserUsername = userUsername,
            LogTypeID = 1,
            Elevation = userInput.Elevation,
            Aspect = userInput.Aspect,
            Slope = userInput.Slope,
            Vertical_Distance_To_Hydrology = userInput.Vertical_Distance_To_Hydrology,
            Horizontal_Distance_To_Hydrology = userInput.Horizontal_Distance_To_Hydrology,
            Horizontal_Distance_To_Roadways = userInput.Horizontal_Distance_To_Roadways,
            Hillshade_9am = userInput.Hillshade_9am,
            Hillshade_Noon = userInput.Hillshade_Noon,
            Hillshade_3pm = userInput.Hillshade_3pm,
            Horizontal_Distance_To_Fire_Points = userInput.Horizontal_Distance_To_Fire_Points,
            Wilderness_Area = userInput.Wilderness_Area,
            Soil_Type = userInput.Soil_Type,
            Prediction = predictionResult
        };
        await uow.Logs.InsertLogAsync(log);

        await uow.SaveChangesAsync();
    }
}
