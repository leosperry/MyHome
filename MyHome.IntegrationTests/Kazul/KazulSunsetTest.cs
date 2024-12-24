using System;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using HaKafkaNet;
using HaKafkaNet.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace MyHome.IntegrationTests.Kazul;

public class KazulSunsetTest : IClassFixture<HaKafkaNetFixture>
{
    private readonly HaKafkaNetFixture fixture;

    public KazulSunsetTest(HaKafkaNetFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task KazulSunSet()
    { 
        var tp = (FakeTimeProvider)fixture.Services.GetRequiredService<TimeProvider>();

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelper.Api_GetEntity_Response<OnOff>(OnOff.On));

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelper.Api_GetEntity_Response<OnOff>(OnOff.Off));

        var sunState = TestHelper.Make<SunState, SunAttributes>("sun.sun", SunState.Above_Horizon, new()
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

        await fixture.Services.SendState(sunState);
        await Task.Delay(10);
        
        tp.Advance(TimeSpan.FromMinutes(20));
        await Task.Delay(100);

        fixture.API.Verify(api => api.TurnOn(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()));
        fixture.API.Verify(api => api.TurnOff(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()));
    }
}

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
        var tp = (FakeTimeProvider)fixture.Services.GetRequiredService<TimeProvider>();

        fixture.API.Invocations.Clear();

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelper.Api_GetEntity_Response<OnOff>(OnOff.On));

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelper.Api_GetEntity_Response<OnOff>(OnOff.Off));

        var sunState = TestHelper.Make<SunState, SunAttributes>("sun.sun", SunState.Above_Horizon, new()
        {
            NextSetting = DateTime.MaxValue,
            Azimuth = default,
            Elevation = default,
            FriendlyName = "Sun",
            NextDawn = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextDusk = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextMidnight = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextNoon = tp.GetLocalNow().AddDays(1).LocalDateTime,
            NextRising = tp.GetLocalNow().AddMinutes(10).LocalDateTime,
            Rising = default
        }, tp.GetLocalNow().LocalDateTime.AddMinutes(1));

        await fixture.Services.SendState(sunState);
        await Task.Delay(10);
        
        tp.Advance(TimeSpan.FromMinutes(20));
        await Task.Delay(100);

        fixture.API.Verify(api => api.TurnOn(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()));
        fixture.API.Verify(api => api.TurnOff(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()));
    }
}
