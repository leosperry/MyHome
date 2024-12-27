using System;
using System.Text.Json;
using HaKafkaNet;
using HaKafkaNet.Testing;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace MyHome.IntegrationTests;

[Collection("HomeAutomation")]
public class SunsetTests : IClassFixture<HaKafkaNetFixture>
{
    private readonly HaKafkaNetFixture fixture;
    private readonly TestHelper _helper;

    public SunsetTests(HaKafkaNetFixture fixture)
    {
        this.fixture = fixture;
        this._helper = fixture.Helpers;
    }

    [Fact]
    public async Task KazulSunSet()
    { 
        var tp = _helper.Time;

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_helper.Api_GetEntity_Response<OnOff>(OnOff.On));

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_helper.Api_GetEntity_Response<OnOff>(OnOff.Off));

        var sunState = _helper.Make<SunState, SunAttributes>("sun.sun", SunState.Above_Horizon, new()
        {
            NextSetting = tp.GetLocalNow().AddMinutes(10).LocalDateTime,
            Azimuth = default,
            Elevation = default,
            FriendlyName = "Sun",
            NextDawn = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextDusk = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextMidnight = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextNoon = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextRising = tp.GetLocalNow().AddDays(1).LocalDateTime,
            Rising = default
        }, tp.GetLocalNow().LocalDateTime.AddMinutes(1));

        await _helper.SendState(sunState);
        await Task.Delay(10);
        
        tp.Advance(TimeSpan.FromMinutes(20));
        await Task.Delay(100);

        fixture.API.Verify(api => api.TurnOn(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()));
        fixture.API.Verify(api => api.TurnOff(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()));
    }
}

