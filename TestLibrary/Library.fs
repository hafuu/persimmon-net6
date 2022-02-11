module TestLibrary

open Persimmon
open Persimmon.Syntax.UseTestNameByReflection

let t = test {
    do! assertEquals 4 4
}