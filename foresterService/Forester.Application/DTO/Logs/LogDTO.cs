using System;
using Forester.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Forester.Application.DTO.Logs;

public class LogDTO
{
    public UserInput UserInput { get; set; }
    public string Text { get; set; } = String.Empty;
    public string UserUsername { get; set; } = String.Empty;
}
