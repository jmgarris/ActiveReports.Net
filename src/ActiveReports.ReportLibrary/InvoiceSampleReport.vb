Imports System
Imports System.Data
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.SectionReportModel

Public Class InvoiceSampleReport
    Inherits SectionReport

    Private Const TaxRate As Decimal = 0.0825D

    Public Sub New()
        BuildReport()
    End Sub

    Private Sub BuildReport()
        Dim invoiceTable = SampleDataFactory.CreateInvoiceLineTable()
        Dim subtotal = CalculateSubtotal(invoiceTable)
        Dim tax = Decimal.Round(subtotal * TaxRate, 2, MidpointRounding.AwayFromZero)
        Dim grandTotal = subtotal + tax

        Me.MasterReport = False
        Me.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait
        Me.PrintWidth = 7.5!
        Me.Sections.Clear()
        Me.DataSource = invoiceTable

        Dim reportHeader As New ReportHeader() With {
            .Height = 1.45!
        }

        reportHeader.Controls.Add(New Label() With {
            .Text = "Invoice",
            .Left = 0.0!,
            .Top = 0.0!,
            .Width = 3.0!,
            .Height = 0.3!,
            .Style = "font-size: 22pt; font-weight: bold"
        })

        reportHeader.Controls.Add(New Label() With {
            .Text = "Sample Customer" & Environment.NewLine &
                    "Acme Distribution" & Environment.NewLine &
                    "1450 Market Street" & Environment.NewLine &
                    "Philadelphia, PA 19107",
            .Left = 0.0!,
            .Top = 0.42!,
            .Width = 3.0!,
            .Height = 0.7!,
            .Style = "font-size: 10pt"
        })

        reportHeader.Controls.Add(New Label() With {
            .Text = "Invoice #: INV-2026-001",
            .Left = 4.7!,
            .Top = 0.42!,
            .Width = 2.2!,
            .Height = 0.18!,
            .Style = "font-size: 10pt; font-weight: bold",
            .Alignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Right
        })

        reportHeader.Controls.Add(New Label() With {
            .Text = "Invoice Date: June 23, 2026",
            .Left = 4.7!,
            .Top = 0.66!,
            .Width = 2.2!,
            .Height = 0.18!,
            .Style = "font-size: 10pt",
            .Alignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Right
        })

        reportHeader.Controls.Add(New Line() With {
            .Left = 0.0!,
            .Top = 1.25!,
            .X1 = 0.0!,
            .X2 = 7.5!,
            .Y1 = 1.25!,
            .Y2 = 1.25!,
            .LineWeight = 1.0!
        })

        Dim pageHeader As New PageHeader() With {
            .Height = 0.25!
        }

        AddHeaderLabel(pageHeader, "Qty", 0.0!, 0.03!, 0.6!)
        AddHeaderLabel(pageHeader, "Description", 0.75!, 0.03!, 3.75!)
        AddHeaderLabel(pageHeader, "Unit Price", 4.8!, 0.03!, 1.05!, GrapeCity.ActiveReports.Document.Section.TextAlignment.Right)
        AddHeaderLabel(pageHeader, "Line Total", 6.0!, 0.03!, 1.3!, GrapeCity.ActiveReports.Document.Section.TextAlignment.Right)

        Dim detail As New Detail() With {
            .Height = 0.24!
        }

        AddDetailText(detail, "Quantity", 0.0!, 0.03!, 0.6!)
        AddDetailText(detail, "Description", 0.75!, 0.03!, 3.75!)
        AddDetailText(detail, "UnitPrice", 4.8!, 0.03!, 1.05!, GrapeCity.ActiveReports.Document.Section.TextAlignment.Right, "C2")
        AddDetailText(detail, "LineTotal", 6.0!, 0.03!, 1.3!, GrapeCity.ActiveReports.Document.Section.TextAlignment.Right, "C2")

        Dim reportFooter As New ReportFooter() With {
            .Height = 0.9!
        }

        reportFooter.Controls.Add(New Line() With {
            .Left = 4.55!,
            .Top = 0.05!,
            .X1 = 4.55!,
            .X2 = 7.3!,
            .Y1 = 0.05!,
            .Y2 = 0.05!,
            .LineWeight = 1.0!
        })

        AddSummaryLabel(reportFooter, "Subtotal:", 4.75!, 0.14!, 1.1!)
        AddSummaryValue(reportFooter, subtotal, 6.0!, 0.14!)
        AddSummaryLabel(reportFooter, "Tax:", 4.75!, 0.36!, 1.1!)
        AddSummaryValue(reportFooter, tax, 6.0!, 0.36!)
        AddSummaryLabel(reportFooter, "Grand Total:", 4.75!, 0.6!, 1.1!, True)
        AddSummaryValue(reportFooter, grandTotal, 6.0!, 0.6!, True)

        Me.Sections.Add(reportHeader)
        Me.Sections.Add(pageHeader)
        Me.Sections.Add(detail)
        Me.Sections.Add(reportFooter)
    End Sub

    Private Shared Function CalculateSubtotal(invoiceTable As DataTable) As Decimal
        Dim subtotal As Decimal = 0D

        For Each row As DataRow In invoiceTable.Rows
            subtotal += DirectCast(row("LineTotal"), Decimal)
        Next

        Return subtotal
    End Function

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

    Private Shared Sub AddSummaryLabel(section As Section, text As String, left As Single, top As Single, width As Single, Optional bold As Boolean = False)
        section.Controls.Add(New Label() With {
            .Text = text,
            .Left = left,
            .Top = top,
            .Width = width,
            .Height = 0.18!,
            .Alignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Right,
            .Style = If(bold, "font-size: 10pt; font-weight: bold", "font-size: 10pt")
        })
    End Sub

    Private Shared Sub AddSummaryValue(section As Section, value As Decimal, left As Single, top As Single, Optional bold As Boolean = False)
        section.Controls.Add(New TextBox() With {
            .Text = value.ToString("C2"),
            .Left = left,
            .Top = top,
            .Width = 1.3!,
            .Height = 0.18!,
            .Alignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Right,
            .Style = If(bold, "font-size: 10pt; font-weight: bold", "font-size: 10pt")
        })
    End Sub
End Class
