# System.Runtime.Versioning

netstandard2.0? https://github.com/dotnet/corefx/issues/12247

# System.SystemException

netstandard2.0? https://github.com/dotnet/corefx/issues/9854

# System.Security.Permissions

netstandard2.0?
- FileIOPermission - https://github.com/dotnet/corefx/issues/13289
- FileIOPermissionAttribute - https://github.com/dotnet/corefx/issues/11111

# System.ConfigurationException

Maybe do what they did here? https://github.com/dotnet/corefx/pull/12194/files

# System.Runtime.ConstrainedExecution

They recommend to strip out CERs https://github.com/dotnet/corefx/issues/1345#issuecomment-147569967
E.g. Remove ReliabilityContractAttribute usages.

# Microsoft.Win32.Registry

Calling its methods will throw exceptions on Linux and Mac.

# DataTable

This conflicts with the types in System.Data.Common. Remove?

# Converter<T1, T2>

Missing? netstandard2.0? not clear from here https://github.com/dotnet/corefx/issues/9995

# mscorlib


# https://github.com/mono/mono/tree/b6a3005/mcs/class/System.Data/ReferenceSources

Migration/Mono.System.Data.ReferenceSources

# https://github.com/dotnet/corefx/blob/master/src/System.Security.Permissions

Does not seem to be a Nuget package available

