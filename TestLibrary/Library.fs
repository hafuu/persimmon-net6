module TestLibrary

open Persimmon
open Persimmon.Syntax.UseTestNameByReflection
open Persimmon.MuscleAssert
open System

let testOption = test {
    do! assertEquals (Some 4) (None)
}

let testList = test {
    do! assertEquals [ 1 ] [ 2; 3 ]
}
let nullTest = test {
    do! assertEquals [ (Some 4, 4) ] [  ]
}

open FSharpApiSearch

let expected =
  [ApiSignature.StaticMember
  (Identifier
     (ConcreteType
        { AssemblyName = "CSharpLoadTestAssembly"
          Name =
           [{ Name = SymbolName "Operators"
              GenericParameters = [] };
            { Name = SymbolName "CSharpLoadTestAssembly"
              GenericParameters = [] }] }, Unknown),
   { Name = "op_Addition"
     Kind = MemberKind.Method
     GenericParameters = []
     Parameters =
      [[{ Type =
           Identifier
             (ConcreteType
                { AssemblyName = "CSharpLoadTestAssembly"
                  Name =
                   [{ Name = SymbolName "Operators"
                      GenericParameters = [] };
                    { Name = SymbolName "CSharpLoadTestAssembly"
                      GenericParameters = [] }] }, Unknown)
          Name = Some "x"
          IsOptional = false
          IsParamArray = false };
        { Type =
           Identifier
             (ConcreteType
                { AssemblyName = "CSharpLoadTestAssembly"
                  Name =
                   [{ Name = SymbolName "Operators"
                      GenericParameters = [] };
                    { Name = SymbolName "CSharpLoadTestAssembly"
                      GenericParameters = [] }] }, Unknown)
          Name = Some "y"
          IsOptional = false
          IsParamArray = false }]]
     ReturnParameter =
      { Type =
         Identifier
           (ConcreteType
              { AssemblyName = "CSharpLoadTestAssembly"
                Name =
                 [{ Name = SymbolName "Operators"
                    GenericParameters = [] };
                  { Name = SymbolName "CSharpLoadTestAssembly"
                    GenericParameters = [] }] }, Unknown)
        Name = None
        IsOptional = false
        IsParamArray = false } })]

let complex = test {
    do! assertEquals expected []
}