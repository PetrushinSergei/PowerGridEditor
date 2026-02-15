//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Mainwlfl.h"
// This is a NET program - NET-W v/1.1 for win95/98/NT 6/06/01
#include <iostream>
#include<strstream>
#include <sstream>
#include <fstream>
#include <cstring>
//#include <dstring.h>
#include <stdio.h>
//#include <stdlib.h>
//#undef abs
#include <ShellAPI.h>
//#include <DateUtils.hpp>
#include <sysutils.hpp>
#include <systdate.h>
#include <iomanip.h>
#include <conio>
#include <math.h>
#include "process.h"
#include "inifiles.hpp"
#include "Unit2.h"

//#include "Loadflow.cpp"

#define inn 100    //Number of buses with the slack bus
#define imm 150    //Number of brunches + 1
// Прототипы функций
//void New_list (AnsiString);
int deb();
bool preparation(),DelFile=0;
void zerovalue();
void ylfl();
float func(int count);
void jacoby();
//void SaveCDU();
//void ToDos();
//void DoubleSpase(AnsiString OldFileName,AnsiString NewFileName);
void raion(int);
void gauss();
void alpha();
bool load();
// Объявление глобальных переменных

AnsiString Out_Str;
string in,out,rez,cdu;
AnsiString FILE_NAME_STR;
int count,n,m,i,j,L;
int kkk=100,kk=10;//Служит для определения района(kkk - показывает кратность)
// переменные для разделения потерь
float rd[30], dP[30];
float ta[30],tr[30],tza[20],tzr[20];
float aa[30][20],a[30][20],ar[30][20], sia;
float b0,b1,b2,b3,b4,b5,b6,b7,b8,b9,b10;
int   na[30],ka[30],nr[30],kr[30],N[30],k[30],k0,kx;
int j1,j2,j3,j4,j5,j6,j7,j8,j9,j10;
int n1,n2,n3,n4,n5,n6,n7,n8,n9,n10;
int k1,k2,k3,k4,k5,k6,k7,k8,k9,k10;
float norm,r1,r2,Precision=0.001;
char buffer[20000];
String sFile,sFile1="",sFile2="",sFile3="";
//Const size = 20000;
int nn[inn],nk[inn],ron[10][inn],nus [10];
int nm[3][imm],nm1[3][imm],Iteraz=10;
bool ok, rezalt;
float grrr[imm],gxxx[imm],unom[inn],p0[inn],q0[inn],g[inn],b[inn]
     ,r[imm],x[imm],gy[imm],by[imm],kt[imm],line[10][10],tran[10][13];
float gg[inn],bb[inn],va[inn],vr[inn],gr[imm],bx[imm]
     ,p[inn],q[inn],ja[inn],jr[inn]
     ,ds[2 * inn],A[2 * inn][2 * inn];
int iFileHandle;
  int iFileLength;
  int iBytesRead;
  char *pszBuffer;
   TIniFile *INI;
//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm1 *Form1;
//---------------------------------------------------------------------------

__fastcall TForm1::TForm1(TComponent* Owner)
        : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void New_list (AnsiString NameTabFile)
{
  Form1->TabSheet2->Visible=true;
   Form1->Memo5->Lines->LoadFromFile(NameTabFile);

   //TTabSheet* ww =
    //w TTabSheet(Application);//->TabSheet;//(this);//(Application);//this);
   //if (!JJ) return;
   //JJ->Caption=NameTabFile;
   //TabSheet->Caption = NameTabFile;
   //Form1->Memo1->Lines->LoadFromFile(NameTabFile);
   //Form1->Memo1->SelStart = 0;
   //Form1->Memo1->Modified = false;
    //Form1= new TForm1(this);
  //Form1->Memo1->Caption="Режим "+IntToStr(MDIChildCount);
  //Caption = ExtractFileName(AFileName);
   //Editor->Lines->LoadFromFile(PathName);
   //JJ->Show();
}
//---------------------------------------------------------------------------
 int deb()
{
  int i;
    ofstream fout("network.deb");
  if (!fout) {
    Out_Str = "Невозможно открыть файл 'network.deb' ";
     // Memo1->Lines->Append(Out_Str);
     ShowMessage("Невозможно открыть файл \"network.deb\"");
    //cout << "Невозможно открыть файл 'network.deb' \n";
    return 1;
  }
  for (i = 0; i <= n; i++)
    fout << setw(5) << nn[i] << setw(5) << nk[i]
         << setw(9)  << unom[i]
         << setw(9) << setprecision(2) << va[i]
         << setw(9) << setprecision(2) << vr[i]
         << setw(9)  << p0[i]
         << setw(9)  << q0[i]
         << setw(13) << setprecision(7) << gg[i]
         << setw(13) << setprecision(7) << bb[i]
         << endl;
  for (i = 1; i <= m; i++)
    fout << setw(5) << nm1[1][i] << setw(5) << nm1[2][i]
         << setw(7)  << r[i]
         << setw(7)  << x[i]
         << setw(12) << setprecision(6) << gy[i]
         << setw(12) << setprecision(6) << by[i]
         << setw(12) << setprecision(6) << gr[i]
         << setw(12) << setprecision(6) << bx[i]
         << setw(9) << setprecision(3) << kt[i]
         << endl;
 // Out_Str = "Узлов " + n + " Ветвей " + m ;
   //   Memo1->Lines->Append(Out_Str);
  //cout << endl << "deb - Ok - Узлов " << n << " Ветвей " << m
  //     << " Press any key" << endl;
  fout << endl << "deb - Ok - Buses " << n << " Ветвей " << m
       << endl;
  fout.close();
  return 1;
  //getch();
}
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
 //----------------------------------------------------------------------------
 //Блок создания массивов узлов по районам
 void raion(int nnn)
{
 int kratn=nnn/kkk;
 while (kratn > 9) kratn/=kk;
 nus[kratn]++;
 ron[kratn][nus[kratn]-1]=nnn;
 }
//----------------------------------------------------------------------------
// Date input and preparation
bool preparation()
{
  int i,j,k,l1, NUL[5];
  char ch, str[80];
  float r1,r2,AA[10];
  bool right,right1,right2,allright;
  //Обнуление внешних массивов уже использов при использ. этого модуля.

     for (int i=0;i<10;i++){nus[i]=0;for(int j=1;j<inn;j++)ron[i][j]=0;};

  //istringstream net1(in);
  ifstream net1("network.cdu"); // открытие файла для чтеия (ввода)
  if(!net1)
  {ShowMessage("Если нет ошибок в данных то перезапустите расчет.");
  //cout << "Невозможно открыть файл\n";
    return false;
  }
// Установка флагов fixed - числа в обычной нотации
// showpoints - дает десятичню точку и последующие нули
  //cout.setf(ios::fixed | ios::showpoint);

 j = 0; k = 0;
   // for (i = 0; i<350; i++)
 while ( net1  >> NUL[1] )  //Как только не найдет очередной лексемы возратит 0
 {
           //net1  >> NUL[1];
           // Балансирующий узел
        switch(NUL[1]) {
         case 102:

        net1  >>  NUL[2] >> nn[0] >>   unom[0]
              >> p0[0] >> q0[0] >>
              AA[1] >>   AA[2] >> AA[3] >> AA[4] >>
              AA[5];
      nk[0]=3;
      g[0]= 0;
          b[0] = 0;
         raion(nn[0]);
      break;
      case 201:
         j++;
        net1  >>  NUL[2] >> nn[j] >>  unom[j]
              >> p0[j] >> q0[j] >>
              AA[1] >>   AA[2] >> AA[3] >> AA[4] >>
              AA[5];
          nk[j] = 1;
          g[j]= 0;
          b[j] = 0;
       if (AA[3] > 0.1)
        {
        unom[j] = AA[3];
        nk[j] = 2;
        }
         raion(nn[j]);
      break;
      case 301:
    k++;
    net1  >> NUL[2] >> nm[1][k] >> nm[2][k];
       if (nm[2][k] == 0)
        {
         //  'Нарвались на шунт
         net1  >>  g[k] >> b[k];

        for (l1 = 1; l1 <= j; l1++)
         {
               if (nn[l1] == nm[1][k])
                 {
                   b[l1]=-b[k];
                   b[k]=0;
                   }
         }

         k--;
          }
        //  else
        if (nm[2][k] != 0)
      {

        net1  >>  r[k] >> x[k] >> by[k]
        >> kt[k] >> AA[3] >> AA[4] >> AA[5];
        if (abs(x[k]) < 1.001) x[k] = 1.01;
        if (kt[k] < 0.001) kt[k] = 1;
        by[k] = -by[k];
        gy[k] = 0;
       }
      break;

         default: ;//net1>>AA[4];//while (net1.get()!='\n'){};
         };
  }
   n = j; m = k; // число независимых узлов и общее число ветвей
  //Определение районных линий
 /*  for (int i=1;i<=m;i++){
    for (int d=0;d<=9;d++){
     if (nus[d]==0)continue;
      for (int g=0;g<nus[d];g++){
       if (nm[1][i]==ron[d][g])na[i]=d;
       if (nm[2][i]==ron[d][g])ko[i]=d;
      }
    }
   }*/
// Манипуляторы ввода/вывода
// setw - минимальная ширина поля, setprecision - точность вывода
// по умолчанию точность равна 6 цифрам (после десятичной точки)
// cout << endl; // закрыть файл ввода данных
// net1.close();
  ofstream fout("network.err");
    if (!fout) { ShowMessage("Невозможно открыть файл \"network.err\"");};
  allright = true; // общий флаг фатальной некорректности данных
  // нет ли в списке узлов таких, которым не инцидентна ни одна ветвь?
  if (n==0 || m==0){ShowMessage("Совсем нет ветвей или узлов");fout.close();
  return false;};
   for (i = 1; i <= n; i++) {
    right = false;
    for (j = 1; j <= m; j++) {
      if (nn[i] == nm[1][j]) right = true;
      if (nn[i] == nm[2][j]) right = true;
    }
    if (!right) {
      allright = false; // Первая возможная ошибка в графе
      fout << "Ошибка связи 1. Нет ветвей для узла " << nn[i]
           << endl;
          }
  }
  // нет ли в списке ветвей таких, которые инцидентны
  // несуществующему узлу
  for (j = 1; j <= m; j++) {
    right1 = false; right2 = false;
    for (i = 0; i <= n; i++) {
      if (nm[1][j] == nn[i]) right1 = true;
      if (nm[2][j] == nn[i]) right2 = true;
    }
    if (!(right1 && right2)) {
      allright = false; // Вторая возможная ошибка в графе
      fout << "Ошибка связи 2. Нет узлов для ветви "
           << nm[1][j] << "\t" << nm[2][j] << endl;
    }
  }
  // Числовые ошибки
  right = true;
  for (i = 1; i <= n; i++) {
    if (nn[i] > 100000) {
      right = false;
      fout << "Числовая ошибка  1. Номер узла > 100000\n";
    }
    if ((unom[i] < 0.1) || (unom[i] > 1500)) {
      right = false;
     fout << "Числовая ошибка 2. U ном. велико или мало\n";
    }
  }
  for (j = 1; j <= m; j++) {
    if (abs(x[j]) > 999999) {
      right = false;
      fout << "Числовая ошибка 3. Х Ветви > 999999\n";
      fout << "Ветвь " << nm[1][j] << ' '
           << nm[2][j] << endl;
    }
    if ((by[j] < 0) || (by[j] > 99999)) {
      right = false;
      fout << "Числовая ошибка 4. Неверное значение проводимости\n";
      fout << "Ветвь " << nm[1][j] << ' '
           << nm[2][j] << endl;
    }
    // Ошибки в схеме сети
    if ((abs(kt[j] - 1) > 0.1) && ((nm[1][j] == nn[0])
      || (nm[2][j] == nn[0]))) {
      right = false;
      fout << "Ошибка схемы сети 1. \n";
      fout << "Ветвь " << nm[1][j] << ' '
           << nm[2][j] << " не может быть трансформатором" << endl;
    }
  }
  if (!right) allright = false;// числовые ошибки

  if (!allright){fout.close(); return false;} // какие-либо ошибки

  // nm1 creation
  for (i = 1; i <= m; i++) {
    if (nm[1][i] == nn[0]) nm1[1][i] = 0;
    if (nm[2][i] == nn[0]) nm1[2][i] = 0;

    for (j = 1; j <= n; j++) {
      if (nm[1][i] == nn[j]) nm1[1][i] = j;
      if (nm[2][i] == nn[j]) nm1[2][i] = j;
    }
  }
//  deb();
  // kt manipulations: missing
  // Changing of gy, by, g, b
  for (i = 1; i <= m; i++) {
    gy[i] *= 1e-6;
    by[i] *= 1e-6;
  /* if ((abs(kt[i] - 1) > 0.01) && (nk[nm1[1][i]] == 1))
      unom[nm1[1][i]] = unom[nm1[2][i]] * kt[i]; */
  }
  for (i = 0; i <= n; i++) {
    g[i] *= 1e-6;
    b[i] *= 1e-6;
  }
    fout.close();
  //  deb();
  return allright;
}
//***************************************************************
 //********************************************************************
