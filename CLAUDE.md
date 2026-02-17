# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CAST ReportGenerator generates Word, PowerPoint, and Excel reports from templates using data from the CAST AIP REST API. It provides both CLI automation (Console) and GUI (WPF) workflows for report generation.

**Current Version:** 1.31.0
**Target Framework:** .NET 8.0
**Language Version:** C# 7.3

## Build Commands

```bash
# Build the solution
dotnet build -c Release

# Run all tests
dotnet test -l trx

# Build a specific project
dotnet build CastReporting.Console.Core/CastReporting.Console.Core.csproj -c Release

# Run the console application
dotnet run --project CastReporting.Console.Core/CastReporting.Console.Core.csproj

# Run the WPF application (Windows only)
dotnet run --project CastReporting.UI.WPF.V2/CastReporting.UI.WPF.Core.csproj
```

## Test Commands

```bash
# Run all tests with detailed output
dotnet test -l trx

# Run tests for a specific project
dotnet test CastReporting.UnitTest.Core/CastReporting.UnitTest.Core.csproj

# Run a specific test class
dotnet test --filter "FullyQualifiedName~ComputingTest"

# Run tests with verbose logging
dotnet test --logger "console;verbosity=detailed"
```

Test results are output in TRX format to the `TestResults/` directory.

## Architecture Overview

The solution follows a **layered architecture** with clear separation of concerns:

### Layer Structure

```
Domain Layer (CastReporting.Domain.Core)
  ↑
Mediation Layer (CastReporting.Mediation.Core)
  ↑
Data Access Layer (CastReporting.Repositories.Core)
  ↑
Business Logic Layer (CastReporting.BLL.Core + CastReporting.BLL.Computing.Core)
  ↑
Presentation Layer (CastReporting.Reporting.Core)
  ↑
Application Layer (CastReporting.Console.Core + CastReporting.UI.WPF.Core)
```

### Project Dependencies

- **Domain.Core**: Core domain models (Application, Snapshot, QualityIndicators, Violations, etc.)
- **Mediation.Core**: REST API client proxy (`ICastProxy`) for CAST AIP integration
- **Repositories.Core**: Data access abstraction (`ICastRepsitory`) over the REST API
- **BLL.Core**: Business logic for aggregating and computing report data
- **BLL.Computing.Core**: Advanced computations for quality metrics and technical debt
- **Reporting.Core**: Report generation engine using OpenXML libraries
- **Console.Core**: CLI application entry point
- **UI.WPF.Core**: Windows desktop GUI application
- **Util.Core**: Cross-cutting utilities (logging, serialization, etc.)

### Report Generation Flow

1. User selects a template (DOCX, PPTX, or XLSX)
2. BLL queries repositories for application/snapshot data
3. Repositories call CAST REST API via Mediation layer
4. Data aggregated into `ReportData` model
5. `BuilderFactory` creates appropriate document builder (Word/PowerPoint/Excel)
6. Template processing generates content blocks (tables, charts, metrics)
7. Output saved to configured path (optional PDF conversion on Windows)

## Key Domain Concepts

### Application Analysis Hierarchy

- **CastDomain**: Top-level container for CAST analysis data
- **Application**: A software application being analyzed
- **Snapshot**: Point-in-time analysis of an application (contains quality metrics, violations, sizing)
- **Module**: Component within an application
- **Transaction**: Business transaction analysis

### Quality & Compliance

- **QIBusinessCriteria**: Business-focused quality metrics (Security, Performance, Robustness, etc.)
- **QITechnicalCriteria**: Technical quality metrics
- **QIQualityRules**: Specific quality rules with pass/fail criteria
- **QIQualityDistribution**: Distribution of quality grades (1-4 stars)
- **Violation**: Code violation with location bookmarks
- **StandardTag**: Industry standards mapping (OWASP, CWE, PCI-DSS, STIG)
- **ActionPlan**: Remediation plans for quality issues

### Sizing & Technical Debt

- **SizingMeasure**: Size metrics (LOC, function points, code lines)
- **OmgTechnicalDebt**: OMG-compliant technical debt calculations
- **IfpugFunction**: IFPUG function point analysis

## Configuration Files

### Application Settings

