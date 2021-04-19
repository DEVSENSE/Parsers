<?php
namespace test{
  use function MySpace\bar as foo;
  
  use MySpace\X as Y;  
  
  use const MySpace\gconst as lconst;
  
  Y::$x;
  Y::yConst;
  
  B::$x; 
  B::bConst;
  namespace\B::$x;
  $x->foo();
  
  try{}
  catch(Y|A $a){}
  
  function xar(Y $a): Y {}
  
  $x instanceof Y;
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
        "UseStatement":{"Kind":"Function","Aliases":{"foo":{}}},
        "UseStatement":{"Kind":"Type","Aliases":{"Y":{}}},
        "UseStatement":{"Kind":"Constant","Aliases":{"lconst":{}}},
        "DirectStFldUse":{"PropertyName":"x","TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}},
        "ClassConstUse":{"Name":"yConst","TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}},
        "DirectStFldUse":{"PropertyName":"x","TranslatedTypeRef":{"ClassName":"test\B","OriginalName":"B"}},  
        "ClassConstUse":{"Name":"bConst","TranslatedTypeRef":{"ClassName":"test\B","OriginalName":"B"}},
        "DirectStFldUse":{"PropertyName":"x","ClassTypeRef":{"ClassName":"test\B"}},
        "DirectFcnCall":{"Name":"foo","OriginalName":"foo","FallbackQualifiedName":"","IsMemberOf":{"DirectVarUse":{"VarName":"x"}}},
        "TryStmt":{
          "Body":{"BlockStmt":{}},
          "Catches":{
            "CatchItem":{
              "TypeRef":{
                "MultipleTypeRef":{
                  "TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"},
                  "TranslatedTypeRef":{"ClassName":"test\A","OriginalName":"A"}
                }
              },
              "Variable":{"DirectVarUse":{"VarName":"a"}},
              "Body":{"BlockStmt":{}}
            }
          }
        },
        "FunctionDecl":{
          "Name":"xar",
          "IsConditional":"False",
          "FormalParams":{
            "FormalParam":{
              "Name":"a",
              "PassedByRef":"False",
              "IsVariadic":"False",
              "TypeHint":{"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}}
            }
          },
          "Body":{"BlockStmt":{}},
          "ReturnType":{"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}}
        },
        "InstanceOfEx":{"DirectVarUse":{"VarName":"x"},"TranslatedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}}
      }
    }
  }
}
