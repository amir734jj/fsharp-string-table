namespace string.table

module StringTableModule =
    open System.Reflection
    open System
    open System.Linq
    open System.Collections.Generic

    type Alignment = | Left | Right
    type Border = | Full | Minimal

    type IStringTable =
        abstract Result : string

    [<StructuredFormatDisplay("{AsString}")>]
    type StringTable<'T>(header : Dictionary<PropertyInfo, string>, alignment : Alignment, border : Border, items : IEnumerable<'T>) =
        let mutable result = ""

        interface IStringTable with
            member this.Result = result

        member this.Space() = ' '.ToString()
        member this.Delimiter() = '|'
        member this.EmptyChar() = '-'
        member this.Blank() = String.Empty
        member this.NewLine() = Environment.NewLine

        member this.BasicAlignment(value : string) =
            String.Format(" {0} ", value)

        member this.Repeat(value : string, count : int) =
            if count > 0 then
                String.Join(value, Enumerable.Range(0, count + 1).Select(fun _ -> this.Blank()))
            else
                this.Blank()

        member this.PadRight(value : string, padding : int) =
            this.Repeat(this.Space(), padding) + value

        member this.PadLeft(value : string, padding : int) =
            value + this.Repeat(this.Space(), padding)

        member this.FormatCell(value : string, width : int) =
            match alignment with
                | Alignment.Right -> this.PadLeft(value, width - value.Length)
                | Alignment.Left -> this.PadRight(value, width - value.Length)

        member this.Line(width : int) =
            String.Join(this.EmptyChar(), Enumerable.Range(0, width + 1).Select(fun _ -> this.Blank())) + this.NewLine()

        member this.ItemToList(item : 'T) =
            header.Select(fun x -> this.BasicAlignment(x.Key.GetValue(item).ToString()))

        member this.FormatRow(row : IEnumerable<string>, width : int) =
            String.Join(this.Delimiter(), row.Select(fun x -> this.FormatCell(x, width)))

        member this.Init() =
            let header = header.ToDictionary((fun x -> x.Key), (fun x -> this.BasicAlignment(x.Value)))
            let count = header.Count
            let headerFields = header.Values
            let itemsAsStringList = items.Select(fun x -> this.ItemToList(x))
            let maxWidth = Math.Max(headerFields.Max(fun x -> x.Length), itemsAsStringList.Max(fun x -> x.Max(fun y -> y.Length)))

            let mutable headerStr = this.FormatRow(headerFields, maxWidth)

            headerStr <- match border with
                            | Border.Full -> this.Delimiter().ToString() + headerStr + this.Delimiter().ToString()
                            | _ -> headerStr
                            
            let fullWidth = headerStr.Length            

            let str = (match border with
                        | Border.Full -> this.Line(fullWidth)
                        | Border.Minimal -> this.Blank()) + headerStr

            result <- str + this.NewLine() + this.Line(fullWidth) + String.Join(this.NewLine(), itemsAsStringList.Select(fun x ->
                            let temp = this.FormatRow(x, maxWidth)

                            match border with
                                | Border.Full ->
                                    this.Delimiter().ToString() + temp + this.Delimiter().ToString()
                                | _ -> temp
                ))
                + match border with
                            | Border.Full -> this.NewLine() + this.Line(fullWidth)
                            | _ -> this.Blank()

            this
