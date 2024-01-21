namespace MyHome.Tests;

public class DynamicLightAdjusterTests
{
    IDynamicLightAdjuster.DynamicLightModel _model = new IDynamicLightAdjuster.DynamicLightModel()
        {
            TargetIllumination = 100,
            IlluminationAddedAtMin =3,
            IlluminationAddedAtMax = 50,
            MinBrightness = 6,
            MaxLightBrightness = 100
        };

    [Fact]
    public void WhenIlluminationIsMoreThanMax_Return0()
    {
        //arrange
        var adjusterUnderTest = new DynamicLightAdjuster(_model);

        //act
        var result = adjusterUnderTest.GetAppropriateBrightness(200, 0);

        //asert
        Assert.Equal(0, result);
    }

    [Fact]
    public void WhenIlluminationIsZero_ReturnMax()
    {
        //arrange
        var adjusterUnderTest = new DynamicLightAdjuster(_model);

        //act
        var result = adjusterUnderTest.GetAppropriateBrightness(0, 0);

        Assert.Equal(_model.MaxLightBrightness, result);
    }

    [Fact]
    public void WhenHalfNeeded_AddsHalf()
    {
        //arrange
        var adjusterUnderTest = new DynamicLightAdjuster(_model);

        //act
        var result = adjusterUnderTest.GetAppropriateBrightness(75, 0);

        //assert
        Assert.Equal(50, result);
    }

    [Fact]
    public void WhenALittleNeeded_AddsALittle()
    {
        //arrange
        var adjusterUnderTest = new DynamicLightAdjuster(_model);

        //act
        var result = adjusterUnderTest.GetAppropriateBrightness(99, 40);

        //assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void WhenPlenty_Returns0()
    {
        var adjusterUnderTest = new DynamicLightAdjuster(_model);

        //act
        var result = adjusterUnderTest.GetAppropriateBrightness(200, 100);

        //asert
        Assert.Equal(0, result);    
    }


}