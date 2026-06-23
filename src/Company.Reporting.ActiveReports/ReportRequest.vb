Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Public NotInheritable Class ReportRequest
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

    Public ReadOnly Property ReportKey As String
    Public ReadOnly Property SqlConnectionString As String
    Public ReadOnly Property ReportParameters As IReadOnlyDictionary(Of String, Object)
End Class
