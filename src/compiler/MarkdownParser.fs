module compiler.Markdown

open compiler.Domain
open compiler.ContentHandle
open compiler.YamlParser
open System.Text.RegularExpressions
open FSharp.Markdown
open FSharp.Data

let private extract pattern input =
  let m = Regex.Match(input,pattern) 
  m.Groups.[1].Value

let rec removeAnchors html =
  let m = Regex.Match(html, "<a .*?>")
  match m.Success with
  | true -> removeAnchors (html.Replace(m.Groups.[0].Value,"").Replace("</a>",""))
  | false -> html

let private extractAbstract (html:string) = 
  let findParagraphWithAbstract () = 
    try
      let h = HtmlDocument.Parse(html)
      let p = h.Descendants ["p"] |> Seq.head 
      p.ToString()
    with _ -> ""

  findParagraphWithAbstract ()
  |> removeAnchors

let private extractAnnotations (markdown:MarkdownDocument) = 
  let found = Seq.item 0 markdown.Paragraphs
  match found with
    | CodeBlock (text,_,_) -> text
    | _ -> ""

let private convertToVocab {Name = name; Fields = fields} = {Vocab = name; Terms = fields}

let private HandleNoPositionalIdAnnotationError =
  printfn "[Error] A statement was missing the PositionalId annotation"
  ""

let private extractQSandSTNumbers annotation =
  match annotation with
    | Some annotation -> annotation.Terms.Head.Replace("-", "/")
    | None -> HandleNoPositionalIdAnnotationError

let private removeText (a:string) =
  a.Replace("qs","").Replace("st","")

let private splitPositionalId (positionalId:string) =
  positionalId.Split [|'/'|]

let private standardAndStatementNumbers id = 
  match id with
  | "" -> [|"0";"0"|]
  | _ -> id |> removeText |> splitPositionalId

let extractStatement (contentHandle, html) =
  let markdown = Markdown.Parse(contentHandle.Content)

  let abs = extractAbstract html
  let annotations = markdown
                    |> extractAnnotations 
                    |> parseYaml
                    |> List.map convertToVocab
  
  let id = annotations
            |> List.tryFind (fun x -> x.Vocab.Equals("PositionalId"))
            |> extractQSandSTNumbers

  let standardId = (standardAndStatementNumbers id).[0] |> System.Int32.Parse
  let statementId = (standardAndStatementNumbers id).[1] |> System.Int32.Parse 

  let title = sprintf "Quality statement %d from quality standard %d" statementId standardId

  {Id = contentHandle.Thing
   Title = title 
   Abstract = abs 
   StandardId = standardId
   StatementId = statementId
   Annotations = annotations
   Content = contentHandle.Content
   Html = html}
