namespace FSharpApiSearch

open System.Text
open System
open System.Collections.Generic
open MessagePack

type internal DotNetType = System.Type

[<MessagePackObject>]
type TypeVariable = {
  [<Key(0)>]
  Name: string
  [<Key(1)>]
  IsSolveAtCompileTime: bool
}

[<MessagePackObject>]
type NamePart =
  | SymbolName of name:string
  | OperatorName of name:string * compiledName:string
  | WithCompiledName of name:string * compiledName:string

[<MessagePackObject>]
type NameItem = {
  [<Key(0)>]
  Name: NamePart
  [<Key(1)>]
  GenericParameters: TypeVariable list
}

type Name = NameItem list


[<RequireQualifiedAccess>]
[<MessagePackObject>]
type VariableSource = Query | Target

[<MessagePackObject>]
type ConcreteType = {
  [<Key(0)>]
  AssemblyName: string
  [<Key(1)>]
  Name: Name
}

[<MessagePackObject>]
type UserInputType = {
  [<Key(0)>]
  Name: Name
}

[<MessagePackObject>]
type Identifier =
  | ConcreteType of ConcreteType
  | UserInputType of UserInputType

[<MessagePackObject>]
type LoadingName = {
  [<Key(0)>]
  AssemblyName: string
  [<Key(1)>]
  RawName: string
  [<Key(2)>]
  MemberName: Name
}
[<MessagePackObject>]
type QueryRange = {
  [<Key(0)>]
  Begin: int
  [<Key(1)>]
  End: int
}

[<MessagePackObject>]
[<CustomEquality; CustomComparison>]
type Position =
  | AtSignature of SignatureId
  | AtQuery of QueryId option * QueryRange
  | Unknown

  // ignore this type on equality and comparison
  override this.Equals(other: obj) =
    match other with
    | :? Position -> true
    | _ -> false
  override this.GetHashCode() = 0

  interface IComparable with
    member this.CompareTo(other: obj) =
      match other with
      | :? Position -> 0
      | _ -> invalidArg "other" "cannot compare"


and [<MessagePackObject>] SignatureId = SignatureId of id:int

and [<MessagePackObject>] QueryId = QueryId of id:int

[<MessagePackObject>]
type LowType =
  | Wildcard of string option * Position
  | Variable of VariableSource * TypeVariable * Position
  | Identifier of Identifier * Position
  | Arrow of Arrow * Position
  | Tuple of TupleType * Position
  | Generic of LowType * LowType list * Position
  | TypeAbbreviation of TypeAbbreviation * Position
  | Delegate of delegateType: LowType * Arrow * Position
  | ByRef of isOut:bool * LowType * Position
  | Subtype of baseType:LowType * Position
  | Choice of original:LowType * LowType list * Position
  | LoadingType of LoadingName * Position

and [<MessagePackObject>] TypeAbbreviation = {
  [<Key(0)>]
  Abbreviation: LowType
  [<Key(1)>]
  Original: LowType
}
and [<MessagePackObject>] TupleType = {
  [<Key(0)>]
  Elements: LowType list
  [<Key(1)>]
  IsStruct: bool
}
and Arrow = LowType list * LowType // parameters and return type


[<MessagePackObject>]
type Accessibility =
  | Public
  | Private
  //| Internal

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type PropertyKind = Get | Set | GetSet

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type MemberKind =
  | Method
  | Property of PropertyKind
  | Field

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type MemberModifier = Instance | Static

[<MessagePackObject>]
type Parameter = {
  [<Key(0)>]
  Type: LowType
  [<Key(1)>]
  Name: string option
  [<Key(2)>]
  IsOptional: bool
  [<Key(3)>]
  IsParamArray: bool
}

type ParameterGroups = Parameter list list
type Function = ParameterGroups * Parameter


[<MessagePackObject>]
type Member = {
  [<Key(0)>]
  Name: string
  [<Key(1)>]
  Kind: MemberKind
  [<Key(2)>]
  GenericParameters: TypeVariable list
  [<Key(3)>]
  Parameters: ParameterGroups
  [<Key(4)>]
  ReturnParameter: Parameter
}

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type TypeDefinitionKind =
  | Class
  | Interface
  | Type
  | Union
  | Record
  | Enumeration

[<MessagePackObject>]
type Constraint =
  | SubtypeConstraints of LowType
  | NullnessConstraints
  | MemberConstraints of MemberModifier * Member
  | DefaultConstructorConstraints
  | ValueTypeConstraints
  | ReferenceTypeConstraints
  | EnumerationConstraints
  | DelegateConstraints
  | UnmanagedConstraints
  | EqualityConstraints
  | ComparisonConstraints

