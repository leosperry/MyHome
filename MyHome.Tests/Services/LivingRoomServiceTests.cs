using System.Text.Json;
using Castle.Core.Logging;
using HaKafkaNet;
using HaKafkaNet.Tests;
using Microsoft.Extensions.Logging;
using Moq;

namespace MyHome.Tests;

public class LivingRoomServiceTests
{
    [Fact]
    public async Task WhenSunIsUp_andJustUnderThreshold_TurnOnALittle()
    {
        // Given
        Mock<IHaEntityProvider> provider = new();
            
        Mock<IHaApiProvider> api = new();

        Mock<ILogger<LivingRoomService>> logger = new();

        LivingRoomService sut = new LivingRoomService(api.Object, provider.Object, logger.Object);
        float? fakeState = 699f;
        // When
        await sut.SetLightsBasedOnPower(fakeState, default);
    
        // Then
        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);

        api.Verify(a => a.LightTurnOn(It.Is<LightTurnOnModel>(m => m.Brightness < 10)
            , default), Times.Exactly(2));    
    }

    [Fact]
    public async Task WhenNoLight_TurnOnToMax()
    {
        Mock<IHaEntityProvider> provider = new();

        Mock<IHaApiProvider> api = new();

        Mock<ILogger<LivingRoomService>> logger = new();

        LivingRoomService sut = new LivingRoomService(api.Object, provider.Object, logger.Object);

        float? fakeState = 0;
        // When
        await sut.SetLightsBasedOnPower(fakeState, default);
    
        // Then

        api.Verify(a => a.TurnOff(It.IsAny<IEnumerable<string>>(), default), Times.Never);

        api.Verify(a => a.LightTurnOn(It.Is<LightTurnOnModel>(m => m.Brightness >= 60)
            , default), Times.Exactly(2));    
  
    }
}
