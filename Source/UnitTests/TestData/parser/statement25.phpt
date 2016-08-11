<?php
       
$array = array(
    1 => "a",
    "hello",
    1.5 => &$x,
    &$x,
    true => list( 1 => "a" ),
    list( 1 => "a" ),
);


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
