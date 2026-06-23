Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports GrapeCity.ActiveReports.Document
Imports GrapeCity.ActiveReports.Viewer.Win

''' <summary>
''' Hosts the library-owned ActiveReports WinForms viewer and loads the selected embedded report.
''' </summary>
Public Class ReportViewerForm
    Inherits Form

    Private ReadOnly _reportDefinition As ReportDefinition
    Private ReadOnly _request As ReportRequest

    Private ReadOnly headerPanel As Panel
    Private ReadOnly lblTitle As Label
    Private ReadOnly btnClose As Button
    Private ReadOnly viewer As Viewer

    Private _temporaryReportPath As String
    Private _pageDocument As PageDocument
    Private _hasLoaded As Boolean

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ReportViewerForm"/> class.
    ''' </summary>
    ''' <param name="reportDefinition">The selected report definition.</param>
    ''' <param name="request">The runtime request that will be applied to the report.</param>
    Public Sub New(reportDefinition As ReportDefinition, request As ReportRequest)
        _reportDefinition = reportDefinition
        _request = request

        Text = "Embedded ActiveReports Viewer"
        StartPosition = FormStartPosition.CenterParent
        MinimumSize = New Size(900, 650)
        Size = New Size(1100, 800)

        headerPanel = New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 44,
            .Padding = New Padding(12, 10, 12, 8)
        }

        lblTitle = New Label() With {
            .AutoSize = False,
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font(Font.FontFamily, 10.0F, FontStyle.Bold),
            .Text = reportDefinition.DisplayName
        }

        btnClose = New Button() With {
            .Dock = DockStyle.Right,
            .Width = 90,
            .Text = "Close"
        }

        viewer = New Viewer() With {
            .Dock = DockStyle.Fill
        }

        headerPanel.Controls.Add(lblTitle)
        headerPanel.Controls.Add(btnClose)

        Controls.Add(viewer)
        Controls.Add(headerPanel)

        AddHandler btnClose.Click, AddressOf btnClose_Click
        AddHandler Shown, AddressOf ReportViewerForm_Shown
        AddHandler FormClosed, AddressOf ReportViewerForm_FormClosed
    End Sub

    ''' <summary>
    ''' Loads and displays the report the first time the form is shown.
    ''' </summary>
    Private Sub ReportViewerForm_Shown(sender As Object, e As EventArgs)
        If _hasLoaded Then
            Return
        End If

        _hasLoaded = True

        Try
            Dim loadResult = EmbeddedRdlxReportLoader.Load(_reportDefinition)
            _temporaryReportPath = loadResult.TemporaryFilePath
            _pageDocument = RdlxDataSourceConfigurator.Configure(loadResult.PageReport, _reportDefinition, _request)
            viewer.LoadDocument(_pageDocument)
        Catch ex As Exception
            MessageBox.Show(
                Me,
                ex.Message,
                "Unable to Load Report",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

            DialogResult = DialogResult.Abort
            Close()
        End Try
    End Sub

    ''' <summary>
    ''' Closes the viewer dialog when the close button is clicked.
    ''' </summary>
    Private Sub btnClose_Click(sender As Object, e As EventArgs)
        Close()
    End Sub

    ''' <summary>
    ''' Deletes the temporary RDLX file created for report loading.
    ''' </summary>
    Private Sub ReportViewerForm_FormClosed(sender As Object, e As FormClosedEventArgs)
        If String.IsNullOrWhiteSpace(_temporaryReportPath) Then
            Return
        End If

        Try
            If File.Exists(_temporaryReportPath) Then
                File.Delete(_temporaryReportPath)
            End If
        Catch
            ' Best effort cleanup only. The host application never sees the temp path.
        End Try
    End Sub
End Class
