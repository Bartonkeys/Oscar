using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Clients;

public class ClientFeatureShould : UnitTestBase
{
    [Fact]
    public async Task AddClient()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Test Client",
            Status = Status.Active_Consolidated,
            ClientReference = 123,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST",
            Address = new AddressAddDto
            {
                AddressLine1 = "101 Walterton Road",
                AddressLine2 = "Maida Vale",
                AddressLine3 = "LONDON",
                PostZipCode = "W9 3PG",
                Country = "England",
                IsCurrent = true
            }
        };

        // Act
        var result = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Id > 0);
        Assert.True(result.Value.Address?.Id > 0);
        Assert.Equal(Status.Active_Consolidated, result.Value.Status);
    }

    [Fact]
    public async Task FailToAddClient_WhenClientReferenceAlreadyInUse()
    {
        // Arrange
        var firstClientAddDto = new ClientAddDto
        {
            ClientName = "Test Client 1",
            Status = Status.Active_Consolidated,
            ClientReference = 999,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST"
        };
        var firstResult = await Mediator.Send(new AddClientCommand { ClientAddDto = firstClientAddDto });

        var duplicateClientAddDto = new ClientAddDto
        {
            ClientName = "Test Client 2",
            Status = Status.Active_Consolidated,
            ClientReference = firstResult.Value.ClientReference,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST"
        };

        // Act
        var result = await Mediator.Send(new AddClientCommand { ClientAddDto = duplicateClientAddDto });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public async Task FailToAddClient_WhenEmailInvalid()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Test Client",
            Status = Status.Active_Consolidated,
            ClientReference = 6576,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "invalidemail",
            GeneralNotes = "MB_TEST"
        };

        // Act
        var result = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public async Task FailToAddClient_WhenAddressDataInvalid()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Test Client",
            Status = Status.Active_Consolidated,
            ClientReference = 6567,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST",
            Address = new AddressAddDto
            {
                AddressLine2 = "Maida Vale",
                AddressLine3 = "LONDON",
                Country = "England"
            }
        };

        // Act
        var result = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public async Task GetClientById()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Get By Id Client",
            Status = Status.Active_Consolidated,
            ClientReference = 111,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST"
        };
        var addResult = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        // Act
        var result = await Mediator.Send(new GetClientByIdQuery { Id = addResult.Value.Id });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(addResult.Value.Id, result.Value.Id);
    }

    [Fact]
    public async Task UpdateClient()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Original Client Name",
            Status = Status.Active_Consolidated,
            ClientReference = 222,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST"
        };
        var addResult = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        var clientUpdateDto = new ClientUpdateDto
        {
            ClientName = "Updated Client Name",
            Status = Status.Active_Consolidated,
            ClientReference = addResult.Value.ClientReference,
            ClientGrade = ClientGrade.Platinum,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST_U",
            Email = "valid@email.com",
            GeneralNotes = "Updated note"
        };

        // Act
        var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = addResult.Value.Id });
        var updatedRecord = await Mediator.Send(new GetClientByIdQuery { Id = addResult.Value.Id });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(clientUpdateDto.ClientName.ToUpper(), updatedRecord.Value.ClientName);
        Assert.Equal(clientUpdateDto.IMaestroClientCode, updatedRecord.Value.IMaestroClientCode);
        Assert.Equal(clientUpdateDto.GeneralNotes, updatedRecord.Value.GeneralNotes);
    }

    [Fact]
    public async Task FailToUpdateClient_WhenIdIsZero()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Zero Id Client",
            Status = Status.Active_Consolidated,
            ClientReference = 333,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST"
        };
        await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        var clientUpdateDto = new ClientUpdateDto
        {
            ClientName = clientAddDto.ClientName,
            Status = clientAddDto.Status,
            ClientReference = clientAddDto.ClientReference,
            ClientGrade = clientAddDto.ClientGrade,
            ClientType = clientAddDto.ClientType,
            IMaestroClientCode = clientAddDto.IMaestroClientCode,
            Email = clientAddDto.Email,
            GeneralNotes = clientAddDto.GeneralNotes
        };

        // Act
        var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = 0 });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public async Task DeleteClient()
    {
        // Arrange
        var clientAddDto = new ClientAddDto
        {
            ClientName = "Delete Client",
            Status = Status.Active_Consolidated,
            ClientReference = 444,
            ClientGrade = ClientGrade.Anthem,
            ClientType = ClientType.Broadcaster,
            IMaestroClientCode = "MB_TEST",
            Email = "valid@email.com",
            GeneralNotes = "MB_TEST"
        };
        var addResult = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

        // Act
        var result = await Mediator.Send(new DeleteClientCommand { Id = addResult.Value.Id });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task FailToDeleteClient_WhenClientNotFound()
    {
        // Act
        var result = await Mediator.Send(new DeleteClientCommand { Id = -1 });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetAllClientsBasic()
    {
        // Arrange
        for (var i = 0; i < 3; i++)
        {
            var clientAddDto = new ClientAddDto
            {
                ClientName = $"Basic Client {i}",
                Status = Status.Active_Consolidated,
                ClientReference = 500 + i,
                ClientGrade = ClientGrade.Anthem,
                ClientType = ClientType.Broadcaster,
                IMaestroClientCode = "MB_TEST",
                Email = "valid@email.com",
                GeneralNotes = "MB_TEST"
            };
            await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });
        }

        // Act
        var result = await Mediator.Send(new GetClientBasicQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Count >= 3);
    }
}
