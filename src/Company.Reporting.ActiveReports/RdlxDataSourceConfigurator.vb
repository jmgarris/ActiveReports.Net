Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.Document
Imports GrapeCity.ActiveReports.PageReportModel
Imports GrapeCity.Enterprise.Data.Expressions

''' <summary>
''' Applies runtime connection and parameter values to embedded RDLX page reports.
''' </summary>
Public NotInheritable Class RdlxDataSourceConfigurator
    ''' <summary>
    ''' Prevents direct instantiation of this static-style configurator class.
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Applies the runtime connection string and parameter values, then creates the configured page document.
    ''' </summary>
    ''' <param name="pageReport">The loaded page report definition.</param>
    ''' <param name="reportDefinition">The logical report definition selected by the host application.</param>
    ''' <param name="request">The runtime request containing the SQL connection string and parameter values.</param>
    ''' <returns>A configured <see cref="PageDocument"/> ready for the viewer.</returns>
    Public Shared Function Configure(pageReport As PageReport, reportDefinition As ReportDefinition, request As ReportRequest) As PageDocument
        If pageReport Is Nothing Then
            Throw New ReportingException("A PageReport instance is required.")
        End If

        If reportDefinition Is Nothing Then
            Throw New ReportingException("A report definition is required.")
        End If

        If request Is Nothing Then
            Throw New ReportingException("A report request is required.")
        End If

        Try
            Dim reportModel = pageReport.Report
            If reportModel Is Nothing Then
                Throw New ReportingException("The embedded RDLX report could not be parsed into a PageReportModel.Report.")
            End If

            ValidateReportParameters(reportModel, reportDefinition)

            Dim dataSource = FindDataSource(reportModel)
            Dim dataSet = FindDataSet(reportModel)
            Dim query = dataSet.Query

            If query Is Nothing Then
                Throw New ReportingException("The embedded RDLX report does not define a dataset query to configure.")
            End If

            ' Adjust these names if your RDLX file uses different data source or dataset names.
            dataSource.ConnectionProperties.DataProvider = "SQL"
            dataSource.ConnectionProperties.ConnectString = ExpressionInfo.FromString(request.SqlConnectionString)
            query.CommandType = QueryCommandType.StoredProcedure
            query.CommandText = ExpressionInfo.FromString(reportDefinition.StoredProcedureName)

            ' Report parameters drive both the on-screen document state and the underlying stored procedure inputs.
            ConfigureQueryParameters(query, reportDefinition, request.ReportParameters)

            Dim pageDocument As New PageDocument(pageReport)
            ApplyDocumentParameters(pageDocument, reportDefinition, request.ReportParameters)

            Return pageDocument
        Catch ex As ReportingException
            Throw
        Catch ex As Exception
            Throw New ReportingException("Failed to configure the embedded RDLX report data source or parameters.", ex)
        End Try
    End Function

    ''' <summary>
    ''' Ensures that the report definition contains the required report parameter declarations.
    ''' </summary>
    Private Shared Sub ValidateReportParameters(reportModel As Report, reportDefinition As ReportDefinition)
        For Each parameterName As String In reportDefinition.RequiredParameterNames
            Dim found As Boolean = False

            For Each reportParameter As ReportParameter In reportModel.ReportParameters
                If String.Equals(reportParameter.Name, parameterName, StringComparison.OrdinalIgnoreCase) Then
                    found = True
                    Exit For
                End If
            Next

            If Not found Then
                Throw New ReportingException(String.Format("The embedded RDLX report is missing the required report parameter '{0}'.", parameterName))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Finds the first available data source in the loaded report definition.
    ''' </summary>
    Private Shared Function FindDataSource(reportModel As Report) As DataSource
        For Each dataSource As DataSource In reportModel.DataSources
            If dataSource IsNot Nothing Then
                Return dataSource
            End If
        Next

        Throw New ReportingException("The embedded RDLX report does not contain a data source to configure.")
    End Function

    ''' <summary>
    ''' Finds the first concrete dataset in the loaded report definition.
    ''' </summary>
    Private Shared Function FindDataSet(reportModel As Report) As DataSet
        For Each dataSetDefinition As IDataSet In reportModel.DataSets
            Dim concreteDataSet = TryCast(dataSetDefinition, DataSet)
            If concreteDataSet IsNot Nothing Then
                Return concreteDataSet
            End If
        Next

        Throw New ReportingException("The embedded RDLX report does not contain a dataset to configure.")
    End Function

    ''' <summary>
    ''' Maps report parameter names to stored procedure parameter expressions.
    ''' </summary>
    Private Shared Sub ConfigureQueryParameters(query As Query, reportDefinition As ReportDefinition, reportParameters As IReadOnlyDictionary(Of String, Object))
        Dim parameterNames As New HashSet(Of String)(reportDefinition.RequiredParameterNames, StringComparer.OrdinalIgnoreCase)

        For Each parameterName As String In reportParameters.Keys
            parameterNames.Add(parameterName)
        Next

        For Each parameterName As String In parameterNames
            Dim queryParameter = FindQueryParameter(query, "@" & parameterName)
            If queryParameter IsNot Nothing Then
                queryParameter.Value = ExpressionInfo.FromString(String.Format("=Parameters!{0}.Value", parameterName))
            ElseIf ContainsIgnoreCase(reportDefinition.RequiredParameterNames, parameterName) Then
                Throw New ReportingException(
                    String.Format(
                        "The embedded RDLX report is missing the expected stored procedure parameter '@{0}'.",
                        parameterName))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Searches the dataset query for a matching stored procedure parameter.
    ''' </summary>
    Private Shared Function FindQueryParameter(query As Query, parameterName As String) As QueryParameter
        For Each queryParameter As QueryParameter In query.QueryParameters
            If String.Equals(queryParameter.Name, parameterName, StringComparison.OrdinalIgnoreCase) Then
                Return queryParameter
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Applies supplied runtime values to matching page-document parameters.
    ''' </summary>
    Private Shared Sub ApplyDocumentParameters(pageDocument As PageDocument, reportDefinition As ReportDefinition, reportParameters As IReadOnlyDictionary(Of String, Object))
        For Each parameterName As String In reportDefinition.RequiredParameterNames
            If Not pageDocument.Parameters.Contains(parameterName) Then
                Throw New ReportingException(String.Format("The loaded report document does not expose a required parameter named '{0}'.", parameterName))
            End If
        Next

        For Each entry In reportParameters
            If pageDocument.Parameters.Contains(entry.Key) Then
                pageDocument.Parameters(entry.Key).CurrentValue = entry.Value
            End If
        Next
    End Sub

    ''' <summary>
    ''' Performs a case-insensitive search over the provided string sequence.
    ''' </summary>
    Private Shared Function ContainsIgnoreCase(values As IEnumerable(Of String), expectedValue As String) As Boolean
        For Each value As String In values
            If String.Equals(value, expectedValue, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function
End Class
