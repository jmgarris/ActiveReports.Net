Imports System
Imports System.Windows.Forms

''' <summary>
''' Defines the WinForms application entry point for the sample host.
''' </summary>
Module Program
    ''' <summary>
    ''' Starts the sample host application.
    ''' </summary>
    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New MainForm())
    End Sub
End Module
