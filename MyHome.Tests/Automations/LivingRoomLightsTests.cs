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
        var sun = TestHelpers.GetSunState(SunState.BelowHorizon, -10);
        provider.Setup(p => p.GetEntityState<SunAttributes>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"off"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange();
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntityState<SunAttributes>("sun.sun", default));
        provider.Verify(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.LightTurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);
        api.Verify(a => a.LightTurnOn(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenInRange_andOverrideOn_DoNothing()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSunState(SunState.AboveHorizon);
        provider.Setup(p => p.GetEntityState<SunAttributes>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"on"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange();
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntityState<SunAttributes>("sun.sun", default));
        provider.Verify(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.LightTurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);
        api.Verify(a => a.LightTurnOn(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenInRangeAndOverrideOff_andAboveThreshold_TurnOff()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSunState(SunState.AboveHorizon);
        provider.Setup(p => p.GetEntityState<SunAttributes>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"off"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"800");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntityState<SunAttributes>("sun.sun", default));
        provider.Verify(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.LightTurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Once);
        api.Verify(a => a.LightTurnOn(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenSunIsUp_andJustUnderThreshold_TurnOnALittle()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSunState(SunState.AboveHorizon);
        provider.Setup(p => p.GetEntityState<SunAttributes>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"off"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"699");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntityState<SunAttributes>("sun.sun", default));
        provider.Verify(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.LightTurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);

        api.Verify(a => a.LightSetBrightness(It.IsAny<string>(), It.Is<byte>(b => b < 10), default), Times.Exactly(2));    
    }

    [Fact]
    public async Task WhenSunIsUp_andNoLight_TurnOnToMax()
    {
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSunState(SunState.AboveHorizon);
        provider.Setup(p => p.GetEntityState<SunAttributes>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"off"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"0");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntityState<SunAttributes>("sun.sun", default));
        provider.Verify(p => p.GetEntityState(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.LightTurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);

        api.Verify(a => a.LightSetBrightness(It.IsAny<string>(), It.Is<byte>(b => b >= 60), default), Times.Exactly(2));    
    }
}
