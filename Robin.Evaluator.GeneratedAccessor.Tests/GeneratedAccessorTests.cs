using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Expressions;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Variables;
using Robin.Extensions;
using System.Collections.Immutable;

namespace Robin.Evaluator.GeneratedAccessor.Tests;

public class GeneratedAccessorTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public GeneratedAccessorTests()
    {
        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddMemberObjectAccessor<TestModel>(TestModelAccessor.GetNamedProperty)
            .AddMemberObjectAccessor<TestSubModel>(TestSubModelAccessor.GetNamedProperty);
        ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }

    [Fact]
    public void ResolveThis()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            IntValue = 42,
            StringValue = "Test"
        };
        VariablePath path = VariableParser.Parse(".");
        Assert.IsType<ThisSegment>(Assert.Single(path.Segments));
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            TestModel resolved = Assert.IsType<TestModel>(rawValue);
            Assert.Same(model, resolved);
        }
    }

    [Fact]
    public void ResolveIntValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new();
        VariablePath path = VariableParser.Parse("IntValue");
        MemberSegment member = Assert.IsType<MemberSegment>(Assert.Single(path.Segments));
        Assert.Equal("IntValue", member.MemberName);
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            int resolved = Assert.IsType<int>(rawValue);
            Assert.Equal(model.IntValue, resolved);
        }
    }

    [Fact]
    public void ResolveBooleanNull()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            BooleanValue = null
        };
        VariablePath path = VariableParser.Parse("BooleanValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveBooleanTrue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            BooleanValue = true
        };
        VariablePath path = VariableParser.Parse("BooleanValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            bool resolved = Assert.IsType<bool>(rawValue);
            Assert.True(resolved);
        }
    }

    [Fact]
    public void ResolveBooleanFalse()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            BooleanValue = false
        };
        VariablePath path = VariableParser.Parse("BooleanValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            bool resolved = Assert.IsType<bool>(rawValue);
            Assert.False(resolved);
        }
    }




    [Fact]
    public void ResolveCharNull()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            CharValue = null
        };
        VariablePath path = VariableParser.Parse("CharValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveChar()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            CharValue = 'x'
        };
        VariablePath path = VariableParser.Parse("CharValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            char resolved = Assert.IsType<char>(rawValue);
            Assert.Equal('x', resolved);
        }
    }

    [Fact]
    public void ResolveCharFalse()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            CharValue = '\0'
        };
        VariablePath path = VariableParser.Parse("CharValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            char resolved = Assert.IsType<char>(rawValue);
            Assert.Equal('\0', resolved);
        }
    }



    [Fact]
    public void ResolveStringValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            StringValue = "1001",
        };
        VariablePath path = VariableParser.Parse("StringValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            string resolved = Assert.IsType<string>(rawValue);
            Assert.Equal(model.StringValue, resolved);
        }
    }

    [Fact]
    public void ResolveNullStringValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            StringValue = null!,
        };
        VariablePath path = VariableParser.Parse("StringValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.False(facade.IsTrue(rawValue));
        }
    }

    [Fact]
    public void ResolveDoubleValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            DoubleValue = 1.2,
        };
        VariablePath path = VariableParser.Parse("DoubleValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            double resolved = Assert.IsType<double>(rawValue);
            Assert.Equal(1.2, resolved);
        }
    }

    [Fact]
    public void ResolveLongValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            LongValue = 13,
        };
        VariablePath path = VariableParser.Parse("LongValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            long resolved = Assert.IsType<long>(rawValue);
            Assert.Equal(13, resolved);
        }
    }

    [Fact]
    public void ResolveFloatValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            FloatValue = float.Epsilon,
        };
        VariablePath path = VariableParser.Parse("FloatValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            float resolved = Assert.IsType<float>(rawValue);
            Assert.Equal(float.Epsilon, resolved);
        }
    }

    [Fact]
    public void ResolveDecimalValue()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            DecimalValue = decimal.MinusOne,
        };
        VariablePath path = VariableParser.Parse("DecimalValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            decimal resolved = Assert.IsType<decimal>(rawValue);
            Assert.Equal(decimal.MinusOne, resolved);
        }
    }


    [Fact]
    public void ResolveFloatValueNaN()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            FloatValue = float.NaN,
        };
        VariablePath path = VariableParser.Parse("FloatValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            float resolved = Assert.IsType<float>(rawValue);
            Assert.Equal(float.NaN, resolved);
        }
    }


    [Fact]
    public void ResolveDoubleValueNaN()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestModel model = new()
        {
            DoubleValue = double.NaN,
        };
        VariablePath path = VariableParser.Parse("DoubleValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            double resolved = Assert.IsType<double>(rawValue);
            Assert.Equal(double.NaN, resolved);
        }
    }

    [Fact]
    public void ResolveSubModel()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new();
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            TestSubModel resolved = Assert.IsType<TestSubModel>(rawValue);
            Assert.Same(sub, resolved);
        }
    }

    [Fact]
    public void ResolveSubModelProperty()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            DateTimeValue = DateTime.UtcNow,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DateTimeValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            DateTime resolved = Assert.IsType<DateTime>(rawValue);
            Assert.Equal(sub.DateTimeValue, resolved);
        }
    }

    [Fact]
    public void ResolveSubModelPropertyMissing()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            DateTimeValue = DateTime.UtcNow,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.MissingValue");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveSubModelStringCollection()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        string[] collection = ["test"];
        TestSubModel sub = new()
        {
            StringCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.StringCollection");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.True(facade.IsCollection(rawValue, out _));
            string[] resolved = Assert.IsType<string[]>(rawValue);
            Assert.Same(collection, resolved);
        }
    }

    [Fact]
    public void ResolveSubModelStringCollectionItem()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            StringCollection = ["test"],
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.StringCollection[0]");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            string resolved = Assert.IsType<string>(rawValue);
            Assert.Equal("test", resolved);
        }
    }

    [Fact]
    public void ResolveSubModelStringCollectionMissingItem()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            StringCollection = ["test"],
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.StringCollection[1]");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveSubModelStringCollectionEmpty()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            StringCollection = [],
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.StringCollection");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.True(facade.IsCollection(rawValue, out _));
            string[] resolved = Assert.IsType<string[]>(rawValue);
            Assert.Empty(resolved);
        }
    }


    [Fact]
    public void ResolveSubModelDictionaryCollection()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        Dictionary<string, object?> collection = new()
        {
            ["test"] = "test",
        };
        TestSubModel sub = new()
        {
            DictionaryCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryCollection");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.True(facade.IsCollection(rawValue, out _));
            Dictionary<string, object?> resolved = Assert.IsType<Dictionary<string, object?>>(rawValue);
            Assert.Same(collection, resolved);
        }
    }

    [Fact]
    public void ResolveSubModelDictionaryCollectionItem()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        Dictionary<string, object?> collection = new()
        {
            ["test"] = "test",
        };
        TestSubModel sub = new()
        {
            DictionaryCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryCollection.test");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            string resolved = Assert.IsType<string>(rawValue);
            Assert.Equal("test", resolved);
        }
    }

    [Fact]
    public void ResolveSubModelDictionaryCollectionMissingItem()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        Dictionary<string, object?> collection = new()
        {
            ["test"] = "test",
        };
        TestSubModel sub = new()
        {
            DictionaryCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryCollection.TEST");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveSubModelDictionaryCollectionEmpty()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            DictionaryCollection = ImmutableDictionary<string, object?>.Empty,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryCollection");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.True(facade.IsCollection(rawValue, out _));
            ImmutableDictionary<string, object?> resolved = Assert.IsType<ImmutableDictionary<string, object?>>(rawValue);
            Assert.Empty(resolved);
        }
    }





    [Fact]
    public void ResolveSubModelDictionaryImplicitKeyCollection()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        Dictionary<ImplicitKey, object?> collection = new()
        {
            ["test"] = "test",
        };
        TestSubModel sub = new()
        {
            DictionaryImplicitKeyCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryImplicitKeyCollection");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.True(facade.IsTrue(rawValue));
            Assert.True(facade.IsCollection(rawValue, out _));
            Dictionary<ImplicitKey, object?> resolved = Assert.IsType<Dictionary<ImplicitKey, object?>>(rawValue);
            Assert.Same(collection, resolved);
        }
    }

    [Fact]
    public void ResolveSubModelDictionaryImplicitKeyCollectionItem__NoImplicitConversion()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        Dictionary<ImplicitKey, object?> collection = new()
        {
            ["test"] = "test",
        };
        TestSubModel sub = new()
        {
            DictionaryImplicitKeyCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryImplicitKeyCollection.test");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveSubModelDictionaryImplicitKeyCollectionMissingItem()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        Dictionary<ImplicitKey, object?> collection = new()
        {
            ["test"] = "test",
        };
        TestSubModel sub = new()
        {
            DictionaryImplicitKeyCollection = collection,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryImplicitKeyCollection.other");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.False(facade.IsCollection(rawValue, out _));
            Assert.Null(rawValue);
        }
    }

    [Fact]
    public void ResolveSubModelDictionaryImplicitKeyCollectionEmpty()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        TestSubModel sub = new()
        {
            DictionaryImplicitKeyCollection = ImmutableDictionary<ImplicitKey, object?>.Empty,
        };
        TestModel model = new()
        {
            SubModel = sub,
        };
        VariablePath path = VariableParser.Parse("SubModel.DictionaryImplicitKeyCollection");
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(model))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(facade);
            Assert.False(facade.IsTrue(rawValue));
            Assert.True(facade.IsCollection(rawValue, out _));
            ImmutableDictionary<ImplicitKey, object?> resolved = Assert.IsType<ImmutableDictionary<ImplicitKey, object?>>(rawValue);
            Assert.Empty(resolved);
        }
    }
}
