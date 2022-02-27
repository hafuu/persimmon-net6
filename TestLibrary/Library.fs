module TestLibrary

open Persimmon
open Persimmon.Syntax.UseTestNameByReflection
open Persimmon.MuscleAssert

let testOption = test {
    do! assertEquals (Some 4) (None)
}

let testList = test {
    do! assertEquals [ 1 ] [ 2; 3 ]
}