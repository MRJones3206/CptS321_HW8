// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "I will be using my own format.")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Enough of this insanity.")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "It just works.", Scope = "type", Target = "~T:CptS321.SpreadsheetCellAbstract")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Required by project.", Scope = "member", Target = "~F:CptS321.SpreadsheetCellAbstract.value")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Required by project.", Scope = "member", Target = "~F:CptS321.SpreadsheetCellAbstract.text")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "It just works.", Scope = "type", Target = "~T:CptS321.SpreadsheetCellAbstract")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:Statement should not use unnecessary parenthesis", Justification = "Too parinoid to not use them this one time.", Scope = "member", Target = "~M:CptS321.Spreadsheet.UpdateCells(System.Int32,System.Int32,System.String,System.String)")]
