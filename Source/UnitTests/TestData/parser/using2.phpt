<?php
namespace test{
  use function MySpace\bar as foo;
  
  use MySpace\X as Y, MySpace\X;  
  
  use const MySpace\gconst as lconst;
  
  foo(); 
  define();
  new Y();  
  lconst;
  clsconst;
  
  class A extends X implements Y,\Y {}
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
      "Aliases":{"Y":"MySpace\X","X":"MySpace\X"},
      "ConstantAliases":{"lconst":"MySpace\gconst"},
      "FunctionAliases":{"foo":"MySpace\bar"}
    },
    "Body":{
      "BlockStmt":{
        "UseStatement":{"Kind":"Function","Aliases":{"foo":{}}},
        "UseStatement":{"Kind":"Type","Aliases":{"Y":{},"X":{}}},
        "UseStatement":{"Kind":"Constant","Aliases":{"lconst":{}}},
        "DirectFcnCall":{"Name":"MySpace\bar","OriginalName":"foo","FallbackQualifiedName":""},
        "DirectFcnCall":{"Name":"test\define","OriginalName":"define","FallbackQualifiedName":"define"},
        "NewEx":{"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}},
        "GlobalConstUse":{"Name":"MySpace\gconst","OriginalName":"lconst","FallbackName":""},
        "GlobalConstUse":{"Name":"test\clsconst","OriginalName":"clsconst","FallbackName":"clsconst"},
        "NamedTypeDecl":{"Name":"A","MemberAttributes":"Public","IsConditional":"False",
          "BaseClassName":{"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"X"}},
          "ImplementsList":{"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"},"ClassTypeRef":{"ClassName":"Y"}}
        }
      }
    }
  }
}



