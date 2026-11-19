using System;
using Forester.Application.DTO.Logs;
using Forester.Domain.Entities;

namespace Forester.Application.Interfaces;

public interface ILogService
{
    Task LogUserPredictionRequest(UserInput userInput, int predictionResult, string userUsername);
    Task<ICollection<LogDTO>> GetAllLogsByUserUsername(string userUsername);
    Task LogUserBatchPredictionRequest(string fileName, long fileSize, string username);
}
