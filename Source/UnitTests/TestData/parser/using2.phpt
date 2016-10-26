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
  
  class A extends Y implements Y,\Y {}
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
        "DirectFcnCall":{"Name":"MySpace\bar","OrigianlName":"foo","FallbackQualifiedName":""},
        "DirectFcnCall":{"Name":"test\define","OrigianlName":"define","FallbackQualifiedName":"define"},
        "NewEx":{"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}},
        "GlobalConstUse":{"Name":"MySpace\gconst","OrigianlName":"lconst","FallbackName":""},
        "GlobalConstUse":{"Name":"test\clsconst","OrigianlName":"clsconst","FallbackName":"clsconst"},
        "NamedTypeDecl":{"Name":"A","MemberAttributes":"Public","IsConditional":"False","BaseClassName":{"Name":"MySpace\X","OriginalName":"Y"},"ImplementsList":{"Name":"MySpace\X","Name":"Y"}}
      }
    }
  }
}



