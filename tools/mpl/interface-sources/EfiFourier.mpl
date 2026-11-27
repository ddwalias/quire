-- Interface-only module source: exported declarations define this module contract.
module EfiFourier (EfiFourier.mc_fft,EfiFourier.mc_FFT,EfiFourier.mc_cfft,EfiFourier.mc_CFFT,EfiFourier.mc_wave,EfiFourier.mc_iwave,EfiFourier.mc_dft,EfiFourier.mc_idft,EfiFourier.mc_dftr,EfiFourier.mc_idftr,EfiFourier.mc_ifft,EfiFourier.mc_IFFT,EfiFourier.mc_icfft,EfiFourier.mc_ICFFT) where
{
 mc_fft :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::fft";
 mc_FFT :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::FFT";
 mc_cfft :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::cfft";
 mc_CFFT :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::CFFT";
 mc_wave :: McdRun.V {}->McdRun.V {} @ "[efifourier_]Efi.Fourier::wave";
 mc_iwave :: McdRun.V {}->McdRun.V {} @ "[efifourier_]Efi.Fourier::iwave";
 mc_dft :: McdRun.V {}->McdRun.V {} @ "[efifourier_]Efi.Fourier::gfft";
 mc_idft :: McdRun.V {}->McdRun.V {} @ "[efifourier_]Efi.Fourier::gifft";
 mc_dftr :: McdRun.V {}->McdRun.V {} @ "[efifourier_]Efi.Fourier::gfftr";
 mc_idftr :: McdRun.V {}->McdRun.V {} @ "[efifourier_]Efi.Fourier::gifftr";
 mc_ifft :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::ifft";
 mc_IFFT :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::IFFT";
 mc_icfft :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::icfft";
 mc_ICFFT :: McdRun.V {a}->McdRun.V {a} @ "[efifourier_]Efi.Fourier::ICFFT";
 import McdRun;
 import Mpl;
}
