<?php
namespace test{
  use function MySpace\bar as foo;
  
  use MySpace\X as Y;  
  
  use const MySpace\gconst as lconst;
  
  foo(); 
  define();
  new Y();  
  lconst;
  clsconst;
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamespaceDecl":{
    "Name":"test",
    "SimpleSyntax":"False",
    "NamingContext":{
      "Namespace":"test",
      "Aliases":{"Y":"MySpace\X"},
      "ConstantAliases":{"lconst":"MySpace\gconst"},
      "FunctionAliases":{"foo":"MySpace\bar"}
    },
    "Body":{
      "BlockStmt":{
        "DirectFcnCall":{"Name":"MySpace\bar","FallbackQualifiedName":""},
        "DirectFcnCall":{"Name":"test\define","FallbackQualifiedName":"define"},
        "NewEx":{"DirectTypeRef":{"ClassName":"Y"}},
        "GlobalConstUse":{"Name":"MySpace\gconst","FallbackName":""},
        "GlobalConstUse":{"Name":"test\clsconst","FallbackName":"clsconst"}
      }
    }
  }
}

