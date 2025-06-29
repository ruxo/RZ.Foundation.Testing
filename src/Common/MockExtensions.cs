﻿using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace RZ.Foundation.Testing;

[PublicAPI]
public static class MockExtensions
{
    public static IServiceCollection BuildFor<T>(this IServiceCollection services, IReadOnlyList<Type>? skip = null) where T : class =>
        services.BuildFor(typeof(T), skip);

    public static IServiceCollection BuildFor(this IServiceCollection services, Type type, IReadOnlyList<Type>? skip = null) {
        var skipTypes = skip ?? [];
        var subscriptions = services.ToSeq().Map(i => i.ServiceType).ToHashSet();
        var ctorParams = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single().GetParameters();
        var missingParams = from p in ctorParams
                            where !typeof(ILogger).IsAssignableFrom(p.ParameterType) && !subscriptions.Contains(p.ParameterType) && !skipTypes.Contains(p.ParameterType)
                            select p;

        foreach (var p in missingParams){
            if (p.ParameterType.IsAbstract)
                services.AddSingleton(p.ParameterType, CreateMockByType(p.ParameterType).Object);
            else{
                services.AddTransient(p.ParameterType);
                services.BuildFor(p.ParameterType);
            }
        }
        return services;
    }

    public static IServiceCollection UseLogger(this IServiceCollection services, ITestOutputHelper output) {
        services.AddSingleton(output);
        services.AddSingleton(typeof(ILogger<>), typeof(TestLogger<>));
        return services;
    }

    public static T Create<T>(this IServiceProvider sp, params object[] parameters) =>
        ActivatorUtilities.CreateInstance<T>(sp, parameters);

    public static T BuildAndCreate<T>(this IServiceCollection services, params object[] parameters) where T : class =>
        services.BuildFor<T>().BuildServiceProvider().Create<T>(parameters);

    public static Mock CreateMockByType(Type type) {
        var ctor = typeof(Mock<>).MakeGenericType(type).GetConstructor([])!;
        return (Mock) ctor.Invoke(null);
    }

    public static T Mock<T>(this IServiceCollection services, Action<Mock<T>> setup) where T : class {
        var mock = new Mock<T>();
        setup(mock);
        services.AddSingleton(mock.Object);
        return mock.Object;
    }
}