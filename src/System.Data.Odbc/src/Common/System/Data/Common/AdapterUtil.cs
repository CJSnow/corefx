﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// TODO[tinchou]: fix usages of System.Configuration
// TODO[tinchou]: check Microsoft.SqlServer usages
// TODO[tinchou]: check GetComputerNameDnsFullyQualified method and SafeNativeMethods.cs
// TODO[tinchou]: check RevertAssert usages
// TODO[tinchou]: FileIOPermission usages
// TODO[tinchou]: check using SysES = System.EnterpriseServices;

using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SysTx = System.Transactions;

namespace System.Data.Common
{
    internal static class ADP
    {
        // The class ADP defines the exceptions that are specific to the Adapters.f
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource Framework.txt.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error messages.
        // The resource Framework.txt will ensure proper string text based on the appropriate
        // locale.

        internal static Task<T> CreatedTaskWithException<T>(Exception ex)
        {
            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            completion.SetException(ex);
            return completion.Task;
        }

        internal static Task<T> CreatedTaskWithCancellation<T>()
        {
            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            completion.SetCanceled();
            return completion.Task;
        }

        internal static Exception ExceptionWithStackTrace(Exception e)
        {
            try
            {
                throw e;
            }
            catch (Exception caught)
            {
                return caught;
            }
        }

        // NOTE: Initializing a Task in SQL CLR requires the "UNSAFE" permission set (http://msdn.microsoft.com/en-us/library/ms172338.aspx)
        // Therefore we are lazily initializing these Tasks to avoid forcing customers to use the "UNSAFE" set when they are actually using no Async features (See Dev11 Bug #193253)
        private static Task<bool> s_trueTask = null;
        internal static Task<bool> TrueTask
        {
            get
            {
                if (s_trueTask == null)
                {
                    s_trueTask = Task.FromResult<bool>(true);
                }
                return s_trueTask;
            }
        }

        private static Task<bool> s_falseTask = null;
        internal static Task<bool> FalseTask
        {
            get
            {
                if (s_falseTask == null)
                {
                    s_falseTask = Task.FromResult<bool>(false);
                }
                return s_falseTask;
            }
        }

        // this method accepts BID format as an argument, this attribute allows FXCopBid rule to validate calls to it
        private static void TraceException(string trace, Exception e)
        {
            Debug.Assert(null != e, "TraceException: null Exception");
        }

        internal static void TraceExceptionAsReturnValue(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|THROW> '%ls'\n", e);
        }
        internal static void TraceExceptionForCapture(Exception e)
        {
            Debug.Assert(ADP.IsCatchableExceptionType(e), "Invalid exception type, should have been re-thrown!");
            TraceException("<comm.ADP.TraceException|ERR|CATCH> '%ls'\n", e);
        }
        internal static void TraceExceptionWithoutRethrow(Exception e)
        {
            Debug.Assert(ADP.IsCatchableExceptionType(e), "Invalid exception type, should have been re-thrown!");
            TraceException("<comm.ADP.TraceException|ERR|CATCH> '%ls'\n", e);
        }

