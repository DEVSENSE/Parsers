<?php

$expectedOrderDataJson = <<<ORDER_DATA_JSON
{
    "customer_id":1,
    "addresses":
        {"{$addressIds[0]}":
            {"firstname":"John","lastname":"Smith","company":false,"street":"Green str, 67","city":"CityM",
                "country_id":"US",
                "region":"Alabama","region_id":1,
                "postcode":"75477","telephone":"3468676","vat_id":false},
         "{$addressIds[1]}":
            {"firstname":"John","lastname":"Smith","company":false,"street":"Black str, 48","city":"CityX",
                "country_id":"US",
                "region":"Alabama","region_id":1,
                "postcode":"47676","telephone":"3234676","vat_id":false}
         },
     "store_id":1,"currency_symbol":"$","shipping_method_reseted":true,"payment_method":null
}
ORDER_DATA_JSON;

?>

<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{"Operation":"AssignValue",
    "DirectVarUse":{"VarName":"expectedOrderDataJson"},
    "ConcatEx":{
      "StringLiteral":{"Value":"{"customer_id":1,"addresses":{""},
      "ItemUse":{"Array":{"DirectVarUse":{"VarName":"addressIds"}},"Index":{"LongIntLiteral":{"Value":"0"}}},
      "StringLiteral":{"Value":":{"firstname":"John","lastname":"Smith","company":false,"street":"Greenstr,67","city":"CityM","country_id":"US","region":"Alabama","region_id":1,"postcode":"75477","telephone":"3468676","vat_id":false},"},
      "ItemUse":{"Array":{"DirectVarUse":{"VarName":"addressIds"}},"Index":{"LongIntLiteral":{"Value":"1"}}},
      "StringLiteral":{"Value":"":{"firstname":"John","lastname":"Smith","company":false,"street":"Blackstr,48","city":"CityX","country_id":"US","region":"Alabama","region_id":1,"postcode":"47676","telephone":"3234676","vat_id":false}},"store_id":1,"currency_symbol":"$","shipping_method_reseted":true,"payment_method":null}"}
    }
  },
  "EchoStmt":{"StringLiteral":{"Value":""}}
}
