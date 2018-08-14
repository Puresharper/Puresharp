

# Puresharp API .NET 4.0+
Puresharp is a set of features for .NET 4.5.2+ to help to build a software architecture as cleaner as possible without sacrificing performance.

## Purpose
- Dependency Injection Container
- Aspect Oriented Programming
- Metadata Reflection API

## Overview
This framework is divided into 2 parts :
- **IPuresharp** &nbsp;&nbsp;[![NuGet](https://img.shields.io/nuget/v/IPuresharp.svg)](https://www.nuget.org/packages/IPuresharp)

IPuresharp is a nuget package dedicated to rewrite assemblies (using **Mono.Cecil**) to allow them to be highly customizable at runtime. IPuresharp won't add a new library reference to assmblies, but only include a post build process to automatically rewrite assemblies just after success build.

- **Puresharp** &nbsp;&nbsp;[![NuGet](https://img.shields.io/nuget/v/Puresharp.svg)](https://www.nuget.org/packages/Puresharp)

Puresharp is a nuget package offering various features useful for designing a healthy and productive architecture. This package also includes all the artillery to easily handle the elements that brings the IL writer IPuresharp. The nuget package add a library (Puresharp.dll) without any other depencies.

## Preview
_paste here preview code for DI container, AOP & reflection discovery_

## FAQ

### Dependency Injection Container

_paste here the Di Container FAQ!_

### Aspect Oriented Programming

- **How this AOP featues are differents from the most of AOP Framework?** 
_Most of time developping cross-cutting source code required reflection and boxing to be done. Puresharp offer a way to define it using Linq Expressions or ILGenerator because cross-cutting source code have to manage not statically known datas. No need factory and no need base class is the second exclusive feature that make the difference because interception is not based on method overriding, MarshalByRef nor ContextBoundObject._

- **How fast is "low performance overhead"?** 
_There is no perceptible overhead when Linq Expressions or ILGenerator are used. Basic advice introduce a light overhead caused by boxing and arguments array creation. However, MethodBase is not prepared if capture is not required in lambda expression._

- **Why I have to use IPuresharp?** 
_Interception is based on IPuresharp. Indeed IPuresharp add a build action to rewrite CIL to make assembly "Architect Friendly" by injecting transparents and hidden features to to grant full execution control at runtime.

- **Is an attribute required to identify a mehod to weave?** 
_No you can identify a method by the way you want. There is a Pointcut concept based on MethodBase recognition to target methods : Func<MethodBase, bool>._

- **Can I intercept constructor? If yes, how do I implement it?**
_Constructor interception is supported and is treated like another method with declaring type as first argument and void for return type._

- **Is generic types and methods supported?**
_Generic types and methods ares fully supported by injection._

- **Can I intercept async methods?**
_Async methods ares fully supported by injection and offer a way to intercept each asynchronous steps._

## More

_paste here some links talking about DI, AOP, Puresharp, Good Practice._
