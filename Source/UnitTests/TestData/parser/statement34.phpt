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
    "NamedTypeDecl":{"Name":"MyClass","MemberAttributes":"Final","IsConditional":"False","PHPDoc":{"Comment":"final class"}},
    "NamedTypeDecl":{"Name":"MyClass","MemberAttributes":"Multiple","IsConditional":"False","PHPDoc":{"Comment":"final abstract class"}},
    "NamedTypeDecl":{"Name":"MyInterface","MemberAttributes":"Interface","IsConditional":"False"},
    "NamedTypeDecl":{"Name":"MyTrait","MemberAttributes":"Trait","IsConditional":"False"}
  }
}
