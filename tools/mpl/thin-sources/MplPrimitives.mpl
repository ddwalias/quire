-- Reconstructed source: primitive value cases and assertions.
module MplPrimitives (Mpl.unsafe_identity,Mpl.build,Mpl.stelem_void,Mpl.F8(..),Mpl.ldelem,Mpl.dfoldr,Mpl.vb_bld,Mpl.mb_pshF,Mpl.C2(..),Mpl.vb_list,Mpl.veclen,Mpl.mb_psh,Mpl.cgt_i4,Mpl.neg_f8,Mpl.Real(..),Mpl.rem_i4,Mpl.ceq_i4,Mpl.vb_vec,Mpl.mb_psh0,Mpl.vb_psh1,Mpl.mb_pshI,Mpl.stelem,Mpl.vb_psh4,Mpl.Int(I),Mpl.Int(..),Mpl.Char(C),Mpl.Char(..),Mpl.Real(R),Mpl.NullOr(Really),Mpl.NullOr(..),Mpl.NullOr(Null),Mpl.cge_i4,Mpl.add_f8,Mpl.div_i4,Mpl.cne_i4,Mpl.clt_i4,Mpl.cle_i4,Mpl.conv_f8,Mpl.mul_f8,Mpl.conv_i4,Mpl.stelem_i4_void,Mpl.add_i4,Mpl.sub_i4,Mpl.newvec_i4,Mpl.sub_f8,Mpl.newvec,Mpl.ldelem_i4,Mpl.stelem_i4,Mpl.neg_i4,Mpl.mul_i4,Mpl.newvec_f8,Mpl.cge_f8,Mpl.identity,Mpl.dup,Mpl.ldelem_f8,Mpl.ceq_f8,Mpl.cle_f8,Mpl.stelem_f8_void,Mpl.cgt_f8,Mpl.stelem_f8,Mpl.div_f8,Mpl.clt_f8,Mpl.cne_f8,(MplPrimitives.=>),(MplPrimitives.|>),MplPrimitives.really,MplPrimitives.not_null,(MplPrimitives.??),MplPrimitives.assert,MplPrimitives.is_null,MplPrimitives.is_zero_i4,MplPrimitives.is_zero_f8,MplPrimitives.not_i4,MplPrimitives.ib,Mpl.conv_void,Mpl.IoVoid(..),Mpl.IoVoid(IoVoid),Mpl.Void(..),Mpl.Void(Void),Mpl.typeof,Mpl.I4(..)) where
{
 infixr 1 =>;
 (=>) :: a->b->b @ 75;
 infixr 1 |>;
 (|>) :: Mpl.Void->a->a @ 74;
 really :: Mpl.NullOr a->a @ 46;
 not_null :: Mpl.NullOr a->Mpl.Bool @ 46;
 infixl 1 ??;
 inline 1 (??);
 (??) :: Mpl.NullOr a->a->a @ "MplPrimitives.??" = (\z0_ z1_->(case z0_ of 
 {
  Mpl.Really z8_ -> z8_;
  Mpl.Null -> z1_;
 }));
 assert :: Mpl.String->Mpl.NullOr a->a @ "MplPrimitives.assert";
 inline 1 is_null;
 is_null :: Mpl.NullOr a->Mpl.Bool @ "MplPrimitives.is_null" = (\zi_->(case zi_ of 
 {
  Mpl.Null -> Mpl.True;
  _ -> Mpl.False;
 }));
 inline 1 is_zero_i4;
 is_zero_i4 :: Mpl.I4->Mpl.Bool @ "MplPrimitives.is_zero_i4" = (\zp_->(case zp_ of 
 {
  0$ -> Mpl.True;
  _ -> Mpl.False;
 }));
 inline 1 is_zero_f8;
 is_zero_f8 :: Mpl.F8 {a}->Mpl.Bool @ "MplPrimitives.is_zero_f8" = (\z10_->(case z10_ of 
 {
  0$ -> Mpl.True;
  _ -> Mpl.False;
 }));
 inline 1 not_i4;
 not_i4 :: Mpl.I4->Mpl.I4 @ "MplPrimitives.not_i4" = (\z17_->(case z17_ of 
 {
  0$ -> 1$;
  _ -> 0$;
 }));
 inline 1 ib;
 ib :: Mpl.I4->Mpl.Bool @ "MplPrimitives.ib" = (\z1e_->(case z1e_ of 
 {
  0$ -> Mpl.False;
  _ -> Mpl.True;
 }));
 import Mpl;
}
-- implementation
ib value = case value of 0 -> null | _ -> True.singleton
not_i4 value = case value of 0 -> 1 | _ -> 0
is_zero_f8 value = case value of 0.0 -> True.singleton | _ -> null
is_zero_i4 value = case value of 0 -> True.singleton | _ -> null
is_null value = case value of null -> True.singleton | _ -> null
assert message value = case value of Really result -> result | Null -> tail AppException.Raise message
?? left right = case left of Really result -> result | Null -> right
