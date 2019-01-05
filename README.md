# fsharp-string-table
Simple string table structure in F#

```fsharp
let rows = fixture.CreateMany<Person>()

let utility = StringTableBuilder.New<Person>()
                        .ImplicitHeader()
                        .Alignment(Alignment.Left)
                        .Border(Border.Full)
                        .AppendAll(rows)
                        .Finanlize()
                        .Build();
    
let result = utility.Result

(*
---------------------------------------------------------------------------------------
| Name                                     | Age                                      |
---------------------------------------------------------------------------------------
| Namede445523-a705-4560-94af-84bbee14f637 | 216                                      |
| Namefb98ccbe-5fc5-4d1d-ad18-509ac41abb2e | 245                                      |
| Name8a635032-d269-4f7e-b8fc-f78c8668168c | 42                                       |
---------------------------------------------------------------------------------------
*)
 ```
 
 ### Options:
 - Border: `Full | Minimal`
 - Alignment: `Right | Left`
 
 ## Notes:
 - If `DisplayAttribute` is provided on property, it will be used instead as header in case of `ImplicitHeader`
