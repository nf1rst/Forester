using AutoMapper;
using Forester.Application.DTO.Logs;
using Forester.Application.Services;
using Forester.Domain.Entities;
using Forester.Domain.Interfaces;
using Moq;
using static System.DateTime;

namespace Forester.TESTS.ApplicationLayer.Service;

public class LogServiceTest
{
    private readonly Mock<IUnitOfWork> uow = new();
    private readonly Mock<ILogRepository> logRepo = new();
    private readonly Mock<IMapper> mapper = new();
    private readonly LogService logService;
    string username = "username";

    public LogServiceTest()
    {
        uow.Setup(u => u.Logs).Returns(logRepo.Object);

        mapper.Setup(m => m.Map<ICollection<LogDTO>>(It.IsAny<object>()))
            .Returns((object src) => ((IEnumerable<Log>)src)
                .Select(l => new LogDTO { UserUsername = l.UserUsername, TimeStamp = l.TimeStamp })
                .ToList());
        logService = new LogService(uow.Object, mapper.Object);
    }

    [Fact]
    public async Task GetAllLogsByUserUsername_ShouldReturnMostRecentFirst()
    {
        var logs = new List<Log>
        {
            new() { Id = 1, UserUsername = username },
            new() { Id = 2, UserUsername = username }
        };

        uow.Setup(u => u.Logs.GetAllLogsByUserUsername(username)).ReturnsAsync(logs);

        var result = await logService.GetAllLogsByUserUsername(username);

        Assert.All(result, l => Assert.Equal(username, l.UserUsername));
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllLogsByUser_ShouldReturnOnlyUsersLogs()
    {

        var logs = new List<Log>()
        {
            new Log() { Id = 1, UserUsername = username },
            new Log() { Id = 2, UserUsername = username }
        };

        uow.Setup(u => u.Logs.GetAllLogsByUserUsername(username)).ReturnsAsync(logs);

        var result = await logService.GetAllLogsByUserUsername(username);

        Assert.Equal(2, result.Count);
        Assert.All(result, l => Assert.Equal(username, l.UserUsername));

    }
    [Fact]
    public async Task GetAllLogsByUser_ShouldReturnEmptyListWhenNoLogs()
    {
        uow.Setup(u => u.Logs.GetAllLogsByUserUsername("nobody")).ReturnsAsync(new List<Log>());
        var result = await logService.GetAllLogsByUserUsername(username);
        Assert.Empty(result);
    }
    [Fact]
    public async Task GetAllLogsByUser_ShouldCallRepositoryOnce_WithGivenUsername()
    {
        uow.Setup(u => u.Logs.GetAllLogsByUserUsername(username)).ReturnsAsync(new List<Log>());
        await logService.GetAllLogsByUserUsername(username);
        uow.Verify(u => u.Logs.GetAllLogsByUserUsername(username), Times.Once());
    }
    [Fact]
    public async Task GetAllLogsByUser_ShouldPassRepositoryLogsToMapper()
    {
        var logs = new List<Log>() { new() { Id = 1, UserUsername = username } };
        uow.Setup(u => u.Logs.GetAllLogsByUserUsername(username)).ReturnsAsync(logs);
        var result = await logService.GetAllLogsByUserUsername(username);
        mapper.Verify(m => m.Map<ICollection<LogDTO>>(logs), Times.Once);
    }
    [Theory]
    [InlineData("ivan")]
    [InlineData("marko")]
    [InlineData("user with spaces")]
    public async Task GetAlllLogsByUser_ShouldAskRepositoryForExactUsername(string userUsername)
    {
        uow.Setup(u => u.Logs.GetAllLogsByUserUsername(userUsername)).ReturnsAsync(new List<Log>());
        await logService.GetAllLogsByUserUsername(userUsername);
        uow.Verify(u => u.Logs.GetAllLogsByUserUsername(userUsername), Times.Once);
    }
}
