<?php
       
$arr = [
    1 => "a",
    "hello",
    1.5 => &$x,
    &$x,
    true => list( 1 => "a" ),
    list( 1 => "a" ),
];


?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{"Operation":"AssignValue",
  "DirectVarUse":{"VarName":"arr"},
  "ArrayEx":{
    "Item":{"Index":{"LongIntLiteral":{"Value":"1"}},"ValueExpr":{"StringLiteral":{"Value":"a"}}},
    "Item":{"ValueExpr":{"StringLiteral":{"Value":"hello"}}},
    "Item":{"Index":{"DoubleLiteral":{"Value":"1.5"}},"RefToGet":{"DirectVarUse":{"VarName":"x"}}},
    "Item":{"RefToGet":{"DirectVarUse":{"VarName":"x"}}},
    "Item":{"Index":{"GlobalConstUse":{"Name":"true"}},"ValueExpr":{
      "ListEx":{
        "Item":{"Index":{"LongIntLiteral":{"Value":"1"}},"ValueExpr":{"StringLiteral":{"Value":"a"}}}}
      }
    },
    "Item":{"ValueExpr":{"ListEx":{"Item":{"Index":{"LongIntLiteral":{"Value":"1"}
  },
  "ValueExpr":{"StringLiteral":{"Value":"a"}}}}}}}}
}
