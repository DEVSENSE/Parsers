<?php

include "a.php"; // this will include a.php
include_once "A.php"; // this will include a.php again! (PHP 4 only) 
require __ROOT__.'/config.php'; 
require_once(__ROOT__.'/config.php'); 

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "IncludingEx":{"InclusionType":"Include","IsConditional":"False","StringLiteral":{"Value":"a.php"}},
  "IncludingEx":{"InclusionType":"IncludeOnce","IsConditional":"False","StringLiteral":{"Value":"A.php"}},
  "IncludingEx":{"InclusionType":"Require",
    "IsConditional":"False",
    "BinaryEx":{"Operation":"Concat",
      "GlobalConstUse":{"Name":"__ROOT__","FallbackName":""},
      "StringLiteral":{"Value":"/config.php"}
    }
  },
  "IncludingEx":{"InclusionType":"RequireOnce",
    "IsConditional":"False",
    "BinaryEx":{"Operation":"Concat",
      "GlobalConstUse":{"Name":"__ROOT__","FallbackName":""},
      "StringLiteral":{"Value":"/config.php"}
    }
  }
}