using System.Security.Cryptography;
using HaKafkaNet;
using HaKafkaNet.Tests;
using Moq;

namespace MyHome.Tests;

public class KazulAlertsTests
{
    [Fact]
    public async Task WhenBatteryReports_andBatteryLevelLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP_BATTERY, "49.9");
        await sut.Execute(fakeState, default);
    
        // Then
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default));
    }

    [Fact]
    public async Task WhenBatteryReports_andBatteryLevelOk_ShouldNotdReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP_BATTERY, "50.0");
        await sut.Execute(fakeState, default);
    
        // Then
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenTempReports_andTempLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP, "64.9");
        await sut.Execute(fakeState, default);
    
        // Then
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default));
    }

    [Fact]
    public async Task WhenTempReports_andTempOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenCeramicReports_andSwitchOn_andPowerLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntityState(KazulAlerts.CERAMIC_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetState(KazulAlerts.CERAMIC_SWITCH, "on"));
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.CERAMIC_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntityState(KazulAlerts.CERAMIC_SWITCH, default));
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default));
    }

    [Fact]
    public async Task WhenCeramicReports_andSwitchOff_ShouldNotNotify()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntityState(KazulAlerts.CERAMIC_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetState(KazulAlerts.CERAMIC_SWITCH, "off"));
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.CERAMIC_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntityState(KazulAlerts.CERAMIC_SWITCH, default));
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenCeramicReports_andSwitchOn_andPowerOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntityState(KazulAlerts.CERAMIC_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetState(KazulAlerts.CERAMIC_SWITCH, "on"));
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.CERAMIC_POWER, "75.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntityState(KazulAlerts.CERAMIC_SWITCH, default));
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenHalogenReports_andSwitchOn_andPowerLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntityState(KazulAlerts.HALOGEN_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetState(KazulAlerts.HALOGEN_SWITCH, "on"));
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.HALOGEN_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntityState(KazulAlerts.HALOGEN_SWITCH, default));
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default));
    }

    [Fact]
    public async Task WhenHalogenReports_andSwitchOn_andPowerOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntityState(KazulAlerts.HALOGEN_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetState(KazulAlerts.HALOGEN_SWITCH, "on"));
        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.HALOGEN_POWER, "75.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntityState(KazulAlerts.HALOGEN_SWITCH, default));
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task WhenHalogenReports_andSwitchOff_ShouldNotNotify()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntityState(KazulAlerts.HALOGEN_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetState(KazulAlerts.HALOGEN_SWITCH, "off"));

        Mock<IHaApiProvider> api = new();
    
        KazulAlerts sut = new KazulAlerts(api.Object, entities.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.HALOGEN_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntityState(KazulAlerts.HALOGEN_SWITCH, default));
        api.Verify(a => a.NotifyGroupOrDevice(KazulAlerts.NOTIFY_GROUP, It.IsAny<string>(), default), Times.Never);
    }

    

    


}
