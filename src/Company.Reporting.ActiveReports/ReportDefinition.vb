Option Strict On
Option Explicit On
Option Infer On

Imports System.Collections.Generic

Public NotInheritable Class ReportDefinition
    Public Property Key As String
    Public Property DisplayName As String
    Public Property Description As String
    Public Property EmbeddedResourceName As String
    Public Property StoredProcedureName As String
    Public Property RequiredParameterNames As IReadOnlyList(Of String)
End Class
