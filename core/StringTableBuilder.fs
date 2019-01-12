namespace string.table

module StringTableBuilder =
    open System.Collections.Immutable
    open System.ComponentModel.DataAnnotations
    open System.Reflection
    open System.Collections.Generic
    open StringTableModule
    open System.Linq

    type StringTableBuilderBuild<'T>(header : Dictionary<PropertyInfo, string>, alignment : Alignment, border: Border, rows : IEnumerable<'T>) =
        member this.Build() =
            (new StringTable<'T>(header, alignment, border, rows)).Init() :> IStringTable

    type StringTableBuilderRows<'T>(header : Dictionary<PropertyInfo, string>, alignment : Alignment, border: Border) =
        let mutable rows = ImmutableList<'T>.Empty

        member this.Append(arg : 'T) =
            rows <- rows.Add(arg)
            this

        member this.AppendAll(arg : IEnumerable<'T>) =
            rows <- rows.Concat(arg).ToImmutableList()
            this

        member this.Finalize<'T>() =
            new StringTableBuilderBuild<'T>(header, alignment, border, rows)

    type StringTableBuilderBorder<'T>(header : Dictionary<PropertyInfo, string>, alignment : Alignment) =
        member this.Border(border : Border) =
            new StringTableBuilderRows<'T>(header, alignment, border)    
    
    type StringTableBuilderAlignment<'T>(header : Dictionary<PropertyInfo, string>) =
        member this.Alignment(alignment : Alignment) =
            new StringTableBuilderBorder<'T>(header, alignment)

    type StringTableBuilderHeader<'T>() =
        member this.ImplicitHeader() =
            let header = typeof<'T>
                             .GetProperties()
                             .ToDictionary((fun x -> x), (fun x -> (x, x.GetCustomAttribute<DisplayAttribute>())))
                             .ToDictionary((fun x -> x.Key), (fun x -> match x.Value with | (a, b) ->
                                                                                             if b <> null then
                                                                                                 b.Name
                                                                                             else
                                                                                                 a.Name))

            new StringTableBuilderAlignment<'T>(header)

        member this.ExplicitHeader(arg : Dictionary<string, string>) =
            let properties = typeof<'T>.GetProperties()
            let header = properties
                             .Where(fun x -> arg.ContainsKey(x.Name))
                             .ToDictionary((fun x -> x), (fun x -> arg.[x.Name]))

            new StringTableBuilderAlignment<'T>(header)

    type StringTableBuilder =
        static member New<'T>() = new StringTableBuilderHeader<'T>()
