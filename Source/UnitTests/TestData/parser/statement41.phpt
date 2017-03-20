<?php

new namespace\A();

namespace MyNamespace;

new namespace\A();

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{"ByReference":"False","ClassTypeRef":{"ClassName":"A"}},
  "NamespaceDecl":{
    "Name":"MyNamespace",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyNamespace"},
    "Body":{
      "BlockStmt":{
        "NewEx":{"ByReference":"False","ClassTypeRef":{"ClassName":"MyNamespace\A"}}
      }
    }
  }
}
