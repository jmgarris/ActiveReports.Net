Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic

''' <summary>
''' Provides the list of reports that can be launched through the reporting library.
''' </summary>
Public NotInheritable Class ReportCatalog
    Private Shared ReadOnly Reports As IReadOnlyList(Of ReportDefinition) =
        New List(Of ReportDefinition) From {
            New ReportDefinition With {
                .Key = "sample-pi-report",
                .DisplayName = "Sample PI Report",
                .Description = "Embedded RDLX sample that accepts RISName, MBAName, and PINumber.",
                .EmbeddedResourceName = "Company.Reporting.ActiveReports.Reports.SamplePIReport.rdlx",
                .StoredProcedureName = "dbo.usp_SamplePIReport",
                .RequiredParameterNames = New List(Of String) From {
                    "RISName",
                    "MBAName",
                    "PINumber"
                }.AsReadOnly()
            }
        }.AsReadOnly()

    ''' <summary>
    ''' Prevents direct instantiation of this static-style catalog class.
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Returns the reports that the host application may present to the user.
    ''' </summary>
    Public Shared Function GetAvailableReports() As IReadOnlyList(Of ReportDefinition)
        Return Reports
    End Function

    ''' <summary>
    ''' Resolves a registered report definition by key.
    ''' </summary>
    ''' <param name="reportKey">The logical report key.</param>
    ''' <returns>The matching report definition.</returns>
    Public Shared Function GetByKey(reportKey As String) As ReportDefinition
        For Each report As ReportDefinition In Reports
            If String.Equals(report.Key, reportKey, StringComparison.OrdinalIgnoreCase) Then
                Return report
            End If
        Next

        Throw New ReportingException(String.Format("Unknown report key '{0}'.", reportKey))
    End Function
End Class
