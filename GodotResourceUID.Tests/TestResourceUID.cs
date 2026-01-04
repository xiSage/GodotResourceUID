namespace GodotResourceUID.Tests;

public class TestResourceUID
{
    [Fact]
    public void MustEncodeDecodeMaximumMinimumUIDCorrectly()
    {
        Assert.Equal("uid://d4n4ub6itg400", ResourceUID.IdToText(0x7fffffffffffffff));
        Assert.Equal(0x7fffffffffffffff, ResourceUID.TextToId("uid://d4n4ub6itg400"));

        Assert.Equal("uid://a", ResourceUID.IdToText(0));
        Assert.Equal(0, ResourceUID.TextToId("uid://a"));
    }

    [Fact]
    public void MustEncodeDecodeInvalidUIDsCorrectly()
    {
        Assert.Equal("uid://<invalid>", ResourceUID.IdToText(-1));
        Assert.Equal(-1, ResourceUID.TextToId("uid://<invalid>"));

        Assert.Equal(ResourceUID.IdToText(-1), ResourceUID.IdToText(-2));

        Assert.Equal(-1, ResourceUID.TextToId("dm3rdgs30kfci"));
    }

    [Fact]
    public void MustEncodeDecodeVariousUIDsCorrectly()
    {
        Assert.Equal("uid://b", ResourceUID.IdToText(1));
        Assert.Equal(1, ResourceUID.TextToId("uid://b"));

        Assert.Equal("uid://dm3rdgs30kfci", ResourceUID.IdToText(8060368642360689600));
        Assert.Equal(8060368642360689600, ResourceUID.TextToId("uid://dm3rdgs30kfci"));
    }
}
