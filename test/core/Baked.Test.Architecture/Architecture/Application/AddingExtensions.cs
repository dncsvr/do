﻿namespace Baked.Test.Architecture.Application;

public class AddingExtensions : ArchitectureSpec
{
    [Test]
    public void Layer_is_added_without_any_options()
    {
        var bake = GiveMe.ABake();
        var layer1 = MockMe.ALayer();
        var layer2 = MockMe.ALayer();

        var app = bake.Application(app =>
        {
            app.Layers.Add(layer1);
            app.Layers.Add(layer2);
        });

        app.Run();

        layer1.VerifyInitialized();
        layer2.VerifyInitialized();
    }

    [Test]
    public void Feature_is_added_to_configure_layers()
    {
        var bake = GiveMe.ABake();
        var layer = MockMe.ALayer();
        var feature1 = MockMe.AFeature();
        var feature2 = MockMe.AFeature();

        var app = bake.Application(app =>
        {
            app.Layers.Add(layer);

            app.Features.Add(feature1);
            app.Features.Add(feature2);
        });

        app.Run();

        feature1.VerifyInitialized();
        feature2.VerifyInitialized();
    }

    [Test]
    public void Feature_configures_target_configurations_of_the_layers()
    {
        var bake = GiveMe.ABake();
        var layer = MockMe.ALayer(target: "text");
        var feature = MockMe.AFeature();

        var app = bake.Application(app =>
        {
            app.Layers.Add(layer);

            app.Features.Add(feature);
        });

        app.Run();

        feature.VerifyConfigures(target: "text");
    }

    [Test]
    public void Layers_can_provide_multiple_configuration_targets()
    {
        var bake = GiveMe.ABake();
        var layer = MockMe.ALayer(targets: ["text", 10]);
        var feature = MockMe.AFeature();

        var app = bake.Application(app =>
        {
            app.Layers.Add(layer);

            app.Features.Add(feature);
        });

        app.Run();

        feature.VerifyConfigures(target: "text");
        feature.VerifyConfigures(target: 10);
    }

    [Test]
    public void Layers_are_skipped_when_they_provide_no_configuration_target()
    {
        var bake = GiveMe.ABake();
        var layer = MockMe.ALayer(targets: []);
        var feature = MockMe.AFeature();

        var app = bake.Application(app =>
        {
            app.Layers.Add(layer);

            app.Features.Add(feature);
        });

        app.Run();

        feature.VerifyConfiguresNothing();
    }

    [Test]
    public void Adding_the_same_layer_more_than_once_gives_error()
    {
        var bake = GiveMe.ABake();
        var layer = MockMe.ALayer(id: "DuplicateLayer");

        var bakeAction = () => bake.Application(app =>
        {
            app.Layers.Add(layer);
            app.Layers.Add(layer);
        });

        bakeAction.ShouldThrow<InvalidOperationException>().Message.ShouldBe(
            $"Cannot add 'DuplicateLayer', it was already added."
        );
    }

    [Test]
    public void Adding_the_same_feature_more_than_once_gives_error()
    {
        var bake = GiveMe.ABake();
        var feature = MockMe.AFeature(id: "DuplicateFeature");

        var bakeAction = () => bake.Application(app =>
        {
            app.Features.Add(feature);
            app.Features.Add(feature);
        });

        bakeAction.ShouldThrow<InvalidOperationException>().Message.ShouldBe(
            $"Cannot add 'DuplicateFeature', it was already added."
        );
    }
}