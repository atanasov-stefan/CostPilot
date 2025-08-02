using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core;
using CostPilot.ViewModels.CostStatus;

namespace CostPilot.Services.Tests
{
    [TestFixture]
    public class CostStatusServiceTests
    {
        private CostPilotDbContext dbContext;
        private CostStatusService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CostPilotDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            this.dbContext = new CostPilotDbContext(options);
            this.service = new CostStatusService(this.dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Dispose();
        }

        [Test]
        public async Task CreateCostStatusAsync_DuplicateDescription_ReturnsFalse()
        {
            await this.dbContext.CostStatuses.AddAsync(new CostStatus() 
            { 
                Description = "Pending" 
            });

            await this.dbContext.SaveChangesAsync();

            var input = new CostStatusCreateInputModel() 
            { 
                Description = "pending" 
            };

            var result = await this.service.CreateCostStatusAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateCostStatusAsync_UniqueDescription_ReturnsTrue_AndAddsStatus()
        {
            var input = new CostStatusCreateInputModel() 
            { 
                Description = "Approved" 
            };

            var result = await this.service.CreateCostStatusAsync(input);
            var saved = await this.dbContext.CostStatuses.SingleOrDefaultAsync(cs => cs.Description == "Approved");

            Assert.That(result, Is.True);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.Description, Is.EqualTo("Approved"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task DisableCostStatusAsync_InvalidId_ReturnsFalse(string? id)
        {
            var result = await this.service.DisableCostStatusAsync(id);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DisableCostStatusAsync_NonGuidInput_ReturnsFalse()
        {
            var result = await this.service.DisableCostStatusAsync("not-a-guid");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DisableCostStatusAsync_ValidGuid_NotFound_ReturnsFalse()
        {
            var fakeId = Guid.NewGuid().ToString();
            var result = await this.service.DisableCostStatusAsync(fakeId);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DisableCostStatusAsync_ValidGuid_Found_DisablesStatus()
        {
            var status = new CostStatus() 
            { 
                Id = Guid.NewGuid(), 
                Description = "Archived", 
                IsDeleted = false 
            };
            
            await this.dbContext.CostStatuses.AddAsync(status);
            await this.dbContext.SaveChangesAsync();

            var result = await this.service.DisableCostStatusAsync(status.Id.ToString());
            var updated = await this.dbContext.CostStatuses.FindAsync(status.Id);

            Assert.That(result, Is.True);
            Assert.That(updated?.IsDeleted, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EditCostStatusAsync_IdIsNullOrWhitespace_ReturnsFalse(string? id)
        {
            var input = new CostStatusEditInputModel() 
            { 
                Id = id, 
                Description = "Updated" 
            };

            var result = await this.service.EditCostStatusAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostStatusAsync_IdIsInvalidGuid_ReturnsFalse()
        {
            var input = new CostStatusEditInputModel() 
            { 
                Id = "not-a-guid", 
                Description = "Updated" 
            };

            var result = await this.service.EditCostStatusAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostStatusAsync_StatusNotFound_ReturnsFalse()
        {
            var input = new CostStatusEditInputModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Updated"
            };

            var result = await this.service.EditCostStatusAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostStatusAsync_DescriptionIsDuplicate_ReturnsFalse()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            await this.dbContext.CostStatuses.AddRangeAsync(
                new CostStatus() 
                { 
                    Id = id1, 
                    Description = "Pending" 
                },

                new CostStatus() 
                { 
                    Id = id2, 
                    Description = "Approved" 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var input = new CostStatusEditInputModel()
            {
                Id = id2.ToString(),
                Description = "Pending"
            };

            var result = await this.service.EditCostStatusAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostStatusAsync_ValidInput_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostStatuses.AddAsync(new CostStatus() 
            { 
                Id = id, 
                Description = "Initial" 
            });

            await this.dbContext.SaveChangesAsync();

            var input = new CostStatusEditInputModel()
            {
                Id = id.ToString(),
                Description = "Modified"
            };

            var result = await this.service.EditCostStatusAsync(input);
            var updated = await this.dbContext.CostStatuses.FindAsync(id);

            Assert.That(result, Is.True);
            Assert.That(updated!.Description, Is.EqualTo("Modified"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EnableCostStatusAsync_IdIsNullOrWhitespace_ReturnsFalse(string? id)
        {
            var result = await this.service.EnableCostStatusAsync(id);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EnableCostStatusAsync_IdIsInvalidGuid_ReturnsFalse()
        {
            var result = await this.service.EnableCostStatusAsync("not-a-guid");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EnableCostStatusAsync_StatusNotFound_ReturnsFalse()
        {
            var result = await this.service.EnableCostStatusAsync(Guid.NewGuid().ToString());
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EnableCostStatusAsync_StatusExists_EnablesSuccessfully()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostStatuses.AddAsync(new CostStatus() 
            { 
                Id = id, 
                Description = "Disabled", 
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.EnableCostStatusAsync(id.ToString());
            var updated = await this.dbContext.CostStatuses.FindAsync(id);

            Assert.That(result, Is.True);
            Assert.That(updated!.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetCostStatusesAsync_EmptyDb_ReturnsEmptyList()
        {
            var result = await this.service.GetCostStatusesAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetCostStatusesAsync_StatusesExist_ReturnsSortedViewModels()
        {
            await this.dbContext.CostStatuses.AddRangeAsync(
                new CostStatus() 
                { 
                    Id = Guid.NewGuid(), 
                    Description = "Zebra"
                },

                new CostStatus() 
                { 
                    Id = Guid.NewGuid(), 
                    Description = "Apple" 
                }
            );
            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetCostStatusesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Description, Is.EqualTo("Apple"));
            Assert.That(result[1].Description, Is.EqualTo("Zebra"));
        }

        [Test]
        public async Task GetCostStatusesAsync_ObsoleteAndActiveStatuses_MappedCorrectly()
        {
            await this.dbContext.CostStatuses.AddRangeAsync(
                new CostStatus() 
                { 
                    Id = Guid.NewGuid(), 
                    Description = "Active", 
                    IsDeleted = false 
                },

                new CostStatus() 
                { 
                    Id = Guid.NewGuid(), 
                    Description = "Obsolete", 
                    IsDeleted = true 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostStatusesAsync();

            var active = result.First(r => r.Description == "Active");
            var obsolete = result.First(r => r.Description == "Obsolete");

            Assert.That(active.IsObsolete, Is.EqualTo("No"));
            Assert.That(obsolete.IsObsolete, Is.EqualTo("Yes"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task GetCostStatusForEditAsync_IdIsNullOrWhitespace_ReturnsNull(string? id)
        {
            var result = await this.service.GetCostStatusForEditAsync(id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostStatusForEditAsync_IdIsInvalidGuid_ReturnsNull()
        {
            var result = await this.service.GetCostStatusForEditAsync("not-a-guid");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostStatusForEditAsync_StatusNotFound_ReturnsNull()
        {
            var result = await this.service.GetCostStatusForEditAsync(Guid.NewGuid().ToString());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostStatusForEditAsync_ValidStatusExists_ReturnsMappedModel()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostStatuses.AddAsync(new CostStatus()
            {
                Id = id,
                Description = "To Edit",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostStatusForEditAsync(id.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id.ToString()));
            Assert.That(result.Description, Is.EqualTo("To Edit"));
        }
    }
}
