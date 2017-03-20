<?php
       
new $x;   
new $x[];
new $x{1};
new $x->y;  
new X::$y;
new $x::$y;


?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{
    "ByReference":"False",
    "IndirectTypeRef":{
      "DirectVarUse":{"VarName":"x"}
    }
  },
  "NewEx":{
    "ByReference":"False",
    "IndirectTypeRef":{
      "ItemUse":{
        "Array":{"DirectVarUse":{"VarName":"x"}}
      }
    }
  },
  "NewEx":{
    "ByReference":"False",
    "IndirectTypeRef":{
      "ItemUse":{
        "Array":{"DirectVarUse":{"VarName":"x"}},
        "Index":{"LongIntLiteral":{"Value":"1"}}
      }
    }
  },
  "NewEx":{"ByReference":"False","IndirectTypeRef":{
    "DirectVarUse":{"VarName":"y","IsMemberOf":{"DirectVarUse":{"VarName":"x"}}}}
  },
  "NewEx":{"ByReference":"False","IndirectTypeRef":{
    "DirectStFldUse":{"PropertyName":"y","ClassTypeRef":{"ClassName":"X"}}}
  },
  "NewEx":{"ByReference":"False","IndirectTypeRef":{
    "DirectStFldUse":{"PropertyName":"y","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}}}
  }
}
