# The Process of Binding

In theory, the process of binding a Swift binary into C# should be as simple as:

1. Generate and consume the abi.json file
2. Extract and demangle the symbols from the binary
3. Iterate over every type and function and gnerate C# and/or supporting Swift code
4. Generate type datatbase entries
5. Optionally compile generated code

Unfortunately, it is not so simple. There are complications that need to be considered because of special cases in the Swift language as well as special cases or limitations in C#.

For example
- `open` Swift classes need to be handled differently from `public` swift classes.
- Structs and enums that are `@frozen` are passed differently from structs that are not. 
- Structs and enums that contain or may contain non-blitable members need to be defined differently.
- Enums come in different forms that lend themselves to different representations in C#.
- C# doesn't have methods or properties on enums so those need to be put into extension methods.
- Any protocol implementation in C# will need proxy types in both C# and Swift
- P/invokes and `[UnmanagedCallersOnly]` entrypoints (reverse p/invokes) need to be defined in a separate class from the bound type they support if that bound type is generic.
- Mutating methods on value types need to be handled differently from non-mutating methods.
- C# does not support methods that are homonyms.

All of this makes the process of generating code challenging.

The complexities fall into several broad categories:
- Type and member naming
- Multiple types being defined in multiple languages concurrently
- Marhshaling handled differently based on the type of the parameter and the type of the function
- Implicit arguments
- Versioning based on the `@available` attribute.

For this reason I strongly recommend using code-generation tools that can work in a non-linear fashion. There are several ways to achieve this, but I would strongly recommend using the Dynamo framework from Binding Tools for Swift as it can handle both C# and Swift and generates non-linearly. In addition, it shouldn't be a stretch to generate code in parallel on type boundaries.

Because of this, I think we should adopt a strategy and factory pattern for handlers at various levels.

The general pattern would work like this:
1. Start with a Swift language entity
2. Aggregate information about that entity
3. Select a factory to create a handler for that entity
4. The handler will generate a context object for handling that entity
5. Execute a series of steps through the handler that will do work apropriate for each step.
6. Aggregate the result and generate as needed.

Handlers will contain factories for handling sub steps, if needed.

For example, given the following Swift code:
```Swift
public func generateAClass(String name) -> SomeClass { }
```
The process would look at this and identify this as a top-level function and will select a handler factory for it.
The handler will create a context for the object which would include a class for the top-level object to live in (C# doesn't have top-level functions) and a class to hold top-level pinvokes and a function generation context which would include a place to place function argument declarations, generic declarations, function argument pre-marshaling code, pinvoke argument declarations, pinvoke argument expressions, post-marshaling code, return type declaration, and a return expression. 

The handler will execute a step to name the function and the associated pinvoke, including the entry point and library.
Then for each argument, it will gather information about each argument and from the function handler get a factory to build an argument handler for type `String`. This will in turn name the argument, generate the C# type and add it to the C# argument declaration. It will define the argument type for the pinvoke and add it to the C# pinvoke argument list. If needed, it will generate premarshal code and add it to the premarshal list and post marshal code, and finally an expression for calling the pinvoke.

A similar process will be done for handling the return type and value. In this case, the pinvoke return type will be a `NativeHandle` and it will be used in conjunction with a registry to either retrieve an already existing C# object that is bound to that handle or it will build one through a factory.

After all this is done, the function handler will finish up by aggregating all the information, writing the C# method and writing the C# pinvoke.

By doing this, all the special cases get isolated in their own code and state related to the code, if any, can be kept in the handler's context object.

For example, if there is a pre-marshal set up and a post marshal teardown, that code can be written in the same step.

## Handler Factory Selection

A handler factory for a given entity will be given a blob of information about the prospective entity. It will have a try-get predicate to answer whether or not it can handle that entity and a method to build a handler.

A generalized handler might look like this:

```csharp
public interface Handler<EntityInformation, ParentContext> {
    void Begin (ParentContext parentContext, EntityInformation info);
    void Process ();
    void End ();
}
```

A factory might look like this:
```csharp
public interface Factory<EntityInformation, ParentContext> {
    bool TryGetHandler (EntityInformation info, [NotNullWhen(true)] out Handler<EntityInformation, ParentContext>? handler);
}
```

Finally, since Swift allows inner types, all of these steps should be naturally recursive.
It should also be noted that all entities at top level can be processed in parallel trivially. Inner types can also be processed in parallel at the scope at which they're declared, but some care needs to be taken to ensure that two inner types at the same scope don't step on each other in writing to the non-linear representation or have issues when creating p/invokes. This could be done with appropriate locks or using `Concurrent{Dictionary/List}` for the representational types.