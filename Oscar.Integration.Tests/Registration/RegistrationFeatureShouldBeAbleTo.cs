using System.Collections.Generic;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Registration.Commands;
//using Oscar.Infrastructure.Features.Registration.Queries;
//using Oscar.Infrastructure.Features.Registration.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Oscar.Core.Entities;

namespace Oscar.Integration.Tests.Registration
{
    [TestClass]
    public class RegistrationFeatureShouldBeAbleTo : BaseTest
    {

        [TestMethod]
        public async Task AddRegistrationBatch()
        {
            // Arrange
            var registrationBatchCreateDto = new RegistrationBatchCreateDto()
            {

                registrationDtos = new List<RegistrationCreateDto>()
                {
                    new RegistrationCreateDto()
                    {
                        ClientId = 1,
                        CatalogueId =  1,
                        WorksId =  1,
                        RegisterType =  0
                    },
                    new RegistrationCreateDto()
                    {
                        ClientId = 2,
                        CatalogueId =  1,
                        WorksId = 1,
                        RegisterType =  0
                    }
                }
            };

            var addRegistrationBatchCommand = new AddRegistrationBatchCommand
            {
                RegistrationBatchCreateDto = registrationBatchCreateDto
            };

            // Act
            var result = await Mediator.Send(addRegistrationBatchCommand);
            var registrationBatch = OscarContext.RegistrationBatches.OrderByDescending(x => x.Id).First();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(RegisterStatus.Scheduled, registrationBatch.RegisterStatus);
            Assert.AreEqual(registrationBatchCreateDto.RuntimeParamsJson, registrationBatch.RuntimeParamsJson);
        }
    }
}
