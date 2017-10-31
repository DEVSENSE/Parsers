<?php
(-5)->new = 5;
?>
<<<TEST>>>

"GlobalCode":
{
  "NamingContext":{},
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{
      "VarName":"new",
      "IsMemberOf":{"UnaryEx":{"Operation":"Minus","LongIntLiteral":{"Value":"5"}}}
    },
    "LongIntLiteral":{"Value":"5"}
  }
}
