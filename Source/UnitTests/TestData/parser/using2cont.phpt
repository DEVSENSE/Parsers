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
        "DirectStFldUse":{"PropertyName":"x","DirectTypeRef":{"ClassName":"MySpace\X"}},
        "ClassConstUse":{"Name":"yConst","DirectTypeRef":{"ClassName":"MySpace\X"}},
        "DirectStFldUse":{"PropertyName":"x","DirectTypeRef":{"ClassName":"test\B"}},  
        "ClassConstUse":{"Name":"bConst","DirectTypeRef":{"ClassName":"test\B"}},
        "TryStmt":{
          "Body":{"BlockStmt":{}},
          "Catches":{
            "CatchItem":{
              "TypeRef":{
                "MultipleTypeRef":{
                  "DirectTypeRef":{"ClassName":"MySpace\X"},
                  "DirectTypeRef":{"ClassName":"test\A"}
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
              "TypeHint":{"DirectTypeRef":{"ClassName":"MySpace\X"}}
            }
          },
          "Body":{"BlockStmt":{}},
          "ReturnType":{"DirectTypeRef":{"ClassName":"MySpace\X"}}
        },
        "InstanceOfEx":{"DirectVarUse":{"VarName":"x"},"DirectTypeRef":{"ClassName":"MySpace\X"}}
      }
    }
  }
}
