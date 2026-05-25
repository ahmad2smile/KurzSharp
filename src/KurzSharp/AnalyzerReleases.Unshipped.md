; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------------------------------------------------
KS001   | KurzSharp | Error   | A type decorated with a KurzSharp attribute must be partial
KS002   | KurzSharp | Warning | A type decorated with a KurzSharp attribute must have properties
KS003   | KurzSharp | Warning | A KurzSharp model has no resolvable EF Core primary key
