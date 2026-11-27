module Hashtable (Update, mlookupInv) where

Update () = ()
hlookup table key = let value = table[key] in if value == null then null else Just value
mlookupInv table key = hlookup (Map.Field2 table) key
mlookup table key = let value = table in let map: Map = value in tail hlookup map.Field1 key
blookupInv table key = let value = table in let map: Map = value in tail hlookup map.Field2 key
blookup table key = let value = table in let map: Map = value in tail hlookup map.Field1 key
safe_add table key value = table[key] := value; table
add_with_check table key value allow = if hlookup table key then if allow then table else tail AppException.Raise "duplicate hash key" else tail safe_add table key value
hash_set_contains table key = let map = table in if hlookup map key then True.singleton else null
add_entries entries allow inputMap = case entries of Cons cons -> let pairs: array = cons.Field1 in let sourceMap: Map = inputMap in add_with_check sourceMap.Field1 pairs[0] pairs[1] allow; add_with_check sourceMap.Field2 pairs[1] pairs[0] allow; add_entries cons.Field2 allow inputMap | null -> inputMap
make_many_to_one_map pairs = make_map pairs True.singleton
make_bijective_map pairs = make_map pairs null
make_map entries allow = let capacity: int = length entries in let marker = if allow then DAllow.singleton else null in tail add_entries entries marker (new Map(new Hashtable(capacity), new Hashtable(capacity)))
make_hash_map entries = for pair in entries using length entries into result do result[pair[0]] := pair[1]
make_hash_set entries = make_hash_map (MplPrelude.map $$5d_64c00f80 entries)
$$5d_64c00f8 value = [value, True.singleton]
