using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core;
using CostPilot.ViewModels.CostRequest;
using static CostPilot.Common.ApplicationConstants;

namespace CostPilot.Services.Tests
{
    [TestFixture]
    public class CostRequestServiceTests
    {
        private CostPilotDbContext dbContext;
        private CostRequestService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CostPilotDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            this.dbContext = new CostPilotDbContext(options);
            this.service = new CostRequestService(this.dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Dispose();
        }

        [TestCase(null, "user123")]
        [TestCase("", "user123")]
        [TestCase("  ", "user123")]
        [TestCase("guid123", "")]
        [TestCase("guid123", "   ")]
        public async Task CancelCostRequestAsync_InvalidIdOrUserId_ReturnsFalse(string? id, string userId)
        {
            var result = await this.service.CancelCostRequestAsync(id, userId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CancelCostRequestAsync_IdNotGuid_ReturnsFalse()
        {
            var result = await this.service.CancelCostRequestAsync("not-a-guid", "user123");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CancelCostRequestAsync_RequestNotFound_ReturnsFalse()
        {
            var result = await this.service.CancelCostRequestAsync(Guid.NewGuid().ToString(), "user123");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CancelCostRequestAsync_RequestMatches_ReturnsTrueAndMarksDeleted()
        {
            var requestId = Guid.NewGuid();
            var status = new CostStatus()
            {
                Id = Guid.NewGuid(),
                Description = "Pending"
            };

            await this.dbContext.CostStatuses.AddAsync(status);
            await this.dbContext.CostRequests.AddAsync(new CostRequest()
            {
                Id = requestId,
                RequestorId = "user123",
                Status = status,
                IsDeleted = false,
                ApproverId = "ApproverId",
                BriefDescription = "BD",
                DetailedDescription = "DD",
                Number = "Number"
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.CancelCostRequestAsync(requestId.ToString(), "user123");
            Assert.IsTrue(result);

            var updated = await this.dbContext.CostRequests.FindAsync(requestId);
            Assert.IsTrue(updated!.IsDeleted);
        }

        [Test]
        public async Task CreateCostRequestAsync_MissingIds_ReturnsFalse()
        {
            var model = new CostRequestCreateInputModel()
            {
                CurrencyId = "",
                TypeId = "",
                CenterId = "   ",
                BriefDescription = "Test",
                DetailedDescription = "Details"
            };

            var result = await this.service.CreateCostRequestAsync(model, 100, "user123");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateCostRequestAsync_EntityNotFound_ReturnsFalse()
        {
            var model = new CostRequestCreateInputModel()
            {
                CurrencyId = Guid.NewGuid().ToString(),
                TypeId = Guid.NewGuid().ToString(),
                CenterId = Guid.NewGuid().ToString(),
                BriefDescription = "Brief",
                DetailedDescription = "Details"
            };

            var status = new CostStatus()
            {
                Id = Guid.NewGuid(),
                Description = "Pending",
                IsDeleted = false
            };

            await this.dbContext.CostStatuses.AddAsync(status);
            await this.dbContext.SaveChangesAsync();

            var result = await this.service.CreateCostRequestAsync(model, 100, "user123");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateCostRequestAsync_AllValid_ReturnsTrue_AndCreatesRequest()
        {
            var currencyId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var centerId = Guid.NewGuid();
            var statusId = Guid.NewGuid();

            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = currencyId,
                Code = "USD",
                IsDeleted = false
            });

            await this.dbContext.CostTypes.AddAsync(new CostType()
            {
                Id = typeId,
                Code = "TRV",
                Description = "Description",
                IsDeleted = false
            });

            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = centerId,
                Code = "Code",
                Description = "Description",
                OwnerId = "approver001",
                IsDeleted = false
            });

            await this.dbContext.CostStatuses.AddAsync(new CostStatus()
            {
                Id = statusId,
                Description = "Pending",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var model = new CostRequestCreateInputModel()
            {
                CurrencyId = currencyId.ToString(),
                TypeId = typeId.ToString(),
                CenterId = centerId.ToString(),
                BriefDescription = "Travel to Sofia",
                DetailedDescription = "Meetings and lodging"
            };

            var result = await service.CreateCostRequestAsync(model, 2500.50m, "user001");

            Assert.IsTrue(result);

            var request = await this.dbContext.CostRequests.FirstOrDefaultAsync();
            Assert.IsNotNull(request);
            Assert.That(request.RequestorId, Is.EqualTo("user001"));
            Assert.That(request.ApproverId, Is.EqualTo("approver001"));
            Assert.That(request.Amount, Is.EqualTo(2500.50m));
        }

        [Test]
        public async Task EditCostRequestAsync_InvalidIds_ReturnsFalse()
        {
            var model = new CostRequestEditInputModel()
            {
                CurrencyId = "",
                TypeId = "   ",
                CenterId = "",
                Id = "",
                BriefDescription = "Edit brief",
                DetailedDescription = "Edit detail"
            };

            var result = await this.service.EditCostRequestAsync(model, 500, "userX");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostRequestAsync_NonexistentEntities_ReturnsFalse()
        {
            var model = new CostRequestEditInputModel()
            {
                CurrencyId = Guid.NewGuid().ToString(),
                TypeId = Guid.NewGuid().ToString(),
                CenterId = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                BriefDescription = "Update Brief",
                DetailedDescription = "Update Details"
            };

            var result = await this.service.EditCostRequestAsync(model, 999, "userX");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostRequestAsync_ValidUpdate_ChangesAppliedAndReturnsTrue()
        {
            var currencyId = Guid.NewGuid();
            var typeIdOld = Guid.NewGuid();
            var typeIdNew = Guid.NewGuid();
            var centerIdOld = Guid.NewGuid();
            var centerIdNew = Guid.NewGuid();
            var requestId = Guid.NewGuid();

            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = currencyId,
                Code = "USD",
                IsDeleted = false
            });

            await this.dbContext.CostTypes.AddAsync(new CostType()
            {
                Id = typeIdOld,
                Code = "TRV",
                Description = "Description",
                IsDeleted = false
            });

            await this.dbContext.CostTypes.AddAsync(new CostType()
            {
                Id = typeIdNew,
                Code = "EDU",
                Description = "Description2",
                IsDeleted = false
            });

            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = centerIdOld,
                Code = "Code",
                Description = "Description",
                OwnerId = "approver1",
                IsDeleted = false
            });

            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = centerIdNew,
                Code = "Code2",
                Description = "Description2",
                OwnerId = "approver2",
                IsDeleted = false
            });

            await dbContext.CostRequests.AddAsync(new CostRequest()
            {
                Id = requestId,
                CurrencyId = currencyId,
                TypeId = typeIdOld,
                CenterId = centerIdOld,
                RequestorId = "userX",
                IsDeleted = false,
                BriefDescription = "Initial Brief",
                DetailedDescription = "Initial Detail",
                SubmittedOn = DateTime.UtcNow,
                ApproverId = "approver1",
                Amount = 1000,
                Number = "TRV20250100"
            });

            await this.dbContext.SaveChangesAsync();

            var model = new CostRequestEditInputModel()
            {
                CurrencyId = currencyId.ToString(),
                TypeId = typeIdNew.ToString(),
                CenterId = centerIdNew.ToString(),
                Id = requestId.ToString(),
                BriefDescription = "Updated Brief",
                DetailedDescription = "Updated Detail"
            };

            var result = await this.service.EditCostRequestAsync(model, 1500, "userX");
            Assert.IsTrue(result);

            var updated = await this.dbContext.CostRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            Assert.IsNotNull(updated);
            Assert.That(updated.BriefDescription, Is.EqualTo("Updated Brief"));
            Assert.That(updated.DetailedDescription, Is.EqualTo("Updated Detail"));
            Assert.That(updated.CenterId, Is.EqualTo(centerIdNew));
            Assert.That(updated.ApproverId, Is.EqualTo("approver2"));
            Assert.That(updated.TypeId, Is.EqualTo(typeIdNew));
            Assert.That(updated.Amount, Is.EqualTo(1500));
            Assert.IsTrue(updated.Number.StartsWith("EDU"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task GetCostRequestDetailsAsync_IdIsNullOrWhitespace_ReturnsNull(string? id)
        {
            var result = await this.service.GetCostRequestDetailsAsync(id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestDetailsAsync_InvalidGuid_ReturnsNull()
        {
            var result = await this.service.GetCostRequestDetailsAsync("not-a-guid");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestDetailsAsync_RequestNotFound_ReturnsNull()
        {
            var result = await this.service.GetCostRequestDetailsAsync(Guid.NewGuid().ToString());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestDetailsAsync_RequestExists_ReturnsCorrectDetails()
        {
            var id = Guid.NewGuid();
            var submittedOn = new DateTime(2025, 8, 1);
            var decisionOn = new DateTime(2025, 8, 2);

            var approver = new ApplicationUser()
            {
                Id = "approver",
                FirstName = "Anna",
                LastName = "Anders"
            };

            var requestor = new ApplicationUser()
            {
                Id = "requestor",
                FirstName = "Bob",
                LastName = "Builder"
            };

            var center = new CostCenter()
            {
                Id = Guid.NewGuid(),
                Code = "Code",
                Description = "Operations",
                OwnerId = "OwnerId"
            };

            var currency = new CostCurrency()
            {
                Id = Guid.NewGuid(),
                Code = "EUR",
            };

            var status = new CostStatus()
            {
                Id = Guid.NewGuid(),
                Description = "Approved"
            };

            var type = new CostType()
            {
                Id = Guid.NewGuid(),
                Code = "Code",
                Description = "Travel"
            };

            await this.dbContext.Users.AddRangeAsync(approver, requestor);
            await this.dbContext.CostCenters.AddAsync(center);
            await this.dbContext.CostCurrencies.AddAsync(currency);
            await this.dbContext.CostStatuses.AddAsync(status);
            await this.dbContext.CostTypes.AddAsync(type);
            await this.dbContext.CostRequests.AddAsync(new CostRequest()
            {
                Id = id,
                Number = "TRV20250001",
                Amount = 1234.56M,
                SubmittedOn = submittedOn,
                DecisionOn = decisionOn,
                Approver = approver,
                Requestor = requestor,
                Center = center,
                Currency = currency,
                Status = status,
                Type = type,
                Comment = "Processed",
                BriefDescription = "Trip to Paris",
                DetailedDescription = "Business meetings and conferences",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostRequestDetailsAsync(id.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Number, Is.EqualTo("TRV20250001"));
            Assert.That(result.Amount, Is.EqualTo("1234.56"));
            Assert.That(result.SubmittedOn, Is.EqualTo(submittedOn.ToString(DateVisualisationFormat)));
            Assert.That(result.DecisionOn, Is.EqualTo(decisionOn.ToString(DateVisualisationFormat)));
            Assert.That(result.Approver, Is.EqualTo("Anna Anders"));
            Assert.That(result.Requestor, Is.EqualTo("Bob Builder"));
            Assert.That(result.Center, Is.EqualTo("Operations"));
            Assert.That(result.Currency, Is.EqualTo("EUR"));
            Assert.That(result.Status, Is.EqualTo("Approved"));
            Assert.That(result.Type, Is.EqualTo("Travel"));
            Assert.That(result.Comment, Is.EqualTo("Processed"));
            Assert.That(result.BriefDescription, Is.EqualTo("Trip to Paris"));
            Assert.That(result.DetailedDescription, Is.EqualTo("Business meetings and conferences"));
        }

        [TestCase(null, "user123")]
        [TestCase("", "user123")]
        [TestCase("   ", "user123")]
        [TestCase("valid-guid", "")]
        [TestCase("valid-guid", "   ")]
        public async Task GetCostRequestForEditAsync_NullOrEmptyInputs_ReturnsNull(string? id, string userId)
        {
            var result = await this.service.GetCostRequestForEditAsync(id, userId);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestForEditAsync_InvalidGuid_ReturnsNull()
        {
            var result = await this.service.GetCostRequestForEditAsync("not-a-guid", "user123");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestForEditAsync_ValidGuidButNoRequest_ReturnsNull()
        {
            var result = await this.service.GetCostRequestForEditAsync(Guid.NewGuid().ToString(), "user123");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestForEditAsync_RequestExistsButWrongUser_ReturnsNull()
        {
            var requestId = Guid.NewGuid();
            await this.dbContext.CostRequests.AddAsync(new CostRequest()
            {
                Id = requestId,
                Number = "Number",
                RequestorId = "user456",
                ApproverId = "user678",
                BriefDescription = "BD",
                DetailedDescription = "DD",
                Amount = 100,
                IsDeleted = false,
                CenterId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostRequestForEditAsync(requestId.ToString(), "user123");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestForEditAsync_RequestExistsAndUserMatches_ReturnsModel()
        {
            var requestId = Guid.NewGuid();
            var userId = "user123";
            var centerId = Guid.NewGuid();
            var currencyId = Guid.NewGuid();
            var typeId = Guid.NewGuid();

            await this.dbContext.CostRequests.AddAsync(new CostRequest
            {
                Id = requestId,
                Number = "Number",
                RequestorId = userId,
                ApproverId = "ApprovedId",
                Amount = 999.99m,
                BriefDescription = "Marketing campaign",
                DetailedDescription = "Q3 social media push",
                CenterId = centerId,
                CurrencyId = currencyId,
                TypeId = typeId,
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostRequestForEditAsync(requestId.ToString(), userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Amount, Is.EqualTo("999.99"));
            Assert.That(result.BriefDescription, Is.EqualTo("Marketing campaign"));
            Assert.That(result.DetailedDescription, Is.EqualTo("Q3 social media push"));
            Assert.That(result.CenterId, Is.EqualTo(centerId.ToString()));
            Assert.That(result.CurrencyId, Is.EqualTo(currencyId.ToString()));
            Assert.That(result.TypeId, Is.EqualTo(typeId.ToString()));
            Assert.That(result.Id, Is.EqualTo(requestId.ToString()));
        }

        [TestCase("")]
        [TestCase("   ")]
        public async Task GetMyCostRequestsAsync_UserIdIsInvalid_ReturnsNull(string userId)
        {
            var result = await this.service.GetMyCostRequestsAsync(userId, null, null, null);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMyCostRequestsAsync_NoRequestsForUser_ReturnsEmptyList()
        {
            var userId = "userId";
            var result = await service.GetMyCostRequestsAsync(userId, null, null, null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [TestCase("")]
        [TestCase("   ")]
        public async Task GetCostRequestsForApprovalAsync_InvalidUserId_ReturnsNull(string userId)
        {
            var result = await this.service.GetCostRequestsForApprovalAsync(userId);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostRequestsForApprovalAsync_NoRequests_ReturnsEmptyList()
        {
            var approverId = "approverId";
            var result = await this.service.GetCostRequestsForApprovalAsync(approverId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}