`CastReportingSetting.xml` - Main configuration file:
- Application size thresholds (Small: 200K, Medium: 500K, Large: 1M LOC)
- Quality grade thresholds (VeryLow: 2.0, Low: 2.8, Medium: 3.2, Good: 3.5)
- Template and output paths
- Culture/language settings (en-US, zh-Hans, DE-de, ES-es, FR-fr, IT-it)
- CAST REST API connections (URL, credentials)
- CAST Extend URL (https://extend.castsoftware.com)

### Console Parameters

`CastReporting.Console.Core/Parameters/Parameters.xml` - CLI argument configuration
- Uses XML schema validation (`CastReportSchema.xsd`)
- Parsed by `CRArgumentReader.cs`

### Logging

`log4net.config` - Log4Net configuration
- Logs stored in `{AppPath}/Logs/`

## Report Templates

Templates are located in `CastReporting.Reporting.Core/Templates/`:
- `Application/` - Application-level reports (compliance, sizing, legacy)
- `Portfolio/` - Portfolio-level reports
- Supports multiple languages (zh-Hans Chinese localization available)

Templates use OpenXML format with special block attributes for dynamic content generation.

## CAST REST API Integration

### Connection Configuration

```csharp
WSConnection {
    string Url              // CAST AIP REST API base URL
    string Username         // Username or API key
    string Password         // Password (optional if using API key)
    bool ApiKey             // True if using API key authentication
    ServerCertificateValidation // SSL certificate validation mode
}
```

### Key Repository Methods

- `GetDomains()` - List all CAST domains
- `GetApplicationsByDomain()` - Get applications in a domain
- `GetSnapshotsByApplication()` - Get snapshots for an application
- `GetResultsQualityIndicators()` - Fetch quality metrics
- `GetRulesViolations()` - Get violations by quality criteria
- `GetActionPlanBySnapshot()` - Get remediation action plans
- `GetQualityStandardsTagsDoc()` - Get standards documentation

## Development Notes

### Entry Points

**Console Application:**
- Entry point: `CastReporting.Console.Core/Program.cs:Main()`
- Supports XML-based parameters and command-line arguments
- Outputs Word/PowerPoint/Excel reports
- Optional PDF conversion (requires Microsoft Office Interop - Windows only)

**WPF Application:**
- Entry point: `CastReporting.UI.WPF.V2/App.xaml.cs`
- Windows desktop GUI for interactive report generation
- Uses same BLL and reporting engine as console

### Document Generation

The reporting engine uses:
- **DocumentFormat.OpenXml** for Word/Excel manipulation
- **OpenXmlPowerTools-NetStandard** for PowerPoint manipulation
- **Microsoft.Office.Interop** for PDF conversion (Windows only)

Document builders implement `IDocumentBuilder` interface:
- `WordDocumentBuilder` - DOCX generation
- `PowerpointDocumentBuilder` - PPTX generation
- `ExcelDocumentBuilder` - XLSX generation

### Test Structure

Tests are organized by layer in `CastReporting.UnitTest.Core/`:
- `BO/` - Business logic tests
- `DAL/` - Data access and serialization tests
- `Reporting/Graph/` - Chart generation tests (Bubble, Bar, Line, Pie, etc.)
- `Reporting/Text/` - Text block generation tests
- `Data/` - Test fixtures (JSON files with sample CAST API responses)

## Building Installers

The full build pipeline (`_build/build_report_generator.bat`) creates:
1. Compiled binaries (Release configuration)
2. Windows installer using InnoSetup (`Setup/setup.iss`)
3. Code-signed executables
4. Two NuGet packages:
   - `com.castsoftware.aip.reportgenerator` - Windows installer
   - `com.castsoftware.aip.reportgeneratorfordashboard` - CLI package for AIP Dashboard

## Common Patterns

### Factory Pattern
`BuilderFactory` creates document builders based on template type (DOCX/PPTX/XLSX)

### Repository Pattern
`ICastRepsitory` abstracts data access, implemented by `CastRepository`

### Adapter/Proxy Pattern
`ICastProxy` wraps HTTP client calls to CAST REST API with authentication

### Strategy Pattern
Different document builders for different output formats
