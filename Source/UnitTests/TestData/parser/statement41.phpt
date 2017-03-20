<?php

new namespace\A();

namespace MyNamespace;

new namespace\A();

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{"ClassTypeRef":{"ClassName":"A"}},
  "NamespaceDecl":{
    "Name":"MyNamespace",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyNamespace"},
    "Body":{
      "BlockStmt":{
        "NewEx":{"ClassTypeRef":{"ClassName":"MyNamespace\A"}}
      }
    }
  }
}
