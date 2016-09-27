<?php
$a = &$b;  
$a -= $b; 
$a *= $b; 
$a **= $b; 
$a /= $b; 
$a .= $b; 
$a %= $b; 
$a &= $b;   
$a |= $b; 
$a ^= $b; 
$a <<= $b; 
$a >>= $b; 

++$a;
--$a;
$a--; 

$a && $b; 
$a || $b; 
$a and $b; 
$a or $b;
$a xor $b; 
$a & $b; 
$a | $b;
$a ^ $b;  

$a - $b; 
$a ** $b;  
$a / $b;  
$a % $b;  
$a << $b;
$a >> $b; 

+$b;
-$b; 
!$b;
~$b;   

$a === $b; 
$a !== $b; 
$a == $b; 
$a != $b;
  
$a < $b;  
$a >= $b;
$a <=> $b;  
$a instanceof B;  
$a? : $b;

(int)$a;
(double)$a;
(string)$a;
(array)$a;
(object)$a;
(bool)$a;
(unset)$a;

@$a;  
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "RefAssignEx":{"Operation":"AssignRef","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignSub","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignMul","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignPow","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignDiv","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignAppend","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignMod","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignAnd","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignOr","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignXor","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignShiftLeft","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "ValueAssignEx":{"Operation":"AssignShiftRight","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  
  "IncDecEx":{"Inc":"True","Post":"False","DirectVarUse":{"VarName":"a"}},
  "IncDecEx":{"Inc":"False","Post":"False","DirectVarUse":{"VarName":"a"}},
  "IncDecEx":{"Inc":"False","Post":"True","DirectVarUse":{"VarName":"a"}},
  
  "BinaryEx":{"Operation":"And","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Or","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"And","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Or","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Xor","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"BitAnd","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"BitOr","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"BitXor","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Sub","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Pow","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Div","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Mod","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"ShiftLeft","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"ShiftRight","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  
  "UnaryEx":{"Operation":"Plus","DirectVarUse":{"VarName":"b"}},
  "UnaryEx":{"Operation":"Plus","DirectVarUse":{"VarName":"b"}},
  "UnaryEx":{"Operation":"LogicNegation","DirectVarUse":{"VarName":"b"}},
  "UnaryEx":{"Operation":"BitNegation","DirectVarUse":{"VarName":"b"}},
  
  "BinaryEx":{"Operation":"Identical","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"NotIdentical","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Equal","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"NotEqual","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"LessThan","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"GreaterThanOrEqual","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "BinaryEx":{"Operation":"Spaceship","DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
  "InstanceOfEx":{"DirectVarUse":{"VarName":"a"},"DirectTypeRef":{"ClassName":"B"}},
  "ConditionalEx":{"CondExpr":{"DirectVarUse":{"VarName":"a"}},"TrueExpr":{},"FalseExpr":{"DirectVarUse":{"VarName":"b"}}},
  
  "UnaryEx":{"Operation":"Int64Cast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"DoubleCast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"StringCast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"ArrayCast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"ObjectCast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"BoolCast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"UnsetCast","DirectVarUse":{"VarName":"a"}},
  "UnaryEx":{"Operation":"AtSign","DirectVarUse":{"VarName":"a"}}
}
