﻿namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("compiler.api.core")>]
[<assembly: AssemblyProductAttribute("Compiler")>]
[<assembly: AssemblyDescriptionAttribute("A markdown to RDF compiler")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
    let [<Literal>] InformationalVersion = "1.0"
