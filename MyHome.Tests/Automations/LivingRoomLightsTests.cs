using System.Text.Json;
using HaKafkaNet;
using HaKafkaNet.Tests;
using Moq;

namespace MyHome.Tests;

public class LivingRoomLightsTests
{
    [Fact]
    public async Task WhenSunNotInRange_andOverrideOff_DoNothing()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Below_Horizon, -10);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity(Helpers.LivingRoomOverride, default))
            .ReturnsAsync(TestHelpers.GetState(Helpers.LivingRoomOverride,"off"));
        
        Mock<ILivingRoomService> livingRoom = new();
            
        LivingRoomLights sut = new LivingRoomLights(provider.Object, livingRoom.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER, "50");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", It.IsAny<CancellationToken>()));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(Helpers.LivingRoomOverride, default));
        livingRoom.Verify(a => a.SetLightsBasedOnPower(It.IsAny<float?>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenInRange_andOverrideOn_DoNothing()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Above_Horizon);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity(Helpers.LivingRoomOverride, default))
            .ReturnsAsync(TestHelpers.GetState(Helpers.LivingRoomOverride,"on"));
        
        Mock<ILivingRoomService> livingRoom = new();

        LivingRoomLights sut = new LivingRoomLights(provider.Object, livingRoom.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER, "50");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", default));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(Helpers.LivingRoomOverride, default));
        livingRoom.Verify(a => a.SetLightsBasedOnPower(It.IsAny<float?>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenInRangeAndOverrideOff_ShouldCallLivingRoom()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Above_Horizon);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(Helpers.LivingRoomOverride, default))
            .ReturnsAsync(TestHelpers.GetState<OnOff, JsonElement>(Helpers.LivingRoomOverride, OnOff.Off));

        provider.Setup(p => p.GetEntity<HaEntityState<int?, JsonElement>>(Sensors.LivingRoomAndKitchenPresenceCount, default))
            .ReturnsAsync(TestHelpers.GetState<int?, JsonElement>(Sensors.LivingRoomAndKitchenPresenceCount, 1));

        Mock<ILivingRoomService> livingRoom = new();            

        LivingRoomLights sut = new LivingRoomLights(provider.Object, livingRoom.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"800");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", default));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(Helpers.LivingRoomOverride, default));
        livingRoom.Verify(a => a.SetLightsBasedOnPower(It.IsAny<float?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

}
