using Do.Architecture;
using Do.Branding;

namespace Do.Test;

public static class ArchitectureSpecExtensions
{
    #region Build

    public static Build ABuild(this Spec.Stubber source,
        IBanner? banner = default
    )
    {
        banner ??= source.Spec.MockMe.ABanner();

        return new(banner);
    }

    #endregion

    #region Banner

    public static IBanner ABanner(this Spec.Mocker source) =>
        new Mock<IBanner>().Object;

    public static void VerifyPrinted(this IBanner source) =>
        Mock.Get(source).Verify(b => b.Print());

    #endregion

    #region Layer

    public static ILayer ALayer(this Spec.Mocker source,
        object? configurationTarget = default
    )
    {
        var result = new Mock<ILayer>();

        result.Setup(l => l.GetPhases()).Returns(Enumerable.Empty<IPhase>());

        if (configurationTarget != null)
        {
            result.Setup(l => l.GetConfigurationTarget(It.IsAny<ApplicationContext>())).Returns(ConfigurationTarget.Create(configurationTarget));
        }

        return result.Object;
    }

    public static void VerifyInitialized(this ILayer source) =>
        Mock.Get(source).Verify(l => l.GetPhases());

    #endregion

    #region Feature

    public static IFeature AFeature(this Spec.Mocker source) =>
        new Mock<IFeature>().Object;

    public static void VerifyInitialized(this IFeature source) =>
        Mock.Get(source).Verify(f => f.Configure(It.IsAny<ConfigurationTarget>()));

    public static void VerifyConfigures(this IFeature source, object configurationTarget) =>
        Mock.Get(source).Verify(f => f.Configure(ConfigurationTarget.Create(configurationTarget)));

    #endregion
}
