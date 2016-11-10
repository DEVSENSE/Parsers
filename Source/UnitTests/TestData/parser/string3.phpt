<?php

class x
{
        private $str = "hello
                        world";//multilne strings are left as they are, because the indentation is their part
    
            public $components = array(
                'Session',
                'Auth' => array(
                    'loginAction' => array(
                        'controller' => 'users',
                        'action' => 'login'
                    ),
                    array('authorize' => array('Controller'))
                )
            );
   
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamedTypeDecl":{
    "Name":"x",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "FieldDeclList":{
      "FieldDecl":{"Name":"str","MemberAttributes":"Private","StringLiteral":{"Value":"helloworld"}}
    },
    "FieldDeclList":{
      "FieldDecl":{"Name":"components","MemberAttributes":"Public",
      "ArrayEx":{
        "Item":{"ValueExpr":{"StringLiteral":{"Value":"Session"}}},
        "Item":{"Index":{"StringLiteral":{"Value":"Auth"}},
        "ValueExpr":{"ArrayEx":{
          "Item":{"Index":{"StringLiteral":{"Value":"loginAction"}},"ValueExpr":{
            "ArrayEx":{
              "Item":{"Index":{"StringLiteral":{"Value":"controller"}},"ValueExpr":{"StringLiteral":{"Value":"users"}}},
              "Item":{"Index":{"StringLiteral":{"Value":"action"}},"ValueExpr":{"StringLiteral":{"Value":"login"}}}}}},
              "Item":{"ValueExpr":{
                "ArrayEx":{
                  "Item":{"Index":{"StringLiteral":{"Value":"authorize"}},"ValueExpr":{"ArrayEx":{
                    "Item":{"ValueExpr":{"StringLiteral":{"Value":"Controller"}}}}}}
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}

