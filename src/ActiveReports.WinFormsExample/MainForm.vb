Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports Company.Reporting.ActiveReports

''' <summary>
''' Demonstrates how a host application can collect input and launch a report without using ActiveReports APIs directly.
''' </summary>
Public Class MainForm
    Inherits Form

    Private ReadOnly tableLayout As TableLayoutPanel
    Private ReadOnly lblConnectionString As Label
    Private ReadOnly txtConnectionString As TextBox
    Private ReadOnly lblRISName As Label
    Private ReadOnly txtRISName As TextBox
    Private ReadOnly lblMBAName As Label
    Private ReadOnly txtMBAName As TextBox
    Private ReadOnly lblPINumber As Label
    Private ReadOnly numPINumber As NumericUpDown
    Private ReadOnly lblReports As Label
    Private ReadOnly cboReports As ComboBox
    Private ReadOnly btnShowReport As Button

    ''' <summary>
    ''' Initializes the sample host application's main form.
    ''' </summary>
    Public Sub New()
        Text = "ActiveReports Embedded Viewer Demo"
        StartPosition = FormStartPosition.CenterScreen
        MinimumSize = New Size(760, 360)
        Size = New Size(900, 420)

        ' The sample host keeps the UI simple and delegates all report behavior to the class library.
        tableLayout = New TableLayoutPanel() With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 6,
            .Padding = New Padding(16),
            .AutoSize = False
        }
        tableLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 170.0F))
        tableLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        For index As Integer = 0 To 4
            tableLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 46.0F))
        Next

        tableLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 58.0F))

        lblConnectionString = New Label() With {
            .AutoSize = True,
            .Anchor = AnchorStyles.Left,
            .Text = "SQL Connection String"
        }

        txtConnectionString = New TextBox() With {
            .Name = "txtConnectionString",
            .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
            .Text = "Server=localhost;Database=MyDatabase;Trusted_Connection=True;"
        }

        lblRISName = New Label() With {
            .AutoSize = True,
            .Anchor = AnchorStyles.Left,
            .Text = "RIS Name"
        }

        txtRISName = New TextBox() With {
            .Name = "txtRISName",
            .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
            .Text = "RIS-1001"
        }

        lblMBAName = New Label() With {
            .AutoSize = True,
            .Anchor = AnchorStyles.Left,
            .Text = "MBA Name"
        }

        txtMBAName = New TextBox() With {
            .Name = "txtMBAName",
            .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
            .Text = "MBA-East"
        }

        lblPINumber = New Label() With {
            .AutoSize = True,
            .Anchor = AnchorStyles.Left,
            .Text = "PI Number"
        }

        numPINumber = New NumericUpDown() With {
            .Name = "numPINumber",
            .Anchor = AnchorStyles.Left,
            .Minimum = 1D,
            .Maximum = 1000000D,
            .Value = 42D,
            .Width = 140
        }

        lblReports = New Label() With {
            .AutoSize = True,
            .Anchor = AnchorStyles.Left,
            .Text = "Report"
        }

        cboReports = New ComboBox() With {
            .Name = "cboReports",
            .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        btnShowReport = New Button() With {
            .Name = "btnShowReport",
            .Text = "Show Report",
            .Anchor = AnchorStyles.Left,
            .Width = 140,
            .Height = 32
        }

        tableLayout.Controls.Add(lblConnectionString, 0, 0)
        tableLayout.Controls.Add(txtConnectionString, 1, 0)
        tableLayout.Controls.Add(lblRISName, 0, 1)
        tableLayout.Controls.Add(txtRISName, 1, 1)
        tableLayout.Controls.Add(lblMBAName, 0, 2)
        tableLayout.Controls.Add(txtMBAName, 1, 2)
        tableLayout.Controls.Add(lblPINumber, 0, 3)
        tableLayout.Controls.Add(numPINumber, 1, 3)
        tableLayout.Controls.Add(lblReports, 0, 4)
        tableLayout.Controls.Add(cboReports, 1, 4)
        tableLayout.Controls.Add(btnShowReport, 1, 5)

        Controls.Add(tableLayout)

        AddHandler Load, AddressOf MainForm_Load
        AddHandler btnShowReport.Click, AddressOf btnShowReport_Click
    End Sub

    ''' <summary>
    ''' Loads the report list from the reporting library when the form opens.
    ''' </summary>
    Private Sub MainForm_Load(sender As Object, e As EventArgs)
        Dim availableReports As New List(Of ReportDefinition)(ReportCatalog.GetAvailableReports())

        cboReports.DisplayMember = NameOf(ReportDefinition.DisplayName)
        cboReports.ValueMember = NameOf(ReportDefinition.Key)
        cboReports.DataSource = availableReports

        If availableReports.Count > 0 Then
            cboReports.SelectedIndex = 0
        End If
    End Sub

    ''' <summary>
    ''' Builds the sample parameter dictionary and asks the reporting library to display the report.
    ''' </summary>
    Private Sub btnShowReport_Click(sender As Object, e As EventArgs)
        Dim selectedReportKey = TryCast(cboReports.SelectedValue, String)

        If String.IsNullOrWhiteSpace(selectedReportKey) Then
            MessageBox.Show(Me, "Select a report before continuing.", "Missing Report", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' The host application only supplies values. The reporting library owns all ActiveReports behavior.
            Dim parameters As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                {"RISName", txtRISName.Text.Trim()},
                {"MBAName", txtMBAName.Text.Trim()},
                {"PINumber", Decimal.ToInt32(numPINumber.Value)}
            }

            ActiveReportLauncher.ShowReport(
                Me,
                selectedReportKey,
                txtConnectionString.Text.Trim(),
                parameters)
        Catch ex As ReportingException
            MessageBox.Show(Me, ex.Message, "Reporting Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
