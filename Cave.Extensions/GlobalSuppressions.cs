// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Unwanted rule")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Unwanted rule")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1123:Do not place regions within elements", Justification = "Unwanted rule")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Unwanted rule - use regions to categorize")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1205:Partial elements must declare access", Justification = "Unwanted rule - shall be omitted")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:Const field names should begin with upper-case letter", Justification = "Untrue for private")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:Static readonly fields should begin with upper-case letter", Justification = "Untrue for private")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:Access modifier must be declared", Justification = "Unwanted rule - shall be omitted")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Unwanted rule")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace must be followed by blank line", Justification = "Unwanted rule")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:Elements should be separated by blank line", Justification = "Unwanted rule")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File must have header", Justification = "Unwanted rule")]
[assembly: SuppressMessage("Design", "CA1051:Sichtbare Instanzfelder nicht deklarieren", Justification = "Unwanted rule")]
[assembly: SuppressMessage("Globalization", "CA1308:Zeichenfolgen in Großbuchstaben normalisieren", Justification = "Unwanted rule")]
[assembly: SuppressMessage("Reliability", "CA2008:Keine Tasks ohne Übergabe eines TaskSchedulers erstellen", Justification = "Always using default scheduler")]
[assembly: SuppressMessage("Performance", "CA1825:Vermeiden Sie Arrayzuordnungen mit einer Länge von null.", Justification = "Not available at most frameworks")]
