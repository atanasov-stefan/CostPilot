using Moq;
using Microsoft.AspNetCore.Identity;
using MockQueryable;

using CostPilot.Services.Core;
using CostPilot.Data.Models;

namespace CostPilot.Services.Tests
{
    [TestFixture]
    public class RoleServiceTests
    {
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private Mock<RoleManager<IdentityRole>> roleManagerMock;
        private RoleService service;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            this.userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            this.roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            this.service = new RoleService(this.roleManagerMock.Object, this.userManagerMock.Object);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task GetAllRolesExceptUserRolesAsync_NullOrEmptyUserId_ReturnsEmpty(string? userId)
        {
            var result = await this.service.GetAllRolesExceptUserRolesAsync(userId);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllRolesExceptUserRolesAsync_UserNotFound_ReturnsEmpty()
        {
            this.userManagerMock
                .Setup(um => um.FindByIdAsync("user123"))
                .ReturnsAsync((ApplicationUser)null);

            var result = await this.service.GetAllRolesExceptUserRolesAsync("user123");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllRolesExceptUserRolesAsync_UserWithRoles_ReturnsFilteredRoles()
        {
            var user = new ApplicationUser()
            { 
                Id = "user123", 
                UserName = "tester" 
            };

            var allRoles = new List<IdentityRole>()
            {
                new IdentityRole("Admin"),
                new IdentityRole("Editor"),
                new IdentityRole("Viewer")
            }
            .BuildMock();

            this.userManagerMock
                .Setup(um => um.FindByIdAsync("user123"))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string>() { "Editor", "Viewer" });

            this.roleManagerMock
                .Setup(rm => rm.Roles)
                .Returns(allRoles);

            var result = (await this.service.GetAllRolesExceptUserRolesAsync("user123")).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Admin"));
        }

        [Test]
        public async Task GetAllRolesExceptUserRolesAsync_UserWithNoRoles_ReturnsAllRoles()
        {
            var user = new ApplicationUser()
            { 
                Id = "user456", 
                UserName = "tester2" 
            };

            var allRoles = new List<IdentityRole>()
            {
                new IdentityRole("Admin"),
                new IdentityRole("Editor"),
            }
            .BuildMock();

            this.userManagerMock
                .Setup(um => um.FindByIdAsync("user456"))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            this.roleManagerMock
                .Setup(rm => rm.Roles)
                .Returns(allRoles);

            var result = (await this.service.GetAllRolesExceptUserRolesAsync("user456")).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Admin"));
            Assert.That(result[1].Name, Is.EqualTo("Editor"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task GetUserRolesAsync_NullOrEmptyUserId_ReturnsEmpty(string? userId)
        {
            var result = await this.service.GetUserRolesAsync(userId);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetUserRolesAsync_UserNotFound_ReturnsEmpty()
        {
            this.userManagerMock
                .Setup(um => um.FindByIdAsync("user123"))
                .ReturnsAsync((ApplicationUser)null);

            var result = await this.service.GetUserRolesAsync("user123");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetUserRolesAsync_UserHasRoles_ReturnsSortedRoleViewModels()
        {
            var user = new ApplicationUser()
            { 
                Id = "user123", 
                UserName = "testuser" 
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync("user123"))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Viewer", "Admin", "Editor" });

            var result = (await this.service.GetUserRolesAsync("user123")).ToList();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Name, Is.EqualTo("Admin"));
            Assert.That(result[1].Name, Is.EqualTo("Editor"));
            Assert.That(result[2].Name, Is.EqualTo("Viewer"));
        }

        [Test]
        public async Task GetUserRolesAsync_UserHasNoRoles_ReturnsEmptyList()
        {
            var user = new ApplicationUser()
            { 
                Id = "user123" 
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync("user123"))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            var result = await this.service.GetUserRolesAsync("user123");
            Assert.That(result, Is.Empty);
        }
    }
}
