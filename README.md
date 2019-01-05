# fsharp-string-table
Simple string table structure in F#

```fsharp
let rows = fixture.CreateMany<Person>()

StringTableBuilder.New<Person>()
                        .ImplicitHeader()
                        .Alignment(Alignment.Right)
                        .Border(Border.Full)
                        .AppendAll(rows)
                        .Finanlize()
                        .Build();
    
let result = utility.Result
 ```
 
 ### Options:
 - Border: `Full | Minimal`
 - Alignment: `Right | Left`
 
 ## Notes:
 - If `DisplayAttribute` is provided on property, it will be used instead as header in case of `ImplicitHeader`
