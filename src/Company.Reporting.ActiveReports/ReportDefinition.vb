Option Strict On
Option Explicit On
Option Infer On

Imports System.Collections.Generic

''' <summary>
''' Describes a report that can be loaded from the embedded report catalog.
''' </summary>
Public NotInheritable Class ReportDefinition
    ''' <summary>
    ''' Gets or sets the logical report key used by host applications.
    ''' </summary>
    Public Property Key As String
    ''' <summary>
    ''' Gets or sets the display name shown in the sample host application.
    ''' </summary>
    Public Property DisplayName As String
    ''' <summary>
    ''' Gets or sets a short description of the report.
    ''' </summary>
    Public Property Description As String
    ''' <summary>
    ''' Gets or sets the exact manifest resource name for the embedded RDLX file.
    ''' </summary>
    Public Property EmbeddedResourceName As String
    ''' <summary>
    ''' Gets or sets the stored procedure name that the report should execute at runtime.
    ''' </summary>
    Public Property StoredProcedureName As String
    ''' <summary>
    ''' Gets or sets the required parameter names that the host application must supply.
    ''' </summary>
    Public Property RequiredParameterNames As IReadOnlyList(Of String)
End Class
