using System.Reflection.Metadata;
using System.Text.Json;
using Castle.Components.DictionaryAdapter;
using HaKafkaNet;
using Moq;

namespace MyHome.Tests;


public class GarageServiceTests
{
    [Fact]
    public async Task WhenEnsureGarageClosedCalled_ShouldNotCloseIfOpen()
    {
        //arrange
        Mock<IHaStateCache> cache = new();
        cache.Setup(c => c.Get(GarageService.GARAGE1_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_CONTACT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE1_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_TILT, "off"));

        Mock<IHaApiProvider> api = new();

        GarageService sut = new GarageService(cache.Object, api.Object);
        
        //act
        await sut.EnsureGarageClosed(default);

        //assert
        api.Verify(a => a.CallService(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenDoor1Open_ShouldCloseAndVerify()
    {
        //arrange
        Mock<IHaStateCache> cache = new();
        cache.SetupSequence(c => c.Get(GarageService.GARAGE1_CONTACT, default))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "on"))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_CONTACT, "off"));
        cache.SetupSequence(c => c.Get(GarageService.GARAGE1_TILT, default))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "on"))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_TILT, "off"));

        Mock<IHaApiProvider> api = new();

        GarageService sut = new GarageService(cache.Object, api.Object);
        
        //act
        await sut.EnsureGarageClosed(default);

        //assert
        api.Verify(a => a.SwitchTurnOn(GarageService.GARAGE1_DOOR_OPENER, default), Times.Once);
        api.Verify(a => a.NotifyGroupOrDevice(It.IsAny<string>(), It.IsAny<string>(), default), Times.Once);
        cache.Verify(c => c.Get(GarageService.GARAGE1_CONTACT, default), Times.Exactly(2));
    }

    [Fact]
    public async Task WhenShouldCloseAndCantVerify_ShouldNotify()
    {
        //arrange
        Mock<IHaStateCache> cache = new();
        cache.Setup(c => c.Get(GarageService.GARAGE1_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "on"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_CONTACT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE1_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "on"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_TILT, "off"));

        Mock<IHaApiProvider> api = new();

        GarageService sut = new GarageService(cache.Object, api.Object);
        
        //act
        await sut.EnsureGarageClosed(default);

        //assert
        api.Verify(a => a.SwitchTurnOn(GarageService.GARAGE1_DOOR_OPENER, default), Times.Once);
        api.Verify(a => a.NotifyGroupOrDevice(It.IsAny<string>(), It.IsAny<string>(), default), Times.Exactly(2));
        cache.Verify(c => c.Get(GarageService.GARAGE1_CONTACT, default), Times.Exactly(2));
    }

    [Fact]
    public async Task WhenBothDoorsOpen_ShouldCloseBoth()
    {
        //one door will close, the other won't
        //arrange
        Mock<IHaStateCache> cache = new();
        cache.SetupSequence(c => c.Get(GarageService.GARAGE1_CONTACT, default))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "on"))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_CONTACT, "on"));
        cache.SetupSequence(c => c.Get(GarageService.GARAGE1_TILT, default))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "on"))
            .ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_TILT, "on"));

        Mock<IHaApiProvider> api = new();

        GarageService sut = new GarageService(cache.Object, api.Object);
        
        //act
        await sut.EnsureGarageClosed(default);

        //assert
        api.Verify(a => a.SwitchTurnOn(It.IsAny<string>(), default), Times.Exactly(4)); //twice for the garage doors, twice for the light
        api.Verify(a => a.NotifyGroupOrDevice(It.IsAny<string>(), It.IsAny<string>(), default), Times.Exactly(3));
        cache.Verify(c => c.Get(It.IsAny<string>(), default), Times.Exactly(8));    
    }

    [Fact]
    public async Task WhenDoorStateUnknown_ShouldNotifyGroupAndNotCloseDoorAndTurnOnLight()
    {
        //arrange
        Mock<IHaStateCache> cache = new();
        cache.Setup(c => c.Get(GarageService.GARAGE1_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE1_CONTACT, "on"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_CONTACT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_CONTACT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE1_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE1_TILT, "off"));
        cache.Setup(c => c.Get(GarageService.GARAGE2_TILT, default)).ReturnsAsync(createTestState(GarageService.GARAGE2_TILT, "off"));

        Mock<IHaApiProvider> api = new();

        GarageService sut = new GarageService(cache.Object, api.Object);
        
        //act
        await sut.EnsureGarageClosed(default);

        //assert
        api.Verify(a => a.NotifyGroupOrDevice("my_notify_group", It.IsAny<string>(), default), Times.Once);
        api.Verify(a => a.SwitchTurnOn(GarageService.GARAGE1_DOOR_OPENER, default), Times.Never);
        api.Verify(a => a.SwitchTurnOn(GarageService.BACK_HALL_LIGHT, default), Times.Once);
    }

    private HaEntityState createTestState(string entityId, string state)
    {
        return new HaEntityState()
        {
            EntityId = entityId,
            State = state,
            Attributes = JsonSerializer.SerializeToElement(new {})
        };
    }
}
