<?php
echo "$x[1] $x[2]";
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{
    "ConcatEx":{
      "ItemUse":{"Array":{"DirectVarUse":{"VarName":"x"}},"Index":{"LongIntLiteral":{"Value":"1"}}},
      "StringLiteral":{"Value":""},
      "ItemUse":{"Array":{"DirectVarUse":{"VarName":"x"}},"Index":{"LongIntLiteral":{"Value":"2"}}}
    }
  }
}
