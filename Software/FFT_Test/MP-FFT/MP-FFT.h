// 下列 ifdef 區塊是建立巨集以協助從 DLL 匯出的標準方式。
// 這個 DLL 中的所有檔案都是使用命令列中所定義 MPFFT_EXPORTS 符號編譯的。
// 在命令列定義的符號。任何專案都不應定義這個符號
// 這樣一來，原始程式檔中包含這檔案的任何其他專案
// 會將 MPFFT_API 函式視為從 DLL 匯入的，而這個 DLL 則會將這些符號視為
// 匯出的。
#ifdef MPFFT_EXPORTS
#define MPFFT_API __declspec(dllexport)
#else
#define MPFFT_API __declspec(dllimport)
#endif

#include <Windows.h>
#include <stdint.h>

typedef struct
{
    double real;
    double imag;
}FFT_Complex;

typedef struct
{
    double freq_hz;
    double amplitude;
}FFT_Spectrum;

#ifdef __cplusplus
extern "C"
{
#endif
    MPFFT_API INT32 WINAPI FFT_translate(FFT_Complex *p_dst, FFT_Complex *p_src, INT32 num_of_points);
    MPFFT_API INT32 WINAPI FFT_convertToMagnitude(DOUBLE *p_dst, FFT_Complex *p_src, INT32 num_of_points);
    MPFFT_API INT32 WINAPI FFT_translateToSpectrum(DOUBLE *p_delte_hz, DOUBLE *p_amplitude, FFT_Complex *p_dst, FFT_Complex *p_src, INT32 num_of_points, DOUBLE hz);

#ifdef __cplusplus
}
#endif
