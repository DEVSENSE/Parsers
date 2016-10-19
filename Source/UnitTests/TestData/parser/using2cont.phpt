<?php
namespace test{
  use function MySpace\bar as foo;
  
  use MySpace\X as Y;  
  
  use const MySpace\gconst as lconst;
  
  Y::$x;
  Y::yConst;
  
  B::$x; 
  B::bConst;
  
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
        "DirectStFldUse":{"PropertyName":"x","AliasedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}},
        "ClassConstUse":{"Name":"yConst","AliasedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}},
        "DirectStFldUse":{"PropertyName":"x","ClassTypeRef":{"ClassName":"test\B"}},  
        "ClassConstUse":{"Name":"bConst","ClassTypeRef":{"ClassName":"test\B"}},
        "TryStmt":{
          "Body":{"BlockStmt":{}},
          "Catches":{
            "CatchItem":{
              "TypeRef":{
                "MultipleTypeRef":{
                  "AliasedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"},
                  "ClassTypeRef":{"ClassName":"test\A"}
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
              "TypeHint":{"AliasedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}}
            }
          },
          "Body":{"BlockStmt":{}},
          "ReturnType":{"AliasedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}}
        },
        "InstanceOfEx":{"DirectVarUse":{"VarName":"x"},"AliasedTypeRef":{"ClassName":"MySpace\X","OriginalName":"Y"}}
      }
    }
  }
}
