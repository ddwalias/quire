-- Interface-only module source: exported declarations define this module contract.
module EfiIO (EfiIO.mc_READPRN,EfiIO.mc_WRITEPRN,EfiIO.mc_READ_5F_HUE,EfiIO.mc_READ_5F_LIGHT,EfiIO.mc_READ_5F_VALUE,EfiIO.mc_READ_5F_HLS_5F_HUE,EfiIO.mc_READ_5F_RED,EfiIO.mc_READ_5F_GREEN,EfiIO.mc_READ_5F_BLUE,EfiIO.mc_WRITERGB,EfiIO.mc_APPENDPRN,EfiIO.mc_READBMP,EfiIO.mc_WRITEBMP,EfiIO.mc_READRGB,EfiIO.mc_READ_5F_HLS_5F_SAT,EfiIO.mc_READ_5F_HSV,EfiIO.mc_READ_5F_HSV_5F_SAT,EfiIO.mc_READ_5F_IMAGE,EfiIO.mc_WRITEBIN,EfiIO.mc_GETWAVINFO,EfiIO.mc_READWAV,EfiIO.mc_WRITEWAV,EfiIO.mc_READ_5F_HLS_5F_LIGHT,EfiIO.mc_READ_5F_HSV_5F_HUE,EfiIO.mc_READ_5F_HSV_5F_VALUE,EfiIO.mc_READ_5F_HLS,EfiIO.mc_WRITE_5F_HLS,EfiIO.mc_WRITE_5F_HSV,EfiIO.mc_READBIN,EfiIO.mc_WRITEFILE,EfiIO.mc_APPENDDATA,EfiIO.mc_WRITECSV,EfiIO.mc_READCSV,EfiIO.mc_READEXCEL,EfiIO.mc_LoadColormap,EfiIO.mc_SaveColormap,EfiIO.mc_READDATA,EfiIO.mc_WRITEDATA,EfiIO.mc_WRITETEXT,EfiIO.mc_WRITEEXCEL,EfiIO.mc_READFILE,EfiIO.mc_READTEXT) where
{
 mc_READPRN :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READPRN";
 mc_WRITEPRN :: McdRun.C->McdRun.P->McdRun.W->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITEPRN";
 mc_READ_5F_HUE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HLS_HUE";
 mc_READ_5F_LIGHT :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HLS_LIGHT";
 mc_READ_5F_VALUE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HSV_VALUE";
 mc_READ_5F_HLS_5F_HUE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HLS_HUE";
 mc_READ_5F_RED :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_RED";
 mc_READ_5F_GREEN :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_GREEN";
 mc_READ_5F_BLUE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_BLUE";
 mc_WRITERGB :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITERGB";
 mc_APPENDPRN :: McdRun.C->McdRun.P->McdRun.W->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::APPENDPRN";
 mc_READBMP :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READBMP";
 mc_WRITEBMP :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITEBMP";
 mc_READRGB :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READRGB";
 mc_READ_5F_HLS_5F_SAT :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HLS_SAT";
 mc_READ_5F_HSV :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HSV";
 mc_READ_5F_HSV_5F_SAT :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HSV_SAT";
 mc_READ_5F_IMAGE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_IMAGE";
 mc_WRITEBIN :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITEBIN";
 mc_GETWAVINFO :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::GETWAVINFO";
 mc_READWAV :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READWAV";
 mc_WRITEWAV :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITEWAV";
 mc_READ_5F_HLS_5F_LIGHT :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HLS_LIGHT";
 mc_READ_5F_HSV_5F_HUE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HSV_HUE";
 mc_READ_5F_HSV_5F_VALUE :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HSV_VALUE";
 mc_READ_5F_HLS :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READ_HLS";
 mc_WRITE_5F_HLS :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITE_HLS";
 mc_WRITE_5F_HSV :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITE_HSV";
 mc_READBIN :: McdRun.C->McdRun.N (McdRun.V {})->McdRun.V {} @ "[efiio_]Efi.IO::READBIN";
 mc_WRITEFILE :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITEFILE";
 mc_APPENDDATA :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::APPENDDATA";
 mc_WRITECSV :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITECSV";
 mc_READCSV :: McdRun.C->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiio_]Efi.IO::READCSV";
 mc_READEXCEL :: McdRun.C->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiio_]Efi.IO::READEXCEL";
 mc_LoadColormap :: McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::LoadColormap";
 mc_SaveColormap :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::SaveColormap";
 mc_READDATA :: McdRun.C->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::READDATA";
 mc_WRITEDATA :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITEDATA";
 mc_WRITETEXT :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiio_]Efi.IO::WRITETEXT";
 mc_WRITEEXCEL :: McdRun.C->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V () @ "[efiio_]Efi.IO::WRITEEXCEL";
 mc_READFILE :: McdRun.C->McdRun.N (McdRun.V {})->McdRun.V {} @ "[efiio_]Efi.IO::READFILE";
 mc_READTEXT :: McdRun.C->McdRun.N (McdRun.V {})->McdRun.V {} @ "[efiio_]Efi.IO::READTEXT";
 import McdRun;
 import Mpl;
}
