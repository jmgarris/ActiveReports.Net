Imports System
Imports System.Data

Public NotInheritable Class SampleDataFactory
    Private Sub New()
    End Sub

    Public Shared Function CreateCustomerTable() As DataTable
        Dim table As New DataTable("Customers")

        table.Columns.Add("CustomerName", GetType(String))
        table.Columns.Add("City", GetType(String))
        table.Columns.Add("State", GetType(String))
        table.Columns.Add("Phone", GetType(String))
        table.Columns.Add("Balance", GetType(Decimal))

        table.Rows.Add("Northwind Traders", "Seattle", "WA", "(206) 555-0147", 12450.32D)
        table.Rows.Add("Adventure Works", "Denver", "CO", "(303) 555-0105", 985.0D)
        table.Rows.Add("Contoso Retail", "Chicago", "IL", "(312) 555-0199", 4120.75D)
        table.Rows.Add("Fabrikam Services", "Austin", "TX", "(512) 555-0133", 218.4D)
        table.Rows.Add("Wide World Importers", "Boston", "MA", "(617) 555-0170", 7342.19D)

        Return table
    End Function

    Public Shared Function CreateInvoiceLineTable() As DataTable
        Dim table As New DataTable("InvoiceLines")

        table.Columns.Add("Quantity", GetType(Integer))
        table.Columns.Add("Description", GetType(String))
        table.Columns.Add("UnitPrice", GetType(Decimal))
        table.Columns.Add("LineTotal", GetType(Decimal))

        AddInvoiceLine(table, 2, "ActiveReports Professional Services", 350D)
        AddInvoiceLine(table, 5, "Report Template Modernization", 125D)
        AddInvoiceLine(table, 1, "Implementation Workshop", 950D)
        AddInvoiceLine(table, 3, "Support Retainer - Weekly", 210D)

        Return table
    End Function

    Private Shared Sub AddInvoiceLine(table As DataTable, quantity As Integer, description As String, unitPrice As Decimal)
        Dim lineTotal = quantity * unitPrice
        table.Rows.Add(quantity, description, unitPrice, lineTotal)
    End Sub
End Class
