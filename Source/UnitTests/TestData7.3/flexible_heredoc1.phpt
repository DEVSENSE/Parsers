<?php
class foo {
    public $bar = <<<EOT
      foo
     bar
    EOT;
}
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamedTypeDecl":{
    "Name":"foo",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "FieldDeclList":{
      "FieldDecl":{
        "Name":"bar",
        "MemberAttributes":"Public",
        "StringLiteral":{
          "Value":"  foo
 bar"
        }
      }
    }
  }
}