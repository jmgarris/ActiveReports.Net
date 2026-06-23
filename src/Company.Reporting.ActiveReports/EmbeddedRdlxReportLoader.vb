Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.IO
Imports System.Reflection
Imports GrapeCity.ActiveReports

Public NotInheritable Class EmbeddedRdlxReportLoader
    Private Sub New()
    End Sub

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

    Public NotInheritable Class LoadedEmbeddedReport
        Public Sub New(pageReport As PageReport, temporaryFilePath As String)
            Me.PageReport = pageReport
            Me.TemporaryFilePath = temporaryFilePath
        End Sub

        Public ReadOnly Property PageReport As PageReport
        Public ReadOnly Property TemporaryFilePath As String
    End Class
End Class
