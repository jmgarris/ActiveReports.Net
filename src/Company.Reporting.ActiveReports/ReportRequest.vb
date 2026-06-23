Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

''' <summary>
''' Represents a validated request to display a report with a connection string and runtime parameters.
''' </summary>
Public NotInheritable Class ReportRequest
    ''' <summary>
    ''' Initializes a new instance of the <see cref="ReportRequest"/> class.
    ''' </summary>
    ''' <param name="reportKey">The logical report key.</param>
    ''' <param name="sqlConnectionString">The SQL connection string to apply at runtime.</param>
    ''' <param name="reportParameters">The caller-supplied report parameters.</param>
    Public Sub New(reportKey As String, sqlConnectionString As String, reportParameters As IDictionary(Of String, Object))
        If String.IsNullOrWhiteSpace(reportKey) Then
            Throw New ReportingException("A report key is required.")
        End If

        If String.IsNullOrWhiteSpace(sqlConnectionString) Then
            Throw New ReportingException("A SQL connection string is required.")
        End If

        If reportParameters Is Nothing Then
            Throw New ReportingException("A report parameter dictionary is required.")
        End If

        ' The request keeps its own case-insensitive copy so callers cannot mutate state after launch.
        Dim copiedParameters As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

        For Each entry In reportParameters
            If String.IsNullOrWhiteSpace(entry.Key) Then
                Throw New ReportingException("Report parameter names cannot be blank.")
            End If

            copiedParameters(entry.Key.Trim()) = entry.Value
        Next

        Me.ReportKey = reportKey
        Me.SqlConnectionString = sqlConnectionString
        Me.ReportParameters = New ReadOnlyDictionary(Of String, Object)(copiedParameters)
    End Sub

    ''' <summary>
    ''' Gets the logical report key to load.
    ''' </summary>
    Public ReadOnly Property ReportKey As String
    ''' <summary>
    ''' Gets the runtime SQL connection string.
    ''' </summary>
    Public ReadOnly Property SqlConnectionString As String
    ''' <summary>
    ''' Gets the runtime report parameters as a read-only case-insensitive dictionary.
    ''' </summary>
    Public ReadOnly Property ReportParameters As IReadOnlyDictionary(Of String, Object)
End Class
