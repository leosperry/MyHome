using System;
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
    public async Task Test()
    { 
        var tp = (FakeTimeProvider)fixture.Services.GetRequiredService<TimeProvider>();

        tp.SetUtcNow(DateTimeOffset.UtcNow);


        var sunState = TestHelper.Make<SunState, SunAttributes>("sun.sun", SunState.Above_Horizon, new()
        {
            NextSetting = tp.GetLocalNow().AddMinutes(10).LocalDateTime,
            Azimuth = default,
            Elevation = default,
            FriendlyName = "Sun",
            NextDawn = default,
            NextDusk = default,
            NextMidnight = default,
            NextNoon = default,
            NextRising = default,
            Rising = default
        }, tp.GetLocalNow().LocalDateTime.AddMinutes(1));

        await fixture.Services.SendState(sunState);
        await Task.Delay(100);
        
        tp.Advance(TimeSpan.FromMinutes(20));
        //await Task.Delay(1000);

        fixture.API.Verify(api => api.TurnOn(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()));

    }
}
