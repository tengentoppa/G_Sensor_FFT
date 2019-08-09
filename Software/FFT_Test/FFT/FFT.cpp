// FFT.cpp : 定義主控台應用程式的進入點。
//

/*********************************************************************
快速福利葉變換C函數
函數簡介：此函數是通用的快速傅裏葉變換C語言函數，移植性強，以下部分不依
賴硬件。此函數采用聯合體的形式表示一個復數，輸入為自然順序的復
數（輸入實數是可令復數虛部為0），輸出為經過FFT變換的自然順序的
復數
使用說明：使用此函數只需更改宏定義FFT_N的值即可實現點數的改變，FFT_N的
應該為2的N次方，不滿足此條件時應在後面補0
函數調用：FFT(s);
作    者：吉帥虎
時    間：2010-2-20
版    本：Ver1.0
參考文獻：

**********************************************************************/
#include <stdio.h>
#include <windows.h>
#include <stdint.h>
#include <math.h>

#define  PI     3.1415926535897932384626433832795028841971               //定義圓周率值
#define  FFT_N  65536                                                     //定義福利葉變換的點數
#define  FFT_SAMPLING_HZ    (1024)                                    //取樣頻率

struct compx { float real, imag; };                                    //定義一個復數結構
struct compx s[FFT_N];                                              //FFT輸入和輸出：從S[1]開始存放，根據大小自己定義

                                                                    /*******************************************************************
                                                                    函數原型：struct compx EE(struct compx b1,struct compx b2)
                                                                    函數功能：對兩個復數進行乘法運算
                                                                    輸入參數：兩個以聯合體定義的復數a,b
                                                                    輸出參數：a和b的乘積，以聯合體的形式輸出
                                                                    *******************************************************************/
struct compx EE(struct compx a, struct compx b)
{
    struct compx c;

    c.real = a.real*b.real - a.imag*b.imag;
    c.imag = a.real*b.imag + a.imag*b.real;

    return(c);
}
/*****************************************************************
函數原型：void FFT(struct compx *xin,int N)
函數功能：對輸入的復數組進行快速傅裏葉變換（FFT）
輸入參數：*xin復數結構體組的首地址指針，struct型
*****************************************************************/
void FFT(struct compx *xin)
{
    int f, m, nv2, nm1, i, k, l, j = 0;
    struct compx u, w, t;

    nv2 = FFT_N / 2;                   //變址運算，即把自然順序變成倒位序，采用雷德算法
    
    
    nm1 = FFT_N - 1;
    for (i = 0; i < nm1; i++)
    {
        if (i < j)                      //如果i<j,即進行變址
        {
            t = xin[j];
            xin[j] = xin[i];
            xin[i] = t;
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

        f = FFT_N;
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
                for (i = j; i <= FFT_N - 1; i = i + le)            //控制同一蝶形結運算，即計算系數相同蝶形結
                {
                    ip = i + lei;                           //i，ip分別表示參加蝶形運算的兩個節點
                    t = EE(xin[ip], u);                    //蝶形運算，詳見公式
                    xin[ip].real = xin[i].real - t.real;
                    xin[ip].imag = xin[i].imag - t.imag;
                    xin[i].real = xin[i].real + t.real;
                    xin[i].imag = xin[i].imag + t.imag;
                }
                u = EE(u, w);                           //改變系數，進行下一個蝶形運算
            }
        }
    }
}
/************************************************************
函數原型：void main()
函數功能：測試FFT變換，演示函數使用方法
輸入參數：無
輸出參數：無
************************************************************/
void main()
{
    int i;

    FILE *fp1, *fp2;

    fp1 = fopen("real.txt", "wt");
    fp2 = fopen("imag.txt", "wt");
    
    fprintf(fp1, "%d\n", FFT_SAMPLING_HZ);
    fprintf(fp2, "%d\n", FFT_SAMPLING_HZ);
    for (i = 0; i < FFT_N; i++)                                        //給結構體賦值
    {
        s[i].imag = 0;                                                //虛部為0

        s[i].real = 1 + 2 * sin(9.5 * 2 * PI * i / FFT_SAMPLING_HZ) + 5 * sin(3.5f * 2 * PI * i / FFT_SAMPLING_HZ) + 10 * cos(50.0f * 2 * PI * i / FFT_SAMPLING_HZ);
		
        // 方波
		for (int32_t j = 1; j < 768; j+=2) s[i].real += 4 * sin(j * 2 * PI * i / FFT_SAMPLING_HZ) / (j * PI);
        
		/*
        // 方波
        s[i].real = 4 * sin(2 * PI * i / FFT_SAMPLING_HZ) / PI +
            4 * sin(3 * 2 * PI * i / FFT_SAMPLING_HZ) / (3 * PI) +
            4 * sin(5 * 2 * PI * i / FFT_SAMPLING_HZ) / (5 * PI) +
            4 * sin(7 * 2 * PI * i / FFT_SAMPLING_HZ) / (7 * PI) +
            4 * sin(9 * 2 * PI * i / FFT_SAMPLING_HZ) / (9 * PI) + 
            4 * sin(11 * 2 * PI * i / FFT_SAMPLING_HZ) / (11 * PI) + 
            4 * sin(13 * 2 * PI * i / FFT_SAMPLING_HZ) / (13 * PI) +
            4 * sin(15 * 2 * PI * i / FFT_SAMPLING_HZ) / (15 * PI);
        */
        //s[i].real = 1 + 2 * sin(2 * PI * i / FFT_SAMPLING_HZ);         //實部為正弦波FFT_N點采樣，賦值為1

        /*
        s[i].real = 0.7f * sin(50 * 2 * PI * (double)i / FFT_SAMPLING_HZ) + sin(120 * 2 * PI * (double)i / FFT_SAMPLING_HZ);         //signal containing a 50 Hz sinusoid of amplitude 0.7 and a 120 Hz sinusoid of amplitude
        s[i].real += (2 * rand() % FFT_SAMPLING_HZ / FFT_SAMPLING_HZ);          // Corrupt the signal with zero-mean white noise with a variance of 4.
        */

        fprintf(fp1, "%lf\n", s[i].real);
        fprintf(fp2, "%lf\n", s[i].imag);
    }

    fcloseall();

    fp1 = fopen("real_imag.txt", "wt");
    fprintf(fp1, "%d\n", FFT_SAMPLING_HZ);
    for (i = 0; i < FFT_N; i++)
    {
        fprintf(fp1, "%lf, %lf\n", s[i].real, s[i].imag);
    }
    fclose(fp1);

    uint32_t t = GetTickCount();
    FFT(s);                                                //進行快速福利葉變換

    for (i = 0; i < FFT_N; i++)                             //求變換後結果的模值，存入復數的實部部分
        s[i].real = sqrt(s[i].real*s[i].real + s[i].imag * s[i].imag);

    uint32_t tt = GetTickCount() - t;

    //while (1);
}