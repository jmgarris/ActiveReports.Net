Imports System.Collections.Generic
Imports GrapeCity.ActiveReports

Public Interface IReportProvider
    Function GetAvailableReports() As IReadOnlyList(Of ReportDefinition)
    Function CreateReport(reportKey As String) As SectionReport
End Interface
