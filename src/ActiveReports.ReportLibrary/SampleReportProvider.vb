Imports System
Imports System.Collections.Generic

Public Class SampleReportProvider
    Implements IReportProvider

    Private Shared ReadOnly Reports As IReadOnlyList(Of ReportDefinition) =
        New List(Of ReportDefinition) From {
            New ReportDefinition With {
                .Key = "customer-list",
                .DisplayName = "Customer List",
                .Description = "A simple customer directory with balances."
            },
            New ReportDefinition With {
                .Key = "invoice-sample",
                .DisplayName = "Invoice Sample",
                .Description = "An invoice-style report with calculated totals."
            }
        }.AsReadOnly()

    Public Function GetAvailableReports() As IReadOnlyList(Of ReportDefinition) Implements IReportProvider.GetAvailableReports
        Return Reports
    End Function

    Public Function CreateReport(reportKey As String) As GrapeCity.ActiveReports.SectionReport Implements IReportProvider.CreateReport
        If String.IsNullOrWhiteSpace(reportKey) Then
            Throw New ArgumentException("A report key is required.", NameOf(reportKey))
        End If

        Select Case reportKey.Trim().ToLowerInvariant()
            Case "customer-list"
                Return New CustomerListReport()
            Case "invoice-sample"
                Return New InvoiceSampleReport()
            Case Else
                ' Real production report registrations would typically be added here.
                Throw New ArgumentException(String.Format("Unknown report key '{0}'.", reportKey), NameOf(reportKey))
        End Select
    End Function
End Class
