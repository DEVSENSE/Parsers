<?php

{
  /** final class */
  final class MyClass
  {
  } 
        
  /** final abstract class */
  final abstract class MyClass
  {
  }
  
  interface MyInterface
  {
  }
  
  trait MyTrait
  {
  }
}



<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "BlockStmt":{
    "NamedTypeDecl":{"Name":"MyClass","MemberAttributes":"Final","IsConditional":"True","PHPDoc":{"Comment":"final class"}},
    "NamedTypeDecl":{"Name":"MyClass","MemberAttributes":"Error","IsConditional":"True","PHPDoc":{"Comment":"final abstract class"}},
    "NamedTypeDecl":{"Name":"MyInterface","MemberAttributes":"Interface","IsConditional":"True"},
    "NamedTypeDecl":{"Name":"MyTrait","MemberAttributes":"Trait","IsConditional":"True"}
  }
}
