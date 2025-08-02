using Microsoft.EntityFrameworkCore;

using CostPilot.Data;
using CostPilot.Services.Core;
using CostPilot.ViewModels.CostCurrency;
using CostPilot.Data.Models;

namespace CostPilot.Services.Tests
{
    [TestFixture]
    public class CostCurrencyServiceTests
    {
        private CostPilotDbContext dbContext;
        private CostCurrencyService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CostPilotDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            this.dbContext = new CostPilotDbContext(options);
            this.service = new CostCurrencyService(this.dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Dispose();
        }

        [Test]
        public async Task CreateCostCurrencyAsync_CodeIsDuplicated_ReturnsFalse()
        {
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency() 
            { 
                Code = "USD",
            });

            await this.dbContext.SaveChangesAsync();

            var inputModel = new CostCurrencyCreateInputModel() 
            {
                Code = "usd" 
            };
            
            var result = await this.service.CreateCostCurrencyAsync(inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateCostCurrencyAsync_CodeIsUnique_ReturnsTrueAndAddsCurrency()
        {
            var inputModel = new CostCurrencyCreateInputModel() 
            { 
                Code = "EUR" 
            };

            var result = await this.service.CreateCostCurrencyAsync(inputModel);
            Assert.IsTrue(result);

            var currency = await this.dbContext.CostCurrencies.FirstOrDefaultAsync(c => c.Code == "EUR");
            Assert.IsNotNull(currency);
            Assert.That(currency.Code, Is.EqualTo(inputModel.Code));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task DisableCostCurrencyAsync_IdIsNullOrWhitespace_ReturnsFalse(string? inputId)
        {
            var result = await this.service.DisableCostCurrencyAsync(inputId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableCostCurrencyAsync_IdIsNotValidGuid_ReturnsFalse()
        {
            var result = await this.service.DisableCostCurrencyAsync("not-a-guid");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableCostCurrencyAsync_CurrencyNotFound_ReturnsFalse()
        {
            var result = await this.service.DisableCostCurrencyAsync(Guid.NewGuid().ToString());
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableCostCurrencyAsync_ValidId_DisablesCurrency()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = id,
                Code = "JPY",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.DisableCostCurrencyAsync(id.ToString());
            Assert.IsTrue(result);

            var updated = await this.dbContext.CostCurrencies.FindAsync(id);
            Assert.IsTrue(updated!.IsDeleted);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EditCostCurrencyAsync_IdIsNullOrWhitespace_ReturnsFalse(string? id)
        {
            var model = new CostCurrencyEditInputModel() 
            { 
                Id = id, 
                Code = "USD" 
            };

            var result = await this.service.EditCostCurrencyAsync(model);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCurrencyAsync_InvalidGuid_ReturnsFalse()
        {
            var model = new CostCurrencyEditInputModel() 
            { 
                Id = "invalid-guid", 
                Code = "USD" 
            };

            var result = await this.service.EditCostCurrencyAsync(model);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCurrencyAsync_CurrencyNotFound_ReturnsFalse()
        {
            var model = new CostCurrencyEditInputModel
            {
                Id = Guid.NewGuid().ToString(),
                Code = "USD"
            };

            var result = await this.service.EditCostCurrencyAsync(model);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCurrencyAsync_CodeIsDuplicate_ReturnsFalse()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            await this.dbContext.CostCurrencies.AddRangeAsync(
                new CostCurrency() 
                { 
                    Id = id1, 
                    Code = "USD" 
                },

                new CostCurrency() 
                { 
                    Id = id2, 
                    Code = "EUR" 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var model = new CostCurrencyEditInputModel()
            {
                Id = id2.ToString(),
                Code = "USD"
            };

            var result = await service.EditCostCurrencyAsync(model);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditCostCurrencyAsync_ValidInput_UpdatesCode()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = id,
                Code = "USD",
                IsDeleted = false
            });

            await dbContext.SaveChangesAsync();

            var model = new CostCurrencyEditInputModel()
            {
                Id = id.ToString(),
                Code = "JPY"
            };

            var result = await this.service.EditCostCurrencyAsync(model);
            Assert.IsTrue(result);

            var updated = await this.dbContext.CostCurrencies.FindAsync(id);
            Assert.That(updated.Code, Is.EqualTo(model.Code));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task EnableCostCurrencyAsync_IdIsNullOrWhitespace_ReturnsFalse(string? id)
        {
            var result = await this.service.EnableCostCurrencyAsync(id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EnableCostCurrencyAsync_InvalidGuid_ReturnsFalse()
        {
            var result = await this.service.EnableCostCurrencyAsync("not-a-guid");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EnableCostCurrencyAsync_CurrencyNotFound_ReturnsFalse()
        {
            var result = await this.service.EnableCostCurrencyAsync(Guid.NewGuid().ToString());
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EnableCostCurrencyAsync_CurrencyFound_SetsIsDeletedFalse()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = id,
                Code = "GBP",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.EnableCostCurrencyAsync(id.ToString());
            Assert.IsTrue(result);

            var updated = await this.dbContext.CostCurrencies.FindAsync(id);
            Assert.IsFalse(updated!.IsDeleted);
        }

        [Test]
        public async Task GetActiveCostCurrenciesAsync_NoCurrencies_ReturnsEmpty()
        {
            var result = await this.service.GetActiveCostCurrenciesAsync();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetActiveCostCurrenciesAsync_OnlyDeletedCurrencies_ReturnsEmpty()
        {
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = Guid.NewGuid(),
                Code = "USD",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetActiveCostCurrenciesAsync();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetActiveCostCurrenciesAsync_ActiveCurrencies_ReturnsOnlyActiveSorted()
        {
            await this.dbContext.CostCurrencies.AddRangeAsync(
                new CostCurrency() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "GBP", 
                    IsDeleted = false 
                },

                new CostCurrency() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "USD", 
                    IsDeleted = true 
                },

                new CostCurrency() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "AUD", 
                    IsDeleted = false 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetActiveCostCurrenciesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Code, Is.EqualTo("AUD"));
            Assert.That(result[1].Code, Is.EqualTo("GBP"));
        }

        [Test]
        public async Task GetCostCurrenciesAsync_NoEntries_ReturnsEmptyList()
        {
            var result = await this.service.GetCostCurrenciesAsync();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCostCurrenciesAsync_OneActiveCurrency_ReturnsNotObsolete()
        {
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = Guid.NewGuid(),
                Code = "USD",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostCurrenciesAsync();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().IsObsolete, Is.EqualTo("No"));
        }

        [Test]
        public async Task GetCostCurrenciesAsync_OneDeletedCurrency_ReturnsObsolete()
        {
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = Guid.NewGuid(),
                Code = "EUR",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostCurrenciesAsync();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().IsObsolete, Is.EqualTo("Yes"));
        }

        [Test]
        public async Task GetCostCurrenciesAsync_MixedCurrencies_ReturnsSortedAndFlagsCorrect()
        {
            await this.dbContext.CostCurrencies.AddRangeAsync(
                new CostCurrency() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "GBP", 
                    IsDeleted = false 
                },

                new CostCurrency() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "AUD", 
                    IsDeleted = true 
                },

                new CostCurrency() 
                { 
                    Id = Guid.NewGuid(), 
                    Code = "CAD", 
                    IsDeleted = false 
                }
            );

            await this.dbContext.SaveChangesAsync();

            var result = (await this.service.GetCostCurrenciesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Code, Is.EqualTo("AUD"));
            Assert.That(result[1].Code, Is.EqualTo("CAD"));
            Assert.That(result[2].Code, Is.EqualTo("GBP"));
            Assert.That(result[0].IsObsolete, Is.EqualTo("Yes"));
            Assert.That(result[1].IsObsolete, Is.EqualTo("No"));
            Assert.That(result[2].IsObsolete, Is.EqualTo("No"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task GetCostCurrencyForEditAsync_IdIsNullOrWhitespace_ReturnsNull(string? id)
        {
            var result = await this.service.GetCostCurrencyForEditAsync(id);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCostCurrencyForEditAsync_InvalidGuid_ReturnsNull()
        {
            var result = await this.service.GetCostCurrencyForEditAsync("not-a-guid");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCostCurrencyForEditAsync_CurrencyNotFound_ReturnsNull()
        {
            var result = await this.service.GetCostCurrencyForEditAsync(Guid.NewGuid().ToString());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCostCurrencyForEditAsync_CurrencyFound_ReturnsEditModel()
        {
            var id = Guid.NewGuid();
            await this.dbContext.CostCurrencies.AddAsync(new CostCurrency()
            {
                Id = id,
                Code = "CHF"
            });

            await this.dbContext.SaveChangesAsync();

            var result = await this.service.GetCostCurrencyForEditAsync(id.ToString());

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id.ToString()));
            Assert.That(result.Code, Is.EqualTo("CHF"));
        }
    }
}
