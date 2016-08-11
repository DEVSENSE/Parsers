<?php
       
list($a, $b) = 1;


?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{"IndirectTypeRef":{
    "DirectVarUse":{"VarName":"x"}}
  },
  "NewEx":{"IndirectTypeRef":{
    "ItemUse":{
      "Array":{"DirectVarUse":{"VarName":"x"}}}
    }
  },
  "NewEx":{"IndirectTypeRef":{
    "ItemUse":{
      "Array":{"DirectVarUse":{"VarName":"x"}},
      "Index":{"LongIntLiteral":{"Value":"1"}}}
    }
  },
  "NewEx":{"IndirectTypeRef":{
    "DirectVarUse":{"VarName":"y","IsMemberOf":{"DirectVarUse":{"VarName":"x"}}}}
  },
  "NewEx":{"IndirectTypeRef":{
    "DirectStFldUse":{"PropertyName":"y","DirectTypeRef":{"ClassName":"X"}}}
  },
  "NewEx":{"IndirectTypeRef":{
    "DirectStFldUse":{"PropertyName":"y","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}}}
  }
}
