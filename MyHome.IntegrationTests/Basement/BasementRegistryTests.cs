using System;
using HaKafkaNet;
using HaKafkaNet.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace MyHome.IntegrationTests.Basement;

[Collection("HomeAutomation")]
public class BasementRegistryTests : IClassFixture<HaKafkaNetFixture>
{
    private readonly HaKafkaNetFixture fixture;
    TimeProvider _time;

    private readonly TestHelper TestHelper;

    public BasementRegistryTests(HaKafkaNetFixture fixture)
    {
        this.fixture = fixture;
        _time = (FakeTimeProvider)fixture.Services.GetRequiredService<TimeProvider>();
        this.TestHelper = fixture.Helpers;
    }
    
    [Fact]
    public async Task OverrideOn()
    {
        //fixture.API.Reset();

        var overrideState = TestHelper.Make<OnOff>(Input_Boolean.BasementOverride, OnOff.On);
        await TestHelper.SendState(overrideState);
        await Task.Delay(200);

        fixture.API.Verify(api => api.ZwaveJs_SetConfigParameter(It.IsAny<object>()
            , It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task OverrideOff()
    {
        //fixture.API.Reset();

        var overrideState = TestHelper.Make<OnOff>(Input_Boolean.BasementOverride, OnOff.Off);
        
        await TestHelper.SendState(overrideState);
        await Task.Delay(200);

        fixture.API.Verify(api => api.ZwaveJs_SetConfigParameter(It.IsAny<object>()
            , It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task LightsOnMotion()
    {
        //fixture.API.Reset();

        fixture.API.Setup(api => api.GetEntity<HaEntityState<OnOff, LightModel>>(Light.BasementLightGroup, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelper.Api_GetEntity_Response(OnOff.On, new LightModel()
            {
                Brightness = Bytes._10pct
            }));
        
        var lightState = TestHelper.Make(Light.BasementLightGroup, OnOff.On, new LightModel()
        {
            Brightness = Bytes._5pct
        });
        var overrideState = TestHelper.Make<OnOff>(Input_Boolean.BasementOverride, OnOff.Off);

        await TestHelper.SendState(lightState);        
        await TestHelper.SendState(overrideState);

        var motionState = TestHelper.Make(Binary_Sensor.BasementMotionMotionDetection, OnOff.On);

        await TestHelper.SendState(motionState);

        fixture.API.Verify<Task<HttpResponseMessage>>(api => api.LightSetBrightnessByLabel(Labels.BasementLights, Bytes._75pct, It.IsAny<CancellationToken>()));
    }
    
}
