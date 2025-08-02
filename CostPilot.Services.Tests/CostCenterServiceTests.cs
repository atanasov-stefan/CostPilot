using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core;
using CostPilot.ViewModels.CostCenter;

namespace CostPilot.Services.Tests
{
    [TestFixture]
    public class CostCenterServiceTests
    {
        private CostPilotDbContext dbContext;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private CostCenterService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CostPilotDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            this.dbContext = new CostPilotDbContext(options);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            this.userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            
            this.service = new CostCenterService(this.dbContext, this.userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Dispose();
        }

        [Test]
        public async Task CreateCostCenterAsync_CodeIsDuplicated_ReturnsFalse()
        {
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Code = "Duplicate",
                Description = "Description",
                OwnerId = "user1"
            });
            await this.dbContext.SaveChangesAsync();

            var inputModel = new CostCenterCreateInputModel()
            {
                Code = "duplicate",
                Description = "Test Description",
                OwnerId = "owner123"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync("owner123"))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.CreateCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateCostCenterAsync_OwnerNotFound_ReturnsFalse()
        {
            var inputModel = new CostCenterCreateInputModel()
            {
                Code = "NEWCODE",
                Description = "Description",
                OwnerId = "unknownOwner"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync((ApplicationUser)null);

            var result = await this.service.CreateCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateCostCenterAsync_CodeIsUnique_OwnerExists_ReturnsTrue()
        {
            var inputModel = new CostCenterCreateInputModel()
            {
                Code = "UNIQUECODE",
                Description = "Valid Entry",
                OwnerId = "owner123"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.CreateCostCenterAsync(inputModel);

            Assert.IsTrue(result);

            var entity = await this.dbContext.CostCenters
                .FirstOrDefaultAsync(cc => cc.Code == inputModel.Code);

            Assert.IsNotNull(entity);
            Assert.That(entity.OwnerId, Is.EqualTo("owner123"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task DisableCostCenterAsync_IdIsNullOrEmptyOrWhitespace_ReturnsFalse(string? invalidId)
        {
            var result = await this.service.DisableCostCenterAsync(invalidId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableCostCenterAsync_IdIsNotValidGuid_ReturnsFalse()
        {
            var result = await this.service.DisableCostCenterAsync("not-a-guid");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableCostCenterAsync_CostCenterNotFound_ReturnsFalse()
        {
            var validId = Guid.NewGuid().ToString();
            var result = await this.service.DisableCostCenterAsync(validId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableCostCenterAsync_ValidId_CostCenterExists_DisablesSuccessfully()
        {
            var costCenter = new CostCenter()
            {
                Id = Guid.NewGuid(),
                Code = "ABC123",
                Description = "Description",
                OwnerId = "ownerId",
                IsDeleted = false
            };

            await this.dbContext.CostCenters.AddAsync(costCenter);
            await this.dbContext.SaveChangesAsync();

            var result = await this.service.DisableCostCenterAsync(costCenter.Id.ToString());
            Assert.IsTrue(result);

            var updated = await this.dbContext.CostCenters.FindAsync(costCenter.Id);
            Assert.IsNotNull(updated);
            Assert.IsTrue(updated.IsDeleted);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task EditCostCenterAsync_IdIsInvalid_ReturnsFalse(string? invalidId)
        {
            var inputModel = new CostCenterEditInputModel()
            {
                Id = invalidId,
                Description = "Updated Desc",
                OwnerId = "ownerId123"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.EditCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCenterAsync_IdIsNotGuid_ReturnsFalse()
        {
            var inputModel = new CostCenterEditInputModel()
            {
                Id = "not-a-guid",
                Description = "Updated Desc",
                OwnerId = "ownerId123"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.EditCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCenterAsync_OwnerNotFound_ReturnsFalse()
        {
            var inputModel = new CostCenterEditInputModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Update Desc",
                OwnerId = "invalidOwner"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync((ApplicationUser)null);

            var result = await this.service.EditCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCenterAsync_CostCenterNotFound_ReturnsFalse()
        {
            var inputModel = new CostCenterEditInputModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Update Desc",
                OwnerId = "owner123"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.EditCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCenterAsync_DescriptionIsDuplicated_ReturnsFalse()
        {
            var existingId = Guid.NewGuid();
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = existingId,
                Code = "code",
                Description = "Original Desc",
                OwnerId = "owner123"
            });

            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = Guid.NewGuid(),
                Code = "other code",
                Description = "Duplicate Desc",
                OwnerId = "ownerX"
            });

            await this.dbContext.SaveChangesAsync();

            var inputModel = new CostCenterEditInputModel()
            {
                Id = existingId.ToString(),
                Description = "Duplicate Desc",
                OwnerId = "owner123"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.EditCostCenterAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCenterAsync_ValidInput_UpdatesSuccessfully()
        {
            var existingId = Guid.NewGuid();
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = existingId,
                Code = "code",
                Description = "Old Desc",
                OwnerId = "originalOwner"
            });

            await this.dbContext.SaveChangesAsync();

            var inputModel = new CostCenterEditInputModel()
            {
                Id = existingId.ToString(),
                Description = "New Desc",
                OwnerId = "newOwner"
            };

            this.userManagerMock
                .Setup(u => u.FindByIdAsync(inputModel.OwnerId))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.EditCostCenterAsync(inputModel);
            Assert.IsTrue(result);

            var edited = await this.dbContext.CostCenters.FindAsync(existingId);
            Assert.That(edited!.Description, Is.EqualTo(inputModel.Description));
            Assert.That(edited!.OwnerId, Is.EqualTo(inputModel.OwnerId));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EnableCostCenterAsync_IdIsNullOrEmpty_ReturnsFalse(string? invalidId)
        {
            var result = await this.service.EnableCostCenterAsync(invalidId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EnableCostCenterAsync_IdIsNotGuid_ReturnsFalse()
        {
            var result = await this.service.EnableCostCenterAsync("not-a-guid");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EnableCostCenterAsync_CostCenterNotFound_ReturnsFalse()
        {
            var validId = Guid.NewGuid().ToString();
            var result = await this.service.EnableCostCenterAsync(validId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EnableCostCenterAsync_ValidId_EnablesCostCenter()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = id,
                Code = "Code",
                Description = "Description",
                OwnerId = "OwnerId",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();
            
            var result = await this.service.EnableCostCenterAsync(id.ToString());
            Assert.IsTrue(result);

            var enabled = await this.dbContext.CostCenters.FindAsync(id);
            Assert.IsFalse(enabled!.IsDeleted);
        }

        [Test]
        public async Task GetActiveCostCentersAsync_WhenNoneExist_ReturnsEmptyList()
        {
            var result = await this.service.GetActiveCostCentersAsync();
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetActiveCostCentersAsync_FiltersOutDeleted_ReturnsOnlyActive()
        {
            await this.dbContext.CostCenters.AddRangeAsync(new List<CostCenter>()
            {
                new CostCenter { Id = Guid.NewGuid(), Code = "A01", Description = "Finance", OwnerId = "Owner1" ,IsDeleted = false },
                new CostCenter { Id = Guid.NewGuid(), Code = "A02", Description = "HR", OwnerId = "Owner2", IsDeleted = true },
                new CostCenter { Id = Guid.NewGuid(), Code = "B01", Description = "IT", OwnerId = "Owner3", IsDeleted = false }
            });

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetActiveCostCentersAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsTrue(result.All(x => x.CodeDescription.Contains("Finance") || x.CodeDescription.Contains("IT")));
        }

        [Test]
        public async Task GetActiveCostCentersAsync_ProjectionIsCorrect()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = id,
                Code = "X01",
                Description = "Marketing",
                OwnerId = "OwnerId",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetActiveCostCentersAsync()).FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id.ToString()));
            Assert.That(result.CodeDescription, Is.EqualTo("X01 - Marketing"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public async Task GetCostCenterForEditAsync_NullOrEmptyId_ReturnsNull(string? id)
        {
            var result = await this.service.GetCostCenterForEditAsync(id);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCostCenterForEditAsync_InvalidGuid_ReturnsNull()
        {
            var result = await this.service.GetCostCenterForEditAsync("not-a-guid");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCostCenterForEditAsync_IdNotFound_ReturnsNull()
        {
            var result = await this.service.GetCostCenterForEditAsync(Guid.NewGuid().ToString());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCostCenterForEditAsync_IdFound_ReturnsCorrectModel()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = id,
                Code = "Code",
                Description = "R&D",
                OwnerId = "owner-789",
                IsDeleted = false
            });
            
            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostCenterForEditAsync(id.ToString());

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id.ToString()));
            Assert.That(result.Description, Is.EqualTo("R&D"));
            Assert.That(result.OwnerId, Is.EqualTo("owner-789"));
        }

        [Test]
        public async Task GetCostCentersAsync_WhenNoneExist_ReturnsEmptyList()
        {
            var result = await this.service.GetCostCentersAsync();
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCostCentersAsync_IsObsoleteReflectsDeletionStatus()
        {
            var owner = new ApplicationUser() 
            { 
                Id = "2", 
                FirstName = "Alice", 
                LastName = "Smith" 
            };

            await this.dbContext.Users.AddAsync(owner);
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = Guid.NewGuid(),
                Code = "X99",
                Description = "Obsolete Team",
                IsDeleted = true,
                Owner = owner
            });

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetCostCentersAsync()).FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.That(result.IsObsolete, Is.EqualTo("Yes"));
        }

        [Test]
        public async Task GetCostCentersAsync_ProjectionIncludesOwnerName()
        {
            var owner = new ApplicationUser() 
            { 
                Id = "3", 
                FirstName = "Jane", 
                LastName = "Doe"
            };

            await this.dbContext.Users.AddAsync(owner);
            await this.dbContext.CostCenters.AddAsync(new CostCenter()
            {
                Id = Guid.NewGuid(),
                Code = "Z88",
                Description = "Creative",
                IsDeleted = false,
                Owner = owner
            });

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetCostCentersAsync()).FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.That(result.Owner, Is.EqualTo("Jane Doe"));
        }
    }
}