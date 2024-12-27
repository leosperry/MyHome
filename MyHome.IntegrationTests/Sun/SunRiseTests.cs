using System;
using System.Text.Json;
using HaKafkaNet;
using Moq;

namespace MyHome.IntegrationTests.Sun;

[Collection("HomeAutomation")]
public class KazulSunriseTest : IClassFixture<HaKafkaNetFixture>
{
    private readonly HaKafkaNetFixture fixture;

    public KazulSunriseTest(HaKafkaNetFixture fixture)
    {
        this.fixture = fixture;
    }
    
    [Fact]
    public async Task KazulSunrise()
    { 
        fixture.API.Invocations.Clear();

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Helpers.Api_GetEntity_Response<OnOff>(OnOff.On));

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Helpers.Api_GetEntity_Response<OnOff>(OnOff.Off));

        var sunState = fixture.Helpers.Make<SunState, SunAttributes>("sun.sun", SunState.Above_Horizon, new SunAttributes()
        {
            NextSetting = DateTime.MaxValue,
            Azimuth = default,
            Elevation = default,
            FriendlyName = "Sun",
            NextDawn =  fixture.Helpers.Time.GetLocalNow().AddDays(1).LocalDateTime,
            NextDusk = fixture.Helpers.Time.GetLocalNow().AddDays(1).LocalDateTime,
            NextMidnight = fixture.Helpers.Time.GetLocalNow().AddDays(1).LocalDateTime,
            NextNoon = fixture.Helpers.Time.GetLocalNow().AddDays(1).LocalDateTime,
            NextRising = fixture.Helpers.Time.GetLocalNow().AddMinutes(10).LocalDateTime,
            Rising = default
        }, fixture.Helpers.Time.GetLocalNow().LocalDateTime.AddMinutes(1));

        await fixture.Helpers.SendState(sunState);
        
        fixture.Helpers.Time.Advance(TimeSpan.FromMinutes(20));
        await Task.Delay(100);

        fixture.API.Verify(api => api.TurnOn(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()));
        fixture.API.Verify(api => api.TurnOff(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()));
    }
}

