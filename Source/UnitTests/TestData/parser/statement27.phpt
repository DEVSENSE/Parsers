<?php
       
"hello $wrold $what[a] $nice->day ${$to + 1} ${be} ${alive[$and]} {$Well}"


?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ConcatEx":{
    "StringLiteral":{"Value":"hello"},
    "DirectVarUse":{"VarName":"wrold"},
    "StringLiteral":{"Value":""},
    "ItemUse":{"Array":{"DirectVarUse":{"VarName":"what"}},"Index":{"StringLiteral":{"Value":"a"}}},
    "StringLiteral":{"Value":""},
    "DirectVarUse":{"VarName":"day","IsMemberOf":{"DirectVarUse":{"VarName":"nice"}}},
    "StringLiteral":{"Value":""},
    "IndirectVarUse":{"BinaryEx":{"Operation":"Add","DirectVarUse":{"VarName":"to"},"LongIntLiteral":{"Value":"1"}}},
    "StringLiteral":{"Value":""},
    "DirectVarUse":{"VarName":"be"},
    "StringLiteral":{"Value":""},
    "ItemUse":{"Array":{"DirectVarUse":{"VarName":"alive"}},"Index":{"DirectVarUse":{"VarName":"and"}}},
    "StringLiteral":{"Value":""},
    "DirectVarUse":{"VarName":"Well"}
  }
}
