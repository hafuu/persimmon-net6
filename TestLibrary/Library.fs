module TestLibrary

open Persimmon
open Persimmon.Syntax.UseTestNameByReflection
open Persimmon.MuscleAssert

let t = test {
    do! assertEquals 4 4
}