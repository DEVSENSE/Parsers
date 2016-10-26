<?php
       
$x[];
x[0];
$x{5};  
$x[0][];


?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ItemUse":{
    "Array":{"DirectVarUse":{"VarName":"x"}}
  },
  "ItemUse":{
    "Array":{"GlobalConstUse":{"Name":"x","OrigianlName":"x","FallbackName":""}},
    "Index":{"LongIntLiteral":{"Value":"0"}}
  },
  "ItemUse":{
    "Array":{"DirectVarUse":{"VarName":"x"}},
    "Index":{"LongIntLiteral":{"Value":"5"}}
  },
  "ItemUse":{
    "Array":{
      "ItemUse":{
        "Array":{"DirectVarUse":{"VarName":"x"}},
        "Index":{"LongIntLiteral":{"Value":"0"}}
      }
    }
  }
}
