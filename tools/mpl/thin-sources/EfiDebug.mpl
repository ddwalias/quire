-- Authored Mpl source for EfiDebug.
module EfiDebug (EfiDebug.mc_format,EfiDebug.mc_formatV,EfiDebug.mc_trace,EfiDebug.mc_mapvector,EfiDebug.mc_update1,EfiDebug.mc_update2,EfiDebug.mc_foldr,EfiDebug.mc_pause,EfiDebug.as_functionN) where
{
 mc_format :: Mpl.Object->McdRun.N Mpl.Object->McdRun.V {} @ "[efidebug_]Efi.Debug::format";
 mc_formatV :: McdRun.N Mpl.Object->McdRun.V {} @ "[efidebug_]Efi.Debug::formatV";
 mc_trace :: (i->o)->Mpl.Object->McdRun.N Mpl.Object->McdRun.V {} @ "[efidebug_]Efi.Debug::trace";
 mc_mapvector :: a->McdRun.Dyn->Mpl.Value {} @ "EfiDebug.mc_mapvector";
 mc_update1 :: Mpl.Matrix a->Mpl.Value {}->a->Mpl.Matrix a @ "EfiDebug.mc_update1";
 mc_update2 :: Mpl.Matrix a->Mpl.Value {}->Mpl.Value {}->a->Mpl.Matrix a @ "EfiDebug.mc_update2";
 mc_foldr :: a->b->McdRun.Dyn->b @ "EfiDebug.mc_foldr";
 mc_pause :: (i->o)->Mpl.Object->McdRun.N Mpl.Object->McdRun.V {} @ "[efidebug_]Efi.Debug::pause";
 as_functionN :: value->Mpl.I4->Mpl.I4->b->c @ "virtual[mplrun]Mcd.DimVal::asFunctionN(System.Int32,System.UInt32)";
 import McdRun;
 import Mpl;
}
-- implementation
-- Authored Mpl source for dimensional function and matrix operations.
mc_foldr fn seed vector = foldr fn seed vector
mc_update2 matrix row column value = tail MatVal.mcd_setm_2 matrix (DimVal.asDimlessInt row) (DimVal.asDimlessInt column) value
mc_update1 matrix row value = tail MatVal.mcd_setm_1 matrix (DimVal.asDimlessInt row) value
mc_mapvector fn value = tail MatVal.map (DimVal.asFunctionN fn 1 1) value
