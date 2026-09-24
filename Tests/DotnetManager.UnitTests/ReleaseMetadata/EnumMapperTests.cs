using DotnetManager.ReleaseMetadata.Contracts;
using DotnetManager.ReleaseMetadata.Mapping;
using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.UnitTests.ReleaseMetadata;

public class EnumMapperTests
{
    [Theory]
    [InlineData(RawReleaseTypes.Lts, ReleaseTypes.Lts)]
    [InlineData(RawReleaseTypes.Sts, ReleaseTypes.Sts)]
    public void ReleaseTypeMapperMapsKnownValues(RawReleaseTypes raw, ReleaseTypes expected)
    {
        Assert.Equal(expected, ReleaseTypeMapper.Map(raw));
    }

    [Theory]
    [InlineData(RawSupportPhases.Preview, SupportPhases.Preview)]
    [InlineData(RawSupportPhases.GoLive, SupportPhases.GoLive)]
    [InlineData(RawSupportPhases.Active, SupportPhases.Active)]
    [InlineData(RawSupportPhases.Maintenance, SupportPhases.Maintenance)]
    [InlineData(RawSupportPhases.Eol, SupportPhases.Eol)]
    public void SupportPhaseMapperMapsKnownValues(RawSupportPhases raw, SupportPhases expected)
    {
        Assert.Equal(expected, SupportPhasesMapper.Map(raw));
    }

    [Fact]
    public void ReleaseTypeMapperRejectsUnknownValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ReleaseTypeMapper.Map((RawReleaseTypes)999));
    }
}