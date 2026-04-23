# cave-extensions

A compact .NET library with reusable extensions and helper utilities for everyday development tasks.

## Contents

- [Supported target platforms](#supported-target-platforms)
- [Static extension classes](#static-extension-classes)
- [Collections (`Collections/**`)](#collections-collections)
- [Backports for modern language features on older frameworks](#backports-for-modern-language-features-on-older-frameworks)
- [Unicode, encoding, and hashing](#unicode-encoding-and-hashing)
- [Progress system (`Progress/**`)](#progress-system-progress)
- [Contributing](#contributing)
- [License](#license)

## Supported target platforms

This library is designed to support multiple target platforms, including:

- .NET Framework 2.0
- .NET Framework 3.5
- .NET Framework 4.0
- .NET Framework 4.5
- .NET Framework 4.6
- .NET Framework 4.7
- .NET Framework 4.8
- .NET Standard 2.0
- .NET Standard 2.1
- .NET 8
- .NET 10

## Static extension classes

The library includes several static extension types, some of which are conditionally compiled depending on the target framework.

### `StringExtensions`

Comprehensive string and text helpers, including:

- substring operations (`Before*`, `After*`, `SubstringEnd`)
- formatting (`FormatSize`, `FormatBinarySize`, `FormatTime`, `FormatSeconds`)
- parsing (`ParseDateTime`, `ParseHexString`, `ParsePoint`, `ParseRectangle`, `ParseSize`)
- escaping and unescaping (`Escape`, `EscapeUtf8`, `Unescape`)
- case conversion (`GetCamelCaseName`, `GetPascalCaseName`, `GetSnakeCaseName`, `GetKebabCaseName`)
- split and join extensions (`SplitNewLine`, `SplitKeepSeparators`, `Join*`)
- safe conversion (`ToInt32`, `ToInt64`, `ToUInt32`, `ToBool`)
- framework-specific backport additions (`*.Backports`, `*.NetCoreApp1`, `*.Latest`)

### `TypeExtension`

Reflection and conversion helpers for `Type`:

- universal runtime conversion (`ConvertValue`, `ConvertPrimitive`)
- attribute access (`GetAttribute`, `GetAttributes`, `HasAttribute`)
- assembly metadata access (`GetCompanyName`, `GetProductName`)
- reflection API backports for older frameworks (`GetMethod`, `GetProperties`, `GetProperty`, `GetFields`, ...)
- structure checks (`IsStruct`)

### `AssemblyExtension`

Extensions for `Assembly`:

- resolve assembly path and directory
- read product and company names from assembly attributes
- backports for `GetCustomAttributes<T>()` on older frameworks

### `ArrayExtension`

Extensions for `byte[]`, arrays, `IList<T>`, and `IEnumerable<T>`:

- concatenation and slicing (`Concat`, `GetRange`, `SubRange`)
- pattern and byte processing (`IndexOf`, `RangeEquals`, `ReplaceFirst`, `StartsWith`)
- reverse iteration
- `Empty<T>()` backport

### `IListExtension`

Extensions for `IList<T>` and arrays:

- read-only wrapper (`AsReadOnly`)
- reverse iteration for lists

### `DictionaryExtensions`

Extension for `IDictionary<TKey,TValue>`:

- `TryGetValue(..., defaultValue)` with fallback value

### `EnumExtension`

Enum helpers, including backports:

- retrieve flags (`GetFlags`, `GetString`)
- robust parsing (`Parse`, `TryParse`)
- `HasFlag` backport for older targets

### `DateTimeExtension`

Validation helpers:

- `IsValidDate(year, month, day)`
- `IsValidTime(hours, minutes, seconds)`

### `TimeSpanExtension`

Small utility extension:

- `Absolute()` for non-negative time spans

### `ObjectExtension`

Reflection-based object access:

- recursive property enumeration
- property value access by path (`GetPropertyValue`, `TryGetPropertyValue`)
- typed property output
- property comparison between objects (`PropertiesEqual`)

### `PropertyInfoExtension`

Extensions for `PropertyInfo`:

- `IsIndexProperty()`
- plus a backport file with `GetGetMethod`, `GetSetMethod`, `SetValue`, and `GetValue` for older targets

### `StreamExtensions`

Extensions for `Stream`:

- block-based copying (`CopyBlocksTo`)
- full and partial reads (`ReadAllBytes`, `ReadBlock`)
- UTF-8 writing (`WriteUtf8`)
- configurable block size

### `IPAddressExtension`

Networking extensions:

- broadcast and subnet calculation
- reverse lookup zones for IPv4 and IPv6
- multicast detection
- address masking and byte-based manipulation

### Backport extension classes (`Backports/**`)

- `BackportedExtensions` in `System.Linq` (mainly for `NET20`): large LINQ backport (`Where`, `Select`, `GroupBy`, `Join`, `Sum`, `Average`, `Min/Max`, ...)
- `BackportedExtensions` in `Backports/StringExtensions.cs`: small string backports (`StartsWith(char)`, `EndsWith(char)`)

## Collections (`Collections/**`)

The collections area provides custom data structures for sets, range expressions, and 2D mappings, including read-only and concurrent variants.

### Core building blocks

- `Counter` / `CountEnumerator`  
  Integer counting ranges as `IEnumerable<int>` with `Start`, `End`, `Step`, `Contains(...)`, and deterministic iteration.

- `RangeExpression`  
  Parser and model for range strings (for example single values, intervals, step sizes, `*`).  
  Supports `Parse(...)`, `Contains(...)`, union (`+`), and enumeration of all contained values.

### Set structures (`Collections/Generic`)

- `IItemSet<T>`  
  Unified set API beyond `ICollection<T>` (`Include`, `IncludeRange`, `TryRemoveRange`, `IsEmpty`).

- `Set<T>`  
  Classic unique set based on `HashSet<T>`, with set operators:
  - intersection `&`
  - union `|`
  - difference `-`
  - symmetric difference `^`

- `ReadOnlySet<T>`  
  Read-only wrapper around `IItemSet<T>`; all mutating operations throw `ReadOnlyException`.

- `IndexedSet<T>`  
  Set with stable index-based access (`IList<T>` + unique values).  
  Combines `List<T>` for ordering and a dictionary index for fast lookups.

### 2D sets / pair mappings

- `ItemPair<T1,T2>`  
  Simple pair container (`A`, `B`) for associated values.

- `IItemSet<TKey,TValue>`  
  Interface for bidirectional pair structures with access to the A and B dimensions.

- `Set<TKey,TValue>`  
  Unique on the A side, while the B side may contain duplicate values.  
  Optimized for A-side indexing; B-related operations are intentionally limited.

- `UniqueSet<TKey1,TKey2>`  
  Unique on **both** sides (A and B), including lookups in both directions (`GetA`, `GetB`, `TryGetA`, `TryGetB`).

- `ReadOnlyListA<,>` / `ReadOnlyListB<,>`  
  Read-only views over the A and B side of a 2D set.

### Concurrent collections

- `ConcurrentSet<T>` (`Collections/Concurrent`)  
  Thread-safe set implementation based on `ConcurrentDictionary<T, byte>`, including the same set operators as `Set<T>`.

### Collection-related backports

- `Backports/Collections/ObjectModel/ReadOnlyDictionary<TKey,TValue>`  
  Read-only dictionary backport for older targets.

- LINQ backports (`Backports/Linq/*`)  
  Include supporting collection types such as `Lookup<TKey,TElement>`, `Grouping<TKey,TElement>`, and `OrderedEnumerable<T>` for older frameworks.

## Backports for modern language features on older frameworks

The `Backports/**` directory contains targeted backports that make newer C# features and APIs available on older target frameworks.

- `Backports/Index.cs`  
  Backport of `System.Index` for index access, including `from end` semantics (`^`).

- `Backports/Range.cs`  
  Backport of `System.Range` as the foundation for range expressions such as `start..end`.

- `Backports/Runtime/CompilerServices/IsExternalInit.cs`  
  Provides the compiler type required for `init` setters and `record` compilation on older frameworks.

- `Backports/Linq/BackportedExtensions.cs` (`#if NET20`)  
  Adds core LINQ functionality for very old platforms, for example:
  - sequence operators: `Any`, `All`, `Cast`, `OfType`, `Concat`
  - generation: `Range`, `Repeat`, `Empty`
  - processing: `Skip`, `Take`, `Reverse`, `ToArray`, `ToList`
  - aggregation: `Aggregate`, `Average`, `LastOrDefault`, `ElementAtOrDefault`

This keeps modern tooling and language constructs broadly compatible, even when the library is deployed on older .NET target platforms.

## Unicode, encoding, and hashing

### Lightweight `IUnicode` implementations (interop-friendly without a marshaller)

The library includes lightweight `IUnicode` types designed for low allocation overhead and direct interop scenarios:

- data is stored in a compact, struct-based representation
- focused on blittable and interop-friendly memory layouts so values can be passed without an additional marshaller
- operates directly on bytes and characters instead of heavyweight object graphs
- suitable for P/Invoke-adjacent paths, protocol parsers, and performance-critical string conversion

Goal: modern Unicode handling on older frameworks with minimal overhead.

### Extensions

The extensions complement these types with convenient helpers for Unicode and text-related paths:

- conversion between internal Unicode representations and .NET strings
- simplified processing over sequences and spans, depending on framework support
- consistent APIs for parsing, comparing, and serializing text data

This keeps the low-level types lightweight while still making them ergonomic to use in application code.

### `BaseX`, `Base32`, `Base64`

The library provides flexible encoding and decoding for binary data:

- `BaseX` as a general mechanism for alphabet-based encodings
- specialized `Base32` and `Base64` implementations for common transport and persistence scenarios
- consistent encode/decode APIs, optionally supporting variants depending on the implementation (for example alphabet or padding behavior)

Use cases: reproducible text representations for IDs, tokens, signatures, and binary payloads.

### `HashCode` implementations

For target platforms without modern runtime features, the library provides compatible hash helpers:

- deterministic combination of multiple fields or values into a single hash
- performance-oriented mixing functions for good distribution
- consistent behavior across older and newer frameworks

This simplifies robust `GetHashCode()` implementations in custom types and reduces platform-specific differences.

## Progress system (`Progress/**`)

The `Progress` subsystem provides centralized, thread-safe tracking for running processes.

### Architecture

- `IProgress`  
  Unified progress object with:
  - `Value` (`0..1`)
  - `Text`
  - `Created`, `Identifier`, `Source`
  - `Completed`
  - event `Updated`
  - methods `Update(...)` and `Complete()`

- `ProgressItem`  
  Internal concrete implementation of `IProgress`.  
  Behavior:
  - only monotonic updates are allowed; smaller values are ignored
  - values greater than `1` are rejected
  - after `Complete()`, further updates are invalid
  - raises the `Updated` event when changed or completed

- `IProgressManager` / `ProgressManagerBase`  
  Manages active progress instances and centrally forwards their events.  
  Completed items are automatically removed from `Items`.

- `ProgressManager` (static facade)  
  Global entry point:
  - `CreateProgress(source)`
  - `Items` for currently running operations
  - global `Updated` event
  - replaceable implementation through `Instance`

- `DefaultProgressManager`  
  Default implementation based on `ProgressManagerBase`.

### Event model

- progress events use `Progress/ProgressEventArgs` with exactly one payload:
  - `Progress` (`IProgress`)
- this ensures every event handler always receives the full state of the corresponding progress object

### ETA / estimation system

- `IEstimation`  
  API for runtime and remaining-time estimation:
  - `Elapsed`, `Started`
  - `Progress`, `ProgressPercent`
  - `EstimatedCompletionTime`, `EstimatedTimeLeft`
  - `Update(...)`, `Reset()`

- `Estimation` (abstract base)  
  Stores history points (`EstimationItem`) and provides:
  - monotonic progress history
  - bounded history (`MaximumItems`)
  - `Updated` event with `EstimationUpdatedEventArgs`

- `LinearEstimation`  
  Linear ETA calculation based on progress and elapsed time.  
  For very small progress values, it falls back to a conservative large remaining time.

### Summary

The system clearly separates:
1. **tracking** (`IProgress`, `ProgressItem`)
2. **global management and event distribution** (`ProgressManager*`)
3. **time estimation** (`IEstimation`, `Estimation`, `LinearEstimation`)

This makes it easy to report progress per task, observe it globally, and optionally enrich it with ETA information.

## Contributing

Contributions are welcome:

1. Create a fork
2. Create a feature branch
3. Commit your changes
4. Open a pull request

## License

See `LICENSE` in the repository.


