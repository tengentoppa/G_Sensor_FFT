// MP-FFT.cpp : 定義 DLL 應用程式的匯出函式。
//
#include <stdint.h>
#include <math.h>
#include "MP-FFT.h"

#define  PI     3.1415926535897932384626433832795028841971f               //定義圓周率值


void FFT(FFT_Complex out[], FFT_Complex in[], int32_t len);
void Magnitude(double out[], FFT_Complex in[], int32_t len);
void Spectrum(double *p_delte_hz, double amplitude[], FFT_Complex out[], FFT_Complex in[], int32_t len, double hz);

INT32 WINAPI FFT_translate(FFT_Complex *p_dst, FFT_Complex *p_src, INT32 num_of_points)
{
    FFT(p_dst, p_src, num_of_points);
    return 0;
}

INT32 WINAPI FFT_convertToMagnitude(DOUBLE *p_dst, FFT_Complex *p_src, INT32 num_of_points)
{
    Magnitude(p_dst, p_src, num_of_points);

    return 0;
}

INT32 WINAPI FFT_translateToSpectrum(DOUBLE *p_delte_hz, DOUBLE *p_amplitude, FFT_Complex *p_dst, FFT_Complex *p_src, INT32 num_of_points, DOUBLE hz)
{
    
    Spectrum(p_delte_hz, p_amplitude, p_dst, p_src, num_of_points, hz);
    return 0;
}

/*******************************************************************
函數原型：struct compx EE(struct compx b1,struct compx b2)
函數功能：對兩個復數進行乘法運算
輸入參數：兩個以聯合體定義的復數a,b
輸出參數：a和b的乘積，以聯合體的形式輸出
*******************************************************************/
FFT_Complex EE(FFT_Complex a, FFT_Complex b)
{
    FFT_Complex c;

    c.real = a.real*b.real - a.imag*b.imag;
    c.imag = a.real*b.imag + a.imag*b.real;

    return(c);
}
/*****************************************************************
函數原型：void FFT(FFT_Complex out[], FFT_Complex in[], int32_t len)
函數功能：對輸入的復數組進行快速傅裏葉變換（FFT）
輸入參數：*in復數結構體組的首地址指針，FFT_Complex型
*****************************************************************/
void FFT(FFT_Complex out[], FFT_Complex in[], int32_t len)
{
    int32_t f, m, nv2, nm1, i, k, l, j = 0;
    FFT_Complex u, w, t;

    nv2 = len / 2;                   //變址運算，即把自然順序變成倒位序，采用雷德算法

    memmove(out, in, sizeof(FFT_Complex) * len);
    nm1 = len - 1;
    for (i = 0; i < nm1; i++)
    {
        if (i < j)                      //如果i<j,即進行變址
        {
            t = out[j];
            out[j] = out[i];
            out[i] = t;
        }
        k = nv2;                       //求j的下一個倒位序
        while (k <= j)                 //如果k<=j,表示j的最高位為1   
        {
            j = j - k;                 //把最高位變成0
            k = k / 2;                 //k/2，比較次高位，依次類推，逐個比較，直到某個位為0
        }
        j = j + k;                     //把0改為1
    }

    {
        int le, lei, ip;                            //FFT運算核，使用蝶形運算完成FFT運算

        f = len;
        for (l = 1; (f = f / 2) != 1; l++)                  //計算l的值，即計算蝶形級數
            ;
        for (m = 1; m <= l; m++)                         // 控制蝶形結級數
        {                                        //m表示第m級蝶形，l為蝶形級總數l=log（2）N
            le = 2 << (m - 1);                            //le蝶形結距離，即第m級蝶形的蝶形結相距le點
            lei = le / 2;                               //同一蝶形結中參加運算的兩點的距離
            u.real = 1.0;                             //u為蝶形結運算系數，初始值為1
            u.imag = 0.0;
            w.real = cos(PI / lei);                     //w為系數商，即當前系數與前一個系數的商
            w.imag = -sin(PI / lei);
            for (j = 0; j <= lei - 1; j++)                   //控制計算不同種蝶形結，即計算系數不同的蝶形結
            {
                for (i = j; i <= (len - 1); i = i + le)            //控制同一蝶形結運算，即計算系數相同蝶形結
                {
                    ip = i + lei;                           //i，ip分別表示參加蝶形運算的兩個節點
                    t = EE(out[ip], u);                    //蝶形運算，詳見公式
                    out[ip].real = out[i].real - t.real;
                    out[ip].imag = out[i].imag - t.imag;
                    out[i].real = out[i].real + t.real;
                    out[i].imag = out[i].imag + t.imag;
                }
                u = EE(u, w);                           //改變系數，進行下一個蝶形運算
            }
        }
    }
}

void Magnitude(double out[], FFT_Complex in[], int32_t len)
{
    for (int32_t i = 0; i < len; i++)   //求變換後結果的模值，存入復數的實部部分
    {
        out[i] = sqrt(in[i].real * in[i].real + in[i].imag * in[i].imag);
    }
}

void Spectrum(double *p_delte_hz, double amplitude[], FFT_Complex out[], FFT_Complex in[], int32_t len, double hz)
{
    /*
    for (int32_t i = 0; i < len; i++)   //求變換後結果的模值，存入復數的實部部分
    {
        double multiplier = 0.5f * (1 - cos(2 * PI*i / (len - 1)));//Hanning Window
        in[i].real = multiplier * in[i].real;
        in[i].imag = multiplier * in[i].imag;
    }
    */

    FFT(out, in, len);

    *p_delte_hz = hz / len;

    for (int32_t i = 0; i < (len / 2 - 1); i++)
    {
        amplitude[i] = sqrt(out[i].real * out[i].real + out[i].imag * out[i].imag) / (len / 2 - 1);  //Here   I  have calculated the y axis of the spectrum in dB
    }
}