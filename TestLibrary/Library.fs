module TestLibrary

open Persimmon
open Persimmon.Syntax.UseTestNameByReflection

let t = test {
    do! assertEquals 3 4
}