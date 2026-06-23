Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.IO
Imports System.Reflection
Imports GrapeCity.ActiveReports

''' <summary>
''' Loads embedded RDLX report resources into <see cref="PageReport"/> instances.
''' </summary>
Public NotInheritable Class EmbeddedRdlxReportLoader
    ''' <summary>
    ''' Prevents direct instantiation of this static-style loader class.
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Loads the embedded report resource for the supplied definition and returns the loaded report plus any temp file used.
    ''' </summary>
    ''' <param name="reportDefinition">The report definition that identifies the embedded resource.</param>
    ''' <returns>A wrapper containing the loaded <see cref="PageReport"/> and its temporary file path.</returns>
    Public Shared Function Load(reportDefinition As ReportDefinition) As LoadedEmbeddedReport
        If reportDefinition Is Nothing Then
            Throw New ReportingException("A report definition is required before loading an embedded RDLX report.")
        End If

        Dim assembly = GetType(EmbeddedRdlxReportLoader).Assembly
        Dim availableResources = assembly.GetManifestResourceNames()

        Try
            Using resourceStream = assembly.GetManifestResourceStream(reportDefinition.EmbeddedResourceName)
                If resourceStream Is Nothing Then
                    Throw New ReportingException(
                        String.Format(
                            "Embedded report resource '{0}' was not found. Available resources: {1}",
                            reportDefinition.EmbeddedResourceName,
                            String.Join(", ", availableResources)))
                End If

                ' ActiveReports page reports are loaded from a temporary file so the host application does not need
                ' to understand anything about embedded resource extraction.
                Dim tempFilePath = Path.Combine(
                    Path.GetTempPath(),
                    String.Format("{0}.{1}.rdlx", assembly.GetName().Name, Guid.NewGuid().ToString("N")))

                Using fileStream As New FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None)
                    resourceStream.CopyTo(fileStream)
                End Using

                Dim pageReport As New PageReport(New FileInfo(tempFilePath))
                Return New LoadedEmbeddedReport(pageReport, tempFilePath)
            End Using
        Catch ex As ReportingException
            Throw
        Catch ex As Exception
            Throw New ReportingException(
                String.Format("Failed to load embedded RDLX report '{0}'.", reportDefinition.EmbeddedResourceName),
                ex)
        End Try
    End Function

    ''' <summary>
    ''' Represents the result of loading an embedded RDLX report.
    ''' </summary>
    Public NotInheritable Class LoadedEmbeddedReport
        ''' <summary>
        ''' Initializes a new instance of the <see cref="LoadedEmbeddedReport"/> class.
        ''' </summary>
        ''' <param name="pageReport">The loaded page report.</param>
        ''' <param name="temporaryFilePath">The temporary file path backing the loaded report.</param>
        Public Sub New(pageReport As PageReport, temporaryFilePath As String)
            Me.PageReport = pageReport
            Me.TemporaryFilePath = temporaryFilePath
        End Sub

        ''' <summary>
        ''' Gets the loaded ActiveReports page report instance.
        ''' </summary>
        Public ReadOnly Property PageReport As PageReport
        ''' <summary>
        ''' Gets the temporary file path created to load the embedded resource.
        ''' </summary>
        Public ReadOnly Property TemporaryFilePath As String
    End Class
End Class
