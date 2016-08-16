<?php

include "a.php"; // this will include a.php
include_once "A.php"; // this will include a.php again! (PHP 4 only) 
require __ROOT__.'/config.php'; 
require_once(__ROOT__.'/config.php'); 

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "IncludingEx":{"InclusionType":"Include","StringLiteral":{"Value":"a.php"}},
  "IncludingEx":{"InclusionType":"IncludeOnce","StringLiteral":{"Value":"A.php"}},
  "IncludingEx":{"InclusionType":"Require",
    "BinaryEx":{"Operation":"Concat",
      "GlobalConstUse":{"Name":"__ROOT__"},
      "StringLiteral":{"Value":"/config.php"}
    }
  },
  "IncludingEx":{"InclusionType":"RequireOnce",
    "BinaryEx":{"Operation":"Concat",
      "GlobalConstUse":{"Name":"__ROOT__"},
      "StringLiteral":{"Value":"/config.php"}
    }
  }
}