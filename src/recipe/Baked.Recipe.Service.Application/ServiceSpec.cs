﻿using Baked.Architecture;
using Baked.Business;
using Baked.Caching;
using Baked.Communication;
using Baked.Core;
using Baked.Database;
using Baked.ExceptionHandling;
using Baked.MockOverrider;
using Baked.Orm;
using Baked.Testing;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

using ITransaction = NHibernate.ITransaction;

namespace Baked;

public abstract class ServiceSpec : Spec
{
    static IServiceProvider _serviceProvider = default!;

    internal static IServiceProvider ServiceProvider => _serviceProvider;
    internal static ISession Session => _serviceProvider.GetRequiredService<ISession>();

    protected static ApplicationContext Init(
        Func<BusinessConfigurator, IFeature<BusinessConfigurator>> business,
        Func<CachingConfigurator, IFeature<CachingConfigurator>>? caching = default,
        Func<CommunicationConfigurator, IFeature<CommunicationConfigurator>>? communication = default,
        Func<CoreConfigurator, IFeature<CoreConfigurator>>? core = default,
        Func<DatabaseConfigurator, IFeature<DatabaseConfigurator>>? database = default,
        Func<ExceptionHandlingConfigurator, IFeature<ExceptionHandlingConfigurator>>? exceptionHandling = default,
        Func<MockOverriderConfigurator, IFeature<MockOverriderConfigurator>>? mockOverrider = default,
        Func<OrmConfigurator, IFeature<OrmConfigurator>>? orm = default,
        Action<ApplicationDescriptor>? configure = default
    )
    {
        caching ??= c => c.ScopedMemory();
        communication ??= c => c.Mock();
        core ??= c => c.Mock();
        database ??= c => c.InMemory();
        exceptionHandling ??= c => c.Default();
        mockOverrider ??= c => c.FirstInterface();
        orm ??= c => c.AutoMap();

        var context = Init(app =>
        {
            app.Layers.AddCodeGeneration();
            app.Layers.AddConfiguration();
            app.Layers.AddDataAccess();
            app.Layers.AddDependencyInjection();
            app.Layers.AddDomain();
            app.Layers.AddMonitoring();
            app.Layers.AddTesting();

            app.Features.AddBusiness(business);
            app.Features.AddCaching(caching);
            app.Features.AddCodingStyles([
                c => c.AddRemoveChild(),
                c => c.CommandPattern(),
                c => c.EntityExtensionViaComposition(),
                c => c.EntitySubclassViaComposition(),
                c => c.ObjectAsJson(),
                c => c.RemainingServicesAreSingleton(),
                c => c.RichEntity(),
                c => c.ScopedBySuffix(),
                c => c.SingleByUnique(),
                c => c.UriReturnIsRedirect(),
                c => c.UseBuiltInTypes(),
                c => c.UseNullableTypes(),
                c => c.WithMethod()
            ]);
            app.Features.AddCommunication(communication);
            app.Features.AddCore(core);
            app.Features.AddDatabase(database);
            app.Features.AddExceptionHandling(exceptionHandling);
            app.Features.AddLifetimes([
                c => c.Scoped(),
                c => c.Singleton(),
                c => c.Transient()
            ]);
            app.Features.AddMockOverrider(mockOverrider);
            app.Features.AddOrm(orm);

            configure?.Invoke(app);
        });

        _serviceProvider = context.GetServiceProvider();

        return context;
    }

    ITransaction _transaction = default!;
    internal Dictionary<string, string> Settings { get; private set; } = default!;

    public override void SetUp()
    {
        base.SetUp();

        Caster.SetServiceProvider(_serviceProvider);
        _transaction = Session.BeginTransaction();
        Settings = [];
        MockMe.TheConfiguration(settings: Settings, defaultValueProvider: GetDefaultSettingsValue);

        // This is the initial release date. Do not change this to avoid
        // potential "Cannot go back in time." errors.
        MockMe.TheTime(now: new DateTime(2023, 06, 15, 16, 59, 00), reset: true);
    }

    public override void TearDown()
    {
        base.TearDown();

        Session.Flush();
        _transaction.Rollback();
        Session.Clear();

        GiveMe.The<IMockOverrider>().Reset();
        GiveMe.AMemoryCache(clear: true);
    }

    protected virtual string? GetDefaultSettingsValue(string key) =>
        "test value";
}