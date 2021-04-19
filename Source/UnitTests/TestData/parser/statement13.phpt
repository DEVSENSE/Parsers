<?php
yield;
yield $id;
yield $id => $fields; 
yield from from();
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{
  },
  "YieldEx" : {}, 
  "YieldEx" : {   
    "DirectVarUse" : {
      "VarName" : "id"
    }
  },
  "YieldEx" : {   
    "DirectVarUse" : {
      "VarName" : "id"
    },    
    "DirectVarUse" : {
      "VarName" : "fields"
    }
  },   
  "YieldFromEx" : {   
    "DirectFcnCall" : {
      "Name" : "from",
      "OriginalName":"from",        
      "FallbackQualifiedName":""
    }
  }
}
