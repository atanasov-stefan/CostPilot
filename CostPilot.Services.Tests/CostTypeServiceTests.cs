using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Data.Models;
using CostPilot.Services.Core;
using CostPilot.ViewModels.CostType;

namespace CostPilot.Services.Tests
{
    [TestFixture]
    public class CostTypeServiceTests
    {
        private CostPilotDbContext dbContext;
        private CostTypeService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CostPilotDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            this.dbContext = new CostPilotDbContext(options);
            this.service = new CostTypeService(this.dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Dispose();
        }

        [Test]
        public async Task CreateCostTypeAsync_CodeExists_ReturnsFalse()
        {
            await this.dbContext.CostTypes.AddAsync(new CostType() 
            { 
                Code = "TRV", 
                Description = "Travel" 
            });

            await this.dbContext.SaveChangesAsync();

            var input = new CostTypeCreateInputModel() 
            { 
                Code = "trv", 
                Description = "New travel type"
            };

            var result = await this.service.CreateCostTypeAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateCostTypeAsync_CodeIsUnique_ReturnsTrue_AndAddsType()
        {
            var input = new CostTypeCreateInputModel()
            { 
                Code = "EDU", 
                Description = "Education"
            };

            var result = await this.service.CreateCostTypeAsync(input);
            var inserted = await this.dbContext.CostTypes.SingleOrDefaultAsync(ct => ct.Code == "EDU");

            Assert.That(result, Is.True);
            Assert.That(inserted, Is.Not.Null);
            Assert.That(inserted!.Description, Is.EqualTo("Education"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task DisableCostTypeAsync_NullOrWhitespaceId_ReturnsFalse(string? input)
        {
            var result = await this.service.DisableCostTypeAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DisableCostTypeAsync_InvalidGuidFormat_ReturnsFalse()
        {
            var result = await this.service.DisableCostTypeAsync("not-a-guid");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DisableCostTypeAsync_IdNotFound_ReturnsFalse()
        {
            var result = await this.service.DisableCostTypeAsync(Guid.NewGuid().ToString());
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DisableCostTypeAsync_ValidId_DisablesSuccessfully()
        {
            var costType = new CostType()
            { 
                Id = Guid.NewGuid(), 
                Code = "TRV", 
                Description = "Description",
                IsDeleted = false 
            };

            await this.dbContext.CostTypes.AddAsync(costType);
            await this.dbContext.SaveChangesAsync();

            var result = await this.service.DisableCostTypeAsync(costType.Id.ToString());
            var updated = await this.dbContext.CostTypes.FindAsync(costType.Id);

            Assert.That(result, Is.True);
            Assert.That(updated!.IsDeleted, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EditCostTypeAsync_InvalidId_ReturnsFalse(string? id)
        {
            var input = new CostTypeEditInputModel() 
            { 
                Id = id, 
                Description = "Anything" 
            };

            var result = await this.service.EditCostTypeAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostTypeAsync_IdNotFound_ReturnsFalse()
        {
            var input = new CostTypeEditInputModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Nonexistent"
            };

            var result = await this.service.EditCostTypeAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostTypeAsync_DuplicateDescription_ReturnsFalse()
        {
            var type1 = new CostType()
            { 
                Id = Guid.NewGuid(), 
                Code = "TRV", 
                Description = "Travel" 
            };

            var type2 = new CostType() 
            { 
                Id = Guid.NewGuid(), 
                Code = "BUS", 
                Description = "Business"
            };

            await this.dbContext.CostTypes.AddRangeAsync(type1, type2);
            await this.dbContext.SaveChangesAsync();

            var input = new CostTypeEditInputModel()
            { 
                Id = type2.Id.ToString(), 
                Description = "Travel" 
            };

            var result = await this.service.EditCostTypeAsync(input);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditCostTypeAsync_ValidEdit_UpdatesDescription()
        {
            var costType = new CostType()
            { 
                Id = Guid.NewGuid(), 
                Code = "TRV", 
                Description = "Travel" 
            };

            await this.dbContext.CostTypes.AddAsync(costType);
            await this.dbContext.SaveChangesAsync();

            var input = new CostTypeEditInputModel()
            { 
                Id = costType.Id.ToString(), 
                Description = "Business Travel" 
            };

            var result = await this.service.EditCostTypeAsync(input);
            var updated = await this.dbContext.CostTypes.FindAsync(costType.Id);

            Assert.That(result, Is.True);
            Assert.That(updated!.Description, Is.EqualTo(input.Description));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EnableCostTypeAsync_IdIsNullOrWhitespace_ReturnsFalse(string? input)
        {
            var result = await this.service.EnableCostTypeAsync(input);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EnableCostTypeAsync_IdIsInvalidGuid_ReturnsFalse()
        {
            var result = await this.service.EnableCostTypeAsync("not-a-guid");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EnableCostTypeAsync_IdNotFound_ReturnsFalse()
        {
            var result = await this.service.EnableCostTypeAsync(Guid.NewGuid().ToString());
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EnableCostTypeAsync_ValidId_SetsIsDeletedFalse_AndReturnsTrue()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostTypes.AddAsync(new CostType()
            {
                Id = id,
                Code = "TRV",
                Description = "Travel",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.EnableCostTypeAsync(id.ToString());
            var updated = await this.dbContext.CostTypes.FindAsync(id);

            Assert.That(result, Is.True);
            Assert.That(updated!.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetActiveCostTypesAsync_NoRecords_ReturnsEmptyList()
        {
            var result = await this.service.GetActiveCostTypesAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetActiveCostTypesAsync_AllDeleted_ReturnsEmptyList()
        {
            await this.dbContext.CostTypes.AddAsync(new CostType()
            {
                Id = Guid.NewGuid(),
                Code = "TRV",
                Description = "Travel",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetActiveCostTypesAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetActiveCostTypesAsync_ActiveAndDeleted_FiltersAndSortsCorrectly()
        {
            await this.dbContext.CostTypes.AddRangeAsync(
                new CostType()
                { 
                    Id = Guid.NewGuid(), 
                    Code = "SUP", 
                    Description = "Supplies", 
                    IsDeleted = false 
                },

                new CostType() 
                { 
                    Id = Guid.NewGuid(),
                    Code = "TRV",
                    Description = "Travel",
                    IsDeleted = true
                },

                new CostType()
                {
                    Id = Guid.NewGuid(),
                    Code = "ADV",
                    Description = "Advertising",
                    IsDeleted = false
                }
            );

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetActiveCostTypesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Description, Is.EqualTo("Advertising"));
            Assert.That(result[1].Description, Is.EqualTo("Supplies"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task GetCostTypeForEditAsync_IdIsNullOrWhitespace_ReturnsNull(string? id)
        {
            var result = await this.service.GetCostTypeForEditAsync(id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostTypeForEditAsync_InvalidGuid_ReturnsNull()
        {
            var result = await this.service.GetCostTypeForEditAsync("not-a-guid");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostTypeForEditAsync_IdNotFound_ReturnsNull()
        {
            var result = await this.service.GetCostTypeForEditAsync(Guid.NewGuid().ToString());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCostTypeForEditAsync_ValidId_ReturnsMappedModel()
        {
            var id = Guid.NewGuid();
            
            await this.dbContext.CostTypes.AddAsync(new CostType()
            {
                Id = id,
                Code = "TRV",
                Description = "Travel",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostTypeForEditAsync(id.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id.ToString()));
            Assert.That(result.Description, Is.EqualTo("Travel"));
        }

        [Test]
        public async Task GetCostTypesAsync_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await this.service.GetCostTypesAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetCostTypesAsync_MixedStatuses_ReturnsAllWithCorrectObsoleteFlags()
        {
            await this.dbContext.CostTypes.AddRangeAsync(
                new CostType() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "TRV", 
                    Description = "Travel", 
                    IsDeleted = false 
                },

                new CostType() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "SUP", 
                    Description = "Supplies", 
                    IsDeleted = true 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostTypesAsync();
            Assert.That(result.Count(), Is.EqualTo(2));

            var travel = result.First(r => r.Code == "TRV");
            var supplies = result.First(r => r.Code == "SUP");
            Assert.That(travel.IsObsolete, Is.EqualTo("No"));
            Assert.That(supplies.IsObsolete, Is.EqualTo("Yes"));
        }

        [Test]
        public async Task GetCostTypesAsync_ReturnsAllSortedByCode()
        {
            await this.dbContext.CostTypes.AddRangeAsync(
                new CostType() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "ZEB", 
                    Description = "Zebra", 
                    IsDeleted = false 
                },

                new CostType()
                { 
                    Id = Guid.NewGuid(), 
                    Code = "ANT", 
                    Description = "Ant", 
                    IsDeleted = false 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetCostTypesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Code, Is.EqualTo("ANT"));
            Assert.That(result[1].Code, Is.EqualTo("ZEB"));
        }
    }
}
