namespace string.table

module StringTable =
    open System.Reflection
    open System
    open System.Linq
    open System.Collections.Generic

    type Alignment = | Left | Right

    [<StructuredFormatDisplay("{AsString}")>]
    type StringTable<'T>(header : Dictionary<PropertyInfo, string>,  alignment : Alignment, items : IEnumerable<'T>) =
        member this.Space() = ' '.ToString()
        member this.Delimiter() = '|'
        member this.Empty() = '-'
        member this.NewLine() = Environment.NewLine

        member this.RepeatToLength(value : string, length : int) : string =
            if length < value.Length then
                value.Substring(0, length)
            else
                let str = value
                while (str.Length * 2) < length do
                    str = str + str |> ignore

                str + str.Substring(0, length - str.Length)

        member this.Repeat(value : string, count : int) =
            String.Join(value, Enumerable.Range(0, count + 1))

        member this.PadRight(value : string, padding : int) =
            this.Repeat(this.Space(), padding) + value

        member this.PadLeft(value : string, padding : int) =
            value + this.Repeat(this.Space(), padding)

        member this.FormatCell(value : string, width : int, alignment : Alignment) =
            match alignment with
                | Alignment.Right -> this.PadLeft(value, width)
                | Alignment.Left -> this.PadRight(value, width)

        member this.Line(width: int) =
            String.Join(this.Empty(), Enumerable.Range(0, width + 1))

        member this.ItemToList(item: 'T) =
            header.Select(fun x -> x.Key.GetValue(item).ToString())
        
        member this.FormatRow(item : 'T) =
            String.Join(this.Delimiter(), this.ItemToList(item))

        override this.ToString() =
            let count = header.Count
            let headerFields = header.Values
            let itemsAsStringList = items.Select(fun x -> this.ItemToList(x))
            let maxWidth = Math.Max(headerFields.Max(fun x -> x.Length), itemsAsStringList.Max(fun x -> x.Max(fun y -> y.Length)))
            
            ""