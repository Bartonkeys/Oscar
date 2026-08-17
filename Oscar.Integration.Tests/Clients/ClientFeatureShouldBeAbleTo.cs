using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Clients
{
    [TestClass]
    public class ClientFeatureShouldBeAbleTo: BaseTest
    {
        [TestMethod]
        public async Task GetAllClients()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                var clientAddDto = new ClientAddDto
                {
                    ClientName = $"test{i}",
                    Status = Core.Enums.Status.Active_Consolidated,
                    ClientReference = 54,
                    ClientGrade = Core.Enums.ClientGrade.Anthem,
                    ClientType = Core.Enums.ClientType.Broadcaster,
                    IMaestroClientCode = "MB_TEST",
                    Email = "valid@email.com",
                    GeneralNotes = "MB_TEST"
                };

                var _ = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });
            }

            // Act
            var result = await Mediator.Send(new GetClientsQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 6);
        }

        [TestMethod]
        public async Task GetClientByName()
        {
            // Arrange
            var client = OscarContext.Clients.First();
    
            // Act
            var request = new GetClientsQuery();
            var searchObject = new SearchObject("Clients", "string", "ClientName", client.ClientName);
            request.SearchObjects.Add(searchObject);
            var result = await Mediator.Send(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(client.Id, result.Value.Records.First().Id);
            Assert.IsTrue(result.Value.Records.ToList().Count() == 1);
            Assert.IsTrue(result.Value.TotalRecords == 1);
        }


        [TestMethod]
        public async Task GetClientById()
        {
            // Arrange
            var client = OscarContext.Clients.Last();

            // Act
            var result = await Mediator.Send(new GetClientByIdQuery { Id = client.Id });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(client.Id, result.Value.Id);
        }


        [TestMethod]
        public async Task GetClientById_CurrentAddress()
        {
            // Arrange
            var client = OscarContext.Clients.Last();
            var address = OscarContext.Addresses.FirstOrDefault(a => a.ClientId == client.Id && a.IsCurrent.GetValueOrDefault());

            // Act
            var result = await Mediator.Send(new GetClientByIdQuery { Id = client.Id });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(address?.Id, result.Value?.Address?.Id);
        }


        [TestMethod]
        public async Task AddClient()
        {
            // Arrange
            var clientAddDto = new ClientAddDto
            {
                ClientName = $"Test client 2",
                Status = Core.Enums.Status.Active_Consolidated,
                ClientReference = 123,
                ClientGrade = Core.Enums.ClientGrade.Anthem,
                ClientType = Core.Enums.ClientType.Broadcaster,
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
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
            Assert.IsTrue(result.Value.Address?.Id > 0);
            Assert.AreEqual(Core.Enums.Status.Active_Consolidated, result.Value.Status);
        }


        [TestMethod]
        public async Task AddClient_ExistingReference_ReturnError()
        {
            // Arrange
            var testRefValue = OscarContext.Clients.First().ClientReference;
            var clientAddDto = new ClientAddDto
            {
                ClientName = $"Test client 3",
                Status = Core.Enums.Status.Active_Consolidated,
                ClientReference = testRefValue,
                ClientGrade = Core.Enums.ClientGrade.Anthem,
                ClientType = Core.Enums.ClientType.Broadcaster,
                IMaestroClientCode = "MB_TEST",
                Email = "valid@email.com",
                GeneralNotes = "MB_TEST"
            };

            // Act
            var result = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddClient_InvalidData_ReturnError()
        {
            // Arrange
            var clientAddDto = new ClientAddDto
            {
                ClientName = "Test client 4",
                Status = Core.Enums.Status.Active_Consolidated,
                ClientReference = 6576,
                ClientGrade = Core.Enums.ClientGrade.Anthem,
                ClientType = Core.Enums.ClientType.Broadcaster,
                IMaestroClientCode = "MB_TEST",
                Email = "invalidemail",
                GeneralNotes = "MB_TEST"
            };

            // Act
            var result = await Mediator.Send(new AddClientCommand { ClientAddDto = clientAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddClient_InvalidAdddressData_ReturnError()
        {
            // Arrange
            var clientAddDto = new ClientAddDto
            {
                ClientName = $"Test Client 5",
                Status = Core.Enums.Status.Active_Consolidated,
                ClientReference = 6567,
                ClientGrade = Core.Enums.ClientGrade.Anthem,
                ClientType = Core.Enums.ClientType.Broadcaster,
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
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task UpdateClient()
        {
            // Arrange
            var recordToUpdate = OscarContext.Clients.Last();

            var clientUpdateDto = new ClientUpdateDto
            {
                ClientName =  $"{recordToUpdate.ClientName}UPDATED",
                Status = Core.Enums.Status.Active_Consolidated,
                ClientReference = recordToUpdate.ClientReference,
                ClientGrade = Core.Enums.ClientGrade.Platinum,
                ClientType = Core.Enums.ClientType.Broadcaster,
                IMaestroClientCode =  $"{recordToUpdate.IMaestroClientCode}U",
                Email = "valid@email.com",
                GeneralNotes = "Updated note"
            };

            // Act
            var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = recordToUpdate.Id });
            var updatedRecord = await Mediator.Send(new GetClientByIdQuery { Id = recordToUpdate.Id });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clientUpdateDto.ClientName, updatedRecord.Value.ClientName);
            Assert.AreEqual(null, updatedRecord.Value.ClientReference);
            Assert.AreEqual(clientUpdateDto.IMaestroClientCode, updatedRecord.Value.IMaestroClientCode);
            Assert.AreEqual(clientUpdateDto.GeneralNotes, updatedRecord.Value.GeneralNotes);
        }


        [TestMethod]
        public async Task UpdateClient_ChangeAddress_CreatesNewAddress()
        {
            // Arrange
            var recordToUpdate = OscarContext.Clients.Include(i => i.Addresses).First();
            var currentAddress = recordToUpdate.Addresses?.FirstOrDefault(a => a.IsCurrent.GetValueOrDefault());

            var addressUpdate = new AddressAddDto()
            {
                AddressLine1 = "Updated address line 1",
                AddressLine2 = "Updated address line 2",
                PostZipCode = "UU48 UU8",
                Country = "IRELAND",
                IsCurrent = true
            };


            var clientUpdateDto = new ClientUpdateDto
            {
                ClientName = recordToUpdate.ClientName,
                Status = recordToUpdate.Status,
                ClientReference = recordToUpdate.ClientReference,
                ClientGrade = recordToUpdate.ClientGrade,
                ClientType = recordToUpdate.ClientType,
                IMaestroClientCode = recordToUpdate.IMaestroClientCode,
                Email = recordToUpdate.Email,
                GeneralNotes = recordToUpdate.GeneralNotes,
                Address = addressUpdate
            };

            // Act
            var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = recordToUpdate.Id });
            var updatedRecord = await Mediator.Send(new GetClientByIdQuery { Id = recordToUpdate.Id });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(updatedRecord.Value);
            Assert.IsTrue(updatedRecord.Value.Address?.Id > currentAddress?.Id);
        }


        [TestMethod]
        public async Task UpdateClient_InvalidData_ReturnError()
        {
            // Arrange
            var recordToUpdate = OscarContext.Clients.Include(i => i.Addresses).First();


            var clientUpdateDto = new ClientUpdateDto
            {
                ClientName = recordToUpdate.ClientName,
                Status = recordToUpdate.Status,
                ClientReference = recordToUpdate.ClientReference,
                ClientGrade = recordToUpdate.ClientGrade,
                ClientType = recordToUpdate.ClientType,
                IMaestroClientCode = recordToUpdate.IMaestroClientCode,
                Email = "notarealemail",
            };

            // Act
            var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = recordToUpdate.Id });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }


        [TestMethod]
        public async Task UpdateClient_ID0_ReturnError()
        {
            // Arrange
            var recordToUpdate = OscarContext.Clients.Include(i => i.Addresses).First();


            var clientUpdateDto = new ClientUpdateDto
            {
                ClientName = recordToUpdate.ClientName,
                Status = recordToUpdate.Status,
                ClientReference = recordToUpdate.ClientReference,
                ClientGrade = recordToUpdate.ClientGrade,
                ClientType = recordToUpdate.ClientType,
                IMaestroClientCode = recordToUpdate.IMaestroClientCode,
                Email = recordToUpdate.Email,
                GeneralNotes = recordToUpdate.GeneralNotes,
            };

            // Act
            var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = 0 });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }


        [TestMethod]
        public async Task UpdateClient_InvalidAdddressData_ReturnError()
        {
            // Arrange
            var recordToUpdate = OscarContext.Clients.Last();

            var clientUpdateDto = new ClientUpdateDto
            {
                ClientName = $"{recordToUpdate.ClientName}UPDATED",
                Status = Core.Enums.Status.Active_Consolidated,
                ClientReference = recordToUpdate.ClientReference,
                ClientGrade = Core.Enums.ClientGrade.Platinum,
                ClientType = Core.Enums.ClientType.Broadcaster,
                IMaestroClientCode = $"{recordToUpdate.IMaestroClientCode}U",
                Email = "valid@email.com",
                GeneralNotes = "Updated note",
                Address = new AddressAddDto()
                {
                    AddressLine1 = "Address line one",
                    Country = "Country"
                }
            };

            // Act
            var result = await Mediator.Send(new UpdateClientCommand { ClientUpdateDto = clientUpdateDto, Id = recordToUpdate.Id });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }


        [TestMethod]
        public async Task DeleteClient()
        {
            // Arrange
            var clientId = OscarContext.Clients?.FirstOrDefault()?.Id;

            // Act
            var resultDelete = await Mediator.Send(new DeleteClientCommand { Id = clientId.GetValueOrDefault() });

            // Assert
            Assert.IsTrue(resultDelete.IsSuccess);
            Assert.IsTrue(resultDelete.Value == true);
        }

        [TestMethod]
        public async Task GetAllClientsBasic()
        {
            // Arrange
            var clientCount = OscarContext.Clients.Count();

            // Act
            var request = new GetClientBasicQuery();
            var result = await Mediator.Send(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.ToList().Count() == clientCount);
        }

    }
}
