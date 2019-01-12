namespace core.Tests

open AutoFixture
open Xunit
open string.table.StringTableBuilder

type Person() =
    member val Name = "" with get, set
    member val Age = 0 with get, set

module StringBuilderTestModule =
    open string.table.StringTableModule

    let fixture = new Fixture();
    
    [<Fact>]
    let ``My test``() =
        let rows = fixture.CreateMany<Person>()
        
        let utility = StringTableBuilder.New<Person>()
                        .ImplicitHeader()
                        .Alignment(Alignment.Center)
                        .Border(Border.Full)
                        .AppendAll(rows)
                        .Finanlize()
                        .Build();
    
        let result = utility.Result
    
        Assert.NotEmpty(result)

