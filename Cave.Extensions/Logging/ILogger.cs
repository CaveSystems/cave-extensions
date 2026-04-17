using System;
using System.Runtime.CompilerServices;

namespace Cave.Logging;

/// <summary>
/// Defines a logging abstraction that supports multiple severity levels and
/// overloads for structured, formattable messages and exceptions.
/// Implementations are responsible for routing log entries to the desired
/// sinks (console, file, telemetry, etc.) and for honoring any configured
/// log-level filters.
/// </summary>
/// <remarks>
/// A full featured async logging solution able to handle hundred thousands
/// of messages per second is available in packet Cave.Logging.
/// </remarks>
public interface ILogger
{
    /// <summary>
    /// The logical name of the sender (typically the type or component name)
    /// that will be associated with emitted log entries.
    /// </summary>
    string SenderName { get; set; }

    /// <summary>
    /// Optional additional source or context identifier (for example an
    /// assembly or module name).
    /// </summary>
    string? SenderSource { get; set; }

    /// <summary>
    /// Optional concrete .NET <see cref="Type"/> of the sender. Can be used
    /// for more precise identification or reflection-based processing.
    /// </summary>
    Type? SenderType { get; set; }

    /// <summary>
    /// Log an alert with a required exception. Use this for highest-priority
    /// conditions that include an exception object.
    /// </summary>
    /// <param name="exception">Exception to log (required).</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Alert(Exception exception, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an alert with formattable content and an optional exception.
    /// Prefer interpolated strings to preserve structured data when supported
    /// by the implementation.
    /// </summary>
    /// <param name="content">Formattable message content (e.g. interpolated string).</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Alert(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an alert using an <see cref="IFormattable"/> message (pre-formatted).
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Alert(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a critical condition. Use when the application encounters a failure
    /// that requires immediate attention.
    /// </summary>
    /// <param name="exception">Optional exception related to the critical condition.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Critical(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a critical condition with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Critical(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a critical condition using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Critical(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a debug message. Intended for development-time diagnostics and
    /// verbose information that is usually disabled in production.
    /// </summary>
    /// <param name="exception">Optional exception to attach.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Debug(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a debug message with formattable content and an optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Debug(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a debug message using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Debug(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an emergency condition that requires immediate human or automated intervention.
    /// </summary>
    /// <param name="exception">Optional exception related to the emergency.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Emergency(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an emergency message with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Emergency(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an emergency using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Emergency(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an error condition.
    /// </summary>
    /// <param name="exception">Optionally provides error details.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Error(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an error message with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Error(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an error using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Error(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an informational message.
    /// </summary>
    /// <param name="exception">Optional exception to include.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Info(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an informational message with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Info(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log an informational message using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Info(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a notice-level message; typically used for normal but significant events.
    /// </summary>
    /// <param name="exception">Optional exception to include.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Notice(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a notice message with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Notice(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a notice using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Notice(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a verbose message for very detailed diagnostic output.
    /// </summary>
    /// <param name="exception">Optional exception to include.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Verbose(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a verbose message with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Verbose(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a verbose message using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Verbose(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a warning indicating a potential problem or important runtime condition.
    /// </summary>
    /// <param name="exception">Optional exception to include.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Warning(Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a warning with formattable content and optional exception.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Warning(FormattableString content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);

    /// <summary>
    /// Log a warning using an <see cref="IFormattable"/> message.
    /// </summary>
    /// <param name="content">Formattable message content.</param>
    /// <param name="exception">Optional related exception.</param>
    /// <param name="member">Compiler-inserted caller member name.</param>
    /// <param name="file">Compiler-inserted caller file path.</param>
    /// <param name="line">Compiler-inserted caller line number.</param>
    void Warning(IFormattable content, Exception? exception = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0);
}
