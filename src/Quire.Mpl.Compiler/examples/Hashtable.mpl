module Hashtable (Update, mlookupInv) where

Update () = ()
mlookupInv table key = hlookup (Map.Field2 table) key
