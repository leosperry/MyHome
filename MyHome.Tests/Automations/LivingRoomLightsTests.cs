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
        provider.Setup(p => p.GetEntity(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"off"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER, "50");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", It.IsAny<CancellationToken>()));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);
        api.Verify(a => a.TurnOn(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenInRange_andOverrideOn_DoNothing()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Above_Horizon);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState(LivingRoomLights.OVERRIDE,"on"));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER, "50");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", default));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);
        api.Verify(a => a.TurnOn(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenInRangeAndOverrideOff_andAboveThreshold_TurnOff()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Above_Horizon);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState<OnOff, JsonElement>(LivingRoomLights.OVERRIDE, OnOff.Off));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"800");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", default));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Once);
        api.Verify(a => a.TurnOff(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenSunIsUp_andJustUnderThreshold_TurnOnALittle()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Above_Horizon);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState<OnOff, JsonElement>(LivingRoomLights.OVERRIDE, OnOff.Off));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"699");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", default));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);

        api.Verify(a => a.LightTurnOn(It.Is<LightTurnOnModel>(m => m.Brightness < 10)
            , default), Times.Exactly(2));    
    }

    [Fact]
    public async Task WhenSunIsUp_andNoLight_TurnOnToMax()
    {
        Mock<IHaEntityProvider> provider = new();
        var sun = TestHelpers.GetSun(SunState.Above_Horizon);
        provider.Setup(p => p.GetEntity<SunModel>("sun.sun", default))
            .ReturnsAsync(sun);
        provider.Setup(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default))
            .ReturnsAsync(TestHelpers.GetState<OnOff, JsonElement>(LivingRoomLights.OVERRIDE, OnOff.Off));
            
        Mock<IHaApiProvider> api = new();

        LivingRoomLights sut = new LivingRoomLights(api.Object, provider.Object);
        var fakeState = TestHelpers.GetStateChange(LivingRoomLights.TRIGGER,"0");
        // When
        await sut.Execute(fakeState, default);
    
        // Then
        provider.Verify(a => a.GetEntity<SunModel>("sun.sun", default));
        provider.Verify(p => p.GetEntity<HaEntityState<OnOff, JsonElement>>(LivingRoomLights.OVERRIDE, default));
        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);

        api.Verify(a => a.LightTurnOn(It.Is<LightTurnOnModel>(m => m.Brightness >= 60)
            , default), Times.Exactly(2));    
  
    }
}
