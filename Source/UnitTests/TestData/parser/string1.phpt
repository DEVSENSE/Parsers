<?php
"Package %s's source key should be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.$";
"Package %s's source key should be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.{"; 
"Package %s's source key should $be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.$"; 
"Package %s's source key should $be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.{"; 
`Package %s's source key should be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.$`;
`Package %s's source key should be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.{`; 
`Package %s's source key should $be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.$`; 
`Package %s's source key should $be specified as {\"type\": ..., \"url\": ..., \"reference\": ...},\n%s given.{`;
?>
<<<TEST>>>
"GlobalCode" : {
  "NamingContext" : {
  },
  "StringLiteral" : {
    "Value" : "Package %s's source key should be specified as {"type": ..., "url": ..., "reference": ...},%s given.$"
  },
  "StringLiteral" : {
    "Value" : "Package %s's source key should be specified as {"type": ..., "url": ..., "reference": ...},%s given.{"
  },
  "ConcatEx" : {
    "StringLiteral" : {
      "Value" : "Package %s's source key should "
    },
    "DirectVarUse" : {
      "VarName" : "be"
    },
    "StringLiteral" : {
      "Value" : " specified as {"type": ..., "url": ..., "reference": ...},%s given.$"
    }
  },
  "ConcatEx" : {
    "StringLiteral" : {
      "Value" : "Package %s's source key should "
    },
    "DirectVarUse" : {
      "VarName" : "be"
    },
    "StringLiteral" : {
      "Value" : " specified as {"type": ..., "url": ..., "reference": ...},%s given.{"
    }
  },
  "ShellEx" : {
    "StringLiteral" : {
      "Value" : "Package %s's source key should be specified as {"type": ..., "url": ..., "reference": ...},%s given.$"
    }
  },
  "ShellEx" : {
    "StringLiteral" : {
      "Value" : "Package %s's source key should be specified as {"type": ..., "url": ..., "reference": ...},%s given.{"
    }
  },
  "ShellEx" : {
    "ConcatEx" : {
      "StringLiteral" : {
        "Value" : "Package %s's source key should "
      },
      "DirectVarUse" : {
        "VarName" : "be"
      },
      "StringLiteral" : {
        "Value" : " specified as {"type": ..., "url": ..., "reference": ...},%s given.$"
      }
    }
  },
  "ShellEx" : {
    "ConcatEx" : {
      "StringLiteral" : {
        "Value" : "Package %s's source key should "
      },
      "DirectVarUse" : {
        "VarName" : "be"
      },
      "StringLiteral" : {
        "Value" : " specified as {"type": ..., "url": ..., "reference": ...},%s given.{"
      }
    }
  }
}