        //
        // COM+ exceptions
        //
        internal static ArgumentException Argument(string error)
        {
            ArgumentException e = new ArgumentException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentException Argument(string error, Exception inner)
        {
            ArgumentException e = new ArgumentException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentException Argument(string error, string parameter)
        {
            ArgumentException e = new ArgumentException(error, parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentException Argument(string error, string parameter, Exception inner)
        {
            ArgumentException e = new ArgumentException(error, parameter, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentNullException ArgumentNull(string parameter)
        {
            ArgumentNullException e = new ArgumentNullException(parameter);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentNullException ArgumentNull(string parameter, string error)
        {
            ArgumentNullException e = new ArgumentNullException(parameter, error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName)
        {
            ArgumentOutOfRangeException e = new ArgumentOutOfRangeException(parameterName);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
        {
            ArgumentOutOfRangeException e = new ArgumentOutOfRangeException(parameterName, message);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName, object value)
        {
            ArgumentOutOfRangeException e = new ArgumentOutOfRangeException(parameterName, value, message);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        //static internal ConfigurationException Configuration(string message)
        //{
        //    ConfigurationException e = new ConfigurationErrorsException(message);
        //    TraceExceptionAsReturnValue(e);
        //    return e;
        //}
        //static internal ConfigurationException Configuration(string message, XmlNode node)
        //{
        //    ConfigurationException e = new ConfigurationErrorsException(message, node);
        //    TraceExceptionAsReturnValue(e);
        //    return e;
        //}
        internal static DataException Data(string message)
        {
            DataException e = new DataException(message);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static IndexOutOfRangeException IndexOutOfRange(int value)
        {
            IndexOutOfRangeException e = new IndexOutOfRangeException(value.ToString(CultureInfo.InvariantCulture));
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static IndexOutOfRangeException IndexOutOfRange(string error)
        {
            IndexOutOfRangeException e = new IndexOutOfRangeException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static IndexOutOfRangeException IndexOutOfRange()
        {
            IndexOutOfRangeException e = new IndexOutOfRangeException();
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static InvalidCastException InvalidCast(string error)
        {
            return InvalidCast(error, null);
        }
        internal static InvalidCastException InvalidCast(string error, Exception inner)
        {
            InvalidCastException e = new InvalidCastException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static InvalidOperationException InvalidOperation(string error)
        {
            InvalidOperationException e = new InvalidOperationException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static TimeoutException TimeoutException(string error)
        {
            TimeoutException e = new TimeoutException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static InvalidOperationException InvalidOperation(string error, Exception inner)
        {
            InvalidOperationException e = new InvalidOperationException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static NotImplementedException NotImplemented(string error)
        {
            NotImplementedException e = new NotImplementedException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static NotSupportedException NotSupported()
        {
            NotSupportedException e = new NotSupportedException();
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static NotSupportedException NotSupported(string error)
        {
            NotSupportedException e = new NotSupportedException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static OverflowException Overflow(string error)
        {
            return Overflow(error, null);
        }
        internal static OverflowException Overflow(string error, Exception inner)
        {
            OverflowException e = new OverflowException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static PlatformNotSupportedException PropertyNotSupported(string property)
        {
            PlatformNotSupportedException e = new PlatformNotSupportedException(Res.GetString(Res.ADP_PropertyNotSupported, property));
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static TypeLoadException TypeLoad(string error)
        {
            TypeLoadException e = new TypeLoadException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static InvalidCastException InvalidCast()
        {
            InvalidCastException e = new InvalidCastException();
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static IOException IO(string error)
        {
            IOException e = new IOException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static IOException IO(string error, Exception inner)
        {
            IOException e = new IOException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }
        internal static InvalidOperationException DataAdapter(string error)
        {
            return InvalidOperation(error);
        }
        internal static InvalidOperationException DataAdapter(string error, Exception inner)
        {
            return InvalidOperation(error, inner);
        }
        private static InvalidOperationException Provider(string error)
        {
            return InvalidOperation(error);
        }
        internal static ObjectDisposedException ObjectDisposed(object instance)
        {
            ObjectDisposedException e = new ObjectDisposedException(instance.GetType().Name);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static InvalidOperationException MethodCalledTwice(string method)
        {
            InvalidOperationException e = new InvalidOperationException(Res.GetString(Res.ADP_CalledTwice, method));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException IncorrectAsyncResult()
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_IncorrectAsyncResult), "AsyncResult");
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException SingleValuedProperty(string propertyName, string value)
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_SingleValuedProperty, propertyName, value));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException DoubleValuedProperty(string propertyName, string value1, string value2)
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_DoubleValuedProperty, propertyName, value1, value2));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidPrefixSuffix()
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_InvalidPrefixSuffix));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidMultipartName(string property, string value)
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_InvalidMultipartName, Res.GetString(property), value));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidMultipartNameIncorrectUsageOfQuotes(string property, string value)
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_InvalidMultipartNameQuoteUsage, Res.GetString(property), value));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException InvalidMultipartNameToManyParts(string property, string value, int limit)
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_InvalidMultipartNameToManyParts, Res.GetString(property), value, limit));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException BadParameterName(string parameterName)
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_BadParameterName, parameterName));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException MultipleReturnValue()
        {
            ArgumentException e = new ArgumentException(Res.GetString(Res.ADP_MultipleReturnValue));
            TraceExceptionAsReturnValue(e);
            return e;
        }

        //
        // Helper Functions
        //
        internal static void CheckArgumentLength(string value, string parameterName)
        {
            CheckArgumentNull(value, parameterName);
            if (0 == value.Length)
            {
                throw Argument(Res.GetString(Res.ADP_EmptyString, parameterName)); // MDAC 94859
            }
        }
        internal static void CheckArgumentLength(Array value, string parameterName)
        {
            CheckArgumentNull(value, parameterName);
            if (0 == value.Length)
            {
                throw Argument(Res.GetString(Res.ADP_EmptyArray, parameterName));
            }
        }
        internal static void CheckArgumentNull(object value, string parameterName)
        {
            if (null == value)
            {
                throw ArgumentNull(parameterName);
            }
        }


        // only StackOverflowException & ThreadAbortException are sealed classes
        private static readonly Type s_stackOverflowType = typeof(StackOverflowException);
        private static readonly Type s_outOfMemoryType = typeof(OutOfMemoryException);
        private static readonly Type s_threadAbortType = typeof(ThreadAbortException);
        private static readonly Type s_nullReferenceType = typeof(NullReferenceException);
        private static readonly Type s_accessViolationType = typeof(AccessViolationException);
        private static readonly Type s_securityType = typeof(SecurityException);

        internal static bool IsCatchableExceptionType(Exception e)
        {
            // a 'catchable' exception is defined by what it is not.
            Debug.Assert(e != null, "Unexpected null exception!");
            Type type = e.GetType();

            return ((type != s_stackOverflowType) &&
                     (type != s_outOfMemoryType) &&
                     (type != s_threadAbortType) &&
                     (type != s_nullReferenceType) &&
                     (type != s_accessViolationType) &&
                     !s_securityType.IsAssignableFrom(type));
        }

        internal static bool IsCatchableOrSecurityExceptionType(Exception e)
        {
            // a 'catchable' exception is defined by what it is not.
            // since IsCatchableExceptionType defined SecurityException as not 'catchable'
            // this method will return true for SecurityException has being catchable.

            // the other way to write this method is, but then SecurityException is checked twice
            // return ((e is SecurityException) || IsCatchableExceptionType(e));

            Debug.Assert(e != null, "Unexpected null exception!");
            Type type = e.GetType();

            return ((type != s_stackOverflowType) &&
                     (type != s_outOfMemoryType) &&
                     (type != s_threadAbortType) &&
                     (type != s_nullReferenceType) &&
                     (type != s_accessViolationType));
        }

        // Invalid Enumeration

        internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
        {
            return ADP.ArgumentOutOfRange(Res.GetString(Res.ADP_InvalidEnumerationValue, type.Name, value.ToString(System.Globalization.CultureInfo.InvariantCulture)), type.Name);
        }

        internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, string value, string method)
        {
            return ADP.ArgumentOutOfRange(Res.GetString(Res.ADP_NotSupportedEnumerationValue, type.Name, value, method), type.Name);
        }

        internal static ArgumentOutOfRangeException InvalidAcceptRejectRule(AcceptRejectRule value)
        {
#if DEBUG
            switch (value)
            {
                case AcceptRejectRule.None:
                case AcceptRejectRule.Cascade:
                    Debug.Assert(false, "valid AcceptRejectRule " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(AcceptRejectRule), (int)value);
        }
        // DbCommandBuilder.CatalogLocation
        internal static ArgumentOutOfRangeException InvalidCatalogLocation(CatalogLocation value)
        {
#if DEBUG
            switch (value)
            {
                case CatalogLocation.Start:
                case CatalogLocation.End:
                    Debug.Assert(false, "valid CatalogLocation " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(CatalogLocation), (int)value);
        }

        internal static ArgumentOutOfRangeException InvalidCommandBehavior(CommandBehavior value)
        {
#if DEBUG
            if ((0 <= (int)value) && ((int)value <= 0x3F))
            {
                Debug.Assert(false, "valid CommandType " + value.ToString());
            }
#endif
            return InvalidEnumerationValue(typeof(CommandBehavior), (int)value);
        }
        internal static void ValidateCommandBehavior(CommandBehavior value)
        {
            if (((int)value < 0) || (0x3F < (int)value))
            {
                throw InvalidCommandBehavior(value);
            }
        }
        internal static ArgumentException InvalidArgumentLength(string argumentName, int limit)
        {
            return Argument(Res.GetString(Res.ADP_InvalidArgumentLength, argumentName, limit));
        }

        internal static ArgumentException MustBeReadOnly(string argumentName)
        {
            return Argument(Res.GetString(Res.ADP_MustBeReadOnly, argumentName));
        }

        // IDbCommand.CommandType
        internal static ArgumentOutOfRangeException InvalidCommandType(CommandType value)
        {
#if DEBUG
            switch (value)
            {
                case CommandType.Text:
                case CommandType.StoredProcedure:
                case CommandType.TableDirect:
                    Debug.Assert(false, "valid CommandType " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(CommandType), (int)value);
        }

        internal static ArgumentOutOfRangeException InvalidConflictOptions(ConflictOption value)
        {
#if DEBUG
            switch (value)
            {
                case ConflictOption.CompareAllSearchableValues:
                case ConflictOption.CompareRowVersion:
                case ConflictOption.OverwriteChanges:
                    Debug.Assert(false, "valid ConflictOption " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(ConflictOption), (int)value);
        }

        // IDataAdapter.Update
        internal static ArgumentOutOfRangeException InvalidDataRowState(DataRowState value)
        {
#if DEBUG
            switch (value)
            {
                case DataRowState.Detached:
                case DataRowState.Unchanged:
                case DataRowState.Added:
                case DataRowState.Deleted:
                case DataRowState.Modified:
                    Debug.Assert(false, "valid DataRowState " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(DataRowState), (int)value);
        }

        // IDataParameter.SourceVersion
        internal static ArgumentOutOfRangeException InvalidDataRowVersion(DataRowVersion value)
        {
#if DEBUG
            switch (value)
            {
                case DataRowVersion.Default:
                case DataRowVersion.Current:
                case DataRowVersion.Original:
                case DataRowVersion.Proposed:
                    Debug.Assert(false, "valid DataRowVersion " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(DataRowVersion), (int)value);
        }

        // IDbConnection.BeginTransaction, OleDbTransaction.Begin
        internal static ArgumentOutOfRangeException InvalidIsolationLevel(IsolationLevel value)
        {
#if DEBUG
            switch (value)
            {
                case IsolationLevel.Unspecified:
                case IsolationLevel.Chaos:
                case IsolationLevel.ReadUncommitted:
                case IsolationLevel.ReadCommitted:
                case IsolationLevel.RepeatableRead:
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    Debug.Assert(false, "valid IsolationLevel " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(IsolationLevel), (int)value);
        }

        // DBDataPermissionAttribute.KeyRestrictionBehavior
        internal static ArgumentOutOfRangeException InvalidKeyRestrictionBehavior(KeyRestrictionBehavior value)
        {
#if DEBUG
            switch (value)
            {
                case KeyRestrictionBehavior.PreventUsage:
                case KeyRestrictionBehavior.AllowOnly:
                    Debug.Assert(false, "valid KeyRestrictionBehavior " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(KeyRestrictionBehavior), (int)value);
        }

        // IDataAdapter.FillLoadOption
        internal static ArgumentOutOfRangeException InvalidLoadOption(LoadOption value)
        {
#if DEBUG
            switch (value)
            {
                case LoadOption.OverwriteChanges:
                case LoadOption.PreserveChanges:
                case LoadOption.Upsert:
                    Debug.Assert(false, "valid LoadOption " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(LoadOption), (int)value);
        }

        // IDataAdapter.MissingMappingAction
        internal static ArgumentOutOfRangeException InvalidMissingMappingAction(MissingMappingAction value)
        {
#if DEBUG
            switch (value)
            {
                case MissingMappingAction.Passthrough:
                case MissingMappingAction.Ignore:
                case MissingMappingAction.Error:
                    Debug.Assert(false, "valid MissingMappingAction " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(MissingMappingAction), (int)value);
        }

        // IDataAdapter.MissingSchemaAction
        internal static ArgumentOutOfRangeException InvalidMissingSchemaAction(MissingSchemaAction value)
        {
#if DEBUG
            switch (value)
            {
                case MissingSchemaAction.Add:
                case MissingSchemaAction.Ignore:
                case MissingSchemaAction.Error:
                case MissingSchemaAction.AddWithKey:
                    Debug.Assert(false, "valid MissingSchemaAction " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(MissingSchemaAction), (int)value);
        }

        // IDataParameter.Direction
        internal static ArgumentOutOfRangeException InvalidParameterDirection(ParameterDirection value)
        {
#if DEBUG
            switch (value)
            {
                case ParameterDirection.Input:
                case ParameterDirection.Output:
                case ParameterDirection.InputOutput:
                case ParameterDirection.ReturnValue:
                    Debug.Assert(false, "valid ParameterDirection " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(ParameterDirection), (int)value);
        }

        internal static ArgumentOutOfRangeException InvalidPermissionState(PermissionState value)
        {
#if DEBUG
            switch (value)
            {
                case PermissionState.Unrestricted:
                case PermissionState.None:
                    Debug.Assert(false, "valid PermissionState " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(PermissionState), (int)value);
        }

        internal static ArgumentOutOfRangeException InvalidRule(Rule value)
        {
#if DEBUG
            switch (value)
            {
                case Rule.None:
                case Rule.Cascade:
                case Rule.SetNull:
                case Rule.SetDefault:
                    Debug.Assert(false, "valid Rule " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(Rule), (int)value);
        }

        // IDataAdapter.FillSchema
        internal static ArgumentOutOfRangeException InvalidSchemaType(SchemaType value)
        {
#if DEBUG
            switch (value)
            {
                case SchemaType.Source:
                case SchemaType.Mapped:
                    Debug.Assert(false, "valid SchemaType " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(SchemaType), (int)value);
        }

        // RowUpdatingEventArgs.StatementType
        internal static ArgumentOutOfRangeException InvalidStatementType(StatementType value)
        {
#if DEBUG
            switch (value)
            {
                case StatementType.Select:
                case StatementType.Insert:
                case StatementType.Update:
                case StatementType.Delete:
                case StatementType.Batch:
                    Debug.Assert(false, "valid StatementType " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(StatementType), (int)value);
        }

        // IDbCommand.UpdateRowSource
        internal static ArgumentOutOfRangeException InvalidUpdateRowSource(UpdateRowSource value)
        {
#if DEBUG
            switch (value)
            {
                case UpdateRowSource.None:
                case UpdateRowSource.OutputParameters:
                case UpdateRowSource.FirstReturnedRecord:
                case UpdateRowSource.Both:
                    Debug.Assert(false, "valid UpdateRowSource " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(UpdateRowSource), (int)value);
        }

        // RowUpdatingEventArgs.UpdateStatus
        internal static ArgumentOutOfRangeException InvalidUpdateStatus(UpdateStatus value)
        {
#if DEBUG
            switch (value)
            {
                case UpdateStatus.Continue:
                case UpdateStatus.ErrorsOccurred:
                case UpdateStatus.SkipAllRemainingRows:
                case UpdateStatus.SkipCurrentRow:
                    Debug.Assert(false, "valid UpdateStatus " + value.ToString());
                    break;
            }
#endif
            return InvalidEnumerationValue(typeof(UpdateStatus), (int)value);
        }

        internal static ArgumentOutOfRangeException NotSupportedCommandBehavior(CommandBehavior value, string method)
        {
            return NotSupportedEnumerationValue(typeof(CommandBehavior), value.ToString(), method);
        }

        internal static ArgumentOutOfRangeException NotSupportedStatementType(StatementType value, string method)
        {
            return NotSupportedEnumerationValue(typeof(StatementType), value.ToString(), method);
        }

        //        static internal ArgumentOutOfRangeException InvalidUserDefinedTypeSerializationFormat(Microsoft.SqlServer.Server.Format value)
        //        {
        //#if DEBUG
        //            switch (value)
        //            {
        //                case Microsoft.SqlServer.Server.Format.Unknown:
        //                case Microsoft.SqlServer.Server.Format.Native:
        //                case Microsoft.SqlServer.Server.Format.UserDefined:
        //                    Debug.Assert(false, "valid UserDefinedTypeSerializationFormat " + value.ToString());
        //                    break;
        //            }
        //#endif
        //            return InvalidEnumerationValue(typeof(Microsoft.SqlServer.Server.Format), (int)value);
        //        }

        //        static internal ArgumentOutOfRangeException NotSupportedUserDefinedTypeSerializationFormat(Microsoft.SqlServer.Server.Format value, string method)
        //        {
        //            return ADP.NotSupportedEnumerationValue(typeof(Microsoft.SqlServer.Server.Format), value.ToString(), method);
        //        }

        //
        // DbProviderFactories
        //
        internal static ArgumentException ConfigProviderNotFound()
        {
            return Argument(Res.GetString(Res.ConfigProviderNotFound));
        }
        internal static InvalidOperationException ConfigProviderInvalid()
        {
            return InvalidOperation(Res.GetString(Res.ConfigProviderInvalid));
        }
        //static internal ConfigurationException ConfigProviderNotInstalled()
        //{
        //    return Configuration(Res.GetString(Res.ConfigProviderNotInstalled));
        //}
        //static internal ConfigurationException ConfigProviderMissing()
        //{
        //    return Configuration(Res.GetString(Res.ConfigProviderMissing));
        //}

        //
        // DbProviderConfigurationHandler
        //
        //static internal ConfigurationException ConfigBaseNoChildNodes(XmlNode node)
        //{ // Res.Config_base_no_child_nodes
        //    return Configuration(Res.GetString(Res.ConfigBaseNoChildNodes), node);
        //}
        //static internal ConfigurationException ConfigBaseElementsOnly(XmlNode node)
        //{ // Res.Config_base_elements_only
        //    return Configuration(Res.GetString(Res.ConfigBaseElementsOnly), node);
        //}
        //static internal ConfigurationException ConfigUnrecognizedAttributes(XmlNode node)
        //{ // Res.Config_base_unrecognized_attribute
        //    return Configuration(Res.GetString(Res.ConfigUnrecognizedAttributes, node.Attributes[0].Name), node);
        //}
        //static internal ConfigurationException ConfigUnrecognizedElement(XmlNode node)
        //{ // Res.Config_base_unrecognized_element
        //    return Configuration(Res.GetString(Res.ConfigUnrecognizedElement), node);
        //}
        //static internal ConfigurationException ConfigSectionsUnique(string sectionName)
        //{ // Res.Res.ConfigSectionsUnique
        //    return Configuration(Res.GetString(Res.ConfigSectionsUnique, sectionName));
        //}
        //static internal ConfigurationException ConfigRequiredAttributeMissing(string name, XmlNode node)
        //{ // Res.Config_base_required_attribute_missing
        //    return Configuration(Res.GetString(Res.ConfigRequiredAttributeMissing, name), node);
        //}
        //static internal ConfigurationException ConfigRequiredAttributeEmpty(string name, XmlNode node)
        //{ // Res.Config_base_required_attribute_empty
        //    return Configuration(Res.GetString(Res.ConfigRequiredAttributeEmpty, name), node);
        //}

        //
        // DbConnectionOptions, DataAccess
        //
        internal static ArgumentException ConnectionStringSyntax(int index)
        {
            return Argument(Res.GetString(Res.ADP_ConnectionStringSyntax, index));
        }
        internal static ArgumentException KeywordNotSupported(string keyword)
        {
            return Argument(Res.GetString(Res.ADP_KeywordNotSupported, keyword));
        }
        /*
        static internal ArgumentException EmptyKeyValue(string keyword) { // MDAC 80715
            return Argument(Res.GetString(Res.ADP_EmptyKeyValue, keyword));
        }
        */
        internal static ArgumentException UdlFileError(Exception inner)
        {
            return Argument(Res.GetString(Res.ADP_UdlFileError), inner);
        }
        internal static ArgumentException InvalidUDL()
        {
            return Argument(Res.GetString(Res.ADP_InvalidUDL));
        }
        internal static InvalidOperationException InvalidDataDirectory()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidDataDirectory));
        }
        internal static ArgumentException InvalidKeyname(string parameterName)
        {
            return Argument(Res.GetString(Res.ADP_InvalidKey), parameterName);
        }
        internal static ArgumentException InvalidValue(string parameterName)
        {
            return Argument(Res.GetString(Res.ADP_InvalidValue), parameterName);
        }
        internal static ArgumentException InvalidMinMaxPoolSizeValues()
        {
            return ADP.Argument(Res.GetString(Res.ADP_InvalidMinMaxPoolSizeValues));
        }
        internal static ArgumentException ConvertFailed(Type fromType, Type toType, Exception innerException)
        {
            return ADP.Argument(Res.GetString(Res.SqlConvert_ConvertFailed, fromType.FullName, toType.FullName), innerException);
        }

        internal static InvalidOperationException InvalidMixedUsageOfSecureAndClearCredential()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfSecureAndClearCredential));
        }

        internal static ArgumentException InvalidMixedArgumentOfSecureAndClearCredential()
        {
            return ADP.Argument(Res.GetString(Res.ADP_InvalidMixedUsageOfSecureAndClearCredential));
        }

        internal static InvalidOperationException InvalidMixedUsageOfSecureCredentialAndIntegratedSecurity()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfSecureCredentialAndIntegratedSecurity));
        }

        internal static ArgumentException InvalidMixedArgumentOfSecureCredentialAndIntegratedSecurity()
        {
            return ADP.Argument(Res.GetString(Res.ADP_InvalidMixedUsageOfSecureCredentialAndIntegratedSecurity));
        }

        internal static InvalidOperationException InvalidMixedUsageOfSecureCredentialAndContextConnection()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfSecureCredentialAndContextConnection));
        }

        internal static ArgumentException InvalidMixedArgumentOfSecureCredentialAndContextConnection()
        {
            return ADP.Argument(Res.GetString(Res.ADP_InvalidMixedUsageOfSecureCredentialAndContextConnection));
        }

        internal static InvalidOperationException InvalidMixedUsageOfAccessTokenAndContextConnection()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfAccessTokenAndContextConnection));
        }

        internal static InvalidOperationException InvalidMixedUsageOfAccessTokenAndIntegratedSecurity()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfAccessTokenAndIntegratedSecurity));
        }

        internal static InvalidOperationException InvalidMixedUsageOfAccessTokenAndUserIDPassword()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfAccessTokenAndUserIDPassword));
        }

        internal static Exception InvalidMixedUsageOfAccessTokenAndCredential()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfAccessTokenAndCredential));
        }

        internal static Exception InvalidMixedUsageOfAccessTokenAndAuthentication()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfAccessTokenAndAuthentication));
        }

        internal static Exception InvalidMixedUsageOfCredentialAndAccessToken()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_InvalidMixedUsageOfCredentialAndAccessToken));
        }

        //
        // DbConnection
        //
        internal static InvalidOperationException NoConnectionString()
        {
            return InvalidOperation(Res.GetString(Res.ADP_NoConnectionString));
        }

        //static internal NotImplementedException MethodNotImplemented(string methodName)
        //{
        //    NotImplementedException e = new NotImplementedException(methodName);
        //    TraceExceptionAsReturnValue(e);
        //    return e;
        //}
        internal static Exception MethodNotImplemented([CallerMemberName] string methodName = "")
        {
            return System.NotImplemented.ByDesignWithMessage(methodName);
        }

        private static string ConnectionStateMsg(ConnectionState state)
        { // MDAC 82165, if the ConnectionState enum to msg the localization looks weird
            switch (state)
            {
                case (ConnectionState.Closed):
                case (ConnectionState.Connecting | ConnectionState.Broken): // treated the same as closed
                    return Res.GetString(Res.ADP_ConnectionStateMsg_Closed);
                case (ConnectionState.Connecting):
                    return Res.GetString(Res.ADP_ConnectionStateMsg_Connecting);
                case (ConnectionState.Open):
                    return Res.GetString(Res.ADP_ConnectionStateMsg_Open);
                case (ConnectionState.Open | ConnectionState.Executing):
                    return Res.GetString(Res.ADP_ConnectionStateMsg_OpenExecuting);
                case (ConnectionState.Open | ConnectionState.Fetching):
                    return Res.GetString(Res.ADP_ConnectionStateMsg_OpenFetching);
                default:
                    return Res.GetString(Res.ADP_ConnectionStateMsg, state.ToString());
            }
        }

        //static internal ConfigurationException ConfigUnableToLoadXmlMetaDataFile(string settingName)
        //{
        //    return Configuration(Res.GetString(Res.OleDb_ConfigUnableToLoadXmlMetaDataFile, settingName));
        //}

        //static internal ConfigurationException ConfigWrongNumberOfValues(string settingName)
        //{
        //    return Configuration(Res.GetString(Res.OleDb_ConfigWrongNumberOfValues, settingName));
        //}

        //
        // : DbConnectionOptions, DataAccess, SqlClient
        //
        internal static Exception InvalidConnectionOptionValue(string key)
        {
            return InvalidConnectionOptionValue(key, null);
        }
        internal static Exception InvalidConnectionOptionValueLength(string key, int limit)
        {
            return Argument(Res.GetString(Res.ADP_InvalidConnectionOptionValueLength, key, limit));
        }
        internal static Exception InvalidConnectionOptionValue(string key, Exception inner)
        {
            return Argument(Res.GetString(Res.ADP_InvalidConnectionOptionValue, key), inner);
        }
        internal static Exception MissingConnectionOptionValue(string key, string requiredAdditionalKey)
        {
            return Argument(Res.GetString(Res.ADP_MissingConnectionOptionValue, key, requiredAdditionalKey));
        }

        //
        // DBDataPermission, DataAccess, Odbc
        //
        internal static Exception InvalidXMLBadVersion()
        {
            return Argument(Res.GetString(Res.ADP_InvalidXMLBadVersion));
        }
        internal static Exception NotAPermissionElement()
        {
            return Argument(Res.GetString(Res.ADP_NotAPermissionElement));
        }
        internal static Exception PermissionTypeMismatch()
        {
            return Argument(Res.GetString(Res.ADP_PermissionTypeMismatch));
        }

        internal static Exception WrongType(Type got, Type expected)
        {
            return Argument(Res.GetString(Res.SQL_WrongType, got.ToString(), expected.ToString()));
        }

        internal static Exception OdbcNoTypesFromProvider()
        {
            return InvalidOperation(Res.GetString(Res.ADP_OdbcNoTypesFromProvider));
        }

        //
        // DbConnectionPool and related
        //
        internal static Exception PooledOpenTimeout()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_PooledOpenTimeout));
        }

        internal static Exception NonPooledOpenTimeout()
        {
            return ADP.TimeoutException(Res.GetString(Res.ADP_NonPooledOpenTimeout));
        }

        //
        // Generic Data Provider Collection
        //
        internal static ArgumentException CollectionRemoveInvalidObject(Type itemType, ICollection collection)
        {
            return Argument(Res.GetString(Res.ADP_CollectionRemoveInvalidObject, itemType.Name, collection.GetType().Name)); // MDAC 68201
        }
        internal static ArgumentNullException CollectionNullValue(string parameter, Type collection, Type itemType)
        {
            return ArgumentNull(parameter, Res.GetString(Res.ADP_CollectionNullValue, collection.Name, itemType.Name));
        }
        internal static IndexOutOfRangeException CollectionIndexInt32(int index, Type collection, int count)
        {
            return IndexOutOfRange(Res.GetString(Res.ADP_CollectionIndexInt32, index.ToString(CultureInfo.InvariantCulture), collection.Name, count.ToString(CultureInfo.InvariantCulture)));
        }
        internal static IndexOutOfRangeException CollectionIndexString(Type itemType, string propertyName, string propertyValue, Type collection)
        {
            return IndexOutOfRange(Res.GetString(Res.ADP_CollectionIndexString, itemType.Name, propertyName, propertyValue, collection.Name));
        }
        internal static InvalidCastException CollectionInvalidType(Type collection, Type itemType, object invalidValue)
        {
            return InvalidCast(Res.GetString(Res.ADP_CollectionInvalidType, collection.Name, itemType.Name, invalidValue.GetType().Name));
        }
        internal static Exception CollectionUniqueValue(Type itemType, string propertyName, string propertyValue)
        {
            return Argument(Res.GetString(Res.ADP_CollectionUniqueValue, itemType.Name, propertyName, propertyValue));
        }
        internal static ArgumentException ParametersIsNotParent(Type parameterType, ICollection collection)
        {
            return Argument(Res.GetString(Res.ADP_CollectionIsNotParent, parameterType.Name, collection.GetType().Name));
        }
        internal static ArgumentException ParametersIsParent(Type parameterType, ICollection collection)
        {
            return Argument(Res.GetString(Res.ADP_CollectionIsNotParent, parameterType.Name, collection.GetType().Name));
        }

        //
        // DbProviderException
        //
        internal static InvalidOperationException TransactionConnectionMismatch()
        {
            return Provider(Res.GetString(Res.ADP_TransactionConnectionMismatch));
        }
        internal static InvalidOperationException TransactionCompletedButNotDisposed()
        {
            return Provider(Res.GetString(Res.ADP_TransactionCompletedButNotDisposed));
        }
        internal static InvalidOperationException TransactionRequired(string method)
        {
            return Provider(Res.GetString(Res.ADP_TransactionRequired, method));
        }

        // IDbDataAdapter.Fill(Schema)
        internal static InvalidOperationException MissingSelectCommand(string method)
        {
            return Provider(Res.GetString(Res.ADP_MissingSelectCommand, method));
        }

        //
        // AdapterMappingException
        //
        private static InvalidOperationException DataMapping(string error)
        {
            return InvalidOperation(error);
        }

        // DataColumnMapping.GetDataColumnBySchemaAction
        internal static InvalidOperationException ColumnSchemaExpression(string srcColumn, string cacheColumn)
        {
            return DataMapping(Res.GetString(Res.ADP_ColumnSchemaExpression, srcColumn, cacheColumn));
        }

        // DataColumnMapping.GetDataColumnBySchemaAction
        internal static InvalidOperationException ColumnSchemaMismatch(string srcColumn, Type srcType, DataColumn column)
        {
            return DataMapping(Res.GetString(Res.ADP_ColumnSchemaMismatch, srcColumn, srcType.Name, column.ColumnName, column.DataType.Name));
        }

        // DataColumnMapping.GetDataColumnBySchemaAction
        internal static InvalidOperationException ColumnSchemaMissing(string cacheColumn, string tableName, string srcColumn)
        {
            if (ADP.IsEmpty(tableName))
            {
                return InvalidOperation(Res.GetString(Res.ADP_ColumnSchemaMissing1, cacheColumn, tableName, srcColumn));
            }
            return DataMapping(Res.GetString(Res.ADP_ColumnSchemaMissing2, cacheColumn, tableName, srcColumn));
        }

        // DataColumnMappingCollection.GetColumnMappingBySchemaAction
        internal static InvalidOperationException MissingColumnMapping(string srcColumn)
        {
            return DataMapping(Res.GetString(Res.ADP_MissingColumnMapping, srcColumn));
        }

        // DataTableMapping.GetDataTableBySchemaAction
        internal static InvalidOperationException MissingTableSchema(string cacheTable, string srcTable)
        {
            return DataMapping(Res.GetString(Res.ADP_MissingTableSchema, cacheTable, srcTable));
        }

        // DataTableMappingCollection.GetTableMappingBySchemaAction
        internal static InvalidOperationException MissingTableMapping(string srcTable)
        {
            return DataMapping(Res.GetString(Res.ADP_MissingTableMapping, srcTable));
        }

        // DbDataAdapter.Update
        internal static InvalidOperationException MissingTableMappingDestination(string dstTable)
        {
            return DataMapping(Res.GetString(Res.ADP_MissingTableMappingDestination, dstTable));
        }

        //
        // DataColumnMappingCollection, DataAccess
        //
        internal static Exception InvalidSourceColumn(string parameter)
        {
            return Argument(Res.GetString(Res.ADP_InvalidSourceColumn), parameter);
        }
        internal static Exception ColumnsAddNullAttempt(string parameter)
        {
            return CollectionNullValue(parameter, typeof(DataColumnMappingCollection), typeof(DataColumnMapping));
        }
        internal static Exception ColumnsDataSetColumn(string cacheColumn)
        {
            return CollectionIndexString(typeof(DataColumnMapping), ADP.DataSetColumn, cacheColumn, typeof(DataColumnMappingCollection));
        }
        internal static Exception ColumnsIndexInt32(int index, IColumnMappingCollection collection)
        {
            return CollectionIndexInt32(index, collection.GetType(), collection.Count);
        }
        internal static Exception ColumnsIndexSource(string srcColumn)
        {
            return CollectionIndexString(typeof(DataColumnMapping), ADP.SourceColumn, srcColumn, typeof(DataColumnMappingCollection));
        }
        internal static Exception ColumnsIsNotParent(ICollection collection)
        {
            return ParametersIsNotParent(typeof(DataColumnMapping), collection);
        }
        internal static Exception ColumnsIsParent(ICollection collection)
        {
            return ParametersIsParent(typeof(DataColumnMapping), collection);
        }
        internal static Exception ColumnsUniqueSourceColumn(string srcColumn)
        {
            return CollectionUniqueValue(typeof(DataColumnMapping), ADP.SourceColumn, srcColumn);
        }
        internal static Exception NotADataColumnMapping(object value)
        {
            return CollectionInvalidType(typeof(DataColumnMappingCollection), typeof(DataColumnMapping), value);
        }

        //
        // DataTableMappingCollection, DataAccess
        //
        internal static Exception InvalidSourceTable(string parameter)
        {
            return Argument(Res.GetString(Res.ADP_InvalidSourceTable), parameter);
        }
        internal static Exception TablesAddNullAttempt(string parameter)
        {
            return CollectionNullValue(parameter, typeof(DataTableMappingCollection), typeof(DataTableMapping));
        }
        internal static Exception TablesDataSetTable(string cacheTable)
        {
            return CollectionIndexString(typeof(DataTableMapping), ADP.DataSetTable, cacheTable, typeof(DataTableMappingCollection));
        }
        internal static Exception TablesIndexInt32(int index, ITableMappingCollection collection)
        {
            return CollectionIndexInt32(index, collection.GetType(), collection.Count);
        }
        internal static Exception TablesIsNotParent(ICollection collection)
        {
            return ParametersIsNotParent(typeof(DataTableMapping), collection);
        }
        internal static Exception TablesIsParent(ICollection collection)
        {
            return ParametersIsParent(typeof(DataTableMapping), collection);
        }
        internal static Exception TablesSourceIndex(string srcTable)
        {
            return CollectionIndexString(typeof(DataTableMapping), ADP.SourceTable, srcTable, typeof(DataTableMappingCollection));
        }
        internal static Exception TablesUniqueSourceTable(string srcTable)
        {
            return CollectionUniqueValue(typeof(DataTableMapping), ADP.SourceTable, srcTable);
        }
        internal static Exception NotADataTableMapping(object value)
        {
            return CollectionInvalidType(typeof(DataTableMappingCollection), typeof(DataTableMapping), value);
        }

        //
        // IDbCommand
        //

        internal static InvalidOperationException CommandAsyncOperationCompleted()
        {
            return InvalidOperation(Res.GetString(Res.SQL_AsyncOperationCompleted));
        }

        internal static Exception CommandTextRequired(string method)
        {
            return InvalidOperation(Res.GetString(Res.ADP_CommandTextRequired, method));
        }

        internal static InvalidOperationException ConnectionRequired(string method)
        {
            return InvalidOperation(Res.GetString(Res.ADP_ConnectionRequired, method));
        }
        internal static InvalidOperationException OpenConnectionRequired(string method, ConnectionState state)
        {
            return InvalidOperation(Res.GetString(Res.ADP_OpenConnectionRequired, method, ADP.ConnectionStateMsg(state)));
        }


        internal static InvalidOperationException UpdateConnectionRequired(StatementType statementType, bool isRowUpdatingCommand)
        {
            string resource;
            if (isRowUpdatingCommand)
            {
                resource = Res.ADP_ConnectionRequired_Clone;
            }
            else
            {
                switch (statementType)
                {
                    case StatementType.Insert:
                        resource = Res.ADP_ConnectionRequired_Insert;
                        break;
                    case StatementType.Update:
                        resource = Res.ADP_ConnectionRequired_Update;
                        break;
                    case StatementType.Delete:
                        resource = Res.ADP_ConnectionRequired_Delete;
                        break;
                    case StatementType.Batch:
                        resource = Res.ADP_ConnectionRequired_Batch;
                        goto default;
#if DEBUG
                    case StatementType.Select:
                        Debug.Assert(false, "shouldn't be here");
                        goto default;
#endif
                    default:
                        throw ADP.InvalidStatementType(statementType);
                }
            }
            return InvalidOperation(Res.GetString(resource));
        }

        internal static InvalidOperationException ConnectionRequired_Res(string method)
        {
            string resource = "ADP_ConnectionRequired_" + method;
#if DEBUG
            switch (resource)
            {
                case Res.ADP_ConnectionRequired_Fill:
                case Res.ADP_ConnectionRequired_FillPage:
                case Res.ADP_ConnectionRequired_FillSchema:
                case Res.ADP_ConnectionRequired_Update:
                case Res.ADP_ConnecitonRequired_UpdateRows:
                    break;
                default:
                    Debug.Assert(false, "missing resource string: " + resource);
                    break;
            }
#endif
            return InvalidOperation(Res.GetString(resource));
        }
        internal static InvalidOperationException UpdateOpenConnectionRequired(StatementType statementType, bool isRowUpdatingCommand, ConnectionState state)
        {
            string resource;
            if (isRowUpdatingCommand)
            {
                resource = Res.ADP_OpenConnectionRequired_Clone;
            }
            else
            {
                switch (statementType)
                {
                    case StatementType.Insert:
                        resource = Res.ADP_OpenConnectionRequired_Insert;
                        break;
                    case StatementType.Update:
                        resource = Res.ADP_OpenConnectionRequired_Update;
                        break;
                    case StatementType.Delete:
                        resource = Res.ADP_OpenConnectionRequired_Delete;
                        break;
#if DEBUG
                    case StatementType.Select:
                        Debug.Assert(false, "shouldn't be here");
                        goto default;
                    case StatementType.Batch:
                        Debug.Assert(false, "isRowUpdatingCommand should have been true");
                        goto default;
#endif
                    default:
                        throw ADP.InvalidStatementType(statementType);
                }
            }
            return InvalidOperation(Res.GetString(resource, ADP.ConnectionStateMsg(state)));
        }

        internal static Exception NoStoredProcedureExists(string sproc)
        {
            return InvalidOperation(Res.GetString(Res.ADP_NoStoredProcedureExists, sproc));
        }
        internal static Exception OpenReaderExists()
        {
            return OpenReaderExists(null);
        }

        internal static Exception OpenReaderExists(Exception e)
        {
            return InvalidOperation(Res.GetString(Res.ADP_OpenReaderExists), e);
        }

        internal static Exception TransactionCompleted()
        {
            return DataAdapter(Res.GetString(Res.ADP_TransactionCompleted));
        }

        //
        // DbDataReader
        //
        internal static Exception NonSeqByteAccess(long badIndex, long currIndex, string method)
        {
            return InvalidOperation(Res.GetString(Res.ADP_NonSeqByteAccess, badIndex.ToString(CultureInfo.InvariantCulture), currIndex.ToString(CultureInfo.InvariantCulture), method));
        }

        internal static Exception NegativeParameter(string parameterName)
        {
            return InvalidOperation(Res.GetString(Res.ADP_NegativeParameter, parameterName));
        }

        internal static Exception NumericToDecimalOverflow()
        {
            return InvalidCast(Res.GetString(Res.ADP_NumericToDecimalOverflow));
        }

        //
        // Stream, SqlTypes, SqlClient
        //

        internal static Exception ExceedsMaxDataLength(long specifiedLength, long maxLength)
        {
            return IndexOutOfRange(Res.GetString(Res.SQL_ExceedsMaxDataLength, specifiedLength.ToString(CultureInfo.InvariantCulture), maxLength.ToString(CultureInfo.InvariantCulture)));
        }

        internal static Exception InvalidSeekOrigin(string parameterName)
        {
            return ArgumentOutOfRange(Res.GetString(Res.ADP_InvalidSeekOrigin), parameterName);
        }

        //
        // SqlMetaData, SqlTypes, SqlClient
        //
        internal static Exception InvalidImplicitConversion(Type fromtype, string totype)
        {
            return InvalidCast(Res.GetString(Res.ADP_InvalidImplicitConversion, fromtype.Name, totype));
        }
        internal static Exception InvalidMetaDataValue()
        {
            return ADP.Argument(Res.GetString(Res.ADP_InvalidMetaDataValue));
        }

        internal static Exception NotRowType()
        {
            return InvalidOperation(Res.GetString(Res.ADP_NotRowType));
        }

        //
        // DbDataAdapter
        //
        internal static ArgumentException UnwantedStatementType(StatementType statementType)
        {
            return Argument(Res.GetString(Res.ADP_UnwantedStatementType, statementType.ToString()));
        }
        internal static InvalidOperationException NonSequentialColumnAccess(int badCol, int currCol)
        {
            return InvalidOperation(Res.GetString(Res.ADP_NonSequentialColumnAccess, badCol.ToString(CultureInfo.InvariantCulture), currCol.ToString(CultureInfo.InvariantCulture)));
        }

        //
        // DbDataAdapter.FillSchema
        //
        internal static Exception FillSchemaRequiresSourceTableName(string parameter)
        {
            return Argument(Res.GetString(Res.ADP_FillSchemaRequiresSourceTableName), parameter);
        }

        //
        // DbDataAdapter.Fill
        //
        internal static Exception InvalidMaxRecords(string parameter, int max)
        {
            return Argument(Res.GetString(Res.ADP_InvalidMaxRecords, max.ToString(CultureInfo.InvariantCulture)), parameter);
        }
        internal static Exception InvalidStartRecord(string parameter, int start)
        {
            return Argument(Res.GetString(Res.ADP_InvalidStartRecord, start.ToString(CultureInfo.InvariantCulture)), parameter);
        }
        internal static Exception FillRequires(string parameter)
        {
            return ArgumentNull(parameter);
        }
        internal static Exception FillRequiresSourceTableName(string parameter)
        {
            return Argument(Res.GetString(Res.ADP_FillRequiresSourceTableName), parameter);
        }
        internal static Exception FillChapterAutoIncrement()
        {
            return InvalidOperation(Res.GetString(Res.ADP_FillChapterAutoIncrement));
        }
        internal static InvalidOperationException MissingDataReaderFieldType(int index)
        {
            return DataAdapter(Res.GetString(Res.ADP_MissingDataReaderFieldType, index));
        }
        internal static InvalidOperationException OnlyOneTableForStartRecordOrMaxRecords()
        {
            return DataAdapter(Res.GetString(Res.ADP_OnlyOneTableForStartRecordOrMaxRecords));
        }
        //
        // DbDataAdapter.Update
        //
        internal static ArgumentNullException UpdateRequiresNonNullDataSet(string parameter)
        {
            return ArgumentNull(parameter);
        }
        internal static InvalidOperationException UpdateRequiresSourceTable(string defaultSrcTableName)
        {
            return InvalidOperation(Res.GetString(Res.ADP_UpdateRequiresSourceTable, defaultSrcTableName));
        }
        internal static InvalidOperationException UpdateRequiresSourceTableName(string srcTable)
        {
            return InvalidOperation(Res.GetString(Res.ADP_UpdateRequiresSourceTableName, srcTable)); // MDAC 70448
        }
        internal static ArgumentNullException UpdateRequiresDataTable(string parameter)
        {
            return ArgumentNull(parameter);
        }

        internal static Exception UpdateConcurrencyViolation(StatementType statementType, int affected, int expected, DataRow[] dataRows)
        {
            string resource;
            switch (statementType)
            {
                case StatementType.Update:
                    resource = Res.ADP_UpdateConcurrencyViolation_Update;
                    break;
                case StatementType.Delete:
                    resource = Res.ADP_UpdateConcurrencyViolation_Delete;
                    break;
                case StatementType.Batch:
                    resource = Res.ADP_UpdateConcurrencyViolation_Batch;
                    break;
#if DEBUG
                case StatementType.Select:
                case StatementType.Insert:
                    Debug.Assert(false, "should be here");
                    goto default;
#endif
                default:
                    throw ADP.InvalidStatementType(statementType);
            }
            DBConcurrencyException exception = new DBConcurrencyException(Res.GetString(resource, affected.ToString(CultureInfo.InvariantCulture), expected.ToString(CultureInfo.InvariantCulture)), null, dataRows);
            TraceExceptionAsReturnValue(exception);
            return exception;
        }

        internal static InvalidOperationException UpdateRequiresCommand(StatementType statementType, bool isRowUpdatingCommand)
        {
            string resource;
            if (isRowUpdatingCommand)
            {
                resource = Res.ADP_UpdateRequiresCommandClone;
            }
            else
            {
                switch (statementType)
                {
                    case StatementType.Select:
                        resource = Res.ADP_UpdateRequiresCommandSelect;
                        break;
                    case StatementType.Insert:
                        resource = Res.ADP_UpdateRequiresCommandInsert;
                        break;
                    case StatementType.Update:
                        resource = Res.ADP_UpdateRequiresCommandUpdate;
                        break;
                    case StatementType.Delete:
                        resource = Res.ADP_UpdateRequiresCommandDelete;
                        break;
#if DEBUG
                    case StatementType.Batch:
                        Debug.Assert(false, "isRowUpdatingCommand should have been true");
                        goto default;
#endif
                    default:
                        throw ADP.InvalidStatementType(statementType);
                }
            }
            return InvalidOperation(Res.GetString(resource));
        }
        internal static ArgumentException UpdateMismatchRowTable(int i)
        {
            return Argument(Res.GetString(Res.ADP_UpdateMismatchRowTable, i.ToString(CultureInfo.InvariantCulture)));
        }
        internal static DataException RowUpdatedErrors()
        {
            return Data(Res.GetString(Res.ADP_RowUpdatedErrors));
        }
        internal static DataException RowUpdatingErrors()
        {
            return Data(Res.GetString(Res.ADP_RowUpdatingErrors));
        }
        internal static InvalidOperationException ResultsNotAllowedDuringBatch()
        {
            return DataAdapter(Res.GetString(Res.ADP_ResultsNotAllowedDuringBatch));
        }

        //
        // : IDbCommand
        //
        internal static Exception InvalidCommandTimeout(int value)
        {
            return Argument(Res.GetString(Res.ADP_InvalidCommandTimeout, value.ToString(CultureInfo.InvariantCulture)), ADP.CommandTimeout);
        }
        internal static Exception DeriveParametersNotSupported(IDbCommand value)
        {
            return DataAdapter(Res.GetString(Res.ADP_DeriveParametersNotSupported, value.GetType().Name, value.CommandType.ToString()));
        }
        internal static Exception UninitializedParameterSize(int index, Type dataType)
        {
            return InvalidOperation(Res.GetString(Res.ADP_UninitializedParameterSize, index.ToString(CultureInfo.InvariantCulture), dataType.Name));
        }
        internal static Exception PrepareParameterType(IDbCommand cmd)
        {
            return InvalidOperation(Res.GetString(Res.ADP_PrepareParameterType, cmd.GetType().Name));
        }
        internal static Exception PrepareParameterSize(IDbCommand cmd)
        {
            return InvalidOperation(Res.GetString(Res.ADP_PrepareParameterSize, cmd.GetType().Name));
        }
        internal static Exception PrepareParameterScale(IDbCommand cmd, string type)
        {
            return InvalidOperation(Res.GetString(Res.ADP_PrepareParameterScale, cmd.GetType().Name, type));
        }
        internal static Exception MismatchedAsyncResult(string expectedMethod, string gotMethod)
        {
            return InvalidOperation(Res.GetString(Res.ADP_MismatchedAsyncResult, expectedMethod, gotMethod));
        }

        //
        // : ConnectionUtil
        //
        internal static Exception ConnectionIsDisabled(Exception InnerException)
        {
            return InvalidOperation(Res.GetString(Res.ADP_ConnectionIsDisabled), InnerException);
        }
        internal static Exception ClosedConnectionError()
        {
            return InvalidOperation(Res.GetString(Res.ADP_ClosedConnectionError));
        }
        internal static Exception ConnectionAlreadyOpen(ConnectionState state)
        {
            return InvalidOperation(Res.GetString(Res.ADP_ConnectionAlreadyOpen, ADP.ConnectionStateMsg(state)));
        }
        internal static Exception DelegatedTransactionPresent()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DelegatedTransactionPresent));
        }
        internal static Exception TransactionPresent()
        {
            return InvalidOperation(Res.GetString(Res.ADP_TransactionPresent));
        }
        internal static Exception LocalTransactionPresent()
        {
            return InvalidOperation(Res.GetString(Res.ADP_LocalTransactionPresent));
        }
        internal static Exception OpenConnectionPropertySet(string property, ConnectionState state)
        {
            return InvalidOperation(Res.GetString(Res.ADP_OpenConnectionPropertySet, property, ADP.ConnectionStateMsg(state)));
        }
        internal static Exception EmptyDatabaseName()
        {
            return Argument(Res.GetString(Res.ADP_EmptyDatabaseName));
        }
        internal static Exception DatabaseNameTooLong()
        {
            return Argument(Res.GetString(Res.ADP_DatabaseNameTooLong));
        }

        internal enum ConnectionError
        {
            BeginGetConnectionReturnsNull,
            GetConnectionReturnsNull,
            ConnectionOptionsMissing,
            CouldNotSwitchToClosedPreviouslyOpenedState,
        }
        internal static Exception InternalConnectionError(ConnectionError internalError)
        {
            return InvalidOperation(Res.GetString(Res.ADP_InternalConnectionError, (int)internalError));
        }

        internal enum InternalErrorCode
        {
            UnpooledObjectHasOwner = 0,
            UnpooledObjectHasWrongOwner = 1,
            PushingObjectSecondTime = 2,
            PooledObjectHasOwner = 3,
            PooledObjectInPoolMoreThanOnce = 4,
            CreateObjectReturnedNull = 5,
            NewObjectCannotBePooled = 6,
            NonPooledObjectUsedMoreThanOnce = 7,
            AttemptingToPoolOnRestrictedToken = 8,
            //          ConnectionOptionsInUse                                  =  9,
            ConvertSidToStringSidWReturnedNull = 10,
            //          UnexpectedTransactedObject                              = 11,
            AttemptingToConstructReferenceCollectionOnStaticObject = 12,
            AttemptingToEnlistTwice = 13,
            CreateReferenceCollectionReturnedNull = 14,
            PooledObjectWithoutPool = 15,
            UnexpectedWaitAnyResult = 16,
            SynchronousConnectReturnedPending = 17,
            CompletedConnectReturnedPending = 18,

            NameValuePairNext = 20,
            InvalidParserState1 = 21,
            InvalidParserState2 = 22,
            InvalidParserState3 = 23,

            InvalidBuffer = 30,

            UnimplementedSMIMethod = 40,
            InvalidSmiCall = 41,

            SqlDependencyObtainProcessDispatcherFailureObjectHandle = 50,
            SqlDependencyProcessDispatcherFailureCreateInstance = 51,
            SqlDependencyProcessDispatcherFailureAppDomain = 52,
            SqlDependencyCommandHashIsNotAssociatedWithNotification = 53,

            UnknownTransactionFailure = 60,
        }
        internal static Exception InternalError(InternalErrorCode internalError)
        {
            return InvalidOperation(Res.GetString(Res.ADP_InternalProviderError, (int)internalError));
        }
        internal static Exception InternalError(InternalErrorCode internalError, Exception innerException)
        {
            return InvalidOperation(Res.GetString(Res.ADP_InternalProviderError, (int)internalError), innerException);
        }
        internal static Exception InvalidConnectTimeoutValue()
        {
            return Argument(Res.GetString(Res.ADP_InvalidConnectTimeoutValue));
        }

        internal static Exception InvalidConnectRetryCountValue()
        {
            return Argument(Res.GetString(Res.SQLCR_InvalidConnectRetryCountValue));
        }

        internal static Exception InvalidConnectRetryIntervalValue()
        {
            return Argument(Res.GetString(Res.SQLCR_InvalidConnectRetryIntervalValue));
        }

        //
        // : DbDataReader
        //
        internal static Exception DataReaderNoData()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DataReaderNoData));
        }
        internal static Exception DataReaderClosed(string method)
        {
            return InvalidOperation(Res.GetString(Res.ADP_DataReaderClosed, method));
        }
        internal static ArgumentOutOfRangeException InvalidSourceBufferIndex(int maxLen, long srcOffset, string parameterName)
        {
            return ArgumentOutOfRange(Res.GetString(Res.ADP_InvalidSourceBufferIndex, maxLen.ToString(CultureInfo.InvariantCulture), srcOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
        }
        internal static ArgumentOutOfRangeException InvalidDestinationBufferIndex(int maxLen, int dstOffset, string parameterName)
        {
            return ArgumentOutOfRange(Res.GetString(Res.ADP_InvalidDestinationBufferIndex, maxLen.ToString(CultureInfo.InvariantCulture), dstOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
        }
        internal static IndexOutOfRangeException InvalidBufferSizeOrIndex(int numBytes, int bufferIndex)
        {
            return IndexOutOfRange(Res.GetString(Res.SQL_InvalidBufferSizeOrIndex, numBytes.ToString(CultureInfo.InvariantCulture), bufferIndex.ToString(CultureInfo.InvariantCulture)));
        }
        internal static Exception InvalidDataLength(long length)
        {
            return IndexOutOfRange(Res.GetString(Res.SQL_InvalidDataLength, length.ToString(CultureInfo.InvariantCulture)));
        }
        internal static InvalidOperationException AsyncOperationPending()
        {
            return InvalidOperation(Res.GetString(Res.ADP_PendingAsyncOperation));
        }

        //
        // : Stream
        //
        internal static Exception StreamClosed(string method)
        {
            return InvalidOperation(Res.GetString(Res.ADP_StreamClosed, method));
        }
        internal static IOException ErrorReadingFromStream(Exception internalException)
        {
            return IO(Res.GetString(Res.SqlMisc_StreamErrorMessage), internalException);
        }

        //
        // : DbDataAdapter
        //
        internal static InvalidOperationException DynamicSQLJoinUnsupported()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLJoinUnsupported));
        }
        internal static InvalidOperationException DynamicSQLNoTableInfo()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLNoTableInfo));
        }
        internal static InvalidOperationException DynamicSQLNoKeyInfoDelete()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLNoKeyInfoDelete));
        }
        internal static InvalidOperationException DynamicSQLNoKeyInfoUpdate()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLNoKeyInfoUpdate));
        }
        internal static InvalidOperationException DynamicSQLNoKeyInfoRowVersionDelete()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLNoKeyInfoRowVersionDelete));
        }
        internal static InvalidOperationException DynamicSQLNoKeyInfoRowVersionUpdate()
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLNoKeyInfoRowVersionUpdate));
        }
        internal static InvalidOperationException DynamicSQLNestedQuote(string name, string quote)
        {
            return InvalidOperation(Res.GetString(Res.ADP_DynamicSQLNestedQuote, name, quote));
        }
        internal static InvalidOperationException NoQuoteChange()
        {
            return InvalidOperation(Res.GetString(Res.ADP_NoQuoteChange));
        }
        internal static InvalidOperationException ComputerNameEx(int lastError)
        {
            return InvalidOperation(Res.GetString(Res.ADP_ComputerNameEx, lastError));
        }
        internal static InvalidOperationException MissingSourceCommand()
        {
            return InvalidOperation(Res.GetString(Res.ADP_MissingSourceCommand));
        }
        internal static InvalidOperationException MissingSourceCommandConnection()
        {
            return InvalidOperation(Res.GetString(Res.ADP_MissingSourceCommandConnection));
        }

        //
        // : IDataParameter
        //
        internal static ArgumentException InvalidDataType(TypeCode typecode)
        {
            return Argument(Res.GetString(Res.ADP_InvalidDataType, typecode.ToString()));
        }
        internal static ArgumentException UnknownDataType(Type dataType)
        {
            return Argument(Res.GetString(Res.ADP_UnknownDataType, dataType.FullName));
        }
        internal static ArgumentException DbTypeNotSupported(System.Data.DbType type, Type enumtype)
        {
            return Argument(Res.GetString(Res.ADP_DbTypeNotSupported, type.ToString(), enumtype.Name));
        }
        internal static ArgumentException UnknownDataTypeCode(Type dataType, TypeCode typeCode)
        {
            return Argument(Res.GetString(Res.ADP_UnknownDataTypeCode, ((int)typeCode).ToString(CultureInfo.InvariantCulture), dataType.FullName));
        }
        internal static ArgumentException InvalidOffsetValue(int value)
        {
            return Argument(Res.GetString(Res.ADP_InvalidOffsetValue, value.ToString(CultureInfo.InvariantCulture)));
        }
        internal static ArgumentException InvalidSizeValue(int value)
        {
            return Argument(Res.GetString(Res.ADP_InvalidSizeValue, value.ToString(CultureInfo.InvariantCulture)));
        }
        internal static ArgumentException ParameterValueOutOfRange(Decimal value)
        {
            return ADP.Argument(Res.GetString(Res.ADP_ParameterValueOutOfRange, value.ToString((IFormatProvider)null)));
        }
        internal static ArgumentException ParameterValueOutOfRange(SqlDecimal value)
        {
            return ADP.Argument(Res.GetString(Res.ADP_ParameterValueOutOfRange, value.ToString()));
        }

        internal static ArgumentException ParameterValueOutOfRange(String value)
        {
            return ADP.Argument(Res.GetString(Res.ADP_ParameterValueOutOfRange, value));
        }

        internal static ArgumentException VersionDoesNotSupportDataType(string typeName)
        {
            return Argument(Res.GetString(Res.ADP_VersionDoesNotSupportDataType, typeName));
        }
        internal static Exception ParameterConversionFailed(object value, Type destType, Exception inner)
        { // WebData 75433
            Debug.Assert(null != value, "null value on conversion failure");
            Debug.Assert(null != inner, "null inner on conversion failure");

            Exception e;
            string message = Res.GetString(Res.ADP_ParameterConversionFailed, value.GetType().Name, destType.Name);
            if (inner is ArgumentException)
            {
                e = new ArgumentException(message, inner);
            }
            else if (inner is FormatException)
            {
                e = new FormatException(message, inner);
            }
            else if (inner is InvalidCastException)
            {
                e = new InvalidCastException(message, inner);
            }
            else if (inner is OverflowException)
            {
                e = new OverflowException(message, inner);
            }
            else
            {
                e = inner;
            }
            TraceExceptionAsReturnValue(e);
            return e;
        }

        //
        // : IDataParameterCollection
        //
        internal static Exception ParametersMappingIndex(int index, IDataParameterCollection collection)
        {
            return CollectionIndexInt32(index, collection.GetType(), collection.Count);
        }
        internal static Exception ParametersSourceIndex(string parameterName, IDataParameterCollection collection, Type parameterType)
        {
            return CollectionIndexString(parameterType, ADP.ParameterName, parameterName, collection.GetType());
        }
        internal static Exception ParameterNull(string parameter, IDataParameterCollection collection, Type parameterType)
        {
            return CollectionNullValue(parameter, collection.GetType(), parameterType);
        }
        internal static Exception InvalidParameterType(IDataParameterCollection collection, Type parameterType, object invalidValue)
        {
            return CollectionInvalidType(collection.GetType(), parameterType, invalidValue);
        }

        //
        // : IDbTransaction
        //
        internal static Exception ParallelTransactionsNotSupported(IDbConnection obj)
        {
            return InvalidOperation(Res.GetString(Res.ADP_ParallelTransactionsNotSupported, obj.GetType().Name));
        }
        internal static Exception TransactionZombied(IDbTransaction obj)
        {
            return InvalidOperation(Res.GetString(Res.ADP_TransactionZombied, obj.GetType().Name));
        }

        internal static Exception DbRecordReadOnly(string methodname)
        {
            return InvalidOperation(Res.GetString(Res.ADP_DbRecordReadOnly, methodname));
        }

        internal static Exception OffsetOutOfRangeException()
        {
            return InvalidOperation(Res.GetString(Res.ADP_OffsetOutOfRangeException));
        }

        //
        // : DbMetaDataFactory
        //

        internal static Exception AmbigousCollectionName(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_AmbigousCollectionName, collectionName));
        }

        internal static Exception CollectionNameIsNotUnique(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_CollectionNameISNotUnique, collectionName));
        }

        internal static Exception DataTableDoesNotExist(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_DataTableDoesNotExist, collectionName));
        }

        internal static Exception IncorrectNumberOfDataSourceInformationRows()
        {
            return Argument(Res.GetString(Res.MDF_IncorrectNumberOfDataSourceInformationRows));
        }

        internal static ArgumentException InvalidRestrictionValue(string collectionName, string restrictionName, string restrictionValue)
        {
            return ADP.Argument(Res.GetString(Res.MDF_InvalidRestrictionValue, collectionName, restrictionName, restrictionValue));
        }

        internal static Exception InvalidXml()
        {
            return Argument(Res.GetString(Res.MDF_InvalidXml));
        }

        internal static Exception InvalidXmlMissingColumn(string collectionName, string columnName)
        {
            return Argument(Res.GetString(Res.MDF_InvalidXmlMissingColumn, collectionName, columnName));
        }

        internal static Exception InvalidXmlInvalidValue(string collectionName, string columnName)
        {
            return Argument(Res.GetString(Res.MDF_InvalidXmlInvalidValue, collectionName, columnName));
        }

        internal static Exception MissingDataSourceInformationColumn()
        {
            return Argument(Res.GetString(Res.MDF_MissingDataSourceInformationColumn));
        }

        internal static Exception MissingRestrictionColumn()
        {
            return Argument(Res.GetString(Res.MDF_MissingRestrictionColumn));
        }

        internal static Exception MissingRestrictionRow()
        {
            return Argument(Res.GetString(Res.MDF_MissingRestrictionRow));
        }

        internal static Exception NoColumns()
        {
            return Argument(Res.GetString(Res.MDF_NoColumns));
        }

        internal static Exception QueryFailed(string collectionName, Exception e)
        {
            return InvalidOperation(Res.GetString(Res.MDF_QueryFailed, collectionName), e);
        }

        internal static Exception TooManyRestrictions(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_TooManyRestrictions, collectionName));
        }

        internal static Exception UnableToBuildCollection(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_UnableToBuildCollection, collectionName));
        }

        internal static Exception UndefinedCollection(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_UndefinedCollection, collectionName));
        }

        internal static Exception UndefinedPopulationMechanism(string populationMechanism)
        {
            return Argument(Res.GetString(Res.MDF_UndefinedPopulationMechanism, populationMechanism));
        }

        internal static Exception UnsupportedVersion(string collectionName)
        {
            return Argument(Res.GetString(Res.MDF_UnsupportedVersion, collectionName));
        }


        //
        // : CommandBuilder
        //

        internal static InvalidOperationException InvalidDateTimeDigits(string dataTypeName)
        {
            return InvalidOperation(Res.GetString(Res.ADP_InvalidDateTimeDigits, dataTypeName));
        }

        internal static Exception InvalidFormatValue()
        {
            return Argument(Res.GetString(Res.ADP_InvalidFormatValue));
        }

        internal static InvalidOperationException InvalidMaximumScale(string dataTypeName)
        {
            return InvalidOperation(Res.GetString(Res.ADP_InvalidMaximumScale, dataTypeName));
        }

        internal static Exception LiteralValueIsInvalid(string dataTypeName)
        {
            return Argument(Res.GetString(Res.ADP_LiteralValueIsInvalid, dataTypeName));
        }

        internal static Exception EvenLengthLiteralValue(string argumentName)
        {
            return Argument(Res.GetString(Res.ADP_EvenLengthLiteralValue), argumentName);
        }

        internal static Exception HexDigitLiteralValue(string argumentName)
        {
            return Argument(Res.GetString(Res.ADP_HexDigitLiteralValue), argumentName);
        }

        internal static InvalidOperationException QuotePrefixNotSet(string method)
        {
            return InvalidOperation(Res.GetString(Res.ADP_QuotePrefixNotSet, method));
        }

        internal static InvalidOperationException UnableToCreateBooleanLiteral()
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_UnableToCreateBooleanLiteral));
        }

        internal static Exception UnsupportedNativeDataTypeOleDb(string dataTypeName)
        {
            return Argument(Res.GetString(Res.ADP_UnsupportedNativeDataTypeOleDb, dataTypeName));
        }

        // Sql Result Set and other generic message
        internal static Exception InvalidArgumentValue(string methodName)
        {
            return Argument(Res.GetString(Res.ADP_InvalidArgumentValue, methodName));
        }

        // global constant strings
        internal const string Append = "Append";
        internal const string BeginExecuteNonQuery = "BeginExecuteNonQuery";
        internal const string BeginExecuteReader = "BeginExecuteReader";
        internal const string BeginTransaction = "BeginTransaction";
        internal const string BeginExecuteXmlReader = "BeginExecuteXmlReader";
        internal const string ChangeDatabase = "ChangeDatabase";
        internal const string Cancel = "Cancel";
        internal const string Clone = "Clone";
        internal const string ColumnEncryptionSystemProviderNamePrefix = "MSSQL_";
        internal const string CommitTransaction = "CommitTransaction";
        internal const string CommandTimeout = "CommandTimeout";
        internal const string ConnectionString = "ConnectionString";
        internal const string DataSetColumn = "DataSetColumn";
        internal const string DataSetTable = "DataSetTable";
        internal const string Delete = "Delete";
        internal const string DeleteCommand = "DeleteCommand";
        internal const string DeriveParameters = "DeriveParameters";
        internal const string EndExecuteNonQuery = "EndExecuteNonQuery";
        internal const string EndExecuteReader = "EndExecuteReader";
        internal const string EndExecuteXmlReader = "EndExecuteXmlReader";
        internal const string ExecuteReader = "ExecuteReader";
        internal const string ExecuteRow = "ExecuteRow";
        internal const string ExecuteNonQuery = "ExecuteNonQuery";
        internal const string ExecuteScalar = "ExecuteScalar";
        internal const string ExecuteSqlScalar = "ExecuteSqlScalar";
        internal const string ExecuteXmlReader = "ExecuteXmlReader";
        internal const string Fill = "Fill";
        internal const string FillPage = "FillPage";
        internal const string FillSchema = "FillSchema";
        internal const string GetBytes = "GetBytes";
        internal const string GetChars = "GetChars";
        internal const string GetOleDbSchemaTable = "GetOleDbSchemaTable";
        internal const string GetProperties = "GetProperties";
        internal const string GetSchema = "GetSchema";
        internal const string GetSchemaTable = "GetSchemaTable";
        internal const string GetServerTransactionLevel = "GetServerTransactionLevel";
        internal const string Insert = "Insert";
        internal const string Open = "Open";
        internal const string Parameter = "Parameter";
        internal const string ParameterBuffer = "buffer";
        internal const string ParameterCount = "count";
        internal const string ParameterDestinationType = "destinationType";
        internal const string ParameterIndex = "index";
        internal const string ParameterName = "ParameterName";
        internal const string ParameterOffset = "offset";
        internal const string ParameterSetPosition = "set_Position";
        internal const string ParameterService = "Service";
        internal const string ParameterTimeout = "Timeout";
        internal const string ParameterUserData = "UserData";
        internal const string Prepare = "Prepare";
        internal const string QuoteIdentifier = "QuoteIdentifier";
        internal const string Read = "Read";
        internal const string ReadAsync = "ReadAsync";
        internal const string Remove = "Remove";
        internal const string RollbackTransaction = "RollbackTransaction";
        internal const string SaveTransaction = "SaveTransaction";
        internal const string SetProperties = "SetProperties";
        internal const string SourceColumn = "SourceColumn";
        internal const string SourceVersion = "SourceVersion";
        internal const string SourceTable = "SourceTable";
        internal const string UnquoteIdentifier = "UnquoteIdentifier";
        internal const string Update = "Update";
        internal const string UpdateCommand = "UpdateCommand";
        internal const string UpdateRows = "UpdateRows";

        internal const CompareOptions compareOptions = CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase;
        internal const int DecimalMaxPrecision = 29;
        internal const int DecimalMaxPrecision28 = 28;  // there are some cases in Odbc where we need that ...
        internal const int DefaultCommandTimeout = 30;
        internal const int DefaultConnectionTimeout = DbConnectionStringDefaults.ConnectTimeout;
        internal const float FailoverTimeoutStep = 0.08F;    // fraction of timeout to use for fast failover connections
        internal const int FirstTransparentAttemptTimeout = 500; // The first login attempt in  Transparent network IP Resolution 

        // security issue, don't rely upon static public readonly values - AS/URT 109635
        internal static readonly String StrEmpty = ""; // String.Empty

        internal static readonly IntPtr PtrZero = new IntPtr(0); // IntPtr.Zero
        internal static readonly int PtrSize = IntPtr.Size;
        internal static readonly IntPtr InvalidPtr = new IntPtr(-1); // use for INVALID_HANDLE
        internal static readonly IntPtr RecordsUnaffected = new IntPtr(-1);

        internal static readonly HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        internal const int CharSize = System.Text.UnicodeEncoding.CharSize;

        internal static bool CompareInsensitiveInvariant(string strvalue, string strconst)
        {
            return (0 == CultureInfo.InvariantCulture.CompareInfo.Compare(strvalue, strconst, CompareOptions.IgnoreCase));
        }

        internal static Delegate FindBuilder(MulticastDelegate mcd)
        { // V1.2.3300
            if (null != mcd)
            {
                Delegate[] d = mcd.GetInvocationList();
                for (int i = 0; i < d.Length; i++)
                {
                    if (d[i].Target is DbCommandBuilder)
                        return d[i];
                }
            }

            return null;
        }

        internal static readonly bool IsWindowsNT = (PlatformID.Win32NT == Environment.OSVersion.Platform);
        internal static readonly bool IsPlatformNT5 = (ADP.IsWindowsNT && (Environment.OSVersion.Version.Major >= 5));

        internal static SysTx.Transaction GetCurrentTransaction()
        {
            SysTx.Transaction transaction = SysTx.Transaction.Current;
            return transaction;
        }

        internal static void SetCurrentTransaction(SysTx.Transaction transaction)
        {
            SysTx.Transaction.Current = transaction;
        }

        internal static SysTx.IDtcTransaction GetOletxTransaction(SysTx.Transaction transaction)
        {
            SysTx.IDtcTransaction oleTxTransaction = null;

            if (null != transaction)
            {
                oleTxTransaction = SysTx.TransactionInterop.GetDtcTransaction(transaction);
            }
            return oleTxTransaction;
        }

        //[System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        //static internal bool IsSysTxEqualSysEsTransaction()
        //{
        //    // This Method won't JIT inproc (ES isn't available), so we code it
        //    // separately and call it behind an if statement.
        //    bool result = (!SysES.ContextUtil.IsInTransaction && null == SysTx.Transaction.Current)
        //               || (SysES.ContextUtil.IsInTransaction && SysTx.Transaction.Current == (SysTx.Transaction)SysES.ContextUtil.SystemTransaction);
        //    return result;
        //}

        //static internal bool NeedManualEnlistment()
        //{
        //    // We need to force a manual enlistment of transactions for ODBC and
        //    // OLEDB whenever the current SysTx transaction != the SysTx transaction
        //    // on the EnterpriseServices ContextUtil, or when ES.ContextUtil is
        //    // not available and there is a non-null current SysTx transaction.
        //    if (IsWindowsNT)
        //    { // we can reference SysTx just not use it on Win9X, we can't ever reference SysES on Win9X
        //        bool isEnterpriseServicesOK = !InOutOfProcHelper.InProc;
        //        if ((isEnterpriseServicesOK && !IsSysTxEqualSysEsTransaction())
        //         || (!isEnterpriseServicesOK && null != SysTx.Transaction.Current))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        internal static bool NeedManualEnlistment()
        {
            return false;
        }

        internal static void TimerCurrent(out long ticks)
        {
            ticks = DateTime.UtcNow.ToFileTimeUtc();
        }

        internal static long TimerCurrent()
        {
            return DateTime.UtcNow.ToFileTimeUtc();
        }

        internal static long TimerFromSeconds(int seconds)
        {
            long result = checked((long)seconds * TimeSpan.TicksPerSecond);
            return result;
        }

        internal static long TimerFromMilliseconds(long milliseconds)
        {
            long result = checked(milliseconds * TimeSpan.TicksPerMillisecond);
            return result;
        }

        internal static bool TimerHasExpired(long timerExpire)
        {
            bool result = TimerCurrent() > timerExpire;
            return result;
        }

        internal static long TimerRemaining(long timerExpire)
        {
            long timerNow = TimerCurrent();
            long result = checked(timerExpire - timerNow);
            return result;
        }

        internal static long TimerRemainingMilliseconds(long timerExpire)
        {
            long result = TimerToMilliseconds(TimerRemaining(timerExpire));
            return result;
        }

        internal static long TimerRemainingSeconds(long timerExpire)
        {
            long result = TimerToSeconds(TimerRemaining(timerExpire));
            return result;
        }

        internal static long TimerToMilliseconds(long timerValue)
        {
            long result = timerValue / TimeSpan.TicksPerMillisecond;
            return result;
        }

        private static long TimerToSeconds(long timerValue)
        {
            long result = timerValue / TimeSpan.TicksPerSecond;
            return result;
        }

        [EnvironmentPermission(SecurityAction.Assert, Read = "COMPUTERNAME")]
        internal static string MachineName()
        {
            // Note: In Longhorn you'll be able to rename a machine without
            // rebooting.  Therefore, don't cache this machine name.
            return Environment.MachineName;
        }

        internal static string BuildQuotedString(string quotePrefix, string quoteSuffix, string unQuotedString)
        {
            StringBuilder resultString = new StringBuilder();
            if (ADP.IsEmpty(quotePrefix) == false)
            {
                resultString.Append(quotePrefix);
            }

            // Assuming that the suffix is escaped by doubling it. i.e. foo"bar becomes "foo""bar".
            if (ADP.IsEmpty(quoteSuffix) == false)
            {
                resultString.Append(unQuotedString.Replace(quoteSuffix, quoteSuffix + quoteSuffix));
                resultString.Append(quoteSuffix);
            }
            else
            {
                resultString.Append(unQuotedString);
            }

            return resultString.ToString();
        }

        private static readonly string s_hexDigits = "0123456789abcdef";

        internal static byte[] ByteArrayFromString(string hexString, string dataTypeName)
        {
            if ((hexString.Length & 0x1) != 0)
            {
                throw ADP.LiteralValueIsInvalid(dataTypeName);
            }
            char[] c = hexString.ToCharArray();
            byte[] b = new byte[hexString.Length / 2];

            CultureInfo invariant = CultureInfo.InvariantCulture;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                int h = s_hexDigits.IndexOf(Char.ToLower(c[i], invariant));
                int l = s_hexDigits.IndexOf(Char.ToLower(c[i + 1], invariant));

                if (h < 0 || l < 0)
                {
                    throw ADP.LiteralValueIsInvalid(dataTypeName);
                }
                b[i / 2] = (byte)((h << 4) | l);
            }
            return b;
        }

        internal static void EscapeSpecialCharacters(string unescapedString, StringBuilder escapedString)
        {
            // note special characters list is from character escapes
            // in the MSDN regular expression language elements documentation
            // added ] since escaping it seems necessary
            const string specialCharacters = ".$^{[(|)*+?\\]";

            foreach (char currentChar in unescapedString)
            {
                if (specialCharacters.IndexOf(currentChar) >= 0)
                {
                    escapedString.Append("\\");
                }
                escapedString.Append(currentChar);
            }
            return;
        }




        internal static string FixUpDecimalSeparator(string numericString,
                                                     Boolean formatLiteral,
                                                     string decimalSeparator,
                                                     char[] exponentSymbols)
        {
            String returnString;
            // don't replace the decimal separator if the string is in exponent format
            if (numericString.IndexOfAny(exponentSymbols) == -1)
            {
                // if the user has set a decimal separator use it, if not use the current culture's value
                if (ADP.IsEmpty(decimalSeparator) == true)
                {
                    decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                }
                if (formatLiteral == true)
                {
                    returnString = numericString.Replace(".", decimalSeparator);
                }
                else
                {
                    returnString = numericString.Replace(decimalSeparator, ".");
                }
            }
            else
            {
                returnString = numericString;
            }
            return returnString;
        }

        //[FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.PathDiscovery)]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        internal static string GetFullPath(string filename)
        { // MDAC 77686
            return Path.GetFullPath(filename);
        }

        // 
        //static internal string GetComputerNameDnsFullyQualified()
        //{
        //    const int ComputerNameDnsFullyQualified = 3; // winbase.h, enum COMPUTER_NAME_FORMAT
        //    const int ERROR_MORE_DATA = 234; // winerror.h

        //    string value;
        //    if (IsPlatformNT5)
        //    {
        //        int length = 0; // length parameter must be zero if buffer is null
        //        // query for the required length
        //        // VSTFDEVDIV 479551 - ensure that GetComputerNameEx does not fail with unexpected values and that the length is positive
        //        int getComputerNameExError = 0;
        //        if (0 == SafeNativeMethods.GetComputerNameEx(ComputerNameDnsFullyQualified, null, ref length))
        //        {
        //            getComputerNameExError = Marshal.GetLastWin32Error();
        //        }
        //        if ((getComputerNameExError != 0 && getComputerNameExError != ERROR_MORE_DATA) || length <= 0)
        //        {
        //            throw ADP.ComputerNameEx(getComputerNameExError);
        //        }

        //        StringBuilder buffer = new StringBuilder(length);
        //        length = buffer.Capacity;
        //        if (0 == SafeNativeMethods.GetComputerNameEx(ComputerNameDnsFullyQualified, buffer, ref length))
        //        {
        //            throw ADP.ComputerNameEx(Marshal.GetLastWin32Error());
        //        }

        //        // Note: In Longhorn you'll be able to rename a machine without
        //        // rebooting.  Therefore, don't cache this machine name.
        //        value = buffer.ToString();
        //    }
        //    else
        //    {
        //        value = ADP.MachineName();
        //    }
        //    return value;
        //}


        //// SxS: the file is opened in FileShare.Read mode allowing several threads/apps to read it simultaneously
        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //static internal Stream GetFileStream(string filename)
        //{
        //    (new FileIOPermission(FileIOPermissionAccess.Read, filename)).Assert();
        //    try
        //    {
        //        return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    }
        //    finally
        //    {
        //        FileIOPermission.RevertAssert();
        //    }
        //}

        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //static internal FileVersionInfo GetVersionInfo(string filename)
        //{
        //    (new FileIOPermission(FileIOPermissionAccess.Read, filename)).Assert(); // MDAC 62038
        //    try
        //    {
        //        return FileVersionInfo.GetVersionInfo(filename); // MDAC 60411
        //    }
        //    finally
        //    {
        //        FileIOPermission.RevertAssert();
        //    }
        //}

        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //static internal Stream GetXmlStreamFromValues(String[] values, String errorString)
        //{
        //    if (values.Length != 1)
        //    {
        //        throw ADP.ConfigWrongNumberOfValues(errorString);
        //    }
        //    return ADP.GetXmlStream(values[0], errorString);
        //}

        //// SxS (VSDD 545786): metadata files are opened from <.NetRuntimeFolder>\CONFIG\<metadatafilename.xml>
        //// this operation is safe in SxS because the file is opened in read-only mode and each NDP runtime accesses its own copy of the metadata
        //// under the runtime folder.
        //// This method returns stream to open file, so its ResourceExposure value is ResourceScope.Machine.
        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //static internal Stream GetXmlStream(String value, String errorString)
        //{
        //    Stream XmlStream;
        //    const string config = "config\\";
        //    // get location of config directory
        //    string rootPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
        //    if (rootPath == null)
        //    {
        //        throw ADP.ConfigUnableToLoadXmlMetaDataFile(errorString);
        //    }
        //    StringBuilder tempstring = new StringBuilder(rootPath.Length + config.Length + value.Length);
        //    tempstring.Append(rootPath);
        //    tempstring.Append(config);
        //    tempstring.Append(value);
        //    String fullPath = tempstring.ToString();

        //    // don't allow relative paths
        //    if (ADP.GetFullPath(fullPath) != fullPath)
        //    {
        //        throw ADP.ConfigUnableToLoadXmlMetaDataFile(errorString);
        //    }

        //    try
        //    {
        //        XmlStream = ADP.GetFileStream(fullPath);
        //    }
        //    catch (Exception e)
        //    {
        //        // 
        //        if (!ADP.IsCatchableExceptionType(e))
        //        {
        //            throw;
        //        }
        //        throw ADP.ConfigUnableToLoadXmlMetaDataFile(errorString);
        //    }

        //    return XmlStream;

        //}

        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //static internal object ClassesRootRegistryValue(string subkey, string queryvalue)
        //{ // MDAC 77697
        //    (new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_CLASSES_ROOT\\" + subkey)).Assert(); // MDAC 62028
        //    try
        //    {
        //        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(subkey, false))
        //        {
        //            return ((null != key) ? key.GetValue(queryvalue) : null);
        //        }
        //    }
        //    catch (SecurityException e)
        //    {
        //        // Even though we assert permission - it's possible there are
        //        // ACL's on registry that cause SecurityException to be thrown.
        //        ADP.TraceExceptionWithoutRethrow(e);
        //        return null;
        //    }
        //    finally
        //    {
        //        RegistryPermission.RevertAssert();
        //    }
        //}

        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //static internal object LocalMachineRegistryValue(string subkey, string queryvalue)
        //{ // MDAC 77697
        //    (new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\" + subkey)).Assert(); // MDAC 62028
        //    try
        //    {
        //        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subkey, false))
        //        {
        //            return ((null != key) ? key.GetValue(queryvalue) : null);
        //        }
        //    }
        //    catch (SecurityException e)
        //    {
        //        // Even though we assert permission - it's possible there are
        //        // ACL's on registry that cause SecurityException to be thrown.
        //        ADP.TraceExceptionWithoutRethrow(e);
        //        return null;
        //    }
        //    finally
        //    {
        //        RegistryPermission.RevertAssert();
        //    }
        //}

        //// SxS: although this method uses registry, it does not expose anything out
        //[ResourceExposure(ResourceScope.None)]
        //[ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
        //static internal void CheckVersionMDAC(bool ifodbcelseoledb)
        //{
        //    int major, minor, build;
        //    string version;

        //    try
        //    {
        //        version = (string)ADP.LocalMachineRegistryValue("Software\\Microsoft\\DataAccess", "FullInstallVer");
        //        if (ADP.IsEmpty(version))
        //        {
        //            string filename = (string)ADP.ClassesRootRegistryValue(System.Data.OleDb.ODB.DataLinks_CLSID, ADP.StrEmpty);
        //            FileVersionInfo versionInfo = ADP.GetVersionInfo(filename); // MDAC 60411
        //            major = versionInfo.FileMajorPart;
        //            minor = versionInfo.FileMinorPart;
        //            build = versionInfo.FileBuildPart;
        //            version = versionInfo.FileVersion;
        //        }
        //        else
        //        {
        //            string[] parts = version.Split('.');
        //            major = Int32.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture);
        //            minor = Int32.Parse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture);
        //            build = Int32.Parse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture);
        //            Int32.Parse(parts[3], NumberStyles.None, CultureInfo.InvariantCulture);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        // 
        //        if (!ADP.IsCatchableExceptionType(e))
        //        {
        //            throw;
        //        }

        //        throw System.Data.OleDb.ODB.MDACNotAvailable(e);
        //    }

        //    // disallow any MDAC version before MDAC 2.6 rtm
        //    // include MDAC 2.51 that ships with Win2k
        //    if ((major < 2) || ((major == 2) && ((minor < 60) || ((minor == 60) && (build < 6526)))))
        //    { // MDAC 66628
        //        if (ifodbcelseoledb)
        //        {
        //            throw ADP.DataAdapter(Res.GetString(Res.Odbc_MDACWrongVersion, version));
        //        }
        //        else
        //        {
        //            throw ADP.DataAdapter(Res.GetString(Res.OleDb_MDACWrongVersion, version));
        //        }
        //    }
        //}

        // the return value is true if the string was quoted and false if it was not
        // this allows the caller to determine if it is an error or not for the quotedString to not be quoted
        internal static Boolean RemoveStringQuotes(string quotePrefix, string quoteSuffix, string quotedString, out string unquotedString)
        {
            int prefixLength;
            if (quotePrefix == null)
            {
                prefixLength = 0;
            }
            else
            {
                prefixLength = quotePrefix.Length;
            }

            int suffixLength;
            if (quoteSuffix == null)
            {
                suffixLength = 0;
            }
            else
            {
                suffixLength = quoteSuffix.Length;
            }

            if ((suffixLength + prefixLength) == 0)
            {
                unquotedString = quotedString;
                return true;
            }

            if (quotedString == null)
            {
                unquotedString = quotedString;
                return false;
            }

            int quotedStringLength = quotedString.Length;

            // is the source string too short to be quoted
            if (quotedStringLength < prefixLength + suffixLength)
            {
                unquotedString = quotedString;
                return false;
            }

            // is the prefix present?
            if (prefixLength > 0)
            {
                if (quotedString.StartsWith(quotePrefix, StringComparison.Ordinal) == false)
                {
                    unquotedString = quotedString;
                    return false;
                }
            }

            // is the suffix present?
            if (suffixLength > 0)
            {
                if (quotedString.EndsWith(quoteSuffix, StringComparison.Ordinal) == false)
                {
                    unquotedString = quotedString;
                    return false;
                }
                unquotedString = quotedString.Substring(prefixLength, quotedStringLength - (prefixLength + suffixLength)).Replace(quoteSuffix + quoteSuffix, quoteSuffix);
            }
            else
            {
                unquotedString = quotedString.Substring(prefixLength, quotedStringLength - prefixLength);
            }
            return true;
        }

        internal static DataRow[] SelectAdapterRows(DataTable dataTable, bool sorted)
        {
            const DataRowState rowStates = DataRowState.Added | DataRowState.Deleted | DataRowState.Modified;

            // equivalent to but faster than 'return dataTable.Select("", "", rowStates);'
            int countAdded = 0, countDeleted = 0, countModifed = 0;
            DataRowCollection rowCollection = dataTable.Rows;
            foreach (DataRow dataRow in rowCollection)
            {
                switch (dataRow.RowState)
                {
                    case DataRowState.Added:
                        countAdded++;
                        break;
                    case DataRowState.Deleted:
                        countDeleted++;
                        break;
                    case DataRowState.Modified:
                        countModifed++;
                        break;
                    default:
                        Debug.Assert(0 == (rowStates & dataRow.RowState), "flagged RowState");
                        break;
                }
            }
            DataRow[] dataRows = new DataRow[countAdded + countDeleted + countModifed];
            if (sorted)
            {
                countModifed = countAdded + countDeleted;
                countDeleted = countAdded;
                countAdded = 0;

                foreach (DataRow dataRow in rowCollection)
                {
                    switch (dataRow.RowState)
                    {
                        case DataRowState.Added:
                            dataRows[countAdded++] = dataRow;
                            break;
                        case DataRowState.Deleted:
                            dataRows[countDeleted++] = dataRow;
                            break;
                        case DataRowState.Modified:
                            dataRows[countModifed++] = dataRow;
                            break;
                        default:
                            Debug.Assert(0 == (rowStates & dataRow.RowState), "flagged RowState");
                            break;
                    }
                }
            }
            else
            {
                int index = 0;
                foreach (DataRow dataRow in rowCollection)
                {
                    if (0 != (dataRow.RowState & rowStates))
                    {
                        dataRows[index++] = dataRow;
                        if (index == dataRows.Length)
                        {
                            break;
                        }
                    }
                }
            }
            return dataRows;
        }

        internal static int StringLength(string inputString)
        {
            return ((null != inputString) ? inputString.Length : 0);
        }

        // { "a", "a", "a" } -> { "a", "a1", "a2" }
        // { "a", "a", "a1" } -> { "a", "a2", "a1" }
        // { "a", "A", "a" } -> { "a", "A1", "a2" }
        // { "a", "A", "a1" } -> { "a", "A2", "a1" } // MDAC 66718
        internal static void BuildSchemaTableInfoTableNames(string[] columnNameArray)
        {
            Dictionary<string, int> hash = new Dictionary<string, int>(columnNameArray.Length);

            int startIndex = columnNameArray.Length; // lowest non-unique index
            for (int i = columnNameArray.Length - 1; 0 <= i; --i)
            {
                string columnName = columnNameArray[i];
                if ((null != columnName) && (0 < columnName.Length))
                {
                    columnName = columnName.ToLower(CultureInfo.InvariantCulture);
                    int index;
                    if (hash.TryGetValue(columnName, out index))
                    {
                        startIndex = Math.Min(startIndex, index);
                    }
                    hash[columnName] = i;
                }
                else
                {
                    columnNameArray[i] = ADP.StrEmpty; // MDAC 66681
                    startIndex = i;
                }
            }
            int uniqueIndex = 1;
            for (int i = startIndex; i < columnNameArray.Length; ++i)
            {
                string columnName = columnNameArray[i];
                if (0 == columnName.Length)
                { // generate a unique name
                    columnNameArray[i] = "Column";
                    uniqueIndex = GenerateUniqueName(hash, ref columnNameArray[i], i, uniqueIndex);
                }
                else
                {
                    columnName = columnName.ToLower(CultureInfo.InvariantCulture);
                    if (i != hash[columnName])
                    {
                        GenerateUniqueName(hash, ref columnNameArray[i], i, 1); // MDAC 66718
                    }
                }
            }
        }

        private static int GenerateUniqueName(Dictionary<string, int> hash, ref string columnName, int index, int uniqueIndex)
        {
            for (; ; ++uniqueIndex)
            {
                string uniqueName = columnName + uniqueIndex.ToString(CultureInfo.InvariantCulture);
                string lowerName = uniqueName.ToLower(CultureInfo.InvariantCulture); // MDAC 66978
                if (!hash.ContainsKey(lowerName))
                {
                    columnName = uniqueName;
                    hash.Add(lowerName, index);
                    break;
                }
            }
            return uniqueIndex;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static IntPtr IntPtrOffset(IntPtr pbase, Int32 offset)
        {
            if (4 == ADP.PtrSize)
            {
                return (IntPtr)checked(pbase.ToInt32() + offset);
            }
            Debug.Assert(8 == ADP.PtrSize, "8 != IntPtr.Size"); // MDAC 73747
            return (IntPtr)checked(pbase.ToInt64() + offset);
        }

        internal static int IntPtrToInt32(IntPtr value)
        {
            if (4 == ADP.PtrSize)
            {
                return (int)value;
            }
            else
            {
                long lval = (long)value;
                lval = Math.Min((long)Int32.MaxValue, lval);
                lval = Math.Max((long)Int32.MinValue, lval);
                return (int)lval;
            }
        }

        // 
        internal static int SrcCompare(string strA, string strB)
        { // this is null safe
            return ((strA == strB) ? 0 : 1);
        }

        internal static int DstCompare(string strA, string strB)
        { // this is null safe
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, ADP.compareOptions);
        }

        internal static bool IsDirection(IDataParameter value, ParameterDirection condition)
        {
#if DEBUG
            IsDirectionValid(condition);
#endif
            return (condition == (condition & value.Direction));
        }
