Imports System
Imports System.Drawing
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.SectionReportModel

Public Class CustomerListReport
    Inherits SectionReport

    Public Sub New()
        BuildReport()
    End Sub

    Private Sub BuildReport()
        Me.MasterReport = False
        Me.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait
        Me.PrintWidth = 7.5!
        Me.Sections.Clear()
        Me.DataSource = SampleDataFactory.CreateCustomerTable()

        Dim pageHeader As New PageHeader() With {
            .Height = 0.75!
        }

        pageHeader.Controls.Add(New Label() With {
            .Text = "Customer List",
            .Left = 0.0!,
            .Top = 0.05!,
            .Width = 3.6!,
            .Height = 0.3!,
            .Style = "font-size: 18pt; font-weight: bold"
        })

        pageHeader.Controls.Add(New Line() With {
            .Left = 0.0!,
            .Top = 0.42!,
            .X1 = 0.0!,
            .X2 = 7.5!,
            .Y1 = 0.42!,
            .Y2 = 0.42!,
            .LineWeight = 1.0!
        })

        AddHeaderLabel(pageHeader, "Customer", 0.0!, 0.48!, 2.15!)
        AddHeaderLabel(pageHeader, "City", 2.2!, 0.48!, 1.35!)
        AddHeaderLabel(pageHeader, "State", 3.6!, 0.48!, 0.65!)
        AddHeaderLabel(pageHeader, "Phone", 4.35!, 0.48!, 1.4!)
        AddHeaderLabel(pageHeader, "Balance", 5.85!, 0.48!, 1.55!, GrapeCity.ActiveReports.Document.Section.TextAlignment.Right)

        Dim detail As New Detail() With {
            .Height = 0.24!
        }

        AddDetailText(detail, "CustomerName", 0.0!, 0.02!, 2.15!)
        AddDetailText(detail, "City", 2.2!, 0.02!, 1.35!)
        AddDetailText(detail, "State", 3.6!, 0.02!, 0.65!)
        AddDetailText(detail, "Phone", 4.35!, 0.02!, 1.4!)
        AddDetailText(detail, "Balance", 5.85!, 0.02!, 1.55!, GrapeCity.ActiveReports.Document.Section.TextAlignment.Right, "C2")

        Dim pageFooter As New PageFooter() With {
            .Height = 0.3!
        }

        pageFooter.Controls.Add(New Line() With {
            .Left = 0.0!,
            .Top = 0.02!,
            .X1 = 0.0!,
            .X2 = 7.5!,
            .Y1 = 0.02!,
            .Y2 = 0.02!,
            .LineWeight = 1.0!
        })

        pageFooter.Controls.Add(New ReportInfo() With {
            .Left = 0.0!,
            .Top = 0.08!,
            .Width = 2.4!,
            .Height = 0.18!,
            .FormatString = "{RunDateTime:MMMM dd, yyyy}",
            .Style = "font-size: 8pt"
        })

        pageFooter.Controls.Add(New ReportInfo() With {
            .Left = 5.8!,
            .Top = 0.08!,
            .Width = 1.7!,
            .Height = 0.18!,
            .Alignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Right,
            .FormatString = "Page {PageNumber}",
            .Style = "font-size: 8pt"
        })

        Me.Sections.Add(pageHeader)
        Me.Sections.Add(detail)
        Me.Sections.Add(pageFooter)
    End Sub

    Private Shared Sub AddHeaderLabel(section As Section, text As String, left As Single, top As Single, width As Single, Optional alignment As GrapeCity.ActiveReports.Document.Section.TextAlignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Left)
        section.Controls.Add(New Label() With {
            .Text = text,
            .Left = left,
            .Top = top,
            .Width = width,
            .Height = 0.18!,
            .Alignment = alignment,
            .Style = "font-size: 9pt; font-weight: bold"
        })
    End Sub

    Private Shared Sub AddDetailText(section As Section, dataField As String, left As Single, top As Single, width As Single, Optional alignment As GrapeCity.ActiveReports.Document.Section.TextAlignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Left, Optional outputFormat As String = Nothing)
        Dim textBox As New TextBox() With {
            .DataField = dataField,
            .Left = left,
            .Top = top,
            .Width = width,
            .Height = 0.18!,
            .Alignment = alignment,
            .Style = "font-size: 9pt"
        }

        If Not String.IsNullOrWhiteSpace(outputFormat) Then
            textBox.OutputFormat = outputFormat
        End If

        section.Controls.Add(textBox)
    End Sub
End Class
