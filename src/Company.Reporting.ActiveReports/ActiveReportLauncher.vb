Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Windows.Forms

''' <summary>
''' Provides the public entry point that host applications use to display embedded ActiveReports reports.
''' </summary>
Public NotInheritable Class ActiveReportLauncher
    ''' <summary>
    ''' Prevents direct instantiation of this static-style launcher class.
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Displays the selected report without an explicit owner window.
    ''' </summary>
    ''' <param name="reportKey">The logical report key registered in <see cref="ReportCatalog"/>.</param>
    ''' <param name="sqlConnectionString">The runtime SQL Server connection string for the report.</param>
    ''' <param name="reportParameters">A dictionary of report parameter names and values.</param>
    Public Shared Sub ShowReport(
        reportKey As String,
        sqlConnectionString As String,
        reportParameters As IDictionary(Of String, Object))

        ShowReport(Nothing, reportKey, sqlConnectionString, reportParameters)
    End Sub

    ''' <summary>
    ''' Displays the selected report inside the library-owned modal viewer dialog.
    ''' </summary>
    ''' <param name="owner">The optional owner window for modal centering and dialog parenting.</param>
    ''' <param name="reportKey">The logical report key registered in <see cref="ReportCatalog"/>.</param>
    ''' <param name="sqlConnectionString">The runtime SQL Server connection string for the report.</param>
    ''' <param name="reportParameters">A dictionary of report parameter names and values.</param>
    Public Shared Sub ShowReport(
        owner As IWin32Window,
        reportKey As String,
        sqlConnectionString As String,
        reportParameters As IDictionary(Of String, Object))

        Try
            ValidateInput(reportKey, sqlConnectionString, reportParameters)

            Dim request As New ReportRequest(reportKey.Trim(), sqlConnectionString.Trim(), reportParameters)
            Dim reportDefinition = ReportCatalog.GetByKey(request.ReportKey)
            ValidateRequiredParameters(reportDefinition, request)

            ' The host application never sees the viewer control directly.
            Using viewerForm As New ReportViewerForm(reportDefinition, request)
                If owner IsNot Nothing Then
                    viewerForm.ShowDialog(owner)
                Else
                    viewerForm.ShowDialog()
                End If
            End Using
        Catch ex As ReportingException
            Throw
        Catch ex As Exception
            Throw New ReportingException(String.Format("Unable to show report '{0}'.", reportKey), ex)
        End Try
    End Sub

    ''' <summary>
    ''' Validates the minimum launcher inputs before any report-specific logic runs.
    ''' </summary>
    Private Shared Sub ValidateInput(reportKey As String, sqlConnectionString As String, reportParameters As IDictionary(Of String, Object))
        If String.IsNullOrWhiteSpace(reportKey) Then
            Throw New ReportingException("A report key is required.")
        End If

        If String.IsNullOrWhiteSpace(sqlConnectionString) Then
            Throw New ReportingException("A SQL connection string is required.")
        End If

        If reportParameters Is Nothing Then
            Throw New ReportingException("A report parameter dictionary is required.")
        End If
    End Sub

    ''' <summary>
    ''' Validates the required parameters declared by the selected report definition.
    ''' </summary>
    Private Shared Sub ValidateRequiredParameters(reportDefinition As ReportDefinition, request As ReportRequest)
        For Each parameterName As String In reportDefinition.RequiredParameterNames
            If Not request.ReportParameters.ContainsKey(parameterName) Then
                Throw New ReportingException(
                    String.Format(
                        "The selected report '{0}' requires a parameter named '{1}'.",
                        reportDefinition.Key,
                        parameterName))
            End If

            Dim value = request.ReportParameters(parameterName)
            If value Is Nothing Then
                Throw New ReportingException(String.Format("The required report parameter '{0}' cannot be null.", parameterName))
            End If

            Dim stringValue = TryCast(value, String)
            If stringValue IsNot Nothing AndAlso String.IsNullOrWhiteSpace(stringValue) Then
                Throw New ReportingException(String.Format("The required report parameter '{0}' cannot be blank.", parameterName))
            End If

            ' Sample-report validation can be extended per report as new parameter types are introduced.
            If String.Equals(parameterName, "PINumber", StringComparison.OrdinalIgnoreCase) Then
                Dim parsedValue As Integer

                Try
                    parsedValue = Convert.ToInt32(value, CultureInfo.InvariantCulture)
                Catch ex As Exception
                    Throw New ReportingException("The required report parameter 'PINumber' must be convertible to an integer.", ex)
                End Try

                If parsedValue <= 0 Then
                    Throw New ReportingException("The required report parameter 'PINumber' must be greater than zero.")
                End If
            End If
        Next
    End Sub
End Class
