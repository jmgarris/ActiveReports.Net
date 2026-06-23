Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic

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

    Private Sub New()
    End Sub

    Public Shared Function GetAvailableReports() As IReadOnlyList(Of ReportDefinition)
        Return Reports
    End Function

    Public Shared Function GetByKey(reportKey As String) As ReportDefinition
        For Each report As ReportDefinition In Reports
            If String.Equals(report.Key, reportKey, StringComparison.OrdinalIgnoreCase) Then
                Return report
            End If
        Next

        Throw New ReportingException(String.Format("Unknown report key '{0}'.", reportKey))
    End Function
End Class