[<MessagePackObject>]
type TypeConstraint = {
  [<Key(0)>]
  Variables: TypeVariable list
  [<Key(1)>]
  Constraint: Constraint
}

[<MessagePackObject>]
type ConstraintStatus =
  | Satisfy
  | NotSatisfy
  | Dependence of TypeVariable list

[<MessagePackObject>]
type FullTypeDefinition = {
  [<Key(0)>]
  Name: Name
  [<Key(1)>]
  FullName: string
  [<Key(2)>]
  AssemblyName: string
  [<Key(3)>]
  Accessibility: Accessibility
  [<Key(4)>]
  Kind: TypeDefinitionKind
  [<Key(5)>]
  BaseType: LowType option
  [<Key(6)>]
  AllInterfaces: LowType list
  [<Key(7)>]
  GenericParameters: TypeVariable list
  [<Key(8)>]
  TypeConstraints: TypeConstraint list
  [<Key(9)>]
  InstanceMembers: Member list
  [<Key(10)>]
  StaticMembers: Member list
  
  [<Key(11)>]
  ImplicitInstanceMembers: Member list  
  [<Key(12)>]
  ImplicitStaticMembers: Member list

  // pre-compute for type constraints
  [<Key(13)>]
  SupportNull: ConstraintStatus
  [<Key(14)>]
  ReferenceType: ConstraintStatus
  [<Key(15)>]
  ValueType: ConstraintStatus
  [<Key(16)>]
  DefaultConstructor: ConstraintStatus
  [<Key(17)>]
  Equality: ConstraintStatus
  [<Key(18)>]
  Comparison: ConstraintStatus
}

[<MessagePackObject>]
type TypeAbbreviationDefinition = {
  [<Key(0)>]
  Name: Name
  [<Key(1)>]
  FullName: string
  [<Key(2)>]
  AssemblyName: string
  [<Key(3)>]
  Accessibility: Accessibility
  [<Key(4)>]
  GenericParameters: TypeVariable list
  [<Key(5)>]
  Abbreviated: LowType
  [<Key(6)>]
  Original: LowType
}

[<MessagePackObject>]
type TypeExtension = {
  [<Key(0)>]
  ExistingType: LowType
  [<Key(1)>]
  Declaration: Name
  [<Key(2)>]
  MemberModifier: MemberModifier
  [<Key(3)>]
  Member: Member
}

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type ApiKind =
  | ModuleValue
  | Constructor
  | Member of MemberModifier * MemberKind
  | TypeExtension of MemberModifier * MemberKind
  | ExtensionMember
  | UnionCase
  | ModuleDefinition
  | TypeDefinition
  | TypeAbbreviation
  | ComputationExpressionBuilder

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type ActivePatternKind =
  | ActivePattern
  | PartialActivePattern

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type UnionCaseField = {
  [<Key(0)>]
  Name: string option
  [<Key(1)>]
  Type: LowType
}

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type UnionCase = {
  [<Key(0)>]
  DeclaringType: LowType
  [<Key(1)>]
  Name: string
  [<Key(2)>]
  Fields: UnionCaseField list
}

[<MessagePackObject>]
type ModuleDefinition = {
  [<Key(0)>]
  Name: Name
  [<Key(1)>]
  AssemblyName: string
  [<Key(2)>]
  Accessibility: Accessibility
}

[<MessagePackObject>]
type ComputationExpressionSyntax = {
  [<Key(0)>]
  Syntax: string
  [<Key(1)>]
  Position: Position
}

[<MessagePackObject>]
type ComputationExpressionBuilder = {
  [<Key(0)>]
  BuilderType: LowType
  [<Key(1)>]
  ComputationExpressionTypes: LowType list
  [<Key(2)>]
  Syntaxes: ComputationExpressionSyntax list
}

[<RequireQualifiedAccess>]
[<MessagePackObject>]
type ApiSignature =
  | ModuleValue of LowType
  | ModuleFunction of Function
  | ActivePatten of ActivePatternKind * Function
  | InstanceMember of LowType * Member
  | StaticMember of LowType * Member
  | Constructor of LowType * Member
  | ModuleDefinition of ModuleDefinition
  | FullTypeDefinition of FullTypeDefinition
  | TypeAbbreviation of TypeAbbreviationDefinition
  /// F# Type Extension
  | TypeExtension of TypeExtension
  /// C# Extension Member
  | ExtensionMember of Member
  | UnionCase of UnionCase
  | ComputationExpressionBuilder of ComputationExpressionBuilder
