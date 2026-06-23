Option Strict On
Option Explicit On
Option Infer On

Imports System

''' <summary>
''' Represents an application-level failure that occurs while locating, configuring, or displaying a report.
''' </summary>
Public Class ReportingException
    Inherits Exception

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ReportingException"/> class with a message.
    ''' </summary>
    ''' <param name="message">The failure message.</param>
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ReportingException"/> class with a message and inner exception.
    ''' </summary>
    ''' <param name="message">The failure message.</param>
    ''' <param name="innerException">The underlying exception.</param>
    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub
End Class