// Преобразование файла - "удвоение пробелов"-----Мартышкин труд!!!!!!!!
// Но стоит оставить.
/*
void DoubleSpase(AnsiString OldFileName,AnsiString NewFileName)
{
  TStringList *List= new TStringList;
  try {
  List->LoadFromFile(OldFileName);}
  catch(...)
  {ShowMessage ("Файл \""+OldFileName+"\" не найден"); return;}
   //Преобразование листа со стоками AnsiString
    int i=0,k=1,j=1,leng;
      const AnsiString space=' ';
   for (;i<(List->Count);i++)
   {
        //CharToOem(List->Strings[i],List->Strings[i]);
        leng=(List->Strings[i]).Length();
    for (j=1;j<=leng;j++)
    {

      if (List->Strings[i][j]==' ')
         {List->Strings[i]=List->Strings[i].Insert(space,j);j++;leng++;
         continue;};
      if (List->Strings[i][j]=='.')
         {
         if ((k+j)>(leng))break;
         for (int n=1;(List->Strings[i][j+n])!=' ';n++){k++;
         if ((k+j)>(leng)){break;} };j=j+k;
          List->Strings[i]=List->Strings[i].Insert(space+space,j);j++;j++;k=1;leng=leng+2;
          continue;};
      if (List->Strings[i][j]=='-')
         {
         if ((j-1)==0)
         {List->Strings[i]=List->Strings[i].Insert(space,j);j++;leng++;continue;}
         else {
         for(int n=-1;(List->Strings[i][j+n])!=' ';n--){if((j+n)==0){k=-n-1;break;}k++; };

         List->Strings[i]=List->Strings[i].Insert(space,j-k);j++;k=1;leng++;
         continue;};}
    }
   };
  try {
   List->SaveToFile(NewFileName);}
  catch(...)
  {ShowMessage ("Файл \""+NewFileName+"\" невозможно создать"); return;}

} */
//***************************************************************
// Задание начальных приближений напряжений
void zerovalue()
{
  int i;

  for (i = 0; i <= n; i++) {
    va[i] = unom[i];
    vr[i] = 0;
  }
}
//***************************************************************
void ylfl()
{
  int i,j;
  float c;

  for (i = 1; i <= m; i++) {
    c = r[i] * r[i] + x[i] * x[i];
    gr[i] = r[i] / c;
    bx[i] = -x[i] / c;
  }
  for (i = 0; i <= n; i++) {
    gg[i] = g[i];
    bb[i] = b[i];
  }
  //deb();
  for (j = 1; j <= m; j++) {
    i = nm1[1][j];
    gg[i] = gg[i] + gr[j] / (kt[j] * kt[j]) + gy[j] / 2.;
    bb[i] = bb[i] + bx[j] / (kt[j] * kt[j]) + by[j] / 2.;
    i = nm1[2][j];
    gg[i] = gg[i] + gr[j] + gy[j] / 2.;
    bb[i] = bb[i] + bx[j] + by[j] / 2.;
//  cout << i << " " << bb[i] << " " << bx[j] << " "
//       << kt[j] << " " << by[j] << endl;          getch();
  }
 //deb();
}
//***************************************************************
float func(int count)
{
  int j,i1,i2;
  float w,h,pp;

  for (i = 0; i <= n; i++) {
    ja[i] = gg[i] * va[i] - bb[i] * vr[i];
    jr[i] = bb[i] * va[i] + gg[i] * vr[i];
//    cout << nn[i] << " " << ja[i] << " " << jr[i] <<endl; getch();
  }
  for (j = 1; j <= m; j++) {
    i1 = nm1[1][j]; i2 = nm1[2][j];
    ja[i1] = ja[i1] - (gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
    jr[i1] = jr[i1] - (bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
    ja[i2] = ja[i2] - (gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
    jr[i2] = jr[i2] - (bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
//    if (jr[i1] > 1000) cout << gr[j] << va[i2]
//    << bx[j] << vr[i2] << kt[j] << endl;
  }
  w = 0;
  for (i = 0; i <= n; i++) {
    p[i] = va[i] * ja[i] + vr[i] * jr[i];
    q[i] = vr[i] * ja[i] - va[i] * jr[i];
    if (i > 0) {
      ds[2*i] = -(p[i] + p0[i]);
      if (nk[i] == 1) {ds[2*i-1] = -(q[i] + q0[i]);
     if (fabs(q0[i])<1){h=ds[2*i-1];} else {h=ds[2*i-1]/q0[i];} }
      else {ds[2*i-1] = -(va[i] * va[i] + vr[i] * vr[i]
                       -unom[i] * unom[i]) / unom[i];h=ds[2*i-1]/unom[i];}
    if (fabs(p0[i])<1){pp=ds[2*i];} else {pp=ds[2*i]/p0[i];}
    w = w + pp*pp+ h*h;
    }
  }
  w = sqrt(w / (2 * n));
  return w;
}
//***************************************************************
void jacoby()
{
  int i,j,i1,i2,j1,j2,j3,j4;     // n is the independet buses + 1

  for (i = 1; i <= 2 * n; i++)
    for (j = 1; j <= 2 * n; j++)
      A[i][j] = 0;
  for (i = 1; i <= n; i++) {
    if (nk[i] == 1) {
      A[2*i-1][2*i-1] = -bb[i] * va[i] + gg[i] * vr[i] - jr[i];
      A[2*i-1][2*i] = -gg[i] * va[i] - bb[i] * vr[i] + ja[i];
    }
    else {
      A[2*i-1][2*i-1] = 2 * va[i] / unom[i];
      A[2*i-1][2*i] = 2 * vr[i] / unom[i];
    }
    A[2*i][2*i-1] = gg[i] * va[i] + bb[i] * vr[i] + ja[i];
    A[2*i][2*i] = -bb[i] * va[i] + gg[i] * vr[i] + jr[i];
  }
  for (j = 1; j <= m; j++) {
    i1 = nm1[1][j]; i2 = nm1[2][j];
    j1 = 2 * i1 - 1; j2 = 2 * i1;
    j3 = 2 * i2 - 1; j4 = 2 * i2;
    if ((i1 != 0) && (i2 != 0)) {
      if (nk[i1] == 1) {
        A[j1][j3] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
        A[j1][j4] = -(-gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
      }
      A[j2][j3] = -(gr[j] * va[i1] + bx[j] * vr[i1]) / kt[j];
      A[j2][j4] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
      if (nk[i2] == 1) {
        A[j3][j1] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
        A[j3][j2] = -(-gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
      }
      A[j4][j1] = -(gr[j] * va[i2] + bx[j] * vr[i2]) / kt[j];
      A[j4][j2] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
    }
  }
}
//***************************************************************
void gauss()
{
  float c,d,s;
  int i,j,k,l;
  float u[2 * inn];

  for (i = 1; i <= 2 * n - 1; i++) {
    c = A[i][i];
    if (abs(c) < 0.0000001) c = 0.0000001;
    for (j = i + 1; j <= 2 * n; j++) if (fabs(A[i][j])>0.0000001) A[i][j] = A[i][j] / c;
    ds[i] = ds[i] / c;
    for (k = i + 1; k <= 2 * n; k++) {
      d = A[k][i];
      if (fabs(d)>0.0000001) {
        for (l = i + 1; l <= 2 * n; l++)
          if (fabs(A[i][l])>0.0000001) A[k][l] = A[k][l] - A[i][l] * d;
        ds[k] = ds[k] - ds[i] * d;
      }
    }
  }
  u[2*n] = ds[2*n] / A[2*n][2*n];
  for (k = 2 * n - 1; k >= 1; k--) {
    s = 0;
    for (j = k + 1; j <= 2 * n; j++) if (fabs(A[k][j])>0.0000001) s = s + A[k][j] * u[j];
    u[k] = ds[k] - s;
  }
/*  for (i =1; i <= n; i++) {
    cout << setw(9) << setprecision(3) << ds[2*i-1]
         << setw(9) << setprecision(3) << ds[2*i]
         << setw(9) << setprecision(3) << u[2*i-1]
         << setw(9) << setprecision(3) << u[2*i];
    cout << endl;
  }   */
  for (i = 1; i <= n; i++) {
    va[i] = va[i] + u[2*i-1];
    vr[i] = vr[i] + u[2*i];
  }

}
//***************************************************************
bool load()
{
  int i,j,i1,i2,k,trn[10],nuzlN[10][13],nuzlK[10][13],linc[10][10],sv[10];
  float sp=0,sq=0,spg=0,sqb=0
       ,pg,qb,mv,dv,i1a,i1r,i2a,i2r,p12,q12,p21,q21,dPsum=0 ,
       saldo[2][10],sumpot[10],sumoll=0,SumTrPot;

    for (i=0;i<10;i++){saldo [0][i]=0;sv[i]=0;saldo [1][i]=0;sumpot[i]=0;trn[i]=0;
    for (int b=0;b<10;b++){linc[i][b]=0;};for (int b=0;b<13;b++)
    {nuzlK[i][b]=0;nuzlN[i][b]=0;};};

  ofstream net2("network.rez"); // создание файла для записи (ввода)
  if(!net2)
  {
  ShowMessage(
  AnsiString("Невозможно открыть файл для хранения результатов.") +
   "Проверте не ограничены ли ваши права, или имеется ли свободное"+
    "место в директории программы.\n"+
    "Попробуйте перезапустить расчет потокораспределения");
  //cout << "I can not open file\n";
    return 1;
  }

  net2.setf(ios::fixed | ios::showpoint);
  ofstream raipot("network.rip"); // создание файла для записи (ввода)
  if(!raipot)
  {
  ShowMessage(
  AnsiString("Невозможно открыть файл network.rip.")+
   "Проверте не ограничены ли ваши права, или имеется ли свободное"+
    "место в директории программы.\n"+
   "Попробуйте перезапустить расчет потокораспределения");
  // cout << "I can not open file\n";
  return 1;
  }
  raipot.setf(ios::fixed | ios::showpoint);
  //cout << "          Результаты расчета по узлам\n";
  net2 << "\t"<< "          Результаты расчета по узлам\n";
 // cout << "    N        V         dV        P         Q";
  //cout << "         Pg        Qb\n";
  net2 << "    N        V         dV         P         Q";
  net2 << "        Pg       Qb\n";
  for (i = 0; i <= n; i++) {
    mv = va[i] * va[i] + vr[i] * vr[i];
    dv = atan(vr[i] / va[i]) * 57.295779515;
    pg = mv * g[i];
    qb = -mv * b[i];
    sp += p[i];
    sq += q[i];
    spg += pg;
    sqb += qb;
    mv = sqrt(mv);

    net2 << setw(5) << nn[i]
         << setw(10) << setprecision(2) << mv
         << setw(10) << setprecision(2) << dv
         << setw(10) << setprecision(2) << -p[i]
         << setw(10) << setprecision(2) << -q[i]
         << setw(10) << setprecision(2) << pg
         << setw(10) << setprecision(2) << qb
         << endl;
  }

  net2 << "---------------------------------------------------\n";
  net2 << "Баланс пассивных элементов ";
  net2 << setw(10) << setprecision(2) << sp
       << setw(10) << setprecision(2) << sq
       << setw(10) << setprecision(2) << spg
       << setw(10) << setprecision(2) << sqb
       << endl;
  net2 << "                         + потребление, - генерация";
  net2 << " \n";
  net2 << " \n";
  net2 << "                   Результаты расчета по ветвям\n";
  net2 << "   N1   N1       P12       Q12       P21       Q21       dP\n";

ofstream tok("network.tok");
  if(!tok)
  {
  ShowMessage(
  AnsiString("Невозможно открыть файл network.tok.")+
   "Проверте не ограничены ли ваши права, или имеется ли свободное"+
    "место в директории программы.\n"+
   "Попробуйте перезапустить расчет потокораспределения");
  return 1;
  }
    tok.setf(ios::fixed | ios::showpoint);
  //tok << "   Составляющие токов по ветвям \n";
  //tok << "   N1   N1      Iа       Iр     Активное сопротивление\n";
  // TDateTime FistTime(Now());
  for (j = 1; j <= m; j++) {
    i1 = nm1[1][j]; i2 = nm1[2][j];
// Эти токи здесь не используются
//    ia = ((va[i1] - va[i2]) * gr[j] - (vr[i1] - vr[i2])) / kt[j];
//    ir = ((vr[i1] - vr[i2]) * gr[j] + (va[i1] - va[i2])) / kt[j];
    i1a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] / kt[j]
         -(va[i2] * gr[j] - vr[i2] * bx[j]) / kt[j];
    i1r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] / kt[j]
         -(va[i2] * bx[j] + vr[i2] * gr[j]) / kt[j];
    i2a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j]
         -(va[i2] * gr[j] - vr[i2] * bx[j]);
    i2r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j]
         -(va[i2] * bx[j] + vr[i2] * gr[j]);
    mv = va[i1] * va[i1] + vr[i1] * vr[i1];
    p12 = va[i1] * i1a + vr[i1] * i1r - gy[j] * mv / 2.;
    q12 = vr[i1] * i1a - va[i1] * i1r - by[j] * mv / 2.;
    mv = va[i2] * va[i2] + vr[i2] * vr[i2];
    p21 = va[i2] * i2a + vr[i2] * i2r + gy[j] * mv / 2.;
    q21 = vr[i2] * i2a - va[i2] * i2r + by[j] * mv / 2.;
       // tok << setw(5) << nn[i1] << setw(5) << nn[i2] Это действительные номера
     // а здесь - машинные
     tok << setw(5) << nm1[1][j] << setw(5) << nm1[2][j]
         << setw(10)<< setprecision(4) << i1a
         << setw(10)<< setprecision(4) << i1r
         << setw(15)<< setprecision(3) << r[j]<< endl;
    net2 << setw(5) << nn[i1] << setw(5) << nn[i2]
         << setw(10) << setprecision(2) << -p12
         << setw(10) << setprecision(2) << -q12
         << setw(10) << setprecision(2) << p21
         << setw(10) << setprecision(2) << q21
         //   Добавление потерь
         << setw(10) << setprecision(2) << fabs(p21-p12)
         //
         << endl;
         dPsum+=fabs(p21-p12);//Cуммарные потери

    k=nn[i1]/kkk;
    while (k > 9) k/=kk;
    int krat=nn[i2]/kkk ;while (krat > 9) krat/=kk;
    if (krat==k)
    { sv[k]++;
    //rot<<nn[i1]/kkk<<nn[i2]/kkk <<endl;
    /*raipot <<k<< nn[i1]<< nn[i1]<< -p12<< -q12<< p21<< q21<<fabs(p21-p12)<<endl;*/
    sumpot[k]+=fabs(p21-p12);
    {if (((kt[j]-1)<0.001)&&((kt[j]-1)>-0.001)){
     if (unom[i1]<=8){line[k][0]+=fabs(p21-p12);linc[k][0]++;};
     if (unom[i1]>8&&unom[i1]<=15){line[k][1]+=fabs(p21-p12);linc[k][1]++;};
     if (unom[i1]>15&&unom[i1]<=28){line[k][2]+=fabs(p21-p12);linc[k][2]++;};
     if (unom[i1]>28&&unom[i1]<=70){line[k][3]+=fabs(p21-p12);linc[k][3]++;};
     if (unom[i1]>70&&unom[i1]<=140){line[k][4]+=fabs(p21-p12);linc[k][4]++;};
     if (unom[i1]>140&&unom[i1]<=270){line[k][5]+=fabs(p21-p12);linc[k][5]++;};
     if (unom[i1]>270&&unom[i1]<=430){line[k][6]+=fabs(p21-p12);linc[k][6]++;};
     if (unom[i1]>430&&unom[i1]<=600){line[k][7]+=fabs(p21-p12);linc[k][7]++;};
     if (unom[i1]>600&&unom[i1]<=900){line[k][8]+=fabs(p21-p12);linc[k][8]++;};
     if (unom[i1]>900){line[k][9]+=fabs(p21-p12);linc[k][9]++;};
     }
    else {tran[k][trn[k]]+=fabs(p21-p12);nuzlN[k][trn[k]]=nn[i1];
    nuzlK[k][trn[k]]=nn[i2];trn[k]++;};}
    }
    else {saldo[0][k]+=-p12;saldo[1][k]+=-q12;
    saldo[0][krat]+=p21;saldo[1][krat]+=q21;}
    //if (((nn[i2]/kkk)==k)||((nn[i1]/kkk)!=k)){saldo[0][k]+=p21;saldo[1][k]+=q21;};

  }
  //Если неоходимо знать время выполнения какого-либо блока программы
  /*TDateTime SecondTime(Now());
   Word data,sec,min,hour,msec,sec2,msec2;
DecodeDate(FistTime,data,data,data);
DecodeTime (FistTime,hour,min,sec,msec);
DecodeDate(SecondTime,data,data,data);
DecodeTime (SecondTime,hour,min,sec2,msec2);
ShowMessage
("Время расчета  "+IntToStr(sec2*1000+msec2-sec*1000-msec)+"  мили секунд");*/

  int ul[10]={6,10,20,35,110,220,330,500,750,1150};

 for(int i=0;i<10;i++){if (sv[i]==0)continue;
 raipot<<"Район  № "<<setw(5)<<i<< endl;
  for (int g=0;g<10;g++)
  {if(linc[i][g]!=0){raipot<<" Потери в линиях"<<endl;
  raipot<<setw(5)<<ul[g]<<" кВ"<<setw(15)<< setprecision(3)<<line[i][g]
  <<endl;};};
  if (trn[i]==0)continue;
   //Блок вывода потерь по всем трансформаторам района
  //raipot<<" Потери в трансформаторах № нач. узла   № кон. узла"<<endl;
  // for (int f=0;f<trn[i];f++){
  //raipot<<setw(8)<< setprecision(3)<< tran[i][f]<<setw(22)<<nuzlN[i][f]
  //<<setw(22)<<nuzlK[i][f]<<endl;};
  SumTrPot=0;
  for (int f=0;f<trn[i];f++) SumTrPot+=tran[i][f];
  raipot<<" Потери в трансформаторах текущего района  "
  <<setw(10)<< setprecision(3)<<SumTrPot<<endl;
  };

   raipot<<endl <<"№ района   "
   <<"       Сальдо P   "<<"        Сальдо Q  "
   <<"     Сумм. потери в районе"<<endl;
   for (k=0;k<10;k++){if (nus[k]!=0) {
   raipot <<setw(5)<<k<<setw(20)<< setprecision(2)<<saldo[0][k]
   <<setw(20)<< setprecision(2)<<saldo[1][k]<<setw(20)<< setprecision(2)
   <<sumpot[k]<< endl;sumoll+=sumpot[k];}}
   ; raipot<<" Суммарные потери в сетях районов "<<setw(25)
   << setprecision(2)<<sumoll<< endl;
   raipot.close();
     //cout<< setw(80) << setprecision(2) << dPsum << endl;
     net2<< setw(60) << setprecision(2) << dPsum << endl;
   net2.close();
   tok.close();
  //rot.close();
  return true;
}
//***************************************************************
void __fastcall TForm1::N2Click(TObject *Sender)
//int main()
{
//TDateTime FistTime(Now());
 //Обнуление для прохождения проверки при повторном расчете
 for (i = 0; i < inn; i++){nn[i]=0;nk[i]=0;}
 for (i = 0; i < imm; i++)
     {for (int j = 0; j < 3; j++) {nm[j][i]=0;nm1[j][i]=0;}}
      //in= (Memo1->Lines);

   Memo1->Lines->SaveToFile("network.cdu"); //Открытый файл переписывается
  // TDateTime SecondTime(Now());
  //Memo5->Lines->Clear();   // Чистим Экран
  Out_Str =  "Идёт расчет потокораспределения электрической сети WinLFL V.M 1.1";
  Form1->Panel1->Visible=true;Form1->Splitter1->Visible=true;
   Memo4->Clear();
  Memo4->Lines->Append(Out_Str);
    //TDateTime SecondTime(Now());
  ok = preparation();
 if (!ok)
    {
    // Все неправильно
    ShowMessage("Расчет невозможен!");
    //Out_Str =  "Расчет невозможен! ";
      //New_list ("network.err"); Form1->TabSheet3->TabVisible=true;
      //DoubleSpase("network.err","in.err");Memo3->Lines->LoadFromFile("in.err");
      Form1->TabSheet3->TabVisible=false;Form1->TabSheet2->TabVisible=false;
      Form1->TabSheet4->TabVisible=false;Form1->TabSheet5->TabVisible=false;
      Memo4->Lines->LoadFromFile("network.err");
      Memo4->ReadOnly=true;Panel1->Show();
      //Form1->Panel1->Visible=false;
      return;
      //TDateTime SecondTime(Now());
      //iFileHandle = FileOpen("network.err", fmOpenRead);
      //iFileLength = FileSeek(iFileHandle,0,2);
      //FileSeek(iFileHandle,0,0);
      //pszBuffer = new char[iFileLength];
      //iBytesRead = FileRead(iFileHandle, pszBuffer, iFileLength);
     // FileClose(iFileHandle);
      //Memo1->Lines->Append(pszBuffer);
     // delete [] pszBuffer;
      }
       //Out_Str =  "В данных ошибок не обнаружено";
        //Memo1->Lines->Append(Out_Str);
  ofstream fout("network.out");
    // ostringstream fout (out);
     if (!fout) {
      ShowMessage("Невозможно открыть(создать) файл \"network.out\"");
      //fout.close();
    return ;
      }
  // fout << "File of Errors" << endl;

   // TDateTime SecondTime(Now());
  fout << "Число узлов n = " << n << "\t" << "Число Ветвей m = " << m << endl;
  fout << "     Входные данные для расчета потокораспределения\n";
  fout << "           У з л ы    с е т и \n";
  fout << " Узел   Тип  Uном        P        Q        g           b \n";

  for (i = 0; i <= n; i++)    // 0 is the slack bus
  {
// Манипуляторы ввода/вывода
// setw - минимальная ширина поля, setprecision - точность вывода
// по умолчанию точность равна 6 цифрам (после десятичной точки)
    fout << setw(5) << nn[i] << setw(5) << nk[i]
         << setw(7) <<  unom[i]
         << setw(9) <<  p0[i]
         << setw(9) <<  q0[i]
         << setw(9) <<  g[i]
         << setw(12) <<  b[i]
       //  << setw(10) << setprecision(2) << unom[i]
      // << setw(9) << setprecision(2) << p0[i]
     //<< setw(9) << setprecision(2) << q0[i]
    //     << setw(9) << setprecision(2) << g[i]
   //    << setw(9) << setprecision(2) << b[i]
         << endl;
  }
  fout << endl;
  fout << "               В е т в и    с е т и\n";
  fout << "   N1   N2        r         x            b           g       Kt \n";
  for (i = 1; i <= m; i++) {   // as lines, as transformers
    fout << setw(5) << nm[1][i] << setw(5) << nm[2][i]
         << setw(10)  << r[i]
         << setw(10)  << x[i]
         << setw(14)  << by[i]
         << setw(10)  << gy[i]
         << setw(9)  << kt[i]
      //   << setw(10) << setprecision(2) << r[i]
      //   << setw(10) << setprecision(2) << x[i]
      //   << setw(9) << setprecision(2) << gy[i]
      //   << setw(9) << setprecision(2) << by[i]
      //   << setw(9) << setprecision(2) << kt[i]
         << endl;
  }
  fout << endl;//TDateTime SecondTime(Now());
   //TDateTime FistTime(Now());
 if (ok)
  {
    // Основной итерационный цикл
    zerovalue();
    ylfl();
    count = 0;
    norm = func(count);
    while ((norm > Precision) && (count < Iteraz)) {
    jacoby();
    gauss();
    count++;
    norm = func(count);  // getch();
    fout << count<< setw(12) << setprecision(3) <<  norm << endl;
  }

    if ((count >= Iteraz)&&(norm > Precision))
     { Out_Str =
     AnsiString("За данное число итераций сходимость с заданной точностью не обеспечена.\n")+
     "Точность на последней "+IntToStr(count)+"-ой итерации составляет "+FloatToStrF(norm,ffFixed,5,6);
      //rezalt=false;
       ShowMessage(Out_Str);
   }
    //else {
      load(); rezalt=true;
      Out_Str = "Расчет закончен ";
  fout.close();
  }
     //TDateTime SecondTime(Now());
    Form1->Panel1->Visible=false;Form1->Splitter1->Visible=false;
      //Вывод на экран в Memo1
    // Сначала отладка и ошибки
// TDateTime FistTime(Now());

   /* TDateTime SecondTime(Now());
Word data,sec,min,hour,msec,sec2,msec2;
DecodeDate(FistTime,data,data,data);
DecodeTime (FistTime,hour,min,sec,msec);
DecodeDate(SecondTime,data,data,data);
DecodeTime (SecondTime,hour,min,sec2,msec2);
ShowMessage
("Время расчета  "+IntToStr(sec2*1000+msec2-sec*1000-msec)+"  мили секунд");*/
 if (!rezalt){}
 else{

 if (ok)
     {
   // Memo1->Lines->Append(AnsiString* out);
    //DoubleSpase("network.out","in.out");
    Form1->TabSheet2->TabVisible=true;
    //Memo2->Lines->LoadFromFile("in.out");
    Memo2->Lines->LoadFromFile("network.out");
    //TDateTime SecondTime(Now());
    //New_list ("network.rez");
   Form1->TabSheet3->TabVisible=true;
   //DoubleSpase("network.rez","in.rez");
   //Memo3->Lines->LoadFromFile("in.rez");//Append(AnsiString* rez);
   Memo3->Lines->LoadFromFile("network.rez");
   if (Form1->TabSheet4->TabVisible){
   //DoubleSpase("network.rip","in.rip");
//Memo5->Lines->LoadFromFile("in.rip");
  Memo5->Lines->LoadFromFile("network.rip"); };
  if (Form1->TabSheet5->TabVisible){
   Memo6->Lines->LoadFromFile("network.tok"); };
   Form1->TabSheet3->Show();
   Form1->N13->Visible=true;
   Form1->N22->Enabled=true;
   Form1->N27->Enabled=true;
 }}
//Определения времени выполнения того или иного блока программы.
//Включается в любое место, но захват нач. и конечн. временини а также их
//обработка должны находиться на одном уровне вложенности!
/*TDateTime SecondTime(Now());
Word data,sec,min,hour,msec,sec2,msec2;
DecodeDate(FistTime,data,data,data);
DecodeTime (FistTime,hour,min,sec,msec);
DecodeDate(SecondTime,data,data,data);
DecodeTime (SecondTime,hour,min,sec2,msec2);
ShowMessage
("Время расчета  "+IntToStr(sec2*1000+msec2-sec*1000-msec)+"  мили секунд"); */
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N6Click(TObject *Sender)
{
  // ensure that all the text is cut, not just the current selection
 switch (PageControl1->ActivePageIndex){
  case 0:Memo1->CutToClipboard();break;
  case 1:Memo2->CutToClipboard();break;
  case 2:Memo3->CutToClipboard();break;
  case 3:Memo5->CutToClipboard();break;
  case 4:Memo6->CutToClipboard();break;}
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N7Click(TObject *Sender)
{
 // ensure that all the text is cut, not just the current selection
 switch (PageControl1->ActivePageIndex){
  case 0:Memo1->CopyToClipboard();break;
  case 1:Memo2->CopyToClipboard();break;
  case 2:Memo3->CopyToClipboard();break;
  case 3:Memo5->CopyToClipboard();break;
  case 4:Memo6->CopyToClipboard();break;}
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N8Click(TObject *Sender)
{
 switch (PageControl1->ActivePageIndex){
  case 0:Memo1->PasteFromClipboard();break;
  case 1:Memo2->PasteFromClipboard();break;
  case 2:Memo3->PasteFromClipboard();break;
  case 3:Memo5->PasteFromClipboard();break;
  case 4:Memo6->PasteFromClipboard();break;}

}
//---------------------------------------------------------------------------
void __fastcall TForm1::N9Click(TObject *Sender)
{
 switch (PageControl1->ActivePageIndex){
  case 0:Memo1->SelectAll();break;
  case 1:Memo2->SelectAll();break;
  case 2:Memo3->SelectAll();break;
  case 3:Memo5->SelectAll();break;
  case 4:Memo6->SelectAll();break;}
}
//---------------------------------------------------------------------------

void __fastcall TForm1::N11Click(TObject *Sender)
{
      SaveDialog1->Title = "Сохранить как:";
      if (SaveDialog1->Execute()) {
        Memo1->Lines->SaveToFile(SaveDialog1->FileName);
        Form1->TabSheet1->Caption=ExtractFileName(SaveDialog1->FileName);
        Form1->Memo1->Modified=false;
        if (sFile1==""){sFile1=SaveDialog1->FileName;}
         else {if (sFile1==OpenDialog1->FileName){goto repear;};if (sFile2==""){sFile2=SaveDialog1->FileName;}
               else {if (sFile2==OpenDialog1->FileName){goto repear;};if (sFile3==""){sFile3=SaveDialog1->FileName;}
                     else {if (sFile3==OpenDialog1->FileName){goto repear;};sFile1=sFile2;sFile2=sFile3;sFile3=SaveDialog1->FileName;}
               }
         }
        repear: 
      if (sFile1!=""){N30->Caption=sFile1;N30->Visible=true;}
      if (sFile2!=""){N31->Caption=sFile2;N31->Visible=true;}
      if (sFile3!=""){N32->Caption=sFile3;N32->Visible=true;}
      }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::N10Click(TObject *Sender)
{
 if (Form1->TabSheet1->Caption=="Без_имени(*.cdu)"){N11Click(Sender);}
 else {Memo1->Lines->SaveToFile(FILE_NAME_STR);
 Form1->Memo1->Modified=false; }
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N14Click(TObject *Sender)
{
//Shellexec "notepad.exe " & "network.rez";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N16Click(TObject *Sender)
{
 ShellExecute(Handle,NULL,"notepad.exe","network.rez",NULL,SW_RESTORE);
 if (!GetLastError)
ShowMessage("Программа блокнот не найдена");
//if (spawnlp(P_WAIT,"notepad.exe ","notepad.exe network.rez",NULL))
//ShowMessage("Программа блокнот не найдена");
}
//---------------------------------------------------------------------------

void __fastcall TForm1::N15Click(TObject *Sender)
{
 ShellExecute(Handle,NULL,"notepad.exe","network.out",NULL,SW_RESTORE);
 if (!GetLastError)
ShowMessage("Программа блокнот не найдена");

//if (spawnlp(P_WAIT,"notepad.exe ","notepad.exe network.out",NULL))
//ShowMessage("Программа блокнот не найдена");
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N12Click(TObject *Sender)
{
//SaveCDU();
if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click(Sender);if(res==IDCANCEL) return;
}
Form1->Close();
//Application->Terminate();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Form1Close(TObject *Sender, TCloseAction &Action)
{
   if (!FileExists(sFile)){
   ShowMessage(AnsiString("Файл настроек \"Loadflow.ini\" был удален или перемещен.\n")+
     "Настройки не сохранены.");return;};
 INI = new TIniFile(sFile);
 //INI->WriteString("Files","main",ParamStr(0));
 if (sFile1!="")INI->WriteString("Files","file1",sFile1);
 if (sFile2!="")INI->WriteString("Files","file2",sFile2);
 if (sFile3!="")INI->WriteString("Files","file3",sFile3);
 INI->WriteString("Parametr","Font",Memo1->Font->Name);
 INI->WriteInteger("Parametr","FontSize",Memo1->Font->Size);
 INI->WriteFloat("Parametr","precision",floor(Precision*100+0.5)/100);
 INI->WriteInteger("Parametr","iteraz",Iteraz);
 INI->WriteInteger("Parametr","Height",Form1->Height);
 INI->WriteInteger("Parametr","Width",Form1->Width);
 INI->WriteBool("Parametr","DelFile",DelFile);
 INI->UpdateFile();delete INI;
 //DeleteFile("in.cdu");DeleteFile("in.err");DeleteFile("in.out");
 //DeleteFile("in.rez"); DeleteFile("in.rip");DeleteFile("in.tok");
 if (DelFile==1){;DeleteFile("Network.err");DeleteFile("Network.out");
 DeleteFile("Network.rez");DeleteFile("Network.rip");DeleteFile("Network.tok");};
}
//---------------------------------------------------------------------
void __fastcall TForm1::Form1Create(TObject *Sender)
{

  // Form1->TabSheet2->Visible=false;
 FILE * F;
 sFile=GetCurrentDir()+"\\Loadflow.ini";
 if (FileExists(sFile))
 {INI = new TIniFile(sFile);
 Memo1->Font->Name=INI->ReadString("Parametr","Font","MS Scan Serif");
 Memo2->Font->Name=Memo1->Font->Name;Memo3->Font->Name=Memo1->Font->Name;
 Memo4->Font->Name=Memo1->Font->Name;Memo5->Font->Name=Memo1->Font->Name;
 Memo6->Font->Name=Memo1->Font->Name;
 Precision=INI->ReadFloat("Parametr","precision",0.01);
 Iteraz=INI->ReadInteger("Parametr","iteraz",10);
 Form1->Height=INI->ReadInteger("Parametr","Height",550);
 Form1->Width=INI->ReadInteger("Parametr","Width",750);
 Memo1->Font->Size=INI->ReadInteger("Parametr","FontSize",10);
 Memo2->Font->Size=Memo1->Font->Size;Memo3->Font->Size=Memo1->Font->Size;
 Memo4->Font->Size=Memo1->Font->Size;Memo5->Font->Size=Memo1->Font->Size;
 Memo6->Font->Size=Memo1->Font->Size;
 DelFile=INI->ReadBool("Parametr","DelFile",0);
 if (INI->ValueExists("Files","file1"))
 {N30->Caption=INI->ReadString("Files","file1","");N30->Visible=true;
 N28->Visible=true;N29->Visible=true;sFile1=INI->ReadString("Files","file1","");}
 if (INI->ValueExists("Files","file2"))
 {N31->Caption=INI->ReadString("Files","file2","");N31->Visible=true;
 sFile2=INI->ReadString("Files","file2","");}
 if (INI->ValueExists("Files","file3"))
 {N32->Caption=INI->ReadString("Files","file3","");N32->Visible=true;
 sFile3=INI->ReadString("Files","file3","");}
 delete INI;}
 else {if((F=fopen(sFile.c_str(),"w+"))==NULL)
 {ShowMessage("Невозможно создать файл настроек");return;} fclose(F);
 INI = new TIniFile(sFile);
 INI->WriteString("Files","main",ParamStr(0));
 //INI->WriteString("Files","file1","");
 //INI->WriteString("Files","file2","");
 //INI->WriteString("Files","file3","");
 INI->WriteString("Parametr","Font",Memo1->Font->Name);
 INI->WriteInteger("Parametr","FontSize",Memo1->Font->Size);
 INI->WriteFloat("Parametr","precision",0.01);
 INI->WriteInteger("Parametr","iteraz",10);
 INI->WriteInteger("Parametr","Height",Form1->Height);
 INI->WriteInteger("Parametr","Width",Form1->Width);
 INI->WriteBool("Parametr","DelFile",0);
 INI->UpdateFile();delete INI;
 }
}
//---------------------------------------------------------------------
//---------------------------------------------------------------------------
void __fastcall TForm1::N18Click(TObject *Sender)
{
ShowMessage("Программа расчета установившегося режима  V2.0 НГТУ 2003 г.");
}
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
void __fastcall TForm1::N22Click(TObject *Sender)
{
Form1->TabSheet4->TabVisible=true;
 //DoubleSpase("network.rip","in.rip");
//Memo5->Lines->LoadFromFile("in.rip");
Memo5->Lines->LoadFromFile("network.rip");
Form1->TabSheet4->Show();
}
//---------------------------------------------------------------------------
  void __fastcall TForm1::N4Click(TObject *Sender)
{
OpenDialog1->Title = "Открыть";
      if (OpenDialog1->Execute()){
        //SaveCDU();
        if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click(Sender); if(res==IDCANCEL) return;
}       Form1->Panel1->Visible=false;Form1->Splitter1->Visible=false;
        Form1->N1->Visible=true;
        Form1->N22->Enabled=false;Form1->N27->Enabled=false;
        Form1->N13->Visible=false;
        Form1->N5->Visible=true;
        Form1->N11->Visible=true;
        Form1->N10->Visible=true;
        Form1->ToolBar1->Visible=true;
        Form1->ToolButton1->Visible=true; //Обязательно показывать ,
                                       //Родительского свойства недостаточно!?
        Form1->PageControl1->Visible=true;
         Form1->Memo1->Modified=false;Form1->Memo2->Modified=false;
     Form1->Memo3->Modified=false;Form1->Memo4->Modified=false;
     Form1->Memo5->Modified=false;Form1->Memo6->Modified=false;
        //DoubleSpase(OpenDialog1->FileName,"in.cdu");
        //Memo1->Lines->LoadFromFile("in.cdu");
        Memo1->Lines->LoadFromFile(OpenDialog1->FileName);
        Form1->TabSheet2->TabVisible=false;Form1->TabSheet3->TabVisible=false;
         Form1->TabSheet4->TabVisible=false;Form1->TabSheet5->TabVisible=false;
        Form1->TabSheet1->Visible=false;Form1->TabSheet1->Show();
        FILE_NAME_STR= OpenDialog1->FileName;
        Form1->TabSheet1->Caption=ExtractFileName(FILE_NAME_STR);
         if (sFile1==""){sFile1=OpenDialog1->FileName;}
         else {if (sFile1==OpenDialog1->FileName){goto repear;};if (sFile2==""){sFile2=OpenDialog1->FileName;}
               else {if (sFile2==OpenDialog1->FileName){goto repear;};if (sFile3==""){sFile3=OpenDialog1->FileName;}
                     else {if (sFile3==OpenDialog1->FileName){goto repear;};sFile1=sFile2;sFile2=sFile3;sFile3=OpenDialog1->FileName;}
               }
         }
      repear:
      if (sFile1!=""){N30->Caption=sFile1;N30->Visible=true;}
      if (sFile2!=""){N31->Caption=sFile2;N31->Visible=true;}
      if (sFile3!=""){N32->Caption=sFile3;N32->Visible=true;}
      }
}      
//---------------------------------------------------------------------------
void __fastcall TForm1::N25Click(TObject *Sender)
{
 Form2->Edit1->Text=FloatToStrF(Precision,ffFixed,3,2);
 Form2->Edit2->Text=IntToStr(Iteraz);
 Form2->Edit1->Modified=false;
 Form2->Edit2->Modified=false;Form2->BitBtn1->Enabled=false;
 Form2->CheckBox1->Checked=DelFile;
  Form2->ShowModal();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N26Click(TObject *Sender)
{
Form1->Panel1->Enabled=false;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N27Click(TObject *Sender)
//Разделение потерь
{
 int i1,i2;
Form1->TabSheet5->TabVisible=true;

// Memo6->Lines->LoadFromFile("network.tok");

Form1->TabSheet5->Show();
Memo6;

L = n + 1;
if (L >30)
    {ShowMessage("Число узлов >30. Расчет разделения потерь невозможен");
      return;
            }
  ifstream net1("network.tok"); // открытие файла для чтения (ввода)
  if(!net1)
  {
  Out_Str =  "Невозможно открыть файл\n";
  Memo6->Lines->Append(Out_Str);
  }
               L = m;     //число ветвей
               m = n + 1;      // узлов

           for (i = 1; i <= L; i++)
       {
        net1  >> na[i] >> ka[i] >> ta[i]>> tr[i] >> rd[i];
        }
              for (i = 1; i <= L; i++)
       {
         if (na[i]==0) na[i] = m;
         nr[i] = na[i];
         kr[i] = ka[i];
        }
   //  А вот эти токи надо посчитать
  //    for (j = 1; j <= m; j++)
  //     {
  //      net1  >> tza[j] >>  tzr[j];
  //      }
   net1.close();
   // вычисление задающих токов
   for (j = 1; j <= m; j++)
       {
        tza[j] = 0;
        tzr[j] = 0;
      for (i = 1; i <= L; i++)
       {
         if (na[i]==j)
           {   tza[j] =   tza[j] -   ta[i] ;
               tzr[j] =   tzr[j] -   tr[i] ;
             }
         if (ka[i]==j)
           {   tza[j] =   tza[j] +   ta[i] ;
               tzr[j] =   tzr[j] +   tr[i] ;
             }

        }
        }

   ofstream net2("losses.rez"); // создание файла для записи (вывода)
  if(!net2)
  { Out_Str = "I can not open file losses.rez";
     }
  net2.setf(ios::fixed | ios::showpoint);
  net2 <<  "  Разделение потерь мощности электрической сети  \n";
  net2 <<  "  Входные данные для расчета \n";
  net2 << "  Число узлов  = " << m << "\t" << "Число Ветвей  = " << L << endl;
  net2 <<  "         Токи ветвей \n ";
  net2 <<  " Ветвь Нач.   Кон.    Ток Ak   Ток Re    R  \n" ;
  for (i = 1; i <= L; i++)
  {
      i1 = nm1[1][i]; i2 = nm1[2][i];
      net2 << setw(10) <<   setprecision(2)
         << nn[i1] << setw(10) << setprecision(2)
         << nn[i2] << setw(10) << setprecision(2)
         << ta[i] << setw(10) << setprecision(2)
         << tr[i] << setw(10) << setprecision(2)
         << rd[i] << setw(10) << setprecision(2)
         << endl;
          }
  // Реакт ток не выводим
  //net2 <<  "          Реакт. ток ветви \n ";
  //net2 <<  " Ветвь       Нач.   Кон.    Ток \n" ;
  //for (i = 1; i <= L; i++)
  //{
  //  net2 << setw(10) <<  i << setw(10) << setprecision(2)
  //       << nr[i] << setw(10) << setprecision(2)
  //       << kr[i] << setw(10) << setprecision(2)
  //       << tr[i] << setw(10) << setprecision(2)<< endl;
  //               }
     net2  << "       Задающие токи узлов  \n";
     net2  << "     Узел            ТЗа     ТЗр  \n";
   for (j = 1; j <= m; j++)
  {
     if (nn[j] == 0)nn[j]=nn[0];
      net2  << setw(10) << nn[j] << setw(10) << setprecision(2)
         << tza[j]<< setw(10)  << setprecision(2)
         << tzr[j] << setw(10) << setprecision(2)<< endl;
          }
    for (i = 1; i <= L; i++)
    {       for (j = 1; j <= m; j++)
       { aa[i][j]=0;
          a[i][j]=0;
           ar[i][j]=0;
             }  }
   // * * * Alfa perwyj
      for (j = 1; j <= m; j++)
      {        sia =  tza[j];
              for (i = 1; i <= L; i++)
               {
                if (na[i] == j)
                 {
                 sia = sia + ta[i];
                  }
               }

                  for (i = 1; i <= L; i++)
               {
                if (ka[i] == j)
                 {
                 a[i][j] =  ta[i]/sia;
                  }  } }

   // -- REACTIV
    for (j = 1; j <= m; j++)
      {
       sia =  tzr[j];
              for (i = 1; i <= L; i++)
               {
                if (nr[i] == j)
                {
                sia = sia + tr[i];
                } }

                for (i = 1; i <= L; i++)
               {
                if (kr[i] == j)
                {
                 ar[i][j] =  tr[i]/sia;
               }  }
        }



//  * * * Wtoroj Alfa
 //      kx = 0;  это мы выбросили
       for (i = 1; i <= L; i++)
     {
        N[i] = na [i];
        k[i] = ka [i];
       for (j = 1; j <= m; j++)
       {
        aa[i][j] = a[i][j];
         }
       }

   alpha();
  // Перекачиваем все обратно в а, чтобы освободить место
  // для реактивных составляющих
     for (i = 1; i <= L; i++)
       {
             for (j = 1; j <= m; j++)
               {
                 a[i][j]=aa[i][j];
                }
        }
     // готовим  реактивные составляющие
        for (i = 1; i <= L; i++)
     {
        N[i] = nr [i];
        k[i] = kr [i];
       for (j = 1; j <= m; j++)
       {
        aa[i][j] = ar[i][j];
         }
       }
     alpha();

  // Перекачиваем все обратно
       for (i = 1; i <= L; i++)
       {
             for (j = 1; j <= m; j++)
               {
                 ar[i][j]=aa[i][j];
                }
        }



    net2 << endl;
    net2 <<  "              Результаты расчетов  \n" ;

    //net2 <<  "              Матрица Токораспределения АА(i,j) - акт.   \n" ;
    //net2 << "                       Узлы   \n" ;
    //net2 << "Ветви";
    //      for (j = 1; j <= m; j++) // Пока только заголовок
    //     {    net2 <<  setw(6) <<  j ;
    //       }
    //    net2 << endl;
   //    Вывод матрицы
   // for (i = 1; i <= L; i++)      // Do 64
    // {
      // net2 << na[i]<< setw(2) << ka[i] ;
      //     for (j = 1; j <= m; j++) // Print 65
       //    {

    //
      //       net2  << setw(7)  << setprecision(2)<< a[i][j];

       //     }
     //  net2 << endl;
    //  }
      // Здесь вывести матрицу реактивных коэффициентов

  //    net2 <<  "              Матрица Токораспределения АR(i,j) - реакт.   \n" ;
  //  net2 << "                       Узлы   \n" ;
  //  net2 << "Ветви";
    //      for (j = 1; j <= m; j++) // Пока только заголовок
    //     {    net2 <<  setw(6) <<  j ;
     //      }
   //     net2 << endl;
   //    Вывод матрицы
   // for (i = 1; i <= L; i++)      // Do 64
   //  {
   //    net2 << na[i]<< setw(2) << ka[i] ;
   //        for (j = 1; j <= m; j++) // Print 65
   //        {

   //          net2  << setw(7)  << setprecision(2)<< ar[i][j];

   //         }
    //   net2 << endl;
    //  }
 // Пересчитываем матрицы Т в действительные токи. Для этого
//   задающий ток умножаем на элемент марицы.
//     net2 <<  "              Матрица  токов - акт. составляющая   \n" ;
//    net2 << "                       Узлы   \n" ;
//    net2 << "Ветви";
//          for (j = 1; j <= m; j++) // Пока только заголовок
//         {    net2 <<  setw(7) <<  j ;
//           }
//        net2 << endl;
   //    Вывод матрицы
    for (i = 1; i <= L; i++)      // Do 64
     {
//       net2 << na[i]<< setw(2) << ka[i] ;
           for (j = 1; j <= m; j++) // Print 65
           {
             a[i][j] = tza[j] * a[i][j];
//             net2  << setw(7)  << setprecision(2)<< a[i][j];

            }
//       net2 << endl;
      }

//     net2 <<  "              Матрица  токов - реакт. составляющая   \n" ;
//    net2 << "                       Узлы   \n" ;
//    net2 << "Ветви";
//          for (j = 1; j <= m; j++) // Пока только заголовок
//         {    net2 <<  setw(7) <<  j ;
//           }
//        net2 << endl;
   //    Вывод матрицы
    for (i = 1; i <= L; i++)      // Do 64
     {
//       net2 << na[i]<< setw(2) << ka[i] ;
           for (j = 1; j <= m; j++) // Print 65
           {
             ar[i][j] = tzr[j] * ar[i][j];
//             net2  << setw(7)  << setprecision(2)<< ar[i][j];

            }
 //      net2 << endl;
      }



  // Вычисляем токи через составляющие
 // net2 << "          Токи   \n" ;

       for (i = 1; i <= L; i++)
     {
         for (j = 1; j <= m; j++)
           {
           aa[i][j] = a[i][j]* a[i][j] + ar[i][j]* ar[i][j];
           aa[i][j] = sqrt (aa[i][j]);
   //        net2  << setw(7)  << setprecision(2)<< aa[i][j];
            }
    // Для отладки вычислим ток
       b0 =  (ta[i] * ta[i] + tr[i] * tr[i]);
       b0= sqrt (b0);
//      net2 <<"    Ток ветви"<< setw(7)  << setprecision(2)<< b0;
//       net2 << endl;
      }
 // Вычисляем вектор потерь в ветвях
 // массив  dP
 //  net2 << "    Вектор   результирующих потерь по ветвям   \n" ;
      for (i = 1; i <= L; i++)
 {  dP[i]= rd[i] * (ta[i] * ta[i] + tr[i] * tr[i]);
  //    net2  << setw(7)  << setprecision(2)<< dP[i];
            }
       net2 << endl;


    // Вычисляем сотавляющие потерь через коэф. альфа
 net2 << "          Составляющие потерь от нагрузок узлов   \n" ;
  net2 << "Ветвь    " ;
  // Заголовок для узлов
    for (j = 1; j <= m; j++)
  {
       net2  <<  nn[j] << setw(6) << setprecision(2);
   }
   net2 << "   dP\n" ;
           for (i = 1; i <= L; i++)    // Каждая ветвь
 {
  i1 = nm1[1][i]; i2 = nm1[2][i];
      net2 << setw(2) <<   setprecision(2)
         << nn[i1] << setw(4) << setprecision(2)
         << nn[i2] << setw(4) << setprecision(2);

    sia = 0; // Сумма потерь по составляющим
     for (j = 1; j <= m; j++)
        //    Теперь для каждого из узлов
     {
     //коэффициент альфа
     //Квадрат тока от "своей" составляющей
     b0 =  a[i][j] * a[i][j] + ar[i][j] * ar[i][j]; //Здесь будет сумма всех остальных составляющих
         for (j1 = 1; j1 <= L; j1++) //Опять для каждого из узлов
         {
          if (j == j1)continue;
            b0 = b0 + a[i][j] * a[i][j1] + ar[i][j] * ar[i][j1];
          }
      aa[i][j] = rd[i] * b0;
      sia = sia + aa[i][j];
      net2  << setw(6)  << setprecision(2)<< aa[i][j];
                  }
        net2  << setw(7)  << setprecision(2)<< sia;
       net2 << endl;
      }

       net2 << endl;
// Вывод результатов по ветвям
  for (i = 1; i <= L; i++)    // Каждая ветвь
 {
  net2 << "Ветвь   " ;
    i1 = nm1[1][i]; i2 = nm1[2][i];
      net2 << setw(2) <<   setprecision(2)
         << nn[i1] << "-"  << nn[i2];
   net2 << "  Потери  " << dP[i] ;
   net2 << endl;
   net2 << "Составляющие " ;
   for (j = 1; j <= m; j++)
    {
     if (aa[i][j]==0)continue;
    net2 << " dP" << nn[j]<<"="<<aa[i][j];
     }
    net2 << endl;
 }
 net2 << endl;
 // Вывод результатов долей транзитных потерь по узлам
    net2 << "          Доля транзитных потерь от нагрузок узлов   \n" ;
 net2 << "Узлы    " ;
  // Заголовок для узлов
    for (j = 1; j <= m; j++)
  {
       net2  <<  nn[j] << setw(6) << setprecision(2);
   }
 net2 << endl;
  net2 << "Потери " ;
      for (j = 1; j <= m; j++)
  { sia = 0;
         for (i = 1; i <= L; i++)    // Каждая ветвь
         {
          sia = sia +  aa[i][j];

          }
      net2  << sia << setw(6) << setprecision(2);
   }
 net2 << endl;




 // ------------------------------
     net2.close();
          //Вывод на экран в Memo6
        Memo6->Lines->LoadFromFile("losses.rez");

}
//---------------------------------------------------------------------------
 //***************************************************************
void alpha()
{
 for (i = 1; i <= L; i++)
  {
   k0 = k[i];
   b0 = aa[i][k0];
      for (j1 = 1; j1 <= L; j1++)
      {
      n1 = N[j1];
      k1 = k[j1];
      if (n1 != k0) continue;
          b1 = b0 * aa[j1][k1];
          aa[i][k1] = aa[i][k1] + b1;
          for (j2 = 1; j2 <= L; j2++)
         {
          n2= N[j2];
          k2=k[j2];
          if (n2 != k1) continue;
          b2 = b1 * aa[j2][k2];
          aa[i][k2] = aa[i][k2] + b2;

          for (j3 = 1; j3 <= L; j3++)
         {
          n3= N[j3];
          k3=k[j3];
          if (n3 != k2) continue;
          b3 = b2 * aa[j3][k3];
          aa[i][k3] = aa[i][k3] + b3;
            for (j4 = 1; j4 <= L; j4++)
           {n4= N[j4];
            k4=k[j4];
            if (n4 != k3) continue;
            b4 = b3 * aa[j4][k4];
            aa[i][k4] = aa[i][k4] + b4;
              for (j5 = 1; j5 <= L; j5++)
              {n5= N[j5];
               k5=k[j5];
              if (n5 != k4) continue;
              b5 = b4 * aa[j5][k5];
              aa[i][k5] = aa[i][k5] + b5;
                for (j6 = 1; j6 <= L; j6++)
                {n6= N[j6];
                k6=k[j6];
                if (n6 != k5) continue;
                b6 = b5 * aa[j6][k6];
                aa[i][k6] = aa[i][k6] + b6;
                  for (j7 = 1; j7 <= L; j7++)
                  { n7= N[j7];
                  k7=k[j7];
                  if (n7 != k7) continue;
                  b7 = b6 * aa[j7][k7];
                  aa[i][k7] = aa[i][k7] + b7;
                     for (j8 = 1; j8 <= L; j8++)
                     {n8= N[j8];
                      k8=k[j8];
                     if (n8 != k8) continue;
                     b8 = b7 * aa[j8][k8];
                     aa[i][k8] = aa[i][k8] + b8;
                        for (j9 = 1; j9 <= L; j9++)
                       {n9= N[j9];
                        k9=k[j9];
                        if (n9 != k9) continue;
                        b9 = b8 * aa[j9][k9];
                        aa[i][k9] = aa[i][k9] + b9;
                          for (j10 = 1; j10 <= L; j10++)
                          {n10= N[j10];
                          k10=k[j10];
                          if (n10 != k10) continue;
                          b10 = b9 * aa[j10][k10];
                          aa[i][k10] = aa[i][k10] + b10;
            }  } } } } }  } }   }
      }  // 11 continue
  }      // 9 continue


}

//---------------------------------------------------------------------------
void __fastcall TForm1::N28Click(TObject *Sender)
{
 Form1->Panel1->Enabled=true;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N3Click(TObject *Sender)
{
//SaveCDU();
if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click(Sender);if(res==IDCANCEL) return;
}
 if (Form1->TabSheet3->TabVisible==true)
  {if (Application->MessageBox("Вы действительно хотите задать новые исходные данные?\n"
  "При этом вы потеряете результаты текущего расчета.",
 "Открытие нового окна",MB_YESNO+MB_ICONQUESTION)!=IDYES)return;}
       Form1->N11->Visible=true;Form1->Panel1->Visible=false;
        Form1->N10->Visible=true;Form1->Splitter1->Visible=false;
 Form1->N1->Visible=true;Form1->N22->Enabled=false;Form1->N27->Enabled=false;
        Form1->N13->Visible=false;
        Form1->N5->Visible=true;
        Form1->ToolBar1->Visible=true;
        Form1->ToolButton1->Visible=true;
 Form1->PageControl1->Visible=true;
 Form1->TabSheet1->Visible=false;
 Form1->TabSheet2->TabVisible=false;Form1->TabSheet3->TabVisible=false;
 Form1->TabSheet4->TabVisible=false;Form1->TabSheet5->TabVisible=false;
    Form1->Memo1->Modified=false;Form1->Memo2->Modified=false;
     Form1->Memo3->Modified=false;Form1->Memo4->Modified=false;
     Form1->Memo5->Modified=false;Form1->Memo6->Modified=false;
  Form1->Memo1->Clear();
  Form1->TabSheet1->Caption="Без_имени(*.cdu)";
  Form1->TabSheet1->Show();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N110Click(TObject *Sender)
{
 Form1->TabSheet1->Visible=true;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBar1(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
  Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo1->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo1->CaretPos.x+1);
 if (Form1->Memo1->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBar2(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
   Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo1->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo1->CaretPos.x+1);
 if (Form1->Memo1->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBar3(TObject *Sender)
{
Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo1->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo1->CaretPos.x+1);
 if (Form1->Memo1->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBar4(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
   Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo3->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo3->CaretPos.x+1);
 if (Form1->Memo3->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBar5(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
    Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo3->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo3->CaretPos.x+1);
 if (Form1->Memo3->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBa36(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo2->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo2->CaretPos.x+1);
 if (Form1->Memo2->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::StatBa37(TObject *Sender, WORD &Key,
      TShiftState Shift)
{Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo2->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo2->CaretPos.x+1);
 if (Form1->Memo2->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Stat15(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo5->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo5->CaretPos.x+1);
 if (Form1->Memo5->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Stat25(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo5->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo5->CaretPos.x+1);
 if (Form1->Memo5->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N30Click(TObject *Sender)
{
//SaveCDU();
if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click(Sender);if(res==IDCANCEL) return;
}
 Form1->N1->Visible=true;Form1->Panel1->Visible=false;Form1->Splitter1->Visible=false;
     Form1->N22->Enabled=false;Form1->N27->Enabled=false;
     Form1->N13->Visible=false;
     Form1->N5->Visible=true;
     Form1->N11->Visible=true;
     Form1->N10->Visible=true;
     Form1->ToolBar1->Visible=true;
     Form1->ToolButton1->Visible=true; //Обязательно показывать ,
                                    //Родительского свойства недостаточно!?
     Form1->PageControl1->Visible=true;
     Form1->Memo1->Modified=false;Form1->Memo2->Modified=false;
     Form1->Memo3->Modified=false;Form1->Memo4->Modified=false;
     Form1->Memo5->Modified=false;Form1->Memo6->Modified=false;
     //DoubleSpase(sFile1,"in.cdu");
     //Memo1->Lines->LoadFromFile("in.cdu");
     Memo1->Lines->LoadFromFile(sFile1);
     Form1->TabSheet2->TabVisible=false;Form1->TabSheet3->TabVisible=false;
     Form1->TabSheet4->TabVisible=false;Form1->TabSheet5->TabVisible=false;
     Form1->TabSheet1->Visible=false;Form1->TabSheet1->Show();
     FILE_NAME_STR= sFile1;
     Form1->TabSheet1->Caption=ExtractFileName(FILE_NAME_STR);
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N31Click(TObject *Sender)
{
//SaveCDU();
if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click(Sender);if(res==IDCANCEL) return;
}
Form1->N1->Visible=true;Form1->Panel1->Visible=false;Form1->Splitter1->Visible=false;
     Form1->N22->Enabled=false;Form1->N27->Enabled=false;
     Form1->N13->Visible=false;
     Form1->N5->Visible=true;
     Form1->N11->Visible=true;
     Form1->N10->Visible=true;
     Form1->ToolBar1->Visible=true;
     Form1->ToolButton1->Visible=true; //Обязательно показывать ,
                                    //Родительского свойства недостаточно!?
     Form1->PageControl1->Visible=true;
     Form1->Memo1->Modified=false;Form1->Memo2->Modified=false;
     Form1->Memo3->Modified=false;Form1->Memo4->Modified=false;
     Form1->Memo5->Modified=false;Form1->Memo6->Modified=false;
     //DoubleSpase(sFile2,"in.cdu");
     //Memo1->Lines->LoadFromFile("in.cdu");
     Memo1->Lines->LoadFromFile(sFile2);
     Form1->TabSheet2->TabVisible=false;Form1->TabSheet3->TabVisible=false;
     Form1->TabSheet4->TabVisible=false;Form1->TabSheet5->TabVisible=false;
     Form1->TabSheet1->Visible=false;Form1->TabSheet1->Show();
     FILE_NAME_STR= sFile2;
     Form1->TabSheet1->Caption=ExtractFileName(FILE_NAME_STR);
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N32Click(TObject *Sender)
{
//SaveCDU();
if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click(Sender);if(res==IDCANCEL) return;
}
Form1->N1->Visible=true;Form1->Panel1->Visible=false;
Form1->Splitter1->Visible=false;
     Form1->N22->Enabled=false;Form1->N27->Enabled=false;
     Form1->N13->Visible=false;
     Form1->N5->Visible=true;
     Form1->N11->Visible=true;
     Form1->N10->Visible=true;
     Form1->ToolBar1->Visible=true;
     Form1->ToolButton1->Visible=true; //Обязательно показывать ,
                                    //Родительского свойства недостаточно!?
     Form1->PageControl1->Visible=true;
     Form1->Memo1->Modified=false;Form1->Memo2->Modified=false;
     Form1->Memo3->Modified=false;Form1->Memo4->Modified=false;
     Form1->Memo5->Modified=false;Form1->Memo6->Modified=false;
     //DoubleSpase(sFile3,"in.cdu");
     //Memo1->Lines->LoadFromFile("in.cdu");
     Memo1->Lines->LoadFromFile(sFile3);
     Form1->TabSheet2->TabVisible=false;Form1->TabSheet3->TabVisible=false;
     Form1->TabSheet4->TabVisible=false;Form1->TabSheet5->TabVisible=false;
     Form1->TabSheet1->Visible=false;Form1->TabSheet1->Show();
     FILE_NAME_STR= sFile3;
     Form1->TabSheet1->Caption=ExtractFileName(FILE_NAME_STR);
}
//---------------------------------------------------------------------------
void __fastcall TForm1::N33Click(TObject *Sender)
{
FontDialog1->Font->Assign(Memo1->Font);
 if (FontDialog1->Execute())
 {Memo1->Font->Assign(FontDialog1->Font);Memo2->Font->Assign(FontDialog1->Font);
 Memo3->Font->Assign(FontDialog1->Font);Memo4->Font->Assign(FontDialog1->Font);
 Memo5->Font->Assign(FontDialog1->Font);Memo6->Font->Assign(FontDialog1->Font);}
}
//---------------------------------------------------------------------------
/*void SaveCDU()
{
if (Form1->Memo1->Modified)
{int res = Application->MessageBox("Сохранить изменения в файле исходных данных ?"
,"Файл изменен",MB_YESNOCANCEL+MB_ICONQUESTION) ;
if(res==IDYES) N10Click();
}}  */
//********************************************************************
void __fastcall TForm1::SMemo6(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo6->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo6->CaretPos.x+1);
 if (Form1->Memo6->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SbMemo6(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo6->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo6->CaretPos.x+1);
 if (Form1->Memo6->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::M2(TObject *Sender)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo2->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo2->CaretPos.x+1);
 if (Form1->Memo2->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::M3(TObject *Sender)
{
   Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo3->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo3->CaretPos.x+1);
 if (Form1->Memo3->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::M5(TObject *Sender)
{
 Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo5->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo5->CaretPos.x+1);
 if (Form1->Memo5->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::M6(TObject *Sender)
{
Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo6->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo6->CaretPos.x+1);
 if (Form1->Memo6->Modified)  Form1->StatusBar1->Panels->Items[1]->Text= "модиф.";
 else Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::N34Click(TObject *Sender)
{
Application->HelpContext(100);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Mem4key(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo4->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo4->CaretPos.x+1);
Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Mem4Mouse(TObject *Sender, TMouseButton Button,
      TShiftState Shift, int X, int Y)
{
Form1->StatusBar1->Panels->Items[0]->Text=IntToStr((int)Form1->Memo4->CaretPos.y+1)+": "+
 IntToStr((int)Form1->Memo4->CaretPos.x+1);
 Form1->StatusBar1->Panels->Items[1]->Text="";
}
//---------------------------------------------------------------------------


