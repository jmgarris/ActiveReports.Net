# ActiveReportsEmbeddedViewerDemo

This solution demonstrates how to keep all MESCIUS ActiveReports.NET viewer and report-loading logic inside a VB.NET class library while exposing a simple API that any .NET Framework 4.7.2 WinForms application can call.

The solution file is `ActiveReportsEmbeddedViewerDemo.sln`.

## Projects

- `Company.Reporting.ActiveReports`
  A VB.NET class library that owns embedded `.rdlx` report resources, runtime report configuration, and the modal WinForms viewer popup.
- `ActiveReports.WinFormsExample`
  A thin sample host application that gathers input and calls `ActiveReportLauncher.ShowReport(...)`.

## Embedded RDLX Rules

This solution uses embedded `.rdlx` reports only.

When adding another report:

1. Add the `.rdlx` file under `Company.Reporting.ActiveReports\Reports`.
2. Set its Build Action to `Embedded Resource`.
3. Register the report in `ReportCatalog`.
4. Define the report parameters required by that report.
5. Configure the SQL data source to use a stored procedure.
6. Ensure the stored procedure parameters match the report parameter names with an `@` prefix.
7. Register the report's required parameter names in `ReportCatalog`.
8. Ensure the host application continues to call only `ActiveReportLauncher.ShowReport(...)`.

## Restore Packages

From the repository root:

```powershell
dotnet restore .\ActiveReportsEmbeddedViewerDemo.sln
```

## Build

```powershell
dotnet msbuild .\ActiveReportsEmbeddedViewerDemo.sln /t:Build /p:Configuration=Debug
```

## Run the Sample Host

1. Open `ActiveReportsEmbeddedViewerDemo.sln` in Visual Studio.
2. Set `ActiveReports.WinFormsExample` as the startup project.
3. Build the solution.
4. Run the application.
5. Enter the SQL connection string, RIS name, MBA name, PI number, and select a report.
6. Click `Show Report`.

The example host does not create a viewer, does not load embedded resources, and does not manipulate ActiveReports internals.

## Calling the Library From Another Application

Reference `Company.Reporting.ActiveReports` from another .NET Framework 4.7.2 application and call:

```vb
Dim parameters As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
    {"RISName", "RIS Example"},
    {"MBAName", "MBA Example"},
    {"PINumber", 12345}
}

ActiveReportLauncher.ShowReport(
    owner,
    reportKey:="sample-pi-report",
    sqlConnectionString:="Server=localhost;Database=MyDatabase;Trusted_Connection=True;",
    reportParameters:=parameters)
```

The host application must deploy the reporting class library and the required MESCIUS assemblies with the application.

## Notes

- The sample report resource is `Reports\SamplePIReport.rdlx`.
- The sample RDLX is configured around the stored procedure name `dbo.usp_SamplePIReport`.
- Each report registered in `ReportCatalog` declares its own `RequiredParameterNames`.
- The host application only needs to supply a dictionary containing the names and values required by the selected report.
- The sample report layout shows the passed parameter values directly so you can confirm the runtime injection path.
- Adjust datasource names, dataset names, result fields, and stored procedure details to match your actual report design and database.
- A valid MESCIUS ActiveReports license may be required to build or run the application in your environment.
- Do not store production SQL connection strings in source control.