#if DEBUG
        private static void IsDirectionValid(ParameterDirection value)
        {
            switch (value)
            { // @perfnote: Enum.IsDefined
                case ParameterDirection.Input:
                case ParameterDirection.Output:
                case ParameterDirection.InputOutput:
                case ParameterDirection.ReturnValue:
                    break;
                default:
                    throw ADP.InvalidParameterDirection(value);
            }
        }
#endif

        internal static bool IsEmpty(string str)
        {
            return ((null == str) || (0 == str.Length));
        }

        internal static bool IsEmptyArray(string[] array)
        {
            return ((null == array) || (0 == array.Length));
        }

        internal static bool IsNull(object value)
        {
            if ((null == value) || (DBNull.Value == value))
            {
                return true;
            }
            INullable nullable = (value as INullable);
            return ((null != nullable) && nullable.IsNull);
        }

        internal static void IsNullOrSqlType(object value, out bool isNull, out bool isSqlType)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                isNull = true;
                isSqlType = false;
            }
            else
            {
                INullable nullable = (value as INullable);
                if (nullable != null)
                {
                    isNull = nullable.IsNull;
                    isSqlType = DataStorage.IsSqlType(value.GetType());
                }
                else
                {
                    isNull = false;
                    isSqlType = false;
                }
            }
        }

        private static Version s_systemDataVersion;
        internal static Version GetAssemblyVersion()
        {
            // NOTE: Using lazy thread-safety since we don't care if two threads both happen to update the value at the same time
            if (s_systemDataVersion == null)
            {
                s_systemDataVersion = new Version(ThisAssembly.InformationalVersion);
            }

            return s_systemDataVersion;
        }
    }
}
