<?php

new namespace\A();

namespace MyNamespace;

new namespace\A();

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{"DirectTypeRef":{"ClassName":"A"}},
  "NamespaceDecl":{
    "Name":"MyNamespace",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyNamespace"},
    "Body":{
      "BlockStmt":{
        "NewEx":{"DirectTypeRef":{"ClassName":"MyNamespace\A"}}
      }
    }
  }
}